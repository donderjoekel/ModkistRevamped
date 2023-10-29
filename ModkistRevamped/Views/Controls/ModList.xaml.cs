using System.Windows.Controls;
using TNRD.Modkist.Factories.ViewModels;
using TNRD.Modkist.Models;
using TNRD.Modkist.ViewModels.Controls;

namespace TNRD.Modkist.Views.Controls;

public partial class ModList : UserControl
{
    public static readonly DependencyProperty ModTypeProperty = DependencyProperty.Register(
        nameof(ModType),
        typeof(ModType),
        typeof(ModList),
        new PropertyMetadata(default(ModType), ModTypeChanged));

    private static void ModTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ModList modList)
            return;

        modList.ViewModel.ModType = (ModType)e.NewValue;
    }

    public ModList()
    {
        ViewModel = App.GetService<ModListViewModelFactory>().Create();
        DataContext = this;

        InitializeComponent();
    }

    public ModListViewModel ViewModel { get; set; }

    public ModType ModType
    {
        get => (ModType)GetValue(ModTypeProperty);
        set => SetValue(ModTypeProperty, value);
    }
}
