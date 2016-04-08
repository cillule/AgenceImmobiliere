using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Oyosoft.AgenceImmobiliere.Core.DataAccess
{
    [DataContract]
    public class SynchroList
    {
        private List<Model.BienImmobilier> _biensImmobiliers;
        private List<Model.PhotoBienImmobilier> _photosBienImmobilier;
        private List<Model.Personne> _personnes;
        private List<Model.Utilisateur> _utilisateurs;


        [DataMember]
        public List<Model.BienImmobilier> BiensImmobiliers { get { return this._biensImmobiliers; } internal set { this._biensImmobiliers = value; } }
        [DataMember]
        public List<Model.PhotoBienImmobilier> PhotosBienImmobilier { get { return this._photosBienImmobilier; } internal set { this._photosBienImmobilier = value; } }
        [DataMember]
        public List<Model.Personne> Personnes { get { return this._personnes; } internal set { this._personnes = value; } }
        [DataMember]
        public List<Model.Utilisateur> Utilisateurs { get { return this._utilisateurs; } internal set { this._utilisateurs = value; } }

        public SynchroList() : this(false) { }
        internal SynchroList(bool initLists)
        {
            if (!initLists) return;
            this._biensImmobiliers = new List<Model.BienImmobilier>();
            this._photosBienImmobilier = new List<Model.PhotoBienImmobilier>();
            this._personnes = new List<Model.Personne>();
            this._utilisateurs = new List<Model.Utilisateur>();
        }
    }
}
