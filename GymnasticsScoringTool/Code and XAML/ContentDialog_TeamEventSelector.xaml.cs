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
    public sealed partial class ContentDialog_TeamEventSelector : ContentDialog {

        private bool _updateCancelRemoved = false;

        public ContentDialog_TeamEventSelector() {
            this.InitializeComponent();
            Loaded += SetupCheckBoxes;
        }

        private void SetupCheckBoxes(object sender, RoutedEventArgs e) {
             if (Meet.ProvideTeamStringList().Count == 0) {
                Hide(); // no teams, so just shut this box instead of having someone stuck here with no team to select
            }

            foreach (string tString in Meet.ProvideTeamStringList()) {

                CheckBox cb = new CheckBox();
                cb.Content = tString;
                cb.Checked += Cb_TeamSelected;
                cb.Unchecked += Cb_TeamUnselected;
                StackPanel_TeamSelector.Children.Add(cb);

                cb.Margin = new Thickness(10, 5, 10, 5);
                cb.HorizontalAlignment = HorizontalAlignment.Stretch;
                cb.FontWeight = Windows.UI.Text.FontWeights.Bold;
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
            border.MaxWidth = 600;
            border.MinWidth = 500;

            this.MaxHeight = 500;
            this.MinHeight = 300;
            this.MaxWidth = 600;
            this.MinWidth = 500;
            this.Height = 450; // the desired value
            this.Width = 500; // the desired value
        }

        private void Cb_TeamSelected(object sender, RoutedEventArgs e) {
            CheckBox cb = sender as CheckBox;
            if (cb == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new RadioButton()).GetType(), sender.GetType());
            }

            string teamSelection = HelperMethods.GrabCheckBoxLabelString(cb);
            if(teamSelection != null) {
                Team team = Meet.GetTeamWithName(teamSelection);
                if(team!=null) {
                    if(MainPage._currentlySelectedTeamsList == null) {
                        MainPage._currentlySelectedTeamsList = new List<Team>();
                    }
                    MainPage._currentlySelectedTeamsList.Add(team);
                }
                else {
                    throw new Exception_TeamNotRegistered(teamSelection);
                }
            }
            else {
                throw new Exception("Could not identify team name from Check Box input");
            }
        }


        private void Cb_TeamUnselected(object sender, RoutedEventArgs e) {
            CheckBox cb = sender as CheckBox;
            if (cb == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new RadioButton()).GetType(), sender.GetType());
            }

            string teamSelection = HelperMethods.GrabCheckBoxLabelString(cb);
            if (teamSelection != null) {
                Team team = Meet.GetTeamWithName(teamSelection);
                if (team != null) {
                    MainPage._currentlySelectedTeamsList.Remove(team);
                }
                else {
                    throw new Exception_TeamNotRegistered(teamSelection);
                }
            }
            else {
                throw new Exception("Could not identify team name from Check Box input");
            }
        }

        private void Button_EventSelected(object sender, RoutedEventArgs e) {
            Button btn = sender as Button;
            if (btn == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new Button()).GetType(), sender.GetType());
            }

            string eventSelection = HelperMethods.GrabButtonLabelString(btn);
            if (eventSelection != null) {
                switch(eventSelection.ToUpper()) {
                case "VAULT":
                case "BARS":
                case "BEAM":
                case "FLOOR":
                    MainPage._currentlySelectedEvent = eventSelection.ToUpper();
                    break;
                default:
                    throw new Exception_UnkownEvent(eventSelection.ToUpper());
                }
            }
            else {
                throw new Exception("Could not identify event name from Button input");
            }

            if ((MainPage._currentlySelectedTeamsList != null) && (MainPage._currentlySelectedTeamsList.Count > 0)) {
                Hide();
                ResetForNextUse();
            }
        }

        private void ResetForNextUse() {
            /* No clean-up necessary on event side any more since we switched from 
             * RadioButtons (which would need to be unchecked) to Buttons
             * 
            DependencyObject dObj = HelperMethods.GetFirstVisualTreeElementWithName(this, "StackPanel_EventSelector");
            if(dObj == null) {
                throw new Exception_CouldNotFindUIElement("StackPanel_EventSelector", (new StackPanel()).GetType(), this.GetType());
            }

            StackPanel rb_stackPanel = dObj as StackPanel;
            if(rb_stackPanel==null) {
                throw new Exception_UnexpectedDataTypeEncountered((new StackPanel()).GetType(), dObj.GetType());
            }

            foreach(FrameworkElement fe in rb_stackPanel.Children) {
                RadioButton rb = fe as RadioButton;
                if(rb==null) { throw new Exception_UnexpectedDataTypeEncountered((new RadioButton()).GetType(), fe.GetType()); }
                rb.IsChecked = false;
            }
            */

            DependencyObject dObj = HelperMethods.GetFirstVisualTreeElementWithName(this, "StackPanel_TeamSelector");
            if (dObj == null) {
                throw new Exception_CouldNotFindUIElement("StackPanel_TeamSelector", (new StackPanel()).GetType(), this.GetType());
            }

            StackPanel team_stackPanel = dObj as StackPanel;
            if (team_stackPanel == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new StackPanel()).GetType(), dObj.GetType());
            }

            team_stackPanel.Children.Clear();
        }
    }
}
