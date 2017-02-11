using System;
using System.Collections.Generic;
using System.Threading.Tasks;
// using SDKTemplate;
using Windows.Graphics.Printing;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Printing;
using Windows.UI.Xaml.Documents;

namespace GymnasticsScoringTool_01 {


    public class PrintHelper {

        #region DCL added helper functions
        internal static void PopulateResultsBlocks_UIElements(Page page) {
            string pageVisualTreeString = HelperMethods.PlaceVisualTreeIntoOutputString(page, 0);

            DependencyObject dObj = HelperMethods.GetFirstVisualTreeElementWithName(page, "TextContent");
            if (dObj == null) { throw new Exception_CouldNotFindUIElement("TextContent", (new RichTextBlock()).GetType(), page.GetType(), pageVisualTreeString); }
            RichTextBlock textContent = dObj as RichTextBlock;
            if (textContent == null) { throw new Exception_UnexpectedDataTypeEncountered((new Paragraph()).GetType(), dObj.GetType()); }

            var standardSpacing = new Thickness(0, 5, 0, 20);

            dObj = HelperMethods.GetFirstVisualTreeElementWithName(page, "paragraph_MeetHeader");
            if(dObj==null) { throw new Exception_CouldNotFindUIElement("paragraph_MeetHeader", (new Paragraph()).GetType(), page.GetType(), pageVisualTreeString); }
            Paragraph paragraph_MeetHeader = dObj as Paragraph;
            if(paragraph_MeetHeader == null) { throw new Exception_UnexpectedDataTypeEncountered((new Paragraph()).GetType(), dObj.GetType()); }

            paragraph_MeetHeader.Inlines.Clear();
            paragraph_MeetHeader.Inlines.Add(new Run() {
                Text = Meet.meetParameters.meetName,
                FontSize =24,
                FontWeight =Windows.UI.Text.FontWeights.ExtraBold
            });
            paragraph_MeetHeader.Inlines.Add(new Run() {
                Text = Environment.NewLine + Meet.meetParameters.meetDate,
                FontSize = 18,
                FontWeight = Windows.UI.Text.FontWeights.Bold
            });
            paragraph_MeetHeader.Inlines.Add(new Run() {
                Text = Environment.NewLine + Meet.meetParameters.meetLocation,
                FontSize = 18,
                FontWeight = Windows.UI.Text.FontWeights.Bold
            });


            Paragraph paragraph_TeamHeader = new Paragraph();
            paragraph_TeamHeader.FontSize = 16;
            paragraph_TeamHeader.FontWeight = Windows.UI.Text.FontWeights.Bold;
            paragraph_TeamHeader.Margin = standardSpacing;
            paragraph_TeamHeader.Inlines.Add(new Run() { Text = "Team Standings" });
            textContent.Blocks.Add(paragraph_TeamHeader);

            var teamResultsList = Meet.create_TeamStandings_Complex(MainPage._currentlySelectedDivision);
            foreach(var teamListing in teamResultsList) {
                var teamListingParagraph = new Paragraph();
                teamListingParagraph.Inlines.Add(new InlineUIContainer() { Child = teamListing });
                textContent.Blocks.Add(teamListingParagraph);
            }
            textContent.Blocks.Add(new Paragraph() { Inlines = { new LineBreak() } });

            Paragraph paragraph_VaultHeader = new Paragraph();
            paragraph_VaultHeader.FontSize = 16;
            paragraph_VaultHeader.FontWeight = Windows.UI.Text.FontWeights.Bold;
            paragraph_VaultHeader.Margin = standardSpacing;
            paragraph_VaultHeader.Inlines.Add(new Run() { Text = "Event Standings: Vault" });
            textContent.Blocks.Add(paragraph_VaultHeader);

            var vaultResultsList = UserControl_RankingDisplayGymnast.RankingDisplayOfGymnasts_ForPrinting("Vault", MainPage._currentlySelectedDivision);
            foreach(var resultsSubGroup in vaultResultsList) {
                var resultsSubGroupParagraph = new Paragraph();
                resultsSubGroupParagraph.Inlines.Add(new InlineUIContainer() { Child = resultsSubGroup });
                textContent.Blocks.Add(resultsSubGroupParagraph);
            }
            textContent.Blocks.Add(new Paragraph() { Inlines = { new LineBreak() } });

            Paragraph paragraph_BarsHeader = new Paragraph();
            paragraph_BarsHeader.FontSize = 16;
            paragraph_BarsHeader.FontWeight = Windows.UI.Text.FontWeights.Bold;
            paragraph_BarsHeader.Margin = standardSpacing;
            paragraph_BarsHeader.Inlines.Add(new Run() { Text = "Event Standings: Bars" });
            textContent.Blocks.Add(paragraph_BarsHeader);

            var barsResultsList = UserControl_RankingDisplayGymnast.RankingDisplayOfGymnasts_ForPrinting("Bars", MainPage._currentlySelectedDivision);
            foreach (var resultsSubGroup in barsResultsList) {
                var resultsSubGroupParagraph = new Paragraph();
                resultsSubGroupParagraph.Inlines.Add(new InlineUIContainer() { Child = resultsSubGroup });
                textContent.Blocks.Add(resultsSubGroupParagraph);
            }
            textContent.Blocks.Add(new Paragraph() { Inlines = { new LineBreak() } });

            Paragraph paragraph_BeamHeader = new Paragraph();
            paragraph_BeamHeader.FontSize = 16;
            paragraph_BeamHeader.FontWeight = Windows.UI.Text.FontWeights.Bold;
            paragraph_BeamHeader.Margin = standardSpacing;
            paragraph_BeamHeader.Inlines.Add(new Run() { Text = "Event Standings: Beam" });
            textContent.Blocks.Add(paragraph_BeamHeader);

            var beamResultsList = UserControl_RankingDisplayGymnast.RankingDisplayOfGymnasts_ForPrinting("Beam", MainPage._currentlySelectedDivision);
            foreach (var resultsSubGroup in beamResultsList) {
                var resultsSubGroupParagraph = new Paragraph();
                resultsSubGroupParagraph.Inlines.Add(new InlineUIContainer() { Child = resultsSubGroup });
                textContent.Blocks.Add(resultsSubGroupParagraph);
            }
            textContent.Blocks.Add(new Paragraph() { Inlines = { new LineBreak() } });

            Paragraph paragraph_FloorHeader = new Paragraph();
            paragraph_FloorHeader.FontSize = 16;
            paragraph_FloorHeader.FontWeight = Windows.UI.Text.FontWeights.Bold;
            paragraph_FloorHeader.Margin = standardSpacing;
            paragraph_FloorHeader.Inlines.Add(new Run() { Text = "Event Standings: Floor" });
            textContent.Blocks.Add(paragraph_FloorHeader);

            var floorResultsList = UserControl_RankingDisplayGymnast.RankingDisplayOfGymnasts_ForPrinting("Floor", MainPage._currentlySelectedDivision);
            foreach (var resultsSubGroup in floorResultsList) {
                var resultsSubGroupParagraph = new Paragraph();
                resultsSubGroupParagraph.Inlines.Add(new InlineUIContainer() { Child = resultsSubGroup });
                textContent.Blocks.Add(resultsSubGroupParagraph);
            }
            textContent.Blocks.Add(new Paragraph() { Inlines = { new LineBreak() } });

            Paragraph paragraph_AllAroundHeader = new Paragraph();
            paragraph_AllAroundHeader.FontSize = 16;
            paragraph_AllAroundHeader.FontWeight = Windows.UI.Text.FontWeights.Bold;
            paragraph_AllAroundHeader.Margin = standardSpacing;
            paragraph_AllAroundHeader.Inlines.Add(new Run() { Text = "Individual All Around Standings" });
            textContent.Blocks.Add(paragraph_AllAroundHeader);

            var allAroundResultsList = UserControl_RankingDisplayGymnast.RankingDisplayOfGymnasts_ForPrinting("All Around", MainPage._currentlySelectedDivision);
            foreach (var resultsSubGroup in allAroundResultsList) {
                var resultsSubGroupParagraph = new Paragraph();
                resultsSubGroupParagraph.Inlines.Add(new InlineUIContainer() { Child = resultsSubGroup });
                textContent.Blocks.Add(resultsSubGroupParagraph);
            }
            textContent.Blocks.Add(new Paragraph() { Inlines = { new LineBreak() } });
        }


        internal static void PopulateResultsBlocks_OnlyText(Page page) {
            string pageVisualTreeString = HelperMethods.PlaceVisualTreeIntoOutputString(page, 0);

            DependencyObject dObj = HelperMethods.GetFirstVisualTreeElementWithName(page, "TextContent");
            if (dObj == null) { throw new Exception_CouldNotFindUIElement("TextContent", (new RichTextBlock()).GetType(), page.GetType(), pageVisualTreeString); }
            RichTextBlock textContent = dObj as RichTextBlock;
            if (textContent == null) { throw new Exception_UnexpectedDataTypeEncountered((new Paragraph()).GetType(), dObj.GetType()); }

            var standardSpacing = new Thickness(0, 20, 0, 0);

            dObj = HelperMethods.GetFirstVisualTreeElementWithName(page, "paragraph_MeetHeader");
            if (dObj == null) { throw new Exception_CouldNotFindUIElement("paragraph_MeetHeader", (new Paragraph()).GetType(), page.GetType(), pageVisualTreeString); }
            Paragraph paragraph_MeetHeader = dObj as Paragraph;
            if (paragraph_MeetHeader == null) { throw new Exception_UnexpectedDataTypeEncountered((new Paragraph()).GetType(), dObj.GetType()); }

            var monoFontFamily = new Windows.UI.Xaml.Media.FontFamily("Courier New");

            paragraph_MeetHeader.Inlines.Clear();
            paragraph_MeetHeader.FontFamily = monoFontFamily;
            paragraph_MeetHeader.Inlines.Add(new Run() {
                Text = Meet.meetParameters.meetName,
                FontSize = 24,
                FontWeight = Windows.UI.Text.FontWeights.ExtraBold
            });
            paragraph_MeetHeader.Inlines.Add(new Run() {
                Text = Environment.NewLine + Meet.meetParameters.meetDate,
                FontSize = 18,
                FontWeight = Windows.UI.Text.FontWeights.Bold
            });
            paragraph_MeetHeader.Inlines.Add(new Run() {
                Text = Environment.NewLine + Meet.meetParameters.meetLocation,
                FontSize = 18,
                FontWeight = Windows.UI.Text.FontWeights.Bold
            });

            Paragraph paragraph_TeamHeader = new Paragraph();
            paragraph_TeamHeader.FontSize = 16;
            paragraph_TeamHeader.FontFamily = monoFontFamily;
            paragraph_TeamHeader.FontWeight = Windows.UI.Text.FontWeights.Bold;
            paragraph_TeamHeader.Margin = standardSpacing;
            paragraph_TeamHeader.Inlines.Add(new Run() { Text = "Team Standings" });
            textContent.Blocks.Add(paragraph_TeamHeader);
            
            var teamListings = Meet.create_TeamStandings_InlineCollection(MainPage._currentlySelectedDivision);
            
            foreach (var listing in teamListings) {
                bool bold = true;
                foreach(var line in listing) {
                    var paragraph_teamListing = new Paragraph();
                    if (bold) {
                        line.FontWeight = Windows.UI.Text.FontWeights.Bold;
                        paragraph_teamListing.Margin = standardSpacing;
                        bold = false;
                    }
                    paragraph_teamListing.FontFamily = monoFontFamily;
                    paragraph_teamListing.Inlines.Add(line);
                    textContent.Blocks.Add(paragraph_teamListing);
                }
            }
            
            

            Paragraph paragraph_VaultHeader = new Paragraph();
            paragraph_VaultHeader.FontFamily = monoFontFamily;
            paragraph_VaultHeader.FontSize = 16;
            paragraph_VaultHeader.FontWeight = Windows.UI.Text.FontWeights.Bold;
            paragraph_VaultHeader.Margin = standardSpacing;
            paragraph_VaultHeader.Inlines.Add(new Run() { Text = "Event Standings: Vault" });
            textContent.Blocks.Add(paragraph_VaultHeader);

            var vaultListingsParagraph = new Paragraph();
            vaultListingsParagraph.FontFamily = monoFontFamily;
            vaultListingsParagraph.Inlines.Add(new Run() { Text = Meet.createString_gymnastStandings("Vault", MainPage._currentlySelectedDivision) });
            textContent.Blocks.Add(vaultListingsParagraph);

            Paragraph paragraph_BarsHeader = new Paragraph();
            paragraph_BarsHeader.FontFamily = monoFontFamily;
            paragraph_BarsHeader.FontSize = 16;
            paragraph_BarsHeader.FontWeight = Windows.UI.Text.FontWeights.Bold;
            paragraph_BarsHeader.Margin = standardSpacing;
            paragraph_BarsHeader.Inlines.Add(new Run() { Text = "Event Standings: Bars" });
            textContent.Blocks.Add(paragraph_BarsHeader);

            var barsListingsParagraph = new Paragraph();
            barsListingsParagraph.FontFamily = monoFontFamily;
            barsListingsParagraph.Inlines.Add(new Run() { Text = Meet.createString_gymnastStandings("Bars", MainPage._currentlySelectedDivision) });
            textContent.Blocks.Add(barsListingsParagraph);

            Paragraph paragraph_BeamHeader = new Paragraph();
            paragraph_BeamHeader.FontFamily = monoFontFamily;
            paragraph_BeamHeader.FontSize = 16;
            paragraph_BeamHeader.FontWeight = Windows.UI.Text.FontWeights.Bold;
            paragraph_BeamHeader.Margin = standardSpacing;
            paragraph_BeamHeader.Inlines.Add(new Run() { Text = "Event Standings: Beam" });
            textContent.Blocks.Add(paragraph_BeamHeader);

            var beamListingsParagraph = new Paragraph();
            beamListingsParagraph.FontFamily = monoFontFamily;
            beamListingsParagraph.Inlines.Add(new Run() { Text = Meet.createString_gymnastStandings("Beam", MainPage._currentlySelectedDivision) });
            textContent.Blocks.Add(beamListingsParagraph);

            Paragraph paragraph_FloorHeader = new Paragraph();
            paragraph_FloorHeader.FontFamily = monoFontFamily;
            paragraph_FloorHeader.FontSize = 16;
            paragraph_FloorHeader.FontWeight = Windows.UI.Text.FontWeights.Bold;
            paragraph_FloorHeader.Margin = standardSpacing;
            paragraph_FloorHeader.Inlines.Add(new Run() { Text = "Event Standings: Floor" });
            textContent.Blocks.Add(paragraph_FloorHeader);

            var floorListingsParagraph = new Paragraph();
            floorListingsParagraph.FontFamily = monoFontFamily;
            floorListingsParagraph.Inlines.Add(new Run() { Text = Meet.createString_gymnastStandings("Floor", MainPage._currentlySelectedDivision) });
            textContent.Blocks.Add(floorListingsParagraph);

            Paragraph paragraph_AllAroundHeader = new Paragraph();
            paragraph_AllAroundHeader.FontFamily = monoFontFamily;
            paragraph_AllAroundHeader.FontSize = 16;
            paragraph_AllAroundHeader.FontWeight = Windows.UI.Text.FontWeights.Bold;
            paragraph_AllAroundHeader.Margin = standardSpacing;
            paragraph_AllAroundHeader.Inlines.Add(new Run() { Text = "Individual All Around Standings" });
            textContent.Blocks.Add(paragraph_AllAroundHeader);

            var allAroundListingsParagraph = new Paragraph();
            allAroundListingsParagraph.FontFamily = monoFontFamily;
            allAroundListingsParagraph.Inlines.Add(new Run() { Text = Meet.createString_gymnastStandings("All Around", MainPage._currentlySelectedDivision) });
            textContent.Blocks.Add(allAroundListingsParagraph);
        }


        internal static void PopulateQualificationBlocks_OnlyText(Page page) {
            string pageVisualTreeString = HelperMethods.PlaceVisualTreeIntoOutputString(page, 0);

            DependencyObject dObj = HelperMethods.GetFirstVisualTreeElementWithName(page, "TextContent");
            if (dObj == null) { throw new Exception_CouldNotFindUIElement("TextContent", (new RichTextBlock()).GetType(), page.GetType(), pageVisualTreeString); }
            RichTextBlock textContent = dObj as RichTextBlock;
            if (textContent == null) { throw new Exception_UnexpectedDataTypeEncountered((new Paragraph()).GetType(), dObj.GetType()); }


            dObj = HelperMethods.GetFirstVisualTreeElementWithName(page, "paragraph_QualificationHeader");
            if (dObj == null) { throw new Exception_CouldNotFindUIElement("paragraph_QualificationHeader", (new Paragraph()).GetType(), page.GetType(), pageVisualTreeString); }
            Paragraph paragraph_QualificationHeader = dObj as Paragraph;
            if (paragraph_QualificationHeader == null) { throw new Exception_UnexpectedDataTypeEncountered((new Paragraph()).GetType(), dObj.GetType()); }

            var monoFontFamily = new Windows.UI.Xaml.Media.FontFamily("Courier New");

            paragraph_QualificationHeader.Inlines.Clear();
            paragraph_QualificationHeader.FontFamily = monoFontFamily;
            paragraph_QualificationHeader.Inlines.Add(new Run() {
                Text = Meet.qualificationParameters.meetQualfiedFor,
                FontSize = 24,
                FontWeight = Windows.UI.Text.FontWeights.ExtraBold,
            });

            string divisionString = "";
            if((MainPage._currentlySelectedDivision != null) 
                && (MainPage._currentlySelectedDivision.name != ProgramConstants.INCLUDE_ALL_DIVISIONS)
                && (MainPage._currentlySelectedDivision.name != ProgramConstants.DIVISIONS_NOT_IN_USE)) {
                divisionString = " - " + MainPage._currentlySelectedDivision.name + " division";
            }
            paragraph_QualificationHeader.Inlines.Add(new Run() {
                Text = Environment.NewLine + "Qualifying meet: " + Meet.meetParameters.meetName + divisionString,
                FontSize = 18,
                FontWeight = Windows.UI.Text.FontWeights.ExtraBold
            });
            paragraph_QualificationHeader.Inlines.Add(new Run() {
                Text = Environment.NewLine + "Date: "+ Meet.meetParameters.meetDate,
                FontSize = 18,
                FontWeight = Windows.UI.Text.FontWeights.Bold
            });
            paragraph_QualificationHeader.Inlines.Add(new Run() {
                Text = Environment.NewLine + "Location: " + Meet.meetParameters.meetLocation + Environment.NewLine,
                FontSize = 18,
                FontWeight = Windows.UI.Text.FontWeights.Bold
            });
            var blocks = Meet.DisplayQualifiers_Printing(MainPage._currentlySelectedDivision);
            foreach(Paragraph block in blocks) {
                textContent.Blocks.Add(block);
            }
        }

        #endregion

        #region Application Content Size Constants given in percents (normalized)

            /// <summary>
            /// The percent of app's margin width, content is set at 85% (0.85) of the area's width
            /// </summary>
        protected const double ApplicationContentMarginLeft = 0.075;

        /// <summary>
        /// The percent of app's margin height, content is set at 94% (0.94) of tha area's height
        /// </summary>
        protected const double ApplicationContentMarginTop = 0.03;

        #endregion

        /// <summary>
        /// PrintDocument is used to prepare the pages for printing.
        /// Prepare the pages to print in the handlers for the Paginate, GetPreviewPage, and AddPages events.
        /// </summary>
        protected PrintDocument printDocument;

        /// <summary>
        /// Marker interface for document source
        /// </summary>
        protected IPrintDocumentSource printDocumentSource;

        /// <summary>
        /// A list of UIElements used to store the print preview pages.  This gives easy access
        /// to any desired preview page.
        /// </summary>
        internal List<UIElement> printPreviewPages;

        // Event callback which is called after print preview pages are generated.  Photos scenario uses this to do filtering of preview pages
        protected event EventHandler PreviewPagesCreated;

        /// <summary>
        /// First page in the printing-content series
        /// From this "virtual sized" paged content is split(text is flowing) to "printing pages"
        /// </summary>
        protected FrameworkElement firstPage;

        /// <summary>
        ///  A reference back to the scenario page used to access XAML elements on the scenario page
        /// </summary>
        protected Page resultsPage;

        /// <summary>
        ///  A hidden canvas used to hold pages we wish to print
        /// </summary>
        protected Canvas PrintCanvas
        {
            get
            {
                return resultsPage.FindName("ResultsCanvas") as Canvas;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resultsPage">The results page constructing us</param>
        public PrintHelper(Page resultsPage) {
            this.resultsPage = resultsPage;
            printPreviewPages = new List<UIElement>();
        }

        /// <summary>
        /// This function registers the app for printing with Windows and sets up the necessary event handlers for the print process.
        /// </summary>
        public virtual void RegisterForPrinting() {
            printDocument = new PrintDocument();
            printDocumentSource = printDocument.DocumentSource;
            printDocument.Paginate += CreatePrintPreviewPages;
            printDocument.GetPreviewPage += GetPrintPreviewPage;
            printDocument.AddPages += AddPrintPages;

            PrintManager printMan = PrintManager.GetForCurrentView();
            printMan.PrintTaskRequested += PrintTaskRequested;
        }

        /// <summary>
        /// This function unregisters the app for printing with Windows.
        /// </summary>
        public virtual void UnregisterForPrinting() {
            if (printDocument == null) {
                return;
            }

            printDocument.Paginate -= CreatePrintPreviewPages;
            printDocument.GetPreviewPage -= GetPrintPreviewPage;
            printDocument.AddPages -= AddPrintPages;

            // Remove the handler for printing initialization.
            PrintManager printMan = PrintManager.GetForCurrentView();
            printMan.PrintTaskRequested -= PrintTaskRequested;

            PrintCanvas.Children.Clear();
        }

        public async Task ShowPrintUIAsync() {
            // Catch and print out any errors reported
            try {
                await PrintManager.ShowPrintUIAsync();
            }
            catch (Exception e) {
                MainPage.Current.NotifyUser("Error printing: " + e.Message + ", hr=" + e.HResult, NotifyType.ErrorMessage);
            }
        }

        /// <summary>
        /// Method that will generate print content for the scenario
        /// For scenarios 1-4: it will create the first page from which content will flow
        /// Scenario 5 uses a different approach
        /// </summary>
        /// <param name="page">The page to print</param>
        public virtual void PreparePrintContent(Page page) {
            if (firstPage == null) {
                firstPage = page;
                StackPanel header = (StackPanel)firstPage.FindName("Header");
                if(header != null) {
                    header.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
            }

            // Add the (newly created) page to the print canvas which is part of the visual tree and force it to go
            // through layout so that the linked containers correctly distribute the content inside them.
            PrintCanvas.Children.Add(firstPage);
            PrintCanvas.InvalidateMeasure();
            PrintCanvas.UpdateLayout();
        }

        /// <summary>
        /// This is the event handler for PrintManager.PrintTaskRequested.
        /// </summary>
        /// <param name="sender">PrintManager</param>
        /// <param name="e">PrintTaskRequestedEventArgs </param>
        protected virtual void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs e) {
            PrintTask printTask = null;
            printTask = e.Request.CreatePrintTask(ProgramConstants.NAME_OF_APP, sourceRequested => {
                // Print Task event handler is invoked when the print job is completed.
                printTask.Completed += async (s, args) => {
                    // Notify the user when the print operation fails.
                    if (args.Completion == PrintTaskCompletion.Failed) {
                        await resultsPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                            MainPage.Current.NotifyUser("Failed to print.", NotifyType.ErrorMessage);
                        });
                    }
                };

                sourceRequested.SetSource(printDocumentSource);
            });
        }

        /// <summary>
        /// This is the event handler for PrintDocument.Paginate. It creates print preview pages for the app.
        /// </summary>
        /// <param name="sender">PrintDocument</param>
        /// <param name="e">Paginate Event Arguments</param>
        protected virtual void CreatePrintPreviewPages(object sender, PaginateEventArgs e) {
            // Clear the cache of preview pages
            printPreviewPages.Clear();

            // Clear the print canvas of preview pages
            PrintCanvas.Children.Clear();

            // This variable keeps track of the last RichTextBlockOverflow element that was added to a page which will be printed
            RichTextBlockOverflow lastRTBOOnPage;

            // Get the PrintTaskOptions
            PrintTaskOptions printingOptions = ((PrintTaskOptions)e.PrintTaskOptions);

            // Get the page description to deterimine how big the page is
            PrintPageDescription pageDescription = printingOptions.GetPageDescription(0);

            // We know there is at least one page to be printed. passing null as the first parameter to
            // AddOnePrintPreviewPage tells the function to add the first page.
            lastRTBOOnPage = AddOnePrintPreviewPage(null, pageDescription);

            bool test = lastRTBOOnPage.HasOverflowContent;

            // We know there are more pages to be added as long as the last RichTextBoxOverflow added to a print preview
            // page has extra content
            while (lastRTBOOnPage.HasOverflowContent && lastRTBOOnPage.Visibility == Windows.UI.Xaml.Visibility.Visible) {
                lastRTBOOnPage = AddOnePrintPreviewPage(lastRTBOOnPage, pageDescription);
            }

            if (PreviewPagesCreated != null) {
                PreviewPagesCreated.Invoke(printPreviewPages, null);
            }

            PrintDocument printDoc = (PrintDocument)sender;

            // Report the number of preview pages created
            printDoc.SetPreviewPageCount(printPreviewPages.Count, PreviewPageCountType.Intermediate);
        }

        /// <summary>
        /// This is the event handler for PrintDocument.GetPrintPreviewPage. It provides a specific print preview page,
        /// in the form of an UIElement, to an instance of PrintDocument. PrintDocument subsequently converts the UIElement
        /// into a page that the Windows print system can deal with.
        /// </summary>
        /// <param name="sender">PrintDocument</param>
        /// <param name="e">Arguments containing the preview requested page</param>
        protected virtual void GetPrintPreviewPage(object sender, GetPreviewPageEventArgs e) {
            PrintDocument printDoc = (PrintDocument)sender;
            printDoc.SetPreviewPage(e.PageNumber, printPreviewPages[e.PageNumber - 1]);
        }

        /// <summary>
        /// This is the event handler for PrintDocument.AddPages. It provides all pages to be printed, in the form of
        /// UIElements, to an instance of PrintDocument. PrintDocument subsequently converts the UIElements
        /// into a pages that the Windows print system can deal with.
        /// </summary>
        /// <param name="sender">PrintDocument</param>
        /// <param name="e">Add page event arguments containing a print task options reference</param>
        protected virtual void AddPrintPages(object sender, AddPagesEventArgs e) {
            // Loop over all of the preview pages and add each one to  add each page to be printied
            for (int i = 0; i < printPreviewPages.Count; i++) {
                // We should have all pages ready at this point...
                printDocument.AddPage(printPreviewPages[i]);
            }

            PrintDocument printDoc = (PrintDocument)sender;

            // Indicate that all of the print pages have been provided
            printDoc.AddPagesComplete();
        }

        /// <summary>
        /// This function creates and adds one print preview page to the internal cache of print preview
        /// pages stored in printPreviewPages.
        /// </summary>
        /// <param name="lastRTBOAdded">Last RichTextBlockOverflow element added in the current content</param>
        /// <param name="printPageDescription">Printer's page description</param>
        protected virtual RichTextBlockOverflow AddOnePrintPreviewPage(RichTextBlockOverflow lastRTBOAdded, PrintPageDescription printPageDescription) {
            // XAML element that is used to represent to "printing page"
            FrameworkElement page;

            // The link container for text overflowing in this page
            RichTextBlockOverflow textLink;

            // Check if this is the first page ( no previous RichTextBlockOverflow)
            if (lastRTBOAdded == null) {
                // If this is the first page add the specific scenario content
                page = firstPage;
                //Hide footer since we don't know yet if it will be displayed (this might not be the last page) - wait for layout
                StackPanel footer = (StackPanel)page.FindName("Footer");
                if(footer != null) {
                    footer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
            }
            else {

                // Flow content (text) from previous pages
                page = new ContinuationPage(lastRTBOAdded);
            }

            // Set "paper" width
            page.Width = printPageDescription.PageSize.Width;
            page.Height = printPageDescription.PageSize.Height;

            string visualTreeString = HelperMethods.PlaceVisualTreeIntoOutputString(page, 0);
            
            Grid printableArea = null;
            DependencyObject dObj = HelperMethods.GetFirstVisualTreeElementWithName(page, "PrintableArea");
            if(dObj==null) {
                throw new Exception_CouldNotFindUIElement("PrintableArea", (new Grid()).GetType(), page.GetType(), visualTreeString);
            }
            if(dObj is Grid) {
                printableArea = dObj as Grid;
            }
            else {
                throw new Exception_UnexpectedDataTypeEncountered((new Grid()).GetType(), dObj.GetType());
            }
            

            // Get the margins size
            // If the ImageableRect is smaller than the app provided margins use the ImageableRect
            double marginWidth = Math.Max(printPageDescription.PageSize.Width - printPageDescription.ImageableRect.Width, printPageDescription.PageSize.Width * ApplicationContentMarginLeft * 2);
            double marginHeight = Math.Max(printPageDescription.PageSize.Height - printPageDescription.ImageableRect.Height, printPageDescription.PageSize.Height * ApplicationContentMarginTop * 2);

            // Set-up "printable area" on the "paper"
            printableArea.Width = firstPage.Width - marginWidth;
            printableArea.Height = firstPage.Height - marginHeight;

            // Add the (newley created) page to the print canvas which is part of the visual tree and force it to go
            // through layout so that the linked containers correctly distribute the content inside them.
            PrintCanvas.Children.Add(page);
            PrintCanvas.InvalidateMeasure();
            PrintCanvas.UpdateLayout();

            // Find the last text container and see if the content is overflowing
            textLink = (RichTextBlockOverflow)page.FindName("ContinuationPageLinkedContainer");

            // Check if this is the last page
            if (!textLink.HasOverflowContent && textLink.Visibility == Windows.UI.Xaml.Visibility.Visible) {
                StackPanel footer = (StackPanel)page.FindName("Footer");
                if(footer != null) {
                    footer.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
            }

            // Add the page to the page preview collection
            printPreviewPages.Add(page);

            return textLink;
        }
    }
}
