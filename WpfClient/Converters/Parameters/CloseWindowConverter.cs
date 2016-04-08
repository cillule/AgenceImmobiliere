using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace Oyosoft.AgenceImmobiliere.WpfClient.Converters.Parameters
{
    public class CloseWindowConverter : IMultiValueConverter
    {
        // Paramètres à passer :
        //    0. Instance de la fenêtre courante (obl)
        //    1. Résultat de la fenêtre courante (obl)
        //    2. Nom de la propriété de la fenêtre parente à affecter si le résultat == true et qu'une fenêtre parente existe
        //    3. Instance à affecter à la propriété nommée de la fenêtre parente si un nom de propriété est fourni, si le résultat == true et si une fenêtre parente existe

        private Window _currentWindow;
        private bool? _currentWindowResult;
        private string _affectPropertyName;
        private object _affectValue;

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length < 2) return values;

            _currentWindow = (Window)values[0];
            _currentWindowResult = (bool?)values[1];

            _affectPropertyName = null;
            _affectValue = null;
            if (values.Length>=4)
            {
                _affectPropertyName = (string)values[2]; ;
                _affectValue = values[3];
            }

            return new Core.Commands.Command(CloseWindow);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }



        private async Task CloseWindow()
        {
            Window parent = _currentWindow.Owner;

            _currentWindow.DialogResult = _currentWindowResult;
            _currentWindow.Close();

            if (parent != null && !string.IsNullOrEmpty(_affectPropertyName) && _currentWindowResult != null && (bool)_currentWindowResult)
            {
                PropertyInfo prop = parent.GetType().GetProperty(_affectPropertyName);
                if (prop != null)
                {
                    prop.SetValue(parent, _affectValue);
                }
            }

        }
    }
}
