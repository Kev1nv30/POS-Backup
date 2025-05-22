using FerreteriaPOS.Data;
using FerreteriaPOS.Data.Entities;
using FerreteriaPOS.Data.Services;
using FerreteriaPOS.UI.Sprints;
using FerreteriaPOS.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FerreteriaPOS.UI.Helpers;
using FerreteriaPOS.UI.Windows;

namespace FerreteriaPOS.UI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private List<Producto> _productos = new();
    private List<Producto> _productosFiltrados = new();
    private List<Cliente> _clientes = new();
    private List<Cliente> _clientesFiltrados = new();
    private readonly FerreteriaContext _context;
    private VentasViewModel _ventasViewModel;
    
    // Comandos para teclas de función
    public ICommand BuscarProductoCommand { get; private set; } = null!;

    // Helpers
    private PaymentScreenHelper _paymentHelper = null!;
    private DialogHelper _dialogHelper = null!;
    private ProductSearchHelper _productSearchHelper = null!;
    private ClientSearchHelper _clientSearchHelper = null!;
    private NavigationHelper _navigationHelper = null!;
    private KeyboardHelper _keyboardHelper = null!;
    private SalesHelper _salesHelper = null!;
    private ProductManagementHelper _productManagementHelper = null!;
    private ClientManagementHelper _clientManagementHelper = null!;

    public MainWindow()
    {
        InitializeComponent();

        // Inicializar listas
        _productos = new List<Producto>();
        _productosFiltrados = new List<Producto>();
        _clientes = new List<Cliente>();
        _clientesFiltrados = new List<Cliente>();
        
        // Inicializar el contexto de datos
        _context = new FerreteriaContext();
        
        // Inicializar el ViewModel de Ventas
        _ventasViewModel = new VentasViewModel(_context);
        
        // Configurar los comandos
        BuscarProductoCommand = new RelayCommand(_ => 
        {
            if (_navigationHelper?.CurrentContent == "ventas" && !_paymentHelper.IsInPaymentScreen)
            {
                _productSearchHelper.BuscarProductoModal();
            }
        });
        
        // Establecer el DataContext para los comandos
        this.DataContext = this;
        
        // Inicializar los elementos de la vista de ventas
        InitializeVentasView();
        
        // Inicializar helpers
        InitializeHelpers();
        
        // Asegurarse de que el contenido de Ventas está visible por defecto
        _navigationHelper.ShowVentasContent();
    }
    
    private void InitializeHelpers()
    {
        // Inicializar el repositorio
        var repositoryService = new RepositoryService(_context);
        
        // Inicializar helpers
        _dialogHelper = new DialogHelper(this, _context);
        _paymentHelper = new PaymentScreenHelper(this, _ventasViewModel, txtStatus, repositoryService);
        _productSearchHelper = new ProductSearchHelper(this, _ventasViewModel, _context, txtStatus);
        _clientSearchHelper = new ClientSearchHelper(this, _ventasViewModel, txtClienteSeleccionado, chkEsFactura, txtStatus, _dialogHelper);
        
        Button[] navigationButtons = { btnF1Ventas, btnF2Clientes, btnF3Productos, btnF4Inventario, btnF5Configuracion };
        _navigationHelper = new NavigationHelper(
            ventasContent,
            productosContent,
            clientesContent,
            configuracionContent,
            txtStatus,
            navigationButtons
        );
        
        _salesHelper = new SalesHelper(this, _ventasViewModel, txtClienteSeleccionado, chkEsFactura, txtStatus, _dialogHelper, _paymentHelper, _context);
        _keyboardHelper = new KeyboardHelper(_ventasViewModel, _navigationHelper, _paymentHelper, _productSearchHelper, _salesHelper, txtStatus);
        
        _productManagementHelper = new ProductManagementHelper(
            this,
            _context,
            txtStatus,
            txtProductCount,
            txtBuscarProd,
            dgProductos,
            _productos,
            _productosFiltrados
        );
        
        _clientManagementHelper = new ClientManagementHelper(
            this,
            _context,
            txtStatus,
            txtClienteCount,
            txtBuscarCliente,
            dgClientes,
            _clientes,
            _clientesFiltrados
        );
    }
    
    private void InitializeVentasView()
    {
        // Configurar DataContext
        ventasContent.DataContext = _ventasViewModel;
        
        // Inicializar el TextBlock del cliente seleccionado
        if (_ventasViewModel.ClienteSeleccionado != null)
        {
            txtClienteSeleccionado.Text = _ventasViewModel.ClienteSeleccionado.Nombre;
        }
        else
        {
            txtClienteSeleccionado.Text = "(Sin cliente)";
        }
        
        // Enlazar propiedades
        chkEsFactura.DataContext = _ventasViewModel;
        chkEsFactura.SetBinding(CheckBox.IsCheckedProperty, new System.Windows.Data.Binding("EsFactura") { Mode = System.Windows.Data.BindingMode.TwoWay });
        
        // Cuando se marca la casilla
        chkEsFactura.Checked += (s, e) => _ventasViewModel.EsFactura = true;
        
        // Cuando se desmarca la casilla, también limpiar el cliente seleccionado
        chkEsFactura.Unchecked += (s, e) => 
        {
            _ventasViewModel.EsFactura = false;
            _ventasViewModel.ClienteSeleccionado = null;
            txtClienteSeleccionado.Text = "(Sin cliente)";
            txtStatus.Text = "Factura desmarcada, cliente eliminado";
        };
        
        // Enlazar propiedades numéricas - configurar con bindings explícitos
        var subtotalBinding = new System.Windows.Data.Binding("Subtotal")
        {
            Source = _ventasViewModel,
            StringFormat = "${0:#,##0}",
            Mode = System.Windows.Data.BindingMode.OneWay,
            UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged
        };
        txtVentaSubtotal.SetBinding(TextBlock.TextProperty, subtotalBinding);
        
        var totalBinding = new System.Windows.Data.Binding("Total")
        {
            Source = _ventasViewModel,
            StringFormat = "${0:#,##0}",
            Mode = System.Windows.Data.BindingMode.OneWay,
            UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged
        };
        txtVentaTotal.SetBinding(TextBlock.TextProperty, totalBinding);
        
        // Enlazar la propiedad DataGrid
        dgVentaItems.ItemsSource = _ventasViewModel.Items;
        
        // Forzar el cálculo inicial de totales para asegurar consistencia
        _ventasViewModel.PropertyChanged += (sender, e) => 
        {
            // Depuración para ver cuando cambian las propiedades
            if (e.PropertyName == "Subtotal" || e.PropertyName == "Total" || e.PropertyName == "Iva")
            {
                System.Diagnostics.Debug.WriteLine($"Propiedad cambiada: {e.PropertyName}, Valor: {GetPropertyValue(_ventasViewModel, e.PropertyName)}");
            }
            
            if (e.PropertyName == "ClienteSeleccionado")
            {
                if (_ventasViewModel.ClienteSeleccionado != null)
                {
                    txtClienteSeleccionado.Text = _ventasViewModel.ClienteSeleccionado.Nombre;
                }
                else
                {
                    txtClienteSeleccionado.Text = "(Sin cliente)";
                }
            }
        };
    }

    // Método auxiliar para obtener el valor de una propiedad
    private object GetPropertyValue(object obj, string propertyName)
    {
        var property = obj.GetType().GetProperty(propertyName);
        return property?.GetValue(obj, null);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            // Cargar productos
            _productManagementHelper.CargarProductos();
            
            // Cargar clientes
            _clientManagementHelper.CargarClientes();
            
            // Mensaje en la barra de estado
            txtStatus.Text = "Aplicación cargada correctamente";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar la aplicación: {ex.Message}", 
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            txtStatus.Text = $"Error: {ex.Message}";
        }
    }
    
    #region Eventos de teclado
    
    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        // Delegar el procesamiento de teclas al helper
        bool handled = _keyboardHelper.ProcessKeyDown(e.Key);
        if (handled)
        {
            e.Handled = true;
        }
    }
    
    #endregion

    #region Eventos de botones de navegación
    
    private void btnF1Ventas_Click(object sender, RoutedEventArgs e)
    {
        _navigationHelper.ShowVentasContent();
    }
    
    private void btnF2Clientes_Click(object sender, RoutedEventArgs e)
    {
        _navigationHelper.ShowClientesContent();
    }
    
    private void btnF3Productos_Click(object sender, RoutedEventArgs e)
    {
        _navigationHelper.ShowProductosContent();
    }
    
    private void btnF4Inventario_Click(object sender, RoutedEventArgs e)
    {
        // Implementar cuando el módulo de inventario esté disponible
        MessageBox.Show("El módulo de Inventario estará disponible próximamente.", "En desarrollo", MessageBoxButton.OK, MessageBoxImage.Information);
    }
    
    private void btnF5Configuracion_Click(object sender, RoutedEventArgs e)
    {
        _navigationHelper.ShowConfiguracionContent();
    }
    
    #endregion

    #region Eventos de Ventas
    
    private void txtBuscarVentaProducto_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            _productSearchHelper.BuscarProductoParaVenta();
            e.Handled = true;
        }
    }
    
    private void btnBuscarVentaProducto_Click(object sender, RoutedEventArgs e)
    {
        _productSearchHelper.BuscarProductoModal();
    }
    
    private void btnBuscarVentaCliente_Click(object sender, RoutedEventArgs e)
    {
        _clientSearchHelper.MostrarSelectorClientes();
    }
    
    private void dgVentaItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _ventasViewModel.ItemSeleccionado = dgVentaItems.SelectedItem as VentaItemViewModel;
    }
    
    private void btnEliminarVentaItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is VentaItemViewModel item)
        {
            _ventasViewModel.QuitarItemCommand.Execute(item);
        }
    }
    
    private void btnNuevaVenta_Click(object sender, RoutedEventArgs e)
    {
        _salesHelper.NuevaVenta();
    }
    
    private void btnGuardarVenta_Click(object sender, RoutedEventArgs e)
    {
        _ventasViewModel.GuardarVentaCommand.Execute(null);
    }
    
    private void btnHold_Click(object sender, RoutedEventArgs e)
    {
        _salesHelper.PonerVentaEnEspera();
    }
    
    private void btnRecuperar_Click(object sender, RoutedEventArgs e)
    {
        _salesHelper.RecuperarVentaEnEspera();
    }
    
    private void btnEmitir_Click(object sender, RoutedEventArgs e)
    {
        _salesHelper.IniciarProcesoCobro();
    }
    
    private void btnArticuloComun_Click(object sender, RoutedEventArgs e)
    {
        _salesHelper.AbrirVentanaProductoComun();
    }
    
    private void btnReimprimirTicket_Click(object sender, RoutedEventArgs e)
    {
        _salesHelper.ReimprimirUltimoTicket();
    }
    
    private void btnVentasDelDia_Click(object sender, RoutedEventArgs e)
    {
        _salesHelper.MostrarVentasDelDia();
    }
    
    #endregion

    #region Eventos de Productos
    
    private void txtBuscarProd_TextChanged(object sender, TextChangedEventArgs e)
    {
        _productManagementHelper.FiltrarProductos();
    }
    
    private void btnNuevoProd_Click(object sender, RoutedEventArgs e)
    {
        _productManagementHelper.CrearNuevoProducto();
    }
    
    private void btnEditarProd_Click(object sender, RoutedEventArgs e)
    {
        _productManagementHelper.EditarProductoSeleccionado();
    }
    
    private void btnEliminarProd_Click(object sender, RoutedEventArgs e)
    {
        _productManagementHelper.EliminarProductoSeleccionado();
    }
    
    private void btnRefrescarProd_Click(object sender, RoutedEventArgs e)
    {
        _productManagementHelper.CargarProductos();
    }
    
    private void btnImportarExcelProd_Click(object sender, RoutedEventArgs e)
    {
        _productManagementHelper.ImportarProductosDesdeExcel();
    }
    
    private void dgProductos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        _productManagementHelper.ManageDoubleClick(sender, e);
    }
    
    #endregion
    
    #region Eventos de Clientes
    
    private void txtBuscarCliente_TextChanged(object sender, TextChangedEventArgs e)
    {
        _clientManagementHelper.FiltrarClientes();
    }
    
    private void btnNuevoCliente_Click(object sender, RoutedEventArgs e)
    {
        _clientManagementHelper.CrearNuevoCliente();
    }
    
    private void btnEditarCliente_Click(object sender, RoutedEventArgs e)
    {
        _clientManagementHelper.EditarClienteSeleccionado();
    }
    
    private void btnEliminarCliente_Click(object sender, RoutedEventArgs e)
    {
        _clientManagementHelper.EliminarClienteSeleccionado();
    }
    
    private void btnRefrescarCliente_Click(object sender, RoutedEventArgs e)
    {
        _clientManagementHelper.CargarClientes();
    }
    
    private void btnImportarExcelCliente_Click(object sender, RoutedEventArgs e)
    {
        _clientManagementHelper.ImportarClientes();
    }
    
    private void dgClientes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        _clientManagementHelper.ManageDoubleClick(sender, e);
    }
    
    private void btnHistorial_Click(object sender, RoutedEventArgs e)
    {
        _clientManagementHelper.MostrarHistorialCliente(sender);
    }
    
    #endregion

    #region Eventos de Configuración
    
    private void btnOpcionesGenerales_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("La configuración de opciones generales estará disponible en futuras versiones.", 
            "Próximamente", MessageBoxButton.OK, MessageBoxImage.Information);
    }
    
    private void btnBaseDatos_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("La configuración de base de datos estará disponible en futuras versiones.", 
            "Próximamente", MessageBoxButton.OK, MessageBoxImage.Information);
    }
    
    private void btnTickets_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("La personalización de tickets estará disponible en futuras versiones.", 
            "Próximamente", MessageBoxButton.OK, MessageBoxImage.Information);
    }
    
    private void btnImpresoraTermica_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var repositoryService = new FerreteriaPOS.Data.Services.RepositoryService(_context);
            var ventanaConfiguracion = new Windows.ConfiguracionImpresoraWindow(repositoryService);
            ventanaConfiguracion.Owner = this;
            ventanaConfiguracion.ShowDialog();
            
            // Refrescar el ViewModel de ventas en caso de cambios en configuración
            if (_ventasViewModel != null)
            {
                _ventasViewModel.ConfiguracionActualizada();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al abrir la configuración de impresora: {ex.Message}", 
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    private void btnRespaldo_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("La configuración de respaldos automáticos estará disponible en futuras versiones.", 
            "Próximamente", MessageBoxButton.OK, MessageBoxImage.Information);
    }
    
    #endregion

    private void ConfirmarVenta(bool imprimir)
    {
        if (_ventasViewModel.Items.Count == 0)
        {
            MessageBox.Show("No hay productos en la venta", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        try
        {
            // Procesar la venta usando el método sin transacciones
            bool resultado = _ventasViewModel.ProcesarVentaFinal();
            
            if (resultado)
            {
                // Si se activa la impresión, aquí iría el código para imprimir el ticket
                // pero NO mostramos ningún mensaje de confirmación, ni siquiera sobre impresión
                
                // Limpiar venta actual - asegurarnos de que se limpie correctamente el panel
                _ventasViewModel.LimpiarVentaCommand.Execute(null);
                
                // Cerrar ventana de pago
                _paymentHelper.CerrarPantallaCobro();
                
                // Asegurarnos de que la vista de ventas está visible y lista
                _navigationHelper.ShowVentasContent();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al procesar la venta: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}