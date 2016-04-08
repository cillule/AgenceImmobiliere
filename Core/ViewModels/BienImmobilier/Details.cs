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
    public class Details : BaseViewModel<Model.BienImmobilier, SearchCriteria>
    {
        protected bool _isInitialized;
        protected bool _isLoadingDetails;
        protected Model.BienImmobilier _bien;
        protected ObservableCollection<Model.PhotoBienImmobilier> _photosBien;
        protected Model.PhotoBienImmobilier _photoSelectionnee;

        private EventBindingCommand<EventArgs> _initializeCommand;
        private Command<Func<string>> _addPhotoCommand;
        private Command _deletePhotoCommand;
        private Command _selectPreviousPhotoCommand;
        private Command _selectNextPhotoCommand;
        private Command<Action> _saveBienCommand;

        public bool InitialisationTerminee
        {
            get { return _isInitialized; }
            protected set { SetProperty(ref _isInitialized, value); }
        }
        public bool ChargementDetailsEnCours
        {
            get { return _isLoadingDetails; }
            protected set
            {
                if (SetProperty(ref _isLoadingDetails, value))
                {
                    this.SaveBienCommand.OnCanExecuteChanged();
                }
            }
        }
        public Model.BienImmobilier Bien
        {
            get { return _bien; }
            protected set
            {
                if (SetProperty(ref _bien, value))
                {
                    this.SaveBienCommand.OnCanExecuteChanged();
                }
            }
        }
        public ObservableCollection<Model.PhotoBienImmobilier> PhotosBien
        {
            get { return _photosBien; }
            protected set
            {
                if (SetProperty(ref _photosBien, value))
                {
                    this.DeletePhotoCommand.OnCanExecuteChanged();
                    this.SelectPreviousPhotoCommand.OnCanExecuteChanged();
                    this.SelectNextPhotoCommand.OnCanExecuteChanged();
                }
            }
        }
        public Model.PhotoBienImmobilier PhotoSelectionnee
        {
            get { return _photoSelectionnee; }
            protected set
            {
                if (SetProperty(ref _photoSelectionnee, value))
                {
                    this.DeletePhotoCommand.OnCanExecuteChanged();
                    this.SelectPreviousPhotoCommand.OnCanExecuteChanged();
                    this.SelectNextPhotoCommand.OnCanExecuteChanged();
                }
            }
        }

        public EventBindingCommand<EventArgs> InitializeCommand
        {
            get
            {
                return _initializeCommand ?? (_initializeCommand = new EventBindingCommand<EventArgs>(async arg => { await LoadDetails(); OnPropertyChanged("Bien"); }));
            }
        }
        public Command<Func<string>> AddPhotoCommand
        {
            get
            {
                return _addPhotoCommand ?? (_addPhotoCommand = new Command<Func<string>>(async (selectPhoto) => AddPhoto(selectPhoto)));
            }
        }
        public Command DeletePhotoCommand
        {
            get
            {
                return _deletePhotoCommand ?? (_deletePhotoCommand = new Command(async () =>
                {
                   await DeletePhoto();
                }, () => {
                    if (this.PhotosBien == null ||
                        this.PhotosBien.Count == 0) return false;
                    return this.PhotoSelectionnee != null;
                }));
            }
        }
        public Command SelectPreviousPhotoCommand
        {
            get
            {
                return _selectPreviousPhotoCommand ?? (_selectPreviousPhotoCommand = new Command(async () =>
                {
                    if (this.PhotosBien.Count <= 0) return;
                    if (this.PhotoSelectionnee == null)
                    {
                        this.PhotoSelectionnee = this.PhotosBien[0];
                    }
                    else
                    {
                        int index = this.PhotosBien.IndexOf(this.PhotoSelectionnee);
                        if (index <= 0) return;
                        this.PhotoSelectionnee = this.PhotosBien[index - 1];
                    }
                }, () =>
                {
                    if (this.PhotosBien == null) return false;
                    if (this.PhotosBien.Count <= 0) return false;
                    if (this.PhotosBien == null) return true;
                    int index = this.PhotosBien.IndexOf(this.PhotoSelectionnee);
                    if (index <= 0) return false;
                    return true;
                }));
            }
        }
        public Command SelectNextPhotoCommand
        {
            get
            {
                return _selectNextPhotoCommand ?? (_selectNextPhotoCommand = new Command(async () =>
                {
                    if (this.PhotosBien.Count <= 0) return;
                    if (this.PhotoSelectionnee == null)
                    {
                        this.PhotoSelectionnee = this.PhotosBien[0];
                    }
                    else
                    {
                        int index = this.PhotosBien.IndexOf(this.PhotoSelectionnee);
                        if (index >= this.PhotosBien.Count - 1) return;
                        this.PhotoSelectionnee = this.PhotosBien[index + 1];
                    }
                }, () =>
                {
                    if (this.PhotosBien == null) return false;
                    if (this.PhotosBien.Count <= 0) return false;
                    if (this.PhotoSelectionnee == null) return true;
                    int index = this.PhotosBien.IndexOf(this.PhotoSelectionnee);
                    if (index >= this.PhotosBien.Count - 1) return false;
                    return true;
                }));
            }
        }
        public Command<Action> SaveBienCommand
        {
            get
            {
                return _saveBienCommand ?? (_saveBienCommand = new Command<Action>(async (act) =>
                {
                    await SaveBien(act);
                }));
            }
        }


        public Details(Model.BienImmobilier bien = null) : base()
        {
            _bien = bien;
            if (_bien == null) _bien = new Model.BienImmobilier();
        }


        public async Task LoadDetails()
        {
            this.ChargementDetailsEnCours = true;
            this.Erreurs.Clear();
            if (this.Bien.Id == -1)
            {
                Details<ObservableCollection<Model.PhotoBienImmobilier>> details = await base.GetDetails<ObservableCollection<Model.PhotoBienImmobilier>>(this.Bien.Id);
                this.PhotosBien = details.Result2;
            }
            else
            {
                this.PhotosBien = new ObservableCollection<Model.PhotoBienImmobilier>();
            }
            this.ChargementDetailsEnCours = false;
        }

        public async Task AddPhoto(Func<string> selectPhoto)
        {
            if (selectPhoto != null)
            {
                string photoBase64 = selectPhoto();
                if (!string.IsNullOrEmpty(photoBase64)) {
                    Model.PhotoBienImmobilier photo = new Model.PhotoBienImmobilier();
                    photo.Base64 = photoBase64;
                    this.PhotosBien.Add(photo);
                }
            }
        }

        public async Task DeletePhoto()
        {
            if (_photoSelectionnee == null) return;
            this.PhotosBien.Remove(this.PhotoSelectionnee);
        }

        public async Task SaveBien(Action action)
        {
            for (int i = 0; i < PhotosBien.Count; i++)
            {
                PhotosBien[i].Principale = (i == 0);
            }

            if (await base.Update<Model.PhotoBienImmobilier>(Bien, PhotosBien))
            {
                if (action != null) action();
            }
        }
    }
}
