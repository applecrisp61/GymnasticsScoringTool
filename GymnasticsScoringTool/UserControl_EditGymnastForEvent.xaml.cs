using System;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace GymnasticsScoringTool {
    public sealed partial class UserControl_EditGymnastForEvent : UserControl {
        private string _previousString;
        private Flyout _invalidScoreFlyout;
        private TextBlock _invalidScoreTextBlock;

        public UserControl_EditGymnastForEvent() {
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
            TextBlock tBkfName = new TextBlock();
            TextBlock tBklName = new TextBlock();
            TextBox tBxScore = new TextBox();

            #region EstablishDataBindings.

            Binding bindingNbr = new Binding();
            Binding bindingFName = new Binding();
            Binding bindingLName = new Binding();
            Binding bindingScore = new Binding();

            bindingNbr.Source = g;
            bindingFName.Source = g;
            bindingLName.Source = g;
            bindingScore.Source = g;

            if((MainPage._currentlySelectedEvent == "") || (MainPage._currentlySelectedEvent == null)) {
                throw new Exception("EXCEPTION: _currentlySelectedEvent is null >> during attempt to set-up gymnasts for edit in UserControl_EditGymnastForEvent");
            }

            string scorePath = "";
            switch(MainPage._currentlySelectedEvent.ToUpper()) {
            case "VAULT":
                scorePath = "vaultScore.displayScore";
                break;
            case "BARS":
                scorePath = "barsScore.displayScore";
                break;
            case "BEAM":
                scorePath = "beamScore.displayScore";
                break;
            case "FLOOR":
                scorePath = "floorScore.displayScore";
                break;
            default:
                throw new Exception_UnkownEvent(MainPage._currentlySelectedEvent.ToUpper());
            }

            bindingNbr.Path = new PropertyPath("competitorNumberDisplay");
            bindingFName.Path = new PropertyPath("firstName");
            bindingLName.Path = new PropertyPath("lastName");
            bindingScore.Path = new PropertyPath(scorePath);

            bindingNbr.Mode = BindingMode.OneWay;
            bindingFName.Mode = BindingMode.OneWay;
            bindingLName.Mode = BindingMode.OneWay;
            bindingScore.Mode = BindingMode.TwoWay;

            bindingScore.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;

            BindingOperations.SetBinding(tBkNbr, TextBlock.TextProperty, bindingNbr);
            BindingOperations.SetBinding(tBkfName, TextBlock.TextProperty, bindingFName);
            BindingOperations.SetBinding(tBklName, TextBlock.TextProperty, bindingLName);
            BindingOperations.SetBinding(tBxScore, TextBox.TextProperty, bindingScore);

            #endregion EstablishDataBindings.

            Grid.SetColumn(tBkNbr, 0); Grid.SetRow(tBkNbr, currentRow);
            Grid.SetColumn(tBkfName, 1); Grid.SetRow(tBkfName, currentRow);
            Grid.SetColumn(tBklName, 2); Grid.SetRow(tBklName, currentRow);
            Grid.SetColumn(tBxScore, 3); Grid.SetRow(tBxScore, currentRow);

            tBxScore.GotFocus += textBox_GotFocus;
            tBxScore.LostFocus += textBox_LostFocus;
            tBxScore.LostFocus += textBox_LostFocus_ValidateScoreInput;

            EditGymnastsGrid.Children.Add(tBkNbr);
            EditGymnastsGrid.Children.Add(tBkfName);
            EditGymnastsGrid.Children.Add(tBklName);
            EditGymnastsGrid.Children.Add(tBxScore);
        }

        public void GymnastToValidate(Gymnast g) {

            EditGymnastsGrid.RowDefinitions.Add(new RowDefinition());
            int currentRow = EditGymnastsGrid.RowDefinitions.Count - 1;

            TextBlock tBkNbr = new TextBlock();
            TextBlock tBkfName = new TextBlock();
            TextBlock tBklName = new TextBlock();
            TextBox tBxScore = new TextBox();

            #region EstablishDataBindings.

            Binding bindingNbr = new Binding();
            Binding bindingFName = new Binding();
            Binding bindingLName = new Binding();
            Binding bindingScore = new Binding();

            bindingNbr.Source = g;
            bindingFName.Source = g;
            bindingLName.Source = g;
            bindingScore.Source = g;

            if ((MainPage._currentlySelectedEvent == "") || (MainPage._currentlySelectedEvent == null)) {
                throw new Exception("EXCEPTION: _currentlySelectedEvent is null >> during attempt to set-up gymnasts for edit in UserControl_EditGymnastForEvent");
            }

            string scorePath = "";
            switch (MainPage._currentlySelectedEvent.ToUpper()) {
            case "VAULT":
                scorePath = "vaultScore.displayScore";
                break;
            case "BARS":
                scorePath = "barsScore.displayScore";
                break;
            case "BEAM":
                scorePath = "beamScore.displayScore";
                break;
            case "FLOOR":
                scorePath = "floorScore.displayScore";
                break;
            default:
                throw new Exception_UnkownEvent(MainPage._currentlySelectedEvent.ToUpper());
            }

            bindingNbr.Path = new PropertyPath("competitorNumberDisplay");
            bindingFName.Path = new PropertyPath("firstName");
            bindingLName.Path = new PropertyPath("lastName");
            bindingScore.Path = new PropertyPath(scorePath);

            bindingNbr.Mode = BindingMode.OneWay;
            bindingFName.Mode = BindingMode.OneWay;
            bindingLName.Mode = BindingMode.OneWay;
            bindingScore.Mode = BindingMode.TwoWay;

            bindingScore.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;

            BindingOperations.SetBinding(tBkNbr, TextBlock.TextProperty, bindingNbr);
            BindingOperations.SetBinding(tBkfName, TextBlock.TextProperty, bindingFName);
            BindingOperations.SetBinding(tBklName, TextBlock.TextProperty, bindingLName);
            BindingOperations.SetBinding(tBxScore, TextBox.TextProperty, bindingScore);

            #endregion EstablishDataBindings. 

            Grid.SetColumn(tBkNbr, 0); Grid.SetRow(tBkNbr, currentRow);
            Grid.SetColumn(tBkfName, 1); Grid.SetRow(tBkfName, currentRow);
            Grid.SetColumn(tBklName, 2); Grid.SetRow(tBklName, currentRow);
            Grid.SetColumn(tBxScore, 3); Grid.SetRow(tBxScore, currentRow);

            tBxScore.GotFocus += textBox_GotFocus;
            tBxScore.LostFocus += textBox_LostFocus;
            tBxScore.LostFocus += textBox_LostFocus_ValidateScoreInput;

            EditGymnastsGrid.Children.Add(tBkNbr);
            EditGymnastsGrid.Children.Add(tBkfName);
            EditGymnastsGrid.Children.Add(tBklName);
            EditGymnastsGrid.Children.Add(tBxScore);
        }

        private void textBox_GotFocus(object sender, RoutedEventArgs e) {
            TextBox textBox = sender as TextBox;
            if (textBox == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new TextBox()).GetType(), sender.GetType());
            }

            _previousString = textBox.Text;
            textBox.SelectAll();
        }

        private void textBox_LostFocus(object sender, RoutedEventArgs e) {
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

            if (textBox.Text == "") {
                textBox.Text = _previousString;
                return;
            }

            if(textBox.Text == _previousString) {
                return;
            }

            string adjString = EventScore.AdjustInputString(textBox.Text);
            textBox.Text = adjString;
            if (EventScore.IsValid(textBox.Text)) {
                // not showing an overall score that needs to be preliminarily updated
                int row = Grid.GetRow(textBox);
                int competitorNbr = (from tbk in EditGymnastsGrid.Children.OfType<TextBlock>()
                                     where Grid.GetRow(tbk) == row
                                     where Grid.GetColumn(tbk) == 0
                                     select Int32.Parse(tbk.Text)).Single();

                EventScore es = new EventScore();
                if(textBox.Text != ProgramConstants.NULL_SCORE_STRING) {
                    es = new EventScore(Double.Parse(textBox.Text));
                }
                Meet.RegisterEventUpdate(competitorNbr, es);
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

        internal Tuple<bool, string> GymnastAlreadyPresent(int nbr) {
            var query = from child in EditGymnastsGrid.Children
                        where child is TextBlock
                        where (child as TextBlock)?.Text == nbr.ToString()
                        where Grid.GetColumn(child as TextBlock) == 0
                        select child;

            var check = query.Any();
            string msg = $"Query count: {query.Count()}; Total children: {EditGymnastsGrid.Children.Count}";
            return new Tuple<bool, string>(check, msg);
        }

        internal void UpdateGymnastForEvent(int nbr, double score) {
            var query = (from child in EditGymnastsGrid.Children
                         where child is TextBlock
                         where (child as TextBlock)?.Text == nbr.ToString()
                         where Grid.GetColumn(child as TextBlock) == 0
                         select child).Single() as TextBlock;

            int row = Grid.GetRow(query);

            var updateTbx = (from child in EditGymnastsGrid.Children
                             where child is TextBox
                             where Grid.GetRow(child as TextBox) == row
                             where Grid.GetColumn(child as TextBox) == 3
                             select child).Single() as TextBox;

            updateTbx.Text = score.ToString();
        }
    }
}
