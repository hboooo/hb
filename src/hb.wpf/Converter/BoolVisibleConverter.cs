using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace hb.wpf.Converter
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/25 16:35:44
    /// description:
    /// </summary>
    public class BoolVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var flag = false;
            if (value is bool)
            {
                flag = (bool)value;
            }
            else if (value is bool?)
            {
                var nullable = (bool?)value;
                flag = nullable.HasValue && nullable.Value;
            }
            return flag ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is Visibility) && ((Visibility)value) == Visibility.Visible;
        }
    }
}
