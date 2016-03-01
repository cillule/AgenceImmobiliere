using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Oyosoft.AgenceImmobiliere.Core.DataAccess;

namespace Oyosoft.AgenceImmobiliere.Core.ViewModels
{
    [DataContract]
    public class AppartenanceSearchCriteria : DataAccess.SearchCriteria
    {
        #region Attributs

        protected string _nomUtilisateurCreation;
        protected DateTime? _dateHeureCreation1;
        protected DateTime? _dateHeureCreation2;
        protected DateTime? _dateHeureModification1;
        protected DateTime? _dateHeureModification2;

        #endregion

        #region Propriétés

        [DataMember]
        public string NomUtilisateurCreation
        {
            get { return _nomUtilisateurCreation; }
            set { SetProperty(ref _nomUtilisateurCreation, value); }
        }

        [DataMember]
        public DateTime? DateHeureCreation1
        {
            get { return _dateHeureCreation1; }
            set { SetProperty(ref _dateHeureCreation1, value); }
        }
        [DataMember]
        public DateTime? DateHeureCreation2
        {
            get { return _dateHeureCreation2; }
            set { SetProperty(ref _dateHeureCreation2, value); }
        }

        [DataMember]
        public DateTime? DateHeureModification1
        {
            get { return _dateHeureModification1; }
            set { SetProperty(ref _dateHeureModification1, value); }
        }
        [DataMember]
        public DateTime? DateHeureModification2
        {
            get { return _dateHeureModification2; }
            set { SetProperty(ref _dateHeureModification2, value); }
        }

        #endregion

        public AppartenanceSearchCriteria() : base()
        {
            this._nomUtilisateurCreation = "";
            this._dateHeureCreation1 = null;
            this._dateHeureCreation2 = null;
            this._dateHeureModification1 = null;
            this._dateHeureModification2 = null;
        }

        protected override async Task<string> GenereWhere()
        {
            string where = await base.GenereWhere();

            where += GenereEqual(Const.DB_COMMON_NOMUTILISATEURCREATION_COLNAME, this.NomUtilisateurCreation, "");

            where += GenereBetween(Const.DB_COMMON_DATEHEURECREATION_COLNAME,
                                   this.DateHeureCreation1,
                                   this.DateHeureCreation2,
                                   null, "Datetime");

            where += GenereBetween(Const.DB_COMMON_DATEHEUREMODIFICATION_COLNAME,
                                   this.DateHeureModification1,
                                   this.DateHeureModification2,
                                   null, "Datetime");

            return where;
        }
    }
}
