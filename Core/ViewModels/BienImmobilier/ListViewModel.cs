using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Oyosoft.AgenceImmobiliere.Core.Service;

namespace Oyosoft.AgenceImmobiliere.Core.ViewModels.BienImmobilier
{
    public class ListViewModel : BaseViewModel<Model.BienImmobilier, SearchCriteria>
    {

        
        public ListViewModel(DataAccess.Connection localConnection, IContractAsync distantService, bool serviceAvailable, Parameters parametres) 
            : base(localConnection, distantService, serviceAvailable, parametres)
        { }


        // Créer une classe générique contenant les méthodes statiques nécessaires au service WCF (prendre ces méthodes dans BaseViewModel)
        // Appeler ces méthodes statiques dans BaseViewModel
        // Ajouter dans le répertoire service une classe par entité contenant les méthodes nécessaires au service WCF
        // Dans une classe viewmodel, on doit avoir toutes les propriétés permettant de remplir l'interface


        //public async Task<OperationResult<DataAccess.SearchResult<Model.BienImmobilier>>> LireListeBiensImmobiliers(long? currentPage, long? itemsCountOnPage)
        //{
        //    OperationResult<DataAccess.SearchResult<Model.BienImmobilier>> result = new OperationResult<DataAccess.SearchResult<Model.BienImmobilier>>();
        //    result.Result = await GetList(currentPage, itemsCountOnPage);
        //    result.Errors.AddRange(this.Erreurs);
        //    result.Success = result.Errors.IsEmpty;
        //    return result;
        //}
        //OperationResult<Model.BienImmobilier, ObservableCollection<Model.PhotoBienImmobilier>> LireDetailsBienImmobilier(string id);
        //OperationResult<bool> AjouterBienImmobilier(Model.BienImmobilier bien, List<Model.PhotoBienImmobilier> photos);
        //OperationResult<bool> ModifierBienImmobilier(Model.BienImmobilier bien, List<Model.PhotoBienImmobilier> photos);
        //OperationResult<bool> SupprimerBienImmobilier(string id);
    }
}
