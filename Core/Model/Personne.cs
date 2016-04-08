using System;
using System.Runtime.Serialization;
using SQLite.Net.Attributes;
using Oyosoft.AgenceImmobiliere.Core.DataAccess;

namespace Oyosoft.AgenceImmobiliere.Core.Model
{

    [Table(Const.DB_PERSONNE_TABLENAME), DataContract]
    public class Personne : AdresseBase
    {

        #region Attributs

        protected Enums.Qualite _qualite;
        protected string _nom;
        protected string _prenom;
        protected string _mail;
        protected string _numeroTelephone;
        protected string _numeroPortable;
        protected DateTime? _dateNaissance;
        protected string _nomNaissance;
        protected string _villeNaissance;
        protected string _photoBase64;

        #endregion

        #region Propriétés

        [Column(Const.DB_PERSONNE_QUALITE_COLNAME), NotNull, DataMember]
        public Enums.Qualite Qualite
        {
            get { return _qualite; }
            set { SetProperty(ref _qualite, value); }
        }

        [Column(Const.DB_PERSONNE_NOM_COLNAME), NotNull, DataMember]
        public string Nom
        {
            get { return _nom; }
            set { SetProperty(ref _nom, value); }
        }

        [Column(Const.DB_PERSONNE_PRENOM_COLNAME), NotNull, DataMember]
        public string Prenom
        {
            get { return _prenom; }
            set { SetProperty(ref _prenom, value); }
        }

        [Column(Const.DB_PERSONNE_MAIL_COLNAME), DataMember]
        public string Mail
        {
            get { return _mail; }
            set { SetProperty(ref _mail, value); }
        }

        [Column(Const.DB_PERSONNE_TELEPHONE_COLNAME), DataMember]
        public string NumeroTelephone
        {
            get { return _numeroTelephone; }
            set { SetProperty(ref _numeroTelephone, value); }
        }

        [Column(Const.DB_PERSONNE_PORTABLE_COLNAME), DataMember]
        public string NumeroPortable
        {
            get { return _numeroPortable; }
            set { SetProperty(ref _numeroPortable, value); }
        }

        [Column(Const.DB_PERSONNE_DATENAISSANCE_COLNAME), DataMember]
        public DateTime? DateNaissance
        {
            get { return _dateNaissance; }
            set { SetProperty(ref _dateNaissance, value); }
        }

        [Column(Const.DB_PERSONNE_NOMNAISSANCE_COLNAME), DataMember]
        public string NomNaissance
        {
            get { return _nomNaissance; }
            set { SetProperty(ref _nomNaissance, value); }
        }

        [Column(Const.DB_PERSONNE_VILLENAISSANCE_COLNAME), DataMember]
        public string VilleNaissance
        {
            get { return _villeNaissance; }
            set { SetProperty(ref _villeNaissance, value); }
        }

        [Column(Const.DB_PERSONNE_PHOTOBASE64_COLNAME), DataMember]
        public string PhotoBase64
        {
            get { return _photoBase64; }
            set { SetProperty(ref _photoBase64, value); }
        }

        #endregion

        public Personne() : base()
        {
            this._qualite = Enums.Qualite.Monsieur;
            this._nom = "";
            this._prenom = "";
            this._mail = "";
            this._numeroTelephone = "";
            this._numeroPortable = "";
            this._dateNaissance = null;
            this._nomNaissance = "";
            this._villeNaissance = "";
            this._photoBase64 = "";
        }

    }
}
