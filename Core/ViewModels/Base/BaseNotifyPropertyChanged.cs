using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Oyosoft.AgenceImmobiliere.Core.ViewModels
{
    [DataContract]
    public abstract class BaseNotifyPropertyChanged : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            return SetProperty(this, this.PropertyChanged, ref field, value, propertyName);
        }
        internal protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(this, this.PropertyChanged, propertyName);
        }


        protected static bool SetProperty<T>(object sender, PropertyChangedEventHandler propertyChanged, ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;

            field = value;
            OnPropertyChanged(sender, propertyChanged, propertyName);

            return true;
        }
        protected static void OnPropertyChanged(object sender, PropertyChangedEventHandler propertyChanged, [CallerMemberName] string propertyName = null)
        {
            if (propertyChanged != null)
            {
                propertyChanged(sender, new PropertyChangedEventArgs(propertyName));

                //if (SynchronizationContext.Current == null)
                //{
                //    propertyChanged(sender, new PropertyChangedEventArgs(propertyName));
                //}
                //else
                //{
                //    SynchronizationContext.Current.Send(
                //            (obj) =>
                //            {
                //                propertyChanged(sender, new PropertyChangedEventArgs(propertyName));
                //            },
                //            null);
                //}

            }
        }


    }

}
