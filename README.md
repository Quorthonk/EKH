# Enterprise Knowledge Hub (EKH)

**Hub de Conocimiento Empresarial para Base de Datos Sage 200**

Una aplicación WPF moderna desarrollada en C# .NET 8 que permite explorar y consultar bases de datos de Sage 200 utilizando lenguaje natural con integración de inteligencia artificial (Ollama).

## 🌟 Características Principales

### 🎯 **Interfaz Amigable y Comprensible**
- **Diseño moderno**: Interfaz limpia y profesional
- **Navegación intuitiva**: Fácil de usar para usuarios no técnicos
- **Código documentado**: Explicaciones detalladas para aprendizaje

### 🗃️ **Explorador de Base de Datos**
- Navegación visual por tablas de Sage 200
- Visualización de datos con paginación
- Filtros de búsqueda en tiempo real
- Explicaciones contextuales de cada tabla

### 💬 **Consultas Inteligentes**
- **Lenguaje Natural**: "¿Cuáles son los proveedores con más albaranes?"
- **SQL Directo**: Para usuarios técnicos avanzados
- **Explicaciones IA**: Interpretación automática de resultados
- **Historial**: Reutilización de consultas anteriores

### 🤖 **Integración con IA (Ollama)**
- Procesamiento de consultas en lenguaje natural
- Generación automática de SQL
- Explicaciones detalladas de los datos
- Sugerencias inteligentes de consultas relacionadas

### 🏗️ **Arquitectura Modular**
- **Patrón MVVM**: Separación clara de responsabilidades
- **Inyección de Dependencias**: Código mantenible y testeable
- **Estructura para principiantes**: Fácil de entender y extender

## 🛠️ Tecnologías Utilizadas

- **Framework**: .NET 8 WPF (Windows)
- **Lenguaje**: C# con explicaciones detalladas
- **Base de Datos**: SQL Server (Sage 200)
- **ORM**: Dapper para consultas eficientes
- **IA**: Ollama para procesamiento de lenguaje natural
- **Patrón**: MVVM con CommunityToolkit.Mvvm
- **Logging**: Microsoft.Extensions.Logging

## 📸 Capturas de Pantalla

*Nota: Las capturas se generarán cuando se ejecute en un entorno Windows con WPF*

### Dashboard Principal
- Vista general del sistema
- Estadísticas rápidas
- Estado de conexiones
- Navegación principal

### Explorador de Base de Datos
- Lista de tablas disponibles
- Visualización de datos con paginación
- Explicaciones detalladas de cada tabla
- Filtros de búsqueda

### Interfaz de Consultas
- Editor de consultas (natural o SQL)
- Resultados en tiempo real
- Explicaciones de IA
- Historial de consultas

## 🚀 Inicio Rápido

### Requisitos Previos
- **Windows 10/11** (WPF requiere Windows)
- **.NET 8 SDK**
- **SQL Server** con base de datos Sage 200
- **Ollama** (opcional, para funcionalidades de IA)

### Instalación

1. **Clonar el repositorio**:
```bash
git clone https://github.com/Quorthonk/EKH.git
cd EKH/EnterpriseKnowledgeHub
```

2. **Configurar base de datos**:
```json
// En appsettings.json
{
  "ConnectionStrings": {
    "Sage200Database": "Server=localhost;Database=Sage200;Integrated Security=true;TrustServerCertificate=true;"
  }
}
```

3. **Instalar dependencias y ejecutar**:
```bash
dotnet restore
dotnet build
dotnet run
```

### Configuración Rápida con Datos de Prueba

Si no tienes una base de datos Sage 200, puedes usar nuestros datos de prueba:

1. Ejecutar el script `DATABASE_SCRIPTS.sql` en SQL Server
2. Actualizar la cadena de conexión en `appsettings.json`
3. Ejecutar la aplicación

## 📚 Documentación

### Para Usuarios
- **[Guía de Instalación](INSTALLATION_GUIDE.md)**: Instrucciones paso a paso
- **[Documentación Completa](DOCUMENTATION.md)**: Características y uso detallado

### Para Desarrolladores
- **[Scripts de Base de Datos](DATABASE_SCRIPTS.sql)**: Estructura y datos de ejemplo
- **Código Fuente**: Completamente documentado con explicaciones para principiantes

## 🗂️ Estructura del Proyecto

```
EnterpriseKnowledgeHub/
├── 📁 Models/              # Modelos de datos de Sage 200
├── 📁 ViewModels/          # Lógica de presentación (MVVM)
├── 📁 Views/               # Interfaces de usuario (XAML)
├── 📁 Services/            # Servicios de negocio y IA
├── 📁 Data/                # Acceso a datos y repositorios
├── 📁 Utilities/           # Herramientas y convertidores
├── 📄 App.xaml            # Configuración de la aplicación
└── 📄 appsettings.json    # Configuración (conexiones, IA)
```

## 🔍 Ejemplos de Uso

### Consultas en Lenguaje Natural
```
"¿Cuáles son los proveedores con más albaranes este mes?"
"¿Qué artículos se han recibido recientemente?"
"¿Cuál es el importe total de compras por proveedor?"
"¿Qué albaranes están pendientes de recepción?"
```

### Consultas SQL Directas
```sql
SELECT TOP 10 NombreProveedor, COUNT(*) as TotalAlbaranes
FROM CabeceraAlbaranProveedor
WHERE FechaAlbaran >= DATEADD(month, -1, GETDATE())
GROUP BY CodigoProveedor, NombreProveedor
ORDER BY TotalAlbaranes DESC
```

## 🎓 Aprendizaje para Principiantes

Este proyecto está diseñado como herramienta de aprendizaje:

### Conceptos Cubiertos
- **Patrón MVVM**: Separación de responsabilidades
- **Inyección de Dependencias**: Código desacoplado
- **Data Binding**: Conexión automática datos-UI
- **Programación Asíncrona**: Operaciones no bloqueantes
- **Integración con IA**: APIs modernas
- **Acceso a Datos**: Repositorios y ORMs

### Características Educativas
- **Comentarios Detallados**: Cada clase y método explicado
- **Arquitectura Clara**: Fácil de seguir y entender
- **Patrones Modernos**: Mejores prácticas de .NET
- **Ejemplos Reales**: Casos de uso empresariales

## 🤝 Contribución

¡Las contribuciones son bienvenidas!

### Cómo Contribuir
1. Fork del repositorio
2. Crear rama para tu característica (`git checkout -b feature/nueva-caracteristica`)
3. Commit de cambios (`git commit -am 'Agregar nueva característica'`)
4. Push a la rama (`git push origin feature/nueva-caracteristica`)
5. Crear Pull Request

### Áreas de Mejora
- [ ] Más tipos de visualizaciones de datos
- [ ] Exportación a diferentes formatos
- [ ] Integración con más modelos de IA
- [ ] Soporte para más bases de datos
- [ ] Mejoras en la interfaz de usuario

## 📋 Roadmap

### Versión Actual (1.0)
- ✅ Explorador de base de datos básico
- ✅ Consultas en lenguaje natural
- ✅ Integración con Ollama
- ✅ Interfaz WPF moderna

### Próximas Versiones
- [ ] **v1.1**: Exportación de datos (Excel, CSV, PDF)
- [ ] **v1.2**: Gráficos y visualizaciones
- [ ] **v1.3**: Reportes personalizados
- [ ] **v2.0**: Versión web con Blazor

## ⚠️ Limitaciones Conocidas

- **Solo Windows**: WPF requiere sistema operativo Windows
- **Sage 200**: Diseñado específicamente para esta base de datos
- **Ollama Local**: Requiere instalación local para funcionalidades de IA
- **Solo Lectura**: No modifica datos de la base de datos

## 📞 Soporte y Contacto

### Reportar Problemas
- Crear issue en GitHub con detalles completos
- Incluir logs y configuración (sin datos sensibles)

### Solicitar Características
- Abrir discussion en GitHub
- Describir el caso de uso y beneficios

## 📄 Licencia

Este proyecto está bajo la licencia MIT - ver [LICENSE](LICENSE) para detalles.

---

## 🎯 ¿Por qué Enterprise Knowledge Hub?

### Para Empresas
- **Democratiza el acceso a datos**: Los usuarios no técnicos pueden explorar datos complejos
- **Reduce dependencia de IT**: Consultas autoservicio con explicaciones claras
- **Acelera la toma de decisiones**: Acceso inmediato a información empresarial

### Para Desarrolladores
- **Código de aprendizaje**: Ejemplo real de aplicación empresarial moderna
- **Patrones establecidos**: MVVM, DI, async/await implementados correctamente
- **Integración con IA**: Ejemplo práctico de incorporar IA en aplicaciones tradicionales

### Para Estudiantes
- **Proyecto completo**: Desde base de datos hasta interfaz de usuario
- **Explicaciones detalladas**: Cada concepto explicado para facilitar el aprendizaje
- **Tecnologías modernas**: .NET 8, WPF, IA, y mejores prácticas actuales

---

**¡Explora tus datos de Sage 200 como nunca antes! 🚀**
