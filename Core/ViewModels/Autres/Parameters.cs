using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oyosoft.AgenceImmobiliere.Core.ViewModels
{
    public class Parameters : BaseNotifyPropertyChanged
    {
        protected DataAccess.Connection _localConnection;
        protected Tools.ErrorsList _erreurs;
        protected DateTime? _dateHeureDerniereSynchro;
        protected DateTime? _dateHeureDerniereConnexion;


        public Tools.ErrorsList Erreurs
        {
            get { return _erreurs; }
        }
        public DateTime? DateHeureDerniereSynchro
        {
            get { return _dateHeureDerniereSynchro; }
            protected internal set { if (SetProperty(ref _dateHeureDerniereSynchro, value)) OnPropertyChanged("SynchronisationNecessaire"); }
        }
        public DateTime? DateHeureDerniereConnexion
        {
            get { return _dateHeureDerniereConnexion; }
            protected internal set { if(SetProperty(ref _dateHeureDerniereConnexion, value)) OnPropertyChanged("SynchronisationNecessaire"); }
        }
        public bool SynchronisationNecessaire
        {
            get { return _dateHeureDerniereSynchro < _dateHeureDerniereConnexion; }
        }


        public Parameters(DataAccess.Connection localConnection)
        {
            this._localConnection = localConnection;
            this._erreurs = new Tools.ErrorsList();
        }


        public async Task<bool> ChargerParametres()
        {
            _erreurs.Clear();

            DateHeureDerniereSynchro = await LireDate(Model.Parametre.CLE_DATE_HEURE_DERINERE_SYNCHRO, "Erreur de lecture de la date de dernière synchronisation :");
            DateHeureDerniereConnexion = await LireDate(Model.Parametre.CLE_DATE_HEURE_DERINERE_CONNEXION, "Erreur de lecture de la date de dernière connexion :");

            return this._erreurs.IsEmpty;
        }
        private async Task<DateTime?> LireDate(string cleParametre, string messageErreur)
        {
            Model.Parametre param;
            DateTime date;

            try
            {
                param = await this._localConnection.SelectItem<Model.Parametre, string>(cleParametre, false);
                _erreurs.AddRange(_localConnection.Errors);
                if (param == null) return null;
                if (string.IsNullOrEmpty(param.Valeur)) return null;
            }
            catch (Exception ex)
            {
                _erreurs.Add(messageErreur + " " + ex.Message, Enums.ErrorType.Exception, ex);
                return null;
            }


            if (DateTime.TryParse(param.Valeur, out date))
            {
                return date;
            }
            else
            {
                this._erreurs.Add(messageErreur + " conversion impossible.");
                return null;
            }
        }

        public async Task<bool> EnregistrerParametres()
        {
            _erreurs.Clear();

            await EnregistrerParametre(Model.Parametre.CLE_DATE_HEURE_DERINERE_SYNCHRO, this.DateHeureDerniereSynchro.ToString());
            await EnregistrerParametre(Model.Parametre.CLE_DATE_HEURE_DERINERE_CONNEXION, this.DateHeureDerniereConnexion.ToString());

            return this._erreurs.IsEmpty;
        }
        private async Task EnregistrerParametre(string cleParametre, string valeur)
        {
            Model.Parametre param;

            param = new Model.Parametre(cleParametre);
            param.Valeur = valeur;

            await this._localConnection.InsertOrReplace(param);
            _erreurs.AddRange(_localConnection.Errors);
        }


    }
}
