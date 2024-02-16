using DeskLibrary.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Mongo.Library.Clients;
using Mongo.Library.Repositories.Desk;
using Mongo.Library.Settings;
using UpdateDesksSlots;

internal class Program
{
    private static void LoadConfiguration(HostBuilderContext host, IConfigurationBuilder builder)
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        builder.SetBasePath(Environment.CurrentDirectory)
            .AddEnvironmentVariables()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
            .Build();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(LoadConfiguration)
            .ConfigureServices(ConfigureServices);

    private static void ConfigureServices(HostBuilderContext host, IServiceCollection services)
    {
        // Add Mongo Db configurations
        services.Configure<DeskSettings>(settings =>
        {
            settings.ConnectionString = host.Configuration.GetSection("DeskSettings:ConnectionString").Value ?? string.Empty;
            settings.DatabaseName = host.Configuration.GetSection("DeskSettings:DatabaseName").Value ?? string.Empty;
        });

        services.AddSingleton(sp => sp.GetRequiredService<IOptions<DeskSettings>>().Value);
        services.AddSingleton(sp => new DeskMongoClient(sp.GetRequiredService<DeskSettings>().ConnectionString));

        // Add services to the container.
        services.AddScoped(typeof(IDeskRepository<>), typeof(DeskRepository<>));
        services.AddScoped<IDeskService, DeskService>();
        services.AddScoped<IWorkflow, Workflow>();
    }

    private static async Task Main(string[] args)
    {
        Console.WriteLine("Script initiated, creating host builder...");
        var host = CreateHostBuilder(args).Build();

        try
        {
            Console.WriteLine("Creating scope...");
            using var scope = host.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IWorkflow>();
            
            Console.WriteLine("Launching service...");
            await service.UpdateDesksSlots();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while processing script: {ex}");
        }
        finally
        {
            Console.WriteLine("Script terminated.");
        }
    }
}
