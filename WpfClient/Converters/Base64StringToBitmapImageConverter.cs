using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows;

namespace Oyosoft.AgenceImmobiliere.WpfClient.Converters
{
    class Base64StringToBitmapImageConverter : IValueConverter
    {
        private const string DEFAULT_IMG_BASE64 = "iVBORw0KGgoAAAANSUhEUgAAADIAAAAyCAYAAAAeP4ixAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAACxIAAAsSAdLdfvwAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNvyMY98AAALlSURBVGhD3djLq01hHMbx7ZL7JZcyISETA5GBiQGimNBRBkbCP4BiRCkTExm4ZMDAQAZiYCDXgTIQJ1OSSynkfolyP77PqVXvWftZe71rn/Ze+/WrT6f97Hfd9nkva60GNfCfsGGKbJgiG6ao8TsXpEjX0PgTBKnSNdgvUmTDFNkwRTZMkQ17gQbwT8TOqjas0188wmnswWH0o2x2tWGdHmI1RkM1EgtwAa59xoZV/cJRfAiydqgbHcEo5Gse1NXcdmLDqq5iKs5jOAvsd2yDqzF4ALed2LCK11gM1Qq8hGsXQ7/4XriaAh3LbSc2rOIAsv6sv/qsAevaltF21zEL+dLAb7VfG8Z6gkUIawIew7WPofF2DDOh7jQJu/ENrn3GhjE0MA9hHPK1AV/htov1CffwHDHjzoYxXiAbG/nSr3gW7Xaxdtgwxgm4aVI1AuvxCm7bTrBhmc+Yg1Y1FudQ1i2+4B2GM22LDVvRAXchpjQRPIXbj8bYLWyGpm2t6K5dLBu2chtueiyqLcjv4yP2YS5Uug3ZCs1Y+baxbFhEB9LKWzQ2XKntTWT7ULfUxWlqDUtr0EWEx6vChkXuYj6q1kpoGr2M/LoT1mw8gzt2GRs6P7AT6gZVS4vkckwe/FRc2vd2aAJw59CKDR0N2unodM2AbkKrrkE2zNNMtQPdqnV4C3cuRWyYdwPj0a3SgrofVdYWG4Y0Va6Cdt7NmohLcOfk2DCk1Xka6qgleA93Xnk2zGjO34S6SnfWevkQs1DaMHMNeoStsxZCb1HKZjEbZtai7tLY7IOe5905ZmwoV9DO4teJ0nmcQuVH3TdYil4qLZT34c5XmgJd9UHoeaKXSl1sI4renTUFuhVZhl4s3avpjaPrYkM+qMEZaKbSf6QXrYG7fRnyQbcEd6Dn8dBxkzlZu9j2odhtTkJvWMLzlqYgVTZMkQ1TNPjiWGMjZVr1S+9hUqBrsF+kyIYpsmGKbJiYxsA/cuBLrn4kUR4AAAAASUVORK5CYII=";

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string base64String;

            if (value == null) value = DEFAULT_IMG_BASE64;

            try
            {
                base64String = (string)value;
            }
            catch { return null; }

            return Base64StringToBitmapImage(base64String);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return "";

            BitmapImage image = null;
            try
            {
                image = (BitmapImage)value;
            }
            catch { return ""; }

            return BitmapImageToBase64String(image);
        }

        public static BitmapImage Base64StringToBitmapImage(string base64String)
        {
            if (string.IsNullOrEmpty(base64String)) base64String = DEFAULT_IMG_BASE64;

            byte[] byteArray = System.Convert.FromBase64String(base64String);

            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();
                image.Freeze();
                return image;
            }
        }

        public static string BitmapImageToBase64String(BitmapImage image)
        {
            if (image == null) return "";

            using (MemoryStream stream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(stream);
                byte[] byteArray = stream.ToArray();
                return System.Convert.ToBase64String(byteArray);
            }
        }
    }
}
