using System.Windows.Controls;
using Modio.Models;
using TNRD.Modkist.Factories.ViewModels;
using TNRD.Modkist.ViewModels.Controls.Details.Dependencies;

namespace TNRD.Modkist.Views.Controls.Details.Dependencies;

public partial class ModDependency : UserControl
{
    public ModDependency(Dependency dependency)
    {
        ViewModel = App.GetService<ModDependencyViewModelFactory>().Create(dependency);
        DataContext = this;

        InitializeComponent();
    }

    public ModDependencyViewModel ViewModel { get; set; }
}
