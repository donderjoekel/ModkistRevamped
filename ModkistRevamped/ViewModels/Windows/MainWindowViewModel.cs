// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System.Collections.ObjectModel;
using TNRD.Modkist.Views.Pages;
using Wpf.Ui.Controls;

namespace TNRD.Modkist.ViewModels.Windows;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty] private string _applicationTitle = "Modkist - Revamped";

    [ObservableProperty]
    private ObservableCollection<object> _menuItems = new()
    {
        new NavigationViewItem()
        {
            Content = "Mods",
            Icon = new SymbolIcon { Symbol = SymbolRegular.AppsAddIn24 },
            TargetPageType = typeof(BrowsePluginsPage)
        },
        new NavigationViewItem()
        {
            Content = "Blueprints",
            Icon = new SymbolIcon { Symbol = SymbolRegular.Cube24 },
            TargetPageType = typeof(BrowseBlueprintsPage)
        },
        // new NavigationViewItem()
        // {
        //     Content = "Guides",
        //     Icon = new SymbolIcon { Symbol = SymbolRegular.Book24 },
        //     TargetPageType = typeof(BrowseGuidesPage)
        // },
        new NavigationViewItemSeparator()
        // new NavigationViewItem()
        // {
        //     Content = "Installed Mods",
        //     Icon = new SymbolIcon { Symbol = SymbolRegular.Apps24 },
        //     TargetPageType = typeof(InstalledModsPage)
        // }
    };

    [ObservableProperty]
    private ObservableCollection<object> _footerMenuItems = new()
    {
        new NavigationViewItem()
        {
            Content = "Settings",
            Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
            TargetPageType = typeof(SettingsPage)
        }
    };

    [ObservableProperty]
    private ObservableCollection<MenuItem> _trayMenuItems = new()
    {
        new MenuItem { Header = "Home", Tag = "tray_home" }
    };
}
