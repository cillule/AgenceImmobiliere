using System;
using System.Runtime.Serialization;
using SQLite.Net.Attributes;
using Oyosoft.AgenceImmobiliere.Core.DataAccess;

namespace Oyosoft.AgenceImmobiliere.Core.Model
{
    [Table(Const.DB_BIEN_TABLENAME), DataContract]
    public class BienImmobilier : AdresseBase
    {

        #region Attributs

        protected string _titre;
        protected Enums.TypeTransaction _typeTransaction;
        protected Enums.TypeBien _typeBien;
        protected string _description;
        protected decimal _prixProprietaire;
        protected decimal _montantHonorairesTransaction;
        protected decimal _montantHonorairesMensuels;
        protected decimal _montantCharges;
        protected double _surface;
        protected int _nbPieces;
        protected int _numEtage;
        protected int _nbEtages;
        protected Enums.TypeChauffage _typeChauffage;
        protected Enums.EnergieChauffage _energieChauffage;

        protected long _idProprietaire;
        protected long _idCommercial;
        protected DateTime? _dateMiseEnTransaction;
        protected bool _transactionEffectuee;
        protected long _idAcquereur;
        protected DateTime? _dateTransaction;

        protected string _photoPrincipaleBase64;

        #endregion

        #region Propriétés

        [Column(Const.DB_BIEN_TITRE_COLNAME), DataMember]
        public string Titre
        {
            get { return _titre; }
            set { SetProperty(ref _titre, value); }
        }

        [Column(Const.DB_BIEN_TYPETRANSACTION_COLNAME), NotNull, DataMember]
        public Enums.TypeTransaction TypeTransaction
        {
            get { return _typeTransaction; }
            set { if (SetProperty(ref _typeTransaction, value)) OnPropertyChanged("PrixTotal"); }
        }

        [Column(Const.DB_BIEN_TYPEBIEN_COLNAME), NotNull, DataMember]
        public Enums.TypeBien TypeBien
        {
            get { return _typeBien; }
            set { SetProperty(ref _typeBien, value); }
        }

        [Column(Const.DB_BIEN_DESCRIPTION_COLNAME), DataMember]
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        [Column(Const.DB_BIEN_PRIXPROPRIETAIRE_COLNAME), NotNull, DataMember]
        public decimal PrixProprietaire
        {
            get { return _prixProprietaire; }
            set { if (SetProperty(ref _prixProprietaire, value)) OnPropertyChanged("PrixTotal"); }
        }

        [Column(Const.DB_BIEN_MONTANTHONORAIRESTRANSACTION_COLNAME), NotNull, DataMember]
        public decimal MontantHonorairesTransaction
        {
            get { return _montantHonorairesTransaction; }
            set { if (SetProperty(ref _montantHonorairesTransaction, value)) OnPropertyChanged("PrixTotal"); }
        }

        [Column(Const.DB_BIEN_MONTANTHONORAIRESMENSUELS_COLNAME), NotNull, DataMember]
        public decimal MontantHonorairesMensuels
        {
            get { return _montantHonorairesMensuels; }
            set { if (SetProperty(ref _montantHonorairesMensuels, value)) OnPropertyChanged("PrixTotal"); }
        }

        public decimal PrixTotal
        {
            get { return (_typeTransaction == Enums.TypeTransaction.Location) ? _prixProprietaire + _montantHonorairesMensuels : _prixProprietaire + _montantHonorairesTransaction; }
        }

        [Column(Const.DB_BIEN_MONTANTCHARGES_COLNAME), NotNull, DataMember]
        public decimal MontantCharges
        {
            get { return _montantCharges; }
            set { SetProperty(ref _montantCharges, value); }
        }

        [Column(Const.DB_BIEN_SURFACE_COLNAME), NotNull, DataMember]
        public double Surface
        {
            get { return _surface; }
            set { SetProperty(ref _surface, value); }
        }

        [Column(Const.DB_BIEN_NBPIECES_COLNAME), NotNull, DataMember]
        public int NbPieces
        {
            get { return _nbPieces; }
            set { SetProperty(ref _nbPieces, value); }
        }

        [Column(Const.DB_BIEN_NUMETAGE_COLNAME), NotNull, DataMember]
        public int NumEtage
        {
            get { return _numEtage; }
            set { SetProperty(ref _numEtage, value); }
        }

        [Column(Const.DB_BIEN_NBETAGES_COLNAME), NotNull, DataMember]
        public int NbEtages
        {
            get { return _nbEtages; }
            set { SetProperty(ref _nbEtages, value); }
        }

        [Column(Const.DB_BIEN_TYPECHAUFFAGE_COLNAME), NotNull, DataMember]
        public Enums.TypeChauffage TypeChauffage
        {
            get { return _typeChauffage; }
            set { SetProperty(ref _typeChauffage, value); }
        }

        [Column(Const.DB_BIEN_ENERGIECHAUFFAGE_COLNAME), NotNull, DataMember]
        public Enums.EnergieChauffage EnergieChauffage
        {
            get { return _energieChauffage; }
            set { SetProperty(ref _energieChauffage, value); }
        }


        [Column(Const.DB_BIEN_IDPROPRIETAIRE_COLNAME), NotNull, DataMember]
        public long IdProprietaire
        {
            get { return _idProprietaire; }
            set { SetProperty(ref _idProprietaire, value); }
        }

        [Column(Const.DB_BIEN_IDCOMMERCIAL_COLNAME), NotNull, DataMember]
        public long IdCommercial
        {
            get { return _idCommercial; }
            set { SetProperty(ref _idCommercial, value); }
        }

        [Column(Const.DB_BIEN_DATEMISEENTRANSACTION_COLNAME), DataMember]
        public DateTime? DateMiseEnTransaction
        {
            get { return _dateMiseEnTransaction; }
            set { SetProperty(ref _dateMiseEnTransaction, value); }
        }

        [Column(Const.DB_BIEN_TRANSACTIONEFFECTUEE_COLNAME), NotNull, DataMember]
        public bool TransactionEffectuee
        {
            get { return _transactionEffectuee; }
            set { SetProperty(ref _transactionEffectuee, value); }
        }

        [Column(Const.DB_BIEN_IDACQUEREUR_COLNAME), DataMember]
        public long IdAcquereur
        {
            get { return _idAcquereur; }
            set { SetProperty(ref _idAcquereur, value); }
        }

        [Column(Const.DB_BIEN_DATETRANSACTION_COLNAME), DataMember]
        public DateTime? DateTransaction
        {
            get { return _dateTransaction; }
            set { SetProperty(ref _dateTransaction, value); }
        }

        [DataMember]
        public string PhotoPrincipaleBase64
        {
            get { return _photoPrincipaleBase64; }
            internal set { SetProperty(ref _photoPrincipaleBase64, value); }
        }

        #endregion

        public BienImmobilier() : base()
        {
            this._titre = "";
            this._typeTransaction = Enums.TypeTransaction.Vente;
            this._typeBien = Enums.TypeBien.Appartement;
            this._description = "";
            this._prixProprietaire = 0;
            this._montantHonorairesTransaction = 0;
            this._montantHonorairesMensuels = 0;
            this._montantCharges = 0;
            this._surface = 0;
            this._nbPieces = 0;
            this._numEtage = 0;
            this._nbEtages = 0;
            this._typeChauffage = Enums.TypeChauffage.Individuel;
            this._energieChauffage = Enums.EnergieChauffage.Fioul;
            this._idProprietaire = -1;
            this._idCommercial = -1;
            this._dateMiseEnTransaction = null;
            this._transactionEffectuee = false;
            this._idAcquereur = -1;
            this._dateTransaction = null;
        }
    }
}