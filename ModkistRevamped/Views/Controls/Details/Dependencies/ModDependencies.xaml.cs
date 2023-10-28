using System.Windows.Controls;
using Modio;
using TNRD.Modkist.Factories.Controls;
using TNRD.Modkist.Services;
using ModDependenciesViewModel = TNRD.Modkist.ViewModels.Controls.Details.Dependencies.ModDependenciesViewModel;

namespace TNRD.Modkist.Views.Controls.Details.Dependencies;

public partial class ModDependencies : UserControl
{
    public ModDependencies()
    {
        ViewModel = new ModDependenciesViewModel(App.GetService<SelectedModService>(),
            App.GetService<ModsClient>(),
            App.GetService<ModDependencyFactory>());
        DataContext = this;

        InitializeComponent();
    }

    public ModDependenciesViewModel ViewModel { get; set; }
}
