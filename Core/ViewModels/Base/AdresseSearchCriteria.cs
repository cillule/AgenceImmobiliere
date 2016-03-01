using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Oyosoft.AgenceImmobiliere.Core.DataAccess;

namespace Oyosoft.AgenceImmobiliere.Core.ViewModels
{
    [DataContract]
    public class AdresseSearchCriteria : AppartenanceSearchCriteria
    {
        #region Attributs

        protected string _adresseContient;
        protected string _codePostal;
        protected string _ville;
        protected double _latitude;
        protected double _longitude;
        protected double _altitude;

        #endregion

        #region Propriétés

        [DataMember]
        public string AdresseContient
        {
            get { return _adresseContient; }
            set { SetProperty(ref _adresseContient, value); }
        }

        [DataMember]
        public string CodePostal
        {
            get { return _codePostal; }
            set { SetProperty(ref _codePostal, value); }
        }

        [DataMember]
        public string Ville
        {
            get { return _ville; }
            set { SetProperty(ref _ville, value); }
        }

        [DataMember]
        public double Latitude
        {
            get { return _latitude; }
            set { SetProperty(ref _latitude, value); }
        }

        [DataMember]
        public double Longitude
        {
            get { return _longitude; }
            set { SetProperty(ref _longitude, value); }
        }

        [DataMember]
        public double Altitude
        {
            get { return _altitude; }
            set { SetProperty(ref _altitude, value); }
        }
        
        #endregion

        public AdresseSearchCriteria() : base()
        {
            this._adresseContient = "";
            this._codePostal = "";
            this._ville = "";
            this._latitude = -1;
            this._longitude = -1;
            this._altitude = -1;
        }

        protected override async Task<string> GenereWhere()
        {
            string where = await base.GenereWhere();
            
            where += GenereContains(Const.DB_COMMON_ADRESSE_COLNAME, this.AdresseContient);

            where += GenereEqual(Const.DB_COMMON_CODEPOSTAL_COLNAME, this.CodePostal, "", startsWith: true);

            where += GenereEqual(Const.DB_COMMON_VILLE_COLNAME, this.Ville, "");

            where += GenereEqual(Const.DB_COMMON_LATITUDE_COLNAME, this.Latitude, -1);

            where += GenereEqual(Const.DB_COMMON_LONGITUDE_COLNAME, this.Longitude, -1);

            where += GenereEqual(Const.DB_COMMON_ALTITUDE_COLNAME, this.Altitude, -1);

            return where;
        }
    }
}
