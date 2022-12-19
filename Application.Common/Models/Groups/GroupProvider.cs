using Application.Common.Models.Devises;
using Application.Common.Models.Providers;
using System;

namespace Application.Common.Models.Groups
{
    public class GroupProvider : GroupBase<Provider>
    {
        private int m_IdProvider;
        private double m_PriceHt;
        private string m_ArticleNumber;

        public GroupProvider()
        {
            IDProvider = -1;
            PriceHT = 0.0;
            ArticleNumber = "";
        }

        public int IDProvider
        {
            get => m_IdProvider;
            set
            {
                var oldValue = m_IdProvider;
                m_IdProvider = value;
                OnPropertyChanged(nameof(IDProvider), oldValue, m_IdProvider);
            }
        }

        public double PriceHT
        {
            get => m_PriceHt;
            set
            {
                var oldValue = m_PriceHt;
                m_PriceHt = Math.Round(value, 2);
                OnPropertyChanged(nameof(PriceHT), oldValue, m_PriceHt);
            }
        }

        public string ArticleNumber
        {
            get => m_ArticleNumber;
            set
            {
                var oldValue = m_ArticleNumber;
                m_ArticleNumber = value;
                OnPropertyChanged(nameof(ArticleNumber), oldValue, m_ArticleNumber);
            }
        }

        public Currency Currency { get; set; }

        public override int ZIndex => 1000;

        public override object Clone()
        {
            GroupProvider groupProvider = (GroupProvider)MemberwiseClone();
            groupProvider.Item = (Provider)Item.Clone();
            return groupProvider;
        }

        public override void CopyTo(GroupBase<Provider> _Group)
        {
            ID = _Group.ID;

            if (_Group is GroupProvider groupProvider)
            {
                PriceHT = groupProvider.PriceHT;
                IDProvider = groupProvider.IDProvider;
                ArticleNumber = groupProvider.ArticleNumber;
            }
        }
    }
}
