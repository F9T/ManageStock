using Application.Common.ViewModels;
using ManageStock.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace ManageStock.Templates
{
    internal class ViewModelTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ArticlesTemplate { get; set; }

        public DataTemplate ProviderTemplate { get; set; }

        public DataTemplate HistoryTemplate { get; set; }

        public DataTemplate CurrencyTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ViewModelBase viewModel)
            {
                switch (viewModel.TemplateName)
                {
                    case "provider":
                        return ProviderTemplate;
                    case "articles":
                        return ArticlesTemplate;
                    case "history":
                        return HistoryTemplate;
                    case "currency":
                        return CurrencyTemplate;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}
