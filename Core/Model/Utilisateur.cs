using System;
using System.Runtime.Serialization;
using SQLite.Net.Attributes;
using Oyosoft.AgenceImmobiliere.Core.DataAccess;

namespace Oyosoft.AgenceImmobiliere.Core.Model
{
    [Table(Const.DB_UTILISATEUR_TABLENAME), DataContract]
    public class Utilisateur : ModeleBase
    {
        #region Attributs

        protected long _idPersonne;
        protected string _nomUtilisateur;
        protected string _motDePasseCrypte;

        #endregion

        #region Propriétés

        [Column(Const.DB_UTILISATEUR_IDPERSONNE_COLNAME), NotNull, DataMember]
        public long IdPersonne
        {
            get { return _idPersonne; }
            internal set { SetProperty(ref _idPersonne, value); }
        }

        [Column(Const.DB_UTILISATEUR_NOMUTILISATEUR_COLNAME), NotNull, Indexed, DataMember]
        public string NomUtilisateur
        {
            get { return _nomUtilisateur; }
            internal set { SetProperty(ref _nomUtilisateur, value); }
        }

        [Column(Const.DB_UTILISATEUR_MOTDEPASSE_COLNAME), NotNull, DataMember]
        public string MotDePasseCrypte
        {
            get { return _motDePasseCrypte; }
            internal set { SetProperty(ref _motDePasseCrypte, value); }
        }

        #endregion

        public Utilisateur() : base()
        {
            this._idPersonne = -1;
            this._nomUtilisateur = "";
            this._motDePasseCrypte = "";
        }

        internal Utilisateur(long idPersonne, string nomUtilisateur) : this()
        {
            this._idPersonne = idPersonne;
            this._nomUtilisateur = nomUtilisateur;
            this._motDePasseCrypte = "";
        }
    }
}
