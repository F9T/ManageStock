using Application.Common.Models.Providers;
using ManageStock.Builder;
using ManageStock.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ManageStock.Views.Providers
{
    /// <summary>
    /// Interaction logic for ProviderMainView.xaml
    /// </summary>
    public partial class ProviderMainView : UserControl
    {
        public ProviderMainView()
        {
            InitializeComponent();
        }

        public ProviderViewModel ViewModel => (ProviderViewModel)DataContext;

        private void DataGridRowOnDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ManageStockBuilder.IsLocked) 
            { 
                return; 
            }

            FrameworkElement element = sender as FrameworkElement;
            if (element != null)
            {
                if (element.Tag is Provider provider)
                {
                    ViewModel.EditProvider(provider);
                }
            }
        }
    }
}
