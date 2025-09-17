# Enterprise Knowledge Hub - Documentación

## Descripción del Proyecto

El Enterprise Knowledge Hub es una aplicación de escritorio desarrollada en **C# WPF con .NET 8** que permite explorar y consultar una base de datos de **Sage 200** de manera intuitiva. La aplicación integra **inteligencia artificial (Ollama)** para proporcionar consultas en lenguaje natural y explicaciones detalladas de los datos.

## Características Principales

### 🏗️ Arquitectura Modular y Comprensible
- **Patrón MVVM**: Separación clara entre presentación, lógica y datos
- **Inyección de Dependencias**: Facilita testing y mantenimiento
- **Código Documentado**: Explicaciones detalladas para principiantes
- **Estructura Clara**: Organización lógica de carpetas y archivos

### 🗃️ Explorador de Base de Datos
- Navegación intuitiva por las tablas de Sage 200
- Visualización de datos con paginación
- Filtros de búsqueda en tiempo real
- Explicaciones detalladas de cada tabla y campo

### 💬 Interfaz de Consultas Inteligentes
- **Lenguaje Natural**: "¿Cuáles son los proveedores con más albaranes?"
- **SQL Directo**: Para usuarios técnicos
- **Explicaciones IA**: Interpretación automática de resultados
- **Historial**: Reutilización de consultas anteriores

### 🤖 Integración con Ollama
- Procesamiento de consultas en lenguaje natural
- Generación automática de SQL
- Explicaciones contextuales de los datos
- Sugerencias inteligentes de consultas relacionadas

### 🎨 Interfaz Amigable
- Diseño moderno y limpio
- Indicadores visuales de estado
- Navegación intuitiva
- Responsive design

## Estructura del Proyecto

```
EnterpriseKnowledgeHub/
├── Models/                     # Modelos de datos
│   ├── CabeceraAlbaranProveedor.cs
│   ├── LineasAlbaranProveedor.cs
│   ├── Proveedor.cs
│   └── Articulo.cs
├── ViewModels/                 # Lógica de presentación (MVVM)
│   ├── MainWindowViewModel.cs
│   ├── DatabaseExplorerViewModel.cs
│   └── QueryInterfaceViewModel.cs
├── Views/                      # Interfaces de usuario
│   ├── MainWindow.xaml
│   ├── DatabaseExplorerView.xaml
│   └── QueryInterfaceView.xaml
├── Services/                   # Servicios de negocio
│   ├── IOllamaService.cs
│   ├── OllamaService.cs
│   ├── IQueryService.cs
│   └── QueryService.cs
├── Data/                       # Acceso a datos
│   ├── ISage200Repository.cs
│   ├── Sage200Repository.cs
│   ├── IDatabaseConnectionService.cs
│   └── DatabaseConnectionService.cs
└── Utilities/                  # Utilidades
    └── ValueConverters.cs
```

## Tecnologías Utilizadas

### Framework Principal
- **.NET 8**: Framework multiplataforma moderno
- **WPF (Windows Presentation Foundation)**: Framework de UI nativo para Windows
- **C#**: Lenguaje de programación principal

### Librerías y Paquetes
- **CommunityToolkit.Mvvm**: Implementación moderna del patrón MVVM
- **Microsoft.Extensions.Hosting**: Hosting y ciclo de vida de la aplicación
- **Microsoft.Extensions.DependencyInjection**: Inyección de dependencias
- **Microsoft.Data.SqlClient**: Conectividad con SQL Server
- **Dapper**: ORM ligero para consultas SQL
- **Newtonsoft.Json**: Serialización JSON

### Servicios Externos
- **Ollama**: Plataforma local de modelos de IA
- **SQL Server**: Base de datos Sage 200

## Configuración

### Archivo appsettings.json
```json
{
  "ConnectionStrings": {
    "Sage200Database": "Server=localhost;Database=Sage200;Integrated Security=true;TrustServerCertificate=true;"
  },
  "OllamaSettings": {
    "BaseUrl": "http://localhost:11434",
    "Model": "llama2",
    "Timeout": 30
  }
}
```

### Requisitos del Sistema
- **Windows 10/11**: Sistema operativo requerido para WPF
- **.NET 8 Runtime**: Framework de ejecución
- **SQL Server**: Base de datos con Sage 200
- **Ollama**: Servicio de IA local (opcional)

## Instalación y Configuración

### 1. Prerrequisitos
```bash
# Instalar .NET 8 SDK
winget install Microsoft.DotNet.SDK.8

# Instalar Ollama (opcional)
# Descargar desde: https://ollama.ai/
```

### 2. Configuración de Base de Datos
- Asegurar acceso a la base de datos Sage 200
- Actualizar la cadena de conexión en `appsettings.json`
- Verificar permisos de lectura en las tablas

### 3. Configuración de Ollama (Opcional)
```bash
# Instalar un modelo (ejemplo: llama2)
ollama pull llama2

# Verificar que el servicio está ejecutándose
ollama list
```

### 4. Compilación y Ejecución
```bash
# Restaurar dependencias
dotnet restore

# Compilar el proyecto
dotnet build

# Ejecutar la aplicación
dotnet run
```

## Uso de la Aplicación

### 1. Dashboard Principal
- Vista general del estado del sistema
- Conexión a base de datos y servicio IA
- Estadísticas rápidas
- Navegación a funcionalidades principales

### 2. Explorador de Base de Datos
- Seleccionar tabla para explorar
- Ver datos con paginación
- Aplicar filtros de búsqueda
- Leer explicaciones detalladas

### 3. Interfaz de Consultas
- Escribir consulta en lenguaje natural o SQL
- Ejecutar y ver resultados
- Leer explicación de IA
- Revisar historial de consultas

## Ejemplos de Consultas

### Lenguaje Natural
```
¿Cuáles son los proveedores con más albaranes este mes?
¿Qué artículos se han recibido recientemente?
¿Cuál es el importe total de compras por proveedor?
¿Qué albaranes están pendientes de recepción?
```

### SQL Directo
```sql
SELECT TOP 10 NombreProveedor, COUNT(*) as TotalAlbaranes
FROM CabeceraAlbaranProveedor
WHERE FechaAlbaran >= DATEADD(month, -1, GETDATE())
GROUP BY CodigoProveedor, NombreProveedor
ORDER BY TotalAlbaranes DESC
```

## Extensibilidad

### Agregar Nuevas Tablas
1. Crear modelo en `Models/`
2. Agregar métodos en `ISage200Repository`
3. Implementar en `Sage200Repository`
4. Actualizar ViewModels según necesidad

### Integrar Nuevos Servicios IA
1. Crear interfaz en `Services/`
2. Implementar servicio
3. Registrar en `App.xaml.cs`
4. Actualizar `QueryService` para usar nuevo servicio

## Documentación Técnica

### Patrón MVVM
- **Model**: Representa los datos (modelos de Sage 200)
- **View**: Interfaz de usuario (archivos XAML)
- **ViewModel**: Lógica de presentación y binding de datos

### Inyección de Dependencias
- Servicios registrados en `App.xaml.cs`
- ViewModels reciben dependencias en constructor
- Facilita testing y intercambio de implementaciones

### Data Binding
- Propiedades observables con `ObservableProperty`
- Comandos con `RelayCommand`
- Convertidores de valores para transformaciones UI

## Mantenimiento y Soporte

### Logs
- Sistema de logging configurado con `Microsoft.Extensions.Logging`
- Logs en consola y debug output
- Niveles: Information, Warning, Error

### Testing
- Estructura preparada para unit testing
- Interfaces permiten mocking fácil
- Separación de responsabilidades facilita testing

### Monitoreo
- Indicadores de estado en la interfaz
- Verificación automática de conexiones
- Manejo de errores con mensajes amigables

## Licencia y Créditos

- Desarrollado como Enterprise Knowledge Hub
- Diseñado para facilitar el aprendizaje de programación
- Código documentado para fines educativos

---

**Nota**: Esta aplicación requiere Windows para ejecutarse debido al uso de WPF. Para entornos no Windows, considerar migrar a tecnologías multiplataforma como Avalonia UI o crear una versión web con Blazor.