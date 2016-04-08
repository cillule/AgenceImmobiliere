using System.Runtime.Serialization;
using SQLite.Net.Attributes;
using Oyosoft.AgenceImmobiliere.Core.DataAccess;
using Oyosoft.AgenceImmobiliere.Core.ViewModels;

namespace Oyosoft.AgenceImmobiliere.Core.Model
{
    [Table(Const.DB_PARAMETRE_TABLENAME), DataContract]
    public class Parametre : BaseNotifyPropertyChanged
    {

        #region Constantes

        public const string CLE_DATE_HEURE_DERINERE_SYNCHRO = "DHSYNCHRO";
        public const string CLE_DATE_HEURE_DERINERE_CONNEXION = "DHCONNEXION";

        #endregion

        #region Attributs

        protected string _cle;      // Clé du paramètre
        protected string _valeur;   // Valeur du paramètre

        #endregion

        #region Propriétés

        [Column(Const.DB_PARAMETRE_CLE_COLNAME), PrimaryKey, DataMember]
        public string Cle
        {
            get { return _cle; }
            private set { SetProperty(ref _cle, value); }
        }

        [Column(Const.DB_PARAMETRE_VALEUR_COLNAME), DataMember]
        public string Valeur
        {
            get { return _valeur; }
            set { SetProperty(ref _valeur, value); }
        }

        #endregion

        public Parametre()
        {
            this._cle = "";
            this._valeur = "";
        }

        public Parametre(string cle) : this()
        {
            this._cle = cle;
        }

    }
}
