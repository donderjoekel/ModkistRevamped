using System.Windows.Controls;
using TNRD.Modkist.Factories.ViewModels;
using TNRD.Modkist.ViewModels.Controls;

namespace TNRD.Modkist.Views.Controls;

public partial class SideloadList : UserControl
{
    public SideloadList()
    {
        ViewModel = App.GetService<SideloadListViewModelFactory>().Create();
        DataContext = this;

        InitializeComponent();
    }

    public SideloadListViewModel ViewModel { get; set; }
}
