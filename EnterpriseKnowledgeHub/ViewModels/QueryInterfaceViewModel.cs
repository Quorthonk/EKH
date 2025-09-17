using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using EnterpriseKnowledgeHub.Services;
using System.Collections.ObjectModel;

namespace EnterpriseKnowledgeHub.ViewModels;

/// <summary>
/// ViewModel para la interfaz de consultas con lenguaje natural y SQL.
/// 
/// EXPLICACIÓN PARA PRINCIPIANTES:
/// - Este ViewModel maneja la interfaz donde los usuarios pueden hacer preguntas
/// - Acepta tanto lenguaje natural como consultas SQL directas
/// - Integra IA para hacer el sistema más accesible para usuarios no técnicos
/// - Mantiene un historial de consultas para facilitar el aprendizaje
/// </summary>
public partial class QueryInterfaceViewModel : ObservableObject
{
    private readonly IQueryService _queryService;
    private readonly ILogger<QueryInterfaceViewModel> _logger;

    public QueryInterfaceViewModel(
        IQueryService queryService,
        ILogger<QueryInterfaceViewModel> logger)
    {
        _queryService = queryService;
        _logger = logger;

        // Inicializamos las colecciones
        QueryHistory = new ObservableCollection<QueryHistoryItem>();
        Suggestions = new ObservableCollection<string>();
        QueryResults = new ObservableCollection<dynamic>();

        // Cargamos sugerencias iniciales
        _ = LoadInitialSuggestionsAsync();
    }

    #region Propiedades Observables

    /// <summary>
    /// Consulta actual que está escribiendo el usuario
    /// </summary>
    [ObservableProperty]
    private string currentQuery = string.Empty;

    /// <summary>
    /// Indica si se está procesando una consulta
    /// </summary>
    [ObservableProperty]
    private bool isProcessing;

    /// <summary>
    /// Tipo de consulta seleccionado (Natural o SQL)
    /// </summary>
    [ObservableProperty]
    private QueryType selectedQueryType = QueryType.Natural;

    /// <summary>
    /// Resultado de la última consulta ejecutada
    /// </summary>
    [ObservableProperty]
    private QueryResult? lastQueryResult;

    /// <summary>
    /// Explicación de los resultados
    /// </summary>
    [ObservableProperty]
    private string resultExplanation = string.Empty;

    /// <summary>
    /// Consulta SQL generada (visible cuando se usa lenguaje natural)
    /// </summary>
    [ObservableProperty]
    private string generatedSql = string.Empty;

    /// <summary>
    /// Número de resultados obtenidos
    /// </summary>
    [ObservableProperty]
    private int resultCount;

    /// <summary>
    /// Tiempo de ejecución de la última consulta
    /// </summary>
    [ObservableProperty]
    private string executionTime = string.Empty;

    /// <summary>
    /// Indica si hay errores en la consulta
    /// </summary>
    [ObservableProperty]
    private bool hasError;

    /// <summary>
    /// Mensaje de error si lo hay
    /// </summary>
    [ObservableProperty]
    private string errorMessage = string.Empty;

    /// <summary>
    /// Historial de consultas realizadas
    /// </summary>
    public ObservableCollection<QueryHistoryItem> QueryHistory { get; }

    /// <summary>
    /// Sugerencias de consultas
    /// </summary>
    public ObservableCollection<string> Suggestions { get; }

    /// <summary>
    /// Resultados de la consulta actual
    /// </summary>
    public ObservableCollection<dynamic> QueryResults { get; }

    #endregion

    #region Comandos

    /// <summary>
    /// Comando para ejecutar la consulta actual
    /// </summary>
    [RelayCommand]
    private async Task ExecuteQuery()
    {
        if (string.IsNullOrWhiteSpace(CurrentQuery))
        {
            _logger.LogWarning("Intento de ejecutar consulta vacía");
            return;
        }

        try
        {
            IsProcessing = true;
            HasError = false;
            ErrorMessage = string.Empty;
            QueryResults.Clear();

            _logger.LogInformation("Ejecutando consulta: {Query} (Tipo: {Type})", 
                CurrentQuery, SelectedQueryType);

            // Ejecutamos la consulta según el tipo
            QueryResult result;
            if (SelectedQueryType == QueryType.Natural)
            {
                result = await _queryService.ProcessNaturalLanguageQueryAsync(CurrentQuery);
            }
            else
            {
                result = await _queryService.ExecuteSqlQueryAsync(CurrentQuery);
            }

            // Procesamos el resultado
            await ProcessQueryResultAsync(result);

            // Agregamos al historial
            AddToHistory(CurrentQuery, result);

            // Actualizamos sugerencias basadas en la consulta actual
            await UpdateSuggestionsAsync();

            _logger.LogInformation("Consulta ejecutada exitosamente - {Count} resultados en {Time}ms", 
                result.RecordCount, result.ExecutionTime.TotalMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar consulta");
            HandleQueryError(ex.Message);
        }
        finally
        {
            IsProcessing = false;
        }
    }

    /// <summary>
    /// Comando para limpiar la consulta actual
    /// </summary>
    [RelayCommand]
    private void ClearQuery()
    {
        CurrentQuery = string.Empty;
        HasError = false;
        ErrorMessage = string.Empty;
        QueryResults.Clear();
        GeneratedSql = string.Empty;
        ResultExplanation = string.Empty;
        ResultCount = 0;
        ExecutionTime = string.Empty;
    }

    /// <summary>
    /// Comando para usar una sugerencia como consulta
    /// </summary>
    [RelayCommand]
    private void UseSuggestion(string suggestion)
    {
        if (!string.IsNullOrWhiteSpace(suggestion))
        {
            CurrentQuery = suggestion;
            _logger.LogInformation("Sugerencia seleccionada: {Suggestion}", suggestion);
        }
    }

    /// <summary>
    /// Comando para reutilizar una consulta del historial
    /// </summary>
    [RelayCommand]
    private void ReuseHistoryQuery(QueryHistoryItem historyItem)
    {
        if (historyItem != null)
        {
            CurrentQuery = historyItem.Query;
            SelectedQueryType = historyItem.QueryType;
            _logger.LogInformation("Consulta del historial reutilizada: {Query}", historyItem.Query);
        }
    }

    /// <summary>
    /// Comando para exportar resultados
    /// </summary>
    [RelayCommand]
    private async Task ExportResults()
    {
        if (QueryResults.Any())
        {
            // TODO: Implementar exportación de resultados
            _logger.LogInformation("Exportando {Count} resultados", QueryResults.Count);
        }
    }

    /// <summary>
    /// Comando para obtener ayuda sobre consultas
    /// </summary>
    [RelayCommand]
    private async Task ShowQueryHelp()
    {
        try
        {
            var help = await _queryService.ExplainDatabaseEntityAsync("consultas");
            // TODO: Mostrar ayuda en un diálogo
            _logger.LogInformation("Mostrando ayuda de consultas");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener ayuda");
        }
    }

    /// <summary>
    /// Comando para cambiar el tipo de consulta
    /// </summary>
    [RelayCommand]
    private void ChangeQueryType(QueryType queryType)
    {
        SelectedQueryType = queryType;
        _logger.LogInformation("Tipo de consulta cambiado a: {Type}", queryType);
    }

    #endregion

    #region Métodos Privados

    /// <summary>
    /// Carga las sugerencias iniciales de consultas
    /// </summary>
    private async Task LoadInitialSuggestionsAsync()
    {
        try
        {
            var suggestions = await _queryService.GetQuerySuggestionsAsync();
            
            Suggestions.Clear();
            foreach (var suggestion in suggestions)
            {
                Suggestions.Add(suggestion);
            }

            _logger.LogInformation("Cargadas {Count} sugerencias iniciales", Suggestions.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar sugerencias iniciales");
        }
    }

    /// <summary>
    /// Procesa el resultado de una consulta
    /// </summary>
    private async Task ProcessQueryResultAsync(QueryResult result)
    {
        LastQueryResult = result;

        if (result.Success)
        {
            // Cargamos los resultados en la colección observable
            QueryResults.Clear();
            foreach (var item in result.Data)
            {
                QueryResults.Add(item);
            }

            // Actualizamos propiedades de información
            ResultCount = result.RecordCount;
            ExecutionTime = $"{result.ExecutionTime.TotalMilliseconds:F2} ms";
            ResultExplanation = result.Explanation;
            GeneratedSql = result.SqlQuery;

            // Si usamos lenguaje natural, mostramos el SQL generado
            if (SelectedQueryType == QueryType.Natural && !string.IsNullOrWhiteSpace(result.SqlQuery))
            {
                GeneratedSql = result.SqlQuery;
            }
        }
        else
        {
            HandleQueryError(result.ErrorMessage);
        }
    }

    /// <summary>
    /// Maneja errores en las consultas
    /// </summary>
    private void HandleQueryError(string errorMessage)
    {
        HasError = true;
        ErrorMessage = errorMessage;
        ResultCount = 0;
        ExecutionTime = string.Empty;
        QueryResults.Clear();
    }

    /// <summary>
    /// Agrega una consulta al historial
    /// </summary>
    private void AddToHistory(string query, QueryResult result)
    {
        var historyItem = new QueryHistoryItem
        {
            Query = query,
            QueryType = SelectedQueryType,
            ExecutedAt = DateTime.Now,
            Success = result.Success,
            RecordCount = result.RecordCount,
            ExecutionTime = result.ExecutionTime
        };

        // Agregamos al inicio del historial
        QueryHistory.Insert(0, historyItem);

        // Limitamos el historial a 50 items
        while (QueryHistory.Count > 50)
        {
            QueryHistory.RemoveAt(QueryHistory.Count - 1);
        }
    }

    /// <summary>
    /// Actualiza las sugerencias basadas en la consulta actual
    /// </summary>
    private async Task UpdateSuggestionsAsync()
    {
        try
        {
            var newSuggestions = await _queryService.GetQuerySuggestionsAsync(CurrentQuery);
            
            Suggestions.Clear();
            foreach (var suggestion in newSuggestions)
            {
                Suggestions.Add(suggestion);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar sugerencias");
        }
    }

    #endregion
}

/// <summary>
/// Tipos de consulta disponibles
/// </summary>
public enum QueryType
{
    Natural,    // Lenguaje natural
    SQL         // SQL directo
}

/// <summary>
/// Item del historial de consultas
/// </summary>
public class QueryHistoryItem
{
    public string Query { get; set; } = string.Empty;
    public QueryType QueryType { get; set; }
    public DateTime ExecutedAt { get; set; }
    public bool Success { get; set; }
    public int RecordCount { get; set; }
    public TimeSpan ExecutionTime { get; set; }

    public string DisplayText => $"{Query.Substring(0, Math.Min(50, Query.Length))}...";
    public string StatusIcon => Success ? "✅" : "❌";
    public string TypeIcon => QueryType == QueryType.Natural ? "🗣️" : "💻";
}