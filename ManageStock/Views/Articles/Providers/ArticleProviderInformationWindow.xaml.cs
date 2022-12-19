using Application.Common;
using Application.Common.PopupWindows;
using Application.Common.Managers;
using Application.Common.Models.Groups;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using Application.Common.Managers.DatabaseManagerBase;

namespace ManageStock.Views.Articles.Providers
{
    /// <summary>
    /// Interaction logic for ArticleProviderInformationWindow.xaml
    /// </summary>
    public partial class ArticleProviderInformationWindow : CustomWindow
    {
        private bool m_ManualClosing;
        private GroupProvider m_SaveGroupProvider;

        public ArticleProviderInformationWindow(GroupProvider groupProvider)
        {
            m_ManualClosing = false;
            m_SaveGroupProvider = (GroupProvider)groupProvider.Clone();
            GroupProvider = groupProvider;
            Currencies = DataManager.CurrencyManager.FetchAll();
            InitializeComponent();

            DataContext = this;
        }

        public List<Application.Common.Models.Devises.Currency> Currencies { get; set; }

        public GroupProvider GroupProvider { get; set; }

        private void CancelButtonOnClick(object sender, RoutedEventArgs e)
        {
            GroupProvider.CopyTo(m_SaveGroupProvider);
            m_ManualClosing = true;
            DialogResult = false;
        }

        private void ConfirmButtonOnClick(object sender, RoutedEventArgs e)
        {
            m_ManualClosing = true;
            DialogResult = true;
        }

        private void ArticleProviderInformationWindowOnClosing(object sender, CancelEventArgs e)
        {
            if (!m_ManualClosing)
            {
                GroupProvider.CopyTo(m_SaveGroupProvider);
                DialogResult = false;
            }
        }

        private void DeleteProviderButtonOnClick(object sender, RoutedEventArgs e)
        {
            ConfirmationPopup popup = new ConfirmationPopup($"Êtes-vous sûr de vouloir supprimer le fournisseur '{GroupProvider.Item.Name}' de cet article ?", "Confirmation")
            {
                ShowCancelButton = false,
                Owner = System.Windows.Application.Current.MainWindow
            };
            popup.ShowDialog();

            if (popup.Result == EnumPopupResult.Yes)
            {
                if (DataManager.Execute(EnumDatabaseAction.Delete, GroupProvider) == true)
                {
                    DialogResult = true;
                }
            }
        }
    }
}
