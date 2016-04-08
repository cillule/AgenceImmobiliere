using System;
using Windows.UI.Xaml.Data;

namespace Oyosoft.AgenceImmobiliere.UniversalAppWin10.Converters
{
    public class ObjectToStringConverter : Core.Converters.ObjectToStringConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return base.Convert(value, targetType, parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return base.ConvertBack(value, targetType, parameter);
        }
    }
}
