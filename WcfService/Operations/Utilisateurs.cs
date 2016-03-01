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
    internal static class Utilisateurs
    {
        internal async static Task<OperationResult> ConnecterUtilisateur(Connection connection, string nomUtilisateur, params string[] parametres)
        {
            OperationResult resultat = new OperationResult();

            // Chargement des paramètres
            string motDePasseCrypte = null;
            if (parametres.Length > 0) motDePasseCrypte = parametres[0];

            if (nomUtilisateur == null)
            {
                Manager.ReadURIParameter("nomUtilisateur", out nomUtilisateur);
            }

            if (motDePasseCrypte == null)
            {
                Manager.ReadURIParameter("motDePasseCrypte", out motDePasseCrypte);
            }

            // Connexion
            ErrorsList errors = new ErrorsList();
            resultat.Success = await Core.ViewModels.BaseViewModel<Utilisateur, Core.ViewModels.Utilisateur.SearchCriteria>.ConnectUser(connection, errors, nomUtilisateur, motDePasseCrypte);
            if (!errors.IsEmpty)
            {
                await Manager.ManageError(resultat, errors);
                return resultat;
            }

            return resultat;
        }

        internal async static Task<OperationResult> DeconnecterUtilisateur(Connection connection, string nomUtilisateur, params object[] nonUtilise)
        {
            OperationResult resultat = new OperationResult();

            // Chargement des paramètres
            if (nomUtilisateur == null)
            {
                Manager.ReadURIParameter("nomUtilisateur", out nomUtilisateur);
            }

            // Déconnexion
            ErrorsList errors = new ErrorsList();
            resultat.Success = await Core.ViewModels.BaseViewModel<Utilisateur, Core.ViewModels.Utilisateur.SearchCriteria>.DisconnectUser(connection, errors, nomUtilisateur);
            if (!errors.IsEmpty)
            {
                await Manager.ManageError(resultat, errors);
                return resultat;
            }

            return resultat;
        }
    }
}
