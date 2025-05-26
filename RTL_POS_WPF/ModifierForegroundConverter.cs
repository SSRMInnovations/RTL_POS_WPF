using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RTL_POS_WPF
{
    public class ModifierForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value as string;
            return string.IsNullOrWhiteSpace(str)
                ? new SolidColorBrush(Colors.Gray)
                : new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
