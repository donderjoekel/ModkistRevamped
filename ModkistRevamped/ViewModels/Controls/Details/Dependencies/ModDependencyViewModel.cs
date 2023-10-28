using System.Windows.Media;
using System.Windows.Media.Imaging;
using Modio;
using Modio.Models;
using TNRD.Modkist.Services;
using TNRD.Modkist.Views.Pages;
using Wpf.Ui;
using Visibility = System.Windows.Visibility;

namespace TNRD.Modkist.ViewModels.Controls.Details.Dependencies;

public partial class ModDependencyViewModel : ObservableObject
{
    private readonly ImageCachingService imageCachingService;
    private readonly INavigationService navigationService;
    private readonly SelectedModService selectedModService;
    private readonly ModsClient modsClient;
    private readonly Dependency dependency;

    public ModDependencyViewModel(
        ImageCachingService imageCachingService,
        INavigationService navigationService,
        SelectedModService selectedModService,
        ModsClient modsClient,
        Dependency dependency
    )
    {
        this.imageCachingService = imageCachingService;
        this.navigationService = navigationService;
        this.selectedModService = selectedModService;
        this.modsClient = modsClient;
        this.dependency = dependency;

        LoadDependency();
    }

    [ObservableProperty] private string? name;
    [ObservableProperty] private ImageSource? image;
    [ObservableProperty] private Visibility progressVisibility;

    private Mod? mod;

    private async void LoadDependency()
    {
        mod = await modsClient[dependency.ModId].Get();
        Name = mod.Name;
        await LoadImage();
    }

    private async Task LoadImage()
    {
        ProgressVisibility = Visibility.Visible;
        Image = null;

        string imagePath = await imageCachingService.GetImagePath(mod!.Logo!.Thumb320x180!);

        Image = new BitmapImage(new Uri(imagePath));
        ProgressVisibility = Visibility.Collapsed;
    }

    [RelayCommand]
    private void Clicked()
    {
        selectedModService.SetSelectedMod(mod, true);
        // TODO: This needs to be a different page as you cannot show the same page twice in a row :(
        navigationService.NavigateWithHierarchy(typeof(ModDetailsPage));
    }

    [RelayCommand]
    private void SubscribeClicked()
    {
    }
}
