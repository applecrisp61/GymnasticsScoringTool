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
using System.Text;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI;


// ****************************************************************************************************************************
// PRESENTATION ITEMS
// ****************************************************************************************************************************

// TO DO: Think about if I am violating separation of logic and ui through this approach
// DECISION (as of 11/17/2016): Gonna leave this as it is... reasoning:
// - Complexity of program relatively low in current state
// - In terms of MVVM, can think of the Meet class as the "view model"... it knows about both
//      the data model (as represented by the gymnast, team, division, and meet parameters)
//      and the view (as embodied by the UI). 
// - Note that Meet is represented as a partial class... 
//      - the other file (Meet.cs) contains the elements that primarily interact with the data model
//      - this file (Meet_UIandOutput.cs) contains elements that primarily interact with the UI

namespace GymnasticsScoringTool_01 {
    public partial class Meet {

        /* Start UPDATE VALIDATION section */

        // Used during the Edit Team for Event process to present back to the user a list of the updates for verification
        // that are in the exact same order as they were entered (helpful, because they are often entered as a list from the judges,
        // which likely has no relation to how the gymnasts would otherwise be ordered in the UI)
        private static List<Tuple<int, EventScore>> _eventEditingUpdates = new List<Tuple<int, EventScore>>();

        public static bool ScoresToValidate
        {
            get { return _eventEditingUpdates.Count > 0; }
        }

        public static void ClearEventEditingUpdates() { _eventEditingUpdates.Clear(); }

        public static void RegisterEventUpdate(int competitorNbr, EventScore es) {
            if(EventUpdate_ContainsGymnast(competitorNbr)) {
                ChangeEventUpdate(competitorNbr, es);
            }
            else {
                AddEventUpdate(competitorNbr, es);
            }
        }
        
        private static bool EventUpdate_ContainsGymnast(int competitorNbr) {
            return (from u in _eventEditingUpdates
                    where u.Item1 == competitorNbr
                    select u).Count() > 0;
        }

        public static void ChangeEventUpdate(int competitorNbr, EventScore es) {
            bool found = false;
            int idx = 0;

            foreach (var u in _eventEditingUpdates) {
                if (u.Item1 == competitorNbr) {
                    _eventEditingUpdates.RemoveAt(idx);
                    found = true;
                    break;
                }
                ++idx;
            }

            if(found) {
                _eventEditingUpdates.Insert(idx, new Tuple<int, EventScore>(competitorNbr, es));
            }
        }

        public static void AddEventUpdate(int competitorNbr, EventScore es) {
            _eventEditingUpdates.Add(new Tuple<int, EventScore>(competitorNbr, es));
        }



        public static void AddUpdatesTo_UserControl_EditGymnastForEvent(UserControl_EditGymnastForEvent uceg) {
            foreach (var update in _eventEditingUpdates) {
                Gymnast g = GetGymnast(update.Item1);
                uceg.GymnastToValidate(g);
            }
        }

        /* End UPDATE VALIDATION section */

        public static void AddTeamTo_UserControl_EditGymnast(Team t, UserControl_EditGymnast uceg) {
            var roster = from gymnast in _gymnasts
                         where gymnast.team.name == t.name
                         orderby gymnast.competitorNumber
                         select gymnast;

            foreach (Gymnast g in roster) {
                uceg.GymnastToEdit(g);
            }
        }

        public static void AddTeamTo_UserControl_EditGymnastForEvent(Team t, UserControl_EditGymnastForEvent uceg) {
            var roster = from gymnast in _gymnasts
                         where gymnast.team.name == t.name
                         orderby gymnast.competitorNumber
                         select gymnast;

            foreach (Gymnast g in roster) {
                uceg.GymnastToEdit(g);
            }
        }

        public static void AddGymnastTo_ContentDialogEditGymnast(int competitorNbr, ContentDialog_EditGymnast cdeg) {
            Gymnast g = GetGymnast(competitorNbr);
            cdeg.GymnastToEdit(g);
        }


        public static void AddGymnastsTo_UserControl_RankingDisplayGymnasts(string eventString, 
            UserControl_RankingDisplayGymnast ucrdg, Division division, int startingRank, int blockSize, bool needToAddRows = true) {

            var rankedGymnasts = Meet.OrderGymnastsForThisRanking(eventString, division);

            Grid rankingGrid;
            DependencyObject dObj = HelperMethods.GetFirstVisualTreeElementWithName(ucrdg, "RankingGrid");
            if (dObj == null) {
                throw new Exception_CouldNotFindUIElement("Ranking Grid", (new GridLength()).GetType(), ucrdg.GetType());
            }
            else { rankingGrid = dObj as Grid; }
            if (rankingGrid == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new GridLength()).GetType(), dObj.GetType());
            }

            int gymnastIdx = 1;
            int rank = 1;
            double prevGymnastScore = _meetParameters.maxScore + 1;

            foreach (Gymnast g in rankedGymnasts) {
                if (gymnastIdx >= (startingRank + blockSize)) {
                    break;
                }

                double? relevantScore = null;

                switch (eventString.ToUpper()) {
                case "VAULT":
                    relevantScore = g.vaultScore.score;
                    break;
                case "BARS":
                    relevantScore = g.barsScore.score;
                    break;
                case "BEAM":
                    relevantScore = g.beamScore.score;
                    break;
                case "FLOOR":
                    relevantScore = g.floorScore.score;
                    break;
                case "OVERALL":
                case "ALL-AROUND":
                case "ALL AROUND":
                case "INDIVIDUAL ALL AROUND":
                case "INDIVIDUAL ALL-AROUND":
                case "INDIVIDUAL OVERALL":
                    relevantScore = g.overallScore;
                    break;
                default: throw new Exception_UnkownEvent(eventString.ToUpper());
                }

                if (!relevantScore.HasValue) { break; }
                if(eventString.ToUpper().Contains("OVERALL") || eventString.ToUpper().Contains("INDIVIDUAL") || eventString.ToUpper().Contains("AROUND")) {
                    if(!g.IncludeInAllAroundResults) { continue; }
                }

                int rowIdx = needToAddRows ? rankingGrid.RowDefinitions.Count : gymnastIdx;
                if ((gymnastIdx >= startingRank) && (needToAddRows)) {
                    rankingGrid.RowDefinitions.Add(new RowDefinition());
                }

                TextBlock tBkRank = new TextBlock();
                TextBlock tBkTie = new TextBlock();
                TextBlock tBkScore = new TextBlock();
                Button btnNbr = new Button();
                Button btnName = new Button();
                TextBlock tBkTeam = new TextBlock();

                if (relevantScore != prevGymnastScore) {
                    rank = gymnastIdx;
                }

                double? nextGymnastScore = null; ;
                if (gymnastIdx < rankedGymnasts.Count()) {
                    Gymnast nextGymnast = rankedGymnasts.ElementAt(gymnastIdx);
                    switch (eventString.ToUpper()) {
                    case "VAULT":
                        nextGymnastScore = nextGymnast.vaultScore.score;
                        break;
                    case "BARS":
                        nextGymnastScore = nextGymnast.barsScore.score;
                        break;
                    case "BEAM":
                        nextGymnastScore = nextGymnast.beamScore.score;
                        break;
                    case "FLOOR":
                        nextGymnastScore = nextGymnast.floorScore.score;
                        break;
                    case "OVERALL":
                    case "ALL-AROUND":
                    case "ALL AROUND":
                    case "INDIVIDUAL ALL AROUND":
                    case "INDIVIDUAL ALL-AROUND":
                    case "INDIVIDUAL OVERALL":
                        nextGymnastScore = nextGymnast.overallScore;
                        break;
                    default: throw new Exception_UnkownEvent(eventString.ToUpper());
                    }
                }

                if ((relevantScore == prevGymnastScore) || (relevantScore == nextGymnastScore)) {
                    tBkTie.Text = "T";
                }

                if (gymnastIdx >= startingRank) {

                    tBkRank.Text = rank.ToString();
                    tBkScore.Text = HelperMethods.FormatScore(relevantScore);
                    btnNbr.Content = g.competitorNumberDisplay;
                    btnName.Content = g.firstName + " " + g.lastName;
                    tBkTeam.Text = g.team.name;

                    Grid.SetColumn(tBkRank, 0); Grid.SetRow(tBkRank, rowIdx);
                    Grid.SetColumn(tBkTie, 1); Grid.SetRow(tBkTie, rowIdx);
                    Grid.SetColumn(tBkScore, 2); Grid.SetRow(tBkScore, rowIdx);
                    Grid.SetColumn(btnNbr, 3); Grid.SetRow(btnNbr, rowIdx);
                    Grid.SetColumn(btnName, 4); Grid.SetRow(btnName, rowIdx);
                    Grid.SetColumn(tBkTeam, 5); Grid.SetRow(tBkTeam, rowIdx);

                    #region StandardFormatting.
                    tBkRank.HorizontalAlignment = HorizontalAlignment.Right;
                    tBkTie.HorizontalAlignment = HorizontalAlignment.Left;
                    tBkScore.HorizontalAlignment = HorizontalAlignment.Left;
                    btnNbr.HorizontalAlignment = HorizontalAlignment.Left;
                    btnName.HorizontalAlignment = HorizontalAlignment.Left;
                    tBkTeam.HorizontalAlignment = HorizontalAlignment.Left;

                    btnNbr.HorizontalContentAlignment = HorizontalAlignment.Left;
                    btnName.HorizontalContentAlignment = HorizontalAlignment.Left;

                    tBkRank.FontSize = 12;
                    tBkTie.FontSize = 12;
                    tBkScore.FontSize = 12;
                    btnNbr.FontSize = 12;
                    btnName.FontSize = 12;
                    tBkTeam.FontSize = 12;

                    btnNbr.PointerEntered += Button_PointerEntered;
                    btnName.PointerEntered += Button_PointerEntered;
                    btnNbr.PointerExited += Button_PointerExited;
                    btnName.PointerExited += Button_PointerExited;

                    #endregion StandardFormatting.

                    rankingGrid.Children.Add(tBkRank);
                    rankingGrid.Children.Add(tBkTie);
                    rankingGrid.Children.Add(tBkScore);
                    rankingGrid.Children.Add(btnNbr);
                    rankingGrid.Children.Add(btnName);
                    rankingGrid.Children.Add(tBkTeam);
                }

                prevGymnastScore = relevantScore.Value; // execution will break when it hits the first null for relevant score
                ++gymnastIdx;
            }

        }

        private static IOrderedEnumerable<Gymnast> OrderGymnastsForThisRanking(string eventString, Division d = null) {
            IOrderedEnumerable<Gymnast> gymnastsOrdered;

            switch (eventString.ToUpper()) {
            case "VAULT":
                gymnastsOrdered = from g in _gymnasts
                                  where (d == null || d.name == g.division.name || d.name == ProgramConstants.INCLUDE_ALL_DIVISIONS)
                                  orderby g.IComparableVaultScore descending
                                  select g;
                break;
            case "BARS":
                gymnastsOrdered = from g in _gymnasts
                                  where (d == null || d.name == g.division.name || d.name == ProgramConstants.INCLUDE_ALL_DIVISIONS)
                                  orderby g.IComparableBarsScore descending
                                  select g;
                break;
            case "BEAM":
                gymnastsOrdered = from g in _gymnasts
                                  where (d == null || d.name == g.division.name || d.name == ProgramConstants.INCLUDE_ALL_DIVISIONS)
                                  orderby g.IComparableBeamScore descending
                                  select g;
                break;
            case "FLOOR":
                gymnastsOrdered = from g in _gymnasts
                                  where (d == null || d.name == g.division.name || d.name == ProgramConstants.INCLUDE_ALL_DIVISIONS)
                                  orderby g.IComparableFloorScore descending
                                  select g;
                break;
            case "OVERALL":
            case "ALL-AROUND":
            case "ALL AROUND":
            case "INDIVIDUAL ALL AROUND":
            case "INDIVIDUAL ALL-AROUND":
            case "INDIVIDUAL OVERALL":
                gymnastsOrdered = from g in _gymnasts
                                  where (d == null || d.name == g.division.name || d.name == ProgramConstants.INCLUDE_ALL_DIVISIONS)
                                  where g.IncludeInAllAroundResults
                                  orderby g.IComparableOverallScore descending
                                  select g;
                break;
            default:
                throw new Exception_UnkownEvent(eventString.ToUpper());
            }

            return gymnastsOrdered;
        }

        private static IOrderedEnumerable<Gymnast> OrderGymnastsForThisRanking(string eventString, List<Gymnast> gymnastsForThisRanking) {
            IOrderedEnumerable<Gymnast> gymnastsOrdered;

            switch (eventString.ToUpper()) {
            case "VAULT":
                gymnastsOrdered = from g in gymnastsForThisRanking
                                  orderby g.IComparableVaultScore descending
                                  select g;
                break;
            case "BARS":
                gymnastsOrdered = from g in gymnastsForThisRanking
                                  orderby g.IComparableBarsScore descending
                                  select g;
                break;
            case "BEAM":
                gymnastsOrdered = from g in gymnastsForThisRanking
                                  orderby g.IComparableBeamScore descending
                                  select g;
                break;
            case "FLOOR":
                gymnastsOrdered = from g in gymnastsForThisRanking
                                  orderby g.IComparableFloorScore descending
                                  select g;
                break;
            case "OVERALL":
            case "ALL-AROUND":
            case "ALL AROUND":
            case "INDIVIDUAL ALL AROUND":
            case "INDIVIDUAL ALL-AROUND":
            case "INDIVIDUAL OVERALL":
                gymnastsOrdered = from g in gymnastsForThisRanking
                                  where g.IncludeInAllAroundResults
                                  orderby g.IComparableOverallScore descending
                                  select g;
                break;
            default:
                throw new Exception_UnkownEvent(eventString.ToUpper());
            }

            return gymnastsOrdered;
        }

        private static void Button_PointerEntered(object sender, PointerRoutedEventArgs args) {
            if (!(sender is Button)) {
                throw new Exception_UnexpectedDataTypeEncountered((new Button()).GetType(), sender.GetType());
            }

            Button button = sender as Button;
            Grid parentGrid = button.Parent as Grid;
            if (parentGrid == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new Grid()).GetType(), (button.Parent).GetType());
            }

            // NOTE: Cannot just set the foreground state directly for the button in this case - it would
            // be superceded by the VisualStateManagers PointerOver state. Must change this state.
            // --> Everything would work except setting the buttons the Foreground (text color) to Maroon,
            // as this property is set to to black by the PointerOver visual state.

            // The management of the visual states is performed within the code that sets up the buttons.

            #region VisualStateManagement.
            DependencyObject dObj = HelperMethods.GetFirstVisualTreeElementWithName(button, "RootGrid");
            Grid rootGrid = dObj as Grid;

            if (rootGrid != null) {

                var commonStates = (from vsg in VisualStateManager.GetVisualStateGroups(rootGrid)
                                    where vsg.Name.ToUpper() == "COMMONSTATES"
                                    select vsg).Single(); // should only be one

                var pointerOver = (from vs in commonStates.States
                                   where vs.Name.ToUpper() == "POINTEROVER"
                                   select vs).Single(); // should only be one

                var desiredTimeline = (from child in pointerOver.Storyboard.Children
                                       where Storyboard.GetTargetName(child).ToUpper() == (new ContentPresenter()).GetType().Name.ToUpper()
                                       where Storyboard.GetTargetProperty(child).ToUpper() == "FOREGROUND"
                                       select child).Single(); // should only be one

                var frames = (desiredTimeline as ObjectAnimationUsingKeyFrames).KeyFrames;
                var theFrame = frames.ElementAt(0); // based on generic.XAML, should only be one
                theFrame.Value = new SolidColorBrush(Colors.Maroon);

            }
            else {
                throw new Exception_CouldNotFindUIElement("RootGrid", (new Grid()).GetType(), (new Button()).GetType());
            }
            #endregion VisualStateManagement.

            int buttonRow = Grid.GetRow(button);

            foreach (UIElement uie in parentGrid.Children) {
                if ((uie is FrameworkElement) && (Grid.GetRow(uie as FrameworkElement) == buttonRow)) {
                    if (uie is TextBlock) {
                        TextBlock t = uie as TextBlock;
                        // t.FontWeight = Windows.UI.Text.FontWeights.Bold;
                    }

                    else if (uie is Button) {
                        Button b = uie as Button;
                        b.FontWeight = Windows.UI.Text.FontWeights.Bold;
                        b.Foreground = new SolidColorBrush(Colors.Maroon);
                    }
                }
            }

        }

        private static void Button_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e) {
            if (!(sender is Button)) {
                throw new Exception_UnexpectedDataTypeEncountered((new Button()).GetType(), sender.GetType());
            }

            Button button = sender as Button;
            Grid parentGrid = button.Parent as Grid;
            if (parentGrid == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new Grid()).GetType(), (button.Parent).GetType());
            }

            int buttonRow = Grid.GetRow(button);

            foreach (UIElement uie in parentGrid.Children) {
                if ((uie is FrameworkElement) && (Grid.GetRow(uie as FrameworkElement) == buttonRow)) {
                    if (uie is TextBlock) {
                        TextBlock t = uie as TextBlock;
                        // t.FontWeight = Windows.UI.Text.FontWeights.Normal;
                    }

                    else if (uie is Button) {
                        Button b = uie as Button;
                        b.FontWeight = Windows.UI.Text.FontWeights.Normal;
                        b.Foreground = new SolidColorBrush(Colors.Black);
                    }
                }
            }
        }


        public static string TeamRankString(int teamNbr, string eventString = "OVERALL", Division d = null) {
            var teamsOrdered = OrderTeamsForThisRanking(eventString, d);

            double? prevScore = _meetParameters.maxScore * _meetParameters.scoresForCompetition * 4 + 1;
            double noMoreScores = -1;
            double? nextScore = noMoreScores;
            int prevRank = 0;
            int idx = 1;
            foreach (Team t in teamsOrdered) {
                if (idx < teamsOrdered.Count()) { nextScore = teamsOrdered.ElementAt(idx).teamScore(eventString, d); }
                else { nextScore = noMoreScores; }

                if (t.number == teamNbr) {
                    return ((t.teamScore(eventString, d) == prevScore) ? prevRank.ToString() : idx.ToString())
                        + (((t.teamScore(eventString, d) == prevScore) || (t.teamScore(eventString, d) == nextScore)) ? "T" : " ");
                }
                else {
                    if (t.teamScore(eventString, d) != prevScore) {
                        prevRank = idx;
                    }
                    prevScore = t.teamScore(eventString, d);
                    ++idx;
                }
            }

            return "X";
        }

        public static double GapToHighestTeam(int teamNbr, string eventString = "OVERALL", Division d = null) {
            var teamsOrdered = OrderTeamsForThisRanking(eventString, d);
            if (teamsOrdered.Count() == 0) { return -1; }

            int topTeamNbr = teamsOrdered.ElementAt(0).number;

            double? teamScore;
            double? topScore;

            switch (eventString.ToUpper()) {
            case "VAULT":
                teamScore = TeamScoreVault(teamNbr, d);
                topScore = TeamScoreVault(topTeamNbr, d);
                break;
            case "BARS":
                teamScore = TeamScoreBars(teamNbr, d);
                topScore = TeamScoreBars(topTeamNbr, d);
                break;
            case "BEAM":
                teamScore = TeamScoreBeam(teamNbr, d);
                topScore = TeamScoreBeam(topTeamNbr, d);
                break;
            case "FLOOR":
                teamScore = TeamScoreFloor(teamNbr, d);
                topScore = TeamScoreFloor(topTeamNbr, d);
                break;
            case "OVERALL":
            case "ALL-AROUND":
            case "ALL AROUND":
            case "INDIVIDUAL ALL AROUND":
            case "INDIVIDUAL ALL-AROUND":
            case "INDIVIDUAL OVERALL":
            default:
                teamScore = TeamScoreOverall(teamNbr, d);
                topScore = TeamScoreOverall(topTeamNbr, d);
                break;
            }

            return (topScore.HasValue ? topScore.Value : 0) - (teamScore.HasValue ? teamScore.Value : 0);
        }

        private static IOrderedEnumerable<Team> OrderTeamsForThisRanking(string eventString, Division d = null) {
            IOrderedEnumerable<Team> teamsOrdered;

            switch (eventString.ToUpper()) {
            case "VAULT":
                teamsOrdered = from t in _teams
                               where TeamScoreVault(t.number, d) > 0
                               orderby TeamScoreVault(t.number, d) descending
                               select t;
                break;
            case "BARS":
                teamsOrdered = from t in _teams
                               where TeamScoreBars(t.number, d) > 0
                               orderby TeamScoreBars(t.number, d) descending
                               select t;
                break;
            case "BEAM":
                teamsOrdered = from t in _teams
                               where TeamScoreBeam(t.number, d) > 0
                               orderby TeamScoreBeam(t.number, d) descending
                               select t;
                break;
            case "FLOOR":
                teamsOrdered = from t in _teams
                               where TeamScoreFloor(t.number, d) > 0
                               orderby TeamScoreFloor(t.number, d) descending
                               select t;
                break;
            case "OVERALL":
            case "ALL-AROUND":
            case "ALL AROUND":
            case "INDIVIDUAL ALL AROUND":
            case "INDIVIDUAL ALL-AROUND":
            case "INDIVIDUAL OVERALL":
                teamsOrdered = from t in _teams
                               where TeamScoreOverall(t.number, d) > 0
                               orderby TeamScoreOverall(t.number, d) descending
                               select t;
                break;
            default:
                teamsOrdered = from t in _teams
                               where TeamScoreOverall(t.number, d) > 0
                               orderby TeamScoreOverall(t.number, d) descending
                               select t;
                break;
            }

            return teamsOrdered;
        }

        // Used for display within the main program (printed output too complex for space available,
        // so use this simpler output)
        public static string createString_teamStandings(Division div = null) {
            StringBuilder sb = new StringBuilder();
            sb.Append(Environment.NewLine);

            var rankedTeams = OrderTeamsForThisRanking("Overall", div);

            foreach (Team t in rankedTeams) {
                sb.AppendLine("#" + TeamRankString(t.number, "Overall", div) + " " + HelperMethods.FormatScore(Meet.TeamScoreOverall(t.number, div)) + " > " + t.name);
                sb.AppendLine("    Vault: " + HelperMethods.FormatScore(Meet.TeamScoreVault(t.number, div)));
                sb.AppendLine("    Bars : " + HelperMethods.FormatScore(Meet.TeamScoreBars(t.number, div)));
                sb.AppendLine("    Beam : " + HelperMethods.FormatScore(Meet.TeamScoreBeam(t.number, div)));
                sb.AppendLine("    Floor: " + HelperMethods.FormatScore(Meet.TeamScoreFloor(t.number, div)));
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public static List<List<Inline>> create_TeamStandings_InlineCollection(Division div = null) {
            var output = new List<List<Inline>>();

            var rankedTeams = OrderTeamsForThisRanking("Overall", div);

            int labelColWidth = 30;
            int vaultColWidth = 10;
            int barsColWidth = 10;
            int beamColWidth = 10;
            int floorColWidth = 10;
            // int totalColWidth = 10;
            int stop1 = labelColWidth;
            int stop2 = stop1 + vaultColWidth;
            int stop3 = stop2 + barsColWidth;
            int stop4 = stop3 + beamColWidth;
            int stop5 = stop4 + floorColWidth;
            var stopPoints = new Tuple<int, int, int, int, int>(stop1, stop2, stop3, stop4, stop5);
            var contentHolder = new Tuple<string, string, string, string, string, string>("", "", "", "", "", "");

            int teamIdx = 1;
            double prevTeamScore = _meetParameters.maxScore * _meetParameters.scoresForCompetition * 4 + 1;
            double nextTeamScore = _meetParameters.minScore * _meetParameters.scoresForCompetition * 4 - 1;

            foreach (Team t in rankedTeams) {
                var teamList = new List<Inline>();
                teamList.Add(new Run() { Text = "#" + TeamRankString(t.number, "Overall", div) +" " + 
                    HelperMethods.FormatScore(Meet.TeamScoreOverall(t.number, div)) + " > " + t.name });

                contentHolder = new Tuple<string, string, string, string, string, string>("", "Vault", "Bars", "Beam", "Floor", "Total");
                teamList.Add(new Run() { Text = ArrangeLine_Normal(stopPoints, contentHolder) });

                contentHolder = new Tuple<string, string, string, string, string, string>("Score by event:", 
                    HelperMethods.FormatScore(TeamScoreVault(t.number, div)),
                    HelperMethods.FormatScore(TeamScoreBars(t.number, div)),
                    HelperMethods.FormatScore(TeamScoreBeam(t.number, div)),
                    HelperMethods.FormatScore(TeamScoreFloor(t.number, div)),
                    HelperMethods.FormatScore(TeamScoreOverall(t.number, div)));
                teamList.Add(new Run() { Text = ArrangeLine_Normal(stopPoints, contentHolder) });

                contentHolder = new Tuple<string, string, string, string, string, string>("Team rank by event:",
                    TeamRankString(t.number, "Vault", div),
                    TeamRankString(t.number, "Bars", div),
                    TeamRankString(t.number, "Beam", div),
                    TeamRankString(t.number, "Floor", div),
                    TeamRankString(t.number, "Overall", div));
                teamList.Add(new Run() { Text = ArrangeLine_Normal(stopPoints, contentHolder) });

                contentHolder = new Tuple<string, string, string, string, string, string>("Gap to highest team:",
                    HelperMethods.FormatScore(GapToHighestTeam(t.number, "Vault", div)),
                    HelperMethods.FormatScore(GapToHighestTeam(t.number, "Bars", div)),
                    HelperMethods.FormatScore(GapToHighestTeam(t.number, "Beam", div)),
                    HelperMethods.FormatScore(GapToHighestTeam(t.number, "Floor", div)),
                    HelperMethods.FormatScore(GapToHighestTeam(t.number, "Overall", div)));
                teamList.Add(new Run() { Text = ArrangeLine_Normal(stopPoints, contentHolder) });

                contentHolder = new Tuple<string, string, string, string, string, string>("", "", "", "", "", "");
                teamList.Add(new Run() { Text = ArrangeLine_Normal(stopPoints, contentHolder) });

                var gymnastsOnTeam = from g in _gymnasts
                                     where g.team.number == t.number
                                     where div == null || g.division.name == div.name || div.name == ProgramConstants.INCLUDE_ALL_DIVISIONS
                                     where g.vaultScore.score.HasValue || g.barsScore.score.HasValue || g.beamScore.score.HasValue || g.floorScore.score.HasValue
                                     orderby g.IComparableOverallScore descending
                                     select g;
                
                foreach(Gymnast g in gymnastsOnTeam) {
                    string fullname = g.firstName + " " + g.lastName;
                    if(fullname.Length > (labelColWidth - 2)) { fullname = fullname.Substring(0, labelColWidth - 1); }

                    string vax = GymnastScoreUsedByTeamForEvent("Vault", g.competitorNumber, (meetParameters.useDivisions ? div : null)) ? "" : "-";
                    string bax = GymnastScoreUsedByTeamForEvent("Bars", g.competitorNumber, (meetParameters.useDivisions ? div : null)) ? "" : "-";
                    string bex = GymnastScoreUsedByTeamForEvent("Beam", g.competitorNumber, (meetParameters.useDivisions ? div : null)) ? "" : "-";
                    string flx = GymnastScoreUsedByTeamForEvent("Floor", g.competitorNumber, (meetParameters.useDivisions ? div : null)) ? "" : "-";

                    contentHolder = new Tuple<string, string, string, string, string, string>(fullname,
                        (g.vaultScore.score.HasValue ? vax+HelperMethods.FormatScore(g.vaultScore.score)+vax : ""),
                        (g.barsScore.score.HasValue ? bax+HelperMethods.FormatScore(g.barsScore.score)+bax : ""),
                        (g.beamScore.score.HasValue ? bex+HelperMethods.FormatScore(g.beamScore.score)+bex : ""),
                        (g.floorScore.score.HasValue ? flx+HelperMethods.FormatScore(g.floorScore.score)+flx : ""),
                        (g.IncludeInAllAroundResults ? HelperMethods.FormatScore(g.overallScore) : ""));

                    teamList.Add(new Run() { Text = ArrangeLine_Normal(stopPoints, contentHolder) });
                }
                

                output.Add(teamList);
                ++teamIdx;
            }

            return output;
        }

        private static string ArrangeLine_Normal(Tuple<int, int, int, int, int> stops, Tuple<string, string, string, string, string, string> content) {
            StringBuilder sb = new StringBuilder();

            sb.Append(content.Item1);
            while(sb.Length < stops.Item1) { sb.Append(" "); }
            sb.Append(content.Item2);
            while (sb.Length < stops.Item2) { sb.Append(" "); }
            sb.Append(content.Item3);
            while (sb.Length < stops.Item3) { sb.Append(" "); }
            sb.Append(content.Item4);
            while (sb.Length < stops.Item4) { sb.Append(" "); }
            sb.Append(content.Item5);
            while (sb.Length < stops.Item5) { sb.Append(" "); }
            sb.Append(content.Item6);

            return sb.ToString();
        }

        /*
        private static Span ArrangeLine_UnderlineAsSpan(Tuple<int, int, int, int, int> stops, Tuple<string, string, string, string, string, string> content) {
            Span span = new Span();
            StringBuilder sb = new StringBuilder();

            int pad = stops.Item1 - content.Item1.Length;
            var Underline1 = new Underline();
            Underline1.Inlines.Add(new Run() { Text = content.Item1 });
            span.Inlines.Add(Underline1);
            sb.Append(' ', pad);
            span.Inlines.Add(new Run() { Text = sb.ToString() });

            sb.Clear();
            pad = stops.Item2 - content.Item2.Length;
            var Underline2 = new Underline();
            Underline2.Inlines.Add(new Run() { Text = content.Item2 });
            span.Inlines.Add(Underline2);
            sb.Append(' ', pad);
            span.Inlines.Add(new Run() { Text = sb.ToString() });

            sb.Clear();
            pad = stops.Item3 - content.Item3.Length;
            var Underline3 = new Underline();
            Underline3.Inlines.Add(new Run() { Text = content.Item3 });
            span.Inlines.Add(Underline3);
            sb.Append(' ', pad);
            span.Inlines.Add(new Run() { Text = sb.ToString() });

            sb.Clear();
            pad = stops.Item4 - content.Item4.Length;
            var Underline4 = new Underline();
            Underline4.Inlines.Add(new Run() { Text = content.Item4 });
            span.Inlines.Add(Underline4);
            sb.Append(' ', pad);
            span.Inlines.Add(new Run() { Text = sb.ToString() });

            sb.Clear();
            pad = stops.Item5 - content.Item5.Length;
            var Underline5 = new Underline();
            Underline5.Inlines.Add(new Run() { Text = content.Item5 });
            span.Inlines.Add(Underline5);
            sb.Append(' ', pad);
            span.Inlines.Add(new Run() { Text = sb.ToString() });

            var Underline6 = new Underline();
            Underline6.Inlines.Add(new Run() { Text = content.Item6 });
            span.Inlines.Add(Underline6);

            return span;
        }
        */

        public static List<UserControl_RankingDisplayTeam> create_TeamStandings_Complex(Division div = null) {
            var outputList = new List<UserControl_RankingDisplayTeam>();

            var orderedTeams = OrderTeamsForThisRanking("Overall", div);

            foreach(Team t in orderedTeams) {
                outputList.Add(new UserControl_RankingDisplayTeam(t, div));
            }

            return outputList;
        }


        // DCL (11/17/2016): This method has second life due to complications in getting the formatting 
        // that displays in the UI to translate to the printed output...
        // 
        // Even if I get the printed output to match the for my configuration, not sure I will be able to
        // do so in a fully platform independent manner
        //
        // Note (11/12/2016): These are kind of long and would benefit from breaking them
        // up with sub-methods, however, since they are no longer part of the core app
        // will not invest the time to do so (should have no further development here, 
        // thus no need to manage complexity of future extension

        public static string GymnastRankString(int gymnastNbr, string eventString = "OVERALL", Division d = null) {
            var gymnastsOrdered = OrderGymnastsForThisRanking(eventString, d);

            double? prevScore = _meetParameters.maxScore * _meetParameters.scoresForCompetition * 4 + 1;
            double noMoreScores = -1;
            double? nextScore = noMoreScores;
            int prevRank = 0;
            int idx = 1;
            foreach (var g in gymnastsOrdered) {
                if (idx < gymnastsOrdered.Count()) { nextScore = gymnastsOrdered.ElementAt(idx).score(eventString); }
                else { nextScore = noMoreScores; }

                if (g.competitorNumber == gymnastNbr) {
                    return ((g.score(eventString) == prevScore) ? prevRank.ToString() : idx.ToString())
                        + (((g.score(eventString) == prevScore) || (g.score(eventString) == nextScore)) ? "T" : " ");
                }
                else {
                    if (g.score(eventString) != prevScore) {
                        prevRank = idx;
                    }
                    prevScore = g.score(eventString);
                    ++idx;
                }
            }

            return "X";
        }

        public static string GymnastRankString(int gymnastNbr, List<Gymnast> groupForRanking, string eventString = "OVERALL") {
            var gymnastsOrdered = OrderGymnastsForThisRanking(eventString, groupForRanking);

            double? prevScore = _meetParameters.maxScore * _meetParameters.scoresForCompetition * 4 + 1;
            double noMoreScores = -1;
            double? nextScore = noMoreScores;
            int prevRank = 0;
            int idx = 1;
            foreach (var g in gymnastsOrdered) {
                if (idx < gymnastsOrdered.Count()) { nextScore = gymnastsOrdered.ElementAt(idx).score(eventString); }
                else { nextScore = noMoreScores; }

                if (g.competitorNumber == gymnastNbr) {
                    return ((g.score(eventString) == prevScore) ? prevRank.ToString() : idx.ToString())
                        + (((g.score(eventString) == prevScore) || (g.score(eventString) == nextScore)) ? "T" : " ");
                }
                else {
                    if (g.score(eventString) != prevScore) {
                        prevRank = idx;
                    }
                    prevScore = g.score(eventString);
                    ++idx;
                }
            }

            return "X";
        }

        public static string createString_gymnastStandings(string eventString, Division div = null) {
            StringBuilder sb = new StringBuilder();
            sb.Append(Environment.NewLine);

            var rankedGymnasts = OrderGymnastsForThisRanking(eventString, div);

            int rankDigitsToUse = rankedGymnasts.Count().ToString().Length;
            int i = 1;
            double? prevScore = _meetParameters.maxScore + 1;
            double? nextScore = _meetParameters.minScore - 1;

            foreach (Gymnast g in rankedGymnasts) {
                double? score = g.score(eventString);

                if (!score.HasValue) { break; }

                int toAppend = rankDigitsToUse - i.ToString().Length;
                string rankPrefix = "";
                while (toAppend > 0) {
                    rankPrefix += " ";
                    --toAppend;
                }

                string scoreString = HelperMethods.FormatScore(score);
                int currentlyBeforeDot = scoreString.IndexOf(".");
                int currentlyAfterDot = scoreString.Length - currentlyBeforeDot - 1;

                toAppend = ProgramConstants.DIGITS_BEFORE_DOT - currentlyBeforeDot;
                string scorePrefix = "";
                while (toAppend > 0) {
                    scorePrefix += " ";
                    --toAppend;
                }

                toAppend = ProgramConstants.DIGITS_AFTER_DOT - currentlyAfterDot;
                string scoreSuffix = "";
                while (toAppend > 0) {
                    scoreSuffix += " ";
                    --toAppend;
                }

                sb.Append(rankPrefix + GymnastRankString(g.competitorNumber, eventString, div) + ": " + scorePrefix + scoreString + scoreSuffix);
                sb.Append(" " + g.firstName + " " + g.lastName + ", " + g.team.name);
                sb.Append(Environment.NewLine);

                int blockSizeForSpacer = 5;
                if (i % blockSizeForSpacer == 0) {
                    sb.AppendLine();
                }

                ++i;
                prevScore = score.Value;
            }

            return sb.ToString();
        }

        // *********************************************************************************************************
        // CALCULATE QUALIFIERS
        // *********************************************************************************************************

        public static void calculateQualifiers(Division d = null) {

            _qualExcludedGymnasts = new List<Gymnast>();
            foreach (Gymnast g in _gymnasts) {
                if (Meet._qualificationParameters.gymnastsToExclude.Contains(g.competitorNumber)) {
                    _qualExcludedGymnasts.Add(g);
                }
            }

            calculateTeamQualifiers(d);
            calculateAAQualifiers(d);
            calculateEventQualifiers(d);
        }


        public static void calculateTeamQualifiers(Division d = null) {
            _teamQualifiers = new List<Team>();

            var rankedTeams = OrderTeamsForThisRanking("Overall", d);

            int i = 0;
            while (i < _qualificationParameters.teamQualifiers) {
                if (i >= rankedTeams.Count()) { break; }

                Team t = rankedTeams.ElementAt(i);
                _teamQualifiers.Add(t);
                ++i;
            }
        }

        public static void calculateAAQualifiers(Division d = null) {
            _aaQualifiers = new List<Gymnast>();

            var rankedEligibleGymnasts = from g in _gymnasts
                                         where d == null || d.name == g.division.name || d.name == ProgramConstants.INCLUDE_ALL_DIVISIONS
                                         where !_qualExcludedGymnasts.Contains(g)
                                         where !_teamQualifiers.Contains(g.team)
                                         where g.IncludeInAllAroundResults
                                         orderby g.IComparableOverallScore descending
                                         select g;
            int i = 0;
            double? prevScore = _meetParameters.maxScore;
            while ((i < _qualificationParameters.aaQualifiers) ||
                        ((rankedEligibleGymnasts.Count() > i) &&
                        (rankedEligibleGymnasts.ElementAt(i).overallScore == prevScore))) {
                if (i >= rankedEligibleGymnasts.Count()) { break; }

                _aaQualifiers.Add(rankedEligibleGymnasts.ElementAt(i));
                prevScore = rankedEligibleGymnasts.ElementAt(i).overallScore;
                ++i;
            }


        }
        public static void calculateEventQualifiers(Division d = null) {
            _eventQualifiersVault = new List<Gymnast>();
            _eventQualifiersBars = new List<Gymnast>();
            _eventQualifiersBeam = new List<Gymnast>();
            _eventQualifiersFloor = new List<Gymnast>();

            // VAULT
            var rankedEligibleGymnasts = from g in _gymnasts
                                         where d == null || d.name == g.division.name || d.name == ProgramConstants.INCLUDE_ALL_DIVISIONS
                                         where !_qualExcludedGymnasts.Contains(g)
                                         where !_teamQualifiers.Contains(g.team)
                                         where !_aaQualifiers.Contains(g)
                                         where g.vaultScore.score.HasValue
                                         orderby g.IComparableVaultScore descending
                                         select g;

            int i = 0;
            double? prevScore = _meetParameters.maxScore;
            while ((i < _qualificationParameters.eventQualifiers) ||
                        ((rankedEligibleGymnasts.Count() > i) &&
                        (rankedEligibleGymnasts.ElementAt(i).vaultScore.score == prevScore))) {
                if (i >= rankedEligibleGymnasts.Count()) { break; }

                _eventQualifiersVault.Add(rankedEligibleGymnasts.ElementAt(i));
                prevScore = rankedEligibleGymnasts.ElementAt(i).vaultScore.score;
                ++i;
            }

            // BARS
            rankedEligibleGymnasts = from g in _gymnasts
                                     where d == null || d.name == g.division.name || d.name == ProgramConstants.INCLUDE_ALL_DIVISIONS
                                     where !_qualExcludedGymnasts.Contains(g)
                                     where !_teamQualifiers.Contains(g.team)
                                     where !_aaQualifiers.Contains(g)
                                     where g.barsScore.score.HasValue
                                     orderby g.IComparableBarsScore descending
                                     select g;

            i = 0;
            while ((i < _qualificationParameters.eventQualifiers) ||
                        ((rankedEligibleGymnasts.Count() > i) &&
                        (rankedEligibleGymnasts.ElementAt(i).barsScore.score == prevScore))) {
                if (i >= rankedEligibleGymnasts.Count()) { break; }

                _eventQualifiersBars.Add(rankedEligibleGymnasts.ElementAt(i));
                prevScore = rankedEligibleGymnasts.ElementAt(i).barsScore.score;
                ++i;
            }

            // BEAM
            rankedEligibleGymnasts = from g in _gymnasts
                                     where d == null || d.name == g.division.name || d.name == ProgramConstants.INCLUDE_ALL_DIVISIONS
                                     where !_qualExcludedGymnasts.Contains(g)
                                     where !_teamQualifiers.Contains(g.team)
                                     where !_aaQualifiers.Contains(g)
                                     where g.beamScore.score.HasValue
                                     orderby g.IComparableBeamScore descending
                                     select g;

            i = 0;
            while ((i < _qualificationParameters.eventQualifiers) ||
                        ((rankedEligibleGymnasts.Count() > i) &&
                        (rankedEligibleGymnasts.ElementAt(i).beamScore.score == prevScore))) {
                if (i >= rankedEligibleGymnasts.Count()) { break; }

                _eventQualifiersBeam.Add(rankedEligibleGymnasts.ElementAt(i));
                prevScore = rankedEligibleGymnasts.ElementAt(i).beamScore.score;
                ++i;
            }

            // FLOOR
            rankedEligibleGymnasts = from g in _gymnasts
                                     where d == null || d.name == g.division.name || d.name == ProgramConstants.INCLUDE_ALL_DIVISIONS
                                     where !_qualExcludedGymnasts.Contains(g)
                                     where !_teamQualifiers.Contains(g.team)
                                     where !_aaQualifiers.Contains(g)
                                     where g.floorScore.score.HasValue
                                     orderby g.IComparableFloorScore descending
                                     select g;

            i = 0;
            while ((i < _qualificationParameters.eventQualifiers) ||
                        ((rankedEligibleGymnasts.Count() > i) &&
                        (rankedEligibleGymnasts.ElementAt(i).floorScore.score == prevScore))) {
                if (i >= rankedEligibleGymnasts.Count()) { break; }

                _eventQualifiersFloor.Add(rankedEligibleGymnasts.ElementAt(i));
                prevScore = rankedEligibleGymnasts.ElementAt(i).floorScore.score;
                ++i;
            }
        }


        // *********************************************************************************************************
        // DISPLAY QUALIFIERS
        // *********************************************************************************************************
        public static List<TextBlock> DisplayQualifiers(Division d = null) {
            calculateQualifiers(d);

            List<TextBlock> outputTextBlocks = new List<TextBlock>();

            var standardSpacing = new Thickness(0, 5, 0, 20);
            var MonoFont_CourierNew = new Windows.UI.Xaml.Media.FontFamily("Courier New");
            int spacerBlockSize = 5;
            int TitleFontSize = 16;
            int DetailFontSize = 14;

            TextBlock TeamTitle = new TextBlock();
            TeamTitle.FontSize = TitleFontSize;
            TeamTitle.FontFamily = MonoFont_CourierNew;
            TeamTitle.FontWeight = Windows.UI.Text.FontWeights.Bold;
            TeamTitle.Text = "Team qualifiers: Top " + _qualificationParameters.teamQualifiers.ToString() + " teams";

            TextBlock TeamsQualified = new TextBlock();
            TeamsQualified.FontSize = DetailFontSize;
            TeamsQualified.FontFamily = MonoFont_CourierNew;
            TeamsQualified.Text = "";
            TeamsQualified.Margin = standardSpacing;
            foreach (Team t in _teamQualifiers) {
                TeamsQualified.Text += "#" + TeamRankString(t.number, "Overall", d) + " " 
                    + HelperMethods.FormatScore_Mono(TeamScoreOverall(t.number, d), true)  
                    + " " + t.name;
                TeamsQualified.Text += Environment.NewLine;
            }

            TextBlock AATitle = new TextBlock();
            AATitle.FontSize = TitleFontSize;
            AATitle.FontFamily = MonoFont_CourierNew;
            AATitle.FontWeight = Windows.UI.Text.FontWeights.Bold;
            AATitle.Text = "All-around qualifiers: Top " + _qualificationParameters.aaQualifiers.ToString() + " not on a qualifying team (plus ties)";

            TextBlock AAQualified = new TextBlock();
            AAQualified.FontSize = DetailFontSize;
            AAQualified.FontFamily = MonoFont_CourierNew;
            AAQualified.Text = "";
            AAQualified.Margin = standardSpacing;
            int spacerIdx = 0;
            foreach (Gymnast g in _aaQualifiers) {
                AAQualified.Text += rankPrefix(g.competitorNumber, _aaQualifiers, "Overall") 
                    + GymnastRankString(g.competitorNumber, _aaQualifiers, "Overall") 
                    + " " + HelperMethods.FormatScore_Mono(g.overallScore) + " " 
                    + g.firstName + " " + g.lastName + ", " + g.team.name + " (" +  g.competitorNumber.ToString() + ")";
                AAQualified.Text += Environment.NewLine;
                ++spacerIdx;
                if((spacerIdx % spacerBlockSize)==0) {
                    AAQualified.Text += Environment.NewLine;
                }
            }

            TextBlock VaultTitle = new TextBlock();
            VaultTitle.FontSize = TitleFontSize;
            VaultTitle.FontFamily = MonoFont_CourierNew;
            VaultTitle.FontWeight = Windows.UI.Text.FontWeights.Bold;
            VaultTitle.Text = "Vault qualifiers: Top " + _qualificationParameters.eventQualifiers.ToString() + " not already qualified (plus ties)";

            TextBlock VaultQualified = new TextBlock();
            VaultQualified.FontSize = DetailFontSize;
            VaultQualified.FontFamily = MonoFont_CourierNew;
            VaultQualified.Text = "";
            VaultQualified.Margin = standardSpacing;
            spacerIdx = 0;
            foreach (Gymnast g in _eventQualifiersVault) {
                VaultQualified.Text += rankPrefix(g.competitorNumber, _eventQualifiersVault, "Vault") 
                    + GymnastRankString(g.competitorNumber, _eventQualifiersVault, "Vault") 
                    + " " + HelperMethods.FormatScore_Mono(g.vaultScore.score) + " "
                    + g.firstName + " " + g.lastName + ", " + g.team.name + " (" + g.competitorNumber.ToString() + ")";
                VaultQualified.Text += Environment.NewLine;
                ++spacerIdx;
                if ((spacerIdx % spacerBlockSize) == 0) {
                    VaultQualified.Text += Environment.NewLine;
                }
            }

            TextBlock BarsTitle = new TextBlock();
            BarsTitle.FontSize = TitleFontSize;
            BarsTitle.FontFamily = MonoFont_CourierNew;
            BarsTitle.FontWeight = Windows.UI.Text.FontWeights.Bold;
            BarsTitle.Text = "Bars qualifiers: Top " + _qualificationParameters.eventQualifiers.ToString() + " not already qualified (plus ties)";

            TextBlock BarsQualified = new TextBlock();
            BarsQualified.FontSize = DetailFontSize;
            BarsQualified.FontFamily = MonoFont_CourierNew;
            BarsQualified.Text = "";
            BarsQualified.Margin = standardSpacing;
            spacerIdx = 0;
            foreach (Gymnast g in _eventQualifiersBars) {
                BarsQualified.Text += rankPrefix(g.competitorNumber, _eventQualifiersBars, "Bars") 
                    + GymnastRankString(g.competitorNumber, _eventQualifiersBars, "Bars") 
                    + " " + HelperMethods.FormatScore_Mono(g.barsScore.score) + " "
                    + g.firstName + " " + g.lastName + ", " + g.team.name + " (" + g.competitorNumber.ToString() + ")";
                BarsQualified.Text += Environment.NewLine;
                ++spacerIdx;
                if ((spacerIdx % spacerBlockSize) == 0) {
                    BarsQualified.Text += Environment.NewLine;
                }
            }

            TextBlock BeamTitle = new TextBlock();
            BeamTitle.FontSize = TitleFontSize;
            BeamTitle.FontFamily = MonoFont_CourierNew;
            BeamTitle.FontWeight = Windows.UI.Text.FontWeights.Bold;
            BeamTitle.Text = "Beam qualifiers: Top " + _qualificationParameters.eventQualifiers.ToString() + " not already qualified (plus ties)";

            TextBlock BeamQualified = new TextBlock();
            BeamQualified.FontSize = DetailFontSize;
            BeamQualified.FontFamily = MonoFont_CourierNew;
            BeamQualified.Text = "";
            BeamQualified.Margin = standardSpacing;
            spacerIdx = 0;
            foreach (Gymnast g in _eventQualifiersBeam) {
                BeamQualified.Text += rankPrefix(g.competitorNumber, _eventQualifiersBeam, "Beam") 
                    + GymnastRankString(g.competitorNumber, _eventQualifiersBeam, "Beam")
                    + " " + HelperMethods.FormatScore_Mono(g.beamScore.score) + " "
                    + g.firstName + " " + g.lastName + ", " + g.team.name + " (" + g.competitorNumber.ToString() + ")";
                BeamQualified.Text += Environment.NewLine;
                ++spacerIdx;
                if ((spacerIdx % spacerBlockSize) == 0) {
                    BeamQualified.Text += Environment.NewLine;
                }
            }

            TextBlock FloorTitle = new TextBlock();
            FloorTitle.FontSize = TitleFontSize;
            FloorTitle.FontFamily = MonoFont_CourierNew;
            FloorTitle.FontWeight = Windows.UI.Text.FontWeights.Bold;
            FloorTitle.Text = "Floor qualifiers: Top " + _qualificationParameters.eventQualifiers.ToString() + " not already qualified (plus ties)";

            TextBlock FloorQualified = new TextBlock();
            FloorQualified.FontSize = DetailFontSize;
            FloorQualified.FontFamily = MonoFont_CourierNew;
            FloorQualified.Text = "";
            FloorQualified.Margin = standardSpacing;
            spacerIdx = 0;
            foreach (Gymnast g in _eventQualifiersFloor) {
                FloorQualified.Text += rankPrefix(g.competitorNumber, _eventQualifiersFloor, "Floor") 
                    + GymnastRankString(g.competitorNumber, _eventQualifiersFloor, "Floor")
                    + " " + HelperMethods.FormatScore_Mono(g.floorScore.score) + " "
                    + g.firstName + " " + g.lastName + ", " + g.team.name + " (" + g.competitorNumber.ToString() + ")";
                FloorQualified.Text += Environment.NewLine;
                ++spacerIdx;
                if ((spacerIdx % spacerBlockSize) == 0) {
                    FloorQualified.Text += Environment.NewLine;
                }
            }

            outputTextBlocks.Add(TeamTitle);
            outputTextBlocks.Add(TeamsQualified);
            outputTextBlocks.Add(AATitle);
            outputTextBlocks.Add(AAQualified);
            outputTextBlocks.Add(VaultTitle);
            outputTextBlocks.Add(VaultQualified);
            outputTextBlocks.Add(BarsTitle);
            outputTextBlocks.Add(BarsQualified);
            outputTextBlocks.Add(BeamTitle);
            outputTextBlocks.Add(BeamQualified);
            outputTextBlocks.Add(FloorTitle);
            outputTextBlocks.Add(FloorQualified);
            return outputTextBlocks;
        }

        private static string rankPrefix(int n, List<Gymnast> qualList, string eventString) {
            int maxLen = qualList.Count.ToString().Length + 1; // The +1 takes into account the tie indicator ("T")
            int currLen = GymnastRankString(n, qualList, eventString).Length;

            int need = maxLen - currLen;
            int idx = 0;
            string prefix = "";
            while(idx < need) {
                prefix += " ";
                ++idx;
            }
            return prefix;
        }

        public static List<Paragraph> DisplayQualifiers_Printing(Division d = null) {
            calculateQualifiers(d);

            List<Paragraph> outputParagraphs = new List<Paragraph>();

            var standardSpacing = new Thickness(0, 5, 0, 20);
            var MonoFont_CourierNew = new Windows.UI.Xaml.Media.FontFamily("Courier New");
            int spacerBlockSize = 5;
            int TitleFontSize = 16;
            int DetailFontSize = 14;

            Paragraph TeamTitle = new Paragraph();
            TeamTitle.FontSize = TitleFontSize;
            TeamTitle.FontFamily = MonoFont_CourierNew;
            TeamTitle.FontWeight = Windows.UI.Text.FontWeights.Bold;
            TeamTitle.Inlines.Add(new Run() {
                Text = "Team qualifiers: Top " + _qualificationParameters.teamQualifiers.ToString() + " teams"
            });

            Paragraph TeamsQualified = new Paragraph();
            TeamsQualified.FontSize = DetailFontSize;
            TeamsQualified.FontFamily = MonoFont_CourierNew;
            string TeamsQualifiedRunString = "";
            TeamsQualified.Margin = standardSpacing;
            foreach (Team t in _teamQualifiers) {
                TeamsQualifiedRunString += "#" + TeamRankString(t.number, "Overall", d) + " "
                    + HelperMethods.FormatScore_Mono(TeamScoreOverall(t.number, d), true)
                    + " " + t.name;
                TeamsQualifiedRunString += Environment.NewLine;
            }
            TeamsQualified.Inlines.Add(new Run() { Text = TeamsQualifiedRunString });

            Paragraph AATitle = new Paragraph();
            AATitle.FontSize = TitleFontSize;
            AATitle.FontFamily = MonoFont_CourierNew;
            AATitle.FontWeight = Windows.UI.Text.FontWeights.Bold;
            AATitle.Inlines.Add(new Run() {
                Text = "All-around qualifiers: Top " + _qualificationParameters.aaQualifiers.ToString()
                + " not on a qualifying team (plus ties)"
            });

            Paragraph AAQualified = new Paragraph();
            AAQualified.FontSize = DetailFontSize;
            AAQualified.FontFamily = MonoFont_CourierNew;
            string AAQualifiedRunString = "";
            AAQualified.Margin = standardSpacing;
            int spacerIdx = 0;
            foreach (Gymnast g in _aaQualifiers) {
                AAQualifiedRunString += rankPrefix(g.competitorNumber, _aaQualifiers, "Overall")
                    + GymnastRankString(g.competitorNumber, _aaQualifiers, "Overall")
                    + " " + HelperMethods.FormatScore_Mono(g.overallScore) + " "
                    + g.firstName + " " + g.lastName + ", " + g.team.name + " (" + g.competitorNumber.ToString() + ")";
                AAQualifiedRunString += Environment.NewLine;
                ++spacerIdx;
                if (((spacerIdx % spacerBlockSize) == 0) && (spacerIdx != _aaQualifiers.Count)) {
                    AAQualifiedRunString += Environment.NewLine;
                }
            }
            AAQualified.Inlines.Add(new Run() { Text = AAQualifiedRunString });

            Paragraph VaultTitle = new Paragraph();
            VaultTitle.FontSize = TitleFontSize;
            VaultTitle.FontFamily = MonoFont_CourierNew;
            VaultTitle.FontWeight = Windows.UI.Text.FontWeights.Bold;
            VaultTitle.Inlines.Add(new Run() {
                Text = "Vault qualifiers: Top " + _qualificationParameters.eventQualifiers.ToString()
                + " not already qualified (plus ties)"
            });

            Paragraph VaultQualified = new Paragraph();
            VaultQualified.FontSize = DetailFontSize;
            VaultQualified.FontFamily = MonoFont_CourierNew;
            string VaultQualifiedRunString = "";
            VaultQualified.Margin = standardSpacing;
            spacerIdx = 0;
            foreach (Gymnast g in _eventQualifiersVault) {
                VaultQualifiedRunString += rankPrefix(g.competitorNumber, _eventQualifiersVault, "Vault")
                    + GymnastRankString(g.competitorNumber, _eventQualifiersVault, "Vault")
                    + " " + HelperMethods.FormatScore_Mono(g.vaultScore.score) + " "
                    + g.firstName + " " + g.lastName + ", " + g.team.name + " (" + g.competitorNumber.ToString() + ")";
                VaultQualifiedRunString += Environment.NewLine;
                ++spacerIdx;
                if (((spacerIdx % spacerBlockSize) == 0) && (spacerIdx != _eventQualifiersVault.Count)) {
                    VaultQualifiedRunString += Environment.NewLine;
                }
            }
            VaultQualified.Inlines.Add(new Run() { Text = VaultQualifiedRunString });

            Paragraph BarsTitle = new Paragraph();
            BarsTitle.FontSize = TitleFontSize;
            BarsTitle.FontFamily = MonoFont_CourierNew;
            BarsTitle.FontWeight = Windows.UI.Text.FontWeights.Bold;
            BarsTitle.Inlines.Add(new Run() {
                Text = "Bars qualifiers: Top " + _qualificationParameters.eventQualifiers.ToString()
                + " not already qualified (plus ties)"
            });

            Paragraph BarsQualified = new Paragraph();
            BarsQualified.FontSize = DetailFontSize;
            BarsQualified.FontFamily = MonoFont_CourierNew;
            string BarsQualifiedRunString = "";
            BarsQualified.Margin = standardSpacing;
            spacerIdx = 0;
            foreach (Gymnast g in _eventQualifiersBars) {
                BarsQualifiedRunString += rankPrefix(g.competitorNumber, _eventQualifiersBars, "Bars")
                    + GymnastRankString(g.competitorNumber, _eventQualifiersBars, "Bars")
                    + " " + HelperMethods.FormatScore_Mono(g.barsScore.score) + " "
                    + g.firstName + " " + g.lastName + ", " + g.team.name + " (" + g.competitorNumber.ToString() + ")";
                BarsQualifiedRunString += Environment.NewLine;
                ++spacerIdx;
                if (((spacerIdx % spacerBlockSize) == 0) && (spacerIdx != _eventQualifiersBars.Count)) {
                    BarsQualifiedRunString += Environment.NewLine;
                }
            }
            BarsQualified.Inlines.Add(new Run() { Text = BarsQualifiedRunString });

            Paragraph BeamTitle = new Paragraph();
            BeamTitle.FontSize = TitleFontSize;
            BeamTitle.FontFamily = MonoFont_CourierNew;
            BeamTitle.FontWeight = Windows.UI.Text.FontWeights.Bold;
            BeamTitle.Inlines.Add(new Run() {
                Text = "Beam qualifiers: Top " + _qualificationParameters.eventQualifiers.ToString()
                + " not already qualified (plus ties)"
            });

            Paragraph BeamQualified = new Paragraph();
            BeamQualified.FontSize = DetailFontSize;
            BeamQualified.FontFamily = MonoFont_CourierNew;
            string BeamQualifiedRunString = "";
            BeamQualified.Margin = standardSpacing;
            spacerIdx = 0;
            foreach (Gymnast g in _eventQualifiersBeam) {
                BeamQualifiedRunString += rankPrefix(g.competitorNumber, _eventQualifiersBeam, "Beam")
                    + GymnastRankString(g.competitorNumber, _eventQualifiersBeam, "Beam")
                    + " " + HelperMethods.FormatScore_Mono(g.beamScore.score) + " "
                    + g.firstName + " " + g.lastName + ", " + g.team.name + " (" + g.competitorNumber.ToString() + ")";
                BeamQualifiedRunString += Environment.NewLine;
                ++spacerIdx;
                if (((spacerIdx % spacerBlockSize) == 0) && (spacerIdx != _eventQualifiersBeam.Count)) {
                    BeamQualifiedRunString += Environment.NewLine;
                }
            }
            BeamQualified.Inlines.Add(new Run() { Text = BeamQualifiedRunString });

            Paragraph FloorTitle = new Paragraph();
            FloorTitle.FontSize = TitleFontSize;
            FloorTitle.FontFamily = MonoFont_CourierNew;
            FloorTitle.FontWeight = Windows.UI.Text.FontWeights.Bold;
            FloorTitle.Inlines.Add(new Run() {
                Text = "Floor qualifiers: Top " + _qualificationParameters.eventQualifiers.ToString()
                + " not already qualified (plus ties)"
            });

            Paragraph FloorQualified = new Paragraph();
            FloorQualified.FontSize = DetailFontSize;
            FloorQualified.FontFamily = MonoFont_CourierNew;
            string FloorQualifiedRunString = "";
            FloorQualified.Margin = standardSpacing;
            spacerIdx = 0;
            foreach (Gymnast g in _eventQualifiersFloor) {
                FloorQualifiedRunString += rankPrefix(g.competitorNumber, _eventQualifiersFloor, "Floor")
                    + GymnastRankString(g.competitorNumber, _eventQualifiersFloor, "Floor")
                    + " " + HelperMethods.FormatScore_Mono(g.floorScore.score) + " "
                    + g.firstName + " " + g.lastName + ", " + g.team.name + " (" + g.competitorNumber.ToString() + ")";
                FloorQualifiedRunString += Environment.NewLine;
                ++spacerIdx;
                if (((spacerIdx % spacerBlockSize) == 0) && (spacerIdx != _eventQualifiersFloor.Count)) {
                    FloorQualifiedRunString += Environment.NewLine;
                }
            }
            FloorQualified.Inlines.Add(new Run() { Text = FloorQualifiedRunString });

            outputParagraphs.Add(TeamTitle);
            outputParagraphs.Add(TeamsQualified);
            outputParagraphs.Add(AATitle);
            outputParagraphs.Add(AAQualified);
            outputParagraphs.Add(VaultTitle);
            outputParagraphs.Add(VaultQualified);
            outputParagraphs.Add(BarsTitle);
            outputParagraphs.Add(BarsQualified);
            outputParagraphs.Add(BeamTitle);
            outputParagraphs.Add(BeamQualified);
            outputParagraphs.Add(FloorTitle);
            outputParagraphs.Add(FloorQualified);
            return outputParagraphs;
        }

        // *********************************************************************************************************
        // NO LONGER USED EXCEPT FOR IN TESTING
        // *********************************************************************************************************

        public static string RosterString(string divisionName = null) {
            StringBuilder sb = new StringBuilder();
            sb.Append(Environment.NewLine);

            IOrderedEnumerable<Gymnast> rosterOfGymnasts;

            if (divisionName == null) {
                rosterOfGymnasts = from g in _gymnasts
                                   orderby g.competitorNumber
                                   select g;
            }
            else {
                rosterOfGymnasts = from g in _gymnasts
                                   where g.division.name == divisionName
                                   orderby g.competitorNumber
                                   select g;
            }

            int digitsToUse = rosterOfGymnasts.Count().ToString().Length;
            int i = 1;
            foreach (Gymnast g in rosterOfGymnasts) {
                int toAppend = digitsToUse - i.ToString().Length;
                string prefix = "";
                while (toAppend > 0) {
                    prefix += "0";
                    --toAppend;
                }

                sb.Append(prefix + i.ToString() + ": " + g.ToString() + " - " + g.team.name);
                sb.Append(Environment.NewLine);
                ++i;
            }

            return sb.ToString();
        }

    }
}



