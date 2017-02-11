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
    public sealed partial class ContentDialog_TeamSelector : ContentDialog {

        private bool _updateCancelRemoved = false;

        public ContentDialog_TeamSelector() {
            this.InitializeComponent();
            Loaded += SetupTeamButtons;
        }

        private void SetupTeamButtons(object sender, RoutedEventArgs args) {

            if(Meet.ProvideTeamStringList().Count == 0) {
                Hide(); // no teams, so just shut this box instead of having someone stuck here with no team to select
            }

            foreach (string tString in Meet.ProvideTeamStringList()) {

                object o;
                bool resourceKeyFound = App.Current.Resources.TryGetValue("ButtonLC", out o);
                ControlTemplate template = o as ControlTemplate;

                Button b = new Button();
                if (template != null) { b.Template = template; }
                b.Content = tString;
                b.Click += TeamSelection;
                TeamSelectorStackPanel.Children.Add(b);

                b.Margin = new Thickness(20, 5, 10, 5);
                b.HorizontalAlignment = HorizontalAlignment.Stretch;
                b.FontWeight = Windows.UI.Text.FontWeights.Bold;
            }

            if(_updateCancelRemoved) { } // don't need to remove the buttons... they're already gone
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

        private void TeamSelection(object sender, RoutedEventArgs args) {
            Button b = sender as Button;
            if(b==null) {
                throw new Exception_UnexpectedDataTypeEncountered((new Button()).GetType(), sender.GetType());
            }

            string teamName;
            if (b.Content is string) { teamName = b.Content as String; }
            else if (b.Content is TextBlock) { teamName = (b.Content as TextBlock).Text; }
            else {
                throw new Exception_UnexpectedDataTypeEncountered(("").GetType(), b.Content.GetType());
            }
            MainPage._currentlySelectedTeam = Meet.GetTeamWithName(teamName);

            Hide();

            TeamSelectorStackPanel.Children.Clear();
        }
    }
}
