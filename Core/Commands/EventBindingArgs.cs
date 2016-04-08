using System;

namespace Oyosoft.AgenceImmobiliere.Core.Commands
{
    public class EventBindingArgs<TEventArgs> where TEventArgs : EventArgs
    {
        public object Sender { get; set; }
        public TEventArgs EventArgs { get; set; }

        public EventBindingArgs(object sender, TEventArgs e)
        {
            Sender = sender;
            EventArgs = e;
        }
    }
}
