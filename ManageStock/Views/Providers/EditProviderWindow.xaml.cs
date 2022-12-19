using Application.Common;
using Application.Common.Models.Providers;
using System.ComponentModel;
using System.Windows;

namespace ManageStock.Views.Providers
{
    /// <summary>
    /// Interaction logic for EditProviderWindow.xaml
    /// </summary>
    public partial class EditProviderWindow : CustomWindow
    {
        private Provider m_SaveProvider;
        private bool m_ManualClosing = false;

        public EditProviderWindow(Provider _Provider)
        {
            m_SaveProvider = _Provider;
            Provider = (Provider)_Provider.Clone();

            InitializeComponent();

            DataContext = this;
        }

        public Provider Provider { get; set; }

        private void CancelButtonOnClick(object sender, RoutedEventArgs e)
        {
            m_ManualClosing = true;
            DialogResult = false;
        }

        private void ConfirmButtonOnClick(object sender, RoutedEventArgs e)
        {
            m_ManualClosing = true;
            DialogResult = true;
        }

        private void EditProviderWindowOnClosing(object sender, CancelEventArgs e)
        {
            if (!m_ManualClosing)
            {
                Provider.CopyTo(m_SaveProvider);
                DialogResult = false;
            }
        }
    }
}
