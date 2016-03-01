using System;
using System.Threading.Tasks;

namespace Oyosoft.AgenceImmobiliere.Core.Commands
{
    public class EventBindingCommand<TEventArgs> : Command<EventBindingArgs<TEventArgs>> where TEventArgs : EventArgs
    {
        public EventBindingCommand(Func<EventBindingArgs<TEventArgs>, Task> execute) : base(execute, null) { }

        public EventBindingCommand(Func<EventBindingArgs<TEventArgs>, Task> execute, Func<EventBindingArgs<TEventArgs>, bool> canExecute) : base(execute, null) { }
    }
}
