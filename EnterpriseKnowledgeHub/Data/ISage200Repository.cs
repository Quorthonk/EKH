using EnterpriseKnowledgeHub.Models;

namespace EnterpriseKnowledgeHub.Data;

/// <summary>
/// Interfaz que define las operaciones disponibles para acceder a los datos de Sage 200.
/// 
/// EXPLICACIÓN PARA PRINCIPIANTES:
/// - Una interfaz define qué métodos debe tener una clase, pero no cómo los implementa
/// - Esto nos permite cambiar la implementación sin afectar el resto del código
/// - Es parte del patrón Repository que separa la lógica de negocio del acceso a datos
/// </summary>
public interface ISage200Repository
{
    // Operaciones para Albaranes de Proveedor
    /// <summary>
    /// Obtiene todos los albaranes de proveedores
    /// </summary>
    Task<IEnumerable<CabeceraAlbaranProveedor>> GetAlbaranesProveedorAsync();

    /// <summary>
    /// Obtiene un albarán específico por su ID
    /// </summary>
    Task<CabeceraAlbaranProveedor?> GetAlbaranProveedorByIdAsync(int id);

    /// <summary>
    /// Obtiene los albaranes de un proveedor específico
    /// </summary>
    Task<IEnumerable<CabeceraAlbaranProveedor>> GetAlbaranesByProveedorAsync(string codigoProveedor);

    /// <summary>
    /// Obtiene los albaranes en un rango de fechas
    /// </summary>
    Task<IEnumerable<CabeceraAlbaranProveedor>> GetAlbaranesByFechaAsync(DateTime fechaInicio, DateTime fechaFin);

    // Operaciones para Líneas de Albarán
    /// <summary>
    /// Obtiene las líneas de un albarán específico
    /// </summary>
    Task<IEnumerable<LineasAlbaranProveedor>> GetLineasAlbaranAsync(int cabeceraAlbaranId);

    /// <summary>
    /// Obtiene las líneas que contienen un artículo específico
    /// </summary>
    Task<IEnumerable<LineasAlbaranProveedor>> GetLineasByArticuloAsync(string codigoArticulo);

    // Operaciones para Proveedores
    /// <summary>
    /// Obtiene todos los proveedores
    /// </summary>
    Task<IEnumerable<Proveedor>> GetProveedoresAsync();

    /// <summary>
    /// Obtiene un proveedor por su código
    /// </summary>
    Task<Proveedor?> GetProveedorByCodigoAsync(string codigo);

    /// <summary>
    /// Busca proveedores por nombre
    /// </summary>
    Task<IEnumerable<Proveedor>> BuscarProveedoresByNombreAsync(string nombre);

    // Operaciones para Artículos
    /// <summary>
    /// Obtiene todos los artículos
    /// </summary>
    Task<IEnumerable<Articulo>> GetArticulosAsync();

    /// <summary>
    /// Obtiene un artículo por su código
    /// </summary>
    Task<Articulo?> GetArticuloByCodigoAsync(string codigo);

    /// <summary>
    /// Busca artículos por descripción
    /// </summary>
    Task<IEnumerable<Articulo>> BuscarArticulosByDescripcionAsync(string descripcion);

    /// <summary>
    /// Obtiene artículos por categoría
    /// </summary>
    Task<IEnumerable<Articulo>> GetArticulosByCategoriaAsync(string categoria);

    // Operaciones para consultas personalizadas
    /// <summary>
    /// Ejecuta una consulta SQL personalizada y devuelve los resultados
    /// </summary>
    Task<IEnumerable<dynamic>> ExecuteCustomQueryAsync(string query);

    /// <summary>
    /// Obtiene estadísticas generales de la base de datos
    /// </summary>
    Task<DatabaseStatistics> GetDatabaseStatisticsAsync();
}

/// <summary>
/// Clase que contiene estadísticas generales de la base de datos
/// </summary>
public class DatabaseStatistics
{
    public int TotalAlbaranes { get; set; }
    public int TotalLineasAlbaran { get; set; }
    public int TotalProveedores { get; set; }
    public int TotalArticulos { get; set; }
    public decimal ImporteTotalAlbaranes { get; set; }
    public DateTime? UltimoAlbaran { get; set; }
    public string ProveedorMasActivo { get; set; } = string.Empty;
    public string ArticuloMasMovido { get; set; } = string.Empty;
}