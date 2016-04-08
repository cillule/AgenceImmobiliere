using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;

namespace Oyosoft.AgenceImmobiliere.Core.Service
{
    [ServiceContract(Name = "AgenceContract", Namespace = "Oyosoft.AgenceImmobiliere.Core.Service")]
    public interface IContract
    {
        // Test de connexion
        [OperationContract]
        OperationResult TesterConnexionService();

        // Biens immobiliers
        [OperationContract]
        OperationResult<DataAccess.SearchResult<Model.BienImmobilier>> LireListeBiensImmobiliers(ViewModels.BienImmobilier.SearchCriteria criteres, long? currentPage, long? itemsCountOnPage);
        [OperationContract]
        OperationResult<Model.BienImmobilier, ObservableCollection<Model.PhotoBienImmobilier>> LireDetailsBienImmobilier(string id);
        [OperationContract]
        OperationResult AjouterBienImmobilier(Model.BienImmobilier bien, ICollection<Model.PhotoBienImmobilier> photos);
        [OperationContract]
        OperationResult ModifierBienImmobilier(Model.BienImmobilier bien, ICollection<Model.PhotoBienImmobilier> photos);
        [OperationContract]
        OperationResult SupprimerBienImmobilier(string id);

        // Clients
        [OperationContract]
        OperationResult<DataAccess.SearchResult<Model.Personne>> LireListeClients(ViewModels.Client.SearchCriteria criteres, long? currentPage, long? itemsCountOnPage);
        [OperationContract]
        OperationResult<Model.Personne> LireDetailsClient(string id);
        [OperationContract]
        OperationResult AjouterClient(Model.Personne client);
        [OperationContract]
        OperationResult ModifierClient(Model.Personne client);
        [OperationContract]
        OperationResult SupprimerClient(string id);

        // Utilisateurs
        [OperationContract]
        OperationResult<DataAccess.SearchResult<Model.Utilisateur>> LireListeUtilisateurs(ViewModels.Utilisateur.SearchCriteria criteres, long? currentPage, long? itemsCountOnPage);
        [OperationContract]
        OperationResult<Model.Utilisateur, Model.Personne> LireDetailsUtilisateur(string nomUtilisateur);
        [OperationContract]
        OperationResult AjouterUtilisateur(Model.Utilisateur utilisateur);
        [OperationContract]
        OperationResult ModifierUtilisateur(Model.Utilisateur utilisateur);
        [OperationContract]
        OperationResult SupprimerUtilisateur(string nomUtilisateur);
        [OperationContract]
        OperationResult ConnecterUtilisateur(string nomUtilisateur, string motDePasseCrypte);
        [OperationContract]
        OperationResult DeconnecterUtilisateur(string nomUtilisateur);

        // Log
        [OperationContract]
        StringOperationResult LireContenuLog();

        // Synchro
        [OperationContract]
        OperationResult<DataAccess.SynchroList> LireElementsASynchroniser(DateTime? date = null);
        [OperationContract]
        OperationResult SynchroniserElements(DataAccess.SynchroList elements);

    }
}
