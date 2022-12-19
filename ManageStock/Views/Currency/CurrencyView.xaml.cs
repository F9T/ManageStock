using ManageStock.ViewModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace ManageStock.Views.Currency
{
    /// <summary>
    /// Interaction logic for CurrencyView.xaml
    /// </summary>
    public partial class CurrencyView : UserControl
    {
        public CurrencyView()
        {
            InitializeComponent();
        }

        public CurrencyViewModel ViewModel => (CurrencyViewModel)DataContext;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
