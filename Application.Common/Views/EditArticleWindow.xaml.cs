using Application.Common.Models.Articles;
using System.ComponentModel;
using System.Windows;

namespace Application.Common.Views
{
    /// <summary>
    /// Interaction logic for EditArticleWindow.xaml
    /// </summary>
    public partial class EditArticleWindow : CustomWindow
    {
        private Article m_SaveArticle;
        private bool m_ManualClosing = false;

        public EditArticleWindow(Article _Article)
        {
            m_SaveArticle = _Article;
            Article = (Article)_Article.Clone(); ;

            InitializeComponent();

            DataContext = this;
        }

        public Article Article { get; set; }


        private void CancelButtonOnClick(object sender, RoutedEventArgs e)
        {
            m_ManualClosing = true;
            DialogResult = false;
        }

        private void ConfirmButtonOnClick(object sender, RoutedEventArgs e)
        {
            m_ManualClosing = true;
            DialogResult = true;
        }

        private void EditArticleWindowOnClosing(object sender, CancelEventArgs e)
        {
            if (!m_ManualClosing)
            {
                Article.CopyTo(m_SaveArticle);
                DialogResult = false;
            }
        }
    }
}
