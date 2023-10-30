using System.Windows.Controls;
using TNRD.Modkist.Factories.ViewModels;
using TNRD.Modkist.Models;
using TNRD.Modkist.ViewModels.Controls;

namespace TNRD.Modkist.Views.Controls;

public partial class SideloadListItem : UserControl
{
    public SideloadListItem(SideloadedModModel sideloadedMod)
    {
        ViewModel = App.GetService<SideloadListItemViewModelFactory>().Create(sideloadedMod);
        DataContext = this;

        InitializeComponent();
    }

    public SideloadListItemViewModel ViewModel { get; set; }
}
