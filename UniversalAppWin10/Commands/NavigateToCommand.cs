using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Oyosoft.AgenceImmobiliere.UniversalAppWin10.Commands
{
    public class NavigateToCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly Type _nextPageType;

        public NavigateToCommand(Type nextPageType)
        {
            if (nextPageType == null)
            {
                throw new ArgumentNullException("nextPageType");
            }
            _nextPageType = nextPageType;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (CanExecute(parameter) && _nextPageType != null)
            {
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(_nextPageType);
            }
        }
    }
}
