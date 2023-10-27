// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System.Windows.Media;
using TNRD.Modkist.Models;
using Wpf.Ui.Controls;

namespace TNRD.Modkist.ViewModels.Pages;

public partial class DataViewModel : ObservableObject, INavigationAware
{
    private bool _isInitialized = false;

    [ObservableProperty] private IEnumerable<DataColor> _colors;

    public void OnNavigatedTo()
    {
        if (!_isInitialized)
            InitializeViewModel();
    }

    public void OnNavigatedFrom()
    {
    }

    private void InitializeViewModel()
    {
        Random? random = new();
        List<DataColor>? colorCollection = new();

        for (int i = 0; i < 8192; i++)
        {
            colorCollection.Add(
                new DataColor
                {
                    Color = new SolidColorBrush(
                        Color.FromArgb(
                            (byte)200,
                            (byte)random.Next(0, 250),
                            (byte)random.Next(0, 250),
                            (byte)random.Next(0, 250)
                        )
                    )
                }
            );
        }

        Colors = colorCollection;

        _isInitialized = true;
    }
}
