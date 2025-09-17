namespace EnterpriseKnowledgeHub.Models;

/// <summary>
/// Modelo que representa la cabecera de un albarán de proveedor en Sage 200.
/// 
/// EXPLICACIÓN PARA PRINCIPIANTES:
/// - Un modelo es una clase que representa los datos de una tabla de base de datos
/// - Cada propiedad corresponde a una columna de la tabla
/// - Los modelos nos ayudan a trabajar con los datos de forma tipada y segura
/// </summary>
public class CabeceraAlbaranProveedor
{
    /// <summary>
    /// Identificador único del albarán
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Número del albarán de proveedor
    /// </summary>
    public string NumeroAlbaran { get; set; } = string.Empty;

    /// <summary>
    /// Código del proveedor
    /// </summary>
    public string CodigoProveedor { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del proveedor
    /// </summary>
    public string NombreProveedor { get; set; } = string.Empty;

    /// <summary>
    /// Fecha del albarán
    /// </summary>
    public DateTime FechaAlbaran { get; set; }

    /// <summary>
    /// Fecha de recepción
    /// </summary>
    public DateTime? FechaRecepcion { get; set; }

    /// <summary>
    /// Importe total del albarán
    /// </summary>
    public decimal ImporteTotal { get; set; }

    /// <summary>
    /// Estado del albarán (Pendiente, Recibido, Facturado, etc.)
    /// </summary>
    public string Estado { get; set; } = string.Empty;

    /// <summary>
    /// Número de referencia externa
    /// </summary>
    public string? ReferenciaExterna { get; set; }

    /// <summary>
    /// Observaciones del albarán
    /// </summary>
    public string? Observaciones { get; set; }

    /// <summary>
    /// Usuario que creó el albarán
    /// </summary>
    public string UsuarioCreacion { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de creación del registro
    /// </summary>
    public DateTime FechaCreacion { get; set; }

    /// <summary>
    /// Lista de líneas que pertenecen a este albarán
    /// </summary>
    public List<LineasAlbaranProveedor> Lineas { get; set; } = new();
}