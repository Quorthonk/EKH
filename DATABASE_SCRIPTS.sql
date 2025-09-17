# Scripts de Base de Datos para Sage 200

Este archivo contiene scripts SQL para crear las tablas y datos de ejemplo necesarios para el Enterprise Knowledge Hub.

## Creación de Tablas

### Tabla: CabeceraAlbaranProveedor
```sql
-- Tabla principal que almacena la información de cabecera de los albaranes de proveedor
CREATE TABLE CabeceraAlbaranProveedor (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NumeroAlbaran NVARCHAR(50) NOT NULL UNIQUE,
    CodigoProveedor NVARCHAR(20) NOT NULL,
    NombreProveedor NVARCHAR(200) NOT NULL,
    FechaAlbaran DATETIME NOT NULL,
    FechaRecepcion DATETIME NULL,
    ImporteTotal DECIMAL(18,2) NOT NULL DEFAULT 0,
    Estado NVARCHAR(50) NOT NULL DEFAULT 'Pendiente',
    ReferenciaExterna NVARCHAR(100) NULL,
    Observaciones NVARCHAR(500) NULL,
    UsuarioCreacion NVARCHAR(50) NOT NULL,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    
    -- Índices para mejorar el rendimiento
    INDEX IX_CabeceraAlbaran_Proveedor (CodigoProveedor),
    INDEX IX_CabeceraAlbaran_Fecha (FechaAlbaran),
    INDEX IX_CabeceraAlbaran_Estado (Estado)
);
```

### Tabla: LineasAlbaranProveedor
```sql
-- Tabla que almacena las líneas de detalle de cada albarán
CREATE TABLE LineasAlbaranProveedor (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CabeceraAlbaranId INT NOT NULL,
    NumeroLinea INT NOT NULL,
    CodigoArticulo NVARCHAR(30) NOT NULL,
    DescripcionArticulo NVARCHAR(250) NOT NULL,
    Cantidad DECIMAL(18,4) NOT NULL,
    UnidadMedida NVARCHAR(10) NOT NULL,
    PrecioUnitario DECIMAL(18,4) NOT NULL,
    Descuento DECIMAL(5,2) NOT NULL DEFAULT 0,
    ImporteLinea DECIMAL(18,2) NOT NULL,
    CodigoAlmacen NVARCHAR(20) NULL,
    NombreAlmacen NVARCHAR(100) NULL,
    Lote NVARCHAR(50) NULL,
    FechaCaducidad DATETIME NULL,
    NumeroSerie NVARCHAR(100) NULL,
    Observaciones NVARCHAR(300) NULL,
    
    -- Relación con la cabecera
    FOREIGN KEY (CabeceraAlbaranId) REFERENCES CabeceraAlbaranProveedor(Id) ON DELETE CASCADE,
    
    -- Índices
    INDEX IX_LineasAlbaran_Cabecera (CabeceraAlbaranId),
    INDEX IX_LineasAlbaran_Articulo (CodigoArticulo),
    INDEX IX_LineasAlbaran_Almacen (CodigoAlmacen)
);
```

### Tabla: Proveedor
```sql
-- Catálogo de proveedores
CREATE TABLE Proveedor (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Codigo NVARCHAR(20) NOT NULL UNIQUE,
    Nombre NVARCHAR(200) NOT NULL,
    RazonSocial NVARCHAR(250) NOT NULL,
    NIF NVARCHAR(20) NOT NULL,
    Direccion NVARCHAR(300) NULL,
    Ciudad NVARCHAR(100) NULL,
    CodigoPostal NVARCHAR(10) NULL,
    Provincia NVARCHAR(100) NULL,
    Pais NVARCHAR(100) NULL DEFAULT 'España',
    Telefono NVARCHAR(20) NULL,
    Email NVARCHAR(150) NULL,
    PersonaContacto NVARCHAR(150) NULL,
    CondicionesPago NVARCHAR(100) NULL,
    DescuentoHabitual DECIMAL(5,2) NOT NULL DEFAULT 0,
    Activo BIT NOT NULL DEFAULT 1,
    FechaAlta DATETIME NOT NULL DEFAULT GETDATE(),
    Observaciones NVARCHAR(500) NULL,
    
    -- Índices
    INDEX IX_Proveedor_Nombre (Nombre),
    INDEX IX_Proveedor_NIF (NIF),
    INDEX IX_Proveedor_Activo (Activo)
);
```

### Tabla: Articulo
```sql
-- Catálogo de artículos/productos
CREATE TABLE Articulo (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Codigo NVARCHAR(30) NOT NULL UNIQUE,
    Descripcion NVARCHAR(250) NOT NULL,
    DescripcionExtendida NVARCHAR(500) NULL,
    Categoria NVARCHAR(100) NULL,
    Subcategoria NVARCHAR(100) NULL,
    UnidadMedida NVARCHAR(10) NOT NULL,
    PrecioCompra DECIMAL(18,4) NOT NULL DEFAULT 0,
    PrecioVenta DECIMAL(18,4) NOT NULL DEFAULT 0,
    StockActual DECIMAL(18,4) NOT NULL DEFAULT 0,
    StockMinimo DECIMAL(18,4) NOT NULL DEFAULT 0,
    StockMaximo DECIMAL(18,4) NOT NULL DEFAULT 0,
    Peso DECIMAL(18,4) NULL,
    Volumen DECIMAL(18,4) NULL,
    CodigoBarras NVARCHAR(50) NULL,
    Ubicacion NVARCHAR(50) NULL,
    Activo BIT NOT NULL DEFAULT 1,
    ControlLotes BIT NOT NULL DEFAULT 0,
    NumeroSerie BIT NOT NULL DEFAULT 0,
    FechaAlta DATETIME NOT NULL DEFAULT GETDATE(),
    Observaciones NVARCHAR(500) NULL,
    
    -- Índices
    INDEX IX_Articulo_Descripcion (Descripcion),
    INDEX IX_Articulo_Categoria (Categoria),
    INDEX IX_Articulo_CodigoBarras (CodigoBarras),
    INDEX IX_Articulo_Activo (Activo)
);
```

## Datos de Ejemplo

### Proveedores de Ejemplo
```sql
INSERT INTO Proveedor (Codigo, Nombre, RazonSocial, NIF, Ciudad, Telefono, Email, CondicionesPago) VALUES
('PROV001', 'Suministros Industriales SA', 'Suministros Industriales Sociedad Anónima', 'A12345678', 'Madrid', '91-555-0001', 'ventas@suministros.com', '30 días'),
('PROV002', 'Materiales del Norte SL', 'Materiales del Norte Sociedad Limitada', 'B23456789', 'Bilbao', '94-555-0002', 'pedidos@materialnorte.com', '60 días'),
('PROV003', 'Distribuciones Levante', 'Distribuciones Levante SL', 'B34567890', 'Valencia', '96-555-0003', 'comercial@distlevante.es', '45 días'),
('PROV004', 'Proveedora Catalana', 'Proveedora Catalana SA', 'A45678901', 'Barcelona', '93-555-0004', 'info@provcatalana.cat', '30 días'),
('PROV005', 'Almacenes del Sur', 'Almacenes del Sur Andaluz SL', 'B56789012', 'Sevilla', '95-555-0005', 'ventas@almacensur.es', '90 días');
```

### Artículos de Ejemplo
```sql
INSERT INTO Articulo (Codigo, Descripcion, Categoria, UnidadMedida, PrecioCompra, PrecioVenta, StockActual, StockMinimo) VALUES
('ART001', 'Tornillo M8x20 Inoxidable', 'Ferretería', 'Ud', 0.15, 0.25, 500, 100),
('ART002', 'Aceite Hidráulico ISO 46', 'Lubricantes', 'L', 3.50, 5.20, 200, 50),
('ART003', 'Cable Eléctrico 2.5mm', 'Electricidad', 'M', 1.25, 2.10, 1000, 200),
('ART004', 'Válvula de Bola 1/2"', 'Fontanería', 'Ud', 8.75, 15.60, 25, 10),
('ART005', 'Filtro de Aire Industrial', 'Filtros', 'Ud', 12.30, 22.50, 75, 15),
('ART006', 'Rodamiento 6206-2RS', 'Rodamientos', 'Ud', 6.80, 12.40, 40, 8),
('ART007', 'Correa Trapezoidal A38', 'Transmisión', 'Ud', 4.25, 7.90, 60, 12),
('ART008', 'Junta Tórica 30x3', 'Juntas', 'Ud', 0.85, 1.50, 200, 40),
('ART009', 'Soldadura Eléctrica 3.2mm', 'Soldadura', 'Kg', 2.80, 4.75, 150, 30),
('ART010', 'Disco de Corte 230mm', 'Herramientas', 'Ud', 3.45, 6.20, 80, 20);
```

### Albaranes de Ejemplo
```sql
-- Insertar albaranes de cabecera
INSERT INTO CabeceraAlbaranProveedor (NumeroAlbaran, CodigoProveedor, NombreProveedor, FechaAlbaran, ImporteTotal, Estado, UsuarioCreacion) VALUES
('ALB202401001', 'PROV001', 'Suministros Industriales SA', '2024-01-15', 850.75, 'Recibido', 'admin'),
('ALB202401002', 'PROV002', 'Materiales del Norte SL', '2024-01-18', 1250.30, 'Recibido', 'compras'),
('ALB202401003', 'PROV003', 'Distribuciones Levante', '2024-01-22', 675.90, 'Pendiente', 'admin'),
('ALB202401004', 'PROV001', 'Suministros Industriales SA', '2024-01-25', 425.60, 'Recibido', 'compras'),
('ALB202401005', 'PROV004', 'Proveedora Catalana', '2024-01-28', 980.45, 'Facturado', 'admin');

-- Insertar líneas de albarán
INSERT INTO LineasAlbaranProveedor (CabeceraAlbaranId, NumeroLinea, CodigoArticulo, DescripcionArticulo, Cantidad, UnidadMedida, PrecioUnitario, ImporteLinea, CodigoAlmacen, NombreAlmacen) VALUES
-- Albarán ALB202401001
(1, 1, 'ART001', 'Tornillo M8x20 Inoxidable', 100, 'Ud', 0.15, 15.00, 'ALM001', 'Almacén Central'),
(1, 2, 'ART003', 'Cable Eléctrico 2.5mm', 500, 'M', 1.25, 625.00, 'ALM001', 'Almacén Central'),
(1, 3, 'ART008', 'Junta Tórica 30x3', 50, 'Ud', 0.85, 42.50, 'ALM001', 'Almacén Central'),

-- Albarán ALB202401002
(2, 1, 'ART002', 'Aceite Hidráulico ISO 46', 80, 'L', 3.50, 280.00, 'ALM002', 'Almacén Norte'),
(2, 2, 'ART004', 'Válvula de Bola 1/2"', 15, 'Ud', 8.75, 131.25, 'ALM002', 'Almacén Norte'),
(2, 3, 'ART006', 'Rodamiento 6206-2RS', 20, 'Ud', 6.80, 136.00, 'ALM002', 'Almacén Norte'),

-- Albarán ALB202401003
(3, 1, 'ART005', 'Filtro de Aire Industrial', 25, 'Ud', 12.30, 307.50, 'ALM001', 'Almacén Central'),
(3, 2, 'ART007', 'Correa Trapezoidal A38', 30, 'Ud', 4.25, 127.50, 'ALM001', 'Almacén Central'),
(3, 3, 'ART010', 'Disco de Corte 230mm', 40, 'Ud', 3.45, 138.00, 'ALM001', 'Almacén Central');
```

## Consultas de Ejemplo para Testing

### Estadísticas Generales
```sql
-- Obtener estadísticas generales de la base de datos
SELECT 
    (SELECT COUNT(*) FROM CabeceraAlbaranProveedor) as TotalAlbaranes,
    (SELECT COUNT(*) FROM LineasAlbaranProveedor) as TotalLineasAlbaran,
    (SELECT COUNT(*) FROM Proveedor WHERE Activo = 1) as TotalProveedores,
    (SELECT COUNT(*) FROM Articulo WHERE Activo = 1) as TotalArticulos,
    (SELECT ISNULL(SUM(ImporteTotal), 0) FROM CabeceraAlbaranProveedor) as ImporteTotalAlbaranes,
    (SELECT MAX(FechaAlbaran) FROM CabeceraAlbaranProveedor) as UltimoAlbaran;
```

### Proveedores Más Activos
```sql
-- Top 5 proveedores con más albaranes
SELECT TOP 5 
    c.CodigoProveedor,
    c.NombreProveedor,
    COUNT(*) as TotalAlbaranes,
    SUM(c.ImporteTotal) as ImporteTotal
FROM CabeceraAlbaranProveedor c
GROUP BY c.CodigoProveedor, c.NombreProveedor
ORDER BY TotalAlbaranes DESC;
```

### Artículos Más Comprados
```sql
-- Top 10 artículos con mayor volumen de compra
SELECT TOP 10 
    l.CodigoArticulo,
    l.DescripcionArticulo,
    SUM(l.Cantidad) as CantidadTotal,
    COUNT(DISTINCT l.CabeceraAlbaranId) as AlbaranesConArticulo,
    AVG(l.PrecioUnitario) as PrecioPromedio
FROM LineasAlbaranProveedor l
GROUP BY l.CodigoArticulo, l.DescripcionArticulo
ORDER BY CantidadTotal DESC;
```

### Albaranes Pendientes
```sql
-- Albaranes pendientes de recepción
SELECT 
    c.NumeroAlbaran,
    c.NombreProveedor,
    c.FechaAlbaran,
    c.ImporteTotal,
    DATEDIFF(day, c.FechaAlbaran, GETDATE()) as DiasTranscurridos
FROM CabeceraAlbaranProveedor c
WHERE c.Estado = 'Pendiente'
ORDER BY c.FechaAlbaran;
```

### Análisis por Categorías
```sql
-- Compras por categoría de artículo
SELECT 
    a.Categoria,
    COUNT(l.Id) as NumeroLineas,
    SUM(l.Cantidad) as CantidadTotal,
    SUM(l.ImporteLinea) as ImporteTotal,
    AVG(l.PrecioUnitario) as PrecioPromedio
FROM LineasAlbaranProveedor l
INNER JOIN Articulo a ON l.CodigoArticulo = a.Codigo
GROUP BY a.Categoria
ORDER BY ImporteTotal DESC;
```

## Notas de Implementación

1. **Integridad Referencial**: Las tablas están relacionadas mediante claves foráneas para mantener la consistencia de los datos.

2. **Índices**: Se han definido índices en los campos más consultados para optimizar el rendimiento.

3. **Datos de Ejemplo**: Los datos insertados son representativos de un entorno industrial real.

4. **Campos Auditables**: Se incluyen campos como `FechaCreacion` y `UsuarioCreacion` para auditoria.

5. **Flexibilidad**: La estructura permite extensiones futuras sin romper la compatibilidad.

## Adaptación a Sage 200 Real

Para conectar con una base de datos Sage 200 real:

1. **Identificar Tablas**: Las tablas reales pueden tener nombres diferentes
2. **Mapear Campos**: Algunos campos pueden tener nombres distintos
3. **Adaptar Tipos**: Verificar tipos de datos específicos de Sage 200
4. **Permisos**: Asegurar acceso de solo lectura a las tablas críticas

Esta estructura proporciona una base sólida para el desarrollo y testing del Enterprise Knowledge Hub.