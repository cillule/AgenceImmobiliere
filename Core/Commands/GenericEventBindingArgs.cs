using System;

namespace Oyosoft.AgenceImmobiliere.Core.Commands
{
    public class EventBindingArgs<TEventArgs, TCommandParam> where TEventArgs : EventArgs
    {
        public object Sender { get; set; }
        public TEventArgs EventArgs { get; set; }
        public TCommandParam Parameter { get; set; }

        public EventBindingArgs(object sender, TEventArgs e, TCommandParam parameter)
        {
            Sender = sender;
            EventArgs = e;
            Parameter = parameter;
        }
    }

    public class EventBindingArgs<TEventArgs, TCommandParam1, TCommandParam2> where TEventArgs : EventArgs
    {
        public object Sender { get; set; }
        public TEventArgs EventArgs { get; set; }
        public TCommandParam1 Parameter1 { get; set; }
        public TCommandParam2 Parameter2 { get; set; }

        public EventBindingArgs(object sender, TEventArgs e, TCommandParam1 parameter1, TCommandParam2 parameter2)
        {
            Sender = sender;
            EventArgs = e;
            Parameter1 = parameter1;
            Parameter2 = parameter2;
        }
    }
}
