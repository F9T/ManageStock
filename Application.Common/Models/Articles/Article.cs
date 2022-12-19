using Application.CommandManager.Collection;
using Application.Common.Models.Groups;
using Application.Common.Models.Items;
using System;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace Application.Common.Models.Articles
{
    public sealed class Article : ItemBase
    {
        private string m_Number;
        private string m_Description;
        private string m_Commentary;
        private string m_RubricPage;
        private double m_CriticalQuantity;
        private double m_Quantity;
        private double m_PriceTTC;
        private double m_PriceHT;
        private EnumArticleAssemblyType m_AssemblyType;

        public event EventHandler CommentaryChanged;

        private object m_LockHistory = new object();

        public Article()
        {
            Default();
        }

        public override void InitializeTrackable(CommandManager.CommandManager _CommandManager)
        {
            base.InitializeTrackable(_CommandManager);

            GroupArticles.InitializeTrackable(_CommandManager);
            GroupProviders.InitializeTrackable(_CommandManager);
            History.InitializeTrackable(_CommandManager);
        }

        public bool IsInsufficient => CriticalQuantity > 0 && Quantity <= CriticalQuantity;

        public Guid GroupArticleID { get; set; }
        public Guid GroupProviderID { get; set; }

        public EnumArticleAssemblyType AssemblyType
        {
            get => m_AssemblyType;
            set
            {
                var oldValue = m_AssemblyType;
                m_AssemblyType = value;
                OnPropertyChanged(nameof(AssemblyType), oldValue, m_AssemblyType);
            }
        }

        public double PriceHT
        {
            get => m_PriceHT;
            set
            {
                var oldValue = m_PriceHT;
                m_PriceHT = Math.Round(value, 2);
                OnPropertyChanged(nameof(PriceHT), oldValue, m_PriceHT);
            }
        }

        public double PriceTTC
        {
            get => m_PriceTTC;
            set
            {
                var oldValue = m_PriceTTC;
                m_PriceTTC = Math.Round(value, 2);
                OnPropertyChanged(nameof(PriceTTC), oldValue, m_PriceTTC);
            }
        }

        public double Quantity
        {
            get => m_Quantity;
            set
            {
                var oldValue = m_Quantity;
                m_Quantity = Math.Round(value, 2);
                OnPropertyChanged(nameof(Quantity), oldValue, m_Quantity);
                PerformPropertyChangeWithoutCommand(nameof(IsInsufficient));
            }
        }

        public double CriticalQuantity
        {
            get => m_CriticalQuantity;
            set
            {
                var oldValue = m_CriticalQuantity;
                m_CriticalQuantity = Math.Round(value, 2);
                OnPropertyChanged(nameof(CriticalQuantity), oldValue, m_CriticalQuantity);
                PerformPropertyChangeWithoutCommand(nameof(IsInsufficient));
            }
        }

        public string Number
        {
            get => m_Number;
            set
            {
                var oldValue = m_Number;
                m_Number = value;
                OnPropertyChanged(nameof(Number), oldValue, m_Number);
            }
        }

        public string Name { get; set; }
        public string Description
        {
            get => m_Description;
            set
            {
                var oldValue = m_Description;
                m_Description = value;
                OnPropertyChanged(nameof(Description), oldValue, m_Description);
            }
        }

        public string RubricPage
        {
            get => m_RubricPage;
            set
            {
                var oldValue = m_RubricPage;
                m_RubricPage = value;
                OnPropertyChanged(nameof(RubricPage), oldValue, m_RubricPage);
            }
        }
        public string Commentary 
        { 
            get => m_Commentary;
            set
            {
                var oldValue = m_Commentary;
                m_Commentary = value;
                OnCommentaryChanged();
                OnPropertyChanged(nameof(Commentary), oldValue, m_Commentary);
            }
        }

        public ObservableTrackableCollection<GroupArticle> GroupArticles { get; set; }
        public ObservableTrackableCollection<GroupProvider> GroupProviders { get; set; }

        public ObservableTrackableCollection<History.History> History { get; set; }

        public string OrderFileName { get; set; }


        // use for update article if order file name not exist
        public bool MustSaved { get; set; }

        public override int ZIndex => 1;

        public void CopyTo(Article _Article)
        {
            PriceHT = _Article.m_PriceHT;
            PriceTTC = _Article.PriceTTC;
            Quantity = _Article.Quantity;
            CriticalQuantity = _Article.CriticalQuantity;
            Number = _Article.Number;
            Description = _Article.Description;
            RubricPage = _Article.RubricPage;
            OrderFileName = _Article.OrderFileName;
            AssemblyType = _Article.AssemblyType;
        }

        public override void Default()
        {
            AssemblyType = EnumArticleAssemblyType.ProductAtTheOutput;
            MustSaved = false;
            Name = "";
            Description = "";
            RubricPage = "";
            Commentary = "";
            Number = "";
            GroupArticles = new ObservableTrackableCollection<GroupArticle>();
            GroupProviders = new ObservableTrackableCollection<GroupProvider>();
            History = new ObservableTrackableCollection<History.History>();
            GroupProviderID = Guid.NewGuid();
            GroupArticleID = Guid.NewGuid();
            OrderFileName = Guid.NewGuid().ToString();
        }

        public override string ToString()
        {
            return Number;
        }

        public override object Clone()
        {
            Article article = (Article)MemberwiseClone();
            article.GroupArticles = new ObservableTrackableCollection<GroupArticle>();
            foreach (var ga in GroupArticles)
            {
                article.GroupArticles.Add((GroupArticle)ga.Clone());
            }
            article.GroupProviders = new ObservableTrackableCollection<GroupProvider>();
            foreach (var gp in GroupProviders)
            {
                article.GroupProviders.Add((GroupProvider)gp.Clone());
            }
            return article;
        }

        public void OnCommentaryChanged()
        {
            CommentaryChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}