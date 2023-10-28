// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System.IO;
using System.Reflection;
using System.Windows.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Modio;
using TNRD.Modkist.Factories.Controls;
using TNRD.Modkist.Models;
using TNRD.Modkist.Services;
using TNRD.Modkist.Settings;
using TNRD.Modkist.ViewModels.Pages;
using TNRD.Modkist.ViewModels.Windows;
using TNRD.Modkist.Views.Pages;
using TNRD.Modkist.Views.Windows;
using Wpf.Ui;

namespace TNRD.Modkist;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    // The.NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging
    private static readonly IHost _host = Host
        .CreateDefaultBuilder()
        .ConfigureAppConfiguration(
            c => { c.SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)); })
        .ConfigureServices((context, services) =>
        {
            services.Configure<ModioSettings>(context.Configuration.GetSection(ModioSettings.SECTION));

            services.AddHostedService<ApplicationHostService>();

            services.AddSingleton<Client>(provider =>
            {
                SettingsService settingsService = provider.GetRequiredService<SettingsService>();
                IOptions<ModioSettings> modioOptions = provider.GetRequiredService<IOptions<ModioSettings>>();

                if (settingsService.HasValidAccessToken())
                {
                    return new Client(new Uri("https://g-3213.modapi.io/v1"),
                        new Credentials(modioOptions.Value.ApiKey, settingsService.AccessToken!.Value!));
                }
                else
                {
                    return new Client(new Uri("https://g-3213.modapi.io/v1"),
                        new Credentials(modioOptions.Value.ApiKey));
                }
            });

            services.AddSingleton<AuthClient>(provider => provider.GetRequiredService<Client>().Auth);
            services.AddSingleton<ModsClient>(provider => provider.GetRequiredService<Client>().Games[3213].Mods);
            services.AddSingleton<UserClient>(provider => provider.GetRequiredService<Client>().User);

            services.AddHttpClient();
            services.AddSingleton<ImageCachingService>();

            services.AddSingleton<ModCardFactory>();

            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ISnackbarService, SnackbarService>();
            services.AddSingleton<IContentDialogService, ContentDialogService>();

            services.AddSingleton<SteamService>();
            services.AddSingleton<SettingsService>();

            services.AddSingleton<LoginModel>();

            services.AddSingleton<DashboardPage>().AddSingleton<DashboardViewModel>();
            services.AddSingleton<DataPage>().AddSingleton<DataViewModel>();
            services.AddSingleton<SettingsPage>().AddSingleton<SettingsViewModel>();
            services.AddSingleton<InitializationPage>().AddSingleton<InitializationViewModel>();
            services.AddSingleton<VerifyLoginPage>().AddSingleton<VerifyLoginViewModel>();
            services.AddSingleton<RequestLoginCodePage>().AddSingleton<RequestLoginCodeViewModel>();
            services.AddSingleton<EnterLoginCodePage>().AddSingleton<EnterLoginCodeViewModel>();
            services.AddSingleton<BrowsePluginsPage>().AddSingleton<BrowsePluginsViewModel>();
            services.AddSingleton<BrowseBlueprintsPage>();
        }).Build();

    /// <summary>
    /// Gets registered service.
    /// </summary>
    /// <typeparam name="T">Type of the service to get.</typeparam>
    /// <returns>Instance of the service or <see langword="null"/>.</returns>
    public static T GetService<T>()
        where T : class
    {
        return _host.Services.GetService(typeof(T)) as T;
    }

    /// <summary>
    /// Occurs when the application is loading.
    /// </summary>
    private void OnStartup(object sender, StartupEventArgs e)
    {
        _host.Start();
    }

    /// <summary>
    /// Occurs when the application is closing.
    /// </summary>
    private async void OnExit(object sender, ExitEventArgs e)
    {
        await _host.StopAsync();

        _host.Dispose();
    }

    /// <summary>
    /// Occurs when an exception is thrown by an application but not handled.
    /// </summary>
    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // For more info see https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
    }
}
