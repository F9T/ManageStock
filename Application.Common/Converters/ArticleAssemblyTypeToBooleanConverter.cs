using Application.Common.Models.Articles;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Application.Common.Converters
{
    public class ArticleAssemblyTypeToBooleanConverter : IValueConverter
    {
        public EnumArticleAssemblyType TrueValue { get; set; } = EnumArticleAssemblyType.ProductInAdvance;

        public EnumArticleAssemblyType FalseValue { get; set; } = EnumArticleAssemblyType.ProductAtTheOutput;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) 
            { 
                return false; 
            }

            if (Enum.TryParse(value.ToString(), out EnumArticleAssemblyType assemblyType))
            {
                return assemblyType == TrueValue;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return FalseValue;
            }

            if (bool.TryParse(value.ToString(), out bool productInAdvance))
            {
                return productInAdvance ? TrueValue : FalseValue;
            }

            return FalseValue;
        }
    }
}
