using Microsoft.Extensions.Options;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace EnterpriseKnowledgeHub.Data;

/// <summary>
/// Implementación del servicio de conexión a la base de datos SQL Server.
/// 
/// EXPLICACIÓN PARA PRINCIPIANTES:
/// - Esta clase implementa la interfaz IDatabaseConnectionService
/// - Se encarga de crear y gestionar las conexiones a SQL Server
/// - Usa el patrón Options para obtener la configuración de conexión
/// </summary>
public class DatabaseConnectionService : IDatabaseConnectionService
{
    private readonly string _connectionString;
    private readonly ILogger<DatabaseConnectionService> _logger;

    /// <summary>
    /// Constructor que recibe la configuración de conexión
    /// </summary>
    public DatabaseConnectionService(
        IOptions<ConnectionStrings> connectionStrings,
        ILogger<DatabaseConnectionService> logger)
    {
        _connectionString = connectionStrings.Value.Sage200Database;
        _logger = logger;
    }

    /// <summary>
    /// Crea una nueva conexión a SQL Server
    /// </summary>
    public SqlConnection GetConnection()
    {
        try
        {
            var connection = new SqlConnection(_connectionString);
            _logger.LogDebug("Nueva conexión creada a la base de datos");
            return connection;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear la conexión a la base de datos");
            throw;
        }
    }

    /// <summary>
    /// Verifica si podemos conectarnos a la base de datos
    /// </summary>
    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            _logger.LogInformation("Conexión a base de datos verificada correctamente");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar la conexión a la base de datos");
            return false;
        }
    }

    /// <summary>
    /// Obtiene información sobre la base de datos y el servidor
    /// </summary>
    public async Task<DatabaseInfo> GetDatabaseInfoAsync()
    {
        var info = new DatabaseInfo
        {
            ConnectionTime = DateTime.Now
        };

        try
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            info.IsConnected = true;
            info.ServerName = connection.DataSource;
            info.DatabaseName = connection.Database;

            // Obtenemos la versión del servidor
            using var command = new SqlCommand("SELECT @@VERSION", connection);
            var version = await command.ExecuteScalarAsync();
            info.ServerVersion = version?.ToString() ?? "Desconocida";

            _logger.LogInformation("Información de base de datos obtenida: {Server}/{Database}", 
                info.ServerName, info.DatabaseName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener información de la base de datos");
            info.IsConnected = false;
        }

        return info;
    }
}