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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GymnasticsScoringTool_01 {
    /// <summary>
    /// Page content to send to the printer
    /// </summary>
    public sealed partial class Sample_PageToPrint : Page {
        public RichTextBlock TextContentBlock { get; set; }

        public Sample_PageToPrint() {
            this.InitializeComponent();
            TextContentBlock = TextContent;
        }
    }
}
