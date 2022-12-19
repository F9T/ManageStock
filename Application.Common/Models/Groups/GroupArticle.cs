using Application.Common.Models.Articles;

namespace Application.Common.Models.Groups
{
    public class GroupArticle : GroupBase<Article>
    {
        private int m_ArticleId;
        private double m_QuantityUse;

        public int ArticleId
        {
            get => m_ArticleId;
            set
            {
                var oldValue = m_ArticleId;
                m_ArticleId = value;
                OnPropertyChanged(nameof(ArticleId), oldValue, m_ArticleId);
            }
        }

        public double QuantityUse 
        { 
            get => m_QuantityUse;
            set 
            { 
                var oldValue = m_QuantityUse;
                m_QuantityUse = value;
                OnPropertyChanged(nameof(QuantityUse), oldValue, m_QuantityUse);
            }
        }

        public override int ZIndex => 1000;

        public override object Clone()
        {
            GroupArticle groupArticle = (GroupArticle)MemberwiseClone();
            groupArticle.Item = (Article)Item.Clone();
            return groupArticle;
        }

        public override void CopyTo(GroupBase<Article> _Group)
        {

        }
    }
}
