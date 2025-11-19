using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ClienteEscritorio.Converters
{
    public class ImageUrlToSourceConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return null;

            try
            {
                var imageUrl = value.ToString();
                if (string.IsNullOrEmpty(imageUrl))
                    return null;

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imageUrl, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache; // Para actualizar cuando cambien
                bitmap.EndInit();
                bitmap.Freeze(); // Para mejor rendimiento
                
                return bitmap;
            }
            catch
            {
                // Si hay error al cargar, retornar null (se mostrará el placeholder)
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
