using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Oyosoft.AgenceImmobiliere.Core.Commands;
using Oyosoft.AgenceImmobiliere.Core.DataAccess;

namespace Oyosoft.AgenceImmobiliere.Core.ViewModels.BienImmobilier
{
    public class SearchCriteria : AdresseSearchCriteria
    {
        #region Attributs

        protected string _titreContient;
        protected Enums.TypeTransaction? _typeTransaction;
        protected Enums.TypeBien? _typeBien;
        protected string _descriptionContient;
        protected decimal _prixProprietaire1;
        protected decimal _prixProprietaire2;
        protected decimal _montantHonorairesTransaction1;
        protected decimal _montantHonorairesTransaction2;
        protected decimal _montantHonorairesMensuels1;
        protected decimal _montantHonorairesMensuels2;
        protected decimal _montantCharges1;
        protected decimal _montantCharges2;

        protected double _surface1;
        protected double _surface2;
        protected int _nbPieces1;
        protected int _nbPieces2;
        protected int _numEtage1;
        protected int _numEtage2;
        protected int _nbEtages1;
        protected int _nbEtages2;
        protected Enums.TypeChauffage? _typeChauffage;
        protected Enums.EnergieChauffage? _energieChauffage;

        protected long _idProprietaire;
        protected long _idCommercial;
        protected DateTime? _dateMiseEnTransaction1;
        protected DateTime? _dateMiseEnTransaction2;
        protected bool? _transactionEffectuee;
        protected long _idAcquereur;
        protected DateTime? _dateTransaction1;
        protected DateTime? _dateTransaction2;

        private Command _viderFiltresCommand;
        private Command<Command> _appliquerCommand;
        private Command<Command> _annulerCommand;

        #endregion

        #region Propriétés

        [DataMember]
        public string TitreContient
        {
            get { return _titreContient; }
            set { SetProperty(ref _titreContient, value); }
        }

        [DataMember]
        public Enums.TypeTransaction? TypeTransaction
        {
            get { return _typeTransaction; }
            set { SetProperty(ref _typeTransaction, value); }
        }

        [DataMember]
        public Enums.TypeBien? TypeBien
        {
            get { return _typeBien; }
            set { SetProperty(ref _typeBien, value); }
        }

        [DataMember]
        public string DescriptionContient
        {
            get { return _descriptionContient; }
            set { SetProperty(ref _descriptionContient, value); }
        }

        [DataMember]
        public decimal PrixProprietaire1
        {
            get { return _prixProprietaire1; }
            set { SetProperty(ref _prixProprietaire1, value); }
        }
        [DataMember]
        public decimal PrixProprietaire2
        {
            get { return _prixProprietaire2; }
            set { SetProperty(ref _prixProprietaire2, value); }
        }

        [DataMember]
        public decimal MontantHonorairesTransaction1
        {
            get { return _montantHonorairesTransaction1; }
            set { SetProperty(ref _montantHonorairesTransaction1, value); }
        }
        [DataMember]
        public decimal MontantHonorairesTransaction2
        {
            get { return _montantHonorairesTransaction2; }
            set { SetProperty(ref _montantHonorairesTransaction2, value); }
        }

        [DataMember]
        public decimal MontantHonorairesMensuels1
        {
            get { return _montantHonorairesMensuels1; }
            set { SetProperty(ref _montantHonorairesMensuels1, value); }
        }
        [DataMember]
        public decimal MontantHonorairesMensuels2
        {
            get { return _montantHonorairesMensuels2; }
            set { SetProperty(ref _montantHonorairesMensuels2, value); }
        }

        [DataMember]
        public decimal MontantCharges1
        {
            get { return _montantCharges1; }
            set { SetProperty(ref _montantCharges1, value); }
        }
        [DataMember]
        public decimal MontantCharges2
        {
            get { return _montantCharges2; }
            set { SetProperty(ref _montantCharges2, value); }
        }


        [DataMember]
        public double Surface1
        {
            get { return _surface1; }
            set { SetProperty(ref _surface1, value); }
        }
        [DataMember]
        public double Surface2
        {
            get { return _surface2; }
            set { SetProperty(ref _surface2, value); }
        }

        [DataMember]
        public int NbPieces1
        {
            get { return _nbPieces1; }
            set { SetProperty(ref _nbPieces1, value); }
        }
        [DataMember]
        public int NbPieces2
        {
            get { return _nbPieces2; }
            set { SetProperty(ref _nbPieces2, value); }
        }

        [DataMember]
        public int NumEtage1
        {
            get { return _numEtage1; }
            set { SetProperty(ref _numEtage1, value); }
        }
        [DataMember]
        public int NumEtage2
        {
            get { return _numEtage2; }
            set { SetProperty(ref _numEtage2, value); }
        }

        [DataMember]
        public int NbEtages1
        {
            get { return _nbEtages1; }
            set { SetProperty(ref _nbEtages1, value); }
        }
        [DataMember]
        public int NbEtages2
        {
            get { return _nbEtages2; }
            set { SetProperty(ref _nbEtages2, value); }
        }

        [DataMember]
        public Enums.TypeChauffage? TypeChauffage
        {
            get { return _typeChauffage; }
            set { SetProperty(ref _typeChauffage, value); }
        }

        [DataMember]
        public Enums.EnergieChauffage? EnergieChauffage
        {
            get { return _energieChauffage; }
            set { SetProperty(ref _energieChauffage, value); }
        }


        [DataMember]
        public long IdProprietaire
        {
            get { return _idProprietaire; }
            set { SetProperty(ref _idProprietaire, value); }
        }

        [DataMember]
        public long IdCommercial
        {
            get { return _idCommercial; }
            set { SetProperty(ref _idCommercial, value); }
        }

        [DataMember]
        public DateTime? DateMiseEnTransaction1
        {
            get { return _dateMiseEnTransaction1; }
            set { SetProperty(ref _dateMiseEnTransaction1, value); }
        }
        [DataMember]
        public DateTime? DateMiseEnTransaction2
        {
            get { return _dateMiseEnTransaction2; }
            set { SetProperty(ref _dateMiseEnTransaction2, value); }
        }

        [DataMember]
        public bool? TransactionEffectuee
        {
            get { return _transactionEffectuee; }
            set { SetProperty(ref _transactionEffectuee, value); }
        }

        [DataMember]
        public long IdAcquereur
        {
            get { return _idAcquereur; }
            set { SetProperty(ref _idAcquereur, value); }
        }

        [DataMember]
        public DateTime? DateTransaction1
        {
            get { return _dateTransaction1; }
            set { SetProperty(ref _dateTransaction1, value); }
        }
        [DataMember]
        public DateTime? DateTransaction2
        {
            get { return _dateTransaction2; }
            set { SetProperty(ref _dateTransaction2, value); }
        }

        public override bool CriteresVides
        {
            get
            {
                return this._id == -1 &&
                       this._titreContient == "" &&
                       this._typeTransaction == null &&
                       this._typeBien == null &&
                       this._descriptionContient == "" &&
                       this._prixProprietaire1 == -1 &&
                       this._prixProprietaire2 == -1 &&
                       this._montantHonorairesTransaction1 == -1 &&
                       this._montantHonorairesTransaction2 == -1 &&
                       this._montantHonorairesMensuels1 == -1 &&
                       this._montantHonorairesMensuels2 == -1 &&
                       this._montantCharges1 == -1 &&
                       this._montantCharges2 == -1 &&
                       this._surface1 == -1 &&
                       this._surface2 == -1 &&
                       this._nbPieces1 == -1 &&
                       this._nbPieces2 == -1 &&
                       this._numEtage1 == -1 &&
                       this._numEtage2 == -1 &&
                       this._nbEtages1 == -1 &&
                       this._nbEtages2 == -1 &&
                       this._typeChauffage == null &&
                       this._energieChauffage == null &&
                       this._adresseContient == "" &&
                       this._codePostal == "" &&
                       this._ville == "" &&
                       this._latitude == -1 &&
                       this._longitude == -1 &&
                       this._altitude == -1 &&
                       this._idProprietaire == -1 &&
                       this._idCommercial == -1 &&
                       this._dateMiseEnTransaction1 == null &&
                       this._dateMiseEnTransaction2 == null &&
                       this._transactionEffectuee == null &&
                       this._idAcquereur == -1 &&
                       this._dateTransaction1 == null &&
                       this._dateTransaction2 == null;
            }
        }

        public override Array ListeChamps
        {
            get
            {
                return Const.ListeChamps<Model.BienImmobilier>();
            }
        }


        public Command ViderFiltresCommand
        {
            get
            {
                return _viderFiltresCommand ?? (_viderFiltresCommand = new Command(async () => this.ClearFilters()));
            }
        }
        public Command<Command> AppliquerCommand
        {
            get
            {
                return _appliquerCommand ?? (_appliquerCommand = new Command<Command>(async (cmd) => await ExecuteCommand(cmd, null)));
            }
        }
        public Command<Command> AnnulerCommand
        {
            get
            {
                return _annulerCommand ?? (_annulerCommand = new Command<Command>(async (cmd) => await ExecuteCommand(cmd, null)));
            }
        }

        #endregion

        public SearchCriteria() : base() { }
        public SearchCriteria(SearchCriteria source) : base(source) { }

        public override void ClearFilters()
        {
            base.ClearFilters();

            this._titreContient = "";
            this._typeTransaction = null;
            this._typeBien = null;
            this._descriptionContient = "";
            this._prixProprietaire1 = -1;
            this._prixProprietaire2 = -1;
            this._montantHonorairesTransaction1 = -1;
            this._montantHonorairesTransaction2 = -1;
            this._montantHonorairesMensuels1 = -1;
            this._montantHonorairesMensuels2 = -1;
            this._montantCharges1 = -1;
            this._montantCharges2 = -1;

            this._surface1 = -1;
            this._surface2 = -1;
            this._nbPieces1 = -1;
            this._nbPieces2 = -1;
            this._numEtage1 = -1;
            this._numEtage2 = -1;
            this._nbEtages1 = -1;
            this._nbEtages2 = -1;
            this._typeChauffage = null;
            this._energieChauffage = null;

            this._adresseContient = "";
            this._codePostal = "";
            this._ville = "";
            this._latitude = -1;
            this._longitude = -1;
            this._altitude = -1;

            this._idProprietaire = -1;
            this._idCommercial = -1;
            this._dateMiseEnTransaction1 = null;
            this._dateMiseEnTransaction2 = null;
            this._transactionEffectuee = null;
            this._idAcquereur = -1;
            this._dateTransaction1 = null;
            this._dateTransaction2 = null;
        }
        public override void CloneFilters(DataAccess.SearchCriteria source)
        {
            base.CloneFilters(source);
            this._adresseContient = ((SearchCriteria)source).AdresseContient;

            this._titreContient = ((SearchCriteria)source).TitreContient;
            this._typeTransaction = ((SearchCriteria)source).TypeTransaction;
            this._typeBien = ((SearchCriteria)source).TypeBien;
            this._descriptionContient = ((SearchCriteria)source).DescriptionContient;
            this._prixProprietaire1 = ((SearchCriteria)source).PrixProprietaire1;
            this._prixProprietaire2 = ((SearchCriteria)source).PrixProprietaire2;
            this._montantHonorairesTransaction1 = ((SearchCriteria)source).MontantHonorairesTransaction1;
            this._montantHonorairesTransaction2 = ((SearchCriteria)source).MontantHonorairesTransaction2;
            this._montantHonorairesMensuels1 = ((SearchCriteria)source).MontantHonorairesMensuels1;
            this._montantHonorairesMensuels2 = ((SearchCriteria)source).MontantHonorairesMensuels2;
            this._montantCharges1 = ((SearchCriteria)source).MontantCharges1;
            this._montantCharges2 = ((SearchCriteria)source).MontantCharges2;

            this._surface1 = ((SearchCriteria)source).Surface1;
            this._surface2 = ((SearchCriteria)source).Surface2;
            this._nbPieces1 = ((SearchCriteria)source).NbPieces1;
            this._nbPieces2 = ((SearchCriteria)source).NbPieces2;
            this._numEtage1 = ((SearchCriteria)source).NumEtage1;
            this._numEtage2 = ((SearchCriteria)source).NumEtage2;
            this._nbEtages1 = ((SearchCriteria)source).NbEtages1;
            this._nbEtages2 = ((SearchCriteria)source).NbEtages2;
            this._typeChauffage = ((SearchCriteria)source).TypeChauffage;
            this._energieChauffage = ((SearchCriteria)source).EnergieChauffage;

            this._idProprietaire = ((SearchCriteria)source).IdProprietaire;
            this._idCommercial = ((SearchCriteria)source).IdCommercial;
            this._dateMiseEnTransaction1 = ((SearchCriteria)source).DateMiseEnTransaction1;
            this._dateMiseEnTransaction2 = ((SearchCriteria)source).DateMiseEnTransaction2;
            this._transactionEffectuee = ((SearchCriteria)source).TransactionEffectuee;
            this._idAcquereur = ((SearchCriteria)source).IdAcquereur;
            this._dateTransaction1 = ((SearchCriteria)source).DateTransaction1;
            this._dateTransaction2 = ((SearchCriteria)source).DateTransaction2;
        }

        protected override async Task<string> GenereQuery(string tableName, Query limit, string where, string orderBy)
        {
            string query = "", qPhotoPrincipale = "";

            qPhotoPrincipale = string.Format("SELECT {0} FROM {1} WHERE {2}={3} AND {4}=1",
                                             Const.DB_PHOTO_BASE64_COLNAME,
                                             Const.DB_PHOTO_TABLENAME,
                                             Const.DB_COMMON_ID_COLNAME,
                                             Const.DB_PHOTO_IDBIEN_COLNAME,
                                             Const.DB_PHOTO_PRINCIPALE_COLNAME);

            query = string.Format("SELECT *, ({0}) AS {1} FROM {2} {3} {4} {5}",
                                  qPhotoPrincipale,
                                  Const.DB_PHOTO_BASE64_COLNAME,
                                  tableName,
                                  where,
                                  limit.SqlQuery,
                                  orderBy);

            return query;
        }

        protected override async Task<string> GenereWhere()
        {
            string where = await base.GenereWhere();

            where += GenereContains(Const.DB_BIEN_TITRE_COLNAME, this.TitreContient);

            where += GenereBetween(Const.DB_BIEN_DATEMISEENTRANSACTION_COLNAME,
                                   this.DateMiseEnTransaction1,
                                   this.DateMiseEnTransaction2,
                                   null, "Datetime");

            where += GenereBetween(Const.DB_BIEN_DATETRANSACTION_COLNAME,
                                   this.DateTransaction1,
                                   this.DateTransaction2,
                                   null, "Datetime");

            where += GenereContains(Const.DB_BIEN_DESCRIPTION_COLNAME, this.DescriptionContient);

            where += GenereEqual(Const.DB_BIEN_ENERGIECHAUFFAGE_COLNAME, (int?)this.EnergieChauffage, -1);

            where += GenereBetween(Const.DB_BIEN_MONTANTCHARGES_COLNAME,
                                   this.MontantCharges1,
                                   this.MontantCharges2,
                                   -1);

            where += GenereBetween(Const.DB_BIEN_NBETAGES_COLNAME,
                                   this.NbEtages1,
                                   this.NbEtages2,
                                   -1);

            where += GenereBetween(Const.DB_BIEN_NBPIECES_COLNAME,
                                   this.NbPieces1,
                                   this.NbPieces2,
                                   -1);

            where += GenereBetween(Const.DB_BIEN_NUMETAGE_COLNAME,
                                   this.NumEtage1,
                                   this.NumEtage2,
                                   -1);

            where += GenereBetween(Const.DB_BIEN_PRIXPROPRIETAIRE_COLNAME,
                                   this.PrixProprietaire1,
                                   this.PrixProprietaire2,
                                   -1);

            where += GenereBetween(Const.DB_BIEN_MONTANTHONORAIRESTRANSACTION_COLNAME,
                                   this.MontantHonorairesTransaction1,
                                   this.MontantHonorairesTransaction2,
                                   -1);

            where += GenereBetween(Const.DB_BIEN_MONTANTHONORAIRESMENSUELS_COLNAME,
                                   this.MontantHonorairesMensuels1,
                                   this.MontantHonorairesMensuels2,
                                   -1);

            where += GenereBetween(Const.DB_BIEN_SURFACE_COLNAME,
                                   this.Surface1,
                                   this.Surface2,
                                   -1);

            where += GenereEqual(Const.DB_BIEN_TRANSACTIONEFFECTUEE_COLNAME, this.TransactionEffectuee, null);

            where += GenereEqual(Const.DB_BIEN_TYPEBIEN_COLNAME, (int?)this.TypeBien, -1);

            where += GenereEqual(Const.DB_BIEN_TYPECHAUFFAGE_COLNAME, (int?)this.TypeChauffage, -1);

            where += GenereEqual(Const.DB_BIEN_TYPETRANSACTION_COLNAME, (int?)this.TypeTransaction, -1);

            where += GenereEqual(Const.DB_BIEN_IDACQUEREUR_COLNAME, this.IdAcquereur, -1);

            where += GenereEqual(Const.DB_BIEN_IDCOMMERCIAL_COLNAME, this.IdCommercial, -1);

            where += GenereEqual(Const.DB_BIEN_IDPROPRIETAIRE_COLNAME, this.IdProprietaire, -1);

            return where;
        }

        public async Task ExecuteCommand(Command cmd, object parameter)
        {
            if (cmd != null)
            {
                await cmd.ExecuteAsync(parameter);
            }
        }

    }
}
