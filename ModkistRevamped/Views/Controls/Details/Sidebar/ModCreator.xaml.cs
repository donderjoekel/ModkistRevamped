using System.Windows.Controls;
using TNRD.Modkist.Factories.ViewModels;
using TNRD.Modkist.ViewModels.Controls.Details.Sidebar;

namespace TNRD.Modkist.Views.Controls.Details.Sidebar;

public partial class ModCreator : UserControl
{
    public ModCreator()
    {
        ViewModel = App.GetService<ModCreatorViewModelFactory>().Create();
        DataContext = this;

        InitializeComponent();
    }

    public ModCreatorViewModel ViewModel { get; set; }
}
