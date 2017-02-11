using System;
using System.Windows;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Documents;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GymnasticsScoringTool_01
{

    internal enum NotifyType {
        ErrorMessage = 0,
        StatusMessage = 1,
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private static MainPage _current = null;
        internal static MainPage Current { get { return _current; } }

        private ContentDialog_MeetParameters _editMeetParameters_ContentDialog = null;
        private ContentDialog_EditTeam _editTeam_ContentDialog = null;
        private ContentDialog_TeamSelector _teamSelector_ContentDialog = null;
        private ContentDialog_TeamEventSelector _teamEventSelector_ContentDialog = null;
        private ContentDialog_EditTeamForEvent _editTeamForEvent_ContentDialog = null;
        private ContentDialog_EditTeamForEventVerification _editTeamForEventVerification_ContentDialog = null;
        private ContentDialog_GymnastSelector _gymnastSelector_ContentDialog = null;
        private ContentDialog_EditGymnast _editGymnast_ContentDialog = null;
        private ContentDialog_ConfigureDivisions _configureDivisions_ContentDialog = null;

        public static Team _currentlySelectedTeam = null;
        public static string _currentlySelectedEvent = null;
        public static int? _currentlySelectedGymnastNbr = null;
        public static Division _currentlySelectedDivision = null;

        public static List<Team> _currentlySelectedTeamsList = null; // to provide opportunity to edit gymnasts on multiple
        // teams at once: example -> "mixed group" rotation, want to enter the scores for all gymnasts on one edit screen

        private static bool? _existingMeet = null;
        private static bool? _saveTeams = false;

        public static bool _outputPlainTextOnly = false;
        public static StackPanel _publicStaticDebugSpace;

        public MainPage() {
            this.InitializeComponent();
            _current = this;

            Binding meetNameBinding = new Binding();
            meetNameBinding.Source = Meet.meetParameters;
            meetNameBinding.Path = new PropertyPath("meetName");
            BindingOperations.SetBinding(textBlock_meetName, TextBlock.TextProperty, meetNameBinding);

            Binding meetDateBinding = new Binding();
            meetDateBinding.Source = Meet.meetParameters;
            meetDateBinding.Path = new PropertyPath("meetDate");
            BindingOperations.SetBinding(textBlock_meetDate, TextBlock.TextProperty, meetDateBinding);

            Binding meetLocationBinding = new Binding();
            meetLocationBinding.Source = Meet.meetParameters;
            meetLocationBinding.Path = new PropertyPath("meetLocation");
            BindingOperations.SetBinding(textBlock_meetLocation, TextBlock.TextProperty, meetLocationBinding);

            this.DataContext = Meet.divisions;
            if(Meet.meetParameters.useDivisions) {
                comboBox_useDivisions.SelectedIndex = 1;
                comboBox_CurrentDivision.IsEnabled = true;
            }
            else {
                comboBox_useDivisions.SelectedIndex = 0;
                comboBox_CurrentDivision.IsEnabled = false;
            }

            if(_currentlySelectedDivision!=null) {
                int divIdx = Meet.divisions.IndexOf(_currentlySelectedDivision);
                comboBox_CurrentDivision.SelectedIndex = divIdx;
            }

            comboBox_useDivisions.SelectionChanged += comboBox_useDivisions_SelectionChanged;
            comboBox_CurrentDivision.SelectionChanged += comboBox_comboBox_CurrentDivision_SelectionChanged;

            Button_Divisions.FontStyle = Windows.UI.Text.FontStyle.Italic;

            _publicStaticDebugSpace = eventCompetition_box2;

        }

        protected override async void OnNavigatedTo(NavigationEventArgs e) {

            bool potentialRecovery = await FileManagement.DoesAutoRecoverFileExist();

            if (potentialRecovery) {
                await RecoverAutosavedMeetDialog();
            }

            if (!_existingMeet.HasValue) {
                await NewOrExistingMeetDialog();
            }

            if (_existingMeet.HasValue) {
                try { RefreshStandingDisplays(); }
                catch (Exception ex) { HandleRefreshStandingsException(ex); }
            }

            base.OnNavigatedTo(e);
        }


        internal void NotifyUser(string message, NotifyType type) {
            TextBlock notification = new TextBlock();
            notification.Text = "- " + message + "; NotifyType: " + type.ToString();
            notification.Margin = new Thickness(10, 0, 0, 0);
            notification.FontSize = 11;
            notification.FontStyle = Windows.UI.Text.FontStyle.Italic;
            _publicStaticDebugSpace.Children.Add(notification);
        }


        private async Task RecoverAutosavedMeetDialog() {

            // Create the message dialog and set its content
            var messageDialog = new MessageDialog("There is an autosaved meet from the last app shutdown.  Would you like to recover this meet?");

            // Add commands and set their callbacks; both selections will use the same callback function, which will vary 
            // its behavior based on the selection made
            messageDialog.Commands.Add(
                new UICommand("RECOVER AUTOSAVED MEET", new UICommandInvokedHandler(this.RecoverAutosaveCommandInvokedHandler)));

            messageDialog.Commands.Add(
                new UICommand("DISCARD AUTOSAVED MEET", new UICommandInvokedHandler(this.RecoverAutosaveCommandInvokedHandler)));

            // Set the command that will be invoked by default (Enter)
            messageDialog.DefaultCommandIndex = 1;

            // Set the command to be invoked when escape is pressed
            messageDialog.CancelCommandIndex = 1;

            // Show the message dialog
            await messageDialog.ShowAsync();

        }

        private async void RecoverAutosaveCommandInvokedHandler(IUICommand command) {

            switch (command.Label) {
            case "RECOVER AUTOSAVED MEET":
                try {
                    await Meet.restoreFromSerialization(ProgramConstants.AUTOSAVE_FILE_NAME, ProgramConstants.AUTOSAVE_FOLDER_ADJ);
                    _existingMeet = true;
                    await FileManagement.DeleteAutoRecoverFile();

                    // TO UNDERSTAND... Not sure how my data binding is working here
                    // especially if the whole ObservableCollection mumbo jumbo is doing anything
                    // I don't think it is (maybe because my updates don't involve using a setter...?)
                    this.DataContext = Meet.divisions;
                    if (Meet.meetParameters.useDivisions) {
                        comboBox_useDivisions.SelectedIndex = 1;  // Set to ON
                    }
                    else {
                        comboBox_useDivisions.SelectedIndex = 0; // Set to OFF
                    }
                    comboBox_CurrentDivision.SelectedIndex = 0;

                    try { RefreshStandingDisplays(); }
                    catch (Exception ex) { HandleRefreshStandingsException(ex); }
                }
                catch {
                    _existingMeet = null; // let the new or existing meet dialog fire
                    await FileManagement.DeleteAutoRecoverFile();
                    return;
                } // TODO: Add more robust treatment; Means there was a failure on the restore...
                  // Should not happen, so this is a low priority extension... perhaps a wait and see 
                  // (failure should require user to alter deeply hidden application files)
                break;
            case "DISCARD AUTOSAVED MEET":
                _existingMeet = null; // let the new or existing meet dialog fire
                await FileManagement.DeleteAutoRecoverFile();
                break;
            default:
                _existingMeet = null; // let the new or existing meet dialog fire
                await FileManagement.DeleteAutoRecoverFile();
                string unexpectedOutcomeText = "UNEXPECTED OUTCOME" + Environment.NewLine + Environment.NewLine;
                unexpectedOutcomeText += "Selected command: " + command.Label + " not recognized";

                TextBlock tempShowCommand = new TextBlock();
                tempShowCommand.Text = unexpectedOutcomeText;

                aggregateCompetition.Children.Clear();
                aggregateCompetition.Children.Add(tempShowCommand);
                break;
            }
        }

        private async Task NewOrExistingMeetDialog() {

            // Create the message dialog and set its content
            var messageDialog = new MessageDialog("Would you like to open an existing meet file or create a new meet file?");

            // Add commands and set their callbacks; both selections will use the same callback function, which will vary 
            // its behavior based on the selection made
            messageDialog.Commands.Add(
                new UICommand("EXISTING MEET", new UICommandInvokedHandler(this.StartupCommandInvokedHandler)));

            messageDialog.Commands.Add(
                new UICommand("NEW MEET", new UICommandInvokedHandler(this.StartupCommandInvokedHandler)));

            // Set the command that will be invoked by default (Enter)
            messageDialog.DefaultCommandIndex = 1;

            // Set the command to be invoked when escape is pressed
            messageDialog.CancelCommandIndex = 1;

            // Show the message dialog
            await messageDialog.ShowAsync();

        }

        private async void StartupCommandInvokedHandler(IUICommand command) {

            switch(command.Label) {
            case "EXISTING MEET":
                try {
                    await Meet.restoreFromSerialization();
                    _existingMeet = true;
                    
                    // TO UNDERSTAND... Not sure how my data binding is working here
                    // especially if the whole ObservableCollection mumbo jumbo is doing anything
                    // I don't think it is (maybe because my updates don't involve using a setter...?)
                    this.DataContext = Meet.divisions;
                    if (Meet.meetParameters.useDivisions) {
                        comboBox_useDivisions.SelectedIndex = 1;  // Set to ON
                    }
                    else {
                        comboBox_useDivisions.SelectedIndex = 0; // Set to OFF
                    }
                    comboBox_CurrentDivision.SelectedIndex = 0;

                    try { RefreshStandingDisplays(); }
                    catch (Exception ex) { HandleRefreshStandingsException(ex); }
                }
                catch {
                    _existingMeet = false;
                    return;
                } // TODO: Add more robust treatment; currently follow this approach because 
                // this likely indicates the file save process was cancelled by the user
                break;
            case "NEW MEET":
                _existingMeet = false;
                break;
            default:
                _existingMeet = false;
                string unexpectedOutcomeText = "UNEXPECTED OUTCOME" + Environment.NewLine + Environment.NewLine;
                unexpectedOutcomeText += "Selected command: " + command.Label + " not recognized";

                TextBlock tempShowCommand = new TextBlock();
                tempShowCommand.Text = unexpectedOutcomeText;

                aggregateCompetition.Children.Clear();
                aggregateCompetition.Children.Add(tempShowCommand);
                break;
            }
        }

        private async void Button_ClickAddOrEditEventScores(object sender, RoutedEventArgs e) {
            if(_teamEventSelector_ContentDialog==null) {
                _teamEventSelector_ContentDialog = new ContentDialog_TeamEventSelector();
            }

            if (_editTeamForEvent_ContentDialog == null) {
                _editTeamForEvent_ContentDialog = new ContentDialog_EditTeamForEvent();
            }

            if(Meet.TeamCount > 0) {
                await _teamEventSelector_ContentDialog.ShowAsync();
                await _editTeamForEvent_ContentDialog.ShowAsync();

                if(Meet.ScoresToValidate) {
                    if(_editTeamForEventVerification_ContentDialog == null) {
                        _editTeamForEventVerification_ContentDialog = new ContentDialog_EditTeamForEventVerification();
                    }
                    await _editTeamForEventVerification_ContentDialog.ShowAsync(); // **
                }

                _currentlySelectedEvent = null;
                _currentlySelectedTeamsList.Clear();
            }

            try { RefreshStandingDisplays(); }
            catch (Exception ex) { HandleRefreshStandingsException(ex); }
        }

        private async void Button_ClickLookupGymnast(object sender, RoutedEventArgs e) {
            if (_teamSelector_ContentDialog == null) { _teamSelector_ContentDialog = new ContentDialog_TeamSelector(); }
            if (_gymnastSelector_ContentDialog == null) { _gymnastSelector_ContentDialog = new ContentDialog_GymnastSelector(); }
            if (_editGymnast_ContentDialog == null) { _editGymnast_ContentDialog = new ContentDialog_EditGymnast(); }
            
            if (Meet.TeamCount > 0) {
                await _teamSelector_ContentDialog.ShowAsync();

                if (_currentlySelectedTeam != null) {
                    await _gymnastSelector_ContentDialog.ShowAsync();
                }
                else { return; }

                if(_currentlySelectedGymnastNbr != null) {
                    await _editGymnast_ContentDialog.ShowAsync();
                }

                _currentlySelectedTeam = null;
                _currentlySelectedGymnastNbr = null;
            }

            try { RefreshStandingDisplays(); }
            catch (Exception ex) { HandleRefreshStandingsException(ex); }
        }

        private void Button_ClickOutputResults(object sender, RoutedEventArgs e) {
            _outputPlainTextOnly = false;
            this.Frame.Navigate(typeof(OutputResultsPage));
        }

        private void Button_ClickOutputResults_PlainTextOnly(object sender, RoutedEventArgs e) {
            _outputPlainTextOnly = true;
            this.Frame.Navigate(typeof(OutputResultsPage));
        }

        private async void ButtonClick_Rosters(object sender, RoutedEventArgs e) {
            await FileManagement.OutputRostersToTextAsync();
        }

        private async void Button_ClickAddorEditTeam(object sender, RoutedEventArgs e) {
            if (!(sender is Button)) {
                throw new Exception_UnexpectedDataTypeEncountered((new Button()).GetType(), sender.GetType());
            }

            Button button = sender as Button;
            string commandString = HelperMethods.GrabButtonLabelString(button);

            if (_editTeam_ContentDialog == null) { _editTeam_ContentDialog = new ContentDialog_EditTeam(); }
            if (_teamSelector_ContentDialog == null) { _teamSelector_ContentDialog = new ContentDialog_TeamSelector(); }

            if(commandString.ToUpper().Contains("EDIT") && (Meet.TeamCount > 0)) { 
                await _teamSelector_ContentDialog.ShowAsync();
                _editTeam_ContentDialog.newTeam = false;
            }
            else {
                _editTeam_ContentDialog.newTeam = true;
            }

            await _editTeam_ContentDialog.ShowAsync();
            try { RefreshStandingDisplays(); }
            catch (Exception ex) { HandleRefreshStandingsException(ex); }
        }

        /* Related to configuration, enablement, and selection of DIVISIONS */
        private async void Button_ClickConfigureDivisions(object sender, RoutedEventArgs e) {
            if (_configureDivisions_ContentDialog==null) {
                // Meet.ProgrammaticallyAdjustDivisions();
                // comboBox_CurrentDivision.SelectedIndex = 0;
                _configureDivisions_ContentDialog = new ContentDialog_ConfigureDivisions();
            }

            await _configureDivisions_ContentDialog.ShowAsync();
        }

        private void comboBox_useDivisions_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (!(sender is ComboBox)) {
                throw new Exception_UnexpectedDataTypeEncountered((new ComboBox()).GetType(), sender.GetType());
            }

            ComboBox combo = sender as ComboBox;

            if(combo.SelectedIndex==1) { // 1 should be the index of "ON"; 0 should be the index of "OFF"
                Meet.meetParameters.useDivisions = true;
                comboBox_CurrentDivision.IsEnabled = true;
                if(comboBox_CurrentDivision.SelectedIndex==-1) { // an initialization value
                    comboBox_CurrentDivision.SelectedIndex = 0; // places us on "All Divisions" / "None Set"
                }
            }
            else {
                Meet.meetParameters.useDivisions = false;
                _currentlySelectedDivision = Meet.divisions.ElementAt(comboBox_CurrentDivision.SelectedIndex);
                comboBox_CurrentDivision.IsEnabled = false;
                comboBox_CurrentDivision.SelectedIndex = 0; // should place us on "All Divisions" / "None Set"
                _currentlySelectedDivision = Meet.divisions.ElementAt(comboBox_CurrentDivision.SelectedIndex);
            }


            try { RefreshStandingDisplays(); }
            catch (Exception ex) { HandleRefreshStandingsException(ex); }
        }

        public static void UpdateDivisionComboOptionsDisplay() {
            MainPage.Current.DataContext = Meet.divisions;
            MainPage.Current.comboBox_CurrentDivision.SelectedIndex = 0;
        }

        public static int GetDivisionComboBoxSelectionIndex() {
            return MainPage.Current.comboBox_CurrentDivision.SelectedIndex;
        }

        public static void SetDivisionComboBoxSelectionIndex(int idx) {
            MainPage.Current.comboBox_CurrentDivision.SelectedIndex = idx;
        }

        private void comboBox_comboBox_CurrentDivision_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (!(sender is ComboBox)) {
                throw new Exception_UnexpectedDataTypeEncountered((new ComboBox()).GetType(), sender.GetType());
            }

            ComboBox combo = sender as ComboBox;

            if(Meet.divisions.Count == 0) { return; } // do nothing in this case
            if(combo.SelectedIndex==-1) { // the initialization value
                _currentlySelectedDivision = Meet.divisions.ElementAt(0);
            }
            else {
                _currentlySelectedDivision = Meet.divisions.ElementAt(combo.SelectedIndex);
            }
            

            try { RefreshStandingDisplays(); }
            catch (Exception ex) { HandleRefreshStandingsException(ex); }
        }

        /* Done with DIVISIONS */


        private async Task ManageTeamsDialog() {

            // Create the message dialog and set its content
            var messageDialog = new MessageDialog("Manage team files: would you like to save teams for future re-use or load previously saved teams to more quickly set-up new meet deatils?");

            // Add commands and set their callbacks; both selections will use the same callback function, which will vary 
            // its behavior based on the selection made
            messageDialog.Commands.Add(
                new UICommand("LOAD TEAM FILES", new UICommandInvokedHandler(this.ManageTeamsCommandInvokedHandler)));

            messageDialog.Commands.Add(
                new UICommand("SAVE TEAM FILES", new UICommandInvokedHandler(this.ManageTeamsCommandInvokedHandler)));

            messageDialog.Commands.Add(
                new UICommand("CANCEL", new UICommandInvokedHandler(this.ManageTeamsCommandInvokedHandler)));

            // Set the command that will be invoked by default (Enter)
            messageDialog.DefaultCommandIndex = 0;

            // Set the command to be invoked when escape is pressed
            messageDialog.CancelCommandIndex = 0;

            // Show the message dialog
            await messageDialog.ShowAsync();

        }

        private async void ManageTeamsCommandInvokedHandler(IUICommand command) {

            switch (command.Label) {
            case "LOAD TEAM FILES":
                var teamfiles = await FileManagement.SelectTeamsToLoad();
                await FileManagement.InitializeTeams(teamfiles);
                break;
            case "SAVE TEAM FILES":
                _saveTeams = true;
                break;
            case "CANCEL":
                // Nothing to do
                break;
            default:
                _existingMeet = false;
                string unexpectedOutcomeText = "UNEXPECTED OUTCOME" + Environment.NewLine + Environment.NewLine;
                unexpectedOutcomeText += "Selected command: " + command.Label + " not recognized";

                TextBlock tempShowCommand = new TextBlock();
                tempShowCommand.Text = unexpectedOutcomeText;

                aggregateCompetition.Children.Clear();
                aggregateCompetition.Children.Add(tempShowCommand);
                break;
            }
        }

        private async void Button_ClickManageTeams(object sender, RoutedEventArgs e) {
            await ManageTeamsDialog();

            if(_saveTeams == true) {
                string unexpectedOutcomeText = "SAVE TEAMS" + Environment.NewLine + Environment.NewLine;

                TextBlock tempShowCommand = new TextBlock();
                tempShowCommand.Text = unexpectedOutcomeText;

                aggregateCompetition.Children.Clear();
                aggregateCompetition.Children.Add(tempShowCommand);

                await FileManagement.SaveTeamFiles();
            }
        }

        // Simpler version for use when we are exiting the program (and save is really the only valid option)
        private async Task SaveTeamsDialog() {

            // Create the message dialog and set its content
            var messageDialog = new MessageDialog("Would you like to save the currently configured teams for future re-use?");

            messageDialog.Commands.Add(
                new UICommand("SAVE TEAM FILES", new UICommandInvokedHandler(this.SaveTeamsCommandInvokedHandler)));

            messageDialog.Commands.Add(
                new UICommand("CANCEL", new UICommandInvokedHandler(this.SaveTeamsCommandInvokedHandler)));

            // Set the command that will be invoked by default (Enter)
            messageDialog.DefaultCommandIndex = 0;

            // Set the command to be invoked when escape is pressed
            messageDialog.CancelCommandIndex = 0;

            // Show the message dialog
            await messageDialog.ShowAsync();

        }

        private async void SaveTeamsCommandInvokedHandler(IUICommand command) {

            switch (command.Label) {

            case "SAVE TEAM FILES":
                _saveTeams = true;
                await FileManagement.SaveTeamFiles();
                break;
            case "CANCEL":
                // Nothing to do
                break;
            default:
                _existingMeet = false;
                string unexpectedOutcomeText = "UNEXPECTED OUTCOME" + Environment.NewLine + Environment.NewLine;
                unexpectedOutcomeText += "Selected command: " + command.Label + " not recognized";

                TextBlock tempShowCommand = new TextBlock();
                tempShowCommand.Text = unexpectedOutcomeText;

                aggregateCompetition.Children.Clear();
                aggregateCompetition.Children.Add(tempShowCommand);
                break;
            }
        }

        private async void Button_ClickSaveAndExit(object sender, RoutedEventArgs e) {
            await ManageSuspend_WIP(true);

            // TODO (high priority before active distribution; medium / low priority while in prelim user trials)
            // Apparently I am not supposed to be calling Exit explicitly... may cause rejection by the App store... need to investigate
            Application.Current.Exit();
        }

        internal async Task ManageSuspend_WIP(bool userManaged = false) {
            if(userManaged) { 
                try { await Meet.sendForSerialization(); }
                catch { return; } // TODO: Add more robust treatment; currently follow this approach because 
                                  // this likely indicates the file save process was cancelled by the user

                // TODO: medium priority (12/15/2016)
                // Saving teams as second step within the save and exit process is not working... Or, rather, the folder picker isn't showing
                // and I can't find an exception generated, and I can't figure out where any hypothtical files would be going...
                // so it's useless to an end user... remove from flow so user doesn't have false expectation; come back to and fix later
                // await SaveTeamsDialog();
            }

            else {
                string storageFolderPathAdj = ProgramConstants.AUTOSAVE_FOLDER_ADJ;
                string safetyMeetFileName = ProgramConstants.AUTOSAVE_FILE_NAME;                                
                await Meet.sendForSerialization(safetyMeetFileName, storageFolderPathAdj);
            }
        }

        private async void Button_ClickEditMeetConfiguration(object sender, RoutedEventArgs e) {
            if(_editMeetParameters_ContentDialog==null) {
                _editMeetParameters_ContentDialog = new ContentDialog_MeetParameters();
            }

            await _editMeetParameters_ContentDialog.ShowAsync();
        }

        private void Button_ClickCalculateQualifiers(object sender, RoutedEventArgs e) {
            this.Frame.Navigate(typeof(CalculateQualifiers_Page));
        }

        private void Button_ClickAggregateCompetitionView(object sender, RoutedEventArgs e) {
            if (!(sender is Button)) {
                throw new Exception_UnexpectedDataTypeEncountered((new Button()).GetType(), sender.GetType());
            }
            Button button = sender as Button;

            aggregateCompetition.Children.Clear();

            string commandString = HelperMethods.GrabButtonLabelString(button);

            if (commandString.ToUpper().Contains("INDIVIDUAL")) {
                var allaroundRankingsUC = RankingDisplayGymnasts(commandString, _currentlySelectedDivision);
                aggregateCompetition.Children.Add(allaroundRankingsUC);
            }
            if (commandString.ToUpper().Contains("TEAM")) {
                TextBlock TBk_DisplayTeamStandings = new TextBlock();
                TBk_DisplayTeamStandings.Text = Meet.createString_teamStandings(_currentlySelectedDivision);
                aggregateCompetition.Children.Add(TBk_DisplayTeamStandings);
            }

            aggregateDisplayLabel.Text = commandString.ToUpper();
        }

        private void Button_ClickEventCompetitionView(object sender, RoutedEventArgs e) {
            if (!(sender is Button)) {
                throw new Exception_UnexpectedDataTypeEncountered((new Button()).GetType(), sender.GetType());
            }
            Button button = sender as Button;

            string commandString = HelperMethods.GrabButtonLabelString(button);
            // string displayText = Meet.createString_gymnastStandings(commandString);

            var rankingUC = RankingDisplayGymnasts(commandString, _currentlySelectedDivision);

            if (Grid.GetColumn(button) < 6) {
                eventCompetition_box1.Children.Clear();
                eventCompetition_box1.Children.Add(rankingUC);
                eventDisplayLabel_box1.Text = commandString.ToUpper();
            }
            else {
                eventCompetition_box2.Children.Clear();
                eventCompetition_box2.Children.Add(rankingUC);
                eventDisplayLabel_box2.Text = commandString.ToUpper();
            }
        }

        private UserControl_RankingDisplayGymnast RankingDisplayGymnasts(string commandString, Division div = null) {

            var ucrdg = UserControl_RankingDisplayGymnast.RankingDisplayOfGymnasts_ForUI(commandString, div);

            DependencyObject dObj = HelperMethods.GetFirstVisualTreeElementWithName(ucrdg, "RankingGrid");
            if (dObj == null) {
                throw new Exception_CouldNotFindUIElement("RankingGrid", (new Grid()).GetType(), ucrdg.GetType());
            }

            Grid rankingGrid = dObj as Grid;
            if (rankingGrid == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new Grid()).GetType(), dObj.GetType());
            }

            foreach (UIElement ele in rankingGrid.Children) {
                if (ele is Button) {
                    Button temp = ele as Button;
                    temp.Click += ButtonClick_GymnastFromWithinRanking;
                }
            }

            return ucrdg;
        }

        private async void ButtonClick_GymnastFromWithinRanking(object sender, RoutedEventArgs args) {
            if(_editGymnast_ContentDialog==null) {
                _editGymnast_ContentDialog = new ContentDialog_EditGymnast();
            }

            if(!(sender is Button)) {
                throw new Exception_UnexpectedDataTypeEncountered((new Button()).GetType(), sender.GetType());
            }
            Button button = sender as Button;

            string buttonContent = button.Content.ToString();
            int gymnastNbr;
            if(Int32.TryParse(buttonContent, out gymnastNbr)) {
                _currentlySelectedGymnastNbr = gymnastNbr;
            }

            else {
                Grid rankingGrid = button.Parent as Grid;
                if(rankingGrid==null) {
                    throw new Exception_UnexpectedDataTypeEncountered((new Grid()).GetType(), button.Parent.GetType());
                }

                int row = Grid.GetRow(button);
                int col = Grid.GetColumn(button);

                var altButton = (from b in rankingGrid.Children.OfType<Button>()
                                  where Grid.GetRow(b) == row
                                  where Grid.GetColumn(b) == (col - 1)
                                  select b).Single(); // should only be one

                string altButtonContent = altButton.Content.ToString();
                if (Int32.TryParse(altButtonContent, out gymnastNbr)) {
                    _currentlySelectedGymnastNbr = gymnastNbr;
                }
            }

            if(_currentlySelectedGymnastNbr != null) {
                await _editGymnast_ContentDialog.ShowAsync();
                _currentlySelectedGymnastNbr = null;

                try { RefreshStandingDisplays(); }
                catch (Exception ex) { HandleRefreshStandingsException(ex); }
            }
        }

        public void RefreshStandingDisplays() {

            aggregateCompetition.Children.Clear();
            eventCompetition_box1.Children.Clear();
            eventCompetition_box2.Children.Clear();

            if (aggregateDisplayLabel.Text.ToUpper().Contains("TEAM")) {
                TextBlock aggregateTbk = new TextBlock();
                aggregateTbk.Text = Meet.createString_teamStandings(_currentlySelectedDivision);
                aggregateCompetition.Children.Add(aggregateTbk);
            }
            else {
                var allaroundRankingsUC = RankingDisplayGymnasts(aggregateDisplayLabel.Text, _currentlySelectedDivision);
                aggregateCompetition.Children.Add(allaroundRankingsUC);
            }

            var eventBox1StandingsUC = RankingDisplayGymnasts(eventDisplayLabel_box1.Text, _currentlySelectedDivision);
            var eventBox2StandingsUC = RankingDisplayGymnasts(eventDisplayLabel_box2.Text, _currentlySelectedDivision);
            
            eventCompetition_box1.Children.Add(eventBox1StandingsUC);
            eventCompetition_box2.Children.Add(eventBox2StandingsUC);
        }

        private void HandleRefreshStandingsException(Exception e) {
            TextBlock tbk = new TextBlock();
            tbk.FontSize = 10;
            tbk.Text = e.Message + Environment.NewLine + "  >> During call to RefreshStandingsDisplay; source: " + e.Source;
            _publicStaticDebugSpace.Children.Add(tbk);
        }

        private void Button_HandlerWIP(object sender, RoutedEventArgs e) {

            if (!(sender is Button)) {
                throw new Exception_UnexpectedDataTypeEncountered((new Button()).GetType(), sender.GetType());
            }
            Button button = sender as Button;

            // Display message identifying which button was pushed
            string WIP_displayText = "WORK IN PROGRESS" + Environment.NewLine + Environment.NewLine;
            WIP_displayText = HelperMethods.GrabButtonLabelString(button);

            TextBlock tempShowCommand = new TextBlock();
            tempShowCommand.Text = WIP_displayText;

            aggregateCompetition.Children.Clear();
            aggregateCompetition.Children.Add(tempShowCommand);
        }

        /* ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** 
         * TEST AND JUNK CODE
         * ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** */

        private void Button_ClickTestButton(object sender, RoutedEventArgs args) {
            // Display message identifying which button was pushed
            /*
            string test_displayText = Meet.meetParameters.ToString();

            TextBlock tempShowCommand = new TextBlock();
            tempShowCommand.Text = test_displayText;

            aggregateCompetition.Children.Clear();
            aggregateCompetition.Children.Add(tempShowCommand);
            */

            // Display the contents of the Edit Meet Parameters content dialog
            /*
            if (_editMeetParameters_contentDialog == null) {
                _editMeetParameters_contentDialog = new ContentDialog_MeetParameters();
            }

            ContentDialog cd = _editMeetParameters_contentDialog;
            if (cd == null) {
                throw new Exception("EXCEPTION >> Unexpected type for sender on ContentDialog loaded event; expected ContentDialog; actual " + sender.GetType().Name);
            }

            aggregateCompetition.Children.Clear();
            HelperMethods.PlaceVisualTreeIntoStackPanel(cd, aggregateCompetition, 0);
            */

            // Show all gymnasts that have been configure
            /*
            TextBlock configuredGymnastsTBk = new TextBlock();
            configuredGymnastsTBk.Text = Meet.RosterString();
            aggregateCompetition.Children.Clear();
            aggregateCompetition.Children.Add(configuredGymnastsTBk);
            */

            // Show all the Visual State Groups for a UI Element of interest
            TextBlock tempTBk = new TextBlock(); tempTBk.FontSize = 9;
            _publicStaticDebugSpace.Children.Clear();

            var temp = UserControl_RankingDisplayGymnast.RankingDisplayOfGymnasts_ForUI("Vault");
            var dObj = HelperMethods.GetFirstVisualTreeElementWithName(temp, "RankingGrid");
            if(dObj==null) {
                // try / throw / catch construct is very hokie, but it allows me to get the outpuyt message I want without having to recreate it from scratch
                try { 
                    throw new Exception_CouldNotFindUIElement("RankingGrid", (new Grid()).GetType(), temp.GetType());
                }
                catch (Exception e) {
                    tempTBk.Text = e.Message;
                    _publicStaticDebugSpace.Children.Add(tempTBk);
                }
            }
            Grid rankingGrid = dObj as Grid;
            if(rankingGrid == null) {
                try {
                    throw new Exception_UnexpectedDataTypeEncountered((new Grid()).GetType(), dObj.GetType());
                }
                catch (Exception e) {
                    tempTBk.Text = e.Message;
                    _publicStaticDebugSpace.Children.Add(tempTBk);
                }
            }

            try {
                tempTBk.Text = HelperMethods.ExploreVisualStateGroups_ForUIElement(rankingGrid);
            }
            catch(Exception e) {
                tempTBk.Text = e.Message;
            }
            _publicStaticDebugSpace.Children.Add(tempTBk);
        }

        




        /* ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** 
         * CONTENT DIALOG TEST AREA 
         * ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** */

        /*
    private ContentDialog_MeetParameters SetupContentDialog_EditMeetParameters() {

        var contentDialog = new ContentDialog_MeetParameters() {
            Name = "ContentDialog_EditMeetParameters",
            Title = "Edit Meet Parameters"
        };
        contentDialog.Loaded += ContentDialog_LoadedExamine;
        return contentDialog;
    }
    */



    }
}
