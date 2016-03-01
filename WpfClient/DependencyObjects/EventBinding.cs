using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Oyosoft.AgenceImmobiliere.Core.Commands;

namespace Oyosoft.AgenceImmobiliere.WpfClient.DependencyObjects
{
    public class EventBinding : DependencyObject
    {

        public static readonly DependencyProperty EventNameProperty = DependencyProperty.RegisterAttached("EventName", typeof(string), typeof(EventHandler));
        public static string GetEventName(DependencyObject obj)
        {
            return (string)obj.GetValue(EventNameProperty);
        }
        public static void SetEventName(DependencyObject obj, string value)
        {
            obj.SetValue(EventNameProperty, value);
            var eventInfo = obj.GetType().GetEvent(value);
            var eventHandlerType = eventInfo.EventHandlerType;
            var eventHandlerMethod = typeof(EventBinding).GetMethod("EventHandlerMethod", BindingFlags.Static | BindingFlags.NonPublic);
            var eventHandlerParameters = eventHandlerType.GetMethod("Invoke").GetParameters();
            var eventArgsParameterType = eventHandlerParameters.Where(p => typeof(EventArgs).IsAssignableFrom(p.ParameterType)).Single().ParameterType;
            eventHandlerMethod = eventHandlerMethod.MakeGenericMethod(eventArgsParameterType);
            eventInfo.AddEventHandler(obj, Delegate.CreateDelegate(eventHandlerType, eventHandlerMethod));
        }

        private static void EventHandlerMethod<TEventArgs>(object sender, TEventArgs e) where TEventArgs : EventArgs
        {
            var command = GetCommand(sender as DependencyObject);
            var parameter = GetCommandParameter(sender as DependencyObject);

            if (parameter == null)
            {
                command.Execute(new EventBindingArgs<TEventArgs>(sender, e));
            }
            else
            {
                var method = typeof(EventBinding).GetMethod("GetEventBindingArgsInstance");
                var genmethod = method.MakeGenericMethod(typeof(TEventArgs), parameter.GetType());
                object[] args = { sender, e, parameter };
                command.Execute(genmethod.Invoke(null, args));
            }
        }
        private static EventBindingArgs<TEventArgs, TCommandParam> GetEventBindingArgsInstance<TEventArgs, TCommandParam>(object sender, TEventArgs e, TCommandParam parameter) where TEventArgs : EventArgs
        {
            return new EventBindingArgs<TEventArgs, TCommandParam>(sender, e, parameter);
        }
        

        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(EventBinding));
        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }
        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }


        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(EventBinding));
        public static object GetCommandParameter(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandParameterProperty);
        }
        public static void SetCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(CommandParameterProperty, value);
        }

    }
}
