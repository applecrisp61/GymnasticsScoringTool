using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GymnasticsScoringTool_01 {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CalculateQualifiers_Page : Page {

        private PrintHelper _printHelper;

        private string _previousValue;
        private Flyout _invalidParameterFlyout;
        private TextBlock _invalidParameterTextBlock;

        public CalculateQualifiers_Page() {
            this.InitializeComponent();

            _invalidParameterFlyout = new Flyout();
            _invalidParameterFlyout.Placement = FlyoutPlacementMode.Full;
            _invalidParameterTextBlock = new TextBlock();
            _invalidParameterTextBlock.Text = "No invalid parameter message associated";
            TextBlock invalidScoreTitle = new TextBlock();
            invalidScoreTitle.Text = ProgramConstants.TITLE_INVALID_INTEGER_PARAM;
            invalidScoreTitle.FontSize = FontSize + 2;
            invalidScoreTitle.FontWeight = Windows.UI.Text.FontWeights.Bold;
            StackPanel invalidScoreStackPanel = new StackPanel();
            invalidScoreStackPanel.Children.Add(invalidScoreTitle);
            invalidScoreStackPanel.Children.Add(_invalidParameterTextBlock);
            _invalidParameterFlyout.Content = invalidScoreStackPanel;

            #region EstablishDataBindings.
            Binding bindingNextMeet = new Binding();
            Binding bindingTeamQualifiers = new Binding();
            Binding bindingAAQualifiers = new Binding();
            Binding bindingEventQualifiers = new Binding();

            bindingNextMeet.Source = Meet.qualificationParameters;
            bindingTeamQualifiers.Source = Meet.qualificationParameters;
            bindingAAQualifiers.Source = Meet.qualificationParameters;
            bindingEventQualifiers.Source = Meet.qualificationParameters;

            bindingNextMeet.Path = new PropertyPath("meetQualfiedFor");
            bindingTeamQualifiers.Path = new PropertyPath("teamQualifiers");
            bindingAAQualifiers.Path = new PropertyPath("aaQualifiers");
            bindingEventQualifiers.Path = new PropertyPath("eventQualifiers");

            bindingNextMeet.Mode = BindingMode.TwoWay;
            bindingTeamQualifiers.Mode = BindingMode.TwoWay;
            bindingAAQualifiers.Mode = BindingMode.TwoWay;
            bindingEventQualifiers.Mode = BindingMode.TwoWay;

            // Set the update to be explicit so we can validate the input is ok before we send it off for update
            bindingTeamQualifiers.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
            bindingAAQualifiers.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
            bindingEventQualifiers.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;

            BindingOperations.SetBinding(MeetQualifiedFor, TextBox.TextProperty, bindingNextMeet);
            BindingOperations.SetBinding(tbx_teamQualifiers, TextBox.TextProperty, bindingTeamQualifiers);
            BindingOperations.SetBinding(tbx_aaQualifiers, TextBox.TextProperty, bindingAAQualifiers);
            BindingOperations.SetBinding(tbx_eventQualifiers, TextBox.TextProperty, bindingEventQualifiers);
            #endregion EstablishDataBindings.

            // Note that ExcludedGymnasts does not participate in data binding
            if(Meet.qualificationParameters.gymnastsToExclude==null) {
                Meet.qualificationParameters.gymnastsToExclude = new List<int>();
            }
            ExcludedGymnasts.Text = QualificationParameters.displayCompetitorNbrsToExclude(Meet.qualificationParameters.gymnastsToExclude);
            RefreshQualifiersDisplay();

        }

        /*
        protected override void OnNavigatedTo(NavigationEventArgs args) {

            // Want to display the results both on this page (which will be nicely formatted for the computer screen) 
            // and on a second new page which will not be a member of the current visual tree (which will be used to 
            // configure the results to be nicely formatted for the printed page)

            // _pageForPrinting = new Qualifiers_PageToPrint();

            // PrintHelper.PopulateQualificationBlocks_OnlyText(_pageForPrinting);

            // register for the Printer contract
            // if (_printHelper == null) {
            //     _printHelper = new PrintHelper(this);
            // }
            // _printHelper.RegisterForPrinting();
            // _printHelper.PreparePrintContent(_pageForPrinting);

            base.OnNavigatedTo(args);
        }
        */

        protected override void OnNavigatedFrom(NavigationEventArgs args) {
            if (_printHelper != null) {
                _printHelper.UnregisterForPrinting();
            }

            base.OnNavigatedFrom(args);
        }

        private async void Button_ClickPrintQualifiers(object sender, RoutedEventArgs e) {
            try {
                // Show print UI
                await Windows.Graphics.Printing.PrintManager.ShowPrintUIAsync();

            }
            catch {
                // Printing cannot proceed at this time
                ContentDialog noPrintingDialog = new ContentDialog() {
                    Title = "Printing error",
                    Content = "\nSorry, printing can' t proceed at this time.",
                    PrimaryButtonText = "OK"
                };
                await noPrintingDialog.ShowAsync();
            }
        }

        private void Button_ClickReturnToMainPage(object sender, RoutedEventArgs args) {
            this.Frame.Navigate(typeof(MainPage));
        }


        private void RefreshQualifiersDisplay() {
            var toDisplay = Meet.DisplayQualifiers(MainPage._currentlySelectedDivision);
            QualifiersListing_SP.Children.Clear();
            foreach (TextBlock t in toDisplay) {
                QualifiersListing_SP.Children.Add(t);
            }

            // subsequent times through: there can be only one! (after any qualification criteria updates)
            if (_printHelper != null) {
                _printHelper.UnregisterForPrinting();
            }

            _printHelper = new PrintHelper(this);
            Page pageForPrinting = new Qualifiers_PageToPrint();
            PrintHelper.PopulateQualificationBlocks_OnlyText(pageForPrinting);
            _printHelper.RegisterForPrinting();
            _printHelper.PreparePrintContent(pageForPrinting);
        }

        private void textBox_GotFocus(object sender, RoutedEventArgs e) {
            TextBox textBox = sender as TextBox;
            if (textBox == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new TextBox()).GetType(), sender.GetType());
            }

            _previousValue = textBox.Text;
            textBox.SelectAll();
        }

        private void textBox_LostFocus_UInt16(object sender, RoutedEventArgs args) {
            TextBox textBox = sender as TextBox;
            if (textBox == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new TextBox()).GetType(), sender.GetType());
            }

            if (textBox.Text == "") { textBox.Text = _previousValue; return; }

            textBox_LostFocus_ValidateParameterInput_UInt16(textBox);
        }


        private void textBox_LostFocus_ValidateParameterInput_UInt16(TextBox textBox) {

            if (HelperMethods.IsValid_UInt16(textBox.Text)) {
                // It's good... trigger an update of the data bindings
                BindingExpression bExp = textBox.GetBindingExpression(TextBox.TextProperty);
                bExp.UpdateSource();
                RefreshQualifiersDisplay();
            }
            else {
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

        private void textBox_LostFocus_Textual(object sender, RoutedEventArgs args) {
            TextBox textBox = sender as TextBox;
            if (textBox == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new TextBox()).GetType(), sender.GetType());
            }

            if (textBox.Text == "") { textBox.Text = _previousValue; return; }

            RefreshQualifiersDisplay();
        }

        private void textBox_LostFocus_ExclusionList(object sender, RoutedEventArgs args) {
            TextBox textBox = sender as TextBox;
            if (textBox == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new TextBox()).GetType(), sender.GetType());
            }

            if (textBox.Text == "") { textBox.Text = _previousValue; return; }

            List<int> exclusionList = new List<int>();
            bool validUpdate = true;
            try {
                exclusionList = QualificationParameters.parseCompetitorNbrsToExclude(textBox.Text);
            }
            catch {
                validUpdate = false;
                _invalidParameterTextBlock.Text = "Poorly formed competitor exclusion list" + Environment.NewLine + Environment.NewLine + "No update will be made";
                _invalidParameterFlyout.ShowAt(textBox);
                textBox.Text = QualificationParameters.displayCompetitorNbrsToExclude(Meet.qualificationParameters.gymnastsToExclude);
            }

            if(validUpdate) {
                Meet.qualificationParameters.gymnastsToExclude = exclusionList;
                RefreshQualifiersDisplay();
            }

        }


    }
}
