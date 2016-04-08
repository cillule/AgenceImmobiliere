using System;
using System.Runtime.Serialization;
using SQLite.Net.Attributes;
using Oyosoft.AgenceImmobiliere.Core.DataAccess;

namespace Oyosoft.AgenceImmobiliere.Core.Model
{
    [Table(Const.DB_PHOTO_TABLENAME), DataContract]
    public class PhotoBienImmobilier : ModeleBase
    {
        #region Attributs

        protected long _idBien;
        protected bool _principale;
        protected string _base64;

        #endregion

        #region Propriétés

        [Column(Const.DB_PHOTO_IDBIEN_COLNAME), NotNull, DataMember]
        public long IdBien
        {
            get { return _idBien; }
            set { SetProperty(ref _idBien, value); }
        }

        [Column(Const.DB_PHOTO_PRINCIPALE_COLNAME), NotNull, DataMember]
        public bool Principale
        {
            get { return _principale; }
            internal set { SetProperty(ref _principale, value); }
        }

        [Column(Const.DB_PHOTO_BASE64_COLNAME), NotNull, DataMember]
        public string Base64
        {
            get { return _base64; }
            set { SetProperty(ref _base64, value); }
        }

        #endregion

        public PhotoBienImmobilier() : this(-1) { }
        internal PhotoBienImmobilier(long idBien) : base()
        {
            this._idBien = idBien;
            this._principale = false;
            this._base64 = "";
        }
    }
}
