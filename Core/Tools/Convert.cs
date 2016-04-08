using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oyosoft.AgenceImmobiliere.Core.Tools
{
    public static class Convert
    {

        public static string ToString(object valeur, bool trim = true)
        {
            //if (object.ReferenceEquals(valeur, System.DBNull.Value))
            //    return "";
            if (string.IsNullOrEmpty(System.Convert.ToString(valeur)))
                return "";

            if (trim)
            {
                return System.Convert.ToString(valeur).Trim();
            }
            else
            {
                return System.Convert.ToString(valeur);
            }
        }

        public static decimal ToDecimal(object valeur)
        {
            //if (object.ReferenceEquals(valeur, System.DBNull.Value))
            //    return 0;
            if (valeur == null)
                return 0;

            decimal dec = 0;
            string strDec = null;

            strDec = System.Convert.ToString(valeur);
            if (decimal.TryParse(strDec, out dec))
                return dec;

            strDec = System.Convert.ToString(valeur).Replace('.', ',');
            if (decimal.TryParse(strDec, out dec))
                return dec;

            strDec = System.Convert.ToString(valeur).Replace(',', '.');
            if (decimal.TryParse(strDec, out dec))
                return dec;

            return 0;
        }

        public static double ToDouble(object valeur)
        {
            //if (object.ReferenceEquals(valeur, System.DBNull.Value))
            //    return 0;
            if (valeur == null)
                return 0;

            double dbl = 0;
            string strDbl = null;

            strDbl = System.Convert.ToString(valeur);
            if (double.TryParse(strDbl, out dbl))
                return dbl;

            strDbl = System.Convert.ToString(valeur).Replace('.', ',');
            if (double.TryParse(strDbl, out dbl))
                return dbl;

            strDbl = System.Convert.ToString(valeur).Replace(',', '.');
            if (double.TryParse(strDbl, out dbl))
                return dbl;

            return 0;
        }

        public static int ToInt(object valeur)
        {
            //if (object.ReferenceEquals(valeur, System.DBNull.Value))
            //    return 0;
            if (valeur == null)
                return 0;

            int i = 0;
            string strInt = null;

            strInt = System.Convert.ToString(valeur);
            if (int.TryParse(strInt, out i))
                return i;

            return 0;
        }

        public static bool ToBool(object valeur)
        {
            //if (object.ReferenceEquals(valeur, System.DBNull.Value))
            //    return false;
            if (valeur == null)
                return false;

            string strBool = System.Convert.ToString(valeur).ToUpper();
            if (strBool == "1" || strBool == "VRAI" || strBool == "O" || strBool == "T" || strBool == "YES" || strBool == "Y" || strBool == "TRUE")
                return true;

            return false;
        }

        public static DateTime? ToDate(object valeur)
        {
            //if (object.ReferenceEquals(valeur, System.DBNull.Value))
            //    return null;
            if (valeur == null)
                return null;

            return System.Convert.ToDateTime(valeur);
        }

        public static string FormatSQL<TSelf>(TSelf valeur)
        {
            System.Type tValeur = null;

            if (valeur == null)
                tValeur = GetDeclaredType(valeur);
            else
                tValeur = valeur.GetType();

            return FormatSQL(valeur, tValeur);
        }
        private static string FormatSQL(object valeur, System.Type tValeur)
        {
            string strValeur = "";

            if (tValeur == typeof(DateTime) || tValeur == typeof(DateTime?))
            {
                if (valeur == null) return "null";
                strValeur = string.Format("Datetime('{0}')", ((DateTime)valeur).ToString("yyyy-MM-dd HH:mm:ss"));
            }
            else if (tValeur == typeof(string) || tValeur == typeof(char) || tValeur == typeof(char?))
            {
                if (valeur == null) return "''";
                strValeur = string.Format("'{0}'", ((string)valeur).Replace("'", "''"));
            }
            else if (tValeur == typeof(Single) || tValeur == typeof(double) || tValeur == typeof(decimal) || tValeur == typeof(Single?) || tValeur == typeof(double?) || tValeur == typeof(decimal?))
            {
                if (valeur == null) return "0";
                strValeur = valeur.ToString().Replace(",", ".");
            }
            else if (tValeur == typeof(bool) || tValeur == typeof(bool?))
            {
                if (valeur == null) return "0";
                strValeur = ((bool)valeur) ? "1" : "0";
            }
            else
            {
                if (valeur == null) return "";
                strValeur = valeur.ToString();
            }

            return strValeur;
        }
        private static System.Type GetDeclaredType<TSelf>(TSelf self)
        {
            return typeof(TSelf);
        }

        public static byte[] Base64StringToBytes(string base64String, string defaultString = "")
        {
            if (string.IsNullOrEmpty(base64String)) base64String = defaultString;
            if (string.IsNullOrEmpty(defaultString)) return new byte[] { };
            return System.Convert.FromBase64String(base64String);
        }

        public static string BytesToBase64String(byte[] bytes)
        {
            if (bytes == null) return "";
            return System.Convert.ToBase64String(bytes);
        }

    }
}
