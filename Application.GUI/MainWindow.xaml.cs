using Application.Common;
using System.Windows.Controls;

namespace Application.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : CustomWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public UserControl View { get; set; }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
