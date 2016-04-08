﻿using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Oyosoft.AgenceImmobiliere.Core.Tools;

namespace Oyosoft.AgenceImmobiliere.Core.Commands
{
    public class Command<T> : ICommand
    {
        private readonly Func<T, Task> _execute;
        private readonly Func<T, bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public Command(Func<T, Task> execute) : this(execute, null) { }

        public Command(Func<T, Task> execute, Func<T, bool> canExecute)
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
            return _canExecute == null || _canExecute((T)parameter);
        }

        //public virtual async void Execute(object parameter)
        //{
        //    if (CanExecute(parameter) && _execute != null)
        //    {
        //        T param = default(T);
        //        try
        //        {
        //            param = (T)parameter;
        //        }
        //        catch { }

        //        _execute(param);
        //    }
        //}
        public virtual void Execute(object parameter)
        {
            if (CanExecute(parameter) && _execute != null)
            {
                T param = GetParameter(parameter);
                _execute(param);
            }
        }
        public virtual async Task ExecuteAsync(object parameter)
        {
            if (CanExecute(parameter) && _execute != null)
            {
                T param = GetParameter(parameter);
                await _execute(param);
            }
        }

        private T GetParameter(object parameter)
        {
            T param = default(T);
            try
            {
                param = (T)parameter;
            }
            catch { }
            return param;
        }
    }

    public class Command<T1, T2> : ICommand
    {
        private readonly Func<T1, T2, Task> _execute;
        private readonly Func<T1, T2, bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public Command(Func<T1, T2, Task> execute) : this(execute, null) { }

        public Command(Func<T1, T2, Task> execute, Func<T1, T2, bool> canExecute)
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
            T1 param1;
            T2 param2;
            GetParameters(parameter, out param1, out param2);

            return _canExecute == null || _canExecute(param1, param2);
        }

        //public virtual async void Execute(object parameter)
        //{
        //    if (CanExecute(parameter) && _execute != null)
        //    {
        //        T1 param1;
        //        T2 param2;
        //        GetParameters(parameter, out param1, out param2);
        //        _execute(param1, param2);
        //    }
        //}
        public virtual void Execute(object parameter)
        {
            T1 param1;
            T2 param2;
            GetParameters(parameter, out param1, out param2);
            _execute(param1, param2);
        }
        public virtual async Task ExecuteAsync(object parameter)
        {
            if (CanExecute(parameter) && _execute != null)
            {
                T1 param1;
                T2 param2;
                GetParameters(parameter, out param1, out param2);
                await _execute(param1, param2);
            }
        }


        private void GetParameters(object parameter, out T1 param1, out T2 param2)
        {
            param1 = default(T1);
            param2 = default(T2);

            if (parameter == null || parameter.GetType() != typeof(object[]) || ((object[])parameter).Length != 2) return;

            param1 = (T1)((object[])parameter)[0];
            param2 = (T2)((object[])parameter)[1];
        }
    }
}
