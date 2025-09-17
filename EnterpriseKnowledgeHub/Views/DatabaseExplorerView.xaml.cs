using System.Windows.Controls;
using Microsoft.Extensions.Logging;
using EnterpriseKnowledgeHub.ViewModels;

namespace EnterpriseKnowledgeHub.Views;

/// <summary>
/// Vista para el explorador de base de datos.
/// 
/// EXPLICACIÓN PARA PRINCIPIANTES:
/// - Este UserControl contiene la interfaz para explorar la estructura y datos de la base de datos
/// - Permite navegar por tablas, ver registros y obtener explicaciones
/// - Es reutilizable y se puede insertar en diferentes partes de la aplicación
/// </summary>
public partial class DatabaseExplorerView : UserControl
{
    private readonly ILogger<DatabaseExplorerView>? _logger;

    /// <summary>
    /// Constructor por defecto (necesario para el diseñador de Visual Studio)
    /// </summary>
    public DatabaseExplorerView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Constructor con inyección de dependencias
    /// </summary>
    public DatabaseExplorerView(ILogger<DatabaseExplorerView> logger) : this()
    {
        _logger = logger;
        _logger?.LogDebug("DatabaseExplorerView inicializada");
    }

    /// <summary>
    /// Evento que se ejecuta cuando el control se ha cargado
    /// </summary>
    private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        _logger?.LogDebug("DatabaseExplorerView cargada");
    }

    /// <summary>
    /// Maneja el evento de selección de tabla
    /// </summary>
    private void TablesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox listBox && 
            listBox.SelectedItem is DatabaseTable selectedTable &&
            DataContext is DatabaseExplorerViewModel viewModel)
        {
            _logger?.LogDebug("Tabla seleccionada: {TableName}", selectedTable.Name);
            viewModel.LoadTableDataCommand.Execute(selectedTable);
        }
    }
}