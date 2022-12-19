using Application.Common;
using Application.Common.Commands;
using Application.Common.Managers;
using Application.Common.Models.Groups;
using Application.Common.Models.Providers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace ManageStock.Views.Articles.Providers
{
    /// <summary>
    /// Interaction logic for AddArticleProviderWindow.xaml
    /// </summary>
    public partial class AddArticleProviderWindow : CustomWindow, INotifyPropertyChanged
    {
        private Provider m_SelectedProvider;
        private bool m_ManualClosing;
        private Application.Common.Models.Devises.Currency m_SelectedCurrency;

        public AddArticleProviderWindow(Guid _Id)
        {
            ConfirmButtonCommand = new RelayCommand(_ => AddGroupProvider(), _ => SelectedProvider != null);

            m_ManualClosing = false;
            Currencies = DataManager.CurrencyManager.FetchAll();
            Providers = DataManager.ProviderManager.FetchAll();
            SelectedCurrency = Currencies.FirstOrDefault();
            GroupProvider = new GroupProvider { ID = _Id };

            InitializeComponent();

            DataContext = this;
        }

        public List<Application.Common.Models.Devises.Currency> Currencies { get; set; }

        public ICommand ConfirmButtonCommand { get; set; }

        public GroupProvider GroupProvider { get; set; }

        public Application.Common.Models.Devises.Currency SelectedCurrency
        {
            get => m_SelectedCurrency;
            set
            {
                m_SelectedCurrency = value;
                OnPropertyChanged();
            }
        }

        public Provider SelectedProvider
        {
            get => m_SelectedProvider;
            set
            {
                m_SelectedProvider = value;
                OnPropertyChanged();
            }
        }

        public List<Provider> Providers { get; set; }

        private void CancelButtonOnClick(object sender, RoutedEventArgs e)
        {
            m_ManualClosing = true;
            DialogResult = false;
        }

        private void AddGroupProvider()
        {
            m_ManualClosing = true;
            GroupProvider.Currency = SelectedCurrency;
            GroupProvider.Item = SelectedProvider;
            GroupProvider.IDProvider = SelectedProvider.ID;
            DialogResult = true;
        }

        private void AddArticleProviderWindowOnClosing(object sender, CancelEventArgs e)
        {
            if (!m_ManualClosing)
            {
                DialogResult = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
