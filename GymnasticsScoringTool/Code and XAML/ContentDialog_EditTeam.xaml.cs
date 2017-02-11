using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Text;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GymnasticsScoringTool_01 {
    public sealed partial class ContentDialog_EditTeam : ContentDialog {
        string _previousValue;
        bool _newTeam = true;
        public bool newTeam {
            get { return _newTeam; }
            set { _newTeam = value; }
        }
        int _rosterSizeBefore = 0;

        public ContentDialog_EditTeam(Team team = null) {
            this.InitializeComponent();

            TextBox_TeamName.Text = ProgramConstants.TEAM_NOT_SET_TEXT;
                        
            this.Loaded += ContentDialog_LoadedSetUpdateCancelButtons;
            this.Loaded += ContentDialog_LoadedPlaceAddGymnastButton;
            this.Loaded += ContentDialog_LoadedTitleAppropriately;
            this.Opened += ContentDialog_Opened;
            this.Closed += ContentDialog_EditTeam_Closed;
        }

        private void ContentDialog_LoadedTitleAppropriately(object sender, RoutedEventArgs args) {
            if (MainPage._currentlySelectedTeam != null) {
                var tempDO = HelperMethods.GetFirstVisualTreeElementWithName(this, "Title");
                if (tempDO == null) {
                    throw new Exception_CouldNotFindUIElement("Title", (new ContentControl()).GetType(), this.GetType());
                }

                ContentControl titleControl = tempDO as ContentControl;
                if (titleControl == null) {
                    throw new Exception_UnexpectedDataTypeEncountered((new ContentControl()).GetType(), tempDO.GetType());
                }

                titleControl.Content = "Edit Team";
            }
            else {
                var tempDO = HelperMethods.GetFirstVisualTreeElementWithName(this, "Title");
                if (tempDO == null) {
                    throw new Exception_CouldNotFindUIElement("Title", (new ContentControl()).GetType(), this.GetType());
                }

                ContentControl titleControl = tempDO as ContentControl;
                if (titleControl == null) {
                    throw new Exception_UnexpectedDataTypeEncountered((new ContentControl()).GetType(), tempDO.GetType());
                }

                titleControl.Content = "Add New Team";
            }
        }


        private void ContentDialog_LoadedSetUpdateCancelButtons(object sender, RoutedEventArgs e) {

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

        private void ContentDialog_LoadedPlaceAddGymnastButton(object sender, RoutedEventArgs e) {
            /* TEST OUTCOME: Loaded fires each time ShowAsync() is called for ContentDialog
            TextBlock testTBk = new TextBlock();
            testTBk.FontSize = 8;
            testTBk.Text = "ContentDialog_LoadedPlaceAddGymnastButton handler invoked";
            MainPage._publicStaticDebugSpace.Children.Add(testTBk);
            */ 

            ContentDialog cd = sender as ContentDialog;
            if (cd == null) {
                throw new Exception("EXCEPTION >> Unexpected type for sender on ContentDialog loaded event; expected ContentDialog; actual " + sender.GetType().Name);
            }

            DependencyObject temp = HelperMethods.GetFirstVisualTreeElementWithName(cd, "AddGymnastButton");
            if(temp!=null) { return; } // only execute the rest of this code to set up the button if it doesn't already exist
                        
            temp = HelperMethods.GetFirstVisualTreeElementWithName(cd, "DialogSpace");
            Grid gridForPlacement = temp as Grid;

            object o;
            bool resourceKeyFound = App.Current.Resources.TryGetValue("ButtonLC", out o);
            ControlTemplate template = o as ControlTemplate;

            Button button_addGymnastToTeam = new Button();
            if (template != null) { button_addGymnastToTeam.Template = template; }
            button_addGymnastToTeam.Name = "AddGymnastButton";
            button_addGymnastToTeam.Content = "Add Gymnast";
            button_addGymnastToTeam.HorizontalAlignment = HorizontalAlignment.Right;
            button_addGymnastToTeam.VerticalAlignment = VerticalAlignment.Top;
            button_addGymnastToTeam.Margin = new Thickness(10);
            button_addGymnastToTeam.Padding = new Thickness(5);
            button_addGymnastToTeam.Background = new SolidColorBrush(Colors.LightGray);
            button_addGymnastToTeam.BorderBrush = new SolidColorBrush(Colors.Gray);
            button_addGymnastToTeam.BorderThickness = new Thickness(2);
            button_addGymnastToTeam.FontStyle = Windows.UI.Text.FontStyle.Italic;
            button_addGymnastToTeam.Click += ButtonAddNewGymnast_Click;

            gridForPlacement.Children.Add(button_addGymnastToTeam);
        }

        private void ContentDialog_Opened(object sender, ContentDialogOpenedEventArgs e) {

            ContentDialog cd = sender as ContentDialog;
            if (cd == null) {
                throw new Exception("EXCEPTION >> Unexpected type for sender on ContentDialog loaded event; expected ContentDialog; actual " + sender.GetType().Name);
            }

            TextBlock tempTBx = new TextBlock();

            DependencyObject temp = HelperMethods.GetFirstVisualTreeElementWithName(cd, "ButtonCancel");
            Button buttonCancel = temp as Button;
            if(buttonCancel != null) {

                buttonCancel.Focus(FocusState.Keyboard);
            }

            if(MainPage._currentlySelectedTeam != null) {
                TextBox_TeamName.Text = MainPage._currentlySelectedTeam.name;
                TextBox_TeamName.IsEnabled = false;
                Meet.AddTeamTo_UserControl_EditGymnast(MainPage._currentlySelectedTeam, EditGymnasts_UserControl);
                _rosterSizeBefore = MainPage._currentlySelectedTeam.rosterSize;
            }
            else {
                TextBox_TeamName.IsEnabled = true;
                _rosterSizeBefore = 0;
            }
        }

        private void ButtonContentDialog_Click(object sender, RoutedEventArgs args) {
            Button button = sender as Button;
            if (button == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new Button()).GetType(), sender.GetType());
            }

            Grid editGymnastsGrid = EditGymnasts_UserControl.Content as Grid;
            if (editGymnastsGrid == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new Grid()).GetType(), EditGymnasts_UserControl.Content.GetType());
            }

            if (button.Name == "ButtonUpdate") {
                // We have set our data binding to require explicit triggering, so trigger all the appropriate elements here
                foreach (FrameworkElement fe in editGymnastsGrid.Children) {
                    if((fe is TextBox) && (Grid.GetRow(fe) > 0)) {
                        TextBox tBx = fe as TextBox;
                        BindingExpression bExp = tBx.GetBindingExpression(TextBox.TextProperty);
                        bExp.UpdateSource();
                    }
                }

                // Addionally, not included within the scope of data binding, perform any updates to divisions that were made
                var query = from tbk in editGymnastsGrid.Children.OfType<TextBlock>()
                            where Grid.GetColumn(tbk) == 0
                            where Grid.GetRow(tbk) > 0 // the header row
                            select new Tuple<string, int>((tbk).Text, Grid.GetRow(tbk));

                int secondToLastCol = editGymnastsGrid.ColumnDefinitions.IndexOf(editGymnastsGrid.ColumnDefinitions.Last()) - 1;

                foreach(var q in query) {
                    int idx = -1;
                    try {
                        idx = (from cb in editGymnastsGrid.Children.OfType<ComboBox>()
                               where Grid.GetColumn(cb) == secondToLastCol
                               where Grid.GetRow(cb) == q.Item2
                               select (cb).SelectedIndex).Single();
                    }
                    catch (Exception e) {
                        throw new Exception("EXCEPTION: Single item not found, query params: col = " + secondToLastCol.ToString()
                            + ", row = " + q.Item2 + " for competitor: " + q.Item1, e);
                    }


                    Division update = Meet.divisions.ElementAt(idx);
                    int competitorNbr = -1;
                    try { competitorNbr = Int32.Parse(q.Item1); }
                    catch { throw new Exception("EXCEPTION: Could not parse gymnast competitor number > " + q.Item1); }
                    Meet.UpdateGymnastDivision(competitorNbr, update);
                }

                Meet.RemoveGymnastsMarkedForDeletion();
            }
            else {
                // REMOVE any new information that was instantiated prior to the ButtonCancel being pressed (Team and / or gymnasts)
                if((TextBox_TeamName.Text != ProgramConstants.TEAM_NOT_SET_TEXT) && (TextBox_TeamName.Text != "")) {
                    Team t = Meet.GetTeamWithName(TextBox_TeamName.Text);
                    if (t != null) {
                        if (newTeam) { Meet.RemoveTeam(t); }
                        else {
                            int gymnastsToTrim = t.rosterSize - _rosterSizeBefore;
                            Meet.RemoveGymnastsFromTeam_LastN(t, gymnastsToTrim);
                        }
                    }
                }

                Meet.ResetGymnastsMarkedForDeletion();
            }

            Hide();
        }

        private void ContentDialog_EditTeam_Closed(ContentDialog sender, ContentDialogClosedEventArgs args) {
            Grid editGymnastsGrid = EditGymnasts_UserControl.Content as Grid;
            if (editGymnastsGrid == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new Grid()).GetType(), EditGymnasts_UserControl.Content.GetType());
            }

            HelperMethods.RemoveAllButHeaderRowFromGrid(editGymnastsGrid);

            object o = TeamSetupStackPanel.FindName("TextBox_TeamName");
            if (o is TextBox) {
                TextBox tBx = o as TextBox;
                tBx.IsEnabled = true;
                tBx.Text = ProgramConstants.TEAM_NOT_SET_TEXT;
            }

            MainPage._currentlySelectedTeam = null;
        }

        private static void debugInfoDisplay_deletionQueue(IEnumerable<FrameworkElement> ie_fe) {

            TextBlock debugTbkSpacer = new TextBlock();
            debugTbkSpacer.Text = " -- spacer -- ";
            MainPage._publicStaticDebugSpace.Children.Add(debugTbkSpacer);

            foreach (FrameworkElement fe in ie_fe) { 
                int row = Grid.GetRow(fe);
                int col = Grid.GetColumn(fe);

                string contentString = "null";
                if (fe is TextBlock) { contentString = (fe as TextBlock).Text; }
                if (fe is TextBox) { contentString = (fe as TextBox).Text; }

                string debugText = "Row: " + row.ToString() + "; col: " + col.ToString() + "; type: " + fe.GetType().Name + "; content: " + contentString;

                TextBlock debugTbk = new TextBlock();
                debugTbk.Text = debugText;
                MainPage._publicStaticDebugSpace.Children.Add(debugTbk);
            }
        }

        private void textBox_GotFocus(object sender, RoutedEventArgs e) {
            TextBox textBox = sender as TextBox;
            if (textBox == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new TextBox()).GetType(), sender.GetType());
            }

            _previousValue = textBox.Text;
            textBox.SelectAll();
        }

        private void textBox_LostFocus(object sender, RoutedEventArgs e) {
            TextBox textBox = sender as TextBox;
            if (textBox == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new TextBox()).GetType(), sender.GetType());
            }

            if (textBox.Text == "") { textBox.Text = _previousValue; return; }
            if(textBox.Text != ProgramConstants.TEAM_NOT_SET_TEXT) {
                if(newTeam) { textBox_teamNameSet(sender, e); }
            }
        }

        private void textBox_teamNameSet(object sender, RoutedEventArgs e) {
            TextBox tBx = sender as TextBox;
            if (tBx == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new TextBox()).GetType(), sender.GetType());
            }

            // TODO: Handle duplicate team exception
            Team newTeam = new Team(tBx.Text); 
            Meet.AddTeam(newTeam);

            tBx.IsEnabled = false;
            tBx.FontWeight = FontWeights.Bold;
            // tBx.Foreground = new SolidColorBrush(Colors.Maroon); -- Perform this action within storyboard for VisualState "disabled"
        }

        private void ButtonAddNewGymnast_Click(object sender, RoutedEventArgs e) {
            if((this.TextBox_TeamName.Text==ProgramConstants.TEAM_NOT_SET_TEXT) || (this.TextBox_TeamName.Text == "")) {
                TextBlock testTBk = new TextBlock();
                testTBk.FontSize = 8;
                testTBk.Text = "TEAM NAME MUST BE SET before a gymnast can be added";
                MainPage._publicStaticDebugSpace.Children.Add(testTBk);
                return;
            }

            Team team = Meet.GetTeamWithName(TextBox_TeamName.Text);
            Gymnast gymnastToAdd = new Gymnast(ProgramConstants.DEFAULT_NAME, ProgramConstants.DEFAULT_NAME, team, ProgramConstants.INCLUDE_ALL_DIVISIONS);
            Meet.AddGymnast(gymnastToAdd);
            EditGymnasts_UserControl.GymnastToEdit(gymnastToAdd);
        }

    }
}
