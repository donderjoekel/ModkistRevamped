// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using TNRD.Modkist.Services;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace TNRD.Modkist.ViewModels.Pages;

public partial class SettingsViewModel : ObservableObject, INavigationAware
{
    private readonly SettingsService settingsService;

    [ObservableProperty] private string _appVersion = string.Empty;

    public SettingsViewModel(SettingsService settingsService)
    {
        this.settingsService = settingsService;
    }

    public ApplicationTheme Theme
    {
        get => settingsService.Theme;
        private set => settingsService.Theme = value;
    }

    public string SteamDirectory
    {
        get => settingsService.ZeepkistDirectory;
        set => settingsService.ZeepkistDirectory = value;
    }

    public void OnNavigatedTo()
    {
        AppVersion = $"ModkistRevamped - {GetAssemblyVersion()}";
    }

    public void OnNavigatedFrom()
    {
    }

    private string GetAssemblyVersion()
    {
        return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString()
               ?? string.Empty;
    }

    [RelayCommand]
    private void OnChangeTheme(string parameter)
    {
        switch (parameter)
        {
            case "theme_light":
                SwitchToTheme(ApplicationTheme.Light);
                break;
            default:
                SwitchToTheme(ApplicationTheme.Dark);
                break;
        }
    }

    private void SwitchToTheme(ApplicationTheme theme)
    {
        if (Theme == theme)
            return;

        ApplicationThemeManager.Apply(theme);
        Theme = theme;
    }
}
