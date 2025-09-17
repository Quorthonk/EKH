namespace EnterpriseKnowledgeHub.Models;

/// <summary>
/// Modelo que representa una línea de albarán de proveedor en Sage 200.
/// 
/// EXPLICACIÓN PARA PRINCIPIANTES:
/// - Este modelo representa los detalles de cada producto/artículo dentro de un albarán
/// - Cada albarán puede tener múltiples líneas (productos diferentes)
/// - La relación es de uno a muchos: un albarán tiene muchas líneas
/// </summary>
public class LineasAlbaranProveedor
{
    /// <summary>
    /// Identificador único de la línea
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ID del albarán al que pertenece esta línea
    /// </summary>
    public int CabeceraAlbaranId { get; set; }

    /// <summary>
    /// Número de línea dentro del albarán
    /// </summary>
    public int NumeroLinea { get; set; }

    /// <summary>
    /// Código del artículo/producto
    /// </summary>
    public string CodigoArticulo { get; set; } = string.Empty;

    /// <summary>
    /// Descripción del artículo/producto
    /// </summary>
    public string DescripcionArticulo { get; set; } = string.Empty;

    /// <summary>
    /// Cantidad recibida
    /// </summary>
    public decimal Cantidad { get; set; }

    /// <summary>
    /// Unidad de medida (Kg, Unidades, Metros, etc.)
    /// </summary>
    public string UnidadMedida { get; set; } = string.Empty;

    /// <summary>
    /// Precio unitario del artículo
    /// </summary>
    public decimal PrecioUnitario { get; set; }

    /// <summary>
    /// Descuento aplicado (porcentaje)
    /// </summary>
    public decimal Descuento { get; set; }

    /// <summary>
    /// Importe total de la línea (cantidad * precio - descuento)
    /// </summary>
    public decimal ImporteLinea { get; set; }

    /// <summary>
    /// Código del almacén donde se recibe el producto
    /// </summary>
    public string? CodigoAlmacen { get; set; }

    /// <summary>
    /// Nombre del almacén
    /// </summary>
    public string? NombreAlmacen { get; set; }

    /// <summary>
    /// Lote del producto (si aplica)
    /// </summary>
    public string? Lote { get; set; }

    /// <summary>
    /// Fecha de caducidad (si aplica)
    /// </summary>
    public DateTime? FechaCaducidad { get; set; }

    /// <summary>
    /// Número de serie (si aplica)
    /// </summary>
    public string? NumeroSerie { get; set; }

    /// <summary>
    /// Observaciones específicas de la línea
    /// </summary>
    public string? Observaciones { get; set; }

    /// <summary>
    /// Referencia a la cabecera del albarán (navegación)
    /// </summary>
    public CabeceraAlbaranProveedor? CabeceraAlbaran { get; set; }
}