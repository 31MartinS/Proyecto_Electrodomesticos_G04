using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ClienteWeb.Models;
using Newtonsoft.Json;

namespace ClienteWeb.Services
{
    public class ApiService
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        // Se sobreescribe en Program.cs:
        // var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5001/api";
        // ApiService.BaseUrl = apiBaseUrl;
        public static string BaseUrl { get; set; } = "http://localhost:5001/api";

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

        public static async Task<ProductoDto> UpdateProductoAsync(int id, CreateProductoDto producto)
        {
            var json = JsonConvert.SerializeObject(producto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{BaseUrl}/productos/{id}", content);
            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ProductoDto>(responseJson)!;
        }

        public static async Task DeleteProductoAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/productos/{id}");
            response.EnsureSuccessStatusCode();
        }

        public static async Task<string> UploadProductImageAsync(int idProducto, string filePath)
        {
            using (var form = new MultipartFormDataContent())
            using (var fileStream = File.OpenRead(filePath))
            {
                var fileContent = new StreamContent(fileStream);
                var extension = Path.GetExtension(filePath).ToLower();
                var contentType = extension switch
                {
                    ".png" => "image/png",
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".gif" => "image/gif",
                    _ => "application/octet-stream"
                };
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                form.Add(fileContent, "file", Path.GetFileName(filePath));

                var response = await _httpClient.PostAsync($"{BaseUrl}/productos/{idProducto}/image", form);
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(responseJson);
                return result?.ImageUrl ?? "";
            }
        }

        // ==================== CLIENTES ====================

        public static async Task<List<ClienteComercializadora>> GetClientesAsync()
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/clientes");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<ClienteComercializadora>>(json)
                   ?? new List<ClienteComercializadora>();
        }

        public static async Task<ClienteComercializadora?> GetClienteByCedulaAsync(string cedula)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/clientes/{cedula}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ClienteComercializadora>(json);
                }
                return null;
            }
            catch
            {
                return null;
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

        public static async Task<List<FacturaListDto>> GetFacturasAsync()
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/facturas");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<FacturaListDto>>(json) ?? new List<FacturaListDto>();
        }

        public static async Task<FacturaListDto> GetFacturaAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/facturas/{id}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FacturaListDto>(json)!;
        }
    }
}
