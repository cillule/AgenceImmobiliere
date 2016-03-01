using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Oyosoft.AgenceImmobiliere.Core.Service
{
    [ServiceContract(Name = "AgenceContract", Namespace = "Oyosoft.AgenceImmobiliere.Core.Service")]
    public interface IContractAsync : IContract
    {
        // Test de connexion
        [OperationContract]
        Task<OperationResult> TesterConnexionServiceAsync();

        // Biens immobiliers
        [OperationContract]
        Task<OperationResult<DataAccess.SearchResult<Model.BienImmobilier>>> LireListeBiensImmobiliersAsync(ViewModels.BienImmobilier.SearchCriteria criteres, long? currentPage, long? itemsCountOnPage);
        [OperationContract]
        Task<OperationResult<Model.BienImmobilier, ObservableCollection<Model.PhotoBienImmobilier>>> LireDetailsBienImmobilierAsync(string id);
        [OperationContract]
        Task<OperationResult> AjouterBienImmobilierAsync(Model.BienImmobilier bien, ICollection<Model.PhotoBienImmobilier> photos);
        [OperationContract]
        Task<OperationResult> ModifierBienImmobilierAsync(Model.BienImmobilier bien, ICollection<Model.PhotoBienImmobilier> photos);
        [OperationContract]
        Task<OperationResult> SupprimerBienImmobilierAsync(string id);

        // Clients
        [OperationContract]
        Task<OperationResult<DataAccess.SearchResult<Model.Personne>>> LireListeClientsAsync(ViewModels.Client.SearchCriteria criteres, long? currentPage, long? itemsCountOnPage);
        [OperationContract]
        Task<OperationResult<Model.Personne>> LireDetailsClientAsync(string id);
        [OperationContract]
        Task<OperationResult> AjouterClientAsync(Model.Personne client);
        [OperationContract]
        Task<OperationResult> ModifierClientAsync(Model.Personne client);
        [OperationContract]
        Task<OperationResult> SupprimerClientAsync(string id);

        // Utilisateurs
        [OperationContract]
        Task<OperationResult<DataAccess.SearchResult<Model.Utilisateur>>> LireListeUtilisateursAsync(ViewModels.Utilisateur.SearchCriteria criteres, long? currentPage, long? itemsCountOnPage);
        [OperationContract]
        Task<OperationResult<Model.Utilisateur, Model.Personne>> LireDetailsUtilisateurAsync(string nomUtilisateur);
        [OperationContract]
        Task<OperationResult> AjouterUtilisateurAsync(Model.Utilisateur utilisateur);
        [OperationContract]
        Task<OperationResult> ModifierUtilisateurAsync(Model.Utilisateur utilisateur);
        [OperationContract]
        Task<OperationResult> SupprimerUtilisateurAsync(string nomUtilisateur);
        [OperationContract]
        Task<OperationResult> ConnecterUtilisateurAsync(string nomUtilisateur, string motDePasseCrypte);
        [OperationContract]
        Task<OperationResult> DeconnecterUtilisateurAsync(string nomUtilisateur);

        // Log
        [OperationContract]
        Task<StringOperationResult> LireContenuLogAsync();

        // Synchro
        [OperationContract]
        Task<OperationResult<DataAccess.SynchroList>> LireElementsASynchroniserAsync(DateTime? date = null);
        [OperationContract]
        Task<OperationResult> SynchroniserElementsAsync(DataAccess.SynchroList elements);

    }
}
