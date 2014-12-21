using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace CardGames.WPF
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.GetType() != typeof(bool) || targetType != typeof(Visibility))
                return Binding.DoNothing;
            return (bool)value ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.GetType() != typeof(Visibility) || targetType != typeof(bool))
                return Binding.DoNothing;
            return ((Visibility)value == Visibility.Visible);
        }
    }
}
