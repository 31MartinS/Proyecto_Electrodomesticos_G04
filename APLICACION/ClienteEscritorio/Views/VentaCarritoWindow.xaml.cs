using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ClienteEscritorio.Models;
using ClienteEscritorio.Services;

namespace ClienteEscritorio.Views
{
    public partial class VentaCarritoWindow : Window
    {
        private readonly string _cedula;
        private ObservableCollection<CarritoItem> _carritoItems;

        public VentaCarritoWindow(string cedula)
        {
            InitializeComponent();
            _cedula = cedula;
            _carritoItems = new ObservableCollection<CarritoItem>();
            LvCarrito.ItemsSource = _carritoItems;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var productos = await ApiService.GetProductosAsync();
                DgProductos.ItemsSource = productos;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar productos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAgregarCarrito_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var producto = button?.Tag as ProductoDto;

                if (producto == null)
                {
                    MessageBox.Show("Error al obtener el producto", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Buscar el TextBox de cantidad en la misma fila
                var parent = button.Parent as StackPanel;
                var txtCantidad = parent?.Children.OfType<TextBox>().FirstOrDefault();

                if (txtCantidad == null || !int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0)
                {
                    MessageBox.Show("Por favor ingrese una cantidad válida", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Verificar si el producto ya está en el carrito
                var itemExistente = _carritoItems.FirstOrDefault(i => i.IdProducto == producto.IdProducto);

                if (itemExistente != null)
                {
                    // Actualizar cantidad
                    itemExistente.Cantidad += cantidad;
                    itemExistente.Subtotal = itemExistente.Cantidad * itemExistente.Precio;
                    
                    // Forzar actualización del ListView
                    LvCarrito.Items.Refresh();
                }
                else
                {
                    // Agregar nuevo item
                    _carritoItems.Add(new CarritoItem
                    {
                        IdProducto = producto.IdProducto,
                        Nombre = producto.Nombre,
                        Precio = producto.PrecioVenta,
                        Cantidad = cantidad,
                        Subtotal = producto.PrecioVenta * cantidad
                    });
                }

                // Resetear cantidad a 1
                if (txtCantidad != null)
                    txtCantidad.Text = "1";

                ActualizarTotal();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar al carrito: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEliminarItem_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button?.Tag as CarritoItem;

            if (item != null)
            {
                _carritoItems.Remove(item);
                ActualizarTotal();
            }
        }

        private void ActualizarTotal()
        {
            decimal total = _carritoItems.Sum(i => i.Subtotal);
            TxtTotal.Text = total.ToString("C2");
        }

        private async void BtnPagarEfectivo_Click(object sender, RoutedEventArgs e)
        {
            if (_carritoItems.Count == 0)
            {
                MessageBox.Show("El carrito está vacío. Por favor agregue productos.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var createFacturaDto = new CreateFacturaDto
                {
                    Cedula = _cedula,
                    FormaPago = "Efectivo",
                    PlazoMeses = 0,
                    Detalles = _carritoItems.Select(item => new DetalleFacturaDto
                    {
                        IdProducto = item.IdProducto,
                        Cantidad = item.Cantidad
                    }).ToList()
                };

                var resultado = await ApiService.CreateFacturaAsync(createFacturaDto);

                // Mostrar ventana de éxito
                var ventaExitoWindow = new VentaEfectivoExitoWindow(resultado);
                this.Close();
                ventaExitoWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al procesar la venta: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnPagarCredito_Click(object sender, RoutedEventArgs e)
        {
            if (_carritoItems.Count == 0)
            {
                MessageBox.Show("El carrito está vacío. Por favor agregue productos.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Calcular total
            decimal total = _carritoItems.Sum(i => i.Subtotal);

            // Abrir ventana de crédito con el carrito completo
            var ventaCreditoWindow = new VentaCreditoCarritoWindow(_cedula, _carritoItems.ToList(), total);
            this.Close();
            ventaCreditoWindow.ShowDialog();
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    // Clase auxiliar para los items del carrito
    public class CarritoItem : INotifyPropertyChanged
    {
        private int _cantidad;
        private decimal _subtotal;

        public int IdProducto { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        
        public int Cantidad 
        { 
            get => _cantidad;
            set
            {
                if (_cantidad != value)
                {
                    _cantidad = value;
                    OnPropertyChanged(nameof(Cantidad));
                    // Actualizar subtotal automáticamente
                    Subtotal = _cantidad * Precio;
                }
            }
        }
        
        public decimal Subtotal 
        { 
            get => _subtotal;
            set
            {
                if (_subtotal != value)
                {
                    _subtotal = value;
                    OnPropertyChanged(nameof(Subtotal));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
