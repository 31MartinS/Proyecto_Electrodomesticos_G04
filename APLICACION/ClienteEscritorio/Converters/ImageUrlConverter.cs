using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ClienteEscritorio.Converters
{
    public class ImageUrlConverter : IValueConverter
    {
        private static readonly HttpClient _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(10)
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                System.Diagnostics.Debug.WriteLine("[ImageUrlConverter] Valor nulo o vacío");
                return null;
            }

            string url = value.ToString();
            System.Diagnostics.Debug.WriteLine($"[ImageUrlConverter] Procesando URL: {url}");

            try
            {
                // Si es una URL HTTP/HTTPS, usar BitmapImage con UriSource
                if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                    url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine($"[ImageUrlConverter] Cargando desde URL: {url}");
                        
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(url, UriKind.Absolute);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                        bitmap.EndInit();
                        
                        // No freeze, para permitir actualizaciones
                        System.Diagnostics.Debug.WriteLine($"[ImageUrlConverter] Imagen cargada exitosamente");
                        return bitmap;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[ImageUrlConverter] ERROR cargando imagen desde URL {url}: {ex.Message}");
                        System.Diagnostics.Debug.WriteLine($"[ImageUrlConverter] StackTrace: {ex.StackTrace}");
                        return null;
                    }
                }

                // Si es una ruta local
                if (File.Exists(url))
                {
                    System.Diagnostics.Debug.WriteLine($"[ImageUrlConverter] Cargando archivo local: {url}");
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(url, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    return bitmap;
                }
                
                System.Diagnostics.Debug.WriteLine($"[ImageUrlConverter] URL no es HTTP ni archivo local válido: {url}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ImageUrlConverter] ERROR general: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[ImageUrlConverter] StackTrace: {ex.StackTrace}");
                return null;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
