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
}
