using Application.Common;
using Application.Common.Commands;
using Application.Common.Helpers;
using Application.Common.Managers;
using Application.Common.Managers.DatabaseManagerBase;
using Application.Common.Models;
using Application.Common.Models.Articles;
using Application.Common.Models.History;
using Application.Common.Models.Items;
using Application.Common.Notifications;
using Application.Common.PopupWindows;
using Application.Common.ViewModels;
using ManageStock.Builder;
using ManageStock.Utilities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace ManageStock.ViewModels
{
    public class HistoryViewModel : ViewModelBase
    {
        private Article m_SelectedArticle;
        private EnumFilteredHistory m_SelectedFilter;
        private DateTime m_SelectedFilterEndDate;
        private DateTime m_SelectedFilterStartDate;
        private ICollectionView m_HistoryCollection;
        private EnumStockAction m_SelectedStockActionType;

        public HistoryViewModel(Application.CommandManager.CommandManager _CommandManager) : base(_CommandManager)
        {
            Header = "Historique";
            TemplateName = "history";

            GoToArticleCommand = new RelayCommand(_ => GoToArticle(), _ => SelectedArticle != null);
            ClearHistoryCommand = new RelayCommand(_ => ClearHistory(), _ => !ManageStockBuilder.IsLocked && SelectedArticle != null);
            ExportHistoryCommand = new RelayCommand(_ => ExportHistory(), _ => SelectedArticle != null);

            m_SelectedFilterStartDate = DateTime.Today.AddMonths(-6);
            m_SelectedFilterEndDate = DateTime.Today;

            Articles = new ObservableCollection<Article>();
            Filtered = new List<EnumFilteredHistory>
            {
                EnumFilteredHistory.None,
                EnumFilteredHistory.Date,
                EnumFilteredHistory.StockAction
            };

        }

        public ICommand ClearHistoryCommand { get; set; }

        public ICommand GoToArticleCommand { get; set; }

        public ICommand ExportHistoryCommand { get; set; }

        public ICollectionView HistoryCollection
        {
            get
            {
                return m_HistoryCollection;
            }
            set
            {
                m_HistoryCollection = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Article> Articles { get; set; }

        public List<EnumFilteredHistory> Filtered { get; set; }

        public EnumStockAction SelectedStockActionType
        {
            get { return m_SelectedStockActionType; }
            set
            {
                m_SelectedStockActionType = value;
                OnPropertyChanged();
                RefreshCollection();
            }
        }

        public DateTime SelectedFilterStartDate
        {
            get { return m_SelectedFilterStartDate; }
            set
            {
                m_SelectedFilterStartDate = value;
                OnPropertyChanged();
                RefreshCollection();
            }
        }

        public DateTime SelectedFilterEndDate
        {
            get { return m_SelectedFilterEndDate; }
            set
            {
                m_SelectedFilterEndDate = value;
                OnPropertyChanged();
                RefreshCollection();
            }
        }

        public EnumFilteredHistory SelectedFilter
        {
            get { return m_SelectedFilter; }
            set
            {
                m_SelectedFilter = value;
                OnPropertyChanged();
                RefreshCollection();
            }
        }

        public override Type ModelType => typeof(History); 
        
        public override void RequestView(IDatabaseModel _Item)
        {
            SelectedArticle = _Item as Article;
        }

        public Article SelectedArticle
        {
            get => m_SelectedArticle;
            set
            {
                m_SelectedArticle = value;
                if(m_SelectedArticle != null)
                {
                    HistoryCollection = CollectionViewSource.GetDefaultView(m_SelectedArticle.History);
                    HistoryCollection.SortDescriptions.Add(new SortDescription(nameof(History.Date), ListSortDirection.Descending));
                    HistoryCollection.Filter = _Item => HistoryFilter(_Item);
                }
                OnPropertyChanged();
            }
        }

        private bool HistoryFilter(object _History)
        {
            if(_History is History history)
            {
                switch (SelectedFilter)
                {
                    case EnumFilteredHistory.None:
                        return true;
                    case EnumFilteredHistory.Date:
                        return history.Date.IsDateInRange(SelectedFilterStartDate, SelectedFilterEndDate);
                    case EnumFilteredHistory.StockAction:
                        return history.ActionType == SelectedStockActionType;
                }
            }

            return true;
        }

        private void RefreshCollection()
        {
            HistoryCollection.Refresh();
        }

        public override void Initialize(CustomNotificationsManager _NotificationManager)
        {
            base.Initialize(_NotificationManager);

            Articles.Clear();

            foreach (var article in DataManager.ArticleManager.FetchAll())
            {
                Articles.Add(article);
            }
        }

        private void GoToArticle()
        {
            OnChangeViewRequest(new ViewRequestEventArgs(SelectedArticle, typeof(ArticleViewModel)));
        }
        public override void Reload()
        {

        }

        private void ExportHistory()
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "Csv Files (*.csv)|*.csv"
            };
            if (dialog.ShowDialog() == true)
            {
                bool result = Exporter.InstanceOf.ExportArticleHistoryToExcel(dialog.FileName, SelectedArticle);

                if (result)
                {
                    NotifySucess($"L'historique de l'article {SelectedArticle} a été exportée.");
                }
                else
                {
                    NotifyError($"Une erreur est survenue à l'exportation de l'historique de l'article {SelectedArticle}.");
                }
            }
        }

        private void ClearHistory()
        {
            ConfirmationPopup popup = new ConfirmationPopup("Êtes-vous sûr de vouloir supprimer tout l'historique de cet article ?", "Suppression ?")
            {
                Owner = System.Windows.Application.Current.MainWindow
            };
            if (popup.ShowDialog() == true && popup.Result == EnumPopupResult.Yes)
            {

                m_NotificationManager.SuspendNotification();

                CommandManager.BeginGroup();

                bool result = DataManager.HistoryManager.DeleteHistoryArticle(SelectedArticle);

                if (result)
                {
                    for (int i = 0; i < SelectedArticle.History.Count;)
                    {
                        SelectedArticle.History.RemoveAt(i);
                    }
                    NotifySucess("L'historique a été supprimé avec succès.");
                }

                CommandManager.EndGroup();
                m_NotificationManager.ResumeNotification();
            }
        }

        public override void SelectItem(ItemBase itemBase)
        {
            SelectedArticle = itemBase as Article;
        }

        public override void Dispose()
        {
        }

        public enum EnumFilteredHistory
        {
            None,
            Date,
            StockAction
        }
    }
}
