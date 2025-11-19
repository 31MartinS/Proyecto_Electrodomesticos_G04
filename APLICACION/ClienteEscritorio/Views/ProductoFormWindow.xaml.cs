using System.Windows;
using System;
using ClienteEscritorio.Models;
using ClienteEscritorio.Services;

namespace ClienteEscritorio.Views
{
    public partial class ProductoFormWindow : Window
    {
        private ProductoDto? _productoExistente;

        public ProductoFormWindow()
        {
            InitializeComponent();
            TxtTitulo.Text = "Crear Producto";
            BtnSubirImagen.IsEnabled = false;
            TxtImagenInfo.Text = "Nota: Primero guarde el producto para poder subir una imagen";
        }

        public ProductoFormWindow(ProductoDto producto)
        {
            InitializeComponent();
            TxtTitulo.Text = "Editar Producto";
            _productoExistente = producto;
            
            // Cargar datos existentes
            TxtCodigo.Text = producto.Codigo;
            TxtNombre.Text = producto.Nombre;
            TxtDescripcion.Text = producto.Descripcion ?? "";
            TxtPrecio.Text = producto.PrecioVenta.ToString("F2");
            
            // Habilitar botón de imagen
            BtnSubirImagen.IsEnabled = true;
            TxtImagenInfo.Text = string.IsNullOrEmpty(producto.ImageUrl) 
                ? "Sin imagen. Haga clic para subir una." 
                : "Imagen actual: " + System.IO.Path.GetFileName(producto.ImageUrl);
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(TxtCodigo.Text))
            {
                MessageBox.Show("El código es requerido", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtNombre.Text))
            {
                MessageBox.Show("El nombre es requerido", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(TxtPrecio.Text, out decimal precio) || precio <= 0)
            {
                MessageBox.Show("El precio debe ser un número válido mayor a 0", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var productoDto = new CreateProductoDto
                {
                    Codigo = TxtCodigo.Text.Trim(),
                    Nombre = TxtNombre.Text.Trim(),
                    Descripcion = string.IsNullOrWhiteSpace(TxtDescripcion.Text) ? null : TxtDescripcion.Text.Trim(),
                    PrecioVenta = precio
                };

                if (_productoExistente == null)
                {
                    // Crear nuevo
                    await ApiService.CreateProductoAsync(productoDto);
                    MessageBox.Show("Producto creado exitosamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Actualizar existente
                    await ApiService.UpdateProductoAsync(_productoExistente.IdProducto, productoDto);
                    MessageBox.Show("Producto actualizado exitosamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar producto: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private async void BtnSubirImagen_Click(object sender, RoutedEventArgs e)
        {
            if (_productoExistente == null)
            {
                MessageBox.Show("Primero debe guardar el producto antes de subir una imagen.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Imágenes (*.png;*.jpg;*.jpeg;*.gif)|*.png;*.jpg;*.jpeg;*.gif",
                Title = "Seleccionar imagen del producto"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    BtnSubirImagen.IsEnabled = false;
                    BtnSubirImagen.Content = "Subiendo...";

                    string imageUrl = await ApiService.UploadProductImageAsync(_productoExistente.IdProducto, openFileDialog.FileName);
                    
                    MessageBox.Show("Imagen subida exitosamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Actualizar la URL de la imagen en el objeto existente
                    _productoExistente.ImageUrl = imageUrl;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al subir imagen: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    BtnSubirImagen.IsEnabled = true;
                    BtnSubirImagen.Content = "Subir Imagen";
                }
            }
        }
    }
}
