using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Attributes;
using System.Reflection;
using Oyosoft.AgenceImmobiliere.Core.Exceptions;
using Oyosoft.AgenceImmobiliere.Core.Tools;

namespace Oyosoft.AgenceImmobiliere.Core.DataAccess
{
    public class Connection
    {

        private static Connection _connection = null;
        public static async Task<Connection> GetCurrent(SQLite.Net.Interop.ISQLitePlatform sqlitePlatform = null, string databasePath = "")
        {
            if (_connection == null || !_connection._initialized)
            {
                _connection = new Connection();

                if (sqlitePlatform == null)
                    _connection._errors.Add("Aucune plateforme n'est fournie alors que la connexion n'est pas encore initialisée !");
                if (string.IsNullOrEmpty(databasePath))
                    _connection._errors.Add("Aucun chemin à la base de données n'est fourni alors que la connexion n'est pas encore initialisée !");
                if (!_connection._errors.IsEmpty) return _connection;

                await _connection.Initialize(sqlitePlatform, databasePath);
            }
            return _connection;
        }

        private SQLiteConnection _conn;
        private bool _initialized;
        private string _databasePath;
        private string _connectedUserName;
        private Model.Utilisateur _connectedUser;
        private Tools.ErrorsList _errors;

        public string DatabasePath
        {
            get { return _databasePath; }
        }
        public bool UserIsConnected
        {
            get { return !string.IsNullOrEmpty(this._connectedUserName); }
        }
        public string ConnectedUserName
        {
            get { return _connectedUserName; }
        }
        public Model.Utilisateur ConnectedUser
        {
            get
            {
                if (!_initialized) return null;
                if (string.IsNullOrEmpty(_connectedUserName)) return null;
                if (_connectedUser != null && _connectedUser.NomUtilisateur.ToLower() != _connectedUserName.ToLower()) _connectedUser = null;
                if (_connectedUser == null) _connectedUser = SelectItem<Model.Utilisateur, string>(Const.DB_UTILISATEUR_NOMUTILISATEUR_COLNAME, _connectedUserName).ExecuteSynchronously();
                return null;
            }
        }
        public Tools.ErrorsList Errors
        {
            get { return _errors; }
        }


        private Connection()
        {
            this._initialized = false;
            this._databasePath = "";
            this._connectedUserName = "";
            this._connectedUser = null;
            this._errors = new Tools.ErrorsList();
        }

        private async Task<bool> Initialize(SQLite.Net.Interop.ISQLitePlatform sqlitePlatform, string databasePath)
        {
            _errors.Clear();
            _databasePath = "";
            _connectedUserName = "";

            try
            {
                _databasePath = databasePath.Replace(@"\", @"/");
                _conn = new SQLiteConnection(sqlitePlatform, _databasePath, false);
                this._initialized = true;
            }
            catch (Exception ex)
            {
                _errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
                this._initialized = false;
            }

            return _errors.IsEmpty;
        }

        private void CheckIfInstanceIsInitialized()
        {
            if (!this._initialized) throw new NotInitializedException<Connection>(this._errors);
        }
        private void CheckIfUserIsConnected()
        {
            if (!UserIsConnected) throw new NotUserConnectedException(this._errors);
        }
        internal async Task RunInTransaction(Func<bool> action)
        {
            CheckIfInstanceIsInitialized();
            CheckIfUserIsConnected();
            //await this._conn.RunInTransactionAsync(action);
            this._conn.BeginTransaction();
            if (action())
            {
                this._conn.Commit();
            }
            else
            {
                this._conn.Rollback();
            }
        }


        internal async Task<bool> ConnectUser(string userName, string encryptedPassword)
        {
            CheckIfInstanceIsInitialized();

            this._errors.Clear();
            this._connectedUserName = "";
            if (string.IsNullOrEmpty(userName))
            {
                this._errors.Add("Un nom d'utilisateur est obligatoire !");
                return false;
            }

            Model.Utilisateur user = null;
            try
            {
                user = this._conn.Find<Model.Utilisateur>(userName);
                if (user == null || string.IsNullOrEmpty(user.NomUtilisateur))
                {
                    this._errors.Add("L'utilisateur '" + userName + "' n'existe pas.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
                return false;
            }

            if (encryptedPassword != user.MotDePasseCrypte)
            {
                this._errors.Add("Le mot de passe est invalide.");
                return false;
            }

            this._connectedUserName = user.NomUtilisateur;

            return _errors.IsEmpty;
        }
        internal bool DisconnectUser(string userName)
        {
            if (this._connectedUserName.ToUpper() != userName.ToUpper()) return false;
            this._connectedUserName = "";
            return true;
        }


        internal async Task<bool> Insert<T>(T item) where T : class
        {
            CheckIfInstanceIsInitialized();
            CheckIfUserIsConnected();

            return await Insert(this._conn, this._connectedUserName, item, this._errors);
        }
        internal async Task<bool> InsertOrReplace<T>(T item) where T : class
        {
            CheckIfInstanceIsInitialized();
            CheckIfUserIsConnected();

            return await InsertOrReplace(this._conn, this._connectedUserName, item, this._errors);
        }
        internal async Task<bool> Update<T>(T item) where T : Model.ModeleBase
        {
            CheckIfInstanceIsInitialized();
            CheckIfUserIsConnected();

            return await Update(this._conn, item, this._errors);
        }
        internal async Task<bool> Delete<T>(T item) where T : Model.ModeleBase
        {
            CheckIfInstanceIsInitialized();
            CheckIfUserIsConnected();

            return await Delete(this._conn, item, this._errors);
        }
        internal async Task<bool> Delete<TItem, TField>(TField value) where TItem : Model.ModeleBase
        {
            CheckIfInstanceIsInitialized();
            CheckIfUserIsConnected();

            return await Delete<TItem, TField>(this._conn, value, this._errors);
        }
        internal async Task<bool> Delete<TItem, TField>(string fieldName, TField value) where TItem : Model.ModeleBase
        {
            CheckIfInstanceIsInitialized();
            CheckIfUserIsConnected();

            return await Delete<TItem, TField>(this._conn, fieldName, value, this._errors);
        }
        internal async Task<long> SelectCount<T>(bool checkUserConnection = true) where T : Model.ModeleBase
        {
            CheckIfInstanceIsInitialized();
            if (checkUserConnection) CheckIfUserIsConnected();

            this._errors.Clear();
            string sqlQuery = "";

            try
            {
                sqlQuery = "SELECT COUNT(*) FROM " + Const.NomTableSelonType<T>();
                return this._conn.ExecuteScalar<long>(sqlQuery);
            }
            catch (Exception ex)
            {
                this._errors.Add(await Tools.Log.LogSQLException(sqlQuery, ex), Enums.ErrorType.Exception, ex);
                return 0;
            }
        }
        internal async Task<long> SelectCount<TItem, TField>(string fieldName, TField value) where TItem : Model.ModeleBase
        {
            CheckIfInstanceIsInitialized();
            CheckIfUserIsConnected();

            this._errors.Clear();
            string sqlQuery = "";

            try
            {
                sqlQuery = "SELECT COUNT(*) FROM " + Const.NomTableSelonType<TItem>() + " WHERE " + fieldName + "=" + Tools.Convert.FormatSQL(value);
                return this._conn.ExecuteScalar<long>(sqlQuery);
            }
            catch (Exception ex)
            {
                this._errors.Add(await Tools.Log.LogSQLException(sqlQuery, ex), Enums.ErrorType.Exception, ex);
                return 0;
            }
        }
        internal async Task<long> SelectCount<TItem, TKey>(TKey key) where TItem : Model.ModeleBase
        {
            CheckIfInstanceIsInitialized();
            CheckIfUserIsConnected();

            this._errors.Clear();

            try
            {
                return await this.SelectCount<TItem, TKey>(Const.DB_COMMON_ID_COLNAME, key);
            }
            catch (Exception ex)
            {
                this._errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
                return 0;
            }
        }
        internal async Task<long> SelectMax<TItem>(string fieldName) where TItem : Model.ModeleBase
        {
            CheckIfInstanceIsInitialized();
            CheckIfUserIsConnected();

            return await SelectMax<TItem>(this._conn, fieldName, this._errors);
        }
        internal async Task<long> SelectMaxKey<TItem>() where TItem : Model.ModeleBase
        {
            CheckIfInstanceIsInitialized();
            CheckIfUserIsConnected();

            return await SelectMaxKey<TItem>(this._conn, this._errors);
        }
        internal async Task<TItem> SelectItem<TItem, TField>(string fieldName, TField value, bool checkUserConnection = true) where TItem : class
        {
            CheckIfInstanceIsInitialized();
            if (checkUserConnection) CheckIfUserIsConnected();

            this._errors.Clear();

            string sqlQuery = "";
            List<TItem> results = null;

            try
            {
                sqlQuery = "SELECT * FROM " + Const.NomTableSelonAttribut<TItem>() + " WHERE " + fieldName + "=" + Tools.Convert.FormatSQL(value) + " LIMIT 1";
            }
            catch (Exception ex)
            {
                this._errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
                return null;
            }

            try
            {
                results = this._conn.Query<TItem>(sqlQuery);
                if (results.Count == 0)
                {
                    this._errors.Add("Aucune ligne n'a été trouvée !\nRequête : " + sqlQuery);
                    return null;
                }
                return results[0];
            }
            catch (Exception ex)
            {
                this._errors.Add(await Tools.Log.LogSQLException(sqlQuery, ex), Enums.ErrorType.Exception, ex);
                return null;
            }
        }
        internal async Task<TItem> SelectItem<TItem, TKey>(TKey key, bool checkUserConnection = true) where TItem : class
        {
            CheckIfInstanceIsInitialized();
            if(checkUserConnection) CheckIfUserIsConnected();

            return await SelectItem<TItem, TKey>(this._conn, key, this._errors);
        }
        internal async Task<ObservableCollection<TItem>> SelectItems<TItem, TField>(string fieldName, TField value) where TItem : Model.ModeleBase
        {
            CheckIfInstanceIsInitialized();
            CheckIfUserIsConnected();

            return await SelectItems<TItem, TField>(this._conn, fieldName, value, this._errors);
        }
        internal async Task<ObservableCollection<TItem>> SelectItems<TItem, TKey>(TKey key) where TItem : Model.ModeleBase
        {
            CheckIfInstanceIsInitialized();
            CheckIfUserIsConnected();

            this._errors.Clear();

            try
            {
                return await this.SelectItems<TItem, TKey>(Const.DB_COMMON_ID_COLNAME, key);
            }
            catch (Exception ex)
            {
                this._errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
                return null;
            }
        }
        internal async Task<ObservableCollection<TItem>> SelectItems<TItem>() where TItem : Model.ModeleBase
        {
            CheckIfInstanceIsInitialized();
            CheckIfUserIsConnected();

            this._errors.Clear();

            try
            {
                return await this.SelectItems<TItem, string>("", "");
            }
            catch (Exception ex)
            {
                this._errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
                return null;
            }
        }
        internal async Task<SearchResult<T>> SelectCriteria<T>(SearchCriteria criteria, long? currentPage, long? itemsCountOnPage, bool checkUserConnection = true) where T : Model.ModeleBase
        {
            CheckIfInstanceIsInitialized();
            if (checkUserConnection) CheckIfUserIsConnected();

            this._errors.Clear();

            SearchResult<T> result = new SearchResult<T>();
            SearchCriteria.Query query = null;

            try
            {
                query = await criteria.GenereQuery<T>(currentPage, itemsCountOnPage, checkUserConnection);
            }
            catch (Exception ex)
            {
                this._errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
                return null;
            }

            if (!query.Errors.IsEmpty)
            {
                this._errors.AddRange(query.Errors);
                return null;
            }

            result.CurrentItemIndex = query.CurrentItemIndex;
            result.CurrentPage = query.CurrentPage;
            result.ItemsCountOnPage = query.ItemsCountOnPage;
            result.PagesCount = query.PagesCount;
            result.TotalItemsCount = query.TotalItemsCount;

            try
            {
                foreach (T item in this._conn.Query<T>(query.SqlQuery))
                {
                    result.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                this._errors.Add(await Tools.Log.LogSQLException(query.SqlQuery, ex), Enums.ErrorType.Exception, ex);
                return null;
            }

            return result;
        }


        internal async Task<List<T>> SelectSynchronize<T>(DateTime? date = null) where T : Model.ModeleBase
        {
            CheckIfInstanceIsInitialized();
            CheckIfUserIsConnected();

            this._errors.Clear();

            List<T> result = new List<T>();

            try
            {
                var query = this._conn.Table<T>();
                if (Tools.Type.TypeIsChildOf<Model.AppartenanceBase>(typeof(T)))
                {
                    if (this._connectedUserName.ToLower() != Const.ADMIN_USERNAME.ToLower())
                        query = query.Where(line => (line as Model.AppartenanceBase).NomUtilisateurCreation.ToLower() == this._connectedUserName.ToLower());
                    if (date != null)
                        query = query.Where(line => (line as Model.AppartenanceBase).DateHeureCreation > date | (line as Model.AppartenanceBase).DateHeureModification > date);
                }
                using (var e = query.GetEnumerator())
                {
                    while (e.Current != null)
                    {
                        result.Add(e.Current);
                        e.MoveNext();
                    }
                }
            }
            catch (Exception ex)
            {
                this._errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
                return null;
            }

            return result;
        }
        public async Task<SynchroList> SelectSynchronize(DateTime? date = null)
        {
            CheckIfInstanceIsInitialized();
            CheckIfUserIsConnected();

            this._errors.Clear();

            Tools.ErrorsList errors = new Tools.ErrorsList();
            SynchroList result = new SynchroList();

            result.BiensImmobiliers = await this.SelectSynchronize<Model.BienImmobilier>(date);
            errors.AddRange(this._errors);
            result.PhotosBienImmobilier = await this.SelectSynchronize<Model.PhotoBienImmobilier>(date);
            errors.AddRange(this._errors);
            result.Personnes = await this.SelectSynchronize<Model.Personne>(date);
            errors.AddRange(this._errors);
            result.Utilisateurs = await this.SelectSynchronize<Model.Utilisateur>(date);
            errors.AddRange(this._errors);

            this._errors.Clear();
            this._errors.AddRange(errors);

            return result;
        }
        internal async Task<bool> Synchronize<T>(List<T> srcItems) where T : Model.ModeleBase
        {
            CheckIfInstanceIsInitialized();
            CheckIfUserIsConnected();

            this._errors.Clear();

            // Création de la table
            if (!await CreateTable<T>()) return false;

            // Mise à jour des lignes
            try
            {
                // Lecture des lignes de la table destination
                List<T> dstItems = await this.SelectSynchronize<T>();

                // Report des lignes sources dans la table destination
                if (srcItems != null)
                {
                    foreach (T item in srcItems)
                    {
                        try
                        {
                            this._conn.InsertOrReplace(item);
                        }
                        catch (Exception ex)
                        {
                            this._errors.Add("Erreur pendant l'insertion ou la mise à jour d'une ligne : " + await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
                            return false;
                        }
                    }
                }

                // Suppression des lignes de la table destination qui ne sont plus dans la source
                if (dstItems != null)
                {
                    foreach (T dstItem in dstItems)
                    {
                        bool exists = false;
                        if (srcItems != null)
                        {
                            foreach (T srcItem in srcItems)
                            {
                                if (dstItem.Id == srcItem.Id)
                                {
                                    exists = true;
                                    break;
                                }
                            }
                        }
                        if (exists) break;

                        try
                        {
                            this._conn.Delete(dstItem);
                        }
                        catch (Exception ex)
                        {
                            this._errors.Add("Erreur pendant la suppression d'une ligne : " + await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this._errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
            }

            return this._errors.IsEmpty;
        }
        public async Task<bool> Synchronize(SynchroList source, bool createParamsTable = false)
        {
            CheckIfInstanceIsInitialized();
            CheckIfUserIsConnected();

            this._errors.Clear();

            Tools.ErrorsList errors = new Tools.ErrorsList();

            await this.Synchronize<Model.BienImmobilier>(source.BiensImmobiliers);
            errors.AddRange(this._errors);
            await this.Synchronize<Model.PhotoBienImmobilier>(source.PhotosBienImmobilier);
            errors.AddRange(this._errors);
            await this.Synchronize<Model.Personne>(source.Personnes);
            errors.AddRange(this._errors);
            await this.Synchronize<Model.Utilisateur>(source.Utilisateurs);
            errors.AddRange(this._errors);

            this._errors.Clear();
            this._errors.AddRange(errors);

            // Création de la table Parametres
            try
            {
                if (createParamsTable)
                {
                    this._conn.CreateTable<Model.Parametre>();
                    if (this._conn.ExecuteScalar<int>("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='" + Const.DB_PARAMETRE_TABLENAME + "'") != 1)
                        this._errors.Add("Impossible de créer ou de mettre à jour la table '" + Const.DB_PARAMETRE_TABLENAME + "'.");
                }
            }
            catch (Exception ex)
            {
                this._errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
            }

            return this._errors.IsEmpty;
        }

        internal async Task<bool> CreateTable<T>() where T : Model.ModeleBase
        {
            CheckIfInstanceIsInitialized();

            this._errors.Clear();

            this._conn.BeginTransaction();

            try
            {
                // Si la table existe déjà
                string name = Const.NomTableSelonType<T>();
                string tmp_name = "";
                if (this._conn.ExecuteScalar<int>("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='" + name + "'") == 1)
                {
                    // On en crée une copie
                    tmp_name = name + "_tmp";
                    this._conn.Execute("CREATE TABLE '" + tmp_name + "' AS SELECT * FROM " + name);

                    // Et on supprime la table actuelle
                    this._conn.Execute("DROP TABLE '" + name + "'");
                }

                // On génère la requête de création de la table
                string query = GenereCreateTableQuery<T>();
                if (string.IsNullOrEmpty(query))
                {
                    this._conn.Rollback();
                    this._errors.Add("Impossible de créer ou de mettre à jour la table '" + name + "' : La requête de création de la table n'est pas valide !\nRequête : " + query);
                    return false;
                }

                // On crée la table
                this._conn.Execute(query);
                if (this._conn.ExecuteScalar<int>("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='" + name + "'") != 1)
                {
                    this._conn.Rollback();
                    this._errors.Add("Impossible de créer ou de mettre à jour la table '" + name + "' : La table n'a pas été créée !\nRequête : " + query);
                    return false;
                }

                // Si une copie existe
                if (!string.IsNullOrEmpty(tmp_name))
                {
                    // On génère la requête de copie des lignes depuis la table temporaire
                    query = GenereCopyTableQuery<T>(this._conn, tmp_name, name);
                    if (string.IsNullOrEmpty(query))
                    {
                        this._conn.Rollback();
                        this._errors.Add("Impossible de créer ou de mettre à jour la table '" + name + "' : La requête de copie de la table temporaire vers la table définitive n'est pas valide !\nRequête : " + query);
                        return false;
                    }

                    // On recopie les lignes depuis la table temporaire
                    this._conn.Execute(query);

                    // On supprime la table temporaire
                    this._conn.Execute("DROP TABLE '" + tmp_name + "'");
                }

                // S'il s'agit de la table des utilisateurs
                if (typeof(T) == typeof(Model.Utilisateur))
                {
                    // On ajoute l'utilisateur par défaut si nécessaire
                    if (this._conn.Find<Model.Utilisateur>(Const.ADMIN_USERNAME) == null)
                    {
                        // Si aucune personne n'existe dans la base, on ajoute de la personne ADMIN
                        if (this._conn.ExecuteScalar<int>("SELECT COUNT(*) FROM " + Const.DB_PERSONNE_TABLENAME) == 0)
                        {
                            Model.Personne p = new Model.Personne();
                            p.Qualite = Const.ADMIN_QUALITY;
                            p.Nom = Const.ADMIN_NAME;
                            p.Prenom = Const.ADMIN_FIRSTNAME;
                            p.NomUtilisateurCreation = Const.ADMIN_USERNAME;
                            p.DateHeureCreation = DateTime.Now;
                            p.DateHeureModification = DateTime.Now;
                            this._conn.Insert(p);
                        }

                        // Lecture de la première personne de la table
                        int result = this._conn.ExecuteScalar<int>("SELECT MIN(" + Const.DB_COMMON_ID_COLNAME + ") FROM " + Const.DB_PERSONNE_TABLENAME);

                        // Ajout de l'utilisateur ADMIN
                        this._conn.Insert(new Model.Utilisateur(result, Const.ADMIN_USERNAME) { MotDePasseCrypte = Tools.Crypto.Encrypt(Const.ADMIN_PASSWORD) });
                    }
                }

                // On valide la transaction
                this._conn.Commit();

                //this._conn.CreateTable<T>(SQLite.Net.Interop.CreateFlags.AllImplicit);
                //if (typeof(T) == typeof(Model.BienImmobilier) && this._conn.ExecuteScalar<int>("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='" + Const.DB_BIEN_TABLENAME + "'") != 1)
                //    this._errors.Add("Impossible de créer ou de mettre à jour la table '" + Const.DB_BIEN_TABLENAME + "'.");
                //else if (typeof(T) == typeof(Model.Personne) && this._conn.ExecuteScalar<int>("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='" + Const.DB_PERSONNE_TABLENAME + "'") != 1)
                //    this._errors.Add("Impossible de créer ou de mettre à jour la table '" + Const.DB_PERSONNE_TABLENAME + "'.");
                //else if (typeof(T) == typeof(Model.Utilisateur))
                //{
                //    if (this._conn.ExecuteScalar<int>("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='" + Const.DB_UTILISATEUR_TABLENAME + "'") != 1)
                //        this._errors.Add("Impossible de créer ou de mettre à jour la table '" + Const.DB_UTILISATEUR_TABLENAME + "'.");
                //    else if (this._conn.Find<Model.Utilisateur>(Const.ADMIN_USERNAME) == null)
                //    {
                //        // Si aucune personne n'existe dans la base, ajout de la personne ADMIN
                //        if (this._conn.ExecuteScalar<int>("SELECT COUNT(*) FROM " + Const.DB_PERSONNE_TABLENAME) == 0)
                //        {
                //            this._conn.Insert(new Model.Personne
                //            {
                //                Qualite = Const.ADMIN_QUALITY,
                //                Nom = Const.ADMIN_NAME,
                //                Prenom = Const.ADMIN_FIRSTNAME,
                //                NomUtilisateurCreation = Const.ADMIN_USERNAME,
                //                DateHeureCreation = DateTime.Now,
                //                DateHeureModification = DateTime.Now
                //            });
                //        }

                    //        // Lecture de la première personne de la table
                    //        int result = this._conn.ExecuteScalar<int>("SELECT MIN(" + Const.DB_COMMON_ID_COLNAME + ") FROM " + Const.DB_PERSONNE_TABLENAME + " WHERE " + Const.DB_PERSONNE_NOM_COLNAME + "='" + Const.ADMIN_NAME + "' AND " + Const.DB_PERSONNE_PRENOM_COLNAME + "='" + Const.ADMIN_FIRSTNAME + "' AND " + Const.DB_PERSONNE_QUALITE_COLNAME + "='" + Const.ADMIN_QUALITY + "'");

                    //        // Ajout de l'utilisateur ADMIN
                    //        this._conn.Insert(new Model.Utilisateur(result, Const.ADMIN_USERNAME) { MotDePasseCrypte = Tools.Crypto.Encrypt(Const.ADMIN_PASSWORD) });
                    //    }
                    //}
            }
            catch (Exception ex)
            {
                this._conn.Rollback();
                this._errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
                return false;
            }

            return this._errors.IsEmpty;
        }
        public async Task<bool> InitializeDatabase(bool createParamsTable = false)
        {
            CheckIfInstanceIsInitialized();

            this._errors.Clear();

            Tools.ErrorsList errors = new Tools.ErrorsList();

            await this.CreateTable<Model.BienImmobilier>();
            errors.AddRange(this._errors);
            await this.CreateTable<Model.PhotoBienImmobilier>();
            errors.AddRange(this._errors);
            await this.CreateTable<Model.Personne>();
            errors.AddRange(this._errors);
            await this.CreateTable<Model.Utilisateur>();
            errors.AddRange(this._errors);

            this._errors.Clear();
            this._errors.AddRange(errors);

            // Création de la table Parametres
            try
            {
                if (createParamsTable)
                {
                    this._conn.CreateTable<Model.Parametre>();
                    if (this._conn.ExecuteScalar<int>("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='" + Const.DB_PARAMETRE_TABLENAME + "'") != 1)
                        this._errors.Add("Impossible de créer ou de mettre à jour la table '" + Const.DB_PARAMETRE_TABLENAME + "'.");
                }
            }
            catch (Exception ex)
            {
                this._errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
            }

            return this._errors.IsEmpty;
        }




        internal async static Task<bool> Insert<T>(object conn, string connectedUserName, T item, Tools.ErrorsList errors) where T : class
        {
            errors.Clear();
            try
            {
                Model.AppartenanceBase prop = item as Model.AppartenanceBase;
                if (prop != null) prop.AffecterCreation(connectedUserName);

                if (conn.GetType() == typeof(SQLiteAsyncConnection))
                    await ((SQLiteAsyncConnection)conn).InsertAsync(item);
                else if (conn.GetType() == typeof(SQLiteConnection))
                    ((SQLiteConnection)conn).Insert(item);
            }
            catch (Exception ex)
            {
                errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
            }

            return errors.IsEmpty;
        }
        internal async static Task<bool> InsertOrReplace<T>(object conn, string connectedUserName, T item, Tools.ErrorsList errors) where T : class
        {
            errors.Clear();

            try
            {
                Model.AppartenanceBase prop = item as Model.AppartenanceBase;
                if (prop != null) prop.AffecterCreation(connectedUserName);

                if (conn.GetType() == typeof(SQLiteAsyncConnection))
                    await ((SQLiteAsyncConnection)conn).InsertOrReplaceAsync(item);
                else if (conn.GetType() == typeof(SQLiteConnection))
                    ((SQLiteConnection)conn).InsertOrReplace(item);
            }
            catch (Exception ex)
            {
                errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
            }

            return errors.IsEmpty;
        }
        internal async static Task<bool> Update<T>(object conn, T item, Tools.ErrorsList errors) where T : Model.ModeleBase
        {
            errors.Clear();

            try
            {
                Model.AppartenanceBase prop = item as Model.AppartenanceBase;
                if (prop != null) prop.AffecterModification();

                if (conn.GetType() == typeof(SQLiteAsyncConnection))
                    await ((SQLiteAsyncConnection)conn).UpdateAsync(item);
                else if (conn.GetType() == typeof(SQLiteConnection))
                    ((SQLiteConnection)conn).Update(item);
            }
            catch (Exception ex)
            {
                errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
            }

            return errors.IsEmpty;
        }
        internal async static Task<bool> Delete<T>(object conn, T item, Tools.ErrorsList errors) where T : Model.ModeleBase
        {
            errors.Clear();

            try
            {
                if (conn.GetType() == typeof(SQLiteAsyncConnection))
                    await ((SQLiteAsyncConnection)conn).DeleteAsync(item);
                else if (conn.GetType() == typeof(SQLiteConnection))
                    ((SQLiteConnection)conn).Delete(item);
            }
            catch (Exception ex)
            {
                errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
            }

            return errors.IsEmpty;
        }
        internal async static Task<bool> Delete<TItem, TField>(object conn, TField value, Tools.ErrorsList errors) where TItem : Model.ModeleBase
        {
            errors.Clear();

            try
            {
                TItem item = await SelectItem<TItem, TField>(conn, value, errors);
                if (item == null) return false;
                if (conn.GetType() == typeof(SQLiteAsyncConnection))
                    await ((SQLiteAsyncConnection)conn).DeleteAsync(item);
                else if (conn.GetType() == typeof(SQLiteConnection))
                    ((SQLiteConnection)conn).Delete(item);
            }
            catch (Exception ex)
            {
                errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
            }

            return errors.IsEmpty;
        }
        internal async static Task<bool> Delete<TItem, TField>(object conn, string fieldName, TField value, Tools.ErrorsList errors) where TItem : Model.ModeleBase
        {
            errors.Clear();

            try
            {
                ObservableCollection<TItem> items = await SelectItems<TItem, TField>(conn, fieldName, value, errors);
                if (items == null) return false;

                foreach (TItem item in items)
                {
                    try
                    {
                        if (conn.GetType() == typeof(SQLiteAsyncConnection))
                            await ((SQLiteAsyncConnection)conn).DeleteAsync(item);
                        else if (conn.GetType() == typeof(SQLiteConnection))
                            ((SQLiteConnection)conn).Delete(item);
                    }
                    catch (Exception ex)
                    {
                        errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
            }

            return errors.IsEmpty;
        }
        internal async static Task<long> SelectMax<TItem>(object conn, string fieldName, Tools.ErrorsList errors) where TItem : Model.ModeleBase
        {
            errors.Clear();
            string sqlQuery = "";

            try
            {
                sqlQuery = "SELECT MAX(" + fieldName + ") FROM " + Const.NomTableSelonType<TItem>();
                if (conn.GetType() == typeof(SQLiteAsyncConnection))
                    return await ((SQLiteAsyncConnection)conn).ExecuteScalarAsync<long>(sqlQuery);
                else if (conn.GetType() == typeof(SQLiteConnection))
                    return ((SQLiteConnection)conn).ExecuteScalar<long>(sqlQuery);

                return -1;
            }
            catch (Exception ex)
            {
                errors.Add(await Tools.Log.LogSQLException(sqlQuery, ex), Enums.ErrorType.Exception, ex);
                return -1;
            }
        }
        internal async static Task<long> SelectMaxKey<TItem>(object conn, Tools.ErrorsList errors) where TItem : Model.ModeleBase
        {
            errors.Clear();

            try
            {
                return await SelectMax<TItem>(conn, Const.DB_COMMON_ID_COLNAME, errors);
            }
            catch (Exception ex)
            {
                errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
                return 0;
            }
        }
        internal async static Task<TItem> SelectItem<TItem, TKey>(object conn, TKey key, Tools.ErrorsList errors) where TItem : class
        {
            errors.Clear();

            try
            {
                if (conn.GetType() == typeof(SQLiteAsyncConnection))
                    return await ((SQLiteAsyncConnection)conn).FindAsync<TItem>(key);
                else if (conn.GetType() == typeof(SQLiteConnection))
                    return ((SQLiteConnection)conn).Find<TItem>(key);

                return null;
            }
            catch (Exception ex)
            {
                errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
                return null;
            }
        }
        internal async static Task<ObservableCollection<TItem>> SelectItems<TItem, TField>(object conn, string fieldName, TField value, Tools.ErrorsList errors) where TItem : Model.ModeleBase
        {
            errors.Clear();

            string sqlQuery = "";
            List<TItem> results = null;

            try
            {
                sqlQuery = "SELECT * FROM " + Const.NomTableSelonType<TItem>();
                if (!string.IsNullOrEmpty(fieldName)) sqlQuery += " WHERE " + fieldName + "=" + Tools.Convert.FormatSQL(value);
            }
            catch (Exception ex)
            {
                errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
                return null;
            }

            try
            {
                if (conn.GetType() == typeof(SQLiteAsyncConnection))
                    results = await ((SQLiteAsyncConnection)conn).QueryAsync<TItem>(sqlQuery);
                else if (conn.GetType() == typeof(SQLiteConnection))
                    results = ((SQLiteConnection)conn).Query<TItem>(sqlQuery);

                if (results.Count == 0)
                {
                    errors.Add("Aucune ligne n'a été trouvée !\nRequête : " + sqlQuery);
                    return null;
                }

                ObservableCollection<TItem> col = new ObservableCollection<TItem>();
                foreach (TItem item in results) col.Add(item);

                return col;
            }
            catch (Exception ex)
            {
                errors.Add(await Tools.Log.LogSQLException(sqlQuery, ex), Enums.ErrorType.Exception, ex);
                return null;
            }
        }
        internal static string GenereCreateTableQuery<T>() where T : Model.ModeleBase
        {
            System.Type t = typeof(T);
            string tableName = Const.NomTableSelonAttribut<T>();
            if (string.IsNullOrEmpty(tableName)) return "";

            int nbProp = 0;
            string query = "CREATE TABLE '" + tableName + "' (";
            foreach (PropertyInfo p in t.GetRuntimeProperties())
            {
                string name = "", type = "", notnull = "", pkey = "", increment = "";
                Attribute attr = null;

                // Column
                attr = p.GetCustomAttribute(typeof(ColumnAttribute));
                if (attr == null) continue;
                name = ((ColumnAttribute)attr).Name;
                if (string.IsNullOrEmpty(name)) continue;

                // Type
                System.Type pt = p.PropertyType;
                if (pt.GenericTypeArguments.Length > 0) pt = pt.GenericTypeArguments[0];

                if (pt == typeof(int) || pt == typeof(long) || pt == typeof(short) || pt.GetTypeInfo().BaseType == typeof(Enum))
                    type = "integer";
                else if (pt == typeof(float) || pt == typeof(double))
                    type = "real";
                else if (pt == typeof(decimal))
                    type = "numeric";
                else if (pt == typeof(bool))
                    type = "boolean";
                else if (pt == typeof(DateTime))
                    type = "datetime";
                else if (pt == typeof(string))
                    type = "text";
                else
                    continue;

                // NotNull
                attr = p.GetCustomAttribute(typeof(NotNullAttribute));
                if (attr != null) notnull = "not null";

                // PrimaryKey
                attr = p.GetCustomAttribute(typeof(PrimaryKeyAttribute));
                if (attr != null) pkey = "primary key";

                // AutoIncrement
                attr = p.GetCustomAttribute(typeof(AutoIncrementAttribute));
                if (attr != null) increment = "autoincrement";

                // Génération de la ligne
                if (nbProp > 0) query += ",";
                query += "'" + name + "' " + type + (notnull == "" ? "" : " " + notnull) + (pkey == "" ? "" : " " + pkey) + (increment == "" ? "" : " " + increment);
                nbProp++;
            }
            query += ")";

            if (nbProp == 0) return "";
            return query;
        }
        internal static string GenereCopyTableQuery<T>(SQLiteConnection conn, string originName, string destName) where T : Model.ModeleBase
        {
            System.Type t = typeof(T);
            Attribute attr;

            // Lecture des colonnes existantes dans l'origine
            var originCols = conn.GetTableInfo(originName);
            // Lecture des colonnes existantes dans la destination
            var destCols = conn.GetTableInfo(destName);

            int nbProp = 0;
            string cols = "";
            foreach (PropertyInfo p in t.GetRuntimeProperties())
            {
                string name = "";
                attr = null;

                // Column
                attr = p.GetCustomAttribute(typeof(ColumnAttribute));
                if (attr == null) continue;
                name = ((ColumnAttribute)attr).Name;
                if (string.IsNullOrEmpty(name)) continue;

                // AutoIncrement
                attr = p.GetCustomAttribute(typeof(AutoIncrementAttribute));
                if (attr != null) continue;

                // Vérification de l'existence des deux côtés
                if (!originCols.Contains(item => item.Name.ToLower() == name.ToLower()) || !destCols.Contains(item => item.Name.ToLower() == name.ToLower()))
                    continue;

                // Génération de la ligne
                if (nbProp > 0) cols += ",";
                cols += name;
                nbProp++;
            }
            if (nbProp == 0) return "";

            return "INSERT INTO '" + destName + "' (" + cols + ")  SELECT " + cols + " FROM '" + originName + "'";
        }







        //private static Connection _connection = null;
        //public static async Task<Connection> GetCurrent(SQLite.Net.Interop.ISQLitePlatform sqlitePlatform = null, string databasePath = "")
        //{
        //    if (_connection == null || !_connection._initialized)
        //    {
        //        _connection = new Connection();

        //        if (sqlitePlatform == null)
        //            _connection._errors.Add("Aucune plateforme n'est fournie alors que la connexion n'est pas encore initialisée !");
        //        if (string.IsNullOrEmpty(databasePath))
        //            _connection._errors.Add("Aucun chemin à la base de données n'est fourni alors que la connexion n'est pas encore initialisée !");
        //        if (!_connection._errors.IsEmpty) return _connection;

        //        await _connection.Initialize(sqlitePlatform, databasePath);
        //    }
        //    return _connection;
        //}

        //private SQLiteAsyncConnection _conn;
        //private bool _initialized;
        //private string _databasePath;
        //private string _connectedUserName;
        //private Model.Utilisateur _connectedUser;
        //private Tools.ErrorsList _errors;

        //public string DatabasePath
        //{
        //    get { return _databasePath; }
        //}
        //public bool UserIsConnected
        //{
        //    get { return !string.IsNullOrEmpty(this._connectedUserName); }
        //}
        //public string ConnectedUserName
        //{
        //    get { return _connectedUserName; }
        //}
        //public Model.Utilisateur ConnectedUser
        //{
        //    get
        //    {
        //        if (!_initialized) return null;
        //        if (string.IsNullOrEmpty(_connectedUserName)) return null;
        //        if (_connectedUser != null && _connectedUser.NomUtilisateur.ToLower() != _connectedUserName.ToLower()) _connectedUser = null;
        //        if (_connectedUser == null) _connectedUser = SelectItem<Model.Utilisateur, string>(Const.DB_UTILISATEUR_NOMUTILISATEUR_COLNAME, _connectedUserName).ExecuteSynchronously();
        //        return null;
        //    }
        //}
        //public Tools.ErrorsList Errors
        //{
        //    get { return _errors; }
        //}


        //private Connection()
        //{
        //    this._initialized = false;
        //    this._databasePath = "";
        //    this._connectedUserName = "";
        //    this._connectedUser = null;
        //    this._errors = new Tools.ErrorsList();
        //}

        //private async Task<bool> Initialize(SQLite.Net.Interop.ISQLitePlatform sqlitePlatform, string databasePath)
        //{
        //    _errors.Clear();
        //    _databasePath = "";
        //    _connectedUserName = "";

        //    try
        //    {
        //        _databasePath = databasePath.Replace(@"\", @"/");
        //        _conn = new SQLiteAsyncConnection(() => new SQLiteConnectionWithLock(sqlitePlatform, new SQLiteConnectionString(_databasePath, false)));
        //        this._initialized = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        _errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
        //        this._initialized = false;
        //    }

        //    return _errors.IsEmpty;
        //}

        //private void CheckIfInstanceIsInitialized()
        //{
        //    if (!this._initialized) throw new NotInitializedException<Connection>(this._errors);
        //}
        //private void CheckIfUserIsConnected()
        //{
        //    if (!UserIsConnected) throw new NotUserConnectedException(this._errors);
        //}
        //internal async Task RunInTransaction(Action<SQLiteConnection> action)
        //{
        //    CheckIfInstanceIsInitialized();
        //    CheckIfUserIsConnected();
        //    await this._conn.RunInTransactionAsync(action);
        //}


        //internal async Task<bool> ConnectUser(string userName, string cyptedPassword)
        //{
        //    CheckIfInstanceIsInitialized();

        //    this._errors.Clear();
        //    this._connectedUserName = "";
        //    if (string.IsNullOrEmpty(userName))
        //    {
        //        this._errors.Add("Un nom d'utilisateur est obligatoire !");
        //        return false;
        //    }

        //    Model.Utilisateur user = null;
        //    try
        //    {
        //        user = await this._conn.FindAsync<Model.Utilisateur>(userName);
        //        if (user == null || string.IsNullOrEmpty(user.NomUtilisateur))
        //        {
        //            this._errors.Add("L'utilisateur '" + userName + "' n'existe pas.");
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
        //        return false;
        //    }

        //    if (cyptedPassword != user.MotDePasseCrypte)
        //    {
        //        this._errors.Add("Le mot de passe est invalide.");
        //        return false;
        //    }

        //    this._connectedUserName = user.NomUtilisateur;

        //    return _errors.IsEmpty;
        //}
        //internal bool DisconnectUser(string userName)
        //{
        //    if (this._connectedUserName.ToUpper() != userName.ToUpper()) return false;
        //    this._connectedUserName = "";
        //    return true;
        //}


        //internal async Task<bool> Insert<T>(T item) where T : class
        //{
        //    CheckIfInstanceIsInitialized();
        //    CheckIfUserIsConnected();

        //    return await Insert(this._conn, this._connectedUserName, item, this._errors);
        //}
        //internal async Task<bool> InsertOrReplace<T>(T item) where T : class
        //{
        //    CheckIfInstanceIsInitialized();
        //    CheckIfUserIsConnected();

        //    return await InsertOrReplace(this._conn, this._connectedUserName, item, this._errors);
        //}
        //internal async Task<bool> Update<T>(T item) where T : Model.ModeleBase
        //{
        //    CheckIfInstanceIsInitialized();
        //    CheckIfUserIsConnected();

        //    return await Update(this._conn, item, this._errors);
        //}
        //internal async Task<bool> Delete<T>(T item) where T : Model.ModeleBase
        //{
        //    CheckIfInstanceIsInitialized();
        //    CheckIfUserIsConnected();

        //    return await Delete(this._conn, item, this._errors);
        //}
        //internal async Task<bool> Delete<TItem, TField>(TField value) where TItem : Model.ModeleBase
        //{
        //    CheckIfInstanceIsInitialized();
        //    CheckIfUserIsConnected();

        //    return await Delete<TItem, TField>(this._conn, value, this._errors);
        //}
        //internal async Task<bool> Delete<TItem, TField>(string fieldName, TField value) where TItem : Model.ModeleBase
        //{
        //    CheckIfInstanceIsInitialized();
        //    CheckIfUserIsConnected();

        //    return await Delete<TItem, TField>(this._conn, fieldName, value, this._errors);
        //}
        //internal async Task<long> SelectCount<T>() where T : Model.ModeleBase
        //{
        //    CheckIfInstanceIsInitialized();
        //    CheckIfUserIsConnected();

        //    this._errors.Clear();
        //    string sqlQuery = "";

        //    try
        //    {
        //        sqlQuery = "SELECT COUNT(*) FROM " + Const.NomTableSelonType<T>();
        //        return await this._conn.ExecuteScalarAsync<long>(sqlQuery);
        //    }
        //    catch (Exception ex)
        //    {
        //        this._errors.Add(await Tools.Log.LogSQLException(sqlQuery, ex), Enums.ErrorType.Exception, ex);
        //        return 0;
        //    }
        //}
        //internal async Task<long> SelectCount<TItem, TField>(string fieldName, TField value) where TItem : Model.ModeleBase
        //{
        //    CheckIfInstanceIsInitialized();
        //    CheckIfUserIsConnected();

        //    this._errors.Clear();
        //    string sqlQuery = "";

        //    try
        //    {
        //        sqlQuery = "SELECT COUNT(*) FROM " + Const.NomTableSelonType<TItem>() + " WHERE " + fieldName + "=" + Tools.Convert.FormatSQL(value);
        //        return await this._conn.ExecuteScalarAsync<long>(sqlQuery);
        //    }
        //    catch (Exception ex)
        //    {
        //        this._errors.Add(await Tools.Log.LogSQLException(sqlQuery, ex), Enums.ErrorType.Exception, ex);
        //        return 0;
        //    }
        //}
        //internal async Task<long> SelectCount<TItem, TKey>(TKey key) where TItem : Model.ModeleBase
        //{
        //    CheckIfInstanceIsInitialized();
        //    CheckIfUserIsConnected();

        //    this._errors.Clear();

        //    try
        //    {
        //        return await this.SelectCount<TItem, TKey>(Const.DB_COMMON_ID_COLNAME, key);
        //    }
        //    catch (Exception ex)
        //    {
        //        this._errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
        //        return 0;
        //    }
        //}
        //internal async Task<long> SelectMax<TItem>(string fieldName) where TItem : Model.ModeleBase
        //{
        //    CheckIfInstanceIsInitialized();
        //    CheckIfUserIsConnected();

        //    return await SelectMax<TItem>(this._conn, fieldName, this._errors);
        //}
        //internal async Task<long> SelectMaxKey<TItem>() where TItem : Model.ModeleBase
        //{
        //    CheckIfInstanceIsInitialized();
        //    CheckIfUserIsConnected();

        //    return await SelectMaxKey<TItem>(this._conn, this._errors);
        //}
        //internal async Task<TItem> SelectItem<TItem, TField>(string fieldName, TField value) where TItem : Model.ModeleBase
        //{
        //    CheckIfInstanceIsInitialized();
        //    CheckIfUserIsConnected();

        //    this._errors.Clear();

        //    string sqlQuery = "";
        //    List<TItem> results = null;

        //    try
        //    {
        //        sqlQuery = "SELECT * FROM " + Const.NomTableSelonType<TItem>() + " WHERE " + fieldName + "=" + Tools.Convert.FormatSQL(value) + " LIMIT 1";
        //    }
        //    catch (Exception ex)
        //    {
        //        this._errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
        //        return null;
        //    }

        //    try
        //    {
        //        results = await this._conn.QueryAsync<TItem>(sqlQuery);
        //        if (results.Count == 0)
        //        {
        //            this._errors.Add("Aucune ligne n'a été trouvée !\nRequête : " + sqlQuery);
        //            return null;
        //        }
        //        return results[0];
        //    }
        //    catch (Exception ex)
        //    {
        //        this._errors.Add(await Tools.Log.LogSQLException(sqlQuery, ex), Enums.ErrorType.Exception, ex);
        //        return null;
        //    }
        //}
        //internal async Task<TItem> SelectItem<TItem, TKey>(TKey key) where TItem : class
        //{
        //    CheckIfInstanceIsInitialized();
        //    CheckIfUserIsConnected();

        //    return await SelectItem<TItem, TKey>(this._conn, key, this._errors);
        //}
        //internal async Task<ObservableCollection<TItem>> SelectItems<TItem, TField>(string fieldName, TField value) where TItem : Model.ModeleBase
        //{
        //    CheckIfInstanceIsInitialized();
        //    CheckIfUserIsConnected();

        //    return await SelectItems<TItem, TField>(this._conn, fieldName, value, this._errors);
        //}
        //internal async Task<ObservableCollection<TItem>> SelectItems<TItem, TKey>(TKey key) where TItem : Model.ModeleBase
        //{
        //    CheckIfInstanceIsInitialized();
        //    CheckIfUserIsConnected();

        //    this._errors.Clear();

        //    try
        //    {
        //        return await this.SelectItems<TItem, TKey>(Const.DB_COMMON_ID_COLNAME, key);
        //    }
        //    catch (Exception ex)
        //    {
        //        this._errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
        //        return null;
        //    }
        //}
        //internal async Task<ObservableCollection<TItem>> SelectItems<TItem>() where TItem : Model.ModeleBase
        //{
        //    CheckIfInstanceIsInitialized();
        //    CheckIfUserIsConnected();

        //    this._errors.Clear();

        //    try
        //    {
        //        return await this.SelectItems<TItem, string>("", "");
        //    }
        //    catch (Exception ex)
        //    {
        //        this._errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
        //        return null;
        //    }
        //}
        //internal async Task<SearchResult<T>> SelectCriteria<T>(SearchCriteria criteria, long? currentPage, long? itemsCountOnPage) where T : Model.ModeleBase
        //{
        //    CheckIfInstanceIsInitialized();
        //    CheckIfUserIsConnected();

        //    this._errors.Clear();

        //    SearchResult<T> result = new SearchResult<T>();
        //    SearchCriteria.Query query = null;

        //    try
        //    {
        //        query = await criteria.GenereQuery<T>(currentPage, itemsCountOnPage);
        //    }
        //    catch (Exception ex)
        //    {
        //        this._errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
        //        return null;
        //    }

        //    if (!query.Errors.IsEmpty)
        //    {
        //        this._errors.AddRange(query.Errors);
        //        return null;
        //    }

        //    result.CurrentItemIndex = query.CurrentItemIndex;
        //    result.CurrentPage = query.CurrentPage;
        //    result.ItemsCountOnPage = query.ItemsCountOnPage;
        //    result.PagesCount = query.PagesCount;
        //    result.TotalItemsCount = query.TotalItemsCount;

        //    try
        //    {
        //        foreach (T item in await this._conn.QueryAsync<T>(query.SqlQuery))
        //        {
        //            result.Items.Add(item);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        this._errors.Add(await Tools.Log.LogSQLException(query.SqlQuery, ex), Enums.ErrorType.Exception, ex);
        //        return null;
        //    }

        //    return result;
        //}


        //internal async Task<List<T>> SelectSynchronize<T>(DateTime? date = null) where T : Model.ModeleBase
        //{
        //    CheckIfInstanceIsInitialized();
        //    CheckIfUserIsConnected();

        //    this._errors.Clear();

        //    List<T> result = new List<T>();

        //    try
        //    {
        //        var query = this._conn.Table<T>();
        //        if (Tools.Type.TypeIsChildOf<Model.AppartenanceBase>(typeof(T)))
        //        {
        //            if (this._connectedUserName.ToLower() != Const.ADMIN_USERNAME.ToLower())
        //                query = query.Where(line => (line as Model.AppartenanceBase).NomUtilisateurCreation.ToLower() == this._connectedUserName.ToLower());
        //            if (date != null)
        //                query = query.Where(line => (line as Model.AppartenanceBase).DateHeureCreation > date | (line as Model.AppartenanceBase).DateHeureModification > date);
        //        }
        //        result = await query.ToListAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        this._errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
        //        return null;
        //    }

        //    return result;
        //}
        //internal async Task<SynchroList> SelectSynchronize(DateTime? date = null)
        //{
        //    CheckIfInstanceIsInitialized();
        //    CheckIfUserIsConnected();

        //    this._errors.Clear();

        //    Tools.ErrorsList errors = new Tools.ErrorsList();
        //    SynchroList result = new SynchroList();

        //    result.BiensImmobiliers = await this.SelectSynchronize<Model.BienImmobilier>(date);
        //    errors.AddRange(this._errors);
        //    result.PhotosBienImmobilier = await this.SelectSynchronize<Model.PhotoBienImmobilier>(date);
        //    errors.AddRange(this._errors);
        //    result.Personnes = await this.SelectSynchronize<Model.Personne>(date);
        //    errors.AddRange(this._errors);
        //    result.Utilisateurs = await this.SelectSynchronize<Model.Utilisateur>(date);
        //    errors.AddRange(this._errors);

        //    this._errors.Clear();
        //    this._errors.AddRange(errors);

        //    return result;
        //}
        //internal async Task<bool> Synchronize<T>(List<T> srcItems) where T : Model.ModeleBase
        //{
        //    CheckIfInstanceIsInitialized();
        //    CheckIfUserIsConnected();

        //    this._errors.Clear();

        //    // Création de la table
        //    if (!await CreateTable<T>()) return false;

        //    // Mise à jour des lignes
        //    try
        //    {
        //        // Lecture des lignes de la table destination
        //        List<T> dstItems = await this.SelectSynchronize<T>();

        //        // Report des lignes sources dans la table destination
        //        foreach (T item in srcItems)
        //        {
        //            try
        //            {
        //                await this._conn.InsertOrReplaceAsync(item);
        //            }
        //            catch (Exception ex)
        //            {
        //                this._errors.Add("Erreur pendant l'insertion ou la mise à jour d'une ligne : " + await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
        //                return false;
        //            }
        //        }

        //        // Suppression des lignes de la table destination qui ne sont plus dans la source
        //        foreach (T dstItem in dstItems)
        //        {
        //            bool exists = false;
        //            foreach (T srcItem in srcItems)
        //            {
        //                if (dstItem.Id == srcItem.Id)
        //                {
        //                    exists = true;
        //                    break;
        //                }
        //            }
        //            if (exists) break;

        //            try
        //            {
        //                await this._conn.DeleteAsync(dstItem);
        //            }
        //            catch (Exception ex)
        //            {
        //                this._errors.Add("Erreur pendant la suppression d'une ligne : " + await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
        //                return false;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        this._errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
        //    }

        //    return this._errors.IsEmpty;
        //}
        //internal async Task<bool> Synchronize(SynchroList source, bool createParamsTable = false)
        //{
        //    CheckIfInstanceIsInitialized();
        //    CheckIfUserIsConnected();

        //    this._errors.Clear();

        //    Tools.ErrorsList errors = new Tools.ErrorsList();

        //    await this.Synchronize<Model.BienImmobilier>(source.BiensImmobiliers);
        //    errors.AddRange(this._errors);
        //    await this.Synchronize<Model.PhotoBienImmobilier>(source.PhotosBienImmobilier);
        //    errors.AddRange(this._errors);
        //    await this.Synchronize<Model.Personne>(source.Personnes);
        //    errors.AddRange(this._errors);
        //    await this.Synchronize<Model.Utilisateur>(source.Utilisateurs);
        //    errors.AddRange(this._errors);

        //    this._errors.Clear();
        //    this._errors.AddRange(errors);

        //    // Création de la table Parametres
        //    try
        //    {
        //        if (createParamsTable)
        //        {
        //            CreateTablesResult createResult = await this._conn.CreateTableAsync<Model.Parametre>();
        //            int value;
        //            if (!createResult.Results.TryGetValue(typeof(Model.Parametre), out value) || value == 0)
        //                this._errors.Add("Impossible de créer ou de mettre à jour la table '" + Const.DB_PARAMETRE_TABLENAME + "'.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        this._errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
        //    }

        //    return this._errors.IsEmpty;
        //}

        //internal async Task<bool> CreateTable<T>() where T : Model.ModeleBase
        //{
        //    CheckIfInstanceIsInitialized();

        //    this._errors.Clear();

        //    // Création de la table
        //    try
        //    {
        //        CreateTablesResult result = await this._conn.CreateTablesAsync(typeof(T)); //.CreateTableAsync<T>();
        //        int value;
        //        if (typeof(T) == typeof(Model.BienImmobilier) && (!result.Results.TryGetValue(typeof(Model.BienImmobilier), out value) || value == 0))
        //            this._errors.Add("Impossible de créer ou de mettre à jour la table '" + Const.DB_BIEN_TABLENAME + "'.");
        //        else if (typeof(T) == typeof(Model.Personne) && (!result.Results.TryGetValue(typeof(Model.Personne), out value) || value == 0))
        //            this._errors.Add("Impossible de créer ou de mettre à jour la table '" + Const.DB_PERSONNE_TABLENAME + "'.");
        //        else if (typeof(T) == typeof(Model.Utilisateur))
        //        {
        //            if (!result.Results.TryGetValue(typeof(Model.Utilisateur), out value) || value == 0)
        //                this._errors.Add("Impossible de créer ou de mettre à jour la table '" + Const.DB_UTILISATEUR_TABLENAME + "'.");
        //            else if (await this._conn.FindAsync<Model.Utilisateur>(Const.ADMIN_USERNAME) == null)
        //            {
        //                // Si aucune personne n'existe dans la base, ajout de la personne ADMIN
        //                if (await this._conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM " + Const.DB_PERSONNE_TABLENAME, null) == 0)
        //                {
        //                    value = await this._conn.InsertAsync(new Model.Personne
        //                    {
        //                        Qualite = Const.ADMIN_QUALITY,
        //                        Nom = Const.ADMIN_NAME,
        //                        Prenom = Const.ADMIN_FIRSTNAME,
        //                        NomUtilisateurCreation = Const.ADMIN_USERNAME,
        //                        DateHeureCreation = DateTime.Now,
        //                        DateHeureModification = DateTime.Now
        //                    });
        //                }

        //                // Lecture de la première personne de la table
        //                value = await this._conn.ExecuteScalarAsync<int>("SELECT MIN(" + Const.DB_COMMON_ID_COLNAME + ") FROM " + Const.DB_PERSONNE_TABLENAME, null);

        //                // Ajout de l'utilisateur ADMIN
        //                await this._conn.InsertAsync(new Model.Utilisateur(value, Const.ADMIN_USERNAME) { MotDePasseCrypte = Tools.Crypto.Encrypt(Const.ADMIN_PASSWORD) });
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        this._errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
        //        return false;
        //    }

        //    return this._errors.IsEmpty;
        //}
        //public async Task<bool> InitializeDatabase(bool createParamsTable = false)
        //{
        //    CheckIfInstanceIsInitialized();

        //    this._errors.Clear();

        //    Tools.ErrorsList errors = new Tools.ErrorsList();

        //    await this.CreateTable<Model.BienImmobilier>();
        //    errors.AddRange(this._errors);
        //    await this.CreateTable<Model.PhotoBienImmobilier>();
        //    errors.AddRange(this._errors);
        //    await this.CreateTable<Model.Personne>();
        //    errors.AddRange(this._errors);
        //    await this.CreateTable<Model.Utilisateur>();
        //    errors.AddRange(this._errors);

        //    this._errors.Clear();
        //    this._errors.AddRange(errors);

        //    // Création de la table Parametres
        //    try
        //    {
        //        if (createParamsTable)
        //        {
        //            CreateTablesResult createResult = await this._conn.CreateTableAsync<Model.Parametre>();
        //            int value;
        //            if (!createResult.Results.TryGetValue(typeof(Model.Parametre), out value) || value == 0)
        //                this._errors.Add("Impossible de créer ou de mettre à jour la table '" + Const.DB_PARAMETRE_TABLENAME + "'.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        this._errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
        //    }

        //    return this._errors.IsEmpty;
        //}



        //internal async static Task<bool> Insert<T>(object conn, string connectedUserName, T item, Tools.ErrorsList errors) where T : class
        //{
        //    errors.Clear();
        //    try
        //    {
        //        Model.AppartenanceBase prop = item as Model.AppartenanceBase;
        //        if (prop != null) prop.AffecterCreation(connectedUserName);

        //        if (conn.GetType() == typeof(SQLiteAsyncConnection))
        //            await ((SQLiteAsyncConnection)conn).InsertAsync(item);
        //        else if (conn.GetType() == typeof(SQLiteConnection))
        //            ((SQLiteConnection)conn).Insert(item);
        //    }
        //    catch (Exception ex)
        //    {
        //        errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
        //    }

        //    return errors.IsEmpty;
        //}
        //internal async static Task<bool> InsertOrReplace<T>(object conn, string connectedUserName, T item, Tools.ErrorsList errors) where T : class
        //{
        //    errors.Clear();

        //    try
        //    {
        //        Model.AppartenanceBase prop = item as Model.AppartenanceBase;
        //        if (prop != null) prop.AffecterCreation(connectedUserName);

        //        if (conn.GetType() == typeof(SQLiteAsyncConnection))
        //            await ((SQLiteAsyncConnection)conn).InsertOrReplaceAsync(item);
        //        else if (conn.GetType() == typeof(SQLiteConnection))
        //            ((SQLiteConnection)conn).InsertOrReplace(item);
        //    }
        //    catch (Exception ex)
        //    {
        //        errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
        //    }

        //    return errors.IsEmpty;
        //}
        //internal async static Task<bool> Update<T>(object conn, T item, Tools.ErrorsList errors) where T : Model.ModeleBase
        //{
        //    errors.Clear();

        //    try
        //    {
        //        Model.AppartenanceBase prop = item as Model.AppartenanceBase;
        //        if (prop != null) prop.AffecterModification();

        //        if (conn.GetType() == typeof(SQLiteAsyncConnection))
        //            await ((SQLiteAsyncConnection)conn).UpdateAsync(item);
        //        else if (conn.GetType() == typeof(SQLiteConnection))
        //            ((SQLiteConnection)conn).Update(item);
        //    }
        //    catch (Exception ex)
        //    {
        //        errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
        //    }

        //    return errors.IsEmpty;
        //}
        //internal async static Task<bool> Delete<T>(object conn, T item, Tools.ErrorsList errors) where T : Model.ModeleBase
        //{
        //    errors.Clear();

        //    try
        //    {
        //        if (conn.GetType() == typeof(SQLiteAsyncConnection))
        //            await ((SQLiteAsyncConnection)conn).DeleteAsync(item);
        //        else if (conn.GetType() == typeof(SQLiteConnection))
        //            ((SQLiteConnection)conn).Delete(item);
        //    }
        //    catch (Exception ex)
        //    {
        //        errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
        //    }

        //    return errors.IsEmpty;
        //}
        //internal async static Task<bool> Delete<TItem, TField>(object conn, TField value, Tools.ErrorsList errors) where TItem : Model.ModeleBase
        //{
        //    errors.Clear();

        //    try
        //    {
        //        TItem item = await SelectItem<TItem, TField>(conn, value, errors);
        //        if (item == null) return false;
        //        if (conn.GetType() == typeof(SQLiteAsyncConnection))
        //            await ((SQLiteAsyncConnection)conn).DeleteAsync(item);
        //        else if (conn.GetType() == typeof(SQLiteConnection))
        //            ((SQLiteConnection)conn).Delete(item);
        //    }
        //    catch (Exception ex)
        //    {
        //        errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
        //    }

        //    return errors.IsEmpty;
        //}
        //internal async static Task<bool> Delete<TItem, TField>(object conn, string fieldName, TField value, Tools.ErrorsList errors) where TItem : Model.ModeleBase
        //{
        //    errors.Clear();

        //    try
        //    {
        //        ObservableCollection<TItem> items = await SelectItems<TItem, TField>(conn, fieldName, value, errors);
        //        if (items == null) return false;

        //        foreach (TItem item in items)
        //        {
        //            try
        //            {
        //                if (conn.GetType() == typeof(SQLiteAsyncConnection))
        //                    await ((SQLiteAsyncConnection)conn).DeleteAsync(item);
        //                else if (conn.GetType() == typeof(SQLiteConnection))
        //                    ((SQLiteConnection)conn).Delete(item);
        //            }
        //            catch (Exception ex)
        //            {
        //                errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
        //    }

        //    return errors.IsEmpty;
        //}
        //internal async static Task<long> SelectMax<TItem>(object conn, string fieldName, Tools.ErrorsList errors) where TItem : Model.ModeleBase
        //{
        //    errors.Clear();
        //    string sqlQuery = "";

        //    try
        //    {
        //        sqlQuery = "SELECT MAX(" + fieldName + ") FROM " + Const.NomTableSelonType<TItem>();
        //        if (conn.GetType() == typeof(SQLiteAsyncConnection))
        //            return await ((SQLiteAsyncConnection)conn).ExecuteScalarAsync<long>(sqlQuery);
        //        else if (conn.GetType() == typeof(SQLiteConnection))
        //            return ((SQLiteConnection)conn).ExecuteScalar<long>(sqlQuery);

        //        return -1;
        //    }
        //    catch (Exception ex)
        //    {
        //        errors.Add(await Tools.Log.LogSQLException(sqlQuery, ex), Enums.ErrorType.Exception, ex);
        //        return -1;
        //    }
        //}
        //internal async static Task<long> SelectMaxKey<TItem>(object conn, Tools.ErrorsList errors) where TItem : Model.ModeleBase
        //{
        //    errors.Clear();

        //    try
        //    {
        //        return await SelectMax<TItem>(conn, Const.DB_COMMON_ID_COLNAME, errors);
        //    }
        //    catch (Exception ex)
        //    {
        //        errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
        //        return 0;
        //    }
        //}
        //internal async static Task<TItem> SelectItem<TItem, TKey>(object conn, TKey key, Tools.ErrorsList errors) where TItem : class
        //{
        //    errors.Clear();

        //    try
        //    {
        //        if (conn.GetType() == typeof(SQLiteAsyncConnection))
        //            return await ((SQLiteAsyncConnection)conn).FindAsync<TItem>(key);
        //        else if (conn.GetType() == typeof(SQLiteConnection))
        //            return ((SQLiteConnection)conn).Find<TItem>(key);

        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
        //        return null;
        //    }
        //}
        //internal async static Task<ObservableCollection<TItem>> SelectItems<TItem, TField>(object conn, string fieldName, TField value, Tools.ErrorsList errors) where TItem : Model.ModeleBase
        //{
        //    errors.Clear();

        //    string sqlQuery = "";
        //    List<TItem> results = null;

        //    try
        //    {
        //        sqlQuery = "SELECT * FROM " + Const.NomTableSelonType<TItem>();
        //        if (!string.IsNullOrEmpty(fieldName)) sqlQuery += " WHERE " + fieldName + "=" + Tools.Convert.FormatSQL(value);
        //    }
        //    catch (Exception ex)
        //    {
        //        errors.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
        //        return null;
        //    }

        //    try
        //    {
        //        if (conn.GetType() == typeof(SQLiteAsyncConnection))
        //            results = await ((SQLiteAsyncConnection)conn).QueryAsync<TItem>(sqlQuery);
        //        else if (conn.GetType() == typeof(SQLiteConnection))
        //            results = ((SQLiteConnection)conn).Query<TItem>(sqlQuery);

        //        if (results.Count == 0)
        //        {
        //            errors.Add("Aucune ligne n'a été trouvée !\nRequête : " + sqlQuery);
        //            return null;
        //        }

        //        ObservableCollection<TItem> col = new ObservableCollection<TItem>();
        //        foreach (TItem item in results) col.Add(item);

        //        return col;
        //    }
        //    catch (Exception ex)
        //    {
        //        errors.Add(await Tools.Log.LogSQLException(sqlQuery, ex), Enums.ErrorType.Exception, ex);
        //        return null;
        //    }
        //}

    }
}
