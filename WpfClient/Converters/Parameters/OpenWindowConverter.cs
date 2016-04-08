using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace Oyosoft.AgenceImmobiliere.WpfClient.Converters.Parameters
{
    public class OpenWindowConverter : IMultiValueConverter
    {

        // Paramètres à passer :
        //    0. Instance de la fenêtre courante (obl)
        //    1. Type de la fenêtre à ouvrir (obl)
        //    2. Mode d'ouverture de la fenêtre (par défaut : modal)
        //    3. Instance ou type du viewmodel associé à la fenêtre à ouvrir
        //    4 - ~. Paramètres à passer pour le constructeur du viewmodel


        public enum OpenMode
        {
            Modal = 0,
            NotModal = 1,
            CloseParent = 2
        }


        private Window _currentWindow;
        private Type _newWindowType;
        private OpenMode _mode;
        private Type _viewModelType;
        private object _viewModel;
        private List<object> _ctorParameters;

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length < 3) return values;

            _currentWindow = (Window)values[0];
            _newWindowType = (Type)values[1];
            if (!Enum.TryParse(values[2].ToString(), out _mode)) _mode = OpenMode.Modal;


            _viewModelType = null;
            _viewModel = null;
            if (values.Length >= 4)
            {
                if (values[3] != null)
                {
                    if (values[3].GetType() == typeof(Type))
                    {
                        _viewModelType = (Type)values[3];
                    }
                    else
                    {
                        _viewModel = values[3];
                    }
                }
                
            }

            if (values.Length > 4)
            {
                _ctorParameters = new List<object>();
                for (int i = 4; i < values.Length; i++)
                {
                    _ctorParameters.Add(values[i]);
                }
            }

            return new Core.Commands.Command(OpenWindow);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }



        private async Task OpenWindow()
        {
            ConstructorInfo ctor = _newWindowType.GetConstructor(new Type[] { });
            Window newWindow = (Window)ctor.Invoke(new object[] { });

            
            if (_viewModel == null && _viewModelType != null)
            {
                if (_ctorParameters.Count == 0)
                {
                    ctor = _viewModelType.GetConstructor(new Type[] { });
                    _viewModel = ctor.Invoke(new object[] { });
                }
                else
                {
                    List<Type> types = new List<Type>();
                    foreach (object param in _ctorParameters)
                    {
                        if (param == null)
                        {
                            types.Add(typeof(object));
                        }
                        else
                        {
                            types.Add(param.GetType());
                        }
                    }
                    ctor = _viewModelType.GetConstructor(types.ToArray());
                    _viewModel = ctor.Invoke(_ctorParameters.ToArray());
                }
            }
            if (_viewModel != null) newWindow.DataContext = _viewModel;

            switch (_mode)
            {
                case OpenMode.Modal:
                    newWindow.Owner = _currentWindow;
                    newWindow.ShowDialog();
                    return;
                case OpenMode.NotModal:
                    newWindow.Owner = _currentWindow;
                    newWindow.Show();
                    return;
                case OpenMode.CloseParent:
                    newWindow.Show();
                    _currentWindow.Close();
                    return;
            }

            newWindow = null;
        }
    }
}
