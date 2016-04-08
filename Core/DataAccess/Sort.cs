using System.Runtime.Serialization;
using Oyosoft.AgenceImmobiliere.Core.ViewModels;

namespace Oyosoft.AgenceImmobiliere.Core.DataAccess
{
    [DataContract]
    public class Sort : BaseNotifyPropertyChanged
    {
        protected Enums.ChampElement _field;
        protected Enums.OrdreTri _order;


        [DataMember]
        public Enums.ChampElement Field { get { return _field; } set { SetProperty(ref _field, value); } }
        [DataMember]
        public Enums.OrdreTri Order { get { return _order; } set { SetProperty(ref _order, value); } }

        public Sort(Enums.ChampElement field, Enums.OrdreTri order)
        {
            this.Field = field;
            this.Order = order;
        }
        public Sort() : this(Enums.ChampElement.Id, Enums.OrdreTri.Montant) { }
    }
}
