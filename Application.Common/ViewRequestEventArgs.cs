using Application.Common.Models.Items;
using System;

namespace Application.Common
{
    public class ViewRequestEventArgs : EventArgs
    {
        public ViewRequestEventArgs(ItemBase _Item, Type _ViewType)
        {
            Item = _Item;
            ViewType = _ViewType;
        }

        public ItemBase Item { get; set; }

        public Type ViewType { get; set; }
    }
}
