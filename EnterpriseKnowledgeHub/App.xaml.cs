using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Windows;
using EnterpriseKnowledgeHub.ViewModels;
using EnterpriseKnowledgeHub.Services;
using EnterpriseKnowledgeHub.Data;

namespace EnterpriseKnowledgeHub;

/// <summary>
/// Clase principal de la aplicación que maneja la configuración e inyección de dependencias.
/// Esta clase es el punto de entrada de nuestra aplicación WPF.
/// 
/// EXPLICACIÓN PARA PRINCIPIANTES:
/// - App.xaml.cs es donde configuramos todos los servicios que necesita nuestra aplicación
/// - Usamos inyección de dependencias para que el código sea más fácil de mantener y probar
/// - IHost nos permite usar el patrón de hosting moderno de .NET
/// </summary>
public partial class App : Application
{
    private IHost? _host;

    /// <summary>
    /// Método que se ejecuta cuando la aplicación se inicia.
    /// Aquí configuramos todos los servicios y dependencias.
    /// </summary>
    protected override async void OnStartup(StartupEventArgs e)
    {
        // Construimos el host con todos los servicios necesarios
        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, builder) =>
            {
                // Configuramos para leer el archivo appsettings.json
                builder.SetBasePath(Directory.GetCurrentDirectory())
                       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                // Registramos todos nuestros servicios para inyección de dependencias
                ConfigureServices(services, context.Configuration);
            })
            .ConfigureLogging(logging =>
            {
                // Configuramos el sistema de logging
                logging.AddConsole();
                logging.AddDebug();
            })
            .Build();

        // Iniciamos el host
        await _host.StartAsync();

        // Mostramos la ventana principal
        var mainWindow = _host.Services.GetRequiredService<Views.MainWindow>();
        mainWindow.Show();

        base.OnStartup(e);
    }

    /// <summary>
    /// Configuramos todos los servicios que necesita nuestra aplicación.
    /// 
    /// EXPLICACIÓN PARA PRINCIPIANTES:
    /// - AddSingleton: Una sola instancia durante toda la aplicación
    /// - AddTransient: Nueva instancia cada vez que se solicita
    /// - AddScoped: Una instancia por operación/request
    /// </summary>
    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Servicios de datos (acceso a base de datos)
        services.AddSingleton<IDatabaseConnectionService, DatabaseConnectionService>();
        services.AddSingleton<ISage200Repository, Sage200Repository>();
        
        // Servicios de negocio
        services.AddSingleton<IOllamaService, OllamaService>();
        services.AddSingleton<IQueryService, QueryService>();
        
        // ViewModels (patrón MVVM)
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<DatabaseExplorerViewModel>();
        services.AddTransient<QueryInterfaceViewModel>();
        
        // Ventanas
        services.AddTransient<Views.MainWindow>();
        
        // Configuración
        services.Configure<ConnectionStrings>(configuration.GetSection("ConnectionStrings"));
        services.Configure<OllamaSettings>(configuration.GetSection("OllamaSettings"));
    }

    /// <summary>
    /// Método que se ejecuta cuando la aplicación se cierra.
    /// Aquí limpiamos los recursos.
    /// </summary>
    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host != null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }

        base.OnExit(e);
    }
}

/// <summary>
/// Clase para configurar las cadenas de conexión desde appsettings.json
/// </summary>
public class ConnectionStrings
{
    public string Sage200Database { get; set; } = string.Empty;
}

/// <summary>
/// Clase para configurar los ajustes de Ollama desde appsettings.json
/// </summary>
public class OllamaSettings
{
    public string BaseUrl { get; set; } = "http://localhost:11434";
    public string Model { get; set; } = "llama2";
    public int Timeout { get; set; } = 30;
}