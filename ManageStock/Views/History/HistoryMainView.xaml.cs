using ManageStock.ViewModels;
using System.Windows.Controls;

namespace ManageStock.Views.History
{
    /// <summary>
    /// Interaction logic for HistoryMainView.xaml
    /// </summary>
    public partial class HistoryMainView : UserControl
    {
        public HistoryMainView()
        {
            InitializeComponent();
        }

        public HistoryViewModel ViewModel => DataContext as HistoryViewModel;
    }
}
