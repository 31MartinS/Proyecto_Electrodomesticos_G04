using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using ClienteMovil.Models;

namespace ClienteMovil.Services
{
    public class ApiService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        
        // CAMBIAR ESTA IP A LA IP DEL SERVIDOR
        public static string BaseUrl { get; set; } = "http://10.40.20.89:5001/api";

        // ==================== PRODUCTOS ====================
        
        public static async Task<List<ProductoDto>> GetProductosAsync()
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/productos");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<ProductoDto>>(json) ?? new List<ProductoDto>();
        }

        public static async Task<ProductoDto> GetProductoAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/productos/{id}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ProductoDto>(json)!;
        }

        public static async Task<ProductoDto> CreateProductoAsync(CreateProductoDto producto)
        {
            var json = JsonConvert.SerializeObject(producto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{BaseUrl}/productos", content);
            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ProductoDto>(responseJson)!;
        }

        public static async Task UpdateProductoAsync(int id, ProductoDto producto)
        {
            var json = JsonConvert.SerializeObject(producto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{BaseUrl}/productos/{id}", content);
            response.EnsureSuccessStatusCode();
        }

        public static async Task DeleteProductoAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/productos/{id}");
            response.EnsureSuccessStatusCode();
        }

        public static async Task UploadProductImageAsync(int id, FileResult imagen)
        {
            using var stream = await imagen.OpenReadAsync();
            using var content = new MultipartFormDataContent();
            using var streamContent = new StreamContent(stream);
            
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(imagen.ContentType ?? "image/jpeg");
            content.Add(streamContent, "image", imagen.FileName);
            
            var response = await _httpClient.PostAsync($"{BaseUrl}/productos/{id}/image", content);
            response.EnsureSuccessStatusCode();
        }

        // ==================== CLIENTES ====================
        
        public static async Task<List<ClienteComercializadora>> GetClientesAsync()
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/clientes");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<ClienteComercializadora>>(json) ?? new List<ClienteComercializadora>();
        }

        public static async Task<ClienteComercializadora?> GetClienteByCedulaAsync(string cedula)
        {
            try
            {
                var url = $"{BaseUrl}/clientes/{cedula}";
                System.Diagnostics.Debug.WriteLine($"[ApiService] GET {url}");
                
                var response = await _httpClient.GetAsync(url);
                
                System.Diagnostics.Debug.WriteLine($"[ApiService] Status: {response.StatusCode}");
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    System.Diagnostics.Debug.WriteLine($"[ApiService] Cliente {cedula} no encontrado (404)");
                    return null;
                }
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[ApiService] Response: {json}");
                    return JsonConvert.DeserializeObject<ClienteComercializadora>(json);
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"[ApiService] Error: {errorContent}");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ApiService] Exception: {ex.Message}");
                throw new Exception($"Error de conexión: {ex.Message}\n\nAsegúrese de que:\n1. El servidor está ejecutándose\n2. Está conectado a la red WiFi\n3. Puede acceder a http://10.40.20.89:5001", ex);
            }
        }

        public static async Task<ClienteComercializadora> CreateClienteAsync(ClienteComercializadora cliente)
        {
            var json = JsonConvert.SerializeObject(cliente);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{BaseUrl}/clientes", content);
            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ClienteComercializadora>(responseJson)!;
        }

        // ==================== FACTURAS ====================
        
        public static async Task<FacturaResponseDto> CreateFacturaAsync(CreateFacturaDto factura)
        {
            var json = JsonConvert.SerializeObject(factura);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{BaseUrl}/facturas", content);
            
            var responseJson = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Error al crear factura: {responseJson}");
            }
            
            return JsonConvert.DeserializeObject<FacturaResponseDto>(responseJson)!;
        }

        public static async Task<List<FacturaListDto>> GetFacturasByCedulaAsync(string cedula)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/facturas/Cliente/{cedula}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<FacturaListDto>>(json) ?? new List<FacturaListDto>();
        }

        public static async Task<AmortizacionResponseDto> GetAmortizacionAsync(int idCredito)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/facturas/Amortizacion/{idCredito}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<AmortizacionResponseDto>(json)!;
        }
    }
}
