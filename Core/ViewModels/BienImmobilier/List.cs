using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Oyosoft.AgenceImmobiliere.Core.Commands;
using Oyosoft.AgenceImmobiliere.Core.DataAccess;
using Oyosoft.AgenceImmobiliere.Core.Service;
using Oyosoft.AgenceImmobiliere.Core.Tools;


namespace Oyosoft.AgenceImmobiliere.Core.ViewModels.BienImmobilier
{
    public class List : BaseViewModel<Model.BienImmobilier, SearchCriteria>
    {

        public static long? itemsCountOnPage;

        protected bool _isInitialized;
        protected bool _isLoadingList;
        protected bool _isLoadingDetails;
        protected Enums.TypeTransaction? _transactionType;
        protected SearchResult<Model.BienImmobilier> _biens;
        protected Model.BienImmobilier _bienSelectionne;

        private EventBindingCommand<EventArgs> _initializeCommand;
        private Command<long?> _loadListCommand;
        private Command<Command> _modifySearchCriteriaCommand;
        private Command<Command> _addBienCommand;
        private Command<Command> _modifyBienCommand;
        private Command<Func<bool>> _deleteBienCommand;

        public bool InitialisationTerminee
        {
            get { return _isInitialized; }
            protected set { SetProperty(ref _isInitialized, value); }
        }
        public bool ChargementListeEnCours
        {
            get { return _isLoadingList; }
            protected set { SetProperty(ref _isLoadingList, value); }
        }
        public Enums.TypeTransaction? TypeTransaction
        {
            get { return (_transactionType == null) ? Enums.TypeTransaction.Vente : _transactionType; }
            protected set { if (SetProperty(ref _transactionType, value)) LoadList(null); }
        }
        public SearchCriteria CriteresRecherche
        {
            get { return _criteria; }
            set { if (SetProperty(ref _criteria, value)) { OnPropertyChanged("CriteresRecherchesVides"); LoadList(null); } }
        }
        public bool CriteresRecherchesVides
        {
            get
            {
                return this._criteria.CriteresVides;
            }
        }
        public SearchResult<Model.BienImmobilier> Biens
        {
            get { return _biens; }
            protected set
            {
                if (SetProperty(ref _biens, value))
                {
                    OnPropertyChanged("PageCourante");
                    OnPropertyChanged("PagePrecedente");
                    OnPropertyChanged("PageSuivante");
                    this.ModifyBienCommand.OnCanExecuteChanged();
                    this.DeleteBienCommand.OnCanExecuteChanged();
                }
            }
        }
        public long? PageCourante
        {
            get { return (_biens == null) ? null : _biens.CurrentPage; }
        }
        public long? PagePrecedente
        {
            get { return (_biens == null) ? null : (_biens.CurrentPage <= 1) ? null : _biens.CurrentPage - 1; }
        }
        public long? PageSuivante
        {
            get { return (_biens == null) ? null : (_biens.CurrentPage >= _biens.PagesCount) ? null : _biens.CurrentPage + 1; }
        }
        public Model.BienImmobilier BienSelectionne
        {
            get { return _bienSelectionne; }
            protected set
            {
                if (SetProperty(ref _bienSelectionne, value))
                {
                    OnPropertyChanged("BienEstSelectionne");
                    this.ModifyBienCommand.OnCanExecuteChanged();
                    this.DeleteBienCommand.OnCanExecuteChanged();
                }
            }
        }
        public bool BienEstSelectionne
        {
            get { return !_isLoadingDetails && _bienSelectionne != null; }
        }

        public EventBindingCommand<EventArgs> InitializeCommand
        {
            get
            {
                return _initializeCommand ?? (_initializeCommand = new EventBindingCommand<EventArgs>(async arg => { await LoadList(null); OnPropertyChanged("Biens"); }));
            }
        }
        public Command<long?> LoadListCommand
        {
            get
            {
                return _loadListCommand ?? (_loadListCommand = new Command<long?>(async (currentPage) => await LoadList(currentPage)));
            }
        }
        public Command<Command> ModifySearchCriteriaCommand
        {
            get
            {
                return _modifySearchCriteriaCommand ?? (_modifySearchCriteriaCommand = new Command<Command>(async (cmd) => await ExecuteCommand(cmd, this)));
            }
        }
        public Command<Command> AddBienCommand
        {
            get
            {
                return _addBienCommand ?? (_addBienCommand = new Command<Command>(async (cmd) => await ExecuteCommand(cmd, null)));
            }
        }
        public Command<Command> ModifyBienCommand
        {
            get
            {
                return _modifyBienCommand ?? (_modifyBienCommand = new Command<Command>(async (cmd) =>
                {
                    if (this.BienEstSelectionne) await ExecuteCommand(cmd, this.BienSelectionne);
                }, (cmd) => {
                    if (this.Biens == null ||
                        this.Biens.Items.Count == 0) return false;
                    return this.BienEstSelectionne;
                }));
            }
        }
        public Command<Func<bool>> DeleteBienCommand
        {
            get
            {
                return _deleteBienCommand ?? (_deleteBienCommand = new Command<Func<bool>>(async (cmd) =>
                {
                    if (this.BienEstSelectionne) await Delete(cmd);
                }, (cmd) => {
                    if (this.Biens == null ||
                        this.Biens.Items.Count == 0) return false;
                    return this.BienEstSelectionne;
                }));
            }
        }


        public List() : base()
        {
            _isLoadingDetails = false;
            _isLoadingDetails = false;
            _isLoadingList = false;
            _biens = null;
            _bienSelectionne = null;
        }


        public async Task LoadList(long? currentPage)
        {
            this.ChargementListeEnCours = true;
            this.Erreurs.Clear();
            this.BienSelectionne = null;
            if (_criteria == null) _criteria = new SearchCriteria();
            _criteria.TypeTransaction = _transactionType;
            this.Biens = await base.GetList(currentPage, itemsCountOnPage);
            this.ChargementListeEnCours = false;
        }

        public async Task ExecuteCommand(Command cmd, object parameter)
        {
            if (cmd != null)
            {
                await cmd.ExecuteAsync(parameter);
            }
        }

        public async Task Delete(Func<bool> validater)
        {
            if (!BienEstSelectionne) return;

            this.Erreurs.Clear();

            if (validater != null)
            {
                if (!validater()) return;
            }

            if (!await base.Delete(BienSelectionne.Id)) return;

            await LoadList(this.PageCourante);
        }
    }
}
