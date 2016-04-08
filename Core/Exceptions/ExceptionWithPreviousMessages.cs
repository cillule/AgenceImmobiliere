using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oyosoft.AgenceImmobiliere.Core.Exceptions
{
    public abstract class ExceptionWithPreviousMessages : Exception
    {
        private Tools.ErrorsList _previousErrors;
        public Tools.ErrorsList PreviousErrors
        {
            get { return _previousErrors; }
        }

        protected ExceptionWithPreviousMessages(string message, Tools.ErrorsList previousErrors) : base(GetMessage(message, previousErrors))
        {
            this._previousErrors = new Tools.ErrorsList();
            this._previousErrors.AddRange(previousErrors);
        }

        protected ExceptionWithPreviousMessages(string message, Tools.ErrorsList previousErrors, System.Exception inner) : base(GetMessage(message, previousErrors), inner)
        {
            this._previousErrors = new Tools.ErrorsList();
            this._previousErrors.AddRange(previousErrors);
        }

        private static string GetMessage(string message, Tools.ErrorsList previousErrors)
        {
            if (!previousErrors.IsEmpty) message += "\n\nMessages précédents :\n" + previousErrors.ToString();
            return message;
        }
    }
}
