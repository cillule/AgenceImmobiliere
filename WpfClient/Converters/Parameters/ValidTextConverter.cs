using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Oyosoft.AgenceImmobiliere.Core.Commands;

namespace Oyosoft.AgenceImmobiliere.WpfClient.Converters.Parameters
{
    public class ValidTextConverter : IMultiValueConverter
    {
        private string _regex;

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length < 2) return values;

            //_currentWindow = (Window)values[0];
            //_text = (string)values[1];

            //_title = _currentWindow.Title;
            //if (values.Length >= 3) _title = (string)values[2];

            //_buttons = MessageBoxButton.OK;
            //if (values.Length >= 4) _buttons = (MessageBoxButton)values[3];

            //_image = MessageBoxImage.None;
            //if (values.Length >= 5) _image = (MessageBoxImage)values[4];

            //_defaultResult = MessageBoxResult.OK;
            //if (values.Length >= 6) _defaultResult = (MessageBoxResult)values[5];

            //_acceptResult = MessageBoxResult.OK;
            //if (values.Length >= 7) _acceptResult = (MessageBoxResult)values[6];

            return new EventBindingCommand<TextCompositionEventArgs>(ValidationTextBox);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        private async Task ValidationTextBox(EventBindingArgs<TextCompositionEventArgs> e)
        {
            Regex r = new Regex(_regex);
            e.EventArgs.Handled = r.IsMatch(e.EventArgs.Text);
        }

    }
}
