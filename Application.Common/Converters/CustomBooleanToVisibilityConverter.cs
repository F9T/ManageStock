using System.Globalization;
using System;
using System.Windows.Data;
using System.Windows;

namespace Application.Common.Converters
{
    public class CustomBooleanToVisibilityConverter : IValueConverter
    {
        public Visibility VisibilityOnFalse { get; set; } = Visibility.Collapsed;

        public Visibility VisibilityOnTrue { get; set; } = Visibility.Visible;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return VisibilityOnFalse;
            }

            if (bool.TryParse(value.ToString(), out bool v))
            {
                return v ? VisibilityOnTrue : VisibilityOnFalse;
            }

            return VisibilityOnFalse;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
