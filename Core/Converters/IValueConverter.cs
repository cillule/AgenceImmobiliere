using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oyosoft.AgenceImmobiliere.Core.Converters
{
    public interface IValueConverter
    {
        object Convert(object value, Type targetType, object parameter);
        object ConvertBack(object value, Type targetType, object parameter);
    }
}
