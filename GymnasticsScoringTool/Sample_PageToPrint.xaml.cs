using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GymnasticsScoringTool {
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
