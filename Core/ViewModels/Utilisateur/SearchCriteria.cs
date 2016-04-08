using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oyosoft.AgenceImmobiliere.Core.ViewModels.Utilisateur
{
    public class SearchCriteria : DataAccess.SearchCriteria
    {
        public override bool CriteresVides
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override Array ListeChamps
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
