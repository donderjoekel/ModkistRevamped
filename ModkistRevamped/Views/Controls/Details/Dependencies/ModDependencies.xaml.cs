using System.Windows.Controls;
using TNRD.Modkist.Factories.ViewModels;
using TNRD.Modkist.ViewModels.Controls.Details.Dependencies;

namespace TNRD.Modkist.Views.Controls.Details.Dependencies;

public partial class ModDependencies : UserControl
{
    public ModDependencies()
    {
        ViewModel = App.GetService<ModDependenciesViewModelFactory>().Create();
        DataContext = this;

        InitializeComponent();
    }

    public ModDependenciesViewModel ViewModel { get; set; }
}
