using Application.Common;
using Application.Common.Commands;
using Application.Common.Models.Articles;
using Application.Common.Models.Groups;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ManageStock.Views.Articles.SubArticles
{
    /// <summary>
    /// Interaction logic for AddSubArticleWindow.xaml
    /// </summary>
    public partial class EditSubArticleWindow : CustomWindow, INotifyPropertyChanged
    {
        private double m_QuantityUse;
        private string m_ConfirmButtonText;
        private Article m_SelectedArticle;

        public EditSubArticleWindow(List<Article> _Articles, GroupArticle _GroupArticle = null)
        {
            AddArticleCommand = new RelayCommand(_ => AddArticle(), _ => SelectedArticle != null && QuantityUse > 0);

            m_ConfirmButtonText = "Ajouter";

            m_QuantityUse = 1.0;
            if (_GroupArticle != null)
            {
                m_ConfirmButtonText = "Modifier";
                Title = "Modification d'un sous-article";

                m_QuantityUse = _GroupArticle.QuantityUse;
                SelectedArticle = _GroupArticle.Item;
            }

            Articles = _Articles;
            InitializeComponent();

            DataContext = this;
        }

        public string ConfirmButtonText
        {
            get => m_ConfirmButtonText;
            set
            {
                m_ConfirmButtonText = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddArticleCommand { get; set; }

        public List<Article> Articles { get; set; }

        public Article SelectedArticle
        {
            get => m_SelectedArticle;
            set
            {
                m_SelectedArticle = value;
                OnPropertyChanged();
            }
        }

        public double QuantityUse
        {
            get => m_QuantityUse;
            set
            {
                m_QuantityUse = value;
                OnPropertyChanged();
            }
        }

        private void AddArticle()
        {
            DialogResult = true;
        }

        private void CancelButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
