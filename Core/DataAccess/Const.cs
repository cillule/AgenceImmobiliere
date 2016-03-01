using System;
using System.Reflection;
using SQLite.Net.Attributes;

namespace Oyosoft.AgenceImmobiliere.Core.DataAccess
{
    public class Const
    {
        internal const string ADMIN_USERNAME = "Admin";
        internal const string ADMIN_PASSWORD = "cool";
        internal const Enums.Qualite ADMIN_QUALITY = Enums.Qualite.Monsieur;
        internal const string ADMIN_NAME = "ADMINISTRATEUR";
        internal const string ADMIN_FIRSTNAME = "Agence";

        internal const string DB_COMMON_ID_COLNAME = "id";
        internal const string DB_COMMON_NOMUTILISATEURCREATION_COLNAME = "nom_utilisateur_creation";
        internal const string DB_COMMON_DATEHEURECREATION_COLNAME = "date_heure_creation";
        internal const string DB_COMMON_DATEHEUREMODIFICATION_COLNAME = "date_heure_modification";
        internal const string DB_COMMON_ADRESSE_COLNAME = "adresse";
        internal const string DB_COMMON_CODEPOSTAL_COLNAME = "code_postal";
        internal const string DB_COMMON_VILLE_COLNAME = "ville";
        internal const string DB_COMMON_LATITUDE_COLNAME = "latitude";
        internal const string DB_COMMON_LONGITUDE_COLNAME = "longitude";
        internal const string DB_COMMON_ALTITUDE_COLNAME = "altitude";

        internal const string DB_BIEN_TABLENAME = "bien";
        internal const string DB_BIEN_TITRE_COLNAME = "titre";
        internal const string DB_BIEN_TYPETRANSACTION_COLNAME = "type_transaction";
        internal const string DB_BIEN_TYPEBIEN_COLNAME = "type_bien";
        internal const string DB_BIEN_DESCRIPTION_COLNAME = "description";
        internal const string DB_BIEN_PRIXPROPRIETAIRE_COLNAME = "prix_proprietaire";
        internal const string DB_BIEN_MONTANTHONORAIRESTRANSACTION_COLNAME = "montant_honoraires_transaction";
        internal const string DB_BIEN_MONTANTHONORAIRESMENSUELS_COLNAME = "montant_honoraires_mensuels";
        internal const string DB_BIEN_MONTANTCHARGES_COLNAME = "montant_charges";
        internal const string DB_BIEN_SURFACE_COLNAME = "surface";
        internal const string DB_BIEN_NBPIECES_COLNAME = "nombre_pieces";
        internal const string DB_BIEN_NUMETAGE_COLNAME = "numero_etage";
        internal const string DB_BIEN_NBETAGES_COLNAME = "nombre_etages";
        internal const string DB_BIEN_TYPECHAUFFAGE_COLNAME = "type_chauffage";
        internal const string DB_BIEN_ENERGIECHAUFFAGE_COLNAME = "energie_chauffage";
        internal const string DB_BIEN_IDPROPRIETAIRE_COLNAME = "id_proprietaire";
        internal const string DB_BIEN_IDCOMMERCIAL_COLNAME = "id_commercial";
        internal const string DB_BIEN_DATEMISEENTRANSACTION_COLNAME = "date_mise_en_transaction";
        internal const string DB_BIEN_TRANSACTIONEFFECTUEE_COLNAME = "transaction_effectuee";
        internal const string DB_BIEN_IDACQUEREUR_COLNAME = "id_acquereur";
        internal const string DB_BIEN_DATETRANSACTION_COLNAME = "date_transaction";
        
        internal const string DB_PHOTO_TABLENAME = "photo";
        public const string DB_PHOTO_IDBIEN_COLNAME = "id_bien";
        internal const string DB_PHOTO_PRINCIPALE_COLNAME = "principale";
        internal const string DB_PHOTO_BASE64_COLNAME = "base64";

        internal const string DB_PERSONNE_TABLENAME = "personne";
        internal const string DB_PERSONNE_QUALITE_COLNAME = "qualite";
        internal const string DB_PERSONNE_NOM_COLNAME = "nom";
        internal const string DB_PERSONNE_PRENOM_COLNAME = "prenom";
        internal const string DB_PERSONNE_MAIL_COLNAME = "mail";
        internal const string DB_PERSONNE_TELEPHONE_COLNAME = "telephone";
        internal const string DB_PERSONNE_PORTABLE_COLNAME = "portable";
        internal const string DB_PERSONNE_DATENAISSANCE_COLNAME = "date_naissance";
        internal const string DB_PERSONNE_NOMNAISSANCE_COLNAME = "nom_naissance";
        internal const string DB_PERSONNE_VILLENAISSANCE_COLNAME = "ville_naissance";
        internal const string DB_PERSONNE_PHOTOBASE64_COLNAME = "photo_base64";

        internal const string DB_UTILISATEUR_TABLENAME = "utilisateur";
        internal const string DB_UTILISATEUR_IDPERSONNE_COLNAME = "id_personne";
        internal const string DB_UTILISATEUR_NOMUTILISATEUR_COLNAME = "nom_utilisateur";
        internal const string DB_UTILISATEUR_MOTDEPASSE_COLNAME = "mot_de_passe";

        internal const string DB_PARAMETRE_TABLENAME = "parametre";
        internal const string DB_PARAMETRE_CLE_COLNAME = "cle";
        internal const string DB_PARAMETRE_VALEUR_COLNAME = "valeur";

        internal static string NomTableSelonType<T>() where T : Model.ModeleBase
        {
            if (typeof(T) == typeof(Model.BienImmobilier)) return DB_BIEN_TABLENAME;
            if (typeof(T) == typeof(Model.Personne)) return DB_PERSONNE_TABLENAME;
            if (typeof(T) == typeof(Model.PhotoBienImmobilier)) return DB_PHOTO_TABLENAME;
            if (typeof(T) == typeof(Model.Utilisateur)) return DB_UTILISATEUR_TABLENAME;
            return "";
        }
        internal static string NomTableSelonAttribut<T>() where T : class
        {
            System.Type t = typeof(T);
            TypeInfo ti = t.GetTypeInfo();

            Attribute attr = ti.GetCustomAttribute(typeof(TableAttribute));
            if (attr == null) return "";

            return ((TableAttribute)attr).Name;
        }

        internal static string NomSelonChamp(Enums.ChampElement champ)
        {
            string nom = "";
            switch (champ)
            {
                case Enums.ChampElement.Adresse:
                    nom = Const.DB_COMMON_ADRESSE_COLNAME;
                    break;
                case Enums.ChampElement.Altitude:
                    nom = Const.DB_COMMON_ALTITUDE_COLNAME;
                    break;
                case Enums.ChampElement.CodePostal:
                    nom = Const.DB_COMMON_CODEPOSTAL_COLNAME;
                    break;
                case Enums.ChampElement.DateMiseEnTransaction:
                    nom = Const.DB_BIEN_DATEMISEENTRANSACTION_COLNAME;
                    break;
                case Enums.ChampElement.DateNaissance:
                    nom = Const.DB_PERSONNE_DATENAISSANCE_COLNAME;
                    break;
                case Enums.ChampElement.DateTransaction:
                    nom = Const.DB_BIEN_DATETRANSACTION_COLNAME;
                    break;
                case Enums.ChampElement.Description:
                    nom = Const.DB_BIEN_DESCRIPTION_COLNAME;
                    break;
                case Enums.ChampElement.EnergieChauffage:
                    nom = Const.DB_BIEN_ENERGIECHAUFFAGE_COLNAME;
                    break;
                case Enums.ChampElement.Id:
                    nom = Const.DB_COMMON_ID_COLNAME;
                    break;
                case Enums.ChampElement.IdAcquereur:
                    nom = Const.DB_BIEN_IDACQUEREUR_COLNAME;
                    break;
                case Enums.ChampElement.IdPersonne:
                    nom = Const.DB_UTILISATEUR_IDPERSONNE_COLNAME;
                    break;
                case Enums.ChampElement.IdProprietaire:
                    nom = Const.DB_BIEN_IDPROPRIETAIRE_COLNAME;
                    break;
                case Enums.ChampElement.Latitude:
                    nom = Const.DB_COMMON_LATITUDE_COLNAME;
                    break;
                case Enums.ChampElement.Longitude:
                    nom = Const.DB_COMMON_LONGITUDE_COLNAME;
                    break;
                case Enums.ChampElement.Mail:
                    nom = Const.DB_PERSONNE_MAIL_COLNAME;
                    break;
                case Enums.ChampElement.MontantCharges:
                    nom = Const.DB_BIEN_MONTANTCHARGES_COLNAME;
                    break;
                case Enums.ChampElement.MontantHonorairesMensuels:
                    nom = Const.DB_BIEN_MONTANTHONORAIRESMENSUELS_COLNAME;
                    break;
                case Enums.ChampElement.MontantHonorairesTransaction:
                    nom = Const.DB_BIEN_MONTANTHONORAIRESTRANSACTION_COLNAME;
                    break;
                case Enums.ChampElement.MotDePasseCrypte:
                    nom = Const.DB_UTILISATEUR_MOTDEPASSE_COLNAME;
                    break;
                case Enums.ChampElement.NbEtages:
                    nom = Const.DB_BIEN_NBETAGES_COLNAME;
                    break;
                case Enums.ChampElement.NbPieces:
                    nom = Const.DB_BIEN_NBPIECES_COLNAME;
                    break;
                case Enums.ChampElement.Nom:
                    nom = Const.DB_PERSONNE_NOM_COLNAME;
                    break;
                case Enums.ChampElement.NomNaissance:
                    nom = Const.DB_PERSONNE_NOMNAISSANCE_COLNAME;
                    break;
                case Enums.ChampElement.NomUtilisateur:
                    nom = Const.DB_UTILISATEUR_NOMUTILISATEUR_COLNAME;
                    break;
                case Enums.ChampElement.IdCommercial:
                    nom = Const.DB_BIEN_IDCOMMERCIAL_COLNAME;
                    break;
                case Enums.ChampElement.NumeroPortable:
                    nom = Const.DB_PERSONNE_PORTABLE_COLNAME;
                    break;
                case Enums.ChampElement.NumeroTelephone:
                    nom = Const.DB_PERSONNE_TELEPHONE_COLNAME;
                    break;
                case Enums.ChampElement.NumEtage:
                    nom = Const.DB_BIEN_NUMETAGE_COLNAME;
                    break;
                case Enums.ChampElement.Prenom:
                    nom = Const.DB_PERSONNE_PRENOM_COLNAME;
                    break;
                case Enums.ChampElement.PrixProprietaire:
                    nom = Const.DB_BIEN_PRIXPROPRIETAIRE_COLNAME;
                    break;
                case Enums.ChampElement.Qualite:
                    nom = Const.DB_PERSONNE_QUALITE_COLNAME;
                    break;
                case Enums.ChampElement.Surface:
                    nom = Const.DB_BIEN_SURFACE_COLNAME;
                    break;
                case Enums.ChampElement.Titre:
                    nom = Const.DB_BIEN_TITRE_COLNAME;
                    break;
                case Enums.ChampElement.TransactionEffectuee:
                    nom = Const.DB_BIEN_TRANSACTIONEFFECTUEE_COLNAME;
                    break;
                case Enums.ChampElement.TypeBien:
                    nom = Const.DB_BIEN_TYPEBIEN_COLNAME;
                    break;
                case Enums.ChampElement.TypeChauffage:
                    nom = Const.DB_BIEN_TYPECHAUFFAGE_COLNAME;
                    break;
                case Enums.ChampElement.TypeTransaction:
                    nom = Const.DB_BIEN_TYPETRANSACTION_COLNAME;
                    break;
                case Enums.ChampElement.Ville:
                    nom = Const.DB_COMMON_VILLE_COLNAME;
                    break;
                case Enums.ChampElement.VilleNaissance:
                    nom = Const.DB_PERSONNE_VILLENAISSANCE_COLNAME;
                    break;
                case Enums.ChampElement.NomUtilisateurCreation:
                    nom = Const.DB_COMMON_NOMUTILISATEURCREATION_COLNAME;
                    break;
                case Enums.ChampElement.DateHeureCreation:
                    nom = Const.DB_COMMON_DATEHEURECREATION_COLNAME;
                    break;
                case Enums.ChampElement.DateHeureModification:
                    nom = Const.DB_COMMON_DATEHEUREMODIFICATION_COLNAME;
                    break;

            }
            return nom;
        }
    }
}
