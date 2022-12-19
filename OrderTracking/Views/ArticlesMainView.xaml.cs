using Application.Common.Models.Articles;
using System.Windows;
using System.Windows.Controls;

namespace OrderTracking.Views
{
    /// <summary>
    /// Interaction logic for ArticlesMainView.xaml
    /// </summary>
    public partial class ArticlesMainView : UserControl
    {
        public ArticlesMainView()
        {
            MainViewModel = new MainViewModel();
            InitializeComponent();

            DataContext = MainViewModel;
        }

        public MainViewModel MainViewModel { get; set; }

        private void DeleteArticleButttonOnClick(object sender, RoutedEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element != null)
            {
                var article = element.Tag as Article;
                if (article != null)
                {
                    MainViewModel.DeleteArticle(article);
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
                    MainViewModel.UpdateArticle(article);
                }
            }
        }

        private void ArticleListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainViewModel == null)
            {
                return;
            }

            if (sender is ListView listView)
            {
                listView.ScrollIntoView(MainViewModel.SelectedArticle);
            }
        }
    }
}
