using System.Windows.Controls;
using TNRD.Modkist.Factories.ViewModels;
using TNRD.Modkist.Models;
using TNRD.Modkist.ViewModels.Controls;

namespace TNRD.Modkist.Views.Controls;

public partial class SideloadList : UserControl
{
    public static readonly DependencyProperty ModTypeProperty = DependencyProperty.Register(
        nameof(ModType),
        typeof(ModType),
        typeof(SideloadList),
        new PropertyMetadata(default(ModType),
            (o, args) => { (o as SideloadList)!.ViewModel.ModType = (ModType)args.NewValue; }));

    public ModType ModType
    {
        get => (ModType)GetValue(ModTypeProperty);
        set => SetValue(ModTypeProperty, value);
    }

    public SideloadList()
    {
        ViewModel = App.GetService<SideloadListViewModelFactory>().Create();
        DataContext = this;

        InitializeComponent();
    }

    public SideloadListViewModel ViewModel { get; set; }
}
