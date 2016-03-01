using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Oyosoft.AgenceImmobiliere.Core.Commands
{
    public class Command : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public Command(Func<Task> execute) : this(execute, null) { }

        public Command(Func<Task> execute, Func<bool> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            _execute = execute;
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

        public virtual async void Execute(object parameter)
        {
            if (CanExecute(parameter) && _execute != null)
            {
                _execute();
            }
        }
    }
}
