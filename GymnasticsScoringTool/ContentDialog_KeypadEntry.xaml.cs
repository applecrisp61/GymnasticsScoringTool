using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GymnasticsScoringTool {
    public sealed partial class ContentDialog_KeypadEntry : ContentDialog {
        private string _previousCompetitorNbr = "";
        private string _previousScore = "";

        public ContentDialog_KeypadEntry() {
            this.InitializeComponent();

            this.Loaded += ContentDialog_KeypadEntry_LoadedFormat;
            this.Loaded += ContentDialog_LoadedSetUpdateCancelButtons;

            this.Opened += ContentDialog_KeypadEntry_SetFocusToCancel;
            this.Closed += ContentDialog_KeypadEntry_Closed;
        }

        // ****** EVENT HANDLERS ********************************

        private void ContentDialog_KeypadEntry_LoadedFormat(object sender, RoutedEventArgs e) {

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

            BtnAdd.Click += BtnAdd_Click;
            TbxCompetitorNbr.GotFocus += TbxCompetitorNbr_GotFocus;
            TbxEventScore.GotFocus += TbxEventScore_GotFocus;
            TbxCompetitorNbr.LostFocus += TbxCompetitorNbr_LostFocus;
            TbxEventScore.LostFocus += TbxEventScore_LostFocus;
        }

        private void TbxEventScore_LostFocus(object sender, RoutedEventArgs e) {
            string input = (sender as TextBox)?.Text;
            if (string.IsNullOrEmpty(input)) return;

            var tbx = sender as TextBox;

            string adjString = EventScore.AdjustInputString(input);
            tbx.Text = adjString;

            var score = ValidateAndAlert_EventScore(tbx, adjString);
            if (Math.Abs(score - ProgramConstants.INVALID_EVENT_SCORE) < ProgramConstants.EPSILON) {
                tbx.Text = _previousScore;
            }
        }

        private void TbxCompetitorNbr_LostFocus(object sender, RoutedEventArgs e) {
            string input = (sender as TextBox)?.Text;
            if (string.IsNullOrEmpty(input)) return;

            var tbx = sender as TextBox;

            var competitorNbr = ValidateAndAlert_CompetitorNbr(tbx, input);
            if (competitorNbr == ProgramConstants.INVALID_COMPETITOR_NBR) {
                tbx.Text = _previousCompetitorNbr;
            }
        }

        private void TbxEventScore_GotFocus(object sender, RoutedEventArgs e) {
            (sender as TextBox)?.SelectAll();
            _previousScore = (sender as TextBox)?.Text;
        }

        private void TbxCompetitorNbr_GotFocus(object sender, RoutedEventArgs e) {
            (sender as TextBox)?.SelectAll();
            _previousCompetitorNbr = (sender as TextBox)?.Text;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e) {
            Button btn = sender as Button;
            if (btn == null) {
                throw new Exception("EXCEPTION >> Unexpected type for sender on Button clicked event; expected Button; actual " + sender.GetType().Name);
            }

            string stringCompetitorNbr = TbxCompetitorNbr.Text;
            string stringEventScore = TbxEventScore.Text;

            var competitorNbr = ValidateAndAlert_CompetitorNbr(btn, stringCompetitorNbr);
            if (competitorNbr == ProgramConstants.INVALID_COMPETITOR_NBR) return;
            var eventScore = ValidateAndAlert_EventScore(btn, stringEventScore);
            if (eventScore == ProgramConstants.INVALID_EVENT_SCORE) return;

            Gymnast g = Meet.GetGymnastByNumber(competitorNbr);
            if (g == null) return; // perhaps too many null checks; some are definitely redundant
            EditGymnastsForEvent_UserControl.GymnastToEdit(g);

            EditGymnastsForEvent_UserControl.UpdateGymnastForEvent(competitorNbr, eventScore);

            var es = new EventScore(eventScore);
            Meet.RegisterEventUpdate(competitorNbr, es);

            TbxCompetitorNbr.Text = "";
            TbxEventScore.Text = "";

            TbxCompetitorNbr.Focus(FocusState.Programmatic);
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
                // Do not allow addition of new gymnasts to team from this dialog
            }

            TbxCompetitorNbr.Text = "";
            TbxEventScore.Text = "";

            Hide();
        }

        private void ContentDialog_KeypadEntry_SetFocusToCancel(ContentDialog sender, ContentDialogOpenedEventArgs args) {

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
        }

        private void ContentDialog_KeypadEntry_Closed(ContentDialog sender, ContentDialogClosedEventArgs args) {
            Grid editGymnastsGrid = EditGymnastsForEvent_UserControl.Content as Grid;
            if (editGymnastsGrid == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new Grid()).GetType(), EditGymnastsForEvent_UserControl.Content.GetType());
            }
            HelperMethods.RemoveAllButHeaderRowFromGrid(editGymnastsGrid);
        }


        // ****** HELPER METHODS ********************************

        private double ValidateAndAlert_EventScore(FrameworkElement sender, string adjString) {
            if (EventScore.IsPoorlyFormatted(adjString)) {
                var fly = InvalidScoreFlyout(ProgramConstants.POORLY_FORMATTED_SCORE);
                fly.ShowAt(sender);
                return ProgramConstants.INVALID_EVENT_SCORE;
            }
            else if (EventScore.IsOutsideValidRange(adjString)) {
                var fly = InvalidScoreFlyout(ProgramConstants.SCORE_OUTSIDE_VALID_RANGE);
                fly.ShowAt(sender);
                return ProgramConstants.INVALID_EVENT_SCORE;
            }

            var score = double.Parse(adjString); // Guaranteed to succeed due to previous checks
            return score;
        }

        private int ValidateAndAlert_CompetitorNbr(FrameworkElement sender, string input) {
            int competitorNbr = ProgramConstants.INVALID_COMPETITOR_NBR;
            try {
                competitorNbr = Int32.Parse(input);
            }
            catch {
                var parsefly = InvalidCompetitorNbrFlyout(ProgramConstants.POORLY_FORMATTED_COMPETITOR_NBR);
                parsefly.ShowAt(sender);
                return competitorNbr;
            }

            var output = EditGymnastsForEvent_UserControl.GymnastAlreadyPresent(competitorNbr);

            if (output.Item1) {
                var flyAlreaadyPresent =
                    InvalidCompetitorNbrFlyout(ProgramConstants.KEYPAD_GYMNAST_SCORE_ALREADY_ENTERED);
                competitorNbr = ProgramConstants.INVALID_COMPETITOR_NBR;
                flyAlreaadyPresent.ShowAt(sender);
                return competitorNbr;
            }

            if (Meet.ContainsGymnast(competitorNbr)) return competitorNbr;

            var fly = InvalidCompetitorNbrFlyout(ProgramConstants.INVALID_COMPETITOR_NBR_MSG);
            fly.ShowAt(sender as TextBox);
            competitorNbr = ProgramConstants.INVALID_COMPETITOR_NBR;

            return competitorNbr;
        }

        private Flyout InvalidScoreFlyout(string invalidScoreMessage) {
            var invalidScoreFlyout = new Flyout();
            invalidScoreFlyout.Placement = FlyoutPlacementMode.Full;
            var invalidScoreTextBlock = new TextBlock();
            invalidScoreTextBlock.Text = invalidScoreMessage;
            TextBlock invalidScoreTitle = new TextBlock();
            invalidScoreTitle.Text = ProgramConstants.TITLE_INVALID_SCORE;
            invalidScoreTitle.FontSize = FontSize + 2;
            invalidScoreTitle.FontWeight = Windows.UI.Text.FontWeights.Bold;
            StackPanel invalidScoreStackPanel = new StackPanel();
            invalidScoreStackPanel.Children.Add(invalidScoreTitle);
            invalidScoreStackPanel.Children.Add(invalidScoreTextBlock);
            invalidScoreFlyout.Content = invalidScoreStackPanel;

            return invalidScoreFlyout;
        }

        private Flyout InvalidCompetitorNbrFlyout(string invalidCompetitorNbrMessage) {
            var invalidCompetitorNbrFlyout = new Flyout();
            invalidCompetitorNbrFlyout.Placement = FlyoutPlacementMode.Full;
            var invalidCompetitorNbrTextBlock = new TextBlock();
            invalidCompetitorNbrTextBlock.Text = invalidCompetitorNbrMessage;
            TextBlock invalidCompetitorNbrTitle = new TextBlock();
            invalidCompetitorNbrTitle.Text = ProgramConstants.TITLE_INVALID_COMPETITOR_NBR;
            invalidCompetitorNbrTitle.FontSize = FontSize + 2;
            invalidCompetitorNbrTitle.FontWeight = Windows.UI.Text.FontWeights.Bold;
            StackPanel invalidScoreStackPanel = new StackPanel();
            invalidScoreStackPanel.Children.Add(invalidCompetitorNbrTitle);
            invalidScoreStackPanel.Children.Add(invalidCompetitorNbrTextBlock);
            invalidCompetitorNbrFlyout.Content = invalidScoreStackPanel;

            return invalidCompetitorNbrFlyout;
        }
    }
}
