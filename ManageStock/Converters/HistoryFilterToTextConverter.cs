using System;
using System.Globalization;
using System.Windows.Data;
using static ManageStock.ViewModels.HistoryViewModel;

namespace ManageStock.Converters
{
    internal class HistoryFilterToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null)
            {
                return "";
            }

            if(Enum.TryParse(value.ToString(), out EnumFilteredHistory filterHistory))
            {
                switch (filterHistory)
                {
                    case EnumFilteredHistory.None:
                        return "Aucun";
                    case EnumFilteredHistory.Date:
                        return "Par date";
                    case EnumFilteredHistory.StockAction:
                        return "Par type";
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
