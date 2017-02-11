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
using Windows.UI.Xaml.Documents;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GymnasticsScoringTool_01 {

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OutputResultsPage : Page {

        private PrintHelper _printHelper;

        public OutputResultsPage() {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs args) {

            // Want to display the results both on this page (which will be nicely formatted for the computer screen) 
            // and on a second new page which will not be a member of the current visual tree (which will be used to 
            // configure the results to be nicely formatted for the printed page)

            Page pageForPrinting = new Results_PageToPrint();

            if (MainPage._outputPlainTextOnly==true) {
                PrintHelper.PopulateResultsBlocks_OnlyText(this);
                PrintHelper.PopulateResultsBlocks_OnlyText(pageForPrinting);
            }
            else {
                PrintHelper.PopulateResultsBlocks_UIElements(this);
                PrintHelper.PopulateResultsBlocks_UIElements(pageForPrinting);
            }



            // register for the Printer contract
            if (_printHelper==null) {
                _printHelper = new PrintHelper(this);
            }
            _printHelper.RegisterForPrinting();
            _printHelper.PreparePrintContent(pageForPrinting);

            base.OnNavigatedTo(args);
        }



        protected override void OnNavigatedFrom(NavigationEventArgs args) {
            if(_printHelper!=null) {
                _printHelper.UnregisterForPrinting();
            }

            base.OnNavigatedFrom(args);
        }

        private void Button_ClickReturnToMainPage(object sender, RoutedEventArgs args) {
            this.Frame.Navigate(typeof(MainPage));
        }

        private async void Button_ClickPrintMeetResults_Formatted(object sender, RoutedEventArgs e) {

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

        private async void Button_ClickPrintMeetResults_Text(object sender, RoutedEventArgs e) {

            Page pageForPrinting = new Results_PageToPrint();
            PrintHelper.PopulateResultsBlocks_UIElements(pageForPrinting);
            _printHelper.PreparePrintContent(pageForPrinting);

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

    }
}
