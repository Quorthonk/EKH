using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using EnterpriseKnowledgeHub.Services;
using EnterpriseKnowledgeHub.Data;
using System.Collections.ObjectModel;

namespace EnterpriseKnowledgeHub.ViewModels;

/// <summary>
/// ViewModel principal de la aplicación que coordina todas las funcionalidades.
/// 
/// EXPLICACIÓN PARA PRINCIPIANTES:
/// - Un ViewModel en MVVM actúa como intermediario entre la Vista (UI) y el Modelo (datos)
/// - Contiene la lógica de presentación y maneja la interacción del usuario
/// - ObservableObject permite que la UI se actualice automáticamente cuando cambian los datos
/// - Los RelayCommand permiten enlazar acciones de botones con métodos del ViewModel
/// </summary>
public partial class MainWindowViewModel : ObservableObject
{
    private readonly IQueryService _queryService;
    private readonly IDatabaseConnectionService _connectionService;
    private readonly IOllamaService _ollamaService;
    private readonly ILogger<MainWindowViewModel> _logger;

    public MainWindowViewModel(
        IQueryService queryService,
        IDatabaseConnectionService connectionService,
        IOllamaService ollamaService,
        ILogger<MainWindowViewModel> logger)
    {
        _queryService = queryService;
        _connectionService = connectionService;
        _ollamaService = ollamaService;
        _logger = logger;

        // Inicializamos las colecciones observables
        QuickStats = new ObservableCollection<StatisticItem>();
        RecentQueries = new ObservableCollection<string>();
        SystemStatus = new ObservableObject();

        // Inicializamos la aplicación
        _ = InitializeAsync();
    }

    #region Propiedades Observables

    /// <summary>
    /// Indica si la aplicación está cargando datos
    /// </summary>
    [ObservableProperty]
    private bool isLoading = true;

    /// <summary>
    /// Mensaje de bienvenida para el usuario
    /// </summary>
    [ObservableProperty]
    private string welcomeMessage = "Bienvenido al Enterprise Knowledge Hub";

    /// <summary>
    /// Estado de conexión de la base de datos
    /// </summary>
    [ObservableProperty]
    private bool isDatabaseConnected;

    /// <summary>
    /// Estado de conexión de Ollama
    /// </summary>
    [ObservableProperty]
    private bool isOllamaConnected;

    /// <summary>
    /// Información de la base de datos conectada
    /// </summary>
    [ObservableProperty]
    private string databaseInfo = "Verificando conexión...";

    /// <summary>
    /// Información del modelo de IA
    /// </summary>
    [ObservableProperty]
    private string aiModelInfo = "Verificando modelo...";

    /// <summary>
    /// Estadísticas rápidas para mostrar en el dashboard
    /// </summary>
    public ObservableCollection<StatisticItem> QuickStats { get; }

    /// <summary>
    /// Lista de consultas recientes
    /// </summary>
    public ObservableCollection<string> RecentQueries { get; }

    /// <summary>
    /// Objeto que contiene el estado del sistema
    /// </summary>
    public ObservableObject SystemStatus { get; }

    #endregion

    #region Comandos

    /// <summary>
    /// Comando para refrescar la información del dashboard
    /// </summary>
    [RelayCommand]
    private async Task RefreshDashboard()
    {
        try
        {
            IsLoading = true;
            _logger.LogInformation("Refrescando dashboard");

            await LoadSystemStatusAsync();
            await LoadQuickStatsAsync();

            _logger.LogInformation("Dashboard refrescado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al refrescar dashboard");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Comando para navegar al explorador de base de datos
    /// </summary>
    [RelayCommand]
    private void OpenDatabaseExplorer()
    {
        // En una aplicación real, esto navegaría a otra vista
        // Por ahora, registramos la acción
        _logger.LogInformation("Abriendo explorador de base de datos");
        // TODO: Implementar navegación cuando se creen las vistas
    }

    /// <summary>
    /// Comando para navegar a la interfaz de consultas
    /// </summary>
    [RelayCommand]
    private void OpenQueryInterface()
    {
        _logger.LogInformation("Abriendo interfaz de consultas");
        // TODO: Implementar navegación cuando se creen las vistas
    }

    /// <summary>
    /// Comando para abrir la ayuda del sistema
    /// </summary>
    [RelayCommand]
    private async Task ShowHelp()
    {
        try
        {
            var helpText = await _ollamaService.AskQuestionAsync(@"
Explica brevemente qué es el Enterprise Knowledge Hub y cómo puede ayudar 
a un usuario a explorar y entender los datos de Sage 200. 
Incluye las principales funcionalidades disponibles.");

            // En una aplicación real, esto mostraría un diálogo de ayuda
            _logger.LogInformation("Mostrando ayuda: {Help}", helpText);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener ayuda");
        }
    }

    /// <summary>
    /// Comando para verificar conexiones del sistema
    /// </summary>
    [RelayCommand]
    private async Task TestConnections()
    {
        await LoadSystemStatusAsync();
    }

    #endregion

    #region Métodos Privados

    /// <summary>
    /// Inicializa la aplicación cargando todos los datos necesarios
    /// </summary>
    private async Task InitializeAsync()
    {
        try
        {
            _logger.LogInformation("Inicializando aplicación");

            // Cargamos el estado del sistema
            await LoadSystemStatusAsync();

            // Cargamos las estadísticas rápidas
            await LoadQuickStatsAsync();

            // Cargamos consultas de ejemplo
            LoadSampleQueries();

            // Actualizamos el mensaje de bienvenida
            await UpdateWelcomeMessageAsync();

            _logger.LogInformation("Aplicación inicializada exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante la inicialización");
            WelcomeMessage = "Error al inicializar la aplicación. Verifica las conexiones.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Carga el estado de las conexiones del sistema
    /// </summary>
    private async Task LoadSystemStatusAsync()
    {
        try
        {
            // Verificar conexión a base de datos
            IsDatabaseConnected = await _connectionService.TestConnectionAsync();
            
            if (IsDatabaseConnected)
            {
                var dbInfo = await _connectionService.GetDatabaseInfoAsync();
                DatabaseInfo = $"Conectado a: {dbInfo.DatabaseName} ({dbInfo.ServerName})";
            }
            else
            {
                DatabaseInfo = "Base de datos no disponible";
            }

            // Verificar conexión a Ollama
            IsOllamaConnected = await _ollamaService.IsServiceAvailableAsync();
            
            if (IsOllamaConnected)
            {
                var modelInfo = await _ollamaService.GetModelInfoAsync();
                AiModelInfo = $"Modelo activo: {modelInfo.Name} ({(modelInfo.IsLoaded ? "Cargado" : "No cargado")})";
            }
            else
            {
                AiModelInfo = "Ollama no disponible";
            }

            _logger.LogInformation("Estado del sistema actualizado - DB: {DbStatus}, AI: {AiStatus}",
                IsDatabaseConnected ? "Conectada" : "Desconectada",
                IsOllamaConnected ? "Disponible" : "No disponible");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar estado del sistema");
            DatabaseInfo = "Error al verificar conexión";
            AiModelInfo = "Error al verificar modelo";
        }
    }

    /// <summary>
    /// Carga las estadísticas rápidas para el dashboard
    /// </summary>
    private async Task LoadQuickStatsAsync()
    {
        try
        {
            QuickStats.Clear();

            if (IsDatabaseConnected)
            {
                var summary = await _queryService.GetDatabaseSummaryAsync();
                var stats = summary.Statistics;

                QuickStats.Add(new StatisticItem("Albaranes Total", stats.TotalAlbaranes.ToString("N0"), "📋"));
                QuickStats.Add(new StatisticItem("Proveedores Activos", stats.TotalProveedores.ToString("N0"), "🏢"));
                QuickStats.Add(new StatisticItem("Artículos", stats.TotalArticulos.ToString("N0"), "📦"));
                QuickStats.Add(new StatisticItem("Importe Total", stats.ImporteTotalAlbaranes.ToString("C"), "💰"));
                
                if (!string.IsNullOrEmpty(stats.ProveedorMasActivo))
                {
                    QuickStats.Add(new StatisticItem("Proveedor Más Activo", stats.ProveedorMasActivo, "⭐"));
                }
            }
            else
            {
                QuickStats.Add(new StatisticItem("Estado", "Base de datos no conectada", "❌"));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar estadísticas rápidas");
            QuickStats.Clear();
            QuickStats.Add(new StatisticItem("Error", "No se pudieron cargar las estadísticas", "⚠️"));
        }
    }

    /// <summary>
    /// Carga consultas de ejemplo para mostrar al usuario
    /// </summary>
    private void LoadSampleQueries()
    {
        RecentQueries.Clear();
        
        var sampleQueries = new[]
        {
            "¿Cuáles son los proveedores con más albaranes este mes?",
            "¿Qué artículos se han recibido recientemente?",
            "¿Cuál es el importe total de compras por proveedor?",
            "¿Qué albaranes están pendientes de recepción?",
            "¿Cuáles son los artículos con mayor volumen de compra?"
        };

        foreach (var query in sampleQueries)
        {
            RecentQueries.Add(query);
        }
    }

    /// <summary>
    /// Actualiza el mensaje de bienvenida basado en el estado del sistema
    /// </summary>
    private async Task UpdateWelcomeMessageAsync()
    {
        try
        {
            if (IsDatabaseConnected && IsOllamaConnected)
            {
                WelcomeMessage = "🎉 ¡Bienvenido al Enterprise Knowledge Hub! Todos los sistemas están operativos.";
            }
            else if (IsDatabaseConnected && !IsOllamaConnected)
            {
                WelcomeMessage = "⚠️ Base de datos conectada, pero Ollama no está disponible. Funcionalidad limitada.";
            }
            else if (!IsDatabaseConnected && IsOllamaConnected)
            {
                WelcomeMessage = "⚠️ Ollama disponible, pero no hay conexión a la base de datos.";
            }
            else
            {
                WelcomeMessage = "❌ Verifica las conexiones del sistema antes de continuar.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar mensaje de bienvenida");
            WelcomeMessage = "Error al verificar el estado del sistema.";
        }
    }

    #endregion
}

/// <summary>
/// Clase auxiliar para mostrar estadísticas en el dashboard
/// </summary>
public class StatisticItem
{
    public string Title { get; set; }
    public string Value { get; set; }
    public string Icon { get; set; }

    public StatisticItem(string title, string value, string icon)
    {
        Title = title;
        Value = value;
        Icon = icon;
    }
}