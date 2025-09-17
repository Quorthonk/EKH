using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace EnterpriseKnowledgeHub.Services;

/// <summary>
/// Implementación del servicio de integración con Ollama.
/// 
/// EXPLICACIÓN PARA PRINCIPIANTES:
/// - Esta clase se conecta con el servidor Ollama para usar modelos de IA
/// - Envía peticiones HTTP al API de Ollama
/// - Procesa las respuestas y las devuelve en formato útil para la aplicación
/// </summary>
public class OllamaService : IOllamaService
{
    private readonly HttpClient _httpClient;
    private readonly OllamaSettings _settings;
    private readonly ILogger<OllamaService> _logger;

    public OllamaService(
        IOptions<OllamaSettings> settings,
        ILogger<OllamaService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
        
        // Configuramos el cliente HTTP para comunicarse con Ollama
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_settings.BaseUrl),
            Timeout = TimeSpan.FromSeconds(_settings.Timeout)
        };
    }

    /// <summary>
    /// Envía una pregunta general al modelo de IA
    /// </summary>
    public async Task<string> AskQuestionAsync(string question)
    {
        try
        {
            var prompt = $@"
Eres un asistente especializado en bases de datos Sage 200. 
Tu trabajo es ayudar a usuarios a entender y analizar información empresarial.

Contexto: Estamos trabajando con una base de datos Sage 200 que contiene:
- Albaranes de proveedor (CabeceraAlbaranProveedor)
- Líneas de albarán (LineasAlbaranProveedor)  
- Proveedores
- Artículos

Pregunta del usuario: {question}

Proporciona una respuesta clara, detallada y útil. Si la pregunta requiere datos específicos, 
explica qué tipo de consulta sería necesaria.";

            var response = await SendRequestToOllamaAsync(prompt);
            _logger.LogInformation("Pregunta procesada por Ollama");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar pregunta con Ollama");
            return "Lo siento, no pude procesar tu pregunta en este momento. Verifica que Ollama esté ejecutándose.";
        }
    }

    /// <summary>
    /// Genera una consulta SQL basada en lenguaje natural
    /// </summary>
    public async Task<string> GenerateSqlQueryAsync(string naturalLanguageQuery)
    {
        try
        {
            var prompt = $@"
Eres un experto en SQL para bases de datos Sage 200. 
Genera consultas SQL precisas basadas en peticiones en lenguaje natural.

ESQUEMA DE BASE DE DATOS:

CabeceraAlbaranProveedor:
- Id (int): Identificador único
- NumeroAlbaran (string): Número del albarán
- CodigoProveedor (string): Código del proveedor
- NombreProveedor (string): Nombre del proveedor
- FechaAlbaran (datetime): Fecha del albarán
- FechaRecepcion (datetime): Fecha de recepción
- ImporteTotal (decimal): Importe total
- Estado (string): Estado del albarán
- ReferenciaExterna (string): Referencia externa
- Observaciones (string): Observaciones
- UsuarioCreacion (string): Usuario que creó
- FechaCreacion (datetime): Fecha de creación

LineasAlbaranProveedor:
- Id (int): Identificador único
- CabeceraAlbaranId (int): ID del albarán padre
- NumeroLinea (int): Número de línea
- CodigoArticulo (string): Código del artículo
- DescripcionArticulo (string): Descripción del artículo
- Cantidad (decimal): Cantidad
- UnidadMedida (string): Unidad de medida
- PrecioUnitario (decimal): Precio unitario
- Descuento (decimal): Descuento aplicado
- ImporteLinea (decimal): Importe de la línea
- CodigoAlmacen (string): Código del almacén
- NombreAlmacen (string): Nombre del almacén
- Lote (string): Lote del producto
- FechaCaducidad (datetime): Fecha de caducidad
- NumeroSerie (string): Número de serie
- Observaciones (string): Observaciones

Proveedor:
- Id, Codigo, Nombre, RazonSocial, NIF, Direccion, Ciudad, CodigoPostal, Provincia, Pais
- Telefono, Email, PersonaContacto, CondicionesPago, DescuentoHabitual, Activo, FechaAlta, Observaciones

Articulo:
- Id, Codigo, Descripcion, DescripcionExtendida, Categoria, Subcategoria, UnidadMedida
- PrecioCompra, PrecioVenta, StockActual, StockMinimo, StockMaximo, Peso, Volumen
- CodigoBarras, Ubicacion, Activo, ControlLotes, NumeroSerie, FechaAlta, Observaciones

PETICIÓN: {naturalLanguageQuery}

Genera SOLO la consulta SQL, sin explicaciones adicionales. 
Asegúrate de que la consulta sea segura y eficiente.";

            var response = await SendRequestToOllamaAsync(prompt);
            _logger.LogInformation("Consulta SQL generada por Ollama");
            
            // Limpiamos la respuesta para obtener solo el SQL
            return CleanSqlResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar consulta SQL con Ollama");
            return "-- Error: No se pudo generar la consulta SQL";
        }
    }

    /// <summary>
    /// Explica un conjunto de datos de manera comprensible
    /// </summary>
    public async Task<string> ExplainDataAsync(string data, string context)
    {
        try
        {
            var prompt = $@"
Eres un analista de datos especializado en sistemas empresariales.
Tu trabajo es explicar datos complejos de manera simple y clara.

CONTEXTO: {context}

DATOS A EXPLICAR:
{data}

Por favor:
1. Analiza los datos proporcionados
2. Identifica patrones o tendencias importantes
3. Explica el significado de manera simple
4. Proporciona insights útiles para la toma de decisiones
5. Usa un lenguaje comprensible para usuarios no técnicos";

            var response = await SendRequestToOllamaAsync(prompt);
            _logger.LogInformation("Datos explicados por Ollama");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al explicar datos con Ollama");
            return "No pude analizar los datos en este momento.";
        }
    }

    /// <summary>
    /// Obtiene sugerencias de consultas relacionadas
    /// </summary>
    public async Task<IEnumerable<string>> GetQuerySuggestionsAsync(string currentQuery)
    {
        try
        {
            var prompt = $@"
Basándote en esta consulta: ""{currentQuery}""

Genera 5 consultas relacionadas que podrían ser de interés para un usuario.
Las consultas deben ser específicas y útiles para análisis empresarial.

Devuelve solo las consultas, una por línea, sin numeración ni explicaciones.";

            var response = await SendRequestToOllamaAsync(prompt);
            
            // Dividimos la respuesta en líneas y limpiamos
            var suggestions = response.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Trim())
                .Take(5);

            _logger.LogInformation("Sugerencias generadas por Ollama");
            return suggestions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar sugerencias con Ollama");
            return new[] { "No se pudieron generar sugerencias en este momento." };
        }
    }

    /// <summary>
    /// Verifica si el servicio Ollama está disponible
    /// </summary>
    public async Task<bool> IsServiceAvailableAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/tags");
            var isAvailable = response.IsSuccessStatusCode;
            
            _logger.LogInformation("Estado de Ollama: {Status}", 
                isAvailable ? "Disponible" : "No disponible");
            
            return isAvailable;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Ollama no está disponible");
            return false;
        }
    }

    /// <summary>
    /// Obtiene información sobre el modelo actual
    /// </summary>
    public async Task<ModelInfo> GetModelInfoAsync()
    {
        var modelInfo = new ModelInfo
        {
            Name = _settings.Model,
            LastUsed = DateTime.Now
        };

        try
        {
            // Intentamos obtener información del modelo desde Ollama
            var response = await _httpClient.GetAsync("/api/tags");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                // Aquí podríamos parsear la respuesta para obtener más detalles
                modelInfo.IsLoaded = true;
                modelInfo.Description = $"Modelo {_settings.Model} cargado en Ollama";
            }
            else
            {
                modelInfo.IsLoaded = false;
                modelInfo.Description = "Modelo no disponible";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener información del modelo");
            modelInfo.IsLoaded = false;
            modelInfo.Description = "Error al conectar con Ollama";
        }

        return modelInfo;
    }

    /// <summary>
    /// Envía una petición al API de Ollama
    /// </summary>
    private async Task<string> SendRequestToOllamaAsync(string prompt)
    {
        var requestBody = new
        {
            model = _settings.Model,
            prompt = prompt,
            stream = false
        };

        var jsonContent = JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/api/generate", content);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Error en la petición a Ollama: {response.StatusCode}");
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var responseObj = JsonConvert.DeserializeObject<dynamic>(responseContent);
        
        return responseObj?.response?.ToString() ?? "No se obtuvo respuesta del modelo.";
    }

    /// <summary>
    /// Limpia la respuesta SQL para obtener solo el código
    /// </summary>
    private static string CleanSqlResponse(string response)
    {
        // Extraemos solo el código SQL de la respuesta
        var lines = response.Split('\n');
        var sqlLines = new List<string>();
        bool inSqlBlock = false;

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            
            // Detectamos inicio de bloque SQL
            if (trimmedLine.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase) ||
                trimmedLine.StartsWith("WITH", StringComparison.OrdinalIgnoreCase))
            {
                inSqlBlock = true;
            }

            if (inSqlBlock)
            {
                sqlLines.Add(line);
                
                // Detectamos final de consulta
                if (trimmedLine.EndsWith(";") || 
                    (!trimmedLine.Contains("SELECT") && !trimmedLine.Contains("FROM") && 
                     !trimmedLine.Contains("WHERE") && !trimmedLine.Contains("GROUP") &&
                     !trimmedLine.Contains("ORDER") && !trimmedLine.Contains("HAVING") &&
                     trimmedLine.Length > 0))
                {
                    break;
                }
            }
        }

        return sqlLines.Count > 0 ? string.Join("\n", sqlLines) : response;
    }

    /// <summary>
    /// Libera los recursos del HttpClient
    /// </summary>
    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}