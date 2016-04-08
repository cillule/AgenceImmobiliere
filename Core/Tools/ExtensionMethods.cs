using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Oyosoft.AgenceImmobiliere.Core.ViewModels;

namespace Oyosoft.AgenceImmobiliere.Core.Tools
{
    public static class ExtensionMethods
    {
        public static void ObservableCollection<T>(this ObservableCollection<T> col)
        {
            col = new System.Collections.ObjectModel.ObservableCollection<T>();
        }
        public static void AddRange<T>(this ObservableCollection<T> destination, ObservableCollection<T> items)
        {
            if (items == null) return;
            foreach (T item in items)
            {
                destination.Add(item);
            }
        }

        public static bool Contains<T>(this List<T> col, Func<T, bool> predicate)
        {
            foreach (T item in col)
            {
                if (predicate(item)) return true;
            }
            return false;
        }

        public static void ExecuteSynchronously(this Task task)
        {
            var ta = task.GetAwaiter();
            ta.GetResult();
        }
        public static T ExecuteSynchronously<T>(this Task<T> task)
        {
            var ta = task.GetAwaiter();
            return ta.GetResult();
        }



    }
}
