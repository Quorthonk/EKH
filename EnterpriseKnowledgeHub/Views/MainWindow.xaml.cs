using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using EnterpriseKnowledgeHub.ViewModels;

namespace EnterpriseKnowledgeHub.Views;

/// <summary>
/// Ventana principal de la aplicación Enterprise Knowledge Hub.
/// 
/// EXPLICACIÓN PARA PRINCIPIANTES:
/// - Esta es la ventana principal que ve el usuario al abrir la aplicación
/// - En WPF, cada ventana tiene un archivo .xaml (diseño) y un .xaml.cs (lógica)
/// - El código behind maneja eventos y lógica específica de la interfaz
/// - Usa inyección de dependencias para obtener servicios y ViewModels
/// </summary>
public partial class MainWindow : Window
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MainWindow> _logger;
    private MainWindowViewModel _viewModel;

    /// <summary>
    /// Constructor que recibe los servicios necesarios por inyección de dependencias
    /// </summary>
    public MainWindow(
        IServiceProvider serviceProvider,
        ILogger<MainWindow> logger,
        MainWindowViewModel viewModel)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _viewModel = viewModel;

        InitializeComponent();
        
        // Asignamos el ViewModel como DataContext para el data binding
        DataContext = _viewModel;
        
        _logger.LogInformation("MainWindow inicializada correctamente");
    }

    /// <summary>
    /// Evento que se ejecuta cuando la ventana se ha cargado completamente
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        _logger.LogInformation("MainWindow cargada - iniciando configuración final");
    }

    /// <summary>
    /// Muestra el dashboard principal en el área de contenido
    /// </summary>
    private void ShowDashboard_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _logger.LogInformation("Mostrando dashboard principal");
            
            // El dashboard ya está mostrándose por defecto en MainWindow.xaml
            // Aquí podríamos refrescar los datos si es necesario
            _viewModel.RefreshDashboardCommand.Execute(null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al mostrar dashboard");
            ShowErrorMessage("Error al cargar el dashboard");
        }
    }

    /// <summary>
    /// Maneja el click en una consulta de ejemplo
    /// </summary>
    private void ExampleQuery_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is Button button && button.DataContext is string query)
            {
                _logger.LogInformation("Consulta de ejemplo seleccionada: {Query}", query);
                
                // Crear y mostrar la interfaz de consultas con la consulta preseleccionada
                ShowQueryInterface(query);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al manejar consulta de ejemplo");
            ShowErrorMessage("Error al procesar la consulta de ejemplo");
        }
    }

    /// <summary>
    /// Muestra la interfaz de consultas en el área principal
    /// </summary>
    private void ShowQueryInterface(string? initialQuery = null)
    {
        try
        {
            _logger.LogInformation("Mostrando interfaz de consultas");

            // Obtenemos el ViewModel de consultas
            var queryViewModel = _serviceProvider.GetRequiredService<QueryInterfaceViewModel>();
            
            // Si hay una consulta inicial, la establecemos
            if (!string.IsNullOrEmpty(initialQuery))
            {
                queryViewModel.CurrentQuery = initialQuery;
            }

            // Creamos la vista de consultas
            var queryView = new QueryInterfaceView
            {
                DataContext = queryViewModel
            };

            // La mostramos en el área de contenido principal
            MainContentArea.Content = queryView;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al mostrar interfaz de consultas");
            ShowErrorMessage("Error al cargar la interfaz de consultas");
        }
    }

    /// <summary>
    /// Muestra el explorador de base de datos en el área principal
    /// </summary>
    private void ShowDatabaseExplorer()
    {
        try
        {
            _logger.LogInformation("Mostrando explorador de base de datos");

            // Obtenemos el ViewModel del explorador
            var explorerViewModel = _serviceProvider.GetRequiredService<DatabaseExplorerViewModel>();

            // Creamos la vista del explorador
            var explorerView = new DatabaseExplorerView
            {
                DataContext = explorerViewModel
            };

            // La mostramos en el área de contenido principal
            MainContentArea.Content = explorerView;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al mostrar explorador de base de datos");
            ShowErrorMessage("Error al cargar el explorador de base de datos");
        }
    }

    /// <summary>
    /// Muestra un mensaje de error al usuario
    /// </summary>
    private void ShowErrorMessage(string message)
    {
        try
        {
            MessageBox.Show(
                message,
                "Enterprise Knowledge Hub - Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al mostrar mensaje de error");
        }
    }

    /// <summary>
    /// Muestra un mensaje informativo al usuario
    /// </summary>
    private void ShowInfoMessage(string message)
    {
        try
        {
            MessageBox.Show(
                message,
                "Enterprise Knowledge Hub - Información",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al mostrar mensaje informativo");
        }
    }

    /// <summary>
    /// Evento que se ejecuta cuando la ventana se está cerrando
    /// </summary>
    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        try
        {
            _logger.LogInformation("Cerrando MainWindow");
            
            // Aquí podríamos guardar el estado de la aplicación si es necesario
            // O preguntar al usuario si está seguro de cerrar
            
            var result = MessageBox.Show(
                "¿Estás seguro de que quieres cerrar Enterprise Knowledge Hub?",
                "Confirmar Cierre",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
                return;
            }

            _logger.LogInformation("Aplicación cerrada por el usuario");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante el cierre de la aplicación");
        }
    }
}