using Application.Common;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ManageStock.Views.Currency
{
    /// <summary>
    /// Interaction logic for CurrencyWindow.xaml
    /// </summary>
    public partial class CurrencyNamePopup : CustomWindow, INotifyPropertyChanged
    {
        private string deviseName;

        public CurrencyNamePopup()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string DeviseName
        {
            get => deviseName;
            set
            {
                deviseName = value;
                OnPropertyChanged();
            }
        }


        private void CancelAddCurrencyButtonOnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void CurrencyWindowOnClosing(object sender, CancelEventArgs e)
        {
            DialogResult = false;
        }

        private void AddNewCurrencyButtonOnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
