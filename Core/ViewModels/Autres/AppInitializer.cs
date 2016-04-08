using System;
using System.IO;
using System.ServiceModel;
using System.Threading.Tasks;
using Oyosoft.AgenceImmobiliere.Core.Service;
using Oyosoft.AgenceImmobiliere.Core.Tools;

namespace Oyosoft.AgenceImmobiliere.Core.ViewModels
{
    public class AppInitializer
    {
        protected DataAccess.Connection _localConnection;
        protected IContractAsync _distantService;
        protected bool _serviceAvailable;
        protected Parameters _parameters;
        protected ErrorsList _errors;
        protected ErrorsList _warnings;


        public DataAccess.Connection LocalConnection
        {
            get { return _localConnection; }
        }
        public IContractAsync DistantService
        {
            get { return _distantService; }
        }
        public bool DistantServiceAvailable
        {
            get { return _serviceAvailable; }
            protected set { _serviceAvailable = value; }
        }
        public Parameters Parameters
        {
            get { return _parameters; }
            protected set { _parameters = value; }
        }
        public ErrorsList Errors
        {
            get { return _errors; }
        }
        public ErrorsList Warnings
        {
            get { return _warnings; }
        }

        internal AppInitializer() : base()
        {
            this._localConnection = null;
            this._distantService = null;
            this._serviceAvailable = false;
            this._parameters = null;
            this._errors = new Tools.ErrorsList();
            this._warnings = new Tools.ErrorsList();
        }



        public async Task<bool> InitializeApplication(string db_default_directory_path, 
                                                      string db_directory_path, 
                                                      string db_file_name,
                                                      SQLite.Net.Interop.ISQLitePlatform sqlite_platform,
                                                      string endpoint_configuration_name,
                                                      string endpoint_configuration_address)
        {
            this._errors.Clear();
            this._warnings.Clear();

            // 1. Ouverture de la connexion locale
            if (!await OpenLocalConnection(db_default_directory_path, db_directory_path, db_file_name, sqlite_platform))
            {
                return false;
            }

            // 2. Ouverture de la connexion distante
            this.DistantServiceAvailable = await OpenDistantConnection(endpoint_configuration_name, endpoint_configuration_address);

            // 3. Chargement des paramètres locaux
            this.Parameters = new Parameters(_localConnection);
            if (!await this.Parameters.ChargerParametres())
            {
                this._errors.AddRange(this.Parameters.Erreurs);
                return false;
            }

            return true;
        }


        private async Task<bool> OpenLocalConnection(string db_default_directory_path, string db_directory_path, string db_file_name, SQLite.Net.Interop.ISQLitePlatform sqlitePlatform)
        {
            _localConnection = null;

            // Récupération du chemin à la base de données
            string databasePath = "";
            if (string.IsNullOrEmpty(db_file_name)) throw new Exception("Le nom du fichier de la base de données est obligatoire !");
            if (string.IsNullOrEmpty(db_directory_path))
            {
                if (string.IsNullOrEmpty(db_default_directory_path)) throw new Exception("Le chemin d'accès par défaut au répertoire contenant la base de données est obligatoire !");
                databasePath = Path.Combine(db_default_directory_path, db_file_name);
            }
            else
            {
                databasePath = Path.Combine(db_directory_path, db_file_name);
            }


            if (sqlitePlatform == null) throw new Exception("La plateforme SQLite est obligatoire !");

            try
            {
                // Initialisation de la connexion à la base de données
                _localConnection = await DataAccess.Connection.GetCurrent(sqlitePlatform, databasePath);

                // Vérification qu'il n'y a pas eu d'erreur à l'initialisation de la connexion
                if (!_localConnection.Errors.IsEmpty)
                {
                    this._errors.Add("Erreur(s) pendant l'ouverture de la connexion à la base de données locale :");
                    this._errors.AddRange(_localConnection.Errors);
                    return false;
                }

                // Initialisation de la base de données
                await _localConnection.InitializeDatabase(true);

                // Vérification qu'il n'y a pas eu d'erreurs à l'initialisation de la base de données
                if (!_localConnection.Errors.IsEmpty)
                {
                    this._errors.Add("Erreur(s) pendant l'initialisation de la base de données locale :");
                    this._errors.AddRange(_localConnection.Errors);
                    return false;
                }
            }
            catch (Exception ex)
            {
                await Log.LogException(ex);
                this._errors.Add("Exception pendant l'ouverture de la connexion à la base de données locale :");
                this._errors.AddRange(_localConnection.Errors);
                return false;
            }

            return true;
        }

        private async Task<bool> OpenDistantConnection(string endpoint_configuration_name, string endpoint_configuration_address)
        {
            _distantService = null;

            //if (string.IsNullOrEmpty(endpoint_configuration_name)) throw new Exception("Le nom du point de terminaison utilisé pour se connecter au service distant est obligatoire !");
            if (string.IsNullOrEmpty(endpoint_configuration_address)) throw new Exception("L'adresse utilisée pour se connecter au service distant est obligatoire !");

            try
            {
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.MaxBufferSize = 2147483647;
                binding.MaxReceivedMessageSize = 2147483647;
                ChannelFactory<IContractAsync> channel = new ChannelFactory<IContractAsync>(binding, new EndpointAddress(endpoint_configuration_address));
                IContractAsync contract = channel.CreateChannel(new EndpointAddress(endpoint_configuration_address));
                OperationResult result = await contract.TesterConnexionServiceAsync();
                if (result.Success)
                {
                    _distantService = contract;
                    return true;
                }
                else
                {
                    this._warnings.Add("Impossible de se connecter au service distant.\nL'application fonctionnera en mode 'Hors connexion'.");
                }
            }
            catch (Exception ex)
            {
                await Log.LogException(ex);
                this._warnings.Add("Impossible de se connecter au service distant :\n" + ex.Message + "\n\nL'application fonctionnera en mode 'Hors connexion'.");
            }

            return false;
        }


    }
}
