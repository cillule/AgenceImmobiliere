using System;
using Windows.UI.Xaml.Data;

namespace Oyosoft.AgenceImmobiliere.UniversalAppWin10.Converters
{
    public class NumericToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int num;
            bool bValue = false;
            if (value != null && int.TryParse(value.ToString(), out num)) bValue = num > 0;

            BoolToVisibilityConverter conv2 = new BoolToVisibilityConverter();
            return conv2.Convert(bValue, targetType, parameter, language);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
