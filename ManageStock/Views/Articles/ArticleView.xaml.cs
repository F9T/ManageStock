using Application.Common.Managers;
using Application.Common.Managers.DatabaseManagerBase;
using Application.Common.Models.Articles;
using Application.Common.Models.Groups;
using ManageStock.ViewModels;
using ManageStock.Views.Articles.Providers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ManageStock.Views.Articles
{
    /// <summary>
    /// Interaction logic for ArticleView.xaml
    /// </summary>
    public partial class ArticleView : UserControl
    {
        public ArticleView()
        {
            InitializeComponent();
        }

        public ArticleViewModel ViewModel => (ArticleViewModel)DataContext;


        public static readonly DependencyProperty ArticleProperty = DependencyProperty.Register("Article", typeof(Article), typeof(ArticleView), new FrameworkPropertyMetadata(default(Article)));

        public Article Article
        {
            get => (Article)GetValue(ArticleProperty);
            set => SetValue(ArticleProperty, value);
        }

        private void ProviderHyperLinkOnClick(object sender, RoutedEventArgs e)
        {
            Hyperlink element = sender as Hyperlink;
            if (element != null)
            {
                if (element.Tag is GroupProvider groupProvider)
                {
                    ArticleProviderInformationWindow window = new ArticleProviderInformationWindow(groupProvider)
                    {
                        Owner = System.Windows.Application.Current.MainWindow
                    };
                    if (window.ShowDialog() == true)
                    {
                        DataManager.Execute(EnumDatabaseAction.Update, groupProvider);
                    }
                }
            }
        }

        private void DataGridRowMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DataGridRow dataGridRow = sender as DataGridRow;
            if(dataGridRow != null)
            {
                if(dataGridRow.Item is GroupArticle groupArticle)
                {
                    ViewModel.SearchText = "";
                    ViewModel.SelectedArticle = groupArticle.Item;
                }
            }
        }
    }
}
