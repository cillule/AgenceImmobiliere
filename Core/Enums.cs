using System.Runtime.Serialization;
using Oyosoft.AgenceImmobiliere.Core.DataAccess;

namespace Oyosoft.AgenceImmobiliere.Core
{
    [DataContract]
    public class Enums
    {
        #region Commun

        [DataContract]
        public enum TypeElement
        {
            [EnumMember]
            BienImmobilier = 0,
            [EnumMember]
            Personne = 1,
            [EnumMember]
            Utilisateur = 2
        }

        [DataContract]
        public enum ChampElement
        {
            [EnumMember]
            Id = 0,
            [EnumMember]
            Titre = 1,
            [EnumMember]
            TypeTransaction = 2,
            [EnumMember]
            TypeBien = 3,
            [EnumMember]
            Description = 4,
            [EnumMember]
            PrixProprietaire = 5,
            [EnumMember]
            MontantHonorairesTransaction = 6,
            [EnumMember]
            MontantHonorairesMensuels = 7,
            [EnumMember]
            MontantCharges = 8,
            [EnumMember]
            Surface = 9,
            [EnumMember]
            NbPieces = 10,
            [EnumMember]
            NumEtage = 11,
            [EnumMember]
            NbEtages = 12,
            [EnumMember]
            TypeChauffage = 13,
            [EnumMember]
            EnergieChauffage = 14,
            [EnumMember]
            Adresse = 15,
            [EnumMember]
            CodePostal = 16,
            [EnumMember]
            Ville = 17,
            [EnumMember]
            Latitude = 18,
            [EnumMember]
            Longitude = 19,
            [EnumMember]
            Altitude = 20,
            [EnumMember]
            IdProprietaire = 21,
            [EnumMember]
            IdCommercial = 22,
            [EnumMember]
            DateMiseEnTransaction = 23,
            [EnumMember]
            TransactionEffectuee = 24,
            [EnumMember]
            IdAcquereur = 25,
            [EnumMember]
            DateTransaction = 26,
            [EnumMember]
            Qualite = 27,
            [EnumMember]
            Nom = 28,
            [EnumMember]
            Prenom = 29,
            [EnumMember]
            Mail = 30,
            [EnumMember]
            NumeroTelephone = 31,
            [EnumMember]
            NumeroPortable = 32,
            [EnumMember]
            DateNaissance = 33,
            [EnumMember]
            NomNaissance = 34,
            [EnumMember]
            VilleNaissance = 35,
            [EnumMember]
            IdPersonne = 36,
            [EnumMember]
            NomUtilisateur = 37,
            [EnumMember]
            MotDePasseCrypte = 38,
            [EnumMember]
            NomUtilisateurCreation = 39,
            [EnumMember]
            DateHeureCreation = 40,
            [EnumMember]
            DateHeureModification = 41
        }

        [DataContract]
        public enum OrdreTri
        {
            [EnumMember]
            Montant = 0,
            [EnumMember]
            Descendant = 1
        }

        public static string NomSelonChamp(ChampElement champ)
        {
            return Const.NomSelonChamp(champ);
        }

        #endregion

        #region Bien immobilier

        [DataContract]
        public enum TypeTransaction
        {
            [EnumMember]
            Vente = 0,
            [EnumMember]
            Location = 1
        }

        [DataContract]
        public enum TypeBien
        {
            [EnumMember]
            Appartement = 0,
            [EnumMember]
            Maison = 1,
            [EnumMember]
            Garage = 2,
            [EnumMember]
            Terrain = 3
        }

        [DataContract]
        public enum TypeChauffage
        {
            [EnumMember]
            Aucun = 0,
            [EnumMember]
            Individuel = 1,
            [EnumMember]
            Collectif = 2
        }

        [DataContract]
        public enum EnergieChauffage
        {
            [EnumMember]
            Aucun = 0,
            [EnumMember]
            Fioul = 1,
            [EnumMember]
            Gaz = 2,
            [EnumMember]
            Electrique = 3,
            [EnumMember]
            Bois = 4
        }

        #endregion

        #region Personne

        [DataContract]
        public enum Qualite
        {
            [EnumMember]
            Monsieur = 0,
            [EnumMember]
            Madame = 1,
            [EnumMember]
            Mademoiselle = 2
        }

        #endregion

        #region Synchronisation

        [DataContract]
        public enum TypeOperation
        {
            [EnumMember]
            Insertion = 0,
            [EnumMember]
            Modification = 1,
            [EnumMember]
            Suppression = 2
        }

        #endregion

        #region Erreurs

        [DataContract]
        public enum ErrorType
        {
            [EnumMember]
            Message = 0,
            [EnumMember]
            Exception = 1
        }

        #endregion

    }
}
