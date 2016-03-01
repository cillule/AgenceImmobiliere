using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oyosoft.AgenceImmobiliere.Core.DataAccess;
using Oyosoft.AgenceImmobiliere.Core.Model;
using Oyosoft.AgenceImmobiliere.Core.Service;
using Oyosoft.AgenceImmobiliere.Core.Tools;

namespace Oyosoft.AgenceImmobiliere.WcfService.Operations
{
    internal static class Synchro
    {
        internal async static Task<OperationResult<SynchroList>> LireElementsASynchroniser(Connection connection, DateTime? date = null, params object[] nonUtilise)
        {
            OperationResult<SynchroList> resultat = new OperationResult<SynchroList>();

            // Chargement des paramètres
            if (date == null)
            {
                Manager.ReadURIParameter("date", out date);
            }
            
            // Lecture de la liste des éléments à synchroniser
            ErrorsList errors = new ErrorsList();
            resultat.Result = await connection.SelectSynchronize(date);
            errors.AddRange(connection.Errors);
            if (resultat.Result == null || !errors.IsEmpty)
            {
                await Manager.ManageError(resultat, errors);
                return resultat;
            }

            resultat.Success = resultat.Errors.IsEmpty;

            return resultat;
        }
        internal async static Task<OperationResult> SynchroniserElements(Connection connection, SynchroList elements, params object[] nonUtilise)
        {
            OperationResult resultat = new OperationResult();

            // Chargement des paramètres
            if (elements == null)
            {
                Manager.ReadURIParameter("elements", out elements);
            }

            // Exécution de la synchro
            ErrorsList errors = new ErrorsList();
            await connection.Synchronize(elements, false);
            errors.AddRange(connection.Errors);
            if (!errors.IsEmpty)
            {
                await Manager.ManageError(resultat, errors);
                return resultat;
            }
            
            resultat.Success = resultat.Errors.IsEmpty;

            return resultat;
        }
    }
}
