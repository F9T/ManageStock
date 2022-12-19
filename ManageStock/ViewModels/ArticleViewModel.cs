using Application.CommandManager;
using Application.CommandManager.Collection;
using Application.Common;
using Application.Common.Commands;
using Application.Common.Managers;
using Application.Common.Managers.DatabaseManagerBase;
using Application.Common.Models;
using Application.Common.Models.Articles;
using Application.Common.Models.Groups;
using Application.Common.Models.History;
using Application.Common.Models.Items;
using Application.Common.Navigator;
using Application.Common.Notifications;
using Application.Common.PopupWindows;
using Application.Common.ViewModels;
using Application.Common.Views;
using Application.Excel;
using ManageStock.Builder;
using ManageStock.Export;
using ManageStock.Views.Articles.Providers;
using ManageStock.Views.Articles.Stock;
using ManageStock.Views.Articles.SubArticles;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace ManageStock.ViewModels
{
    public class ArticleViewModel : ViewModelBase
    {
        private string m_SearchText;
        private Article m_SelectedArticle;
        private GroupArticle m_SelectedSubArticle;
        private Dictionary<Article, List<History>> m_AddedHistories;

        public ArticleViewModel(Application.CommandManager.CommandManager _CommandManager) : base(_CommandManager)
        {
            GoToHistoryCommand = new RelayCommand(_ => GoToHistory(), _ => SelectedArticle != null);

            ProductionArticleCommand = new RelayCommand(_ => StockAction(EnumStockAction.Production), _ => !ManageStockBuilder.IsLocked && SelectedArticle != null);
            ResupplyArticleCommand = new RelayCommand(_ => StockAction(EnumStockAction.Resupply), _ => !ManageStockBuilder.IsLocked && SelectedArticle != null);
            InputArticleCommand = new RelayCommand(_ => StockAction(EnumStockAction.Input), _ => !ManageStockBuilder.IsLocked && SelectedArticle != null);
            OuputArticleCommand = new RelayCommand(_ => StockAction(EnumStockAction.Output), _ => !ManageStockBuilder.IsLocked && SelectedArticle != null);

            AddArticleCommand = new RelayCommand(_ => AddArticle(), _ => !ManageStockBuilder.IsLocked);

            DeleteSubArticleCommand = new RelayCommand(_ => DeleteSubArticle(), _ => !ManageStockBuilder.IsLocked && SelectedSubArticle != null);
            AddSubArticleCommand = new RelayCommand(_ => AddSubArticle(), _ => !ManageStockBuilder.IsLocked && SelectedArticle != null);
            EditSubArticleCommand = new RelayCommand(_ => EditSubArticle(), _ => !ManageStockBuilder.IsLocked && SelectedSubArticle != null);

            AddArticleProviderCommand = new RelayCommand(_ => AddArticleProvider(), _ => !ManageStockBuilder.IsLocked && SelectedArticle != null);

            ImportListPriceCommand = new RelayCommand(_ => ImportPriceList(), _ => !ManageStockBuilder.IsLocked);

            Navigator = new ItemNavigator<Article>();
            m_AddedHistories = new Dictionary<Article, List<History>>();
            Articles = new ObservableTrackableCollection<Article>();
            SelectedArticle = null;
            Header = "Articles";
            TemplateName = "articles";
        }

        public ICommand GoToHistoryCommand { get; set; }
        public ICommand ImportListPriceCommand { get; set; }

        public ICommand AddArticleProviderCommand { get; set; }

        public ICommand ProductionArticleCommand { get; set; }
        public ICommand ResupplyArticleCommand { get; set; }
        public ICommand InputArticleCommand { get; set; }
        public ICommand OuputArticleCommand { get; set; }

        public ICommand DeleteSubArticleCommand { get; set; }

        public ICommand AddSubArticleCommand { get; set; }
        public ICommand EditSubArticleCommand { get; set; }

        public ICommand AddArticleCommand { get; set; }
        public ICommand DeleteArticleCommand { get; set; }

        public string SearchText
        {
            get => m_SearchText;
            set
            {
                m_SearchText = value;
                Filter();
                OnPropertyChanged();
            }
        }

        public Article SelectedArticle
        {
            get => m_SelectedArticle;
            set
            {
                m_SelectedArticle = value;
                OnPropertyChanged();
            }
        }

        public GroupArticle SelectedSubArticle
        {
            get => m_SelectedSubArticle;
            set
            {
                m_SelectedSubArticle = value;
                OnPropertyChanged();
            }
        }

        public ItemNavigator<Article> Navigator { get; set; }

        public override Type ModelType => typeof(Article);

        public ObservableTrackableCollection<Article> Articles { get; set; }

        public ICollectionView ArticleCollection { get; set; }

        public override void RequestView(IDatabaseModel _Item)
        {
            SelectedArticle = _Item as Article;
        }

        private void GoToHistory()
        {
            OnChangeViewRequest(new ViewRequestEventArgs(SelectedArticle, typeof(HistoryViewModel)));
        }

        public override void SelectItem(ItemBase itemBase)
        {
            SelectedArticle = itemBase as Article;
        }

        private void Filter()
        {
            if (string.IsNullOrEmpty(SearchText))
            {
                // reset filter 
                ArticleCollection.Filter = _ => true;
                return;
            }

            ArticleCollection.Filter = _Item =>
            {
                if (_Item is Article article)
                {
                    return article.Number.ToLower().Contains(SearchText.ToLower());
                }

                return false;
            };
        }

        private void AddArticleProvider()
        {
            AddArticleProviderWindow window = new AddArticleProviderWindow(SelectedArticle.GroupProviderID)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };
            if (window.ShowDialog() == true)
            {
                DataManager.Execute(EnumDatabaseAction.Insert, window.GroupProvider);
            }
        }

        private void StockAction(EnumStockAction _Action)
        {
            try
            {
                string message = "";
                Action<double> action = null;
                Func<double, bool> verify = null;
                m_AddedHistories.Clear();
                switch (_Action)
                {
                    case EnumStockAction.Production:
                        message = "Quantité produite : ";
                        verify = _Quantity => VerifyProductionArticle(_Quantity);
                        action = _Quantity => ProductionArticle(_Quantity);
                        break;
                    case EnumStockAction.Resupply:
                        message = "Quantité à restocker : ";
                        verify = _Quantity => VerifyResupplyArticle(_Quantity);
                        action = _Quantity => ResupplyArticle(_Quantity);
                        break;
                    case EnumStockAction.Input:
                        message = "Quantité à entrer : ";
                        verify = _Quantity => VerifyInputArticle(_Quantity);
                        action = _Quantity => InputArticle(_Quantity);
                        break;
                    case EnumStockAction.Output:
                        message = "Quantité à sortir : ";
                        verify = _Quantity => VerifyOuputArticle(_Quantity);
                        action = _Quantity => OuputArticle(_Quantity);
                        break;
                }
                QuantitiyStockPopup popup = new QuantitiyStockPopup(message)
                {
                    Owner = System.Windows.Application.Current.MainWindow
                };

                if (popup.ShowDialog() == true)
                {
                    if (VerifyQuantity(popup.Quantity))
                    {
                        NotifyError("La quantité entrée n'est pas valide.");
                        return;
                    }

                    if (action != null)
                    {
                        bool result = verify.Invoke(popup.Quantity);

                        if (!result)
                        {
                            return;
                        }

                        CommandManager.BeginGroup();
                        action.Invoke(popup.Quantity);

                        DateTime now = DateTime.Now;
                        // add history
                        foreach (var group in m_AddedHistories)
                        {
                            var article = group.Key;
                            var histories = group.Value;

                            foreach (var history in histories)
                            {
                                history.Date = now;
                                article.History.Add(history);
                            }
                        }
                        CommandManager.EndGroup();

                        DataManager.HistoryManager.InsertAll(m_AddedHistories.Values.SelectMany(_ => _).ToList());
                        DataManager.Execute(EnumDatabaseAction.Update, SelectedArticle);
                    }
                }

                m_AddedHistories.Clear();

            }
            catch (Exception)
            {
                m_AddedHistories.Clear();
            }
        }

        private bool VerifyProductionArticle(double _Quantity)
        {
            if (SelectedArticle.AssemblyType == EnumArticleAssemblyType.ProductAtTheOutput)
            {
                NotifyError($"L'article {SelectedArticle.Number} ne peut pas être produit. Veuillez passer cet article en 'Article produit à l'avance'.");
                return false;
            }

            if (SelectedArticle.GroupArticles.Any(_GroupArticle => _GroupArticle.Item.Quantity - (_Quantity * _GroupArticle.QuantityUse) < 0))
            {
                NotifyWarning("Il n'y a pas assez de quantité de sous-articles.");
                return false;
            }

            return true;
        }

        private void ProductionArticle(double _Quantity)
        {
            SelectedArticle.Quantity += _Quantity;
            m_AddedHistories.Add(SelectedArticle, new List<History>());
            m_AddedHistories[SelectedArticle].Add(new History() { ActionType = EnumStockAction.Production, ArticleID = SelectedArticle.ID, Quantity = _Quantity, Balance = SelectedArticle.Quantity });

            foreach (GroupArticle groupArticle in SelectedArticle.GroupArticles)
            {
                groupArticle.Item.Quantity -= _Quantity * groupArticle.QuantityUse;
                if (!m_AddedHistories.ContainsKey(groupArticle.Item))
                {
                    m_AddedHistories.Add(groupArticle.Item, new List<History>());
                }
                m_AddedHistories[groupArticle.Item].Add(new History() { ActionType = EnumStockAction.Output, ArticleID = groupArticle.ArticleId, Quantity = _Quantity * groupArticle.QuantityUse, Balance = groupArticle.Item.Quantity });
            }
        }

        private bool VerifyQuantity(double _Quantity) => _Quantity <= 0;

        private bool VerifyOuputArticle(double _Quantity)
        {
            switch (SelectedArticle.AssemblyType)
            {
                case EnumArticleAssemblyType.ProductInAdvance:
                    {
                        if (SelectedArticle.Quantity - _Quantity < 0)
                        {
                            NotifyWarning("Il n'y pas assez de quantité d'article.");
                            return false;
                        }
                    }
                    break;
                case EnumArticleAssemblyType.ProductAtTheOutput:
                    {
                        if (SelectedArticle.Quantity - _Quantity < 0)
                        {
                            NotifyWarning("Il n'y pas assez de quantité d'article.");
                            return false;
                        }

                        if (SelectedArticle.GroupArticles.Any(_GroupArticle => _GroupArticle.Item.Quantity - (_Quantity * _GroupArticle.QuantityUse) < 0))
                        {
                            NotifyWarning("Il n'y a pas assez de sous-articles.");
                            return false;
                        }
                    }
                    break;
            }
            return true;
        }

        private void OuputArticle(double _Quantity)
        {
            switch (SelectedArticle.AssemblyType)
            {
                case EnumArticleAssemblyType.ProductInAdvance:
                    {
                        SelectedArticle.Quantity -= _Quantity;
                        m_AddedHistories.Add(SelectedArticle, new List<History>());
                        m_AddedHistories[SelectedArticle].Add(new History() { ActionType = EnumStockAction.Output, ArticleID = SelectedArticle.ID, Quantity = _Quantity, Balance = SelectedArticle.Quantity });
                    }
                    break;
                case EnumArticleAssemblyType.ProductAtTheOutput:
                    {
                        // output
                        SelectedArticle.Quantity -= _Quantity;
                        m_AddedHistories.Add(SelectedArticle, new List<History>());
                        m_AddedHistories[SelectedArticle].Add(new History() { ActionType = EnumStockAction.Output, ArticleID = SelectedArticle.ID, Quantity = _Quantity, Balance = SelectedArticle.Quantity });

                        foreach (GroupArticle groupArticle in SelectedArticle.GroupArticles)
                        {
                            groupArticle.Item.Quantity -= _Quantity * groupArticle.QuantityUse;
                            if (!m_AddedHistories.ContainsKey(groupArticle.Item))
                            {
                                m_AddedHistories.Add(groupArticle.Item, new List<History>());
                            }
                            m_AddedHistories[groupArticle.Item].Add(new History() { ActionType = EnumStockAction.Output, ArticleID = groupArticle.ArticleId, Quantity = _Quantity * groupArticle.QuantityUse, Balance = groupArticle.Item.Quantity });
                        }
                    }
                    break;
            }
        }

        private bool VerifyInputArticle(double _Quantity)
        {
            return true;
        }

        private void InputArticle(double _Quantity)
        {
            switch (SelectedArticle.AssemblyType)
            {
                case EnumArticleAssemblyType.ProductInAdvance:
                    {
                        SelectedArticle.Quantity += _Quantity;
                        m_AddedHistories.Add(SelectedArticle, new List<History>());
                        m_AddedHistories[SelectedArticle].Add(new History() { ActionType = EnumStockAction.Input, ArticleID = SelectedArticle.ID, Quantity = _Quantity, Balance = SelectedArticle.Quantity });
                    }
                    break;
                case EnumArticleAssemblyType.ProductAtTheOutput:
                    {
                        // input
                        SelectedArticle.Quantity += _Quantity;
                        m_AddedHistories.Add(SelectedArticle, new List<History>());
                        m_AddedHistories[SelectedArticle].Add(new History() { ActionType = EnumStockAction.Input, ArticleID = SelectedArticle.ID, Quantity = _Quantity, Balance = SelectedArticle.Quantity });
                    }
                    break;
            }
        }

        private bool VerifyResupplyArticle(double _Quantity)
        {
            return true;
        }

        private void ResupplyArticle(double _Quantity)
        {
            switch (SelectedArticle.AssemblyType)
            {
                case EnumArticleAssemblyType.ProductInAdvance:
                    {
                        SelectedArticle.Quantity += _Quantity;
                        m_AddedHistories.Add(SelectedArticle, new List<History>());
                        m_AddedHistories[SelectedArticle].Add(new History() { ActionType = EnumStockAction.Resupply, ArticleID = SelectedArticle.ID, Quantity = _Quantity, Balance = SelectedArticle.Quantity });
                    }
                    break;
                case EnumArticleAssemblyType.ProductAtTheOutput:
                    {
                        // ressuply
                        SelectedArticle.Quantity += _Quantity;
                        m_AddedHistories.Add(SelectedArticle, new List<History>());
                        m_AddedHistories[SelectedArticle].Add(new History() { ActionType = EnumStockAction.Resupply, ArticleID = SelectedArticle.ID, Quantity = _Quantity, Balance = SelectedArticle.Quantity });

                        foreach (GroupArticle groupArticle in SelectedArticle.GroupArticles)
                        {
                            groupArticle.Item.Quantity += _Quantity * groupArticle.QuantityUse;
                            if (!m_AddedHistories.ContainsKey(groupArticle.Item))
                            {
                                m_AddedHistories.Add(groupArticle.Item, new List<History>());
                            }
                            m_AddedHistories[groupArticle.Item].Add(new History() { ActionType = EnumStockAction.Resupply, ArticleID = groupArticle.ArticleId, Quantity = _Quantity * groupArticle.QuantityUse, Balance = groupArticle.Item.Quantity });
                        }
                    }
                    break;
            }
        }

        private void AddArticle()
        {
            CommandManager.Suspend();
            Article article = new Article();
            article.Default();
            EditArticleWindow window = new EditArticleWindow(article)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };
            if (window.ShowDialog() == true)
            {
                article.CopyTo(window.Article);
                if (DataManager.Execute(EnumDatabaseAction.Insert, article) == true)
                {
                    CommandManager.Resume();
                    Articles.Add(article);
                    article.CommentaryChanged += ArticleCommentaryChanged;
                    SelectedArticle = article;
                }
            }
            CommandManager.Resume();
        }

        public void UpdateArticle(Article _Article)
        {
            CommandManager.Suspend();
            EditArticleWindow window = new EditArticleWindow(_Article)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            if (window.ShowDialog() == true)
            {
                CommandManager.Resume();
                CommandManager.BeginGroup();
                _Article.CopyTo(window.Article);
                CommandManager.EndGroup();
                if (DataManager.Execute(EnumDatabaseAction.Update, _Article) == true)
                {
                    SelectedArticle = _Article;
                }
            }
            CommandManager.Resume();
        }

        public void DeleteArticle(Article _Article)
        {
            ConfirmationPopup popup = new ConfirmationPopup($"Êtes-vous sûr de vouloir supprimer cet article '{_Article.Number}' ?", "Confirmation")
            {
                ShowCancelButton = false,
                Owner = System.Windows.Application.Current.MainWindow
            };
            popup.ShowDialog();

            if (popup.Result == EnumPopupResult.Yes)
            {
                if (DataManager.Execute(EnumDatabaseAction.Delete, _Article) == true)
                {
                    Articles.Remove(_Article);
                    _Article.CommentaryChanged -= ArticleCommentaryChanged;
                }
            }
        }

        public override void Initialize(CustomNotificationsManager _NotificationManager)
        {
            base.Initialize(_NotificationManager);

            CommandManager?.Suspend();

            m_SearchText = "";
            m_SelectedArticle = null;
            Articles.Clear();

            foreach (var article in DataManager.ArticleManager.FetchAll())
            {
                Articles.Add(article);
                article.CommentaryChanged += ArticleCommentaryChanged;
            }

            ArticleCollection = CollectionViewSource.GetDefaultView(Articles);
            ArticleCollection.SortDescriptions.Add(new SortDescription(nameof(Article.Number), ListSortDirection.Ascending));

            Articles.InitializeTrackable(CommandManager);

            CommandManager?.Resume();

            Navigator.ItemOnChanged += NavigatorItemOnChanged;
        }
        public override void Reload()
        {
            foreach (var article in Articles)
            {
                article.CommentaryChanged -= ArticleCommentaryChanged;
            }

            var saveSelectedArticle = m_SelectedArticle?.Number;

            m_SearchText = "";
            m_SelectedArticle = null;
            Articles.Clear();

            foreach (var article in DataManager.ArticleManager.FetchAll(true))
            {
                Articles.Add(article);
                if(article.Number == saveSelectedArticle)
                {
                    m_SelectedArticle = article;
                }
                article.CommentaryChanged += ArticleCommentaryChanged;
            }

            OnPropertyChanged(nameof(SelectedArticle));
        }

        private void NavigatorItemOnChanged(object sender, NavigatorItemChangedEventArgs<Article> e)
        {
            SelectedArticle = e.Item;
        }

        private void ArticleCommentaryChanged(object sender, EventArgs e)
        {
            Article article = sender as Article;
            if (article != null)
            {
                DataManager.ArticleManager.NotificationManager.SuspendNotification();
                DataManager.Execute(EnumDatabaseAction.Update, article);
                DataManager.ArticleManager.NotificationManager.ResumeNotification();
            }
        }

        private void DeleteSubArticle()
        {
            ConfirmationPopup popup = new ConfirmationPopup($"Êtes-vous sûr de vouloir supprimer le sous-article '{SelectedSubArticle.Item.Number}' ?", "Confirmation")
            {
                ShowCancelButton = false,
                Owner = System.Windows.Application.Current.MainWindow
            };
            popup.ShowDialog();

            if (popup.Result == EnumPopupResult.Yes)
            {
                DataManager.Execute(EnumDatabaseAction.Delete, SelectedSubArticle);
            }
        }

        private void EditSubArticle()
        {
            EditSubArticleWindow window = new EditSubArticleWindow(Articles.ToList(), SelectedSubArticle)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };
            if (window.ShowDialog() == true)
            {
                CommandManager.BeginGroup();
                SelectedSubArticle.QuantityUse = window.QuantityUse;
                // remove and add
                if (SelectedSubArticle.ArticleId != window.SelectedArticle.ID)
                {
                    DataManager.GroupArticleManager.NotificationManager.SuspendNotification();
                    DataManager.Execute(EnumDatabaseAction.Delete, SelectedSubArticle);
                    DataManager.GroupArticleManager.NotificationManager.ResumeNotification();

                    GroupArticle groupArticle = new GroupArticle
                    {
                        ArticleId = window.SelectedArticle.ID,
                        QuantityUse = window.QuantityUse,
                        Item = window.SelectedArticle,
                        ID = SelectedArticle.GroupArticleID
                    };
                    DataManager.Execute(EnumDatabaseAction.Insert, groupArticle);

                    SelectedSubArticle = groupArticle;
                }
                // update
                else
                {
                    DataManager.Execute(EnumDatabaseAction.Update, SelectedSubArticle);
                }
                CommandManager.EndGroup();
            }
        }

        private void AddSubArticle()
        {
            EditSubArticleWindow window = new EditSubArticleWindow(Articles.ToList())
            {
                Owner = System.Windows.Application.Current.MainWindow
            };
            if (window.ShowDialog() == true)
            {
                CommandManager.BeginGroup();
                GroupArticle groupArticle = new GroupArticle
                {
                    ArticleId = window.SelectedArticle.ID,
                    QuantityUse = window.QuantityUse,
                    Item = window.SelectedArticle,
                    ID = SelectedArticle.GroupArticleID
                };
                DataManager.Execute(EnumDatabaseAction.Insert, groupArticle);

                CommandManager.EndGroup();
            }
        }

        private void ImportPriceList()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Excel files (*.xlsx, *.xls, *.csv) |*.xlsx;*.xls;*.csv"
            };
            if (dialog.ShowDialog() == true)
            {
                var task = Task<DataExportResult>.Factory.StartNew(() =>
                {
                    m_NotificationManager.SuspendNotification();
                    CommandManager.Suspend();
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        LoadingInfo.IsLoading = true;
                        LoadingInfo.Text = "Lecture du fichier Excel...";
                        LoadingInfo.IsIndeterminate = true;
                        LoadingInfo.Value = 0;
                    });

                    var result = ExcelManager.InstanceOf.ReadWorksheet(dialog.FileName, out ExcelWorksheet worksheet);

                    string error = "";
                    switch (result)
                    {
                        case EnumExcelStatus.FileInUse:
                            error = $"Le fichier {dialog.FileName} est déjà en cours d'utilisation. Veuillez le fermer avant d'importer les données.";
                            break;
                        case EnumExcelStatus.FileNotExist:
                            error = $"Le fichier {dialog.FileName} est introuvable.";
                            break;
                        case EnumExcelStatus.FileOpened:
                            error = $"Une erreur est survenue à l'ouverture du fichier {dialog.FileName}. Vérifier que le fichier n'est pas corrompu.";
                            break;
                        case EnumExcelStatus.Unknown:
                            error = $"Une erreur inconnue est survenue durant la lecture du fichier {dialog.FileName}. Aucune donnée n'a été importée.";
                            break;
                    }

                    if(result != EnumExcelStatus.Success)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            LoadingInfo.IsLoading = false;
                            LoadingInfo.Text = "";
                        });

                        CommandManager.Resume();
                        m_NotificationManager.ResumeNotification();
                        return new DataExportResult { Result = error };
                    }

                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        LoadingInfo.IsIndeterminate = false;
                        LoadingInfo.Text = "Importation des données...";
                    });

                    List<DataExport> articles = new List<DataExport>();
                    double ratio = 100d/(double)worksheet.Cells.GetLength(0);
                    for (int row = 3; row < worksheet.Cells.GetLength(0); row++)
                    {
                        var number = worksheet.Cells[row, 0];
                        var priceHtStr = worksheet.Cells[row, 3];
                        var priceTtcStr = worksheet.Cells[row, 4];
                        var rubricStr = worksheet.Cells[row, 5];

                        var reference = new DataExportValue
                        {
                            Name = "Number",
                            Value = number
                        };
                        var refPriceHt = new DataExportValue { Name = "Prix HT", Value = priceHtStr };
                        var refPriceTtc = new DataExportValue { Name = "Prix TTC", Value = priceTtcStr};
                        var refRubric = new DataExportValue { Name = "Rubrique", Value = rubricStr };

                        var article = Articles.FirstOrDefault(a => a.Number == number);
                        if(article != null)
                        {
                            reference.ExportStatus = EnumDataExportStatus.Sucess;

                            refPriceHt.ExportStatus = EnumDataExportStatus.DataError;
                            refPriceTtc.ExportStatus = EnumDataExportStatus.DataError;
                            refRubric.ExportStatus = EnumDataExportStatus.DataError;

                            // check values
                            if (!string.IsNullOrEmpty(priceHtStr) && double.TryParse(priceHtStr.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double priceHt))
                            {
                                article.PriceHT = priceHt;
                                refPriceHt.ExportStatus = EnumDataExportStatus.Sucess;
                            }

                            if (!string.IsNullOrEmpty(priceTtcStr) && double.TryParse(priceTtcStr.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double priceTtc))
                            {
                                article.PriceTTC = priceTtc;
                                refPriceTtc.ExportStatus = EnumDataExportStatus.Sucess;
                            }

                            if (!string.IsNullOrEmpty(rubricStr))
                            {
                                article.RubricPage = rubricStr;
                                refRubric.ExportStatus = EnumDataExportStatus.Sucess;
                            }

                            DataManager.Execute(EnumDatabaseAction.Update, article);
                        }
                        else
                        {
                            reference.ExportStatus = EnumDataExportStatus.NotFound;
                            refPriceHt.ExportStatus = EnumDataExportStatus.NotHandled;
                            refPriceTtc.ExportStatus = EnumDataExportStatus.NotHandled;
                            refRubric.ExportStatus = EnumDataExportStatus.NotHandled;
                        }

                        articles.Add(new DataExport { Reference = reference, Data = new List<DataExportValue> { refPriceHt, refPriceTtc, refRubric } });

                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            LoadingInfo.Value += ratio;
                        });
                    }

                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        LoadingInfo.IsLoading = false;
                        LoadingInfo.Text = "";
                    });

                    CommandManager.Resume();
                    m_NotificationManager.ResumeNotification();

                    return new DataExportResult { Data = articles };
                });

                task.ContinueWith(_Task =>
                {
                    Action<string, string> notifier = null;

                    DataExportResult result = _Task.Result;
                    string text = "";
                    if (string.IsNullOrEmpty(result.Result))
                    {
                        text = $"Les données de {result.NumberImportSuccesValue} articles ont correctement été importées.";
                        notifier = NotifySucess;
                    }
                    else
                    {
                        text = result.Result;
                        notifier = NotifyError;
                    }
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        notifier.Invoke(text, null);
                    });
                });
            }
        }

        public override void Dispose()
        {
            Navigator.ItemOnChanged -= NavigatorItemOnChanged;

            foreach (Article article in Articles)
            {
                article.CommentaryChanged -= ArticleCommentaryChanged;
            }
        }
    }
}
