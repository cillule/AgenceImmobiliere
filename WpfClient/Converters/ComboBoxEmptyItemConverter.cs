using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Dynamic;

namespace Oyosoft.AgenceImmobiliere.WpfClient.Converters
{
    class ComboBoxEmptyItemConverter : IValueConverter
    {
        internal class EmptyItem
        {
            public override string ToString()
            {
                return "[vide]";
            }
        }
        private static EmptyItem _item = null;
        internal static EmptyItem Item
        {
            get {
                if (_item == null) _item = new EmptyItem();
                return _item;
            }
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // assume that the value at least inherits from IEnumerable 
            // otherwise we cannot use it. 
            System.Collections.IEnumerable container = value as System.Collections.IEnumerable;

            if (container != null)
            {
                // everything inherits from object, so we can safely create a generic IEnumerable 
                IEnumerable<object> genericContainer = container.OfType<object>();
                // create an array with a single EmptyItem object that serves to show en empty line 
                IEnumerable<object> emptyItem = new object[] { Item };
                // use Linq to concatenate the two enumerable 
                return emptyItem.Concat(genericContainer);
            }

            if (value == null) return Item;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is EmptyItem) return null;
            return value;
        }
    }
}
