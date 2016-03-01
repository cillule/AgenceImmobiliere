using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oyosoft.AgenceImmobiliere.Core;
using Oyosoft.AgenceImmobiliere.Core.DataAccess;
using Oyosoft.AgenceImmobiliere.Core.Model;
using Oyosoft.AgenceImmobiliere.Core.Service;
using Oyosoft.AgenceImmobiliere.Core.Tools;

namespace Oyosoft.AgenceImmobiliere.WcfService.Operations
{
    internal static class BiensImmobiliers
    {
        internal async static Task<OperationResult<SearchResult<BienImmobilier>>> LireListeBiensImmobiliers(Connection connection, Core.ViewModels.BienImmobilier.SearchCriteria criteres, params long?[] parametres)
        {
            OperationResult<SearchResult<BienImmobilier>> resultat = new OperationResult<SearchResult<BienImmobilier>>();
            
            // Chargement des paramètres
            long? page = null;
            long? nbBiens = null;
            if (parametres.Length > 0) page = parametres[0];
            if (parametres.Length > 1) nbBiens = parametres[1];

            if (criteres == null)
            {
                Manager.ReadURIParameter("criteres", out criteres);
            }

            if (page == null)
            {
                int tmp;
                if (Manager.ReadIntURIParameter("currentPage", out tmp))
                    page = tmp;
            }

            if (nbBiens == null)
            {
                int tmp;
                if (Manager.ReadIntURIParameter("itemsCountOnPage", out tmp))
                    nbBiens = tmp;
            }

            // Lecture des biens immobiliers
            ErrorsList errors = new ErrorsList();
            resultat.Result = await Core.ViewModels.BaseViewModel<BienImmobilier, Core.ViewModels.BienImmobilier.SearchCriteria>.GetList(connection, criteres, errors, page, nbBiens);
            if (resultat.Result == null || !errors.IsEmpty)
            {
                await Manager.ManageError(resultat, errors);
                return resultat;
            }

            resultat.Success = resultat.Errors.IsEmpty;

            return resultat;
        }
        internal async static Task<OperationResult<BienImmobilier, ObservableCollection<PhotoBienImmobilier>>> LireDetailsBienImmobilier(Connection connection, string id, params object[] nonUtilise)
        {
            OperationResult<BienImmobilier, ObservableCollection<PhotoBienImmobilier>> resultat = new OperationResult<BienImmobilier, ObservableCollection<PhotoBienImmobilier>>();

            // Chargement des paramètres
            if (string.IsNullOrEmpty(id))
            {
                Manager.ReadURIParameter("id", out id);
            }

            // Conversion de l'identifiant
            long idBien;
            if (!long.TryParse(id, out idBien))
            {
                await Manager.ManageError(resultat, "L'identifiant du bien immobilier à charger est invalide !");
                return resultat;
            }

            // Lecture du bien immobilier
            Core.ViewModels.BaseViewModel<BienImmobilier, Core.ViewModels.BienImmobilier.SearchCriteria>.Details<ObservableCollection<PhotoBienImmobilier>> bien;
            ErrorsList errors = new ErrorsList();
            bien = await Core.ViewModels.BaseViewModel<BienImmobilier, Core.ViewModels.BienImmobilier.SearchCriteria>.GetDetails<ObservableCollection<PhotoBienImmobilier>>(connection, errors, idBien);
            if (bien == null || !errors.IsEmpty)
            {
                await Manager.ManageError(resultat, errors);
                return resultat;
            }

            resultat.Result = bien.Result1;
            resultat.Result2 = bien.Result2;
            resultat.Success = resultat.Errors.IsEmpty;

            return resultat;
        }
        internal async static Task<OperationResult> AjouterBienImmobilier(Connection connection, BienImmobilier bien, params ICollection<PhotoBienImmobilier>[] parametres)
        {
            OperationResult resultat = new OperationResult();

            // Chargement des paramètres
            ICollection<PhotoBienImmobilier> photos = null;
            if (parametres.Length > 0) photos = parametres[0];
            if (bien == null)
            {
                Manager.ReadURIParameter("bien", out bien);
            }
            if (photos == null)
            {
                Manager.ReadURIParameter("photos", out photos);
            }

            // Ajout du bien immobilier
            ErrorsList errors = new ErrorsList();
            bool ajout = await Core.ViewModels.BaseViewModel<BienImmobilier, Core.ViewModels.BienImmobilier.SearchCriteria>.Insert<PhotoBienImmobilier>(connection, errors, bien, new ObservableCollection<PhotoBienImmobilier>(photos));
            if (!ajout || !errors.IsEmpty)
            {
                await Manager.ManageError(resultat, errors);
                return resultat;
            }
            
            resultat.Success = resultat.Errors.IsEmpty;

            return resultat;
        }
        internal async static Task<OperationResult> ModifierBienImmobilier(Connection connection, BienImmobilier bien, params ICollection<PhotoBienImmobilier>[] parametres)
        {
            OperationResult resultat = new OperationResult();

            // Chargement des paramètres
            ICollection<PhotoBienImmobilier> photos = null;
            if (parametres.Length > 0) photos = parametres[0];
            if (bien == null)
            {
                Manager.ReadURIParameter("bien", out bien);
            }
            if (photos == null)
            {
                Manager.ReadURIParameter("photos", out photos);
            }

            // Mise à jour du bien immobilier
            ErrorsList errors = new ErrorsList();
            bool maj = await Core.ViewModels.BaseViewModel<BienImmobilier, Core.ViewModels.BienImmobilier.SearchCriteria>.Update<PhotoBienImmobilier>(connection, errors, bien, new ObservableCollection<PhotoBienImmobilier>(photos));
            if (!maj || !errors.IsEmpty)
            {
                await Manager.ManageError(resultat, errors);
                return resultat;
            }
                        
            resultat.Success = resultat.Errors.IsEmpty;

            return resultat;
        }
        internal async static Task<OperationResult> SupprimerBienImmobilier(Connection connection, string id, params object[] nonUtilise)
        {
            OperationResult resultat = new OperationResult();

            // Chargement des paramètres
            if (string.IsNullOrEmpty(id))
            {
                Manager.ReadURIParameter("id", out id);
            }

            // Conversion de l'identifiant
            long idBien;
            if (!long.TryParse(id, out idBien))
            {
                await Manager.ManageError(resultat, "L'identifiant du bien immobilier à charger est invalide !");
                return resultat;
            }

            // Suppression du bien immobilier
            ErrorsList errors = new ErrorsList();
            bool suppr = await Core.ViewModels.BaseViewModel<BienImmobilier, Core.ViewModels.BienImmobilier.SearchCriteria>.Delete(connection, errors, idBien);
            if (!suppr || !errors.IsEmpty)
            {
                await Manager.ManageError(resultat, errors);
                return resultat;
            }
            
            resultat.Success = resultat.Errors.IsEmpty;

            return resultat;
        }
    }
}
