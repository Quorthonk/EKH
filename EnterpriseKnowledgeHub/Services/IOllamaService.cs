namespace EnterpriseKnowledgeHub.Services;

/// <summary>
/// Interfaz para el servicio de integración con Ollama.
/// 
/// EXPLICACIÓN PARA PRINCIPIANTES:
/// - Ollama es un servicio que ejecuta modelos de inteligencia artificial localmente
/// - Este servicio nos permite hacer preguntas en lenguaje natural sobre la base de datos
/// - El modelo de IA puede generar consultas SQL o explicar datos complejos
/// </summary>
public interface IOllamaService
{
    /// <summary>
    /// Envía una pregunta al modelo de IA y obtiene una respuesta
    /// </summary>
    Task<string> AskQuestionAsync(string question);

    /// <summary>
    /// Genera una consulta SQL basada en una pregunta en lenguaje natural
    /// </summary>
    Task<string> GenerateSqlQueryAsync(string naturalLanguageQuery);

    /// <summary>
    /// Explica un conjunto de datos de manera comprensible
    /// </summary>
    Task<string> ExplainDataAsync(string data, string context);

    /// <summary>
    /// Obtiene sugerencias de consultas relacionadas
    /// </summary>
    Task<IEnumerable<string>> GetQuerySuggestionsAsync(string currentQuery);

    /// <summary>
    /// Verifica si el servicio Ollama está disponible
    /// </summary>
    Task<bool> IsServiceAvailableAsync();

    /// <summary>
    /// Obtiene información sobre el modelo actual
    /// </summary>
    Task<ModelInfo> GetModelInfoAsync();
}

/// <summary>
/// Información sobre el modelo de IA activo
/// </summary>
public class ModelInfo
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public bool IsLoaded { get; set; }
    public DateTime LastUsed { get; set; }
    public string Description { get; set; } = string.Empty;
}