using Application.CommandManager.Collection;
using Application.Common.Commands;
using Application.Common.Managers;
using Application.Common.Managers.DatabaseManagerBase;
using Application.Common.Models;
using Application.Common.Models.Items;
using Application.Common.Models.Providers;
using Application.Common.Notifications;
using Application.Common.PopupWindows;
using Application.Common.ViewModels;
using ManageStock.Builder;
using ManageStock.Views.Providers;
using System;
using System.Windows.Input;

namespace ManageStock.ViewModels
{
    public class ProviderViewModel : ViewModelBase
    {
        private Provider m_SelectedProvider;

        public ProviderViewModel(Application.CommandManager.CommandManager _CommandManager) : base(_CommandManager)
        {
            DeleteProviderCommand = new RelayCommand(_ => DeleteProvider(), _ => !ManageStockBuilder.IsLocked && SelectedProvider != null);
            EditProviderCommand = new RelayCommand(_ => EditProvider(SelectedProvider, false), _ => !ManageStockBuilder.IsLocked && SelectedProvider != null);
            AddProviderCommand = new RelayCommand(_ => EditProvider(SelectedProvider, true), _ => !ManageStockBuilder.IsLocked);

            Providers = new ObservableTrackableCollection<Provider>();
            Header = "Fournisseurs";
            TemplateName = "provider";
        }

        public ICommand EditProviderCommand { get; set; }
        public ICommand AddProviderCommand { get; set; }
        public ICommand DeleteProviderCommand { get; set; }

        public ObservableTrackableCollection<Provider> Providers { get; set; }

        public override Type ModelType => typeof(Provider);

        public override void RequestView(IDatabaseModel _Item)
        {
            SelectedProvider = _Item as Provider;
        }

        public override void Reload()
        {

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

        public override void SelectItem(ItemBase itemBase)
        {
            SelectedProvider = itemBase as Provider;
        }

        public void EditProvider(Provider _Provider)
        {
            EditProvider(_Provider, false);
        }

        private void EditProvider(Provider _Provider, bool _Add = true)
        {
            CommandManager.Suspend();
            Provider provider = _Provider;
            if (_Add)
            {
                provider = new Provider();
            }

            if (provider == null)
            {
                throw new NullReferenceException("Provider is null");
            }

            EditProviderWindow window = new EditProviderWindow(provider)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };
            bool? result = window.ShowDialog();

            if (result == true)
            {
                if (_Add)
                {
                    provider.CopyTo(window.Provider);
                    if (DataManager.Execute(EnumDatabaseAction.Insert, provider) == true)
                    {
                        Providers.Add(provider);
                    }
                }
                else
                {
                    CommandManager.Resume();
                    CommandManager.BeginGroup();
                    provider.CopyTo(window.Provider);
                    CommandManager.EndGroup();
                    DataManager.Execute(EnumDatabaseAction.Update, provider);
                }

            }
            CommandManager.Resume();

            SelectedProvider = provider;
        }

        private void DeleteProvider()
        {
            ConfirmationPopup popup = new ConfirmationPopup($"Êtes-vous sûr de vouloir supprimer le fournisseur '{SelectedProvider.Name}' ?", "Confirmation")
            {
                ShowCancelButton = false,
                Owner = System.Windows.Application.Current.MainWindow
            };
            popup.ShowDialog();

            if (popup.Result == EnumPopupResult.Yes)
            {
                if (DataManager.Execute(EnumDatabaseAction.Delete, SelectedProvider) == true)
                {
                    Providers.Remove(SelectedProvider);
                }
            }
        }

        public override void Initialize(CustomNotificationsManager _NotificationManager)
        {
            base.Initialize(_NotificationManager);

            Providers.Clear();

            foreach (Provider provider in DataManager.ProviderManager.FetchAll())
            {
                Providers.Add(provider);
            }

            Providers.InitializeTrackable(CommandManager);
        }

        public override void Dispose()
        {
        }
    }
}
