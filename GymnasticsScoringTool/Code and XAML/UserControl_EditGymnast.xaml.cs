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
    public sealed partial class UserControl_EditGymnast : UserControl {
        private string _previousString;
        private Flyout _invalidScoreFlyout;
        private TextBlock _invalidScoreTextBlock;

        // column ordering (run-time constant)
        private static readonly int _colNbr = 0;
        private static readonly int _colFName = 1;
        private static readonly int _colLName = 2;
        private static readonly int _colVault = 3;
        private static readonly int _colBars = 4;
        private static readonly int _colBeam = 5;
        private static readonly int _colFloor = 6;
        private static readonly int _colOverall = 7;
        private static readonly int _colDiv = 8;
        private static readonly int _colDel = 9;

        public UserControl_EditGymnast() {
            this.InitializeComponent();

            _invalidScoreFlyout = new Flyout();
            _invalidScoreFlyout.Placement = FlyoutPlacementMode.Full;
            _invalidScoreTextBlock = new TextBlock();
            _invalidScoreTextBlock.Text = "No invalid score message associated";
            TextBlock invalidScoreTitle = new TextBlock();
            invalidScoreTitle.Text = ProgramConstants.TITLE_INVALID_SCORE;
            invalidScoreTitle.FontSize = FontSize + 2;
            invalidScoreTitle.FontWeight = Windows.UI.Text.FontWeights.Bold;
            StackPanel invalidScoreStackPanel = new StackPanel();
            invalidScoreStackPanel.Children.Add(invalidScoreTitle);
            invalidScoreStackPanel.Children.Add(_invalidScoreTextBlock);
            _invalidScoreFlyout.Content = invalidScoreStackPanel;
            
        }

        public void GymnastToEdit(Gymnast g) {

            EditGymnastsGrid.RowDefinitions.Add(new RowDefinition());
            int currentRow = EditGymnastsGrid.RowDefinitions.Count - 1;

            TextBlock tBkNbr = new TextBlock();
            TextBox tBxfName = new TextBox();
            TextBox tBxlName = new TextBox();
            TextBox tBxVault = new TextBox();
            TextBox tBxBars = new TextBox();
            TextBox tBxBeam = new TextBox();
            TextBox tBxFloor = new TextBox();
            TextBlock tBkOverall = new TextBlock();
            ComboBox comboDivision = new ComboBox();
            Button btnDelete = new Button();

            #region EstablishDataBindings.

            Binding bindingNbr = new Binding();
            Binding bindingFName = new Binding();
            Binding bindingLName = new Binding();
            Binding bindingVault = new Binding();
            Binding bindingBars = new Binding();
            Binding bindingBeam = new Binding();
            Binding bindingFloor = new Binding();
            Binding bindingOverall = new Binding();

            bindingNbr.Source = g;
            bindingFName.Source = g;
            bindingLName.Source = g;
            bindingVault.Source = g;
            bindingBars.Source = g;
            bindingBeam.Source = g;
            bindingFloor.Source = g;
            bindingOverall.Source = g;

            bindingNbr.Path = new PropertyPath("competitorNumberDisplay");
            bindingFName.Path = new PropertyPath("firstName");
            bindingLName.Path = new PropertyPath("lastName");
            bindingVault.Path = new PropertyPath("vaultScore.displayScore");
            bindingBars.Path = new PropertyPath("barsScore.displayScore");
            bindingBeam.Path = new PropertyPath("beamScore.displayScore");
            bindingFloor.Path = new PropertyPath("floorScore.displayScore");
            bindingOverall.Path = new PropertyPath("overallScoreDisplay");

            bindingNbr.Mode = BindingMode.OneWay;
            bindingFName.Mode = BindingMode.TwoWay;
            bindingLName.Mode = BindingMode.TwoWay;
            bindingVault.Mode = BindingMode.TwoWay;
            bindingBars.Mode = BindingMode.TwoWay;
            bindingBeam.Mode = BindingMode.TwoWay;
            bindingFloor.Mode = BindingMode.TwoWay;
            bindingOverall.Mode = BindingMode.OneWay;

            // by making these explicit, I can change the root values only when the ButtonUpdate is clicked
            // allowing for easier logic to ignore the inputs when ButtonCancel is selected
            bindingFName.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
            bindingLName.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
            bindingVault.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
            bindingBars.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
            bindingBeam.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
            bindingFloor.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;

            BindingOperations.SetBinding(tBkNbr, TextBlock.TextProperty, bindingNbr);
            BindingOperations.SetBinding(tBxfName, TextBox.TextProperty, bindingFName);
            BindingOperations.SetBinding(tBxlName, TextBox.TextProperty, bindingLName);
            BindingOperations.SetBinding(tBxVault, TextBox.TextProperty, bindingVault);
            BindingOperations.SetBinding(tBxBars, TextBox.TextProperty, bindingBars);
            BindingOperations.SetBinding(tBxBeam, TextBox.TextProperty, bindingBeam);
            BindingOperations.SetBinding(tBxFloor, TextBox.TextProperty, bindingFloor);
            BindingOperations.SetBinding(tBkOverall, TextBlock.TextProperty, bindingOverall);

            #endregion EstablishDataBindings.

            #region ComboBoxDivision.
            int idx = Meet.divisions.IndexOf(g.division);
            comboDivision.ItemsSource = Meet.divisions;
            comboDivision.SelectedIndex = idx;
            comboDivision.FontSize = 14;
            comboDivision.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            comboDivision.BorderThickness = new Thickness(0);
            comboDivision.VerticalAlignment = VerticalAlignment.Top;
            comboDivision.HorizontalAlignment = HorizontalAlignment.Left;
            #endregion ComboBoxDivision.

            #region ButtonDelete.
            object o;
            bool resourceKeyFound = App.Current.Resources.TryGetValue("ButtonLC", out o);
            ControlTemplate template = o as ControlTemplate;

            if (template != null) { btnDelete.Template = template; }
            btnDelete.FontSize = 12;
            btnDelete.HorizontalAlignment = HorizontalAlignment.Center;
            btnDelete.VerticalAlignment = VerticalAlignment.Top;
            // btnDelete.Margin = new Thickness(0, 0, 0, 15);
            btnDelete.Content = "Delete";
            btnDelete.Click += BtnDelete_Click;
            #endregion ButtonDelete.

            Grid.SetColumn(tBkNbr, _colNbr); Grid.SetRow(tBkNbr, currentRow);
            Grid.SetColumn(tBxfName, _colFName); Grid.SetRow(tBxfName, currentRow);
            Grid.SetColumn(tBxlName, _colLName); Grid.SetRow(tBxlName, currentRow);
            Grid.SetColumn(tBxVault, _colVault); Grid.SetRow(tBxVault, currentRow);
            Grid.SetColumn(tBxBars, _colBars); Grid.SetRow(tBxBars, currentRow);
            Grid.SetColumn(tBxBeam, _colBeam); Grid.SetRow(tBxBeam, currentRow);
            Grid.SetColumn(tBxFloor, _colFloor); Grid.SetRow(tBxFloor, currentRow);
            Grid.SetColumn(tBkOverall, _colOverall); Grid.SetRow(tBkOverall, currentRow);
            Grid.SetColumn(comboDivision, _colDiv); Grid.SetRow(comboDivision, currentRow);
            Grid.SetColumn(btnDelete, _colDel); Grid.SetRow(btnDelete, currentRow);

            tBxfName.GotFocus += textBox_GotFocus;
            tBxlName.GotFocus += textBox_GotFocus;
            tBxVault.GotFocus += textBox_GotFocus;
            tBxBars.GotFocus += textBox_GotFocus;
            tBxBeam.GotFocus += textBox_GotFocus;
            tBxFloor.GotFocus += textBox_GotFocus;

            tBxfName.LostFocus += textBox_LostFocus;
            tBxlName.LostFocus += textBox_LostFocus;
            tBxVault.LostFocus += textBox_LostFocus;
            tBxBars.LostFocus += textBox_LostFocus;
            tBxBeam.LostFocus += textBox_LostFocus;
            tBxFloor.LostFocus += textBox_LostFocus;

            tBxVault.LostFocus += textBox_LostFocus_ValidateScoreInput;
            tBxBars.LostFocus += textBox_LostFocus_ValidateScoreInput;
            tBxBeam.LostFocus += textBox_LostFocus_ValidateScoreInput;
            tBxFloor.LostFocus += textBox_LostFocus_ValidateScoreInput;

            EditGymnastsGrid.Children.Add(tBkNbr);
            EditGymnastsGrid.Children.Add(tBxfName);
            EditGymnastsGrid.Children.Add(tBxlName);
            EditGymnastsGrid.Children.Add(tBxVault);
            EditGymnastsGrid.Children.Add(tBxBars);
            EditGymnastsGrid.Children.Add(tBxBeam);
            EditGymnastsGrid.Children.Add(tBxFloor);
            EditGymnastsGrid.Children.Add(tBkOverall);
            EditGymnastsGrid.Children.Add(comboDivision);
            EditGymnastsGrid.Children.Add(btnDelete);
        }

        // 
        private void BtnDelete_Click(object sender, RoutedEventArgs e) {
            Button button = sender as Button;
            if (button == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new Button()).GetType(), sender.GetType());
            }

            int row = Grid.GetRow(button);

            var query = (from tbk in EditGymnastsGrid.Children.OfType<TextBlock>()
                         where Grid.GetRow(tbk) == row
                         where Grid.GetColumn(tbk) == _colNbr
                         select tbk.Text).Single();

            int competitorNbr = Int32.Parse(query);

            Meet.MarkGymnastForDeletion(competitorNbr);

            bool keepPruning = true;
            while(keepPruning) {
                var toTrim = (from fe in EditGymnastsGrid.Children.OfType<FrameworkElement>()
                              where Grid.GetRow(fe) == row
                              select fe).FirstOrDefault();

                if(toTrim==null) { keepPruning = false; }
                else {
                    EditGymnastsGrid.Children.Remove(toTrim);
                }
            }

            foreach(var child in EditGymnastsGrid.Children) {
                if(child is FrameworkElement) {
                    if(Grid.GetRow(child as FrameworkElement) > row) {
                        int newRow = Grid.GetRow(child as FrameworkElement) - 1;
                        Grid.SetRow(child as FrameworkElement, newRow);
                    }
                }
            }

            EditGymnastsGrid.RowDefinitions.Remove(EditGymnastsGrid.RowDefinitions.Last());
        }


        private void textBox_GotFocus(object sender, RoutedEventArgs args) {
            TextBox textBox = sender as TextBox;
            if (textBox == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new TextBox()).GetType(), sender.GetType());
            }

            _previousString = textBox.Text;
            textBox.SelectAll();
        }

        private void textBox_LostFocus(object sender, RoutedEventArgs args) {
            TextBox textBox = sender as TextBox;
            if (textBox == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new TextBox()).GetType(), sender.GetType());
            }

            if (textBox.Text == "") { textBox.Text = _previousString; }
        }

        private void textBox_LostFocus_ValidateScoreInput(object sender, RoutedEventArgs args) {
            TextBox textBox = sender as TextBox;
            if (textBox == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new TextBox()).GetType(), sender.GetType());
            }

            string adjString = EventScore.AdjustInputString(textBox.Text);
            textBox.Text = adjString;
            if (EventScore.IsValid(textBox.Text)) {
                int currentRow = Grid.GetRow(textBox);

                TextBlock overallTBk = (from tbk in EditGymnastsGrid.Children.OfType<TextBlock>()
                                        where Grid.GetRow(tbk) == currentRow
                                        where Grid.GetColumn(tbk) == _colOverall
                                        select tbk).Single(); // should only be one

                overallTBk.Text = generatePreliminary_OverallScoreDisplay(currentRow);
            }
            else {
                if (EventScore.IsPoorlyFormatted(textBox.Text)) {
                    _invalidScoreTextBlock.Text = ProgramConstants.POORLY_FORMATTED_SCORE;
                }
                else if (EventScore.IsOutsideValidRange(textBox.Text)) {
                    _invalidScoreTextBlock.Text = ProgramConstants.SCORE_OUTSIDE_VALID_RANGE;
                }
                else {
                    _invalidScoreTextBlock.Text = "Invalid score >> reason unknown (indicates programming error... should not see this message)";
                }

                _invalidScoreFlyout.ShowAt(textBox);
            }
        }

        private string generatePreliminary_OverallScoreDisplay(int row) {

            var rowOfElements = from fe in EditGymnastsGrid.Children.OfType<FrameworkElement>()
                                where Grid.GetRow(fe) == row
                                select fe;

            TextBox tBxVault = (from tbx in rowOfElements.OfType<TextBox>()
                               where Grid.GetColumn(tbx) == _colVault
                               select tbx).Single(); // should only be one!

            TextBox tBxBars = (from tbx in rowOfElements.OfType<TextBox>()
                                where Grid.GetColumn(tbx) == _colBars
                                select tbx).Single(); // should only be one!

            TextBox tBxBeam = (from tbx in rowOfElements.OfType<TextBox>()
                                where Grid.GetColumn(tbx) == _colBeam
                                select tbx).Single(); // should only be one!

            TextBox tBxFloor = (from tbx in rowOfElements.OfType<TextBox>()
                                where Grid.GetColumn(tbx) == _colFloor
                                select tbx).Single(); // should only be one!

            TextBlock tBkNbr = (from tbk in rowOfElements.OfType<TextBlock>()
                                where Grid.GetColumn(tbk) == _colNbr
                                select tbk).Single(); // should only be one!

            int gymnastNbr = Int32.Parse(tBkNbr.Text);
            Tuple<EventScore, EventScore, EventScore, EventScore> scoresTuple = Meet.GetGymnastEventScoresTuple(gymnastNbr);

            double? vault, bars, beam, floor;
            double overall;
            vault = generateEventScore(tBxVault.Text, "Vault", scoresTuple);
            bars = generateEventScore(tBxBars.Text, "Bars", scoresTuple);
            beam = generateEventScore(tBxBeam.Text, "Beam", scoresTuple);
            floor = generateEventScore(tBxFloor.Text, "Floor", scoresTuple);
            overall = (vault.HasValue ? vault.Value : 0) + (bars.HasValue ? bars.Value : 0) 
                + (beam.HasValue ? beam.Value : 0) + (floor.HasValue ? floor.Value : 0);

            return (vault.HasValue || bars.HasValue || beam.HasValue || floor.HasValue) ? overall.ToString() : ProgramConstants.NULL_SCORE_STRING;
        }

        private double? generateEventScore(string s, string eventString, Tuple<EventScore, EventScore, EventScore, EventScore> scoresTuple) {
            if(EventScore.IsValid(s)) {
                if(s== ProgramConstants.NULL_SCORE_STRING) { return null; }
                return Double.Parse(s);
            }
            else {
                switch(eventString.ToUpper()) {
                case "VAULT":
                    return scoresTuple.Item1.score;
                case "BARS":
                    return scoresTuple.Item2.score;
                case "BEAM":
                    return scoresTuple.Item3.score;
                case "FLOOR":
                    return scoresTuple.Item4.score;
                default:
                    return null;
                }
            }
        }
    }
}
