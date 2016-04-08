using System;
using System.Runtime.Serialization;
using System.Reflection;
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

        [DataMember]
        public Array ListeChamps
        {
            get
            {
                MethodInfo method = typeof(Const).GetRuntimeMethod("ListeChamps", new Type[] { });
                MethodInfo genericMethod = method.MakeGenericMethod(this.GetType());
                return (Array)genericMethod.Invoke(this, null);
            }
            private set { }
        }

        #endregion

        protected ModeleBase()
        {
            this._id = -1;
        }

    }
}
