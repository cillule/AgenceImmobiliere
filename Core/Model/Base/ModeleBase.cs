using System;
using System.Runtime.Serialization;
using SQLite.Net.Attributes;
using Oyosoft.AgenceImmobiliere.Core.DataAccess;
using Oyosoft.AgenceImmobiliere.Core.ViewModels;

namespace Oyosoft.AgenceImmobiliere.Core.Model
{
    [DataContract]
    public abstract class ModeleBase : BaseNotifyPropertyChanged
    {
        #region Attributs

        protected long _id;

        #endregion

        #region Propriétés

        [Column(Const.DB_COMMON_ID_COLNAME), NotNull, PrimaryKey, AutoIncrement, DataMember]
        public long Id
        {
            get { return _id; }
            private set { SetProperty(ref _id, value); }
        }

        #endregion

        protected ModeleBase()
        {
            this._id = -1;
        }

    }
}
