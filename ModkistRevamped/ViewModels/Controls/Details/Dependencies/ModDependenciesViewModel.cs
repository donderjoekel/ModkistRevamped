using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using Modio;
using Modio.Models;
using TNRD.Modkist.Factories.Controls;
using TNRD.Modkist.Services;
using TNRD.Modkist.Views.Controls.Details.Dependencies;
using Visibility = System.Windows.Visibility;

namespace TNRD.Modkist.ViewModels.Controls.Details.Dependencies;

public partial class ModDependenciesViewModel : ObservableObject
{
    private readonly SelectedModService selectedModService;
    private readonly ModsClient modsClient;
    private readonly ModDependencyFactory modDependencyFactory;

    public ModDependenciesViewModel(
        SelectedModService selectedModService,
        ModsClient modsClient,
        ModDependencyFactory modDependencyFactory
    )
    {
        this.selectedModService = selectedModService;
        this.modsClient = modsClient;
        this.modDependencyFactory = modDependencyFactory;

        collectionView = CollectionViewSource.GetDefaultView(collection);
        LoadDependencies();
    }

    [ObservableProperty] private ObservableCollection<FrameworkElement> collection = new();
    [ObservableProperty] private ICollectionView collectionView;
    [ObservableProperty] private Visibility sectionVisibility = Visibility.Collapsed;

    private async void LoadDependencies()
    {
        SectionVisibility = Visibility.Collapsed;

        DependenciesClient dependenciesClient = modsClient[selectedModService.SelectedMod!.Id].Dependencies;
        IReadOnlyList<Dependency> dependencies = await dependenciesClient.Get();

        for (int i = 0; i < dependencies.Count; i++)
        {
            Dependency dependency = dependencies[i];

            if (i > 0)
            {
                Collection.Add(new ModDependencySeparator());
            }

            ModDependency modDependency = modDependencyFactory.Create(dependency);
            Collection.Add(modDependency);
        }

        if (dependencies.Count > 0)
        {
            SectionVisibility = Visibility.Visible;
        }
    }
}
