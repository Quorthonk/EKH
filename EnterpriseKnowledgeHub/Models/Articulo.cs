namespace EnterpriseKnowledgeHub.Models;

/// <summary>
/// Modelo que representa un artículo/producto en Sage 200.
/// 
/// EXPLICACIÓN PARA PRINCIPIANTES:
/// - Un artículo es cualquier producto que maneja la empresa
/// - Puede ser un producto físico, un servicio, materia prima, etc.
/// - Se relaciona con las líneas de albarán porque cada línea contiene un artículo
/// </summary>
public class Articulo
{
    /// <summary>
    /// Identificador único del artículo
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Código único del artículo (SKU)
    /// </summary>
    public string Codigo { get; set; } = string.Empty;

    /// <summary>
    /// Descripción del artículo
    /// </summary>
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>
    /// Descripción extendida del artículo
    /// </summary>
    public string? DescripcionExtendida { get; set; }

    /// <summary>
    /// Categoría o familia del artículo
    /// </summary>
    public string? Categoria { get; set; }

    /// <summary>
    /// Subcategoría del artículo
    /// </summary>
    public string? Subcategoria { get; set; }

    /// <summary>
    /// Unidad de medida base (Kg, Unidades, Metros, etc.)
    /// </summary>
    public string UnidadMedida { get; set; } = string.Empty;

    /// <summary>
    /// Precio de compra actual
    /// </summary>
    public decimal PrecioCompra { get; set; }

    /// <summary>
    /// Precio de venta actual
    /// </summary>
    public decimal PrecioVenta { get; set; }

    /// <summary>
    /// Stock actual del artículo
    /// </summary>
    public decimal StockActual { get; set; }

    /// <summary>
    /// Stock mínimo recomendado
    /// </summary>
    public decimal StockMinimo { get; set; }

    /// <summary>
    /// Stock máximo recomendado
    /// </summary>
    public decimal StockMaximo { get; set; }

    /// <summary>
    /// Peso del artículo (si aplica)
    /// </summary>
    public decimal? Peso { get; set; }

    /// <summary>
    /// Volumen del artículo (si aplica)
    /// </summary>
    public decimal? Volumen { get; set; }

    /// <summary>
    /// Código de barras del artículo
    /// </summary>
    public string? CodigoBarras { get; set; }

    /// <summary>
    /// Ubicación en almacén
    /// </summary>
    public string? Ubicacion { get; set; }

    /// <summary>
    /// Indica si el artículo está activo
    /// </summary>
    public bool Activo { get; set; } = true;

    /// <summary>
    /// Indica si requiere control de lotes
    /// </summary>
    public bool ControlLotes { get; set; }

    /// <summary>
    /// Indica si requiere número de serie
    /// </summary>
    public bool NumeroSerie { get; set; }

    /// <summary>
    /// Fecha de alta del artículo
    /// </summary>
    public DateTime FechaAlta { get; set; }

    /// <summary>
    /// Observaciones del artículo
    /// </summary>
    public string? Observaciones { get; set; }
}