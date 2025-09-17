using Microsoft.Extensions.Logging;
using System.Diagnostics;
using EnterpriseKnowledgeHub.Data;
using Newtonsoft.Json;

namespace EnterpriseKnowledgeHub.Services;

/// <summary>
/// Implementación del servicio de consultas que combina base de datos con IA.
/// 
/// EXPLICACIÓN PARA PRINCIPIANTES:
/// - Esta clase es el cerebro del sistema que coordina todas las operaciones
/// - Toma consultas en lenguaje natural y las convierte en resultados útiles
/// - Combina datos reales de la base de datos con explicaciones de IA
/// </summary>
public class QueryService : IQueryService
{
    private readonly ISage200Repository _repository;
    private readonly IOllamaService _ollamaService;
    private readonly ILogger<QueryService> _logger;

    public QueryService(
        ISage200Repository repository,
        IOllamaService ollamaService,
        ILogger<QueryService> logger)
    {
        _repository = repository;
        _ollamaService = ollamaService;
        _logger = logger;
    }

    /// <summary>
    /// Procesa una consulta en lenguaje natural y devuelve resultados completos
    /// </summary>
    public async Task<QueryResult> ProcessNaturalLanguageQueryAsync(string query)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new QueryResult();

        try
        {
            _logger.LogInformation("Procesando consulta en lenguaje natural: {Query}", query);

            // Paso 1: Generar consulta SQL usando IA
            var sqlQuery = await _ollamaService.GenerateSqlQueryAsync(query);
            result.SqlQuery = sqlQuery;

            // Paso 2: Ejecutar la consulta en la base de datos
            if (!string.IsNullOrWhiteSpace(sqlQuery) && !sqlQuery.StartsWith("-- Error"))
            {
                try
                {
                    result.Data = await _repository.ExecuteCustomQueryAsync(sqlQuery);
                    result.RecordCount = result.Data.Count();
                    result.Success = true;

                    // Paso 3: Obtener explicación de los resultados
                    if (result.RecordCount > 0)
                    {
                        var dataForExplanation = JsonConvert.SerializeObject(
                            result.Data.Take(5), // Solo las primeras 5 filas para el análisis
                            Formatting.Indented);
                        
                        result.Explanation = await _ollamaService.ExplainDataAsync(
                            dataForExplanation, 
                            $"Consulta original: {query}");
                    }
                    else
                    {
                        result.Explanation = "No se encontraron datos que coincidan con tu consulta.";
                    }

                    // Paso 4: Obtener sugerencias relacionadas
                    result.Suggestions = await _ollamaService.GetQuerySuggestionsAsync(query);
                }
                catch (Exception dbEx)
                {
                    _logger.LogError(dbEx, "Error al ejecutar consulta SQL generada");
                    result.Success = false;
                    result.ErrorMessage = "Error al ejecutar la consulta en la base de datos: " + dbEx.Message;
                    
                    // Intentamos obtener una explicación del error
                    result.Explanation = await _ollamaService.AskQuestionAsync(
                        $"Explica por qué esta consulta SQL podría fallar: {sqlQuery}");
                }
            }
            else
            {
                result.Success = false;
                result.ErrorMessage = "No se pudo generar una consulta SQL válida";
                result.Explanation = await _ollamaService.AskQuestionAsync(query);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar consulta en lenguaje natural");
            result.Success = false;
            result.ErrorMessage = "Error interno del sistema: " + ex.Message;
            result.Explanation = "Hubo un error al procesar tu consulta. Por favor, intenta reformularla.";
        }
        finally
        {
            stopwatch.Stop();
            result.ExecutionTime = stopwatch.Elapsed;
            _logger.LogInformation("Consulta procesada en {Time}ms", stopwatch.ElapsedMilliseconds);
        }

        return result;
    }

    /// <summary>
    /// Ejecuta una consulta SQL directa con explicaciones
    /// </summary>
    public async Task<QueryResult> ExecuteSqlQueryAsync(string sqlQuery)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new QueryResult { SqlQuery = sqlQuery };

        try
        {
            _logger.LogInformation("Ejecutando consulta SQL directa");

            // Ejecutar la consulta
            result.Data = await _repository.ExecuteCustomQueryAsync(sqlQuery);
            result.RecordCount = result.Data.Count();
            result.Success = true;

            // Obtener explicación de los resultados
            if (result.RecordCount > 0)
            {
                var dataForExplanation = JsonConvert.SerializeObject(
                    result.Data.Take(5),
                    Formatting.Indented);
                
                result.Explanation = await _ollamaService.ExplainDataAsync(
                    dataForExplanation, 
                    $"Resultado de consulta SQL: {sqlQuery}");
            }
            else
            {
                result.Explanation = "La consulta se ejecutó correctamente pero no devolvió resultados.";
            }

            // Obtener sugerencias de consultas relacionadas
            result.Suggestions = await _ollamaService.GetQuerySuggestionsAsync(
                $"SQL: {sqlQuery}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar consulta SQL");
            result.Success = false;
            result.ErrorMessage = ex.Message;
            
            // Intentamos obtener ayuda de IA sobre el error
            result.Explanation = await _ollamaService.AskQuestionAsync(
                $"Explica este error de SQL: {ex.Message} en la consulta: {sqlQuery}");
        }
        finally
        {
            stopwatch.Stop();
            result.ExecutionTime = stopwatch.Elapsed;
        }

        return result;
    }

    /// <summary>
    /// Obtiene explicaciones detalladas sobre entidades de la base de datos
    /// </summary>
    public async Task<string> ExplainDatabaseEntityAsync(string entityName)
    {
        try
        {
            var explanation = entityName.ToLower() switch
            {
                "cabeceraalbaran" or "cabeceraalbaran proveedores" => 
                    await ExplainAlbaranCabeceraAsync(),
                "lineasalbaran" or "lineas albaran" or "lineas" => 
                    await ExplainAlbaranLineasAsync(),
                "proveedores" or "proveedor" => 
                    await ExplainProveedoresAsync(),
                "articulos" or "articulo" or "productos" => 
                    await ExplainArticulosAsync(),
                "sage200" or "base de datos" or "esquema" => 
                    await ExplainDatabaseSchemaAsync(),
                _ => await _ollamaService.AskQuestionAsync(
                    $"Explica qué es '{entityName}' en el contexto de una base de datos empresarial Sage 200")
            };

            return explanation;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al explicar entidad: {Entity}", entityName);
            return $"No pude obtener información sobre '{entityName}' en este momento.";
        }
    }

    /// <summary>
    /// Obtiene sugerencias de consultas basadas en el contexto
    /// </summary>
    public async Task<IEnumerable<string>> GetQuerySuggestionsAsync(string context = "")
    {
        try
        {
            if (string.IsNullOrEmpty(context))
            {
                // Sugerencias generales basadas en las estadísticas actuales
                var stats = await _repository.GetDatabaseStatisticsAsync();
                context = $"Base de datos con {stats.TotalAlbaranes} albaranes, {stats.TotalProveedores} proveedores, {stats.TotalArticulos} artículos";
            }

            return await _ollamaService.GetQuerySuggestionsAsync(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener sugerencias");
            return new[]
            {
                "¿Cuáles son los proveedores con más albaranes?",
                "¿Qué artículos se han recibido este mes?",
                "¿Cuál es el importe total de compras por proveedor?",
                "¿Qué albaranes están pendientes de recepción?",
                "¿Cuáles son los artículos más comprados?"
            };
        }
    }

    /// <summary>
    /// Obtiene un resumen completo de la base de datos con análisis inteligente
    /// </summary>
    public async Task<DatabaseSummary> GetDatabaseSummaryAsync()
    {
        var summary = new DatabaseSummary();

        try
        {
            // Obtener estadísticas básicas
            summary.Statistics = await _repository.GetDatabaseStatisticsAsync();

            // Generar análisis inteligente
            var statsContext = JsonConvert.SerializeObject(summary.Statistics, Formatting.Indented);
            summary.Analysis = await _ollamaService.ExplainDataAsync(
                statsContext,
                "Estadísticas generales de la base de datos empresarial Sage 200");

            // Obtener insights clave
            var insights = await _ollamaService.AskQuestionAsync(
                $"Basándote en estas estadísticas de Sage 200: {statsContext}, " +
                "proporciona 3-5 insights clave para la gestión empresarial. " +
                "Responde solo con los insights, uno por línea, sin numeración.");

            summary.KeyInsights = insights.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Trim())
                .Take(5);

            // Obtener consultas recomendadas
            summary.RecommendedQueries = await GetQuerySuggestionsAsync();

            _logger.LogInformation("Resumen de base de datos generado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar resumen de base de datos");
            summary.Analysis = "No se pudo generar el análisis en este momento.";
            summary.KeyInsights = new[] { "Error al obtener insights" };
            summary.RecommendedQueries = new[] { "Error al obtener recomendaciones" };
        }

        return summary;
    }

    #region Métodos privados de explicación

    private async Task<string> ExplainAlbaranCabeceraAsync()
    {
        return await _ollamaService.AskQuestionAsync(@"
Explica qué es la tabla CabeceraAlbaranProveedor en Sage 200. 
Incluye: su propósito, campos principales, relaciones con otras tablas, 
y su importancia en la gestión de compras empresarial.");
    }

    private async Task<string> ExplainAlbaranLineasAsync()
    {
        return await _ollamaService.AskQuestionAsync(@"
Explica qué es la tabla LineasAlbaranProveedor en Sage 200. 
Incluye: su propósito, campos principales, relación con CabeceraAlbaranProveedor, 
y cómo se usa para el control detallado de recepciones.");
    }

    private async Task<string> ExplainProveedoresAsync()
    {
        return await _ollamaService.AskQuestionAsync(@"
Explica qué es la tabla Proveedor en Sage 200. 
Incluye: su propósito, información que almacena, importancia en la cadena de suministro, 
y cómo se relaciona con los albaranes.");
    }

    private async Task<string> ExplainArticulosAsync()
    {
        return await _ollamaService.AskQuestionAsync(@"
Explica qué es la tabla Articulo en Sage 200. 
Incluye: su propósito, tipos de información que almacena (precios, stock, categorías), 
y su papel en la gestión de inventario.");
    }

    private async Task<string> ExplainDatabaseSchemaAsync()
    {
        return await _ollamaService.AskQuestionAsync(@"
Explica la estructura general de la base de datos Sage 200 enfocándote en:
- El módulo de compras (albaranes, proveedores)
- El módulo de inventario (artículos, stock)
- Cómo se relacionan estas entidades
- Su importancia para la gestión empresarial");
    }

    #endregion
}