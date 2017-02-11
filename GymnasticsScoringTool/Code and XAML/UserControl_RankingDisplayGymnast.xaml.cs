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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace GymnasticsScoringTool_01 {
    public sealed partial class UserControl_RankingDisplayGymnast : UserControl {

        public UserControl_RankingDisplayGymnast() {
            this.InitializeComponent();
        }

        public void FormAppropriateBlock(string eventString, Division division, int startingRank, int blockSize, bool includeHeader = true, bool adjustFormatForPrinting = false) {
            if(!includeHeader) {
                this.RankingGrid.Children.Clear();
                this.RankingGrid.RowDefinitions.Clear();
            }

            try { Meet.AddGymnastsTo_UserControl_RankingDisplayGymnasts(eventString, this, division, startingRank, blockSize); }
            catch (Exception e) {
                TextBlock tBkException = new TextBlock();
                tBkException.Text = e.Message;
                MainPage._publicStaticDebugSpace.Children.Add(tBkException);
            }

            if(adjustFormatForPrinting) {
                // RankingGrid.MinWidth = 600;
                foreach(UIElement child in RankingGrid.Children) {
                    if(child is TextBlock) {
                        (child as TextBlock).FontSize = 16;
                    }
                    else if(child is Button) {
                        (child as Button).FontSize = 16;
                    }
                }

                this.Margin = new Thickness(0, 0, 0, 20);
            }
            
            // logic to flip between white and gray blocks
            if((((startingRank - 1) / blockSize) % 2) != 0) {
                Windows.UI.Color veryLightGray = Windows.UI.Color.FromArgb(0xFF, 0xE0, 0xE0, 0xE0);
                Windows.UI.Color mediumGray = Windows.UI.Color.FromArgb(0xFF, 0xB0, 0xB0, 0xB0);
                RankingGrid.Background = new SolidColorBrush(veryLightGray);
                RankingGrid.BorderBrush = new SolidColorBrush(mediumGray);
                RankingGrid.BorderThickness = new Thickness(0, 1, 0, 1);
            }
        }


        public static UserControl_RankingDisplayGymnast RankingDisplayOfGymnasts_ForUI(string eventString, Division division = null, int blockSize = 5) {

            var ucrdg = new UserControl_RankingDisplayGymnast();
            var dObj = HelperMethods.GetFirstVisualTreeElementWithName(ucrdg, "RankingGrid");
            if (dObj == null) {
                throw new Exception_CouldNotFindUIElement("RankingGrid", (new Grid()).GetType(), ucrdg.GetType());
            }
            Grid rankingGrid = dObj as Grid;
            if (rankingGrid == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new Grid()).GetType(), dObj.GetType());
            }

            int gymnastsInDivision = Meet.CountOfGymnastsToRankByDivision(eventString, division);

            int idx = 0;
            while(idx < gymnastsInDivision) {
                rankingGrid.RowDefinitions.Add(new RowDefinition());
                ++idx;
            }

            idx = 0;
            while(idx < gymnastsInDivision) {
                if(((idx / blockSize) % 2) != 0) {
                    Border bkgd = new Border();
                    Windows.UI.Color veryLightGray = Windows.UI.Color.FromArgb(0xFF, 0xE0, 0xE0, 0xE0);
                    Windows.UI.Color mediumGray = Windows.UI.Color.FromArgb(0xFF, 0xB0, 0xB0, 0xB0);
                    bkgd.Background = new SolidColorBrush(veryLightGray);
                    bkgd.BorderBrush = new SolidColorBrush(mediumGray);
                    bkgd.BorderThickness = new Thickness(0, 1, 0, 1);
                    Grid.SetRow(bkgd, idx + 1);
                    Grid.SetRowSpan(bkgd, blockSize);
                    Grid.SetColumnSpan(bkgd, rankingGrid.ColumnDefinitions.Count);
                    rankingGrid.Children.Add(bkgd);
                }
                idx += blockSize;
            }

            idx = 0;
            while(idx < gymnastsInDivision) {
                Meet.AddGymnastsTo_UserControl_RankingDisplayGymnasts(eventString, ucrdg, division, idx + 1, 1, false);
                ++idx;
            }

            foreach(var child in rankingGrid.Children) {
                switch(child.GetType().Name.ToUpper()) {
                case "TEXTBLOCK":
                    (child as TextBlock).Margin = new Thickness(0, 0.5, 0, 2);
                    break;
                case "BUTTON":
                    (child as Button).Margin = new Thickness(0, 0.5, 0, 2);
                    break;
                default:
                    break;
                }
            }

            return ucrdg;
        }

        public static List<UserControl_RankingDisplayGymnast> RankingDisplayOfGymnasts_ForPrinting(string eventString, Division division = null, int blockSize = 5) {
            var rankingDisplayListOfGrids = new List<UserControl_RankingDisplayGymnast>();

            int gymnastsInDivision = Meet.CountOfGymnastsToRankByDivision(eventString, division);

            int startingRank = 1;
            while (startingRank <= gymnastsInDivision) {
                var ucrdg = new UserControl_RankingDisplayGymnast();
                ucrdg.FormAppropriateBlock(eventString, division, startingRank, blockSize, (startingRank == 1), false);
                ucrdg.MaxWidth = ProgramConstants.STANDARD_OUTPUT_WIDTH;
                ucrdg.MinWidth = ProgramConstants.STANDARD_OUTPUT_WIDTH;

                var dObj = HelperMethods.GetFirstVisualTreeElementWithName(ucrdg, "RankingGrid");
                if(dObj == null) {
                    throw new Exception_CouldNotFindUIElement("RankingGrid", (new Grid()).GetType(), ucrdg.GetType());
                }
                Grid rankingGrid = dObj as Grid;
                if(rankingGrid == null) {
                    throw new Exception_UnexpectedDataTypeEncountered((new Grid()).GetType(), dObj.GetType());
                }

                int rowHeight = ProgramConstants.STANDARD_OUTPUT_ROW_HEIGHT;
                foreach(var row in rankingGrid.RowDefinitions) {
                    row.MinHeight = rowHeight;
                    row.MaxHeight = rowHeight;
                }

                rankingDisplayListOfGrids.Add(ucrdg);
                startingRank += blockSize;
            }

            return rankingDisplayListOfGrids;
        }

    }
}
