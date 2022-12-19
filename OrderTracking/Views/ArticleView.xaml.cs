using Application.Common.Models.Articles;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace OrderTracking.Views
{
    /// <summary>
    /// Interaction logic for ArticleView.xaml
    /// </summary>
    public partial class ArticleView : UserControl
    {
        public ArticleView()
        {
            InitializeComponent();
            Spreadsheet.SetSettings(unvell.ReoGrid.ReoGridSettings.Edit_All);
        }

        public MainViewModel MainViewModel => (MainViewModel)DataContext;


        public static readonly DependencyProperty ArticleProperty = DependencyProperty.Register("Article", typeof(Article), typeof(ArticleView), new FrameworkPropertyMetadata(default(Article)));

        public Article Article
        {
            get => (Article)GetValue(ArticleProperty);
            set => SetValue(ArticleProperty, value);
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is MainViewModel oldMainViewModel)
            {
                oldMainViewModel.SpreadSheetActionRequest -= MainViewModelSpreadSheetActionRequest;
            }

            if (e.NewValue is MainViewModel newMainViewModel)
            {
                newMainViewModel.SpreadSheetActionRequest += MainViewModelSpreadSheetActionRequest;
            }

        }

        private void MainViewModelSpreadSheetActionRequest(object sender, Events.SpreadSheetEventArgs e)
        {
            if(e.Action == Events.EnumAction.Save)
            {
                Spreadsheet.Save(e.FileName);
            }
            else if(e.Action == Events.EnumAction.Load)
            {
                Spreadsheet.Reset();
                if (File.Exists(e.FileName))
                {
                    Spreadsheet.Load(e.FileName);
                }
            }
        }
    }
}
