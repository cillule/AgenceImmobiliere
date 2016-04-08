using System.Runtime.Serialization;
using SQLite.Net.Attributes;
using Oyosoft.AgenceImmobiliere.Core.DataAccess;

namespace Oyosoft.AgenceImmobiliere.Core.Model
{
    [DataContract]
    public abstract class AdresseBase : AppartenanceBase
    {

        #region Attributs

        protected string _adresse;
        protected string _codePostal;
        protected string _ville;
        protected double _latitude;
        protected double _longitude;
        protected double _altitude;

        #endregion

        #region Propriétés

        [Column(Const.DB_COMMON_ADRESSE_COLNAME), DataMember]
        public string Adresse
        {
            get { return _adresse; }
            set { SetProperty(ref _adresse, value); }
        }

        [Column(Const.DB_COMMON_CODEPOSTAL_COLNAME), DataMember]
        public string CodePostal
        {
            get { return _codePostal; }
            set { SetProperty(ref _codePostal, value); }
        }

        [Column(Const.DB_COMMON_VILLE_COLNAME), DataMember]
        public string Ville
        {
            get { return _ville; }
            set { SetProperty(ref _ville, value); }
        }

        [Column(Const.DB_COMMON_LATITUDE_COLNAME), DataMember]
        public double Latitude
        {
            get { return _latitude; }
            set { SetProperty(ref _latitude, value); }
        }

        [Column(Const.DB_COMMON_LONGITUDE_COLNAME), DataMember]
        public double Longitude
        {
            get { return _longitude; }
            set { SetProperty(ref _longitude, value); }
        }

        [Column(Const.DB_COMMON_ALTITUDE_COLNAME), DataMember]
        public double Altitude
        {
            get { return _altitude; }
            set { SetProperty(ref _altitude, value); }
        }

        #endregion

        protected AdresseBase() : base()
        {
            this._adresse = "";
            this._codePostal = "";
            this._ville = "";
            this._latitude = 0;
            this._longitude = 0;
            this._altitude = 0;
        }

    }
}
