using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GymnasticsScoringTool {
    public sealed partial class ContentDialog_EditGymnast : ContentDialog {

        private string _previousString;
        private Flyout _invalidScoreFlyout;
        private TextBlock _invalidScoreTextBlock;

        public ContentDialog_EditGymnast() {
            this.InitializeComponent();
            this.Loaded += ContentDialog_ApplyFormatting;
            this.Loaded += ContentDialog_LoadedSetUpdateCancelButtons;

            this.Opened += ContentDialog_SetupGymnastForEdit;

            _invalidScoreFlyout = new Flyout();
            _invalidScoreFlyout.Placement = FlyoutPlacementMode.Full;
            _invalidScoreTextBlock = new TextBlock();
            _invalidScoreTextBlock.Text = "No invalid score message associated";
            TextBlock invalidScoreTitle = new TextBlock();
            invalidScoreTitle.Text = ProgramConstants.TITLE_INVALID_SCORE;
            invalidScoreTitle.FontSize = FontSize + 2;
            invalidScoreTitle.FontWeight = Windows.UI.Text.FontWeights.Bold;
            StackPanel invalidScoreStackPanel = new StackPanel();
            invalidScoreStackPanel.Children.Add(invalidScoreTitle);
            invalidScoreStackPanel.Children.Add(_invalidScoreTextBlock);
            _invalidScoreFlyout.Content = invalidScoreStackPanel;

            tBxFName.GotFocus += textBox_GotFocus;
            tBxFName.LostFocus += textBox_LostFocus;

            tBxLName.GotFocus += textBox_GotFocus;
            tBxLName.LostFocus += textBox_LostFocus;

            tBxVault.GotFocus += textBox_GotFocus;
            tBxVault.LostFocus += textBox_LostFocus;
            tBxVault.LostFocus += textBox_LostFocus_ValidateScoreInput;

            tBxBars.GotFocus += textBox_GotFocus;
            tBxBars.LostFocus += textBox_LostFocus;
            tBxBars.LostFocus += textBox_LostFocus_ValidateScoreInput;

            tBxBeam.GotFocus += textBox_GotFocus;
            tBxBeam.LostFocus += textBox_LostFocus;
            tBxBeam.LostFocus += textBox_LostFocus_ValidateScoreInput;

            tBxFloor.GotFocus += textBox_GotFocus;
            tBxFloor.LostFocus += textBox_LostFocus;
            tBxFloor.LostFocus += textBox_LostFocus_ValidateScoreInput;
        }

        private void ContentDialog_LoadedSetUpdateCancelButtons(object sender, RoutedEventArgs args) {

            ContentDialog cd = sender as ContentDialog;
            if (cd == null) {
                throw new Exception("EXCEPTION >> Unexpected type for sender on ContentDialog loaded event; expected ContentDialog; actual " + sender.GetType().Name);
            }

            DependencyObject temp = HelperMethods.GetFirstVisualTreeElementWithName(cd, "ButtonUpdate");
            Button buttonUpdate = temp as Button;
            if (buttonUpdate != null) { buttonUpdate.Click += ButtonContentDialog_Click; }

            temp = HelperMethods.GetFirstVisualTreeElementWithName(cd, "ButtonCancel");
            Button buttonCancel = temp as Button;
            if (buttonCancel != null) { buttonCancel.Click += ButtonContentDialog_Click; }
        }

        private void ButtonContentDialog_Click(object sender, RoutedEventArgs args) {
            
            Button button = sender as Button;
            if (button == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new Button()).GetType(), sender.GetType());
            }

            if (button.Name == "ButtonUpdate") {
                // We have set our data binding to require explicit triggering, so trigger all the appropriate elements here
                // Note that we reset the binding to the correct gymnast each time this dialog is opened, so we 
                // the plumbing doesn't get all crossed up causing updates to the wrong gymnast
                foreach (FrameworkElement fe in gridEditGymnast.Children) {
                    if ((fe is TextBox) && (Grid.GetRow(fe) > 0)) {
                        TextBox tBx = fe as TextBox;
                        BindingExpression bExp = tBx.GetBindingExpression(TextBox.TextProperty);
                        bExp.UpdateSource();
                    }
                }
            }
            else { } // user clicked on cancel... nothing specific to cancel to perform here

            Hide();
        }

        private void ContentDialog_ApplyFormatting(object sender, RoutedEventArgs args) {
            // Also, some resizing the first time this ContentDialog is pulled up (don't need it as wide as typical)
            DependencyObject dObj = HelperMethods.GetFirstVisualTreeElementWithName(this, "BackgroundElement");
            Border border = dObj as Border;
            if (border == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new Border()).GetType(), dObj.GetType());
            }

            border.MaxHeight = 500;
            border.MinHeight = 400;
            border.MaxWidth = 400;
            border.MinWidth = 300;

            this.MaxHeight = 500;
            this.MinHeight = 400;
            this.MaxWidth = 400;
            this.MinWidth = 300;
            this.Height = 450; // the desired value
            this.Width = 300; // the desired value
        }

        private void ContentDialog_SetupGymnastForEdit(ContentDialog sender, ContentDialogOpenedEventArgs args) {
            #region EstablishDataBindings.
            if(MainPage._currentlySelectedGymnastNbr.HasValue) {
                Meet.AddGymnastTo_ContentDialogEditGymnast(MainPage._currentlySelectedGymnastNbr.Value, this);
            }
            else {
                throw new Exception("Exception: No gymnast is currently selected for display");
            }

        }

        public void GymnastToEdit(Gymnast g) { 

            Binding bindingNbr = new Binding();
            Binding bindingFName = new Binding();
            Binding bindingLName = new Binding();
            Binding bindingTeam = new Binding();
            Binding bindingVault = new Binding();
            Binding bindingBars = new Binding();
            Binding bindingBeam = new Binding();
            Binding bindingFloor = new Binding();
            Binding bindingOverall = new Binding();
            Binding bindingDivision = new Binding();

            bindingNbr.Source = g;
            bindingFName.Source = g;
            bindingLName.Source = g;
            bindingTeam.Source = g;
            bindingVault.Source = g;
            bindingBars.Source = g;
            bindingBeam.Source = g;
            bindingFloor.Source = g;
            bindingOverall.Source = g;
            bindingDivision.Source = g;

            bindingNbr.Path = new PropertyPath("competitorNumberDisplay");
            bindingFName.Path = new PropertyPath("firstName");
            bindingLName.Path = new PropertyPath("lastName");
            bindingTeam.Path = new PropertyPath("team.name");
            bindingVault.Path = new PropertyPath("vaultScore.displayScore");
            bindingBars.Path = new PropertyPath("barsScore.displayScore");
            bindingBeam.Path = new PropertyPath("beamScore.displayScore");
            bindingFloor.Path = new PropertyPath("floorScore.displayScore");
            bindingOverall.Path = new PropertyPath("overallScoreDisplay");
            bindingDivision.Path = new PropertyPath("division.name");

            bindingNbr.Mode = BindingMode.OneWay;
            bindingFName.Mode = BindingMode.TwoWay;
            bindingLName.Mode = BindingMode.TwoWay;
            bindingTeam.Mode = BindingMode.OneWay;
            bindingVault.Mode = BindingMode.TwoWay;
            bindingBars.Mode = BindingMode.TwoWay;
            bindingBeam.Mode = BindingMode.TwoWay;
            bindingFloor.Mode = BindingMode.TwoWay;
            bindingOverall.Mode = BindingMode.OneWay;
            bindingDivision.Mode = BindingMode.OneWay;

            // by making these explicit, I can change the root values only when the ButtonUpdate is clicked
            // allowing for easier logic to ignore the inputs when ButtonCancel is selected
            bindingFName.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
            bindingLName.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
            bindingVault.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
            bindingBars.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
            bindingBeam.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
            bindingFloor.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;

            BindingOperations.SetBinding(tBkNbr, TextBlock.TextProperty, bindingNbr);
            BindingOperations.SetBinding(tBxFName, TextBox.TextProperty, bindingFName);
            BindingOperations.SetBinding(tBxLName, TextBox.TextProperty, bindingLName);
            BindingOperations.SetBinding(tBkTeam, TextBlock.TextProperty, bindingTeam);
            BindingOperations.SetBinding(tBxVault, TextBox.TextProperty, bindingVault);
            BindingOperations.SetBinding(tBxBars, TextBox.TextProperty, bindingBars);
            BindingOperations.SetBinding(tBxBeam, TextBox.TextProperty, bindingBeam);
            BindingOperations.SetBinding(tBxFloor, TextBox.TextProperty, bindingFloor);
            BindingOperations.SetBinding(tBkOverall, TextBlock.TextProperty, bindingOverall);
            BindingOperations.SetBinding(tBkDiv, TextBlock.TextProperty, bindingDivision);

            #endregion EstablishDataBindings.

        }




        private void textBox_GotFocus(object sender, RoutedEventArgs args) {
            TextBox textBox = sender as TextBox;
            if (textBox == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new TextBox()).GetType(), sender.GetType());
            }

            _previousString = textBox.Text;
            textBox.SelectAll();
        }

        private void textBox_LostFocus(object sender, RoutedEventArgs args) {
            TextBox textBox = sender as TextBox;
            if (textBox == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new TextBox()).GetType(), sender.GetType());
            }

            if (textBox.Text == "") { textBox.Text = _previousString; }
        }

        private void textBox_LostFocus_ValidateScoreInput(object sender, RoutedEventArgs args) {
            TextBox textBox = sender as TextBox;
            if (textBox == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new TextBox()).GetType(), sender.GetType());
            }

            string adjString = EventScore.AdjustInputString(textBox.Text);
            textBox.Text = adjString;
            if (EventScore.IsValid(textBox.Text)) {
                tBkOverall.Text = generatePreliminary_OverallScoreDisplay();
            }
            else {
                if (EventScore.IsPoorlyFormatted(textBox.Text)) {
                    _invalidScoreTextBlock.Text = ProgramConstants.POORLY_FORMATTED_SCORE;
                }
                else if (EventScore.IsOutsideValidRange(textBox.Text)) {
                    _invalidScoreTextBlock.Text = ProgramConstants.SCORE_OUTSIDE_VALID_RANGE;
                }
                else {
                    _invalidScoreTextBlock.Text = "Invalid score >> reason unknown (indicates programming error... should not see this message)";
                }

                _invalidScoreFlyout.ShowAt(textBox);
            }
        }

        private string generatePreliminary_OverallScoreDisplay() {

            int gymnastNbr = Int32.Parse(tBkNbr.Text);
            Tuple<EventScore, EventScore, EventScore, EventScore> scoresTuple = Meet.GetGymnastEventScoresTuple(gymnastNbr);

            double? vault, bars, beam, floor;
            double overall;
            vault = generateEventScore(tBxVault.Text, "Vault", scoresTuple);
            bars = generateEventScore(tBxBars.Text, "Bars", scoresTuple);
            beam = generateEventScore(tBxBeam.Text, "Beam", scoresTuple);
            floor = generateEventScore(tBxFloor.Text, "Floor", scoresTuple);
            overall = (vault.HasValue ? vault.Value : 0) + (bars.HasValue ? bars.Value : 0)
                + (beam.HasValue ? beam.Value : 0) + (floor.HasValue ? floor.Value : 0);

            return (vault.HasValue || bars.HasValue || beam.HasValue || floor.HasValue) ? overall.ToString() : ProgramConstants.NULL_SCORE_STRING;
        }

        private double? generateEventScore(string s, string eventString, Tuple<EventScore, EventScore, EventScore, EventScore> scoresTuple) {
            if (EventScore.IsValid(s)) {
                if (s == ProgramConstants.NULL_SCORE_STRING) { return null; }
                return Double.Parse(s);
            }
            else {
                switch (eventString.ToUpper()) {
                case "VAULT":
                    return scoresTuple.Item1.score;
                case "BARS":
                    return scoresTuple.Item2.score;
                case "BEAM":
                    return scoresTuple.Item3.score;
                case "FLOOR":
                    return scoresTuple.Item4.score;
                default:
                    return null;
                }
            }
        }



    }
}
