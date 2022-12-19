using Application.Common.Models.Articles;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ManageStock.Converters
{
    internal class StockActionToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "";
            }

            if (Enum.TryParse(value.ToString(), out EnumStockAction actionType))
            {
                switch (actionType)
                {
                    case EnumStockAction.Production:
                        return "Production";
                    case EnumStockAction.Resupply:
                        return "Restockage";
                    case EnumStockAction.Input:
                        return "Entrée";
                    case EnumStockAction.Output:
                        return "Sortie";
                }
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
