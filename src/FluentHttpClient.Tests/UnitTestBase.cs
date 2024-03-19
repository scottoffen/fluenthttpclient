using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FluentHttpClient.Tests;

public abstract class UnitTestBase
{
    protected IServiceCollection _services;
    protected IServiceProvider _serviceProvider;
    protected IConfigurationRoot _configuration;

    public UnitTestBase()
    {
        _services = new ServiceCollection();
        _configuration = InitializeConfigurationBuilder().Build();
        ConfigureDefaultLogging(_services);
        ConfigureServices(_services, _configuration);
        _serviceProvider = _services.BuildServiceProvider();
    }

    /// <summary>
    /// This method allows the inheriting class to define a different IConfigurationBuilder initialization.
    /// </summary>
    /// <returns><see cref="IConfigurationBuilder" /></returns>
    protected virtual IConfigurationBuilder InitializeConfigurationBuilder()
    {
        var projectDir = Directory.GetCurrentDirectory();
        var appSettingsPath = Path.Combine(projectDir, "appsettings.json");
        var localAppSettingsPath = Path.Combine(projectDir, "appsettings.local.json");

        var configurationBuilder = new ConfigurationBuilder()
            .AddJsonFile(appSettingsPath);

        if (File.Exists(localAppSettingsPath))
            configurationBuilder.AddJsonFile(localAppSettingsPath);

        return configurationBuilder;
    }

    /// <summary>
    /// Configures a default logging provider that can be overridden in ConfigureServices.
    /// </summary>
    /// <param name="services"></param>
    private static void ConfigureDefaultLogging(IServiceCollection services)
    {
        services.AddLogging(options =>
        {
            options.AddDebug();
            options.AddConsole();
        });
    }

    /// <summary>
    /// Abstract method allows implementing class to inject dependencies.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    protected abstract void ConfigureServices(IServiceCollection services, IConfiguration configuration);

    /// <summary>
    /// Get service of type T from the IServiceProvider.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>A service object of type T or null if there is no such service.</returns>
    protected T? GetService<T>() => _serviceProvider.GetService<T>();

    /// <summary>
    /// Get service of type T from the IServiceProvider.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>A service object of type T.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    protected T GetRequiredService<T>() where T : notnull => _serviceProvider.GetRequiredService<T>();
}
