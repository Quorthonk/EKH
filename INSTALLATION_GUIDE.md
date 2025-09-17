# Enterprise Knowledge Hub - Guía de Instalación

Esta guía te ayudará a instalar y configurar el Enterprise Knowledge Hub paso a paso.

## 📋 Requisitos Previos

### Sistema Operativo
- **Windows 10** (versión 1809 o superior) o **Windows 11**
- **WPF solo funciona en Windows**

### Software Requerido

#### 1. .NET 8 SDK
```powershell
# Opción 1: Usando winget (recomendado)
winget install Microsoft.DotNet.SDK.8

# Opción 2: Descarga manual
# Ir a: https://dotnet.microsoft.com/download/dotnet/8.0
```

#### 2. SQL Server
```powershell
# SQL Server Express (gratuito)
winget install Microsoft.SQLServer.2022.Express

# O usar SQL Server existente con base de datos Sage 200
```

#### 3. Ollama (Opcional - para funcionalidades de IA)
```powershell
# Descargar desde: https://ollama.ai/
# Instalar modelo recomendado:
ollama pull llama2
```

## 🗄️ Configuración de Base de Datos

### Opción A: Base de Datos de Prueba

1. **Crear base de datos de prueba**:
```sql
-- Conectar a SQL Server Management Studio
CREATE DATABASE Sage200Test;
USE Sage200Test;

-- Ejecutar el script completo desde DATABASE_SCRIPTS.sql
```

2. **Cargar datos de ejemplo**:
   - Ejecutar todas las secciones del archivo `DATABASE_SCRIPTS.sql`
   - Verificar que las tablas contienen datos

### Opción B: Base de Datos Sage 200 Real

1. **Identificar la base de datos Sage 200 existente**
2. **Crear usuario de solo lectura**:
```sql
-- Crear usuario para la aplicación
CREATE LOGIN EKH_ReadUser WITH PASSWORD = 'TuPassword123!';
USE [TuBaseDatosSage200];
CREATE USER EKH_ReadUser FOR LOGIN EKH_ReadUser;

-- Otorgar permisos de solo lectura
ALTER ROLE db_datareader ADD MEMBER EKH_ReadUser;
```

3. **Mapear tablas reales**:
   - Identificar las tablas equivalentes en tu Sage 200
   - Actualizar los nombres en el código si es necesario

## ⚙️ Configuración de la Aplicación

### 1. Clonar el Repositorio
```bash
git clone [URL_DEL_REPOSITORIO]
cd EKH/EnterpriseKnowledgeHub
```

### 2. Configurar Conexión de Base de Datos

Editar `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "Sage200Database": "Server=TU_SERVIDOR;Database=TU_BASE_DATOS;User Id=EKH_ReadUser;Password=TuPassword123!;TrustServerCertificate=true;"
  },
  "OllamaSettings": {
    "BaseUrl": "http://localhost:11434",
    "Model": "llama2",
    "Timeout": 30
  }
}
```

**Ejemplos de cadenas de conexión**:

```json
// SQL Server con autenticación Windows
"Server=localhost;Database=Sage200Test;Integrated Security=true;TrustServerCertificate=true;"

// SQL Server con usuario/contraseña
"Server=localhost,1433;Database=Sage200Test;User Id=sa;Password=TuPassword123!;TrustServerCertificate=true;"

// SQL Server Express
"Server=localhost\\SQLEXPRESS;Database=Sage200Test;Integrated Security=true;TrustServerCertificate=true;"

// SQL Server en red
"Server=192.168.1.100;Database=Sage200;User Id=EKH_ReadUser;Password=TuPassword123!;TrustServerCertificate=true;"
```

### 3. Configurar Ollama (Opcional)

Si quieres usar las funcionalidades de IA:

1. **Instalar Ollama**:
   - Descargar desde https://ollama.ai/
   - Seguir el instalador

2. **Instalar un modelo**:
```bash
# Modelos recomendados (elegir uno):
ollama pull llama2          # Modelo general, rápido
ollama pull codellama       # Especializado en código/SQL
ollama pull mistral         # Modelo más pequeño y rápido
```

3. **Verificar instalación**:
```bash
ollama list                 # Ver modelos instalados
ollama serve               # Iniciar servicio (normalmente automático)
```

4. **Actualizar configuración**:
```json
{
  "OllamaSettings": {
    "BaseUrl": "http://localhost:11434",
    "Model": "llama2",        // Cambiar por el modelo que instalaste
    "Timeout": 30
  }
}
```

## 🚀 Compilación y Ejecución

### 1. Restaurar Dependencias
```bash
dotnet restore
```

### 2. Compilar el Proyecto
```bash
dotnet build
```

### 3. Ejecutar la Aplicación
```bash
# Modo desarrollo
dotnet run

# O generar ejecutable
dotnet publish -c Release -o ./publish
./publish/EnterpriseKnowledgeHub.exe
```

## ✅ Verificación de Instalación

### 1. Verificar Conexión de Base de Datos

Al abrir la aplicación, deberías ver:
- 🟢 Indicador verde junto a "Base de Datos"
- Información de conexión en la barra de estado
- Estadísticas en el panel lateral

### 2. Verificar Conexión de Ollama

Si configuraste Ollama:
- 🟢 Indicador verde junto a "Ollama IA"
- Información del modelo en la barra de estado

### 3. Probar Funcionalidades

**Explorador de Base de Datos**:
1. Click en "🗃️ Explorar Base de Datos"
2. Seleccionar "Albaranes" en el panel lateral
3. Verificar que se muestran datos

**Interfaz de Consultas**:
1. Click en "💬 Interfaz de Consultas"
2. Probar consulta: "¿Cuántos albaranes hay en total?"
3. Verificar respuesta (con o sin IA)

## 🔧 Solución de Problemas

### Error de Conexión a Base de Datos

**Síntoma**: Indicador rojo en "Base de Datos"

**Soluciones**:
1. **Verificar que SQL Server está ejecutándose**:
```cmd
# Verificar servicios
services.msc
# Buscar "SQL Server" y verificar que está "Ejecutándose"
```

2. **Probar conexión manualmente**:
```cmd
# Usar SQL Server Management Studio o sqlcmd
sqlcmd -S localhost -d Sage200Test -E
```

3. **Verificar firewall**:
   - Permitir SQL Server en el firewall de Windows
   - Puerto por defecto: 1433

4. **Revisar cadena de conexión**:
   - Verificar servidor, base de datos, credenciales
   - Probar con diferentes opciones de autenticación

### Error de Conexión a Ollama

**Síntoma**: Indicador rojo en "Ollama IA"

**Soluciones**:
1. **Verificar que Ollama está ejecutándose**:
```bash
# Verificar proceso
tasklist | findstr ollama

# Reiniciar si es necesario
ollama serve
```

2. **Probar manualmente**:
```bash
# Hacer una consulta de prueba
ollama run llama2 "Hola, ¿cómo estás?"
```

3. **Verificar puerto**:
   - Puerto por defecto: 11434
   - Verificar que no esté bloqueado

### Errores de Compilación

**Error**: "Microsoft.NET.Sdk.WindowsDesktop.targets was not found"
- **Causa**: Intentando compilar en Linux/Mac
- **Solución**: WPF solo funciona en Windows, usar Windows para compilar

**Error**: Paquetes NuGet no encontrados
```bash
# Limpiar y restaurar
dotnet clean
dotnet restore
dotnet build
```

### Problemas de Rendimiento

**Consultas lentas**:
1. Verificar índices en la base de datos
2. Limitar número de registros mostrados
3. Usar filtros en las consultas

**IA lenta**:
1. Usar modelo más pequeño (mistral en lugar de llama2)
2. Aumentar timeout en configuración
3. Verificar recursos del sistema (RAM, CPU)

## 📞 Soporte

### Logs de la Aplicación

Los logs se muestran en:
- Consola de Visual Studio (modo debug)
- Output window en Visual Studio
- Event Viewer de Windows (si se configura)

### Información de Sistema

Para reportar problemas, incluir:
- Versión de Windows
- Versión de .NET instalada: `dotnet --version`
- Versión de SQL Server
- Configuración de Ollama (si aplica)
- Mensajes de error completos

### Archivos de Configuración

Verificar estos archivos si hay problemas:
- `appsettings.json` - Configuración principal
- `EnterpriseKnowledgeHub.csproj` - Dependencias del proyecto
- Logs de aplicación - Para diagnóstico

## 🎯 Próximos Pasos

Una vez instalado exitosamente:

1. **Familiarizarse con la interfaz**:
   - Explorar el dashboard principal
   - Navegar por las diferentes secciones
   - Probar consultas de ejemplo

2. **Configurar para tu entorno**:
   - Adaptar nombres de tablas si es necesario
   - Personalizar consultas frecuentes
   - Configurar usuarios adicionales

3. **Explorar funcionalidades avanzadas**:
   - Consultas SQL personalizadas
   - Integración con herramientas de reporting
   - Automatización de consultas frecuentes

¡Disfruta explorando tus datos de Sage 200 con Enterprise Knowledge Hub! 🚀