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

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GymnasticsScoringTool_01 {
    public sealed partial class ContentDialog_GymnastSelector : ContentDialog {
        private bool _updateCancelRemoved = false;

        public ContentDialog_GymnastSelector() {
            this.InitializeComponent();
            Loaded += SetupGymnastButtons;
        }

        private void SetupGymnastButtons(object sender, RoutedEventArgs e) {

            if (MainPage._currentlySelectedTeam.rosterSize == 0) {
                Hide(); // no gymnasts on team, so just shut this box instead of having someone 
                // stuck here with no gymnasts to select
                Flyout noGymnastsOnTeamFlyout = new Flyout();
                TextBlock noGymnastsMessageTBk = new TextBlock();
                noGymnastsMessageTBk.Text = "No gymnasts on selected team";
                noGymnastsOnTeamFlyout.Content = noGymnastsMessageTBk;
                noGymnastsMessageTBk.FontSize = 20;
                noGymnastsMessageTBk.FontWeight = Windows.UI.Text.FontWeights.Bold;
                noGymnastsOnTeamFlyout.Placement = FlyoutPlacementMode.Full;
                noGymnastsOnTeamFlyout.ShowAt(this);
            }

            foreach (Tuple<int, string, string> rosterElement in Meet.ProvideTeamRoster_TupleEnumerable(MainPage._currentlySelectedTeam)) {

                object o;
                bool resourceKeyFound = App.Current.Resources.TryGetValue("ButtonLC", out o);
                ControlTemplate template = o as ControlTemplate;

                Button nbrButton = new Button();
                if (template != null) { nbrButton.Template = template; }
                nbrButton.Content = rosterElement.Item1.ToString();
                nbrButton.Click += GymnastSelection;
                nbrButton.Margin = new Thickness(5, 1, 5, 1);

                TextBlock tBkFName = new TextBlock();
                tBkFName.Text = rosterElement.Item2;
                tBkFName.VerticalAlignment = VerticalAlignment.Center;

                TextBlock tBkLName = new TextBlock();
                tBkLName.Text = rosterElement.Item3;
                tBkLName.VerticalAlignment = VerticalAlignment.Center;

                int currentRowIdx = GymnastSelectorGrid.RowDefinitions.Count;
                RowDefinition rowToAdd = new RowDefinition();
                GymnastSelectorGrid.RowDefinitions.Add(rowToAdd);

                Grid.SetColumn(nbrButton, 0);  Grid.SetRow(nbrButton, currentRowIdx);
                Grid.SetColumn(tBkFName, 1); Grid.SetRow(tBkFName, currentRowIdx);
                Grid.SetColumn(tBkLName, 2); Grid.SetRow(tBkLName, currentRowIdx);

                GymnastSelectorGrid.Children.Add(nbrButton);
                GymnastSelectorGrid.Children.Add(tBkFName);
                GymnastSelectorGrid.Children.Add(tBkLName);
            }

            if (_updateCancelRemoved) { } // don't need to remove the buttons... they're already gone
            else {
                HelperMethods.RemoveUpdateCancelButtons_FromContentDialogControlTemplate(this);
                _updateCancelRemoved = true;
            }

            // Also, some resizing the first time this ContentDialog is pulled up (don't need it as wide as typical)
            DependencyObject dObj = HelperMethods.GetFirstVisualTreeElementWithName(this, "BackgroundElement");
            Border border = dObj as Border;
            if (border == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new Border()).GetType(), dObj.GetType());
            }

            border.MaxHeight = 400;
            border.MinHeight = 300;
            border.MaxWidth = 400;
            border.MinWidth = 300;

            this.MaxHeight = 500;
            this.MinHeight = 300;
            this.MaxWidth = 400;
            this.MinWidth = 300;
            this.Height = 450; // the desired value
            this.Width = 300; // the desired value
        }

        private void GymnastSelection(object sender, RoutedEventArgs args) {
            Button b = sender as Button;
            if (b == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new Button()).GetType(), sender.GetType());
            }

            string nbrString;
            if (b.Content is string) { nbrString = b.Content as String; }
            else if (b.Content is TextBlock) { nbrString = (b.Content as TextBlock).Text; }
            else {
                throw new Exception_UnexpectedDataTypeEncountered(("").GetType(), b.Content.GetType());
            }
            MainPage._currentlySelectedGymnastNbr = Int32.Parse(nbrString);

            Hide();

            GymnastSelectorGrid.Children.Clear();
            GymnastSelectorGrid.RowDefinitions.Clear();

            TextBlock debugTBk = new TextBlock();
            debugTBk.Text = "Currently selected gymnast: " + MainPage._currentlySelectedGymnastNbr.ToString();
            MainPage._publicStaticDebugSpace.Children.Add(debugTBk);
        }
    }
}
