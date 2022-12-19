using System;
using System.Windows;
using System.Windows.Controls;
using static ManageStock.ViewModels.HistoryViewModel;

namespace ManageStock.Templates
{
    public class FilterHistoryTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DateTemplate { get; set; }

        public DataTemplate ActionTypeTemplate { get; set; }

        public DataTemplate EmptyTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if(item == null)
            {
                return EmptyTemplate;
            }

            if (Enum.TryParse(item.ToString(), out EnumFilteredHistory actionType))
            {
                switch (actionType)
                {
                    case EnumFilteredHistory.Date:
                        return DateTemplate;
                    case EnumFilteredHistory.StockAction:
                        return ActionTypeTemplate;
                }
            }

            return EmptyTemplate;
        }
    }
}
