using Application.Common.Models.Articles;
using Application.Common.Models.Items;
using System;

namespace Application.Common.Models.History
{
    public class History : ItemBase
    {
        public int ArticleID { get; set; }

        public DateTime Date { get; set; }

        public double Quantity { get; set; }

        public double Balance { get; set; }

        public EnumStockAction ActionType { get; set; }

        public override int ZIndex => 10;

        public override object Clone()
        {
            return (History)MemberwiseClone();
        }

        public override void Default()
        {
            Date = DateTime.Now;
        }
    }
}
