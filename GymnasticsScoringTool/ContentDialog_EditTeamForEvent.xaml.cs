using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GymnasticsScoringTool {
    public sealed partial class ContentDialog_EditTeamForEvent : ContentDialog {
        private Dictionary<string, int> _rosterSizeBefore = new Dictionary<string, int>();


        public ContentDialog_EditTeamForEvent() {
            this.InitializeComponent();

            this.Loaded += ContentDialog_EditTeamForEvent_LoadedFormat;
            this.Loaded += ContentDialog_LoadedSetUpdateCancelButtons;

            this.Opened += ContentDialog_EditTeamForEvent_OpenedLoadGymnasts;
            this.Closed += ContentDialog_EditTeamForEvent_Closed;
        }

        private void ContentDialog_EditTeamForEvent_LoadedFormat(object sender, RoutedEventArgs e) {
            string teamNameDisplay = "";
            if(MainPage._currentlySelectedTeamsList.Count > 1) {
                teamNameDisplay = "Multiple Selected";
            }
            else {
                teamNameDisplay = MainPage._currentlySelectedTeamsList[0].name;
            }

            TextBlock_TeamName.Text = teamNameDisplay;
            TextBlock_EventName.Text = MainPage._currentlySelectedEvent;

            // Also, some resizing the first time this ContentDialog is pulled up (don't need it as wide as typical)
            DependencyObject dObj = HelperMethods.GetFirstVisualTreeElementWithName(this, "BackgroundElement");
            Border border = dObj as Border;
            if (border == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new Border()).GetType(), dObj.GetType());
            }

            border.MaxHeight = 700;
            border.MinHeight = 300;
            border.MaxWidth = 600;
            border.MinWidth = 400;

            this.MaxHeight = 700;
            this.MinHeight = 300;
            this.MaxWidth = 600;
            this.MinWidth = 400;
            this.Height = 650; // the desired value
            this.Width = 400; // the desired value

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

            Grid editGymnastsGrid = EditGymnastsForEvent_UserControl.Content as Grid;
            if (editGymnastsGrid == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new Grid()).GetType(), EditGymnastsForEvent_UserControl.Content.GetType());
            }

            if (button.Name == "ButtonUpdate") {
                // We have set our data binding to require explicit triggering, so trigger all the appropriate elements here
                foreach (FrameworkElement fe in editGymnastsGrid.Children) {
                    if ((fe is TextBox) && (Grid.GetRow(fe) > 0)) {
                        TextBox tBx = fe as TextBox;
                        BindingExpression bExp = tBx.GetBindingExpression(TextBox.TextProperty);
                        bExp.UpdateSource();
                    }
                }
            }
            else {
                // In this case, cannot have created any new teams, but could have added new gymnasts
                // TO CONSIDER: 
                // Do I want to allow user to add and / or remove a new gymnast directly in this dialog?
                Team t = Meet.GetTeamWithName(TextBlock_TeamName.Text);
                if (t != null) {
                    int gymnastsToTrim = t.rosterSize - _rosterSizeBefore[t.name];
                    Meet.RemoveGymnastsFromTeam_LastN(t, gymnastsToTrim);
                    Meet.ClearEventEditingUpdates();
                }
            }

            Hide();

        }



        private void ContentDialog_EditTeamForEvent_OpenedLoadGymnasts(ContentDialog sender, ContentDialogOpenedEventArgs args) {

            ContentDialog cd = sender as ContentDialog;
            if (cd == null) {
                throw new Exception("EXCEPTION >> Unexpected type for sender on ContentDialog loaded event; expected ContentDialog; actual " + sender.GetType().Name);
            }

            TextBlock tempTBx = new TextBlock();

            DependencyObject temp = HelperMethods.GetFirstVisualTreeElementWithName(cd, "ButtonCancel");
            Button buttonCancel = temp as Button;
            if (buttonCancel != null) {

                buttonCancel.Focus(FocusState.Keyboard);
            }

            if (MainPage._currentlySelectedTeamsList != null) {
                foreach(Team team in MainPage._currentlySelectedTeamsList) {
                    _rosterSizeBefore.Add(team.name, team.rosterSize);
                    Meet.AddTeamTo_UserControl_EditGymnastForEvent(team, EditGymnastsForEvent_UserControl);
                }
            }
        }

        private void ContentDialog_EditTeamForEvent_Closed(ContentDialog sender, ContentDialogClosedEventArgs args) {
            Grid editGymnastsGrid = EditGymnastsForEvent_UserControl.Content as Grid;
            if (editGymnastsGrid == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new Grid()).GetType(), EditGymnastsForEvent_UserControl.Content.GetType());
            }
            HelperMethods.RemoveAllButHeaderRowFromGrid(editGymnastsGrid);

            _rosterSizeBefore.Clear();
        }

    }
}
