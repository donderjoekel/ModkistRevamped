// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System.Diagnostics;
using System.Reflection;
using TNRD.Modkist.Services;
using TNRD.Modkist.Views.Pages;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace TNRD.Modkist.ViewModels.Pages;

public partial class SettingsViewModel : ObservableObject, INavigationAware
{
    private readonly SettingsService settingsService;
    private readonly INavigationService navigationService;

    public SettingsViewModel(SettingsService settingsService, INavigationService navigationService)
    {
        this.settingsService = settingsService;
        this.navigationService = navigationService;

        UpdateView();
    }

    [ObservableProperty] private string appVersion = string.Empty;
    [ObservableProperty] private ControlAppearance accountButtonAppearance;
    [ObservableProperty] private string accountButtonContent = null!;

    public void OnNavigatedTo()
    {
        UpdateView();
    }

    private void UpdateView()
    {
        AppVersion = $"Modkist - Revamped | {GetAssemblyVersion()}";

        AccountButtonAppearance = settingsService.HasValidAccessToken()
            ? ControlAppearance.Caution
            : ControlAppearance.Primary;

        AccountButtonContent = settingsService.HasValidAccessToken()
            ? "Sign out"
            : "Sign in";
    }

    public void OnNavigatedFrom()
    {
    }

    private string GetAssemblyVersion()
    {
        return Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty;
    }

    [RelayCommand]
    private void AccountButtonClicked()
    {
        if (settingsService.HasValidAccessToken())
        {
            settingsService.AccessToken = null;
            settingsService.SelectedProfile = string.Empty;
            settingsService.SkippedLogin = true;

            Process.Start(Environment.ProcessPath!);
            Application.Current.Shutdown();
        }
        else
        {
            settingsService.SkippedLogin = false;
            navigationService.Navigate(typeof(VerifyLoginPage));
        }
    }
}
