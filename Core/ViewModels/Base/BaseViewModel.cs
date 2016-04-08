using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Threading.Tasks;
using Oyosoft.AgenceImmobiliere.Core.Service;
using Oyosoft.AgenceImmobiliere.Core.Tools;

namespace Oyosoft.AgenceImmobiliere.Core.ViewModels
{
    public abstract class BaseViewModel<TModel, TSearchCriteria> : BaseNotifyPropertyChanged
        where TModel : Model.ModeleBase, new()
        where TSearchCriteria : DataAccess.SearchCriteria, new()
    {
        public class Details<TModel2> where TModel2 : class
        {
            public TModel Result1 { get; set; }
            public TModel2 Result2 { get; set; }
        }


        protected DataAccess.Connection _localConnection;
        protected IContractAsync _distantService;
        protected bool _serviceAvailable;
        protected TSearchCriteria _criteria;
        protected Parameters _parameters;
        protected Tools.ErrorsList _errors;

        public Tools.ErrorsList Erreurs
        {
            get { return _errors; }
        }
        
        protected BaseViewModel()
        {
            Initialize();
        }
        //protected BaseViewModel(DataAccess.Connection localConnection, IContractAsync distantService, bool serviceAvailable, Parameters parameters)
        //{
        //    this._localConnection = localConnection;
        //    this._distantService = distantService;
        //    this._serviceAvailable = serviceAvailable;
        //    this._criteria = new TSearchCriteria();
        //    this._parameters = parameters;
        //    this._errors = new Tools.ErrorsList();
        //}

        private async Task Initialize()
        {
            this._errors = new Tools.ErrorsList();
            this._criteria = new TSearchCriteria();

            this._localConnection = await DataAccess.Connection.GetCurrent();
            this._errors.AddRange(this._localConnection.Errors);

            this._distantService = Connection.DistantService;
            this._serviceAvailable = Connection.ServiceAvailable;
            this._parameters = Connection.Parameters;
        }


        protected async Task<bool> SynchronizeLocalToDistant()
        {
            return await SynchronizeLocalToDistant(this._localConnection, this._distantService, this._serviceAvailable, this._parameters, this.Erreurs);
        }
        protected async Task<bool> SynchronizeDistantToLocal()
        {
            return await SynchronizeDistantToLocal(this._localConnection, this._distantService, this._serviceAvailable, this._parameters, this.Erreurs);
        }
        

        protected async Task<DataAccess.SearchResult<TModel>> GetList(long? currentPage, long? itemsCountOnPage)
        {
            return await GetList(this._localConnection, this._distantService, this._serviceAvailable, this._criteria, this.Erreurs, currentPage, itemsCountOnPage);
        }

        protected async Task<TModel> GetDetails(long id)
        {
            return await GetDetails(this._localConnection, this._distantService, this._serviceAvailable, this.Erreurs, id);
        }
        protected async Task<Details<T2>> GetDetails<T2>(long id) where T2 : class, new()
        {
            return await GetDetails<T2>(this._localConnection, this._distantService, this._serviceAvailable, this.Erreurs, id);
        }


        protected async Task<bool> Insert(TModel item)
        {
            return await Insert(this._localConnection, this._distantService, this._serviceAvailable, this.Erreurs, item);
        }
        protected async Task<bool> Insert<T2>(TModel item, ObservableCollection<T2> subItems) where T2 : Model.ModeleBase
        {
            return await Insert(this._localConnection, this._distantService, this._serviceAvailable, this.Erreurs, item, subItems);
        }

        protected async Task<bool> Update(TModel item)
        {
            return await Update(this._localConnection, this._distantService, this._serviceAvailable, this.Erreurs, item);
        }
        protected async Task<bool> Update<T2>(TModel item, ObservableCollection<T2> subItems) where T2 : Model.ModeleBase
        {
            return await Update(this._localConnection, this._distantService, this._serviceAvailable, this.Erreurs, item, subItems);
        }

        protected async Task<bool> Delete(long id)
        {
            return await Delete(this._localConnection, this._distantService, this._serviceAvailable, this.Erreurs, id);
        }









        public async static Task<bool> ConnectUser(DataAccess.Connection localConnection,
                                                   Tools.ErrorsList errors,
                                                   string userName,
                                                   string encryptedPassword)
        {
            await localConnection.ConnectUser(userName, encryptedPassword);
            errors.AddRange(localConnection.Errors);
            return errors.IsEmpty;
        }
        public async static Task<bool> DisconnectUser(DataAccess.Connection localConnection,
                                                      Tools.ErrorsList errors,
                                                      string userName)
        {
            localConnection.DisconnectUser(userName);
            errors.AddRange(localConnection.Errors);
            return errors.IsEmpty;
        }


        internal async static Task<bool> SynchronizeLocalToDistant(DataAccess.Connection localConnection, 
                                                                   IContractAsync distantService,
                                                                   bool serviceAvailable, 
                                                                   Parameters parameters, 
                                                                   Tools.ErrorsList errors)
        {
            errors.Clear();

            if (!serviceAvailable)
            {
                errors.Add("Erreur pendant la synchronisation : Le service distant n'est pas disponible.");
                return false;
            }

            // Lecture des éléments à synchroniser
            DataAccess.SynchroList items = await localConnection.SelectSynchronize(parameters.DateHeureDerniereSynchro);
            errors.AddRange(localConnection.Errors);
            if (!errors.IsEmpty) return false;

            // Lancement de la synchro
            OperationResult result = await distantService.SynchroniserElementsAsync(items);
            errors.AddRange(result.Errors);
            if (!errors.IsEmpty) return false;

            // Enregistrement de la date de synchro
            parameters.DateHeureDerniereSynchro = DateTime.Now;
            await parameters.EnregistrerParametres();
            errors.AddRange(parameters.Erreurs);

            return errors.IsEmpty;
        }
        internal async static Task<bool> SynchronizeDistantToLocal(DataAccess.Connection localConnection,
                                                                   IContractAsync distantService,
                                                                   bool serviceAvailable,
                                                                   Parameters parameters,
                                                                   Tools.ErrorsList errors)
        {
            errors.Clear();

            if (!serviceAvailable)
            {
                errors.Add("Erreur pendant la synchronisation : Le service distant n'est pas disponible.");
                return false;
            }

            // Lecture des éléments à synchroniser
            OperationResult<DataAccess.SynchroList> items = await distantService.LireElementsASynchroniserAsync(parameters.DateHeureDerniereSynchro);
            errors.AddRange(items.Errors);
            if (!errors.IsEmpty) return false;

            // Lancement de la synchro
            await localConnection.Synchronize(items.Result, true);
            errors.AddRange(localConnection.Errors);
            if (!errors.IsEmpty) return false;

            // Enregistrement de la date de synchro
            parameters.DateHeureDerniereSynchro = DateTime.Now;
            await parameters.EnregistrerParametres();
            errors.AddRange(parameters.Erreurs);

            return errors.IsEmpty;
        }


        protected async static Task<DataAccess.SearchResult<TModel>> GetList(DataAccess.Connection localConnection,
                                                                             IContractAsync distantService,
                                                                             bool serviceAvailable,
                                                                             TSearchCriteria criteria,
                                                                             Tools.ErrorsList errors,
                                                                             long? currentPage, 
                                                                             long? itemsCountOnPage)
        {
            errors.Clear();

            if (serviceAvailable)
            {
                try
                {
                    OperationResult<DataAccess.SearchResult<TModel>> result;
                    if (typeof(TModel) == typeof(Model.BienImmobilier))
                        result = await distantService.LireListeBiensImmobiliersAsync(criteria as BienImmobilier.SearchCriteria, currentPage, itemsCountOnPage) as OperationResult<DataAccess.SearchResult<TModel>>;
                    else if (typeof(TModel) == typeof(Model.Personne))
                        result = await distantService.LireListeClientsAsync(criteria as Client.SearchCriteria, currentPage, itemsCountOnPage) as OperationResult<DataAccess.SearchResult<TModel>>;
                    else if (typeof(TModel) == typeof(Model.Utilisateur))
                        result = await distantService.LireListeUtilisateursAsync(criteria as Utilisateur.SearchCriteria, currentPage, itemsCountOnPage) as OperationResult<DataAccess.SearchResult<TModel>>;
                    else
                    {
                        errors.Add("Le type '" + typeof(TModel).Name + "' n'est pas pris en charge.");
                        return null;
                    }

                    if (result.Success)
                    {
                        return result.Result;
                    }
                    else
                    {
                        errors.AddRange(result.Errors);
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
                    return null;
                }
            }
            else
            {
                DataAccess.SearchResult<TModel> result;
                result = await localConnection.SelectCriteria<TModel>(criteria, currentPage, itemsCountOnPage);
                errors.AddRange(localConnection.Errors);
                if (errors.IsEmpty) return result;
            }

            return null;
        }
        public async static Task<DataAccess.SearchResult<TModel>> GetList(DataAccess.Connection localConnection,
                                                                          TSearchCriteria criteria,
                                                                          Tools.ErrorsList errors,
                                                                          long? currentPage,
                                                                          long? itemsCountOnPage, 
                                                                          bool checkUserConnection = true)
        {
            DataAccess.SearchResult<TModel> result;

            result = await localConnection.SelectCriteria<TModel>(criteria, currentPage, itemsCountOnPage, checkUserConnection);
            errors.AddRange(localConnection.Errors);

            if (errors.IsEmpty) return result;
            return null;
        }

        protected async static Task<TModel> GetDetails(DataAccess.Connection localConnection,
                                                       IContractAsync distantService,
                                                       bool serviceAvailable,
                                                       Tools.ErrorsList errors,
                                                       long id)
        {
            errors.Clear();

            if (serviceAvailable)
            {
                try
                {
                    OperationResult<TModel> result;
                    if (typeof(TModel) == typeof(Model.Personne))
                        result = await distantService.LireDetailsClientAsync(id.ToString()) as OperationResult<TModel>;
                    else
                    {
                        errors.Add("Le type '" + typeof(TModel).Name + "' n'est pas pris en charge.");
                        return null;
                    }

                    if (result.Success)
                    {
                        return result.Result;
                    }
                    else
                    {
                        errors.AddRange(result.Errors);
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
                    return null;
                }
            }
            else
            {
                TModel result;
                result = await localConnection.SelectItem<TModel, long>(id);
                errors.AddRange(localConnection.Errors);
                if (errors.IsEmpty) return result;
            }

            return null;
        }
        public async static Task<TModel> GetDetails(DataAccess.Connection localConnection,
                                                       Tools.ErrorsList errors,
                                                       long id)
        {
            TModel result;

            result = await localConnection.SelectItem<TModel, long>(id);
            errors.AddRange(localConnection.Errors);

            if (errors.IsEmpty) return result;
            return null;
        }

        protected async static Task<Details<T2>> GetDetails<T2>(DataAccess.Connection localConnection,
                                                                IContractAsync distantService,
                                                                bool serviceAvailable,
                                                                Tools.ErrorsList errors,
                                                                long id) where T2 : class, new()
        {
            Details<T2> details = new Details<T2>();

            errors.Clear();

            if (serviceAvailable)
            {
                try
                {
                    OperationResult<TModel, T2> result;
                    if (typeof(TModel) == typeof(Model.BienImmobilier))
                        result = await distantService.LireDetailsBienImmobilierAsync(id.ToString()) as OperationResult<TModel, T2>;
                    else if (typeof(TModel) == typeof(Model.Utilisateur))
                        result = await distantService.LireDetailsUtilisateurAsync(id.ToString()) as OperationResult<TModel, T2>;
                    else
                    {
                        errors.Add("Le type '" + typeof(TModel).Name + "' n'est pas pris en charge.");
                        return null;
                    }

                    if (result.Success)
                    {
                        details.Result1 = result.Result;
                        details.Result2 = result.Result2;
                        return details;
                    }
                    else
                    {
                        errors.AddRange(result.Errors);
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
                    return null;
                }
            }
            else
            {
                details.Result1 = await localConnection.SelectItem<TModel, long>(id);
                errors.AddRange(localConnection.Errors);
                if (!errors.IsEmpty) return null;

                if (typeof(TModel) == typeof(Model.BienImmobilier))
                {
                    details.Result2 = await localConnection.SelectItems<Model.PhotoBienImmobilier, long>(DataAccess.Const.DB_PHOTO_IDBIEN_COLNAME, id) as T2;
                    errors.AddRange(localConnection.Errors);
                    if (!errors.IsEmpty) return null;
                }
                else if (typeof(TModel) == typeof(Model.Utilisateur))
                {
                    details.Result2 = await localConnection.SelectItem<Model.Personne, long>(DataAccess.Const.DB_UTILISATEUR_IDPERSONNE_COLNAME, id) as T2;
                    errors.AddRange(localConnection.Errors);
                    if (!errors.IsEmpty) return null;
                }
                else
                {
                    errors.Add("Le type '" + typeof(TModel).Name + "' n'est pas pris en charge.");
                    return null;
                }

                return details;
            }
        }
        public async static Task<Details<T2>> GetDetails<T2>(DataAccess.Connection localConnection,
                                                             Tools.ErrorsList errors,
                                                             long id) where T2 : class, new()
        {
            Details<T2> details = new Details<T2>();
            
            details.Result1 = await localConnection.SelectItem<TModel, long>(id);
            errors.AddRange(localConnection.Errors);
            if (!errors.IsEmpty) return null;

            if (typeof(T2) == typeof(Model.BienImmobilier))
            {
                details.Result2 = await localConnection.SelectItems<Model.PhotoBienImmobilier, long>(DataAccess.Const.DB_PHOTO_IDBIEN_COLNAME, id) as T2;
                errors.AddRange(localConnection.Errors);
                if (!errors.IsEmpty) return null;
            }
            else if (typeof(T2) == typeof(Model.Utilisateur))
            {
                details.Result2 = await localConnection.SelectItem<Model.Personne, long>(DataAccess.Const.DB_UTILISATEUR_IDPERSONNE_COLNAME, id) as T2;
                errors.AddRange(localConnection.Errors);
                if (!errors.IsEmpty) return null;
            }
            else
            {
                errors.Add("Le type '" + typeof(TModel).Name + "' n'est pas pris en charge.");
                return null;
            }

            return details;
        }



        protected async static Task<bool> Insert(DataAccess.Connection localConnection,
                                                 IContractAsync distantService,
                                                 bool serviceAvailable,
                                                 Tools.ErrorsList errors, 
                                                 TModel item)
        {
            errors.Clear();

            await localConnection.RunInTransaction(() => {
                // Insertion dans la base locale
                Insert(localConnection, errors, item).ExecuteSynchronously();

                if (errors.IsEmpty && serviceAvailable)
                {
                    try
                    {
                        // Insertion sur le serveur distant
                        OperationResult result;
                        if (typeof(TModel) == typeof(Model.Personne))
                            result = distantService.AjouterClient(item as Model.Personne);
                        else if (typeof(TModel) == typeof(Model.Utilisateur))
                            result = distantService.AjouterUtilisateur(item as Model.Utilisateur);
                        else
                        {
                            errors.Add("Le type '" + typeof(TModel).Name + "' n'est pas pris en charge.");
                            return false;
                        }

                        if (!result.Success)
                        {
                            errors.AddRange(result.Errors);
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add(Tools.Log.LogException(ex).ExecuteSynchronously(), Enums.ErrorType.Exception, ex);
                        return false;
                    }
                }

                return errors.IsEmpty;
            });

            //await localConnection.RunInTransaction(conn => {
            //    // Insertion dans la base locale
            //    Insert(localConnection, errors, item).ExecuteSynchronously();

            //    if (errors.IsEmpty && serviceAvailable)
            //    {
            //        try
            //        {
            //            // Insertion sur le serveur distant
            //            OperationResult result;
            //            if (typeof(TModel) == typeof(Model.Personne))
            //                result = distantService.AjouterClient(item as Model.Personne);
            //            else if (typeof(TModel) == typeof(Model.Utilisateur))
            //                result = distantService.AjouterUtilisateur(item as Model.Utilisateur);
            //            else
            //            {
            //                errors.Add("Le type '" + typeof(TModel).Name + "' n'est pas pris en charge.");
            //                return;
            //            }

            //            if (!result.Success)
            //            {
            //                errors.AddRange(result.Errors);
            //                return;
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            errors.Add(Tools.Log.LogException(ex).ExecuteSynchronously(), Enums.ErrorType.Exception, ex);
            //            return;
            //        }
            //    }
            //});

            return errors.IsEmpty;
        }
        public async static Task<bool> Insert(DataAccess.Connection localConnection,
                                              Tools.ErrorsList errors,
                                              TModel item)
        {
            // Insertion dans la base locale
            return await DataAccess.Connection.Insert(localConnection, localConnection.ConnectedUserName, item, errors);
        }

        protected async static Task<bool> Insert<T2>(DataAccess.Connection localConnection,
                                                     IContractAsync distantService,
                                                     bool serviceAvailable,
                                                     Tools.ErrorsList errors, 
                                                     TModel item, 
                                                     ObservableCollection<T2> subItems) where T2 : Model.ModeleBase
        {
            errors.Clear();

            await localConnection.RunInTransaction(() => {
                // Insertion dans la base locale
                Insert(localConnection, errors, item, subItems).ExecuteSynchronously();

                if (errors.IsEmpty && serviceAvailable)
                {
                    try
                    {
                        // Insertion sur le serveur distant
                        OperationResult result;
                        if (typeof(TModel) == typeof(Model.BienImmobilier))
                        {
                            result = distantService.AjouterBienImmobilier(item as Model.BienImmobilier, subItems as ICollection<Model.PhotoBienImmobilier>);
                            if (!result.Success)
                            {
                                errors.AddRange(result.Errors);
                                return false;
                            }
                        }
                        else
                        {
                            Insert(localConnection, distantService, serviceAvailable, errors, item).ExecuteSynchronously();
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add(Tools.Log.LogException(ex).ExecuteSynchronously(), Enums.ErrorType.Exception, ex);
                        return false;
                    }
                }

                return errors.IsEmpty;
            });

            //await localConnection.RunInTransaction(conn => {
            //    // Insertion dans la base locale
            //    Insert(localConnection, errors, item, subItems).ExecuteSynchronously();

            //    if (errors.IsEmpty && serviceAvailable)
            //    {
            //        try
            //        {
            //            // Insertion sur le serveur distant
            //            OperationResult result;
            //            if (typeof(TModel) == typeof(Model.BienImmobilier))
            //            {
            //                result = distantService.AjouterBienImmobilier(item as Model.BienImmobilier, subItems as ICollection<Model.PhotoBienImmobilier>);
            //                if (!result.Success)
            //                {
            //                    errors.AddRange(result.Errors);
            //                    return;
            //                }
            //            }
            //            else
            //            {
            //                Insert(localConnection, distantService, serviceAvailable, errors, item).ExecuteSynchronously();
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            errors.Add(Tools.Log.LogException(ex).ExecuteSynchronously(), Enums.ErrorType.Exception, ex);
            //            return;
            //        }
            //    }
            //});

            return errors.IsEmpty;
        }
        public async static Task<bool> Insert<T2>(DataAccess.Connection localConnection,
                                                  Tools.ErrorsList errors,
                                                  TModel item,
                                                  ObservableCollection<T2> subItems) where T2 : Model.ModeleBase
        {
            // Insertion dans la base locale
            await DataAccess.Connection.Insert(localConnection, localConnection.ConnectedUserName, item, errors);
            if (!errors.IsEmpty) return false;

            // Récupération de l'identifiant
            long id = await DataAccess.Connection.SelectMaxKey<TModel>(localConnection, errors);
            if (!errors.IsEmpty) return false;

            // Insertion des sous-éléments dans la base locale
            foreach (T2 subItem in subItems)
            {
                if (typeof(T2) == typeof(Model.PhotoBienImmobilier)) (subItem as Model.PhotoBienImmobilier).IdBien = id;
                await DataAccess.Connection.Insert(localConnection, localConnection.ConnectedUserName, subItem, errors);
            }

            return errors.IsEmpty;
        }


        protected async static Task<bool> Update(DataAccess.Connection localConnection,
                                                 IContractAsync distantService,
                                                 bool serviceAvailable,
                                                 Tools.ErrorsList errors, 
                                                 TModel item)
        {
            errors.Clear();

            await localConnection.RunInTransaction(() => {
                // Mise à jour dans la base locale
                Update(localConnection, errors, item).ExecuteSynchronously();

                if (errors.IsEmpty && serviceAvailable)
                {
                    try
                    {
                        // Mise à jour sur le serveur distant
                        OperationResult result;
                        if (typeof(TModel) == typeof(Model.Personne))
                            result = distantService.ModifierClient(item as Model.Personne);
                        else if (typeof(TModel) == typeof(Model.Utilisateur))
                            result = distantService.ModifierUtilisateur(item as Model.Utilisateur);
                        else
                        {
                            errors.Add("Le type '" + typeof(TModel).Name + "' n'est pas pris en charge.");
                            return false;
                        }

                        if (!result.Success)
                        {
                            errors.AddRange(result.Errors);
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add(Tools.Log.LogException(ex).ExecuteSynchronously(), Enums.ErrorType.Exception, ex);
                        return false;
                    }
                }
                return errors.IsEmpty;
            });
            //await localConnection.RunInTransaction(conn => {
            //    // Mise à jour dans la base locale
            //    Update(localConnection, errors, item).ExecuteSynchronously();

            //    if (errors.IsEmpty && serviceAvailable)
            //    {
            //        try
            //        {
            //            // Mise à jour sur le serveur distant
            //            OperationResult result;
            //            if (typeof(TModel) == typeof(Model.Personne))
            //                result = distantService.ModifierClient(item as Model.Personne);
            //            else if (typeof(TModel) == typeof(Model.Utilisateur))
            //                result = distantService.ModifierUtilisateur(item as Model.Utilisateur);
            //            else
            //            {
            //                errors.Add("Le type '" + typeof(TModel).Name + "' n'est pas pris en charge.");
            //                return;
            //            }

            //            if (!result.Success)
            //            {
            //                errors.AddRange(result.Errors);
            //                return;
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            errors.Add(Tools.Log.LogException(ex).ExecuteSynchronously(), Enums.ErrorType.Exception, ex);
            //            return;
            //        }
            //    }
            //});

            return errors.IsEmpty;
        }
        public async static Task<bool> Update(DataAccess.Connection localConnection,
                                              Tools.ErrorsList errors,
                                              TModel item)
        {
            // Mise à jour dans la base locale
            return await DataAccess.Connection.Update(localConnection, item, errors);
        }

        protected async static Task<bool> Update<T2>(DataAccess.Connection localConnection,
                                                     IContractAsync distantService,
                                                     bool serviceAvailable,
                                                     Tools.ErrorsList errors, 
                                                     TModel item,
                                                     ObservableCollection<T2> subItems) where T2 : Model.ModeleBase
        {
            errors.Clear();

            await localConnection.RunInTransaction(() => {
                // Mise à jour dans la base locale
                Update(localConnection, errors, item, subItems).ExecuteSynchronously();

                if (errors.IsEmpty && serviceAvailable)
                {
                    try
                    {
                        // Mise à jour sur le serveur distant
                        OperationResult result;
                        if (typeof(TModel) == typeof(Model.BienImmobilier))
                        {
                            result = distantService.ModifierBienImmobilier(item as Model.BienImmobilier, subItems as ICollection<Model.PhotoBienImmobilier>);
                            if (!result.Success)
                            {
                                errors.AddRange(result.Errors);
                                return false;
                            }
                        }
                        else
                        {
                            Update(localConnection, distantService, serviceAvailable, errors, item).ExecuteSynchronously();
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add(Tools.Log.LogException(ex).ExecuteSynchronously(), Enums.ErrorType.Exception, ex);
                        return false;
                    }
                }
                return errors.IsEmpty;
            });
            //await localConnection.RunInTransaction(conn => {
            //    // Mise à jour dans la base locale
            //    Update(localConnection, errors, item, subItems).ExecuteSynchronously();

            //    if (errors.IsEmpty && serviceAvailable)
            //    {
            //        try
            //        {
            //            // Mise à jour sur le serveur distant
            //            OperationResult result;
            //            if (typeof(TModel) == typeof(Model.BienImmobilier))
            //            {
            //                result = distantService.ModifierBienImmobilier(item as Model.BienImmobilier, subItems as ICollection<Model.PhotoBienImmobilier>);
            //                if (!result.Success)
            //                {
            //                    errors.AddRange(result.Errors);
            //                    return;
            //                }
            //            }
            //            else
            //            {
            //                Update(localConnection, distantService, serviceAvailable, errors, item).ExecuteSynchronously();
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            errors.Add(Tools.Log.LogException(ex).ExecuteSynchronously(), Enums.ErrorType.Exception, ex);
            //            return;
            //        }
            //    }
            //});

            return errors.IsEmpty;
        }
        public async static Task<bool> Update<T2>(DataAccess.Connection localConnection,
                                                  Tools.ErrorsList errors,
                                                  TModel item,
                                                  ObservableCollection<T2> subItems) where T2 : Model.ModeleBase
        {
            // Mise à jour dans la base locale
            await DataAccess.Connection.Update(localConnection, item, errors);
            if (!errors.IsEmpty) return false;


            // Lecture des sous-éléments actuellement présents dans la base locale
            ObservableCollection<T2> currentSubItems = null;
            if (typeof(TModel) == typeof(Model.BienImmobilier) && typeof(T2) == typeof(Model.PhotoBienImmobilier))
            {
                currentSubItems = await DataAccess.Connection.SelectItems<T2, long>(localConnection, DataAccess.Const.DB_PHOTO_IDBIEN_COLNAME, item.Id, errors);
                if (!errors.IsEmpty) return false;
            }

            // Insertion ou mise à jour des sous-éléments dans la base locale
            foreach (T2 subItem in subItems)
            {
                if (typeof(T2) == typeof(Model.PhotoBienImmobilier)) (subItem as Model.PhotoBienImmobilier).IdBien = item.Id;
                await DataAccess.Connection.InsertOrReplace(localConnection, localConnection.ConnectedUserName, subItem, errors);
            }
            if (!errors.IsEmpty) return false;

            // Suppression des sous-éléments en trop dans la base locale
            if (currentSubItems != null)
            {
                foreach (T2 subItem in currentSubItems)
                {
                    if (!subItems.Contains(subItem))
                    {
                        if (typeof(T2) == typeof(Model.PhotoBienImmobilier)) (subItem as Model.PhotoBienImmobilier).IdBien = item.Id;
                        await DataAccess.Connection.Delete(localConnection, subItem, errors);
                    }
                }
                if (!errors.IsEmpty) return false;
            }

            return errors.IsEmpty;
        }


        protected async static Task<bool> Delete(DataAccess.Connection localConnection,
                                                 IContractAsync distantService,
                                                 bool serviceAvailable,
                                                 Tools.ErrorsList errors, 
                                                 long id)
        {
            errors.Clear();

            await localConnection.RunInTransaction(() => {
                // Suppression dans la base locale
                Delete(localConnection, errors, id).ExecuteSynchronously();

                if (errors.IsEmpty && serviceAvailable)
                {
                    try
                    {
                        // Suppression sur le serveur distant
                        OperationResult result;
                        if (typeof(TModel) == typeof(Model.BienImmobilier))
                            result = distantService.SupprimerBienImmobilier(id.ToString());
                        else if (typeof(TModel) == typeof(Model.Personne))
                            result = distantService.SupprimerClient(id.ToString());
                        else if (typeof(TModel) == typeof(Model.Utilisateur))
                            result = distantService.SupprimerUtilisateur(id.ToString());
                        else
                        {
                            errors.Add("Le type '" + typeof(TModel).Name + "' n'est pas pris en charge.");
                            return false;
                        }

                        if (!result.Success)
                        {
                            errors.AddRange(result.Errors);
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add(Tools.Log.LogException(ex).ExecuteSynchronously(), Enums.ErrorType.Exception, ex);
                        return false;
                    }
                }
                return errors.IsEmpty;
            });
            //await localConnection.RunInTransaction(conn => {
            //    // Suppression dans la base locale
            //    Delete(localConnection, errors, id).ExecuteSynchronously();

            //    if (errors.IsEmpty && serviceAvailable)
            //    {
            //        try
            //        {
            //            // Suppression sur le serveur distant
            //            OperationResult result;
            //            if (typeof(TModel) == typeof(Model.BienImmobilier))
            //                result = distantService.SupprimerBienImmobilier(id.ToString());
            //            else if (typeof(TModel) == typeof(Model.Personne))
            //                result = distantService.SupprimerClient(id.ToString());
            //            else if (typeof(TModel) == typeof(Model.Utilisateur))
            //                result = distantService.SupprimerUtilisateur(id.ToString());
            //            else
            //            {
            //                errors.Add("Le type '" + typeof(TModel).Name + "' n'est pas pris en charge.");
            //                return;
            //            }

            //            if (!result.Success)
            //            {
            //                errors.AddRange(result.Errors);
            //                return;
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            errors.Add(Tools.Log.LogException(ex).ExecuteSynchronously(), Enums.ErrorType.Exception, ex);
            //            return;
            //        }
            //    }
            //});

            return errors.IsEmpty;
        }
        public async static Task<bool> Delete(DataAccess.Connection localConnection,
                                              Tools.ErrorsList errors,
                                              long id)
        {
            // Suppression dans la base locale
            await DataAccess.Connection.Delete<TModel, long>(localConnection, id, errors);

            // Suppression des sous-éléments dans la base locale
            if (errors.IsEmpty && typeof(TModel) == typeof(Model.BienImmobilier))
            {
                await DataAccess.Connection.Delete<Model.PhotoBienImmobilier, long>(localConnection, DataAccess.Const.DB_PHOTO_IDBIEN_COLNAME, id, errors);
            }

            return errors.IsEmpty;
        }


    }
}
