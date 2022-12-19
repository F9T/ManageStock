using Application.Common.Commands;
using Application.Common.Logger;
using Application.Common.Managers;
using Application.Common.Managers.DatabaseManagerBase;
using Application.Common.Models;
using Application.Common.Models.Articles;
using Application.Common.Models.Items;
using Application.Common.Notifications;
using Application.Common.PopupWindows;
using Application.Common.ViewModels;
using Application.Common.Views;
using OrderTracking.Events;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Input;

namespace OrderTracking
{
    public class MainViewModel : ViewModelBase
    {
        private string m_SearchText;
        private Article m_SelectedArticle;
        private string m_RootSheetFolder;

        public event EventHandler<SpreadSheetEventArgs> SpreadSheetActionRequest;

        public MainViewModel()
        {
            m_RootSheetFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Sheets");

            AddArticleCommand = new RelayCommand(_ => AddArticle(), _ => true);
            SaveCommand = new RelayCommand(_ => Save(), _ => SelectedArticle != null);

            Articles = new ObservableCollection<Article>();
            SelectedArticle = null;
            Header = "Articles";
            TemplateName = "articles";
        }

        public ICommand AddArticleCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        public override Type ModelType => typeof(Article);

        public override void RequestView(IDatabaseModel _Item)
        {
            SelectedArticle = _Item as Article;
        }

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
                if(m_SelectedArticle != null)
                {
                    Save();
                }
                m_SelectedArticle = value;
                if (value != null)
                {
                    LoadDataArticle();
                }

                OnPropertyChanged();
            }
        }

        public ObservableCollection<Article> Articles { get; set; }

        public ICollectionView ArticleCollection { get; set; }
        public override void Reload()
        {

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
                    return article.Number.Contains(SearchText);
                }

                return false;
            };
        }

        private void Save()
        {
            if (SelectedArticle.MustSaved)
            {
                DataManager.ArticleManager.NotificationManager.SuspendNotification();
                DataManager.Execute(EnumDatabaseAction.Update, SelectedArticle);
                DataManager.ArticleManager.NotificationManager.ResumeNotification();
                SelectedArticle.MustSaved = false;
            }

            string path = Path.Combine(m_RootSheetFolder, SelectedArticle.OrderFileName);
            OnSpreadSheetActionRequest(this, new SpreadSheetEventArgs(path, EnumAction.Save));
        }

        private void LoadDataArticle()
        {
            if (SelectedArticle.MustSaved)
            {
                DataManager.ArticleManager.NotificationManager.SuspendNotification();
                DataManager.Execute(EnumDatabaseAction.Update, SelectedArticle);
                DataManager.ArticleManager.NotificationManager.ResumeNotification();
                SelectedArticle.MustSaved = false;
            }

            string path = Path.Combine(m_RootSheetFolder, SelectedArticle.OrderFileName);
            OnSpreadSheetActionRequest(this, new SpreadSheetEventArgs(path, EnumAction.Load));
        }

        private void AddArticle()
        {
            Article article = new Article();
            article.Default();
            EditArticleWindow window = new EditArticleWindow(article)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };
            if (window.ShowDialog() == true)
            {
                if (DataManager.Execute(EnumDatabaseAction.Insert, article) == true)
                {
                    Articles.Add(article);
                    SelectedArticle = article;
                }
            }
        }

        public void UpdateArticle(Article _Article)
        {
            EditArticleWindow window = new EditArticleWindow(_Article)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            _Article = window.Article;
            if (window.ShowDialog() == true)
            {
                if (DataManager.Execute(EnumDatabaseAction.Update, _Article) == true)
                {
                    SelectedArticle = _Article;
                }
            }
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
                }
            }
        }


        public override void SelectItem(ItemBase itemBase)
        {
            SelectedArticle = itemBase as Article;
        }

        public override void Initialize(CustomNotificationsManager _NotificationManager)
        {
            base.Initialize(_NotificationManager);

            m_SearchText = "";
            m_SelectedArticle = null;
            Articles.Clear();

            // create Sheets directory
            try
            {
                if (!Directory.Exists(m_RootSheetFolder))
                {
                    Directory.CreateDirectory(m_RootSheetFolder);
                }
            }
            catch (Exception e)
            {
                ApplicationLogger.InstanceOf.Write(e.StackTrace);
            }

            foreach (var article in DataManager.ArticleManager.FetchAll())
            {
                Articles.Add(article);
            }

            ArticleCollection = CollectionViewSource.GetDefaultView(Articles);
            ArticleCollection.SortDescriptions.Add(new SortDescription(nameof(Article.Number), ListSortDirection.Ascending));
        }

        public override void Dispose()
        {
        }

        public void OnSpreadSheetActionRequest(object _Sender, SpreadSheetEventArgs _Args)
        {
            SpreadSheetActionRequest?.Invoke(_Sender, _Args);
        }
    }
}
