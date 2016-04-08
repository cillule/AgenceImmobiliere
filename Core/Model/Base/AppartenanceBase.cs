using System;
using System.Runtime.Serialization;
using SQLite.Net.Attributes;
using Oyosoft.AgenceImmobiliere.Core.DataAccess;

namespace Oyosoft.AgenceImmobiliere.Core.Model
{
    [DataContract]
    public abstract class AppartenanceBase : ModeleBase
    {
        #region Attributs

        protected string _nomUtilisateurCreation;
        protected DateTime _dateHeureCreation;
        protected DateTime _dateHeureModification;

        #endregion

        #region Propriétés

        [Column(Const.DB_COMMON_NOMUTILISATEURCREATION_COLNAME), NotNull, DataMember]
        public string NomUtilisateurCreation
        {
            get { return _nomUtilisateurCreation; }
            set { SetProperty(ref _nomUtilisateurCreation, value); }
        }

        [Column(Const.DB_COMMON_DATEHEURECREATION_COLNAME), NotNull, DataMember]
        public DateTime DateHeureCreation
        {
            get { return _dateHeureCreation; }
            set { SetProperty(ref _dateHeureCreation, value); }
        }

        [Column(Const.DB_COMMON_DATEHEUREMODIFICATION_COLNAME), NotNull, DataMember]
        public DateTime DateHeureModification
        {
            get { return _dateHeureModification; }
            set { SetProperty(ref _dateHeureModification, value); }
        }

        #endregion

        protected AppartenanceBase() : base()
        {
            this._nomUtilisateurCreation = "";
            this._dateHeureCreation = DateTime.MinValue;
            this._dateHeureModification = DateTime.MinValue;
        }

        internal void AffecterCreation(string nomUtilisateurProprietaire)
        {
            this._nomUtilisateurCreation = nomUtilisateurProprietaire;
            this._dateHeureCreation = DateTime.Now;
            this._dateHeureModification = DateTime.Now;
        }

        internal void AffecterModification()
        {
            this._dateHeureModification = DateTime.Now;
        }
    }
}
