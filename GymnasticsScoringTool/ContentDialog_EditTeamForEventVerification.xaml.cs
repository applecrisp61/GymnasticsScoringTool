using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GymnasticsScoringTool {
    public sealed partial class ContentDialog_EditTeamForEventVerification : ContentDialog {

        // private bool _updateCancelButtonsChanged = false;

        public ContentDialog_EditTeamForEventVerification() {
            this.InitializeComponent();

            this.Loaded += ContentDialog_EditTeamForEventVerification_LoadedFormat;
            this.Loaded += ContentDialog_LoadedSetUpdateCancelButtons;

            this.Opened += ContentDialog_EditTeamForEvent_OpenedLoadGymnasts;
        }

        private void ContentDialog_EditTeamForEventVerification_LoadedFormat(object sender, RoutedEventArgs e) {
            string teamNameDisplay = "";
            if (MainPage._currentlySelectedTeamsList.Count > 1) {
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

            border.MaxHeight = 400;
            border.MinHeight = 300;
            border.MaxWidth = 600;
            border.MinWidth = 500;

            this.MaxHeight = 500;
            this.MinHeight = 300;
            this.MaxWidth = 600;
            this.MinWidth = 500;
            this.Height = 450; // the desired value
            this.Width = 500; // the desired value
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

            HelperMethods.RemoveCancelButton_ChangeUpdateToVerify_FromContentDialogControlTemplate(this);

            this.Loaded -= ContentDialog_LoadedSetUpdateCancelButtons;

            // Attempt to give the button Focus so that if the user hits enter, it will do what it expected
            // which is to act as if the user clicked on VERIFY with their mouse
            buttonUpdate.Focus(FocusState.Programmatic);
        }

        private void ButtonContentDialog_Click(object sender, RoutedEventArgs args) {
            Button button = sender as Button;
            if (button == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new Button()).GetType(), sender.GetType());
            }

            Grid editGymnastsGrid = ValidateGymnastsForEvent_UserControl.Content as Grid;
            if (editGymnastsGrid == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new Grid()).GetType(), ValidateGymnastsForEvent_UserControl.Content.GetType());
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

            HelperMethods.RemoveAllButHeaderRowFromGrid(editGymnastsGrid);

            Meet.ClearEventEditingUpdates();
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

            Meet.AddUpdatesTo_UserControl_EditGymnastForEvent(ValidateGymnastsForEvent_UserControl);
           
        }


    }
}
