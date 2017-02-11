using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace GymnasticsScoringTool {


    public static class HelperMethods {

        public static bool IsValid_UInt16(string inputString) {
            return !IsPoorlyFormatted_UInt16(inputString) && !IsOutsideValidRange_UInt16(inputString);
        }

        public static bool IsPoorlyFormatted_UInt16(string inputString) {
            int inputValue = -1;
            if (!int.TryParse(inputString, out inputValue)) { return true; }
            return false;
        }

        public static bool IsOutsideValidRange_UInt16(string inputString) {
            int inputValue = -1;
            if (int.TryParse(inputString, out inputValue)) {
                if (inputValue < ProgramConstants.MIN_INTEGER_PARAM) { return true; }
                if (inputValue > ProgramConstants.MAX_INTEGER_PARAM) { return true; }
            }
            return false;
        }

        public static bool IsValid_PositiveCappedDouble(string inputString) {
            return !IsPoorlyFormatted_PositiveCappedDouble(inputString) && !IsOutsideValidRange_PositiveCappedDouble(inputString);
        }

        public static bool IsPoorlyFormatted_PositiveCappedDouble(string inputString) {
            double inputValue = -1;
            if (!double.TryParse(inputString, out inputValue)) { return true; }
            return false;
        }

        public static bool IsOutsideValidRange_PositiveCappedDouble(string inputString) {
            double inputValue = -1;
            if (double.TryParse(inputString, out inputValue)) {
                if (inputValue < ProgramConstants.MIN_DECIMAL_PARAM) { return true; }
                if (inputValue > ProgramConstants.MAX_DECIMAL_PARAM) { return true; }
            }
            return false;
        }

        public static string FormatScore(double? score) {
            if(!score.HasValue) { return ProgramConstants.NULL_SCORE_STRING; }

            double roundedScore = Math.Round(score.Value, 3);
            string output = roundedScore.ToString();

            int dotSpot = output.IndexOf(".");
            if((dotSpot < 0) || (dotSpot >= output.Length)) {
                // there is no dot
                return output + ".0";
            }


            int afterIdx = (ProgramConstants.DIGITS_AFTER_DOT < (output.Length - (dotSpot + 1))
                ? ProgramConstants.DIGITS_AFTER_DOT : (output.Length - (dotSpot + 1)));
            output = output.Substring(0, dotSpot) + "." + output.Substring(dotSpot + 1, afterIdx);

            return output;
        }

        public static string FormatScore_Mono(double? score, bool teamFlag = false) {
            string s = FormatScore(score);
            int dotSpot = s.IndexOf(".");
            int afterDot = s.Length - (dotSpot + 1);

            string prefix = "";
            int prefixNeed = (teamFlag ? ProgramConstants.DIGITS_BEFORE_DOT_TEAM - dotSpot : ProgramConstants.DIGITS_BEFORE_DOT - dotSpot);
            while (prefix.Length < prefixNeed) {
                prefix += " ";
            }

            string suffix = "";
            int suffixNeed = ProgramConstants.DIGITS_AFTER_DOT - afterDot;
            while(suffix.Length < suffixNeed) {
                suffix += " ";
            }

            return prefix + s + suffix;
        }


        internal static string PlaceVisualTreeIntoOutputString(DependencyObject obj, int currentDepth) {
            int childrenThisLevel;
            if(obj is TextElement) {
                childrenThisLevel = 0;
            }
            else if (obj is RichTextBlock) {
                RichTextBlock rtb = obj as RichTextBlock;
                childrenThisLevel = rtb.Blocks.Count;
            }
            else if (obj is ScrollViewer) {
                if((obj as ScrollViewer).Content==null) {
                    childrenThisLevel = 0;
                }
                else {
                    childrenThisLevel = 1;
                }
            }
            else { childrenThisLevel = VisualTreeHelper.GetChildrenCount(obj); }
            

            StringBuilder currentListing = new StringBuilder();

            for (int offset = 0; offset < currentDepth; ++offset) { currentListing.Append("  "); }

            string objectName = "unresolved name";
            if(obj is FrameworkElement) {
                FrameworkElement fe = (obj as FrameworkElement);
                objectName = fe.Name;
            }
            else if(obj is TextElement) {
                TextElement te = (obj as TextElement);
                objectName = te.Name;
            }
            
            currentListing.Append("Level #" + currentDepth.ToString() + " >> " + objectName + " (" + obj.GetType().Name + " )" + " >> branches " + childrenThisLevel.ToString() + Environment.NewLine);

            if(obj is TextElement) {  } // do nothing;
            else if(obj is RichTextBlock) {
                for (int i = 0; i < childrenThisLevel; ++i) {
                    RichTextBlock rtb = obj as RichTextBlock; 
                    string childListing = PlaceVisualTreeIntoOutputString(rtb.Blocks.ElementAt(i), currentDepth + 1);
                    currentListing.Append(childListing);
                }
            }
            else if(obj is ScrollViewer) {
                if(((obj as ScrollViewer).Content != null) && ((obj as ScrollViewer).Content is DependencyObject)) {
                    string childListing = PlaceVisualTreeIntoOutputString(((obj as ScrollViewer).Content as DependencyObject), currentDepth + 1);
                }
            }
            else {
                for (int i = 0; i < childrenThisLevel; ++i) {
                    string childListing = PlaceVisualTreeIntoOutputString(VisualTreeHelper.GetChild(obj, i), currentDepth + 1);
                    currentListing.Append(childListing);
                }
            }

            return currentListing.ToString();
        }

        /// <summary>
        /// returns null if not matching dependency object is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static DependencyObject GetFirstVisualTreeElementWithName(DependencyObject obj, string name) {
            int childrenThisLevel = 0;

            if (obj is FrameworkElement) {
                FrameworkElement fe = (obj as FrameworkElement);
                if (fe.Name == name) { return obj; }

                if(fe is ScrollViewer) {
                    ScrollViewer sv = fe as ScrollViewer;
                    if(sv.Content==null) {
                        childrenThisLevel = 0;
                    }
                    else {
                        DependencyObject result = null;
                        if(sv.Content is DependencyObject) {
                            result = GetFirstVisualTreeElementWithName((sv.Content as DependencyObject), name);
                        }
                        if (result != null) { return result; }
                    }
                }

                if(fe is RichTextBlock) {
                    RichTextBlock rtb = fe as RichTextBlock;
                    childrenThisLevel = rtb.Blocks.Count;
                    for (int i = 0; i < childrenThisLevel; ++i) {
                        DependencyObject result = GetFirstVisualTreeElementWithName(rtb.Blocks.ElementAt(i), name);
                        if (result != null) { return result; }
                    }
                }

                else {
                    childrenThisLevel = VisualTreeHelper.GetChildrenCount(obj);

                    for (int i = 0; i < childrenThisLevel; ++i) {
                        DependencyObject result = GetFirstVisualTreeElementWithName(VisualTreeHelper.GetChild(obj, i), name);
                        if (result != null) { return result; }
                    }
                }
            }

            else if(obj is TextElement) {
                TextElement te = (obj as TextElement);
                if(te.Name == name) { return obj; }
                // should all be leaves with no further elements to parse through
            }

            return null;
        }


        // DependencyObject dObj = HelperMethods.GetFirstVisualTreeElementWithName(button, "RootGrid");
        // Grid rootGrid = dObj as Grid;

        internal static string ExploreVisualStateGroups_ForUIElement(FrameworkElement fe) {
            StringBuilder visualStateStringBuilder = new StringBuilder();

            var stateGroups = from vsg in VisualStateManager.GetVisualStateGroups(fe)
                               select vsg;

            int iVsg = 1;
            foreach(VisualStateGroup vsg in stateGroups) {
                visualStateStringBuilder.AppendLine("Visual state group #" + iVsg.ToString() + ": " + vsg.Name + " which contains " + vsg.States.Count.ToString() + " states");

                var states = from vs in vsg.States
                             select vs;

                int iVs = 1;
                foreach(VisualState vs in states) {
                    visualStateStringBuilder.AppendLine("  Visual state #" + iVs.ToString() + ": " + vs.Name + " which contains " + vs.Storyboard.Children.Count() 
                        + " storyboards, " + vs.Setters.Count.ToString() + "setters, and " + vs.StateTriggers.Count + " state triggers");

                    var storyboards = from s in vs.Storyboard.Children
                                      select s;

                    int iStoryboards = 1;
                    foreach(Timeline t in storyboards) {
                        visualStateStringBuilder.AppendLine("   Storyboard #" + iStoryboards.ToString() + ": Target name = " 
                            + Storyboard.GetTargetName(t) + ", target property = " + Storyboard.GetTargetProperty(t));
                        ++iStoryboards;
                    }

                    var setters = from s in vs.Setters
                                  select s;

                    int iSetters = 1;
                    foreach(Setter setter in setters) {
                        visualStateStringBuilder.AppendLine("    Setter #" + iSetters.ToString() + ": target object = " + setter.Target.Target.ToString()
                            + ", target path = " + setter.Target.Path.Path + ", target property = " + setter.Property.ToString() + ", value = " + setter.Value.ToString());
                        ++iSetters;
                    }

                    var stateTriggers = from s in vs.StateTriggers
                                        select s;

                    int iTriggers = 1;
                    foreach (StateTrigger trigger in stateTriggers) {
                        visualStateStringBuilder.AppendLine("    State trigger #" + iTriggers.ToString() + ": value is <not sure how to work through the metadata yet>");
                        ++iTriggers;
                    }

                    ++iVs;
                }


                ++iVsg;
            }

            if (stateGroups.Count() == 0) {
                visualStateStringBuilder.AppendLine("Framework Element <name: " + fe.Name + "> found; no visual state groups");
            }

            visualStateStringBuilder.AppendLine("Framework Element <name: " + fe.Name + "> appears to have: " + VisualTreeHelper.GetChildrenCount(fe).ToString() + " immediate children in the visual tree");

            return visualStateStringBuilder.ToString();
        }


        internal static void RemoveUpdateCancelButtons_FromContentDialogControlTemplate(ContentDialog cd) {
            DependencyObject dObj = HelperMethods.GetFirstVisualTreeElementWithName(cd, "NewButtonGrid");
            Grid g = dObj as Grid;
            if (g == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new Grid()).GetType(), dObj.GetType());
            }

            dObj = HelperMethods.GetFirstVisualTreeElementWithName(cd, "ButtonCancel");
            Button buttonCancel = dObj as Button;
            if (buttonCancel == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new Button()).GetType(), dObj.GetType());
            }
            g.Children.Remove(buttonCancel);

            dObj = HelperMethods.GetFirstVisualTreeElementWithName(cd, "ButtonUpdate");
            Button buttonUpdate = dObj as Button;
            if (buttonUpdate == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new Button()).GetType(), dObj.GetType());
            }
            g.Children.Remove(buttonUpdate);
        }

        internal static void RemoveCancelButton_ChangeUpdateToVerify_FromContentDialogControlTemplate(ContentDialog cd) {
            DependencyObject dObj = HelperMethods.GetFirstVisualTreeElementWithName(cd, "NewButtonGrid");
            Grid g = dObj as Grid;
            if (g == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new Grid()).GetType(), dObj.GetType());
            }

            dObj = HelperMethods.GetFirstVisualTreeElementWithName(cd, "ButtonCancel");
            Button buttonCancel = dObj as Button;
            if (buttonCancel == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new Button()).GetType(), dObj.GetType());
            }
            g.Children.Remove(buttonCancel);

            dObj = HelperMethods.GetFirstVisualTreeElementWithName(cd, "ButtonUpdate");
            Button buttonUpdate = dObj as Button;
            if (buttonUpdate == null) {
                throw new Exception_UnexpectedDataTypeEncountered((new Button()).GetType(), dObj.GetType());
            }

            buttonUpdate.Content = "VERIFY";
            Grid.SetColumn(buttonUpdate, 2);
        }


        internal static string GrabButtonLabelString(Button button) {
            string buttonLabel = "";
            if (button.Content is TextBlock) { buttonLabel += (button.Content as TextBlock).Text; }
            else { buttonLabel += button.Content.ToString(); }

            return buttonLabel;
        }

        internal static string GrabRadioButtonLabelString(RadioButton rb) {
            string buttonLabel = "";
            if (rb.Content is TextBlock) { buttonLabel += (rb.Content as TextBlock).Text; }
            else { buttonLabel += rb.Content.ToString(); }

            return buttonLabel;
        }

        internal static string GrabCheckBoxLabelString(CheckBox cb) {
            string buttonLabel = "";
            if (cb.Content is TextBlock) { buttonLabel += (cb.Content as TextBlock).Text; }
            else { buttonLabel += cb.Content.ToString(); }

            return buttonLabel;
        }

        internal static void RemoveAllButHeaderRowFromGrid(Grid grid) {
            // Why do I have to set the elements below to be in descending order?
            // Because if we  delete items from the editGymnastGrid.Children in the same order
            // in which they were placed in that collection, then we will skip half the items
            // that we want to delete... Still kind of a kludge answer at this point

            var elementsToDelete = from FrameworkElement fe in grid.Children
                                   where Grid.GetRow(fe) > 0
                                   orderby Grid.GetColumn(fe) descending
                                   orderby Grid.GetRow(fe) descending
                                   select fe;


            foreach (FrameworkElement del in elementsToDelete) {
                grid.Children.Remove(del);
            }

            int rows = grid.RowDefinitions.Count();
            int idx = 1;
            while (idx < rows) {
                RowDefinition rowDefToDelete = grid.RowDefinitions.Last();
                grid.RowDefinitions.Remove(rowDefToDelete);
                ++idx;
            }
        }

        // Intended to take a vertically oriented set of stacked grids and make them all align
        internal static void AlignColumnWidthsAcrossSetOfGrids(List<Grid> gridList) {
            if(gridList.Count == 0) { return; }

            // first check to make sure all the grids have the same number of columns
            int cols = gridList.ElementAt(0).ColumnDefinitions.Count;
            foreach(Grid g in gridList) {
                if(g.ColumnDefinitions.Count != cols) {
                    return;
                }
            }

            List<double> columnWidths = new List<double>();
            foreach(ColumnDefinition colDef in gridList.ElementAt(0).ColumnDefinitions) {
                columnWidths.Add(colDef.ActualWidth);
            }

            foreach(Grid g in gridList) {
                int idx = 0;
                foreach (ColumnDefinition colDef in g.ColumnDefinitions) {
                    colDef.MinWidth = columnWidths.ElementAt(idx);
                    colDef.MaxWidth = columnWidths.ElementAt(idx);
                    ++idx;
                }

                g.UpdateLayout();
            }
             
        }

        public static Tuple<int, string> findDifference(string s1, string s2) {
            int idx = 0;
            while ((idx < s1.Length) && (idx < s2.Length) && (s1[idx] == s2[idx])) {
                ++idx;
            }

            return new Tuple<int, string>(idx, s1.Substring(idx));
        }

    }
}
