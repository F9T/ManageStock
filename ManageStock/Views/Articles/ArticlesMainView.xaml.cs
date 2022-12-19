using Application.Common.Models.Articles;
using Application.Excel;
using ManageStock.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace ManageStock.Views.Articles
{
    /// <summary>
    /// Interaction logic for ArticlesMainView.xaml
    /// </summary>
    public partial class ArticlesMainView : UserControl
    {
        public ArticlesMainView()
        {
            InitializeComponent();
        }
        public ArticleViewModel ViewModel => (ArticleViewModel)DataContext;

        private void DeleteArticleButttonOnClick(object sender, RoutedEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element != null)
            {
                var article = element.Tag as Article;
                if (article != null)
                {
                    ViewModel.DeleteArticle(article);
                }
            }
        }
        private void EditArticleButttonOnClick(object sender, RoutedEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element != null)
            {
                var article = element.Tag as Article;
                if (article != null)
                {
                    ViewModel.UpdateArticle(article);
                }
            }
        }

        private void ArticleListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel == null)
            {
                return;
            }

            if (sender is ListView listView)
            {
                listView.ScrollIntoView(ViewModel.SelectedArticle);
            }

            ViewModel.Navigator.Add(ViewModel.SelectedArticle);
        }
    }
}
