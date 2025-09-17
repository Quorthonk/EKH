using System.Windows.Controls;
using Microsoft.Extensions.Logging;
using EnterpriseKnowledgeHub.ViewModels;

namespace EnterpriseKnowledgeHub.Views;

/// <summary>
/// Vista para la interfaz de consultas inteligentes.
/// 
/// EXPLICACIÓN PARA PRINCIPIANTES:
/// - Este UserControl contiene la interfaz donde los usuarios pueden hacer consultas
/// - Se puede insertar dentro de otras ventanas o controles
/// - Maneja la interacción específica de la interfaz de consultas
/// </summary>
public partial class QueryInterfaceView : UserControl
{
    private readonly ILogger<QueryInterfaceView>? _logger;

    /// <summary>
    /// Constructor por defecto (necesario para el diseñador de Visual Studio)
    /// </summary>
    public QueryInterfaceView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Constructor con inyección de dependencias
    /// </summary>
    public QueryInterfaceView(ILogger<QueryInterfaceView> logger) : this()
    {
        _logger = logger;
        _logger?.LogDebug("QueryInterfaceView inicializada");
    }

    /// <summary>
    /// Evento que se ejecuta cuando el control se ha cargado
    /// </summary>
    private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        _logger?.LogDebug("QueryInterfaceView cargada");
        
        // Enfocamos el campo de consulta cuando se carga la vista
        if (DataContext is QueryInterfaceViewModel)
        {
            // El TextBox de consulta debería tener el foco automáticamente
            // debido a su posición en el tab order
        }
    }
}