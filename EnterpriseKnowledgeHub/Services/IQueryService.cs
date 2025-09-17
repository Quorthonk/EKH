using EnterpriseKnowledgeHub.Data;
using EnterpriseKnowledgeHub.Models;

namespace EnterpriseKnowledgeHub.Services;

/// <summary>
/// Interfaz para el servicio de consultas que combina la base de datos con IA.
/// 
/// EXPLICACIÓN PARA PRINCIPIANTES:
/// - Este servicio actúa como intermediario entre la interfaz de usuario y los datos
/// - Combina las capacidades de la base de datos con la inteligencia artificial
/// - Proporciona una manera fácil de hacer consultas complejas
/// </summary>
public interface IQueryService
{
    /// <summary>
    /// Procesa una consulta en lenguaje natural y devuelve resultados
    /// </summary>
    Task<QueryResult> ProcessNaturalLanguageQueryAsync(string query);

    /// <summary>
    /// Ejecuta una consulta SQL directa
    /// </summary>
    Task<QueryResult> ExecuteSqlQueryAsync(string sqlQuery);

    /// <summary>
    /// Obtiene explicaciones detalladas sobre entidades de la base de datos
    /// </summary>
    Task<string> ExplainDatabaseEntityAsync(string entityName);

    /// <summary>
    /// Obtiene sugerencias de consultas basadas en el contexto actual
    /// </summary>
    Task<IEnumerable<string>> GetQuerySuggestionsAsync(string context = "");

    /// <summary>
    /// Obtiene estadísticas resumidas de la base de datos con explicaciones
    /// </summary>
    Task<DatabaseSummary> GetDatabaseSummaryAsync();
}

/// <summary>
/// Resultado de una consulta con datos y explicaciones
/// </summary>
public class QueryResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public IEnumerable<dynamic> Data { get; set; } = new List<dynamic>();
    public string SqlQuery { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
    public int RecordCount { get; set; }
    public TimeSpan ExecutionTime { get; set; }
    public IEnumerable<string> Suggestions { get; set; } = new List<string>();
}

/// <summary>
/// Resumen de la base de datos con análisis inteligente
/// </summary>
public class DatabaseSummary
{
    public DatabaseStatistics Statistics { get; set; } = new();
    public string Analysis { get; set; } = string.Empty;
    public IEnumerable<string> KeyInsights { get; set; } = new List<string>();
    public IEnumerable<string> RecommendedQueries { get; set; } = new List<string>();
}