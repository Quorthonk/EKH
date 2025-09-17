using Microsoft.Data.SqlClient;

namespace EnterpriseKnowledgeHub.Data;

/// <summary>
/// Interfaz para el servicio de conexión a la base de datos.
/// 
/// EXPLICACIÓN PARA PRINCIPIANTES:
/// - Este servicio maneja las conexiones a SQL Server
/// - Centraliza la configuración de conexión en un solo lugar
/// - Facilita el testing y mantenimiento del código
/// </summary>
public interface IDatabaseConnectionService
{
    /// <summary>
    /// Obtiene una nueva conexión a la base de datos
    /// </summary>
    SqlConnection GetConnection();

    /// <summary>
    /// Verifica si la conexión a la base de datos está disponible
    /// </summary>
    Task<bool> TestConnectionAsync();

    /// <summary>
    /// Obtiene información sobre la base de datos conectada
    /// </summary>
    Task<DatabaseInfo> GetDatabaseInfoAsync();
}

/// <summary>
/// Información básica sobre la base de datos
/// </summary>
public class DatabaseInfo
{
    public string ServerName { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string ServerVersion { get; set; } = string.Empty;
    public bool IsConnected { get; set; }
    public DateTime ConnectionTime { get; set; }
}