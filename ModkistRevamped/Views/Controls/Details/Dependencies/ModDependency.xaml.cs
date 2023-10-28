using System.Windows.Controls;
using Modio;
using Modio.Models;
using TNRD.Modkist.Services;
using Wpf.Ui;
using ModDependencyViewModel = TNRD.Modkist.ViewModels.Controls.Details.Dependencies.ModDependencyViewModel;

namespace TNRD.Modkist.Views.Controls.Details.Dependencies;

public partial class ModDependency : UserControl
{
    public ModDependency(
        ImageCachingService imageCachingService,
        INavigationService navigationService,
        SelectedModService selectedModService,
        ModsClient modsClient,
        Dependency dependency
    )
    {
        ViewModel = new ModDependencyViewModel(imageCachingService,
            navigationService,
            selectedModService,
            modsClient,
            dependency);
        DataContext = this;

        InitializeComponent();
    }

    public ModDependencyViewModel ViewModel { get; set; }
}
