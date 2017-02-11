using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace GymnasticsScoringTool {
    public sealed partial class UserControl_RankingDisplayTeam : UserControl {

        private int _colLabel = 0;
        private int _colVault = 1;
        private int _colBars = 2;
        private int _colBeam = 3;
        private int _colFloor = 4;
        private int _colTotal = 5;

        public UserControl_RankingDisplayTeam(Team t, Division d) {
            this.InitializeComponent();

            TBk_TeamRankScoreName.Text = "#" + Meet.TeamRankString(t.numberBase, "Overall", d) + "  " 
                + Meet.TeamScoreOverall(t.numberBase, d).ToString() + " > " + t.name;

            tbkScoreVault.Text = HelperMethods.FormatScore(Meet.TeamScoreVault(t.numberBase, d));
            tbkScoreBars.Text = HelperMethods.FormatScore(Meet.TeamScoreBars(t.numberBase, d));
            tbkScoreBeam.Text = HelperMethods.FormatScore(Meet.TeamScoreBeam(t.numberBase, d));
            tbkScoreFloor.Text = HelperMethods.FormatScore(Meet.TeamScoreFloor(t.numberBase, d));
            tbkScoreTotal.Text = HelperMethods.FormatScore(Meet.TeamScoreOverall(t.numberBase, d));

            tbkRankVault.Text = Meet.TeamRankString(t.numberBase, "Vault", d);
            tbkRankBars.Text = Meet.TeamRankString(t.numberBase, "Bars", d);
            tbkRankBeam.Text = Meet.TeamRankString(t.numberBase, "Beam", d);
            tbkRankFloor.Text = Meet.TeamRankString(t.numberBase, "Floor", d);
            tbkRankTotal.Text = Meet.TeamRankString(t.numberBase, "Overall", d);

            tbkGapVault.Text = HelperMethods.FormatScore(Meet.GapToHighestTeam(t.numberBase, "Vault", d));
            tbkGapBars.Text = HelperMethods.FormatScore(Meet.GapToHighestTeam(t.numberBase, "Bars", d));
            tbkGapBeam.Text = HelperMethods.FormatScore(Meet.GapToHighestTeam(t.numberBase, "Beam", d));
            tbkGapFloor.Text = HelperMethods.FormatScore(Meet.GapToHighestTeam(t.numberBase, "Floor", d));
            tbkGapTotal.Text = HelperMethods.FormatScore(Meet.GapToHighestTeam(t.numberBase, "Overall", d));

            var teamRosterTupleEnumerable = Meet.ProvideTeamRoster_TupleEnumerable(t, d);

            var sortedTeamRoster = from g in teamRosterTupleEnumerable
                                   orderby (Meet.GetGymnastEventScoresTuple(g.Item1).Item1
                                            + Meet.GetGymnastEventScoresTuple(g.Item1).Item2
                                            + Meet.GetGymnastEventScoresTuple(g.Item1).Item3
                                            + Meet.GetGymnastEventScoresTuple(g.Item1).Item4) descending
                                   select g;

            foreach (var tuple in sortedTeamRoster) {
                var s = Meet.GetGymnastEventScoresTuple(tuple.Item1);

                if(s.Item1.score.HasValue || s.Item2.score.HasValue || s.Item3.score.HasValue || s.Item4.score.HasValue) {
                    int currRowIdx = TeamGrid.RowDefinitions.Count;
                    TeamGrid.RowDefinitions.Add(new RowDefinition() {
                        MinHeight = ProgramConstants.STANDARD_OUTPUT_ROW_HEIGHT + 1,
                        MaxHeight = ProgramConstants.STANDARD_OUTPUT_ROW_HEIGHT + 1});
                    TextBlock tbkName = new TextBlock();
                    tbkName.Text = tuple.Item2 + " " + tuple.Item3;
                    tbkName.VerticalAlignment = VerticalAlignment.Top;
                    Grid.SetColumn(tbkName, _colLabel); Grid.SetRow(tbkName, currRowIdx);
                    TeamGrid.Children.Add(tbkName);

                    int fontDecrementIfNotUsed = 1;
                    var mediumGray = Windows.UI.Color.FromArgb(0xFF, 0x90, 0x90, 0x90);
                    var notUsedScoreForeground = new SolidColorBrush(mediumGray);
                    string beforeMark = "-";
                    string afterMark = "-";

                    if (s.Item1.score.HasValue) {
                        TextBlock tbk = new TextBlock() { Text = s.Item1.ToString(), VerticalAlignment = VerticalAlignment.Top };
                        Grid.SetColumn(tbk, _colVault); Grid.SetRow(tbk, currRowIdx);
                        if (!Meet.GymnastScoreUsedByTeamForEvent("Vault", tuple.Item1)) {
                            tbk.Text = beforeMark + tbk.Text + afterMark;
                            tbk.FontStyle = Windows.UI.Text.FontStyle.Italic;
                            tbk.FontSize -= fontDecrementIfNotUsed;
                            tbk.Foreground = notUsedScoreForeground;
                        }        
                        TeamGrid.Children.Add(tbk);
                    }

                    if (s.Item2.score.HasValue) {
                        TextBlock tbk = new TextBlock() { Text = s.Item2.ToString(), VerticalAlignment = VerticalAlignment.Top };
                        Grid.SetColumn(tbk, _colBars); Grid.SetRow(tbk, currRowIdx);
                        if(!Meet.GymnastScoreUsedByTeamForEvent("Bars", tuple.Item1)) {
                            tbk.Text = beforeMark + tbk.Text + afterMark;
                            tbk.FontStyle = Windows.UI.Text.FontStyle.Italic;
                            tbk.FontSize -= fontDecrementIfNotUsed;
                            tbk.Foreground = notUsedScoreForeground;
                        }
                        TeamGrid.Children.Add(tbk);
                    }

                    if (s.Item3.score.HasValue) {
                        TextBlock tbk = new TextBlock() { Text = s.Item3.ToString(), VerticalAlignment = VerticalAlignment.Top };
                        Grid.SetColumn(tbk, _colBeam); Grid.SetRow(tbk, currRowIdx);
                        if (!Meet.GymnastScoreUsedByTeamForEvent("Beam", tuple.Item1)) {
                            tbk.Text = beforeMark + tbk.Text + afterMark;
                            tbk.FontStyle = Windows.UI.Text.FontStyle.Italic;
                            tbk.FontSize -= fontDecrementIfNotUsed;
                            tbk.Foreground = notUsedScoreForeground;
                        }
                        TeamGrid.Children.Add(tbk);
                    }

                    if (s.Item4.score.HasValue) {
                        TextBlock tbk = new TextBlock() { Text = s.Item4.ToString(), VerticalAlignment = VerticalAlignment.Top };
                        Grid.SetColumn(tbk, _colFloor); Grid.SetRow(tbk, currRowIdx);
                        if (!Meet.GymnastScoreUsedByTeamForEvent("Floor", tuple.Item1)) {
                            tbk.Text = beforeMark + tbk.Text + afterMark;
                            tbk.FontStyle = Windows.UI.Text.FontStyle.Italic;
                            tbk.FontSize -= fontDecrementIfNotUsed;
                            tbk.Foreground = notUsedScoreForeground;
                        }
                        TeamGrid.Children.Add(tbk);
                    }

                    if (s.Item1.score.HasValue && s.Item2.score.HasValue && s.Item3.score.HasValue && s.Item4.score.HasValue) {
                        TextBlock tbk = new TextBlock() { Text = (s.Item1 + s.Item2 + s.Item3 + s.Item4).ToString(),
                            VerticalAlignment = VerticalAlignment.Top };
                        Grid.SetColumn(tbk, _colTotal); Grid.SetRow(tbk, currRowIdx);
                        TeamGrid.Children.Add(tbk);
                    }
                }

            }

            this.MinWidth = ProgramConstants.STANDARD_OUTPUT_WIDTH;
            this.MaxWidth = ProgramConstants.STANDARD_OUTPUT_WIDTH;
            this.Margin = new Thickness(0, 0, 0, 30);
        }
    }
}
