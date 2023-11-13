using System.Windows.Media;
using System.Windows.Media.Imaging;
using Modio;
using Modio.Models;
using TNRD.Modkist.Services;
using TNRD.Modkist.Services.Subscription;
using TNRD.Modkist.Views.Pages;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Visibility = System.Windows.Visibility;

namespace TNRD.Modkist.ViewModels.Controls.Details.Dependencies;

public partial class ModDependencyViewModel : ObservableObject
{
    private readonly ImageCachingService imageCachingService;
    private readonly INavigationService navigationService;
    private readonly SelectedModService selectedModService;
    private readonly ModsClient modsClient;
    private readonly Dependency dependency;
    private readonly Mod mod;

    public ModDependencyViewModel(
        ImageCachingService imageCachingService,
        INavigationService navigationService,
        SelectedModService selectedModService,
        ModsClient modsClient,
        Dependency dependency,
        ModCachingService modCachingService,
        ISubscriptionService subscriptionService,
        DependenciesService dependenciesService
    )
    {
        this.imageCachingService = imageCachingService;
        this.navigationService = navigationService;
        this.selectedModService = selectedModService;
        this.modsClient = modsClient;
        this.dependency = dependency;

        mod = modCachingService[dependency.ModId];
        Name = mod.Name;

        SubscribeButtonEnabled = subscriptionService.IsSubscribed(mod.Id)
            ? !dependenciesService.IsDependency(mod)
            : subscriptionService.CanSubscribe;

        SubscribeButtonText = subscriptionService.IsSubscribed(mod.Id)
            ? "Unsubscribe"
            : "Subscribe";

        SubscribeButtonAppearance = subscriptionService.IsSubscribed(mod.Id)
            ? ControlAppearance.Secondary
            : ControlAppearance.Primary;

        LoadDependency();
    }

    [ObservableProperty] private string? name;
    [ObservableProperty] private ImageSource? image;
    [ObservableProperty] private Visibility progressVisibility;
    [ObservableProperty] private ControlAppearance subscribeButtonAppearance;
    [ObservableProperty] private string? subscribeButtonText;
    [ObservableProperty] private bool subscribeButtonEnabled;

    private async void LoadDependency()
    {
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
