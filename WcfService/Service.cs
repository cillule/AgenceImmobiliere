using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Oyosoft.AgenceImmobiliere.Core.DataAccess;
using Oyosoft.AgenceImmobiliere.Core.Model;
using Oyosoft.AgenceImmobiliere.Core.Service;

namespace Oyosoft.AgenceImmobiliere.WcfService
{
    internal class Service : IContract
    {
        // Test de connexion
        public OperationResult TesterConnexionService()
        {
            OperationResult resultat = new OperationResult();
            resultat.Success = true;
            return resultat;
        }

        // Biens immobiliers
        public OperationResult<SearchResult<BienImmobilier>> LireListeBiensImmobiliers(Core.ViewModels.BienImmobilier.SearchCriteria criteres, long? currentPage, long? itemsCountOnPage)
        {
            return Operations.Manager.ExecuteOperationSynchronously(Operations.BiensImmobiliers.LireListeBiensImmobiliers, criteres, currentPage, itemsCountOnPage);
        }
        public OperationResult<BienImmobilier, ObservableCollection<PhotoBienImmobilier>> LireDetailsBienImmobilier(string id)
        {
            return Operations.Manager.ExecuteOperationSynchronously(Operations.BiensImmobiliers.LireDetailsBienImmobilier, id, new object());
        }
        public OperationResult AjouterBienImmobilier(BienImmobilier bien, ICollection<PhotoBienImmobilier> photos)
        {
            return Operations.Manager.ExecuteOperationSynchronously(Operations.BiensImmobiliers.AjouterBienImmobilier, bien, photos);
        }
        public OperationResult ModifierBienImmobilier(BienImmobilier bien, ICollection<PhotoBienImmobilier> photos)
        {
            return Operations.Manager.ExecuteOperationSynchronously(Operations.BiensImmobiliers.ModifierBienImmobilier, bien, photos);
        }
        public OperationResult SupprimerBienImmobilier(string id)
        {
            return Operations.Manager.ExecuteOperationSynchronously(Operations.BiensImmobiliers.SupprimerBienImmobilier, id, new object());
        }

        // Clients
        public OperationResult<SearchResult<Personne>> LireListeClients(Core.ViewModels.Client.SearchCriteria criteres, long? currentPage, long? itemsCountOnPage)
        {
            throw new NotImplementedException();
        }
        public OperationResult<Personne> LireDetailsClient(string id)
        {
            throw new NotImplementedException();
        }
        public OperationResult AjouterClient(Personne client)
        {
            throw new NotImplementedException();
        }
        public OperationResult ModifierClient(Personne client)
        {
            throw new NotImplementedException();
        }
        public OperationResult SupprimerClient(string id)
        {
            throw new NotImplementedException();
        }

        // Utilisateurs
        public OperationResult<SearchResult<Utilisateur>> LireListeUtilisateurs(Core.ViewModels.Utilisateur.SearchCriteria criteres, long? currentPage, long? itemsCountOnPage)
        {
            throw new NotImplementedException();
        }
        public OperationResult<Utilisateur, Personne> LireDetailsUtilisateur(string nomUtilisateur)
        {
            throw new NotImplementedException();
        }
        public OperationResult AjouterUtilisateur(Utilisateur utilisateur)
        {
            throw new NotImplementedException();
        }
        public OperationResult ModifierUtilisateur(Utilisateur utilisateur)
        {
            throw new NotImplementedException();
        }
        public OperationResult SupprimerUtilisateur(string nomUtilisateur)
        {
            throw new NotImplementedException();
        }
        public OperationResult ConnecterUtilisateur(string nomUtilisateur, string motDePasseCrypte)
        {
            return Operations.Manager.ExecuteOperationSynchronously(Operations.Utilisateurs.ConnecterUtilisateur, nomUtilisateur, motDePasseCrypte);
        }
        public OperationResult DeconnecterUtilisateur(string nomUtilisateur)
        {
            return Operations.Manager.ExecuteOperationSynchronously(Operations.Utilisateurs.DeconnecterUtilisateur, nomUtilisateur, new object());
        }

        // Log
        public StringOperationResult LireContenuLog()
        {
            throw new NotImplementedException();
        }

        // Synchro
        public OperationResult<SynchroList> LireElementsASynchroniser(DateTime? date = default(DateTime?))
        {
            return Operations.Manager.ExecuteOperationSynchronously(Operations.Synchro.LireElementsASynchroniser, date, new object());
        }
        public OperationResult SynchroniserElements(SynchroList elements)
        {
            return Operations.Manager.ExecuteOperationSynchronously(Operations.Synchro.SynchroniserElements, elements, new object());
        }

        
    }
}
