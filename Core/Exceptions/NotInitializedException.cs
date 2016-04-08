using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oyosoft.AgenceImmobiliere.Core.Exceptions
{
    public class NotInitializedException<T> : ExceptionWithPreviousMessages
    {
        private static string ERROR_MESSAGE = "Le type '" + typeof(T).FullName + "' n'est pas initialisé !";

        public NotInitializedException(Tools.ErrorsList previousErrors) : base(ERROR_MESSAGE, previousErrors) { }

        public NotInitializedException(Tools.ErrorsList previousErrors, System.Exception inner) : base(ERROR_MESSAGE, previousErrors, inner) { }

    }
}
