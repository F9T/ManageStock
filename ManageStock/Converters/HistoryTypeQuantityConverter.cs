using Application.Common.Models.Articles;
using Application.Common.Models.History;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ManageStock.Converters
{
    internal class HistoryTypeQuantityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null || parameter == null)
            {
                return "";
            }

            if(value is History history && Enum.TryParse(parameter.ToString(), out EnumStockAction actionType))
            {
                if(history.ActionType == actionType)
                {
                    return history.Quantity;
                }

                return "";
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
