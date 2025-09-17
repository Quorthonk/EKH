using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using EnterpriseKnowledgeHub.Services;
using EnterpriseKnowledgeHub.Data;
using EnterpriseKnowledgeHub.Models;
using System.Collections.ObjectModel;

namespace EnterpriseKnowledgeHub.ViewModels;

/// <summary>
/// ViewModel para explorar la estructura y datos de la base de datos.
/// 
/// EXPLICACIÓN PARA PRINCIPIANTES:
/// - Este ViewModel maneja la navegación y exploración de la base de datos
/// - Permite ver tablas, registros y obtener explicaciones sobre los datos
/// - Facilita el entendimiento de la estructura de datos de Sage 200
/// </summary>
public partial class DatabaseExplorerViewModel : ObservableObject
{
    private readonly ISage200Repository _repository;
    private readonly IQueryService _queryService;
    private readonly ILogger<DatabaseExplorerViewModel> _logger;

    public DatabaseExplorerViewModel(
        ISage200Repository repository,
        IQueryService queryService,
        ILogger<DatabaseExplorerViewModel> logger)
    {
        _repository = repository;
        _queryService = queryService;
        _logger = logger;

        // Inicializamos las colecciones
        DatabaseTables = new ObservableCollection<DatabaseTable>();
        Albaranes = new ObservableCollection<CabeceraAlbaranProveedor>();
        Proveedores = new ObservableCollection<Proveedor>();
        Articulos = new ObservableCollection<Articulo>();

        // Cargamos los datos iniciales
        _ = LoadDatabaseStructureAsync();
    }

    #region Propiedades Observables

    /// <summary>
    /// Indica si se están cargando datos
    /// </summary>
    [ObservableProperty]
    private bool isLoading;

    /// <summary>
    /// Tabla seleccionada actualmente
    /// </summary>
    [ObservableProperty]
    private DatabaseTable? selectedTable;

    /// <summary>
    /// Explicación de la tabla seleccionada
    /// </summary>
    [ObservableProperty]
    private string tableExplanation = string.Empty;

    /// <summary>
    /// Número de registros a mostrar por página
    /// </summary>
    [ObservableProperty]
    private int pageSize = 50;

    /// <summary>
    /// Página actual
    /// </summary>
    [ObservableProperty]
    private int currentPage = 1;

    /// <summary>
    /// Total de páginas disponibles
    /// </summary>
    [ObservableProperty]
    private int totalPages = 1;

    /// <summary>
    /// Filtro de búsqueda actual
    /// </summary>
    [ObservableProperty]
    private string searchFilter = string.Empty;

    /// <summary>
    /// Lista de tablas disponibles en la base de datos
    /// </summary>
    public ObservableCollection<DatabaseTable> DatabaseTables { get; }

    /// <summary>
    /// Lista de albaranes cargados
    /// </summary>
    public ObservableCollection<CabeceraAlbaranProveedor> Albaranes { get; }

    /// <summary>
    /// Lista de proveedores cargados
    /// </summary>
    public ObservableCollection<Proveedor> Proveedores { get; }

    /// <summary>
    /// Lista de artículos cargados
    /// </summary>
    public ObservableCollection<Articulo> Articulos { get; }

    #endregion

    #region Comandos

    /// <summary>
    /// Comando para cargar datos de una tabla específica
    /// </summary>
    [RelayCommand]
    private async Task LoadTableData(DatabaseTable? table)
    {
        if (table == null) return;

        try
        {
            IsLoading = true;
            SelectedTable = table;
            _logger.LogInformation("Cargando datos de tabla: {TableName}", table.Name);

            // Obtenemos explicación de la tabla
            TableExplanation = await _queryService.ExplainDatabaseEntityAsync(table.Name);

            // Cargamos los datos específicos según la tabla
            switch (table.Name.ToLower())
            {
                case "albaranes":
                case "cabeceraalbaran":
                    await LoadAlbaranesAsync();
                    break;
                case "proveedores":
                case "proveedor":
                    await LoadProveedoresAsync();
                    break;
                case "articulos":
                case "articulo":
                    await LoadArticulosAsync();
                    break;
                default:
                    _logger.LogWarning("Tabla no reconocida: {TableName}", table.Name);
                    break;
            }

            _logger.LogInformation("Datos cargados exitosamente para {TableName}", table.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar datos de tabla {TableName}", table?.Name);
            TableExplanation = "Error al cargar la explicación de la tabla.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Comando para buscar en la tabla actual
    /// </summary>
    [RelayCommand]
    private async Task SearchTable()
    {
        if (SelectedTable == null) return;

        try
        {
            IsLoading = true;
            _logger.LogInformation("Buscando en tabla {TableName} con filtro: {Filter}", 
                SelectedTable.Name, SearchFilter);

            // Aplicamos el filtro según la tabla actual
            await ApplySearchFilterAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar en tabla");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Comando para limpiar el filtro de búsqueda
    /// </summary>
    [RelayCommand]
    private async Task ClearSearch()
    {
        SearchFilter = string.Empty;
        CurrentPage = 1;
        await LoadTableData(SelectedTable);
    }

    /// <summary>
    /// Comando para navegar a la página anterior
    /// </summary>
    [RelayCommand]
    private async Task PreviousPage()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
            await LoadTableData(SelectedTable);
        }
    }

    /// <summary>
    /// Comando para navegar a la página siguiente
    /// </summary>
    [RelayCommand]
    private async Task NextPage()
    {
        if (CurrentPage < TotalPages)
        {
            CurrentPage++;
            await LoadTableData(SelectedTable);
        }
    }

    /// <summary>
    /// Comando para refrescar los datos actuales
    /// </summary>
    [RelayCommand]
    private async Task RefreshData()
    {
        await LoadTableData(SelectedTable);
    }

    /// <summary>
    /// Comando para exportar datos a CSV
    /// </summary>
    [RelayCommand]
    private async Task ExportToCSV()
    {
        // TODO: Implementar exportación a CSV
        _logger.LogInformation("Exportando datos a CSV - funcionalidad pendiente");
    }

    #endregion

    #region Métodos Privados

    /// <summary>
    /// Carga la estructura de tablas disponibles
    /// </summary>
    private async Task LoadDatabaseStructureAsync()
    {
        try
        {
            DatabaseTables.Clear();

            // Definimos las tablas principales que manejamos
            var tables = new[]
            {
                new DatabaseTable("Albaranes", "Cabeceras de albaranes de proveedores", "📋"),
                new DatabaseTable("Proveedores", "Información de proveedores", "🏢"),
                new DatabaseTable("Artículos", "Catálogo de artículos/productos", "📦")
            };

            foreach (var table in tables)
            {
                DatabaseTables.Add(table);
            }

            // Seleccionamos la primera tabla por defecto
            if (DatabaseTables.Any())
            {
                await LoadTableData(DatabaseTables.First());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar estructura de base de datos");
        }
    }

    /// <summary>
    /// Carga los albaranes de proveedores
    /// </summary>
    private async Task LoadAlbaranesAsync()
    {
        try
        {
            Albaranes.Clear();
            var albaranes = await _repository.GetAlbaranesProveedorAsync();
            
            // Aplicamos paginación
            var pagedAlbaranes = albaranes
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize);

            foreach (var albaran in pagedAlbaranes)
            {
                Albaranes.Add(albaran);
            }

            // Calculamos el total de páginas
            TotalPages = (int)Math.Ceiling((double)albaranes.Count() / PageSize);
            
            _logger.LogInformation("Cargados {Count} albaranes en página {Page} de {Total}", 
                Albaranes.Count, CurrentPage, TotalPages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar albaranes");
        }
    }

    /// <summary>
    /// Carga los proveedores
    /// </summary>
    private async Task LoadProveedoresAsync()
    {
        try
        {
            Proveedores.Clear();
            var proveedores = await _repository.GetProveedoresAsync();
            
            // Aplicamos paginación
            var pagedProveedores = proveedores
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize);

            foreach (var proveedor in pagedProveedores)
            {
                Proveedores.Add(proveedor);
            }

            TotalPages = (int)Math.Ceiling((double)proveedores.Count() / PageSize);
            
            _logger.LogInformation("Cargados {Count} proveedores en página {Page} de {Total}", 
                Proveedores.Count, CurrentPage, TotalPages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar proveedores");
        }
    }

    /// <summary>
    /// Carga los artículos
    /// </summary>
    private async Task LoadArticulosAsync()
    {
        try
        {
            Articulos.Clear();
            var articulos = await _repository.GetArticulosAsync();
            
            // Aplicamos paginación
            var pagedArticulos = articulos
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize);

            foreach (var articulo in pagedArticulos)
            {
                Articulos.Add(articulo);
            }

            TotalPages = (int)Math.Ceiling((double)articulos.Count() / PageSize);
            
            _logger.LogInformation("Cargados {Count} artículos en página {Page} de {Total}", 
                Articulos.Count, CurrentPage, TotalPages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar artículos");
        }
    }

    /// <summary>
    /// Aplica el filtro de búsqueda actual
    /// </summary>
    private async Task ApplySearchFilterAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchFilter))
        {
            await LoadTableData(SelectedTable);
            return;
        }

        try
        {
            switch (SelectedTable?.Name.ToLower())
            {
                case "proveedores":
                case "proveedor":
                    await SearchProveedoresAsync();
                    break;
                case "articulos":
                case "articulo":
                    await SearchArticulosAsync();
                    break;
                default:
                    // Para albaranes y otras tablas, usamos el método general
                    await LoadTableData(SelectedTable);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al aplicar filtro de búsqueda");
        }
    }

    /// <summary>
    /// Busca proveedores por nombre
    /// </summary>
    private async Task SearchProveedoresAsync()
    {
        try
        {
            Proveedores.Clear();
            var proveedores = await _repository.BuscarProveedoresByNombreAsync(SearchFilter);
            
            foreach (var proveedor in proveedores.Take(PageSize))
            {
                Proveedores.Add(proveedor);
            }

            TotalPages = (int)Math.Ceiling((double)proveedores.Count() / PageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar proveedores");
        }
    }

    /// <summary>
    /// Busca artículos por descripción
    /// </summary>
    private async Task SearchArticulosAsync()
    {
        try
        {
            Articulos.Clear();
            var articulos = await _repository.BuscarArticulosByDescripcionAsync(SearchFilter);
            
            foreach (var articulo in articulos.Take(PageSize))
            {
                Articulos.Add(articulo);
            }

            TotalPages = (int)Math.Ceiling((double)articulos.Count() / PageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar artículos");
        }
    }

    #endregion
}

/// <summary>
/// Clase auxiliar para representar tablas de la base de datos
/// </summary>
public class DatabaseTable
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Icon { get; set; }

    public DatabaseTable(string name, string description, string icon)
    {
        Name = name;
        Description = description;
        Icon = icon;
    }
}