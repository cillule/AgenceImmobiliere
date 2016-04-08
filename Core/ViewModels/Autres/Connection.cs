using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Oyosoft.AgenceImmobiliere.Core.Commands;
using Oyosoft.AgenceImmobiliere.Core.Service;
using Oyosoft.AgenceImmobiliere.Core.Tools;


namespace Oyosoft.AgenceImmobiliere.Core.ViewModels
{
    public class Connection : BaseNotifyPropertyChanged
    {
        public static string _dbDefaultDirectoryPath;
        public static string _dbDirectoryPath;
        public static string _dbFileName;
        public static SQLite.Net.Interop.ISQLitePlatform _sqlitePlatform;
        public static string _endpointConfigurationName;
        public static string _endpointConfigurationAddress;

        private static IContractAsync _distantService;
        public static IContractAsync DistantService { get { return _distantService; } private set { _distantService = value; } }

        private static bool _serviceAvailable;
        public static bool ServiceAvailable { get { return _serviceAvailable; } private set { _serviceAvailable = value; } }

        private static Parameters _parameters;
        public static Parameters Parameters { get { return _parameters; } private set { _parameters = value; } }


        protected bool _working;
        protected bool _initialized;
        protected DataAccess.Connection _localConnection;
        protected ObservableCollection<Model.Utilisateur> _usersList;
        protected Model.Utilisateur _user;
        protected string _password;
        protected ErrorsList _errors;
        protected ErrorsList _warnings;

        private EventBindingCommand<EventArgs> _initializeCommand;
        private Command<Command> _connectUserCommand;
        private Command<Command> _disconnectUserCommand;


        public bool TraitementEnCours
        {
            get { return _working; }
            set
            {
                if (SetProperty(ref _working, value))
                {
                    OnPropertyChanged("ConnexionVisible");
                    OnPropertyChanged("DeconnexionVisible");
                }
            }
        }
        public bool InitialisationTerminee
        {
            get { return _initialized; }
            protected set
            {
                if (SetProperty(ref _initialized, value))
                {
                    OnPropertyChanged("ConnexionVisible");
                    OnPropertyChanged("DeconnexionVisible");
                }
            }
        }
        public DataAccess.Connection ConnexionLocale
        {
            get { return _localConnection; }
        }
        public IContractAsync ServiceDistant
        {
            get { return _distantService; }
        }
        public bool ServiceDistantDisponible
        {
            get { return _serviceAvailable; }
            protected set { SetProperty(ref _serviceAvailable, value); }
        }
        public Parameters Parametres
        {
            get { return _parameters; }
            protected set { SetProperty(ref _parameters, value); }
        }
        public ObservableCollection<Model.Utilisateur> Utilisateurs
        {
            get { return _usersList; }
            protected set { SetProperty(ref _usersList, value); }
        }
        public Model.Utilisateur Utilisateur
        {
            get
            {
                if (UtilisateurEstConnecte && (_user == null || _user.NomUtilisateur.ToUpper() != _localConnection.ConnectedUserName.ToUpper())) _user = _localConnection.ConnectedUser;
                return _user;
            }
            set { SetProperty(ref _user, value); }
        }
        public string MotDePasse
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }
        public bool UtilisateurEstConnecte
        {
            get { return _localConnection != null && _localConnection.UserIsConnected; }
        }
        public bool ConnexionVisible
        {
            get { return _initialized && !_working && !UtilisateurEstConnecte; }
        }
        public bool DeconnexionVisible
        {
            get { return _initialized && !_working && UtilisateurEstConnecte; }
        }
        public ErrorsList Erreurs
        {
            get { return _errors; }
        }
        public ErrorsList Avertissements
        {
            get { return _warnings; }
        }
        public EventBindingCommand<EventArgs> InitializeCommand
        {
            get
            {
                return _initializeCommand ?? (_initializeCommand = new EventBindingCommand<EventArgs>(async arg => await Initialize(arg) ));
            }
        }
        public Command<Command> ConnectUserCommand
        {
            get
            {
                return _connectUserCommand ?? (_connectUserCommand = new Command<Command>(async (cmd) => await ConnectUser(cmd)));
            }
        }
        public Command<Command> DisconnectUserCommand
        {
            get
            {
                return _disconnectUserCommand ?? (_disconnectUserCommand = new Command<Command>(async (cmd) => await DisconnectUser(cmd)));
            }
        }



        public Connection() : base()
        {
            _distantService = null;
            _serviceAvailable = false;
            _parameters = null;

            this._working = false;
            this._initialized = false;
            this._localConnection = null;
            this._usersList = null;
            this._user = null;
            this._password = "";
            this._errors = new Tools.ErrorsList();
            this._warnings = new Tools.ErrorsList();
        }



        public async Task Initialize(EventBindingArgs<EventArgs> info)
        {
            this.TraitementEnCours = true;
            this.InitialisationTerminee = false;
            this.Erreurs.Clear();
            this.Avertissements.Clear();

            // Initialisation de l'application
            AppInitializer appInitializer = new AppInitializer();
            bool appInitialized = await appInitializer.InitializeApplication(_dbDefaultDirectoryPath, _dbDirectoryPath, _dbFileName, _sqlitePlatform, _endpointConfigurationName, _endpointConfigurationAddress);
            this.Avertissements.AddRange(appInitializer.Warnings, "Avertissements", this);
            this.Erreurs.AddRange(appInitializer.Errors, "Erreurs", this);
            this.Parametres = appInitializer.Parameters;
            this.ServiceDistantDisponible = appInitializer.DistantServiceAvailable;
            _distantService = appInitializer.DistantService;
            this._localConnection = appInitializer.LocalConnection;
            if (!appInitialized)
            {
                this.InitialisationTerminee = true;
                this.TraitementEnCours = false;
                return;
            }

            // Chargement des utilisateurs
            DataAccess.SearchResult<Model.Utilisateur> result = await BaseViewModel<Model.Utilisateur, ViewModels.Utilisateur.SearchCriteria>.GetList(_localConnection, new ViewModels.Utilisateur.SearchCriteria(), this.Erreurs, null, null, false);
            this.Utilisateurs = result.Items;
            if (this.Utilisateurs.Count <= 0)
            {
                this.Erreurs.Add("Il n'existe aucun utilisateur dans la base de données !");
            }
            this.Utilisateur = _localConnection.ConnectedUser;

            this.InitialisationTerminee = true;
            this.TraitementEnCours = false;
        }

        public async Task ConnectUser(Command redirectCommand)
        {
            if (UtilisateurEstConnecte && (_user == null || _user.NomUtilisateur.ToUpper() != _localConnection.ConnectedUserName.ToUpper())) return;
            if (UtilisateurEstConnecte)
            {
                await DisconnectUser(null);
                if (!this.Erreurs.IsEmpty) return;
            }

            this.TraitementEnCours = true;
            this.Erreurs.Clear();
            string password = Tools.Crypto.Encrypt(_password);
            MotDePasse = "";

            if (this.Utilisateur == null)
            {
                this.Erreurs.Add("Vous devez sélectionner un utilisteur !");
                this.TraitementEnCours = false;
                return;
            }

            // Connexion à la base locale
            await this._localConnection.ConnectUser(this.Utilisateur.NomUtilisateur, password);
            this.Erreurs.AddRange(this._localConnection.Errors, "Erreurs", this);
            if (!this.Erreurs.IsEmpty)
            {
                this.TraitementEnCours = false;
                return;
            }

            // Connexion à la base distante
            if (_serviceAvailable)
            {
                try
                {
                    OperationResult result;
                    result = await _distantService.ConnecterUtilisateurAsync(this.Utilisateur.NomUtilisateur, password);
                    if (!result.Success && !result.Errors.IsEmpty)
                    {
                        this.Erreurs.AddRange(result.Errors, "Erreurs", this);
                        this.TraitementEnCours = false;
                        return;
                    }
                }
                catch (Exception ex)
                {
                    this.Erreurs.Add(await Tools.Log.LogException(ex), Enums.ErrorType.Exception, ex);
                    this.TraitementEnCours = false;
                    return;
                }
            }

            // Synchro si besoin
            DateTime lastConnection = DateTime.Now;
            if (_serviceAvailable && this.Parametres.DateHeureDerniereSynchro != this.Parametres.DateHeureDerniereConnexion)
            {
                ErrorsList erreurs = new ErrorsList();
                await BaseViewModel<Model.Utilisateur, ViewModels.Utilisateur.SearchCriteria>.SynchronizeDistantToLocal(this._localConnection, _distantService, _serviceAvailable, this.Parametres, erreurs);
                this.Erreurs.AddRange(erreurs, "Erreurs", this);
                if (!this.Erreurs.IsEmpty)
                {
                    this.TraitementEnCours = false;
                    return;
                }
                lastConnection = (DateTime)this.Parametres.DateHeureDerniereSynchro;
            }

            // Mise à jour de la date de connexion
            this.Parametres.DateHeureDerniereConnexion = lastConnection;
            await this.Parametres.EnregistrerParametres();
            this.Erreurs.AddRange(this.Parametres.Erreurs, "Erreurs", this);

            OnPropertyChanged("UtilisateurEstConnecte");
            OnPropertyChanged("ConnexionVisible");
            OnPropertyChanged("DeconnexionVisible");

            this.TraitementEnCours = false;

            if (this.Erreurs.IsEmpty && redirectCommand != null)
            {
                await redirectCommand.ExecuteAsync(null);
            }
        }

        public async Task DisconnectUser(Command redirectCommand)
        {
            if (!UtilisateurEstConnecte) return;

            this.TraitementEnCours = true;
            this.Erreurs.Clear();

            // Déconnexion de la base locale
            this._localConnection.DisconnectUser(this.Utilisateur.NomUtilisateur);
            this.Erreurs.AddRange(this._localConnection.Errors, "Erreurs", this);
            if (!this.Erreurs.IsEmpty)
            {
                this.TraitementEnCours = false;
                return;
            }

            // Déconnexion de la base distante
            if (_serviceAvailable)
            {
                try
                {
                    OperationResult result;
                    result = await _distantService.DeconnecterUtilisateurAsync(this.Utilisateur.NomUtilisateur);
                    if (!result.Success && !result.Errors.IsEmpty)
                    {
                        this.Erreurs.AddRange(result.Errors, "Erreurs", this);
                        this.TraitementEnCours = false;
                        return;
                    }
                }
                catch (Exception ex)
                {
                    this.Erreurs.Add(Tools.Log.LogException(ex).ExecuteSynchronously(), Enums.ErrorType.Exception, ex);
                    this.TraitementEnCours = false;
                    return;
                }
            }

            OnPropertyChanged("UtilisateurEstConnecte");
            this.TraitementEnCours = false;

            if (this.Erreurs.IsEmpty && redirectCommand != null)
            {
                await redirectCommand.ExecuteAsync(null);
            }
        }

    }
}
