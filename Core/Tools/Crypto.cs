using System.Text;


namespace Oyosoft.AgenceImmobiliere.Core.Tools
{
    public class Crypto
    {
        private const int KEY = 79318;
        private const int A = 27;
        private const int B = 543;


        public static string Encrypt(string clear)
        {
            int NouvCar = 0;
            int Tmp = 0;
            int i = 0;
            StringBuilder SB = new StringBuilder();
            int hash = 0;

            if (string.IsNullOrEmpty(clear)) return "";

            // Calcul de hash
            for (i = 0; i <= clear.Length - 1; i++)
            {
                hash += (int)clear[i];
            }
            hash = (hash * B) % A;
            if (hash == 0)
                hash = B;

            SB.Append(hash.ToString("X").PadLeft(2, '0'));

            // Création de la nouvelle chaine
            for (i = 0; i <= clear.Length - 1; i++)
            {
                NouvCar = (int)clear[i];

                Tmp = KEY % ((i + A) * hash);

                NouvCar += Tmp * (i + A);
                NouvCar = NouvCar % 255;

                SB.Append(hash.ToString("X").PadLeft(2, '0'));
            }

            return SB.ToString();
        }

        public static string Decrypt(string encrypted)
        {
            StringBuilder SB = new StringBuilder();
            int NouvCar = 0;
            int Tmp = 0;
            int i = 0;
            int IndCar = 0;
            int hash = 0;

            if (string.IsNullOrEmpty(encrypted))
                return "";
            if (encrypted.Length % 2 != 0)
                return "";

            // Lecture du hash
            if (!int.TryParse(encrypted.Substring(0, 2), System.Globalization.NumberStyles.HexNumber, null, out hash))
            {
                return "";
            }

            if (hash == 0)
                hash = B;

            for (i = 2; i <= encrypted.Length - 1; i += 2)
            {
                NouvCar = int.Parse(encrypted.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);

                Tmp = KEY % ((IndCar + A) * hash);

                NouvCar -= Tmp * (IndCar + A);

                NouvCar = 255 + (NouvCar % 255);

                SB.Append((char)NouvCar);

                IndCar += 1;
            }

            return SB.ToString();
        }

    }
}
