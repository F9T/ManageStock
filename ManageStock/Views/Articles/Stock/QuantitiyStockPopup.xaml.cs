using Application.Common;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace ManageStock.Views.Articles.Stock
{
    /// <summary>
    /// Interaction logic for QuantitiyStockPopup.xaml
    /// </summary>
    public partial class QuantitiyStockPopup : CustomWindow, INotifyPropertyChanged
    {
        private double quantity;

        public QuantitiyStockPopup(string _Message)
        {
            Message = _Message;
            Quantity = 0;

            InitializeComponent();

            DataContext = this;
        }

        public string Message { get; set; }

        public double Quantity
        {
            get => quantity;
            set
            {
                quantity = value;
                OnPropertyChanged();
            }
        }

        private void CancelButtonOnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void ConfirmButtonOnClick(object sender, RoutedEventArgs e)
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
