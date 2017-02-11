using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GymnasticsScoringTool {
    public sealed partial class ContentDialog_MeetParameters : ContentDialog {

        private string _previousValue;
        private Flyout _invalidParameterFlyout;
        private TextBlock _invalidParameterTextBlock;
        private TextBlock _invalidParameterTitle;

        public ContentDialog_MeetParameters(string title = "Edit Meet Configuration", string name = "ContentDialog_EditMeetParameters") {
            this.InitializeComponent();

            _invalidParameterFlyout = new Flyout();
            _invalidParameterFlyout.Placement = FlyoutPlacementMode.Full;
            _invalidParameterTextBlock = new TextBlock();
            _invalidParameterTextBlock.Text = "No invalid parameter message associated";
            _invalidParameterTitle = new TextBlock();
            _invalidParameterTitle.Text = ProgramConstants.TITLE_INVALID_INTEGER_PARAM;
            _invalidParameterTitle.FontSize = FontSize + 2;
            _invalidParameterTitle.FontWeight = Windows.UI.Text.FontWeights.Bold;
            StackPanel invalidScoreStackPanel = new StackPanel();
            invalidScoreStackPanel.Children.Add(_invalidParameterTitle);
            invalidScoreStackPanel.Children.Add(_invalidParameterTextBlock);
            _invalidParameterFlyout.Content = invalidScoreStackPanel;

            #region EstablishDataBindings.
            Binding bindingName = new Binding();
            Binding bindingDate = new Binding();
            Binding bindingLocation = new Binding();
            Binding bindingMaxScore = new Binding();
            Binding bindingMinScore = new Binding();
            Binding bindingGymnastsPerEvent = new Binding();
            Binding bindingScoresToUse = new Binding();

            bindingName.Source = Meet.meetParameters;
            bindingDate.Source = Meet.meetParameters;
            bindingLocation.Source = Meet.meetParameters;
            bindingMaxScore.Source = Meet.meetParameters;
            bindingMinScore.Source = Meet.meetParameters;
            bindingGymnastsPerEvent.Source = Meet.meetParameters;
            bindingScoresToUse.Source = Meet.meetParameters;

            bindingName.Path = new PropertyPath("meetName");
            bindingDate.Path = new PropertyPath("meetDate");
            bindingLocation.Path = new PropertyPath("meetLocation");
            bindingMaxScore.Path = new PropertyPath("maxScore");
            bindingMinScore.Path = new PropertyPath("minScore");
            bindingGymnastsPerEvent.Path = new PropertyPath("competitorsPerTeam");
            bindingScoresToUse.Path = new PropertyPath("scoresForCompetition");

            bindingName.Mode = BindingMode.TwoWay;
            bindingDate.Mode = BindingMode.TwoWay;
            bindingLocation.Mode = BindingMode.TwoWay;
            bindingMaxScore.Mode = BindingMode.TwoWay;
            bindingMinScore.Mode = BindingMode.TwoWay;
            bindingGymnastsPerEvent.Mode = BindingMode.TwoWay;
            bindingScoresToUse.Mode = BindingMode.TwoWay;

            bindingMaxScore.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
            bindingMinScore.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
            bindingGymnastsPerEvent.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
            bindingScoresToUse.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;

            BindingOperations.SetBinding(textBox_meetName, TextBox.TextProperty, bindingName);
            BindingOperations.SetBinding(textBox_meetDate, TextBox.TextProperty, bindingDate);
            BindingOperations.SetBinding(textBox_meetLocation, TextBox.TextProperty, bindingLocation);
            BindingOperations.SetBinding(textBox_maxScore, TextBox.TextProperty, bindingMaxScore);
            BindingOperations.SetBinding(textBox_minScore, TextBox.TextProperty, bindingMinScore);
            BindingOperations.SetBinding(textBox_competitorsPerTeam, TextBox.TextProperty, bindingGymnastsPerEvent);
            BindingOperations.SetBinding(textBox_scoresForCompetition, TextBox.TextProperty, bindingScoresToUse);
            #endregion EstablishDataBindings.

            Title = title;
            Name = name;
            this.Loaded += ContentDialog_LoadedSetUpdateCancelButtons;

            // TODO: Figure out best event to use to move the focus off the first text box and onto ButtonCancel
            //
            // FRUSTRATING!! The focus seems to be set to the first text box AFTER the Loaded event;
            // therefore, I am using the Opened event, which seems to work in most cases. 
            // However, I don't understand the timing on the Opened event, which seems to be variable!
            // 
            // Without testing for presence of ButtonCancel, every once in a while would encounter a null reference 
            // exception stating that the buttonCancel variable contained a null reference
            // (set through Button buttonCancel = temp as Button; statement)
            // 
            // So now I test for this and do nothing when buttonCancel is null, but this is variable and frustrating!!
            this.Opened += ContentDialog_Opened;
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


        private void ContentDialog_Opened(object sender, ContentDialogOpenedEventArgs e) {

            ContentDialog cd = sender as ContentDialog;
            if (cd == null) {
                throw new Exception("EXCEPTION >> Unexpected type for sender on ContentDialog loaded event; expected ContentDialog; actual " + sender.GetType().Name);
            }

            DependencyObject temp = HelperMethods.GetFirstVisualTreeElementWithName(cd, "ButtonCancel");
            Button buttonCancel = temp as Button;
            if (buttonCancel != null) {
                buttonCancel.Focus(FocusState.Keyboard);
            }

            textBox_meetName.Text = Meet.meetParameters.meetName;
            textBox_meetDate.Text = Meet.meetParameters.meetDate;
            textBox_meetLocation.Text = Meet.meetParameters.meetLocation;
            textBox_minScore.Text = Meet.meetParameters.minScore.ToString();
            textBox_maxScore.Text = Meet.meetParameters.maxScore.ToString();
            textBox_competitorsPerTeam.Text = Meet.meetParameters.competitorsPerTeam.ToString();
            textBox_scoresForCompetition.Text = Meet.meetParameters.scoresForCompetition.ToString();

        }

        private void ButtonContentDialog_Click(object sender, RoutedEventArgs e) {
            Button button = sender as Button;
            if (button == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new Button()).GetType(), sender.GetType());
            }

            BindingExpression bExp = textBox_meetName.GetBindingExpression(TextBox.TextProperty);
            bExp.UpdateSource();

            bExp = textBox_meetDate.GetBindingExpression(TextBox.TextProperty);
            bExp.UpdateSource();

            bExp = textBox_meetLocation.GetBindingExpression(TextBox.TextProperty);
            bExp.UpdateSource();

            if(HelperMethods.IsValid_PositiveCappedDouble(textBox_minScore.Text)) {
                bExp = textBox_minScore.GetBindingExpression(TextBox.TextProperty);
                bExp.UpdateSource();
            }

            if (HelperMethods.IsValid_PositiveCappedDouble(textBox_maxScore.Text)) {
                bExp = textBox_maxScore.GetBindingExpression(TextBox.TextProperty);
                bExp.UpdateSource();
            }

            if (HelperMethods.IsValid_UInt16(textBox_competitorsPerTeam.Text)) {
                bExp = textBox_competitorsPerTeam.GetBindingExpression(TextBox.TextProperty);
                bExp.UpdateSource();
            }

            if (HelperMethods.IsValid_UInt16(textBox_scoresForCompetition.Text)) {
                bExp = textBox_scoresForCompetition.GetBindingExpression(TextBox.TextProperty);
                bExp.UpdateSource();
            }

            Hide();
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
        }

        private void textBox_LostFocus_Textual(object sender, RoutedEventArgs e) {
            TextBox textBox = sender as TextBox;
            if (textBox == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new TextBox()).GetType(), sender.GetType());
            }

            if (textBox.Text == "") { textBox.Text = _previousValue; return; }
        }

        private void textBox_LostFocus_UInt16(object sender, RoutedEventArgs e) {
            TextBox textBox = sender as TextBox;
            if (textBox == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new TextBox()).GetType(), sender.GetType());
            }

            if (textBox.Text == "") { textBox.Text = _previousValue; return; }

            textBox_LostFocus_ValidateParameterInput_UInt16(textBox);
        }

        private void textBox_LostFocus_Double(object sender, RoutedEventArgs e) {
            TextBox textBox = sender as TextBox;
            if (textBox == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new TextBox()).GetType(), sender.GetType());
            }

            if (textBox.Text == "") { textBox.Text = _previousValue; return; }

            textBox_LostFocus_ValidateParameterInput_Double(textBox);
        }

        private void textBox_LostFocus_ValidateParameterInput_UInt16(TextBox textBox) {

            if (HelperMethods.IsValid_UInt16(textBox.Text)) {
                // It's good... but nothing to do here, as update of binding expression will
                // fire based upon hitting the update button
            }
            else {

                _invalidParameterTitle.Text = ProgramConstants.TITLE_INVALID_INTEGER_PARAM;
                if (HelperMethods.IsPoorlyFormatted_UInt16(textBox.Text)) {
                    _invalidParameterTextBlock.Text = ProgramConstants.POORLY_FORMATTED_INTEGER_PARAM;
                }
                else if (HelperMethods.IsOutsideValidRange_UInt16(textBox.Text)) {
                    _invalidParameterTextBlock.Text = ProgramConstants.INTEGER_PARAM_OUTSIDE_VALID_RANGE;
                }
                else {
                    _invalidParameterTextBlock.Text = "Invalid parameter >> reason unknown (indicates programming error... should not see this message)";
                }

                _invalidParameterFlyout.ShowAt(textBox);
            }
        }

        private void textBox_LostFocus_ValidateParameterInput_Double(TextBox textBox) {

            if (HelperMethods.IsValid_PositiveCappedDouble(textBox.Text)) {
                // It's good... but nothing to do here, as update of binding expression will
                // fire based upon hitting the update button
            }
            else {
                _invalidParameterTitle.Text = ProgramConstants.TITLE_INVALID_DECIMAL_PARAM;
                if (HelperMethods.IsPoorlyFormatted_PositiveCappedDouble(textBox.Text)) {
                    _invalidParameterTextBlock.Text = ProgramConstants.POORLY_FORMATTED_DECIMAL_PARAM;
                }
                else if (HelperMethods.IsOutsideValidRange_PositiveCappedDouble(textBox.Text)) {
                    _invalidParameterTextBlock.Text = ProgramConstants.DECIMAL_PARAM_OUTSIDE_VALID_RANGE;
                }
                else {
                    _invalidParameterTextBlock.Text = "Invalid parameter >> reason unknown (indicates programming error... should not see this message)";
                }

                _invalidParameterFlyout.ShowAt(textBox);
            }

        }

    }
}
