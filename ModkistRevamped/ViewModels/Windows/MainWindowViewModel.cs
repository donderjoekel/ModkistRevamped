// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System.Collections.ObjectModel;
using TNRD.Modkist.Views.Controls;
using TNRD.Modkist.Views.Pages;
using Wpf.Ui.Controls;

namespace TNRD.Modkist.ViewModels.Windows;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty] private string _applicationTitle = "Modkist - Revamped";

    [ObservableProperty]
    private ObservableCollection<object> _menuItems = new()
    {
        new NavigationViewItem("Mods", SymbolRegular.WrenchScrewdriver24, typeof(BrowsePluginsPage)),
        new NavigationViewItem("Blueprints", SymbolRegular.PuzzlePiece24, typeof(BrowseBlueprintsPage)),
        new NavigationViewItemSeparator(),
        new NavigationViewItem("Installed", SymbolRegular.Library24, typeof(BrowseLibraryPage)),
        new NavigationViewItemSeparator(),
        new NavigationViewItem("Sideload", SymbolRegular.Toolbox24, typeof(SideloadPage)),
        new NavigationViewItemSeparator(),
        new NavigationViewItem("Profiles", SymbolRegular.Person24, typeof(ProfilesPage))
    };

    [ObservableProperty]
    private ObservableCollection<object> _footerMenuItems = new()
    {
        new PlayZeepkistNavigationViewItem(),
        new NavigationViewItemSeparator(),
        new NavigationViewItem("Settings", SymbolRegular.Settings24, typeof(SettingsPage))
    };

    [ObservableProperty]
    private ObservableCollection<MenuItem> _trayMenuItems = new()
    {
        new MenuItem { Header = "Home", Tag = "tray_home" }
    };
}
