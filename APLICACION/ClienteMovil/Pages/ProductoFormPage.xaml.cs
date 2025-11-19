using ClienteMovil.Models;
using ClienteMovil.Services;

namespace ClienteMovil.Pages;

public partial class ProductoFormPage : ContentPage
{
    private ProductoDto? _productoExistente;
    private FileResult? _imagenSeleccionada;

    public ProductoFormPage()
    {
        InitializeComponent();
        LblTitulo.Text = "Nuevo Producto";
    }

    public ProductoFormPage(ProductoDto producto)
    {
        InitializeComponent();
        _productoExistente = producto;
        LblTitulo.Text = "Editar Producto";
        CargarDatosProducto();
    }

    private void CargarDatosProducto()
    {
        if (_productoExistente != null)
        {
            EntCodigo.Text = _productoExistente.Codigo;
            EntNombre.Text = _productoExistente.Nombre;
            EditorDescripcion.Text = _productoExistente.Descripcion;
            EntPrecio.Text = _productoExistente.PrecioVenta.ToString("0.00");

            if (!string.IsNullOrEmpty(_productoExistente.ImageUrl))
            {
                ImagenActualStack.IsVisible = true;
                ImgActual.Source = _productoExistente.ImageUrl;
            }
        }
    }

    private async void OnSeleccionarImagenClicked(object sender, EventArgs e)
    {
        try
        {
            var result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Seleccionar imagen del producto"
            });

            if (result != null)
            {
                _imagenSeleccionada = result;
                LblImagenSeleccionada.Text = $"Imagen seleccionada: {result.FileName}";
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo seleccionar la imagen: {ex.Message}", "OK");
        }
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        // Validaciones
        if (string.IsNullOrWhiteSpace(EntCodigo.Text))
        {
            await DisplayAlert("Error", "El código es requerido", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(EntNombre.Text))
        {
            await DisplayAlert("Error", "El nombre es requerido", "OK");
            return;
        }

        if (!decimal.TryParse(EntPrecio.Text, out decimal precio) || precio <= 0)
        {
            await DisplayAlert("Error", "El precio debe ser un número válido mayor a 0", "OK");
            return;
        }

        try
        {
            if (_productoExistente == null)
            {
                // Crear nuevo producto
                var nuevoProducto = new CreateProductoDto
                {
                    Codigo = EntCodigo.Text.Trim(),
                    Nombre = EntNombre.Text.Trim(),
                    Descripcion = EditorDescripcion.Text?.Trim(),
                    PrecioVenta = precio
                };

                var productoCreado = await ApiService.CreateProductoAsync(nuevoProducto);

                // Subir imagen si se seleccionó
                if (_imagenSeleccionada != null && productoCreado != null)
                {
                    await ApiService.UploadProductImageAsync(productoCreado.IdProducto, _imagenSeleccionada);
                }

                await DisplayAlert("Éxito", "Producto creado exitosamente", "OK");
            }
            else
            {
                // Actualizar producto existente
                var productoActualizado = new ProductoDto
                {
                    IdProducto = _productoExistente.IdProducto,
                    Codigo = EntCodigo.Text.Trim(),
                    Nombre = EntNombre.Text.Trim(),
                    Descripcion = EditorDescripcion.Text?.Trim(),
                    PrecioVenta = precio,
                    ImageUrl = _productoExistente.ImageUrl
                };

                await ApiService.UpdateProductoAsync(_productoExistente.IdProducto, productoActualizado);

                // Subir nueva imagen si se seleccionó
                if (_imagenSeleccionada != null)
                {
                    await ApiService.UploadProductImageAsync(_productoExistente.IdProducto, _imagenSeleccionada);
                }

                await DisplayAlert("Éxito", "Producto actualizado exitosamente", "OK");
            }

            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo guardar el producto: {ex.Message}", "OK");
        }
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
