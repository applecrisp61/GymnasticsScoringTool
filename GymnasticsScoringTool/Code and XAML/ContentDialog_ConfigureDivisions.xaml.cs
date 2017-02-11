using System;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GymnasticsScoringTool_01 {
    public sealed partial class ContentDialog_ConfigureDivisions : ContentDialog {

        private List<Division> _previousDivisions;
        private int _previousComboBoxIndex = 0;

        private string _previousValue;
        private Flyout _exceptionDuringDivisionConfigFlyout;
        private TextBlock _exceptionDuringDivisionConfigTextBlock;
        private TextBlock _exceptionDuringDivisionConfigTitle;

        public ContentDialog_ConfigureDivisions() {
            this.InitializeComponent();

            _exceptionDuringDivisionConfigFlyout = new Flyout();
            _exceptionDuringDivisionConfigFlyout.Placement = FlyoutPlacementMode.Full;
            _exceptionDuringDivisionConfigTextBlock = new TextBlock();
            _exceptionDuringDivisionConfigTextBlock.Text = "No invalid parameter message associated";
            _exceptionDuringDivisionConfigTitle = new TextBlock();
            _exceptionDuringDivisionConfigTitle.Text = ProgramConstants.TITLE_EXCEPTION_DURING_DIVISION_CONFIG;
            _exceptionDuringDivisionConfigTitle.FontSize = FontSize + 2;
            _exceptionDuringDivisionConfigTitle.FontWeight = Windows.UI.Text.FontWeights.Bold;
            StackPanel invalidScoreStackPanel = new StackPanel();
            invalidScoreStackPanel.Children.Add(_exceptionDuringDivisionConfigTitle);
            invalidScoreStackPanel.Children.Add(_exceptionDuringDivisionConfigTextBlock);
            _exceptionDuringDivisionConfigFlyout.Content = invalidScoreStackPanel;

            this.Loaded += ContentDialog_ConfigureDivisions_LoadedFormatting;
            this.Loaded += ContentDialog_LoadedSetUpdateCancelButtons;
            this.Loaded += ShowAndSaveCurrentDivisionNames;
        }

        
        private void ShowAndSaveCurrentDivisionNames(object sender, RoutedEventArgs e) {
            if(_previousDivisions==null) {
                _previousDivisions = new List<Division>();
            }
            else {
                _previousDivisions.Clear();
            }

            foreach(var d in Meet.divisions) {
                _previousDivisions.Add(d);
            }

            _previousComboBoxIndex = MainPage.GetDivisionComboBoxSelectionIndex();

            DisplayConfiguredDivisions();
        }

        private void ContentDialog_ConfigureDivisions_LoadedFormatting(object sender, RoutedEventArgs e) {
            // Some resizing the first time this ContentDialog is pulled up (don't need it as wide as typical)
            DependencyObject dObj = HelperMethods.GetFirstVisualTreeElementWithName(this, "BackgroundElement");
            Border border = dObj as Border;
            if (border == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new Border()).GetType(), dObj.GetType());
            }

            border.MaxHeight = 600;
            border.MinHeight = 500;
            border.MaxWidth = 600;
            border.MinWidth = 500;

            this.MaxHeight = 600;
            this.MinHeight = 500;
            this.MaxWidth = 600;
            this.MinWidth = 500;
            this.Height = 600; // the desired value
            this.Width = 500; // the desired value

            // Only need to do it once
            this.Loaded -= ContentDialog_ConfigureDivisions_LoadedFormatting;
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

            // Only need to do it once
            this.Loaded -= ContentDialog_ConfigureDivisions_LoadedFormatting;
        }

        private void ButtonContentDialog_Click(object sender, RoutedEventArgs e) {
            Button button = sender as Button;
            if (button == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new Button()).GetType(), sender.GetType());
            }

            string command = HelperMethods.GrabButtonLabelString(button);
            if(command.ToUpper()=="CANCEL") {
                Meet.ResetDivisions(_previousDivisions);
                DisplayConfiguredDivisions();
                MainPage.SetDivisionComboBoxSelectionIndex(_previousComboBoxIndex);
            }
            if(command.ToUpper()=="Update") {
                MainPage.UpdateDivisionComboOptionsDisplay();
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

        private void DisplayConfiguredDivisions() {
            DivisionStackPanel.Children.Clear();

            int divisions = Meet.divisions.Count;
            int i = 1;

            while (i < divisions) {
                AddRowForDivision(Meet.divisions.ElementAt(i));
                ++i;
            }
        }

        private void AddRowForDivision(Division d) {
            Grid divisionPlacementGrid = new Grid();
            divisionPlacementGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
            divisionPlacementGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            TextBlock tbk = new TextBlock() {
                Text = d.name,
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 20, 0),
            };
            Grid.SetColumn(tbk, 0);
            divisionPlacementGrid.Children.Add(tbk);

            AddButton_RemoveDivision(divisionPlacementGrid, 1);

            DivisionStackPanel.Children.Add(divisionPlacementGrid);

        }

        private void AddButton_RemoveDivision(Grid g, int col) {

            object o;
            bool resourceKeyFound = App.Current.Resources.TryGetValue("ButtonLC", out o);
            ControlTemplate template = o as ControlTemplate;

            Button b = new Button();
            if (template != null) { b.Template = template; }
            b.FontSize = 14;
            b.HorizontalAlignment = HorizontalAlignment.Left;
            b.VerticalAlignment = VerticalAlignment.Top;
            b.Margin = new Thickness(0, 0, 0, 15);
            b.Content = "Remove";
            Grid.SetColumn(b, col);
            b.Click += RemoveDivision_Click;

            g.Children.Add(b);
        }

        private void Button_ClickAddDivision(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            if (b == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new Button()).GetType(), sender.GetType());
            }

            string newDivisionName = tbk_NewDivision.Text;
            Division newDivision;
            try { newDivision = new Division(newDivisionName); }
            catch {
                _exceptionDuringDivisionConfigTextBlock.Text = ProgramConstants.DIVISION_ALREADY_EXISTS;
                _exceptionDuringDivisionConfigFlyout.ShowAt(b);
                return;
            }

            Meet.AddDivision(newDivision);
            DisplayConfiguredDivisions();
        }

        private void RemoveDivision_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            if (b == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new Button()).GetType(), sender.GetType());
            }

            Grid g = b.Parent as Grid;
            if( g==null ) {
                throw new Exception_UnexpectedDataTypeEncountered((new Grid()).GetType(), b.Parent.GetType());
            }

            var divtbk = (from tbk in g.Children.OfType<TextBlock>()
                          where Grid.GetColumn(tbk) == 0
                          select tbk).Single(); // should only be one

            Division divToRemove = Meet.GetDivisionWithName(divtbk.Text);

            if(Meet.CountOfGymnastsInDivision(divToRemove) > 0) {
                _exceptionDuringDivisionConfigTextBlock.Text = ProgramConstants.DIVISION_NOT_EMPTY;
                _exceptionDuringDivisionConfigFlyout.ShowAt(b);
                return;

            }
            Meet.RemoveDivision(divToRemove);
            DisplayConfiguredDivisions();
        }
    }
}
