using Application.Common.Commands;
using Application.Common.Notifications;
using Application.Common.PopupWindows;
using Application.Common.ViewModels;
using Application.Common.Managers;
using Application.Common.Models.Devises;
using ManageStock.Views.Currency;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Application.Common.Models.Items;
using Application.Common.Managers.DatabaseManagerBase;
using System;
using Application.Common.Models;
using ManageStock.Builder;

namespace ManageStock.ViewModels
{
    public class CurrencyViewModel : ViewModelBase
    {
        private Currency selectedCurrency;

        public CurrencyViewModel(Application.CommandManager.CommandManager _CommandManager) : base(_CommandManager)
        {
            AddCurrencyCommand = new RelayCommand(_ => AddCurrency(), _ => !ManageStockBuilder.IsLocked);
            DeleteCurrencyCommand = new RelayCommand(_ => DeleteCurrency(), _ => !ManageStockBuilder.IsLocked && SelectedCurrency != null);

            Currencies = new ObservableCollection<Currency>();

            Header = "Devises";
            TemplateName = "currency";
        }

        public ObservableCollection<Currency> Currencies { get; set; }

        public override Type ModelType => typeof(Currency);

        public override void RequestView(IDatabaseModel _Item)
        {
            SelectedCurrency = _Item as Currency;
        }

        public Currency SelectedCurrency
        {
            get => selectedCurrency;
            set
            {
                selectedCurrency = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddCurrencyCommand { get; set; }

        public ICommand DeleteCurrencyCommand { get; set; }

        public override void Initialize(CustomNotificationsManager _NotificationManager)
        {
            base.Initialize(_NotificationManager);

            SelectedCurrency = null;
            Currencies.Clear();

            foreach (var article in DataManager.CurrencyManager.FetchAll())
            {
                Currencies.Add(article);
            }
        }
        public override void Reload()
        {

        }

        public override void SelectItem(ItemBase itemBase)
        {

        }

        private void AddCurrency()
        {
            CurrencyNamePopup popup = new CurrencyNamePopup
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            if (popup.ShowDialog() == true)
            {
                Currency newCurrency = new Currency
                {
                    Name = popup.DeviseName
                };

                if (DataManager.Execute(EnumDatabaseAction.Insert, newCurrency) == true)
                {
                    Currencies.Add(newCurrency);
                    SelectedCurrency = newCurrency;
                }
            }
        }

        private void DeleteCurrency()
        {
            ConfirmationPopup popup =
                new ConfirmationPopup($"Êtes-vous sûr de vouloir supprimer la devise '{SelectedCurrency.Name}' ?",
                    "Confirmation")
                {
                    ShowCancelButton = false,
                    Owner = System.Windows.Application.Current.MainWindow
                };
            popup.ShowDialog();

            if (popup.Result == EnumPopupResult.Yes)
            {
                if (DataManager.Execute(EnumDatabaseAction.Delete, SelectedCurrency) == true)
                {
                    Currencies.Remove(SelectedCurrency);
                    SelectedCurrency = null;
                }
            }
        }

        public override void Dispose()
        {
        }
    }
}
