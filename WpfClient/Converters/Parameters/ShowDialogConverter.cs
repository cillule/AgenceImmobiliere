using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace Oyosoft.AgenceImmobiliere.WpfClient.Converters.Parameters
{
    public class ShowDialogConverter : IMultiValueConverter
    {
        // Paramètres à passer :
        //    0. Instance de la fenêtre courante (obl)
        //    1. Texte de la messagebox (obl)
        //    2. Titre de la messagebox (par défaut : titre de la fenêtre courante)
        //    3. Boutons de la messagebox (par défaut : OK)
        //    4. Image de la messagebox (par défaut : None)
        //    5. Bouton par défaut de la messagebox (par défaut : OK)
        //    6. Bouton attendu de la messagebox (par défaut : OK)

        public enum OpenMode
        {
            Modal = 0,
            NotModal = 1,
            CloseParent = 2
        }

        private Window _currentWindow;
        private string _text;
        private string _title;
        private MessageBoxButton _buttons;
        private MessageBoxImage _image;
        private MessageBoxResult _defaultResult;
        private MessageBoxResult _acceptResult;

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length < 2) return values;

            _currentWindow = (Window)values[0];
            _text = (string)values[1];

            _title = _currentWindow.Title;
            if (values.Length >= 3) _title = (string)values[2];

            _buttons = MessageBoxButton.OK;
            if (values.Length >= 4) _buttons = (MessageBoxButton)values[3];

            _image = MessageBoxImage.None;
            if (values.Length >= 5) _image = (MessageBoxImage)values[4];

            _defaultResult = MessageBoxResult.OK;
            if (values.Length >= 6) _defaultResult = (MessageBoxResult)values[5];

            _acceptResult = MessageBoxResult.OK;
            if (values.Length >= 7) _acceptResult = (MessageBoxResult)values[6];

            return new Func<bool>(ShowDialog);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }



        private bool ShowDialog()
        {
            MessageBoxResult result = MessageBox.Show(_currentWindow, _text, _title, _buttons, _image, _defaultResult);
            return result == _acceptResult;
        }
    }
}
