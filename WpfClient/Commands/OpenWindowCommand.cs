using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Oyosoft.AgenceImmobiliere.WpfClient.Commands
{
    public class OpenWindowCommand : ICommand
    {
        public enum OpenMode
        {
            Modal = 0,
            NotModal = 1,
            CloseParent = 2
        }

        private readonly Window _currentWindow;
        private readonly Type _newWindowType;
        private readonly OpenMode _mode;
        private readonly Func<bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public OpenWindowCommand(Window currentWindow, Type newWindowType, OpenMode mode) : this(currentWindow, newWindowType, mode, null) { }

        public OpenWindowCommand(Window currentWindow, Type newWindowType, OpenMode mode, Func<bool> canExecute)
        {
            if (currentWindow == null)
            {
                throw new ArgumentNullException("currentWindow");
            }
            if (newWindowType == null)
            {
                throw new ArgumentNullException("newWindowType");
            }

            _currentWindow = currentWindow;
            _newWindowType = newWindowType;
            _mode = mode;
            _canExecute = canExecute;
        }


        public void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public virtual void Execute(object parameter)
        {
            if (CanExecute(parameter) && _newWindowType != null)
            {
                ConstructorInfo ctor = _newWindowType.GetConstructor(new Type[] { });
                Window newWindow = (Window)ctor.Invoke(new object[] { });
                
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
}
