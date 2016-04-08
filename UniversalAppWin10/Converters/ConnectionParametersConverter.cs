using System;
using Windows.UI.Xaml.Data;

namespace Oyosoft.AgenceImmobiliere.UniversalAppWin10.Converters
{
    public class ConnectionParametersConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return new Commands.NavigateToCommand(typeof(ListeBiensPage));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
