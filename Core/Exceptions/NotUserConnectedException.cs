using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oyosoft.AgenceImmobiliere.Core.Exceptions
{
    public class NotUserConnectedException : ExceptionWithPreviousMessages
    {
        private static string ERROR_MESSAGE = "Aucun utilisateur n'est connecté à la base de données !";

        public NotUserConnectedException(Tools.ErrorsList previousErrors) : base(ERROR_MESSAGE, previousErrors) { }

        public NotUserConnectedException(Tools.ErrorsList previousErrors, System.Exception inner) : base(ERROR_MESSAGE, previousErrors, inner) { }
    }
}
