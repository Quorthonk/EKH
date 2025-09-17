namespace EnterpriseKnowledgeHub.Models;

/// <summary>
/// Modelo que representa un proveedor en Sage 200.
/// 
/// EXPLICACIÓN PARA PRINCIPIANTES:
/// - Un proveedor es una empresa o persona que nos suministra productos o servicios
/// - Este modelo contiene toda la información relevante del proveedor
/// - Se relaciona con los albaranes porque cada albarán pertenece a un proveedor
/// </summary>
public class Proveedor
{
    /// <summary>
    /// Identificador único del proveedor
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Código único del proveedor (usado para identificación rápida)
    /// </summary>
    public string Codigo { get; set; } = string.Empty;

    /// <summary>
    /// Nombre comercial del proveedor
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Razón social completa
    /// </summary>
    public string RazonSocial { get; set; } = string.Empty;

    /// <summary>
    /// Número de identificación fiscal (NIF/CIF)
    /// </summary>
    public string NIF { get; set; } = string.Empty;

    /// <summary>
    /// Dirección completa
    /// </summary>
    public string Direccion { get; set; } = string.Empty;

    /// <summary>
    /// Ciudad
    /// </summary>
    public string Ciudad { get; set; } = string.Empty;

    /// <summary>
    /// Código postal
    /// </summary>
    public string CodigoPostal { get; set; } = string.Empty;

    /// <summary>
    /// Provincia
    /// </summary>
    public string Provincia { get; set; } = string.Empty;

    /// <summary>
    /// País
    /// </summary>
    public string Pais { get; set; } = string.Empty;

    /// <summary>
    /// Teléfono de contacto
    /// </summary>
    public string? Telefono { get; set; }

    /// <summary>
    /// Email de contacto
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Persona de contacto
    /// </summary>
    public string? PersonaContacto { get; set; }

    /// <summary>
    /// Condiciones de pago (30 días, 60 días, etc.)
    /// </summary>
    public string? CondicionesPago { get; set; }

    /// <summary>
    /// Descuento habitual que aplica este proveedor
    /// </summary>
    public decimal DescuentoHabitual { get; set; }

    /// <summary>
    /// Indica si el proveedor está activo
    /// </summary>
    public bool Activo { get; set; } = true;

    /// <summary>
    /// Fecha de alta del proveedor
    /// </summary>
    public DateTime FechaAlta { get; set; }

    /// <summary>
    /// Observaciones generales del proveedor
    /// </summary>
    public string? Observaciones { get; set; }
}