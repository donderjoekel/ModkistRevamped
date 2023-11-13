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
using Serilog;
using TNRD.Modkist.Factories.Controls;
using TNRD.Modkist.Factories.ViewModels;
using TNRD.Modkist.Models;
using TNRD.Modkist.Services;
using TNRD.Modkist.Services.Hosted;
using TNRD.Modkist.Services.Rating;
using TNRD.Modkist.Services.Subscription;
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
        .UseSerilog((context, configuration) =>
        {
            configuration
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .MinimumLevel.Information()
                .WriteTo.File("logs/output.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 3);
        })
        .ConfigureAppConfiguration(
            c => { c.SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)); })
        .ConfigureServices((context, services) =>
        {
            services.AddHostedService<ApplicationHostService>();

            // Add an extra hosted service for the downloader
            services.AddSingleton<ModManagerHostedService>();
            services.AddSingleton<IHostedService>(provider => provider.GetRequiredService<ModManagerHostedService>());

            // Configuration
            services.Configure<ModioSettings>(context.Configuration.GetSection(ModioSettings.SECTION));

            // Main mod.io client with authentication
            services.AddSingleton<Client>(provider =>
            {
                SettingsService settingsService = provider.GetRequiredService<SettingsService>();
                IOptions<ModioSettings> modioOptions = provider.GetRequiredService<IOptions<ModioSettings>>();

                return settingsService.HasValidAccessToken()
                    ? new Client(new Uri("https://g-3213.modapi.io/v1"),
                        new Credentials(modioOptions.Value.ApiKey, settingsService.AccessToken!.Value!))
                    : new Client(new Uri("https://g-3213.modapi.io/v1"), new Credentials(modioOptions.Value.ApiKey));
            });

            // Mod.io clients
            services.AddSingleton<AuthClient>(provider => provider.GetRequiredService<Client>().Auth);
            services.AddSingleton<ModsClient>(provider => provider.GetRequiredService<Client>().Games[3213].Mods);
            services.AddSingleton<UserClient>(provider => provider.GetRequiredService<Client>().User);

            // Image loading
            services.AddHttpClient();
            services.AddSingleton<ImageCachingService>();

            // Factories
            services.AddSingleton<ModListCardItemFactory>();
            services.AddSingleton<ModDependencyFactory>();
            services.AddSingleton<ModCardViewModelFactory>();
            services.AddSingleton<ModDependenciesViewModelFactory>();
            services.AddSingleton<ModDependencyViewModelFactory>();
            services.AddSingleton<ModDescriptionViewModelFactory>();
            services.AddSingleton<ModImagesViewModelFactory>();
            services.AddSingleton<ModListViewModelFactory>();
            services.AddSingleton<ModRatingsViewModelFactory>();
            services.AddSingleton<ModStatisticsViewModelFactory>();
            services.AddSingleton<ModSubscriptionViewModelFactory>();
            services.AddSingleton<ModTagsViewModelFactory>();
            services.AddSingleton<ProfileListViewModelFactory>();
            services.AddSingleton<ProfileListItemViewModelFactory>();
            services.AddSingleton<SideloadListViewModelFactory>();
            services.AddSingleton<SideloadListItemViewModelFactory>();
            services.AddSingleton<ModCreatorViewModelFactory>();
            services.AddSingleton<ModListListItemViewModelFactory>();
            services.AddSingleton<ModListListItemFactory>();

            // Main window and services
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ISnackbarService, SnackbarService>();
            services.AddSingleton<IContentDialogService, ContentDialogService>();

            // Custom services
            services.AddSingleton<SteamService>();
            services.AddSingleton<SelectedModService>();
            services.AddSingleton<SettingsService>();
            services.AddSingleton<ProfileService>();
            services.AddSingleton<InstallationService>();
            services.AddSingleton<DownloadService>();
            services.AddSingleton<SnackbarQueueService>();
            services.AddSingleton<DependenciesService>();
            services.AddSingleton<SideloadService>();
            services.AddSingleton<ModCachingService>();

            services.AddSingleton<RemoteRatingService>();
            services.AddSingleton<StubRatingService>();
            services.AddSingleton<IRatingService>(provider =>
            {
                ProfileService profileService = provider.GetRequiredService<ProfileService>();

                return profileService.SelectedProfileType switch
                {
                    ProfileType.Remote => provider.GetRequiredService<RemoteRatingService>(),
                    _ => provider.GetRequiredService<StubRatingService>()
                };
            });

            services.AddSingleton<RemoteSubscriptionService>();
            services.AddSingleton<LocalSubscriptionService>();
            services.AddSingleton<StubSubscriptionService>();
            services.AddSingleton<ISubscriptionService>(provider =>
            {
                ProfileService profileService = provider.GetRequiredService<ProfileService>();

                return profileService.SelectedProfileType switch
                {
                    ProfileType.Remote => provider.GetRequiredService<RemoteSubscriptionService>(),
                    ProfileType.Local => provider.GetRequiredService<LocalSubscriptionService>(),
                    _ => provider.GetRequiredService<StubSubscriptionService>()
                };
            });

            // Shared models
            services.AddSingleton<LoginModel>();

            // Pages and viewmodels
            services.AddSingleton<SettingsPage>().AddSingleton<SettingsViewModel>();
            services.AddSingleton<InitializationPage>().AddSingleton<InitializationViewModel>();
            services.AddSingleton<VerifyBepInExPage>().AddSingleton<VerifyBepInExViewModel>();
            services.AddSingleton<VerifyLoginPage>().AddSingleton<VerifyLoginViewModel>();
            services.AddSingleton<RequestLoginCodePage>().AddSingleton<RequestLoginCodeViewModel>();
            services.AddSingleton<EnterLoginCodePage>().AddSingleton<EnterLoginCodeViewModel>();
            services.AddSingleton<BrowsePluginsPage>();
            services.AddSingleton<BrowseBlueprintsPage>();
            services.AddSingleton<BrowseLibraryPage>();
            services.AddTransient<ModDetailsPage>().AddTransient<ModDetailsViewModel>();
            services.AddSingleton<ProfilesPage>();
            services.AddSingleton<SideloadPage>();
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
