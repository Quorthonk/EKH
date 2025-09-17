using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using EnterpriseKnowledgeHub.Models;
using System.Data;
using Dapper;

namespace EnterpriseKnowledgeHub.Data;

/// <summary>
/// Implementación del repositorio para acceder a los datos de Sage 200.
/// 
/// EXPLICACIÓN PARA PRINCIPIANTES:
/// - Esta clase implementa todas las operaciones de acceso a datos
/// - Usa Dapper como ORM (Object-Relational Mapping) para simplificar las consultas SQL
/// - Sigue el patrón Repository para separar la lógica de negocio del acceso a datos
/// - Todas las operaciones son asíncronas para mejor rendimiento
/// </summary>
public class Sage200Repository : ISage200Repository
{
    private readonly IDatabaseConnectionService _connectionService;
    private readonly ILogger<Sage200Repository> _logger;

    public Sage200Repository(
        IDatabaseConnectionService connectionService,
        ILogger<Sage200Repository> logger)
    {
        _connectionService = connectionService;
        _logger = logger;
    }

    #region Operaciones de Albaranes de Proveedor

    /// <summary>
    /// Obtiene todos los albaranes de proveedores con sus datos básicos
    /// </summary>
    public async Task<IEnumerable<CabeceraAlbaranProveedor>> GetAlbaranesProveedorAsync()
    {
        const string sql = @"
            SELECT 
                Id,
                NumeroAlbaran,
                CodigoProveedor,
                NombreProveedor,
                FechaAlbaran,
                FechaRecepcion,
                ImporteTotal,
                Estado,
                ReferenciaExterna,
                Observaciones,
                UsuarioCreacion,
                FechaCreacion
            FROM CabeceraAlbaranProveedor 
            ORDER BY FechaAlbaran DESC";

        try
        {
            using var connection = _connectionService.GetConnection();
            var albaranes = await connection.QueryAsync<CabeceraAlbaranProveedor>(sql);
            _logger.LogInformation("Obtenidos {Count} albaranes de proveedor", albaranes.Count());
            return albaranes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener albaranes de proveedor");
            throw;
        }
    }

    /// <summary>
    /// Obtiene un albarán específico por su ID incluyendo sus líneas
    /// </summary>
    public async Task<CabeceraAlbaranProveedor?> GetAlbaranProveedorByIdAsync(int id)
    {
        const string sqlCabecera = @"
            SELECT 
                Id, NumeroAlbaran, CodigoProveedor, NombreProveedor,
                FechaAlbaran, FechaRecepcion, ImporteTotal, Estado,
                ReferenciaExterna, Observaciones, UsuarioCreacion, FechaCreacion
            FROM CabeceraAlbaranProveedor 
            WHERE Id = @Id";

        try
        {
            using var connection = _connectionService.GetConnection();
            
            var cabecera = await connection.QuerySingleOrDefaultAsync<CabeceraAlbaranProveedor>(
                sqlCabecera, new { Id = id });

            if (cabecera != null)
            {
                // Cargamos las líneas del albarán
                cabecera.Lineas = (await GetLineasAlbaranAsync(id)).ToList();
            }

            _logger.LogInformation("Obtenido albarán {Id}", id);
            return cabecera;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener albarán {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Obtiene los albaranes de un proveedor específico
    /// </summary>
    public async Task<IEnumerable<CabeceraAlbaranProveedor>> GetAlbaranesByProveedorAsync(string codigoProveedor)
    {
        const string sql = @"
            SELECT 
                Id, NumeroAlbaran, CodigoProveedor, NombreProveedor,
                FechaAlbaran, FechaRecepcion, ImporteTotal, Estado,
                ReferenciaExterna, Observaciones, UsuarioCreacion, FechaCreacion
            FROM CabeceraAlbaranProveedor 
            WHERE CodigoProveedor = @CodigoProveedor
            ORDER BY FechaAlbaran DESC";

        try
        {
            using var connection = _connectionService.GetConnection();
            var albaranes = await connection.QueryAsync<CabeceraAlbaranProveedor>(
                sql, new { CodigoProveedor = codigoProveedor });
            
            _logger.LogInformation("Obtenidos {Count} albaranes para proveedor {Codigo}", 
                albaranes.Count(), codigoProveedor);
            return albaranes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener albaranes del proveedor {Codigo}", codigoProveedor);
            throw;
        }
    }

    /// <summary>
    /// Obtiene los albaranes en un rango de fechas
    /// </summary>
    public async Task<IEnumerable<CabeceraAlbaranProveedor>> GetAlbaranesByFechaAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        const string sql = @"
            SELECT 
                Id, NumeroAlbaran, CodigoProveedor, NombreProveedor,
                FechaAlbaran, FechaRecepcion, ImporteTotal, Estado,
                ReferenciaExterna, Observaciones, UsuarioCreacion, FechaCreacion
            FROM CabeceraAlbaranProveedor 
            WHERE FechaAlbaran BETWEEN @FechaInicio AND @FechaFin
            ORDER BY FechaAlbaran DESC";

        try
        {
            using var connection = _connectionService.GetConnection();
            var albaranes = await connection.QueryAsync<CabeceraAlbaranProveedor>(
                sql, new { FechaInicio = fechaInicio, FechaFin = fechaFin });
            
            _logger.LogInformation("Obtenidos {Count} albaranes entre {Inicio} y {Fin}", 
                albaranes.Count(), fechaInicio.ToShortDateString(), fechaFin.ToShortDateString());
            return albaranes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener albaranes por fecha");
            throw;
        }
    }

    #endregion

    #region Operaciones de Líneas de Albarán

    /// <summary>
    /// Obtiene todas las líneas de un albarán específico
    /// </summary>
    public async Task<IEnumerable<LineasAlbaranProveedor>> GetLineasAlbaranAsync(int cabeceraAlbaranId)
    {
        const string sql = @"
            SELECT 
                Id, CabeceraAlbaranId, NumeroLinea, CodigoArticulo, DescripcionArticulo,
                Cantidad, UnidadMedida, PrecioUnitario, Descuento, ImporteLinea,
                CodigoAlmacen, NombreAlmacen, Lote, FechaCaducidad, NumeroSerie, Observaciones
            FROM LineasAlbaranProveedor 
            WHERE CabeceraAlbaranId = @CabeceraAlbaranId
            ORDER BY NumeroLinea";

        try
        {
            using var connection = _connectionService.GetConnection();
            var lineas = await connection.QueryAsync<LineasAlbaranProveedor>(
                sql, new { CabeceraAlbaranId = cabeceraAlbaranId });
            
            _logger.LogInformation("Obtenidas {Count} líneas para albarán {Id}", 
                lineas.Count(), cabeceraAlbaranId);
            return lineas;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener líneas del albarán {Id}", cabeceraAlbaranId);
            throw;
        }
    }

    /// <summary>
    /// Obtiene las líneas que contienen un artículo específico
    /// </summary>
    public async Task<IEnumerable<LineasAlbaranProveedor>> GetLineasByArticuloAsync(string codigoArticulo)
    {
        const string sql = @"
            SELECT 
                l.Id, l.CabeceraAlbaranId, l.NumeroLinea, l.CodigoArticulo, l.DescripcionArticulo,
                l.Cantidad, l.UnidadMedida, l.PrecioUnitario, l.Descuento, l.ImporteLinea,
                l.CodigoAlmacen, l.NombreAlmacen, l.Lote, l.FechaCaducidad, l.NumeroSerie, l.Observaciones
            FROM LineasAlbaranProveedor l
            INNER JOIN CabeceraAlbaranProveedor c ON l.CabeceraAlbaranId = c.Id
            WHERE l.CodigoArticulo = @CodigoArticulo
            ORDER BY c.FechaAlbaran DESC, l.NumeroLinea";

        try
        {
            using var connection = _connectionService.GetConnection();
            var lineas = await connection.QueryAsync<LineasAlbaranProveedor>(
                sql, new { CodigoArticulo = codigoArticulo });
            
            _logger.LogInformation("Obtenidas {Count} líneas para artículo {Codigo}", 
                lineas.Count(), codigoArticulo);
            return lineas;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener líneas del artículo {Codigo}", codigoArticulo);
            throw;
        }
    }

    #endregion

    #region Operaciones de Proveedores

    /// <summary>
    /// Obtiene todos los proveedores activos
    /// </summary>
    public async Task<IEnumerable<Proveedor>> GetProveedoresAsync()
    {
        const string sql = @"
            SELECT 
                Id, Codigo, Nombre, RazonSocial, NIF, Direccion, Ciudad, CodigoPostal,
                Provincia, Pais, Telefono, Email, PersonaContacto, CondicionesPago,
                DescuentoHabitual, Activo, FechaAlta, Observaciones
            FROM Proveedor 
            WHERE Activo = 1
            ORDER BY Nombre";

        try
        {
            using var connection = _connectionService.GetConnection();
            var proveedores = await connection.QueryAsync<Proveedor>(sql);
            _logger.LogInformation("Obtenidos {Count} proveedores", proveedores.Count());
            return proveedores;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener proveedores");
            throw;
        }
    }

    /// <summary>
    /// Obtiene un proveedor por su código
    /// </summary>
    public async Task<Proveedor?> GetProveedorByCodigoAsync(string codigo)
    {
        const string sql = @"
            SELECT 
                Id, Codigo, Nombre, RazonSocial, NIF, Direccion, Ciudad, CodigoPostal,
                Provincia, Pais, Telefono, Email, PersonaContacto, CondicionesPago,
                DescuentoHabitual, Activo, FechaAlta, Observaciones
            FROM Proveedor 
            WHERE Codigo = @Codigo";

        try
        {
            using var connection = _connectionService.GetConnection();
            var proveedor = await connection.QuerySingleOrDefaultAsync<Proveedor>(
                sql, new { Codigo = codigo });
            
            _logger.LogInformation("Obtenido proveedor {Codigo}", codigo);
            return proveedor;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener proveedor {Codigo}", codigo);
            throw;
        }
    }

    /// <summary>
    /// Busca proveedores por nombre (búsqueda parcial)
    /// </summary>
    public async Task<IEnumerable<Proveedor>> BuscarProveedoresByNombreAsync(string nombre)
    {
        const string sql = @"
            SELECT 
                Id, Codigo, Nombre, RazonSocial, NIF, Direccion, Ciudad, CodigoPostal,
                Provincia, Pais, Telefono, Email, PersonaContacto, CondicionesPago,
                DescuentoHabitual, Activo, FechaAlta, Observaciones
            FROM Proveedor 
            WHERE Nombre LIKE @Nombre AND Activo = 1
            ORDER BY Nombre";

        try
        {
            using var connection = _connectionService.GetConnection();
            var proveedores = await connection.QueryAsync<Proveedor>(
                sql, new { Nombre = $"%{nombre}%" });
            
            _logger.LogInformation("Encontrados {Count} proveedores con nombre '{Nombre}'", 
                proveedores.Count(), nombre);
            return proveedores;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar proveedores por nombre '{Nombre}'", nombre);
            throw;
        }
    }

    #endregion

    #region Operaciones de Artículos

    /// <summary>
    /// Obtiene todos los artículos activos
    /// </summary>
    public async Task<IEnumerable<Articulo>> GetArticulosAsync()
    {
        const string sql = @"
            SELECT 
                Id, Codigo, Descripcion, DescripcionExtendida, Categoria, Subcategoria,
                UnidadMedida, PrecioCompra, PrecioVenta, StockActual, StockMinimo, StockMaximo,
                Peso, Volumen, CodigoBarras, Ubicacion, Activo, ControlLotes, NumeroSerie,
                FechaAlta, Observaciones
            FROM Articulo 
            WHERE Activo = 1
            ORDER BY Descripcion";

        try
        {
            using var connection = _connectionService.GetConnection();
            var articulos = await connection.QueryAsync<Articulo>(sql);
            _logger.LogInformation("Obtenidos {Count} artículos", articulos.Count());
            return articulos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener artículos");
            throw;
        }
    }

    /// <summary>
    /// Obtiene un artículo por su código
    /// </summary>
    public async Task<Articulo?> GetArticuloByCodigoAsync(string codigo)
    {
        const string sql = @"
            SELECT 
                Id, Codigo, Descripcion, DescripcionExtendida, Categoria, Subcategoria,
                UnidadMedida, PrecioCompra, PrecioVenta, StockActual, StockMinimo, StockMaximo,
                Peso, Volumen, CodigoBarras, Ubicacion, Activo, ControlLotes, NumeroSerie,
                FechaAlta, Observaciones
            FROM Articulo 
            WHERE Codigo = @Codigo";

        try
        {
            using var connection = _connectionService.GetConnection();
            var articulo = await connection.QuerySingleOrDefaultAsync<Articulo>(
                sql, new { Codigo = codigo });
            
            _logger.LogInformation("Obtenido artículo {Codigo}", codigo);
            return articulo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener artículo {Codigo}", codigo);
            throw;
        }
    }

    /// <summary>
    /// Busca artículos por descripción (búsqueda parcial)
    /// </summary>
    public async Task<IEnumerable<Articulo>> BuscarArticulosByDescripcionAsync(string descripcion)
    {
        const string sql = @"
            SELECT 
                Id, Codigo, Descripcion, DescripcionExtendida, Categoria, Subcategoria,
                UnidadMedida, PrecioCompra, PrecioVenta, StockActual, StockMinimo, StockMaximo,
                Peso, Volumen, CodigoBarras, Ubicacion, Activo, ControlLotes, NumeroSerie,
                FechaAlta, Observaciones
            FROM Articulo 
            WHERE (Descripcion LIKE @Descripcion OR DescripcionExtendida LIKE @Descripcion) 
                AND Activo = 1
            ORDER BY Descripcion";

        try
        {
            using var connection = _connectionService.GetConnection();
            var articulos = await connection.QueryAsync<Articulo>(
                sql, new { Descripcion = $"%{descripcion}%" });
            
            _logger.LogInformation("Encontrados {Count} artículos con descripción '{Descripcion}'", 
                articulos.Count(), descripcion);
            return articulos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar artículos por descripción '{Descripcion}'", descripcion);
            throw;
        }
    }

    /// <summary>
    /// Obtiene artículos por categoría
    /// </summary>
    public async Task<IEnumerable<Articulo>> GetArticulosByCategoriaAsync(string categoria)
    {
        const string sql = @"
            SELECT 
                Id, Codigo, Descripcion, DescripcionExtendida, Categoria, Subcategoria,
                UnidadMedida, PrecioCompra, PrecioVenta, StockActual, StockMinimo, StockMaximo,
                Peso, Volumen, CodigoBarras, Ubicacion, Activo, ControlLotes, NumeroSerie,
                FechaAlta, Observaciones
            FROM Articulo 
            WHERE Categoria = @Categoria AND Activo = 1
            ORDER BY Descripcion";

        try
        {
            using var connection = _connectionService.GetConnection();
            var articulos = await connection.QueryAsync<Articulo>(
                sql, new { Categoria = categoria });
            
            _logger.LogInformation("Obtenidos {Count} artículos de categoría '{Categoria}'", 
                articulos.Count(), categoria);
            return articulos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener artículos de categoría '{Categoria}'", categoria);
            throw;
        }
    }

    #endregion

    #region Operaciones personalizadas

    /// <summary>
    /// Ejecuta una consulta SQL personalizada
    /// NOTA: Esta función debe usarse con cuidado por motivos de seguridad
    /// </summary>
    public async Task<IEnumerable<dynamic>> ExecuteCustomQueryAsync(string query)
    {
        try
        {
            using var connection = _connectionService.GetConnection();
            var results = await connection.QueryAsync(query);
            _logger.LogInformation("Ejecutada consulta personalizada");
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar consulta personalizada: {Query}", query);
            throw;
        }
    }

    /// <summary>
    /// Obtiene estadísticas generales de la base de datos
    /// </summary>
    public async Task<DatabaseStatistics> GetDatabaseStatisticsAsync()
    {
        const string sql = @"
            SELECT 
                (SELECT COUNT(*) FROM CabeceraAlbaranProveedor) as TotalAlbaranes,
                (SELECT COUNT(*) FROM LineasAlbaranProveedor) as TotalLineasAlbaran,
                (SELECT COUNT(*) FROM Proveedor WHERE Activo = 1) as TotalProveedores,
                (SELECT COUNT(*) FROM Articulo WHERE Activo = 1) as TotalArticulos,
                (SELECT ISNULL(SUM(ImporteTotal), 0) FROM CabeceraAlbaranProveedor) as ImporteTotalAlbaranes,
                (SELECT MAX(FechaAlbaran) FROM CabeceraAlbaranProveedor) as UltimoAlbaran,
                (SELECT TOP 1 NombreProveedor FROM CabeceraAlbaranProveedor 
                 GROUP BY CodigoProveedor, NombreProveedor 
                 ORDER BY COUNT(*) DESC) as ProveedorMasActivo,
                (SELECT TOP 1 DescripcionArticulo FROM LineasAlbaranProveedor 
                 GROUP BY CodigoArticulo, DescripcionArticulo 
                 ORDER BY SUM(Cantidad) DESC) as ArticuloMasMovido";

        try
        {
            using var connection = _connectionService.GetConnection();
            var stats = await connection.QuerySingleAsync<DatabaseStatistics>(sql);
            _logger.LogInformation("Obtenidas estadísticas de la base de datos");
            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estadísticas de la base de datos");
            throw;
        }
    }

    #endregion
}