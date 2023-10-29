using System.Windows.Media.Imaging;
using Modio.Models;
using TNRD.Modkist.Services;
using TNRD.Modkist.Views.Pages;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Visibility = System.Windows.Visibility;

namespace TNRD.Modkist.ViewModels.Controls;

public partial class ModCardViewModel : ObservableObject
{
    private readonly ImageCachingService imageCachingService;
    private readonly INavigationService navigationService;
    private readonly SelectedModService selectedModService;
    private readonly SubscriptionService subscriptionService;
    private readonly ISnackbarService snackbarService;
    private readonly Mod mod;

    public ModCardViewModel(
        ImageCachingService imageCachingService,
        INavigationService navigationService,
        SelectedModService selectedModService,
        SubscriptionService subscriptionService,
        ISnackbarService snackbarService,
        Mod mod
    )
    {
        this.imageCachingService = imageCachingService;
        this.navigationService = navigationService;
        this.selectedModService = selectedModService;
        this.subscriptionService = subscriptionService;
        this.snackbarService = snackbarService;
        this.mod = mod;

        Title = this.mod.Name!;
        UpdateView();
        LoadImage();
    }

    [ObservableProperty] private BitmapImage? imageSource;
    [ObservableProperty] private string title;
    [ObservableProperty] private Visibility progressVisibility = Visibility.Visible;
    [ObservableProperty] private ControlAppearance buttonAppearance;
    [ObservableProperty] private string? buttonContent;

    public Mod Mod => mod;

    private async void LoadImage()
    {
        ProgressVisibility = Visibility.Visible;
        string imagePath = await imageCachingService.GetImagePath(mod.Logo!.Thumb320x180!);
        ImageSource = new BitmapImage(new Uri(imagePath));
        ProgressVisibility = Visibility.Collapsed;
    }

    [RelayCommand]
    private void Clicked()
    {
        selectedModService.SetSelectedMod(mod, false);
        navigationService.NavigateWithHierarchy(typeof(ModDetailsPage));
    }

    [RelayCommand]
    private async Task ToggleSubscription()
    {
        if (subscriptionService.IsSubscribedToMod(mod))
        {
            await subscriptionService.Unsubscribe(mod);

            snackbarService.Show("Unsubscribe",
                $"You have been unsubscribed from '{mod.Name}'!",
                ControlAppearance.Secondary,
                null,
                TimeSpan.FromSeconds(2.5d));
        }
        else
        {
            await subscriptionService.Subscribe(mod);

            snackbarService.Show("Subscribe",
                $"You have been subscribed to '{mod.Name}'!",
                ControlAppearance.Secondary,
                null,
                TimeSpan.FromSeconds(2.5d));
        }

        UpdateView();
    }

    private void UpdateView()
    {
        ButtonAppearance = subscriptionService.IsSubscribedToMod(mod)
            ? ControlAppearance.Secondary
            : ControlAppearance.Primary;

        ButtonContent = subscriptionService.IsSubscribedToMod(mod)
            ? "Unsubscribe"
            : "Subscribe";
    }
}
