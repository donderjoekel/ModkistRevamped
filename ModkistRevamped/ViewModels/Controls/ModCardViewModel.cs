using System.Windows.Media.Imaging;
using Modio.Models;
using TNRD.Modkist.Services;
using TNRD.Modkist.Services.Subscription;
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
    private readonly ISubscriptionService subscriptionService;
    private readonly SnackbarQueueService snackbarQueueService;
    private readonly DependenciesService dependenciesService;

    public ModCardViewModel(
        ImageCachingService imageCachingService,
        INavigationService navigationService,
        SelectedModService selectedModService,
        ISubscriptionService subscriptionService,
        SnackbarQueueService snackbarQueueService,
        DependenciesService dependenciesService,
        Mod mod
    )
    {
        this.imageCachingService = imageCachingService;
        this.navigationService = navigationService;
        this.selectedModService = selectedModService;
        this.subscriptionService = subscriptionService;
        this.snackbarQueueService = snackbarQueueService;
        this.dependenciesService = dependenciesService;

        this.subscriptionService.SubscriptionAdded += OnSubscriptionAdded;
        this.subscriptionService.SubscriptionRemoved += OnSubscriptionRemoved;

        this.dependenciesService.DependencyAdded += OnDependencyAdded;
        this.dependenciesService.DependencyRemoved += OnDependencyRemoved;

        Mod = mod;
        Title = Mod.Name!;
        UpdateView();
        LoadImage();
    }

    [ObservableProperty] private BitmapImage? imageSource;
    [ObservableProperty] private string title;
    [ObservableProperty] private Visibility progressVisibility = Visibility.Visible;
    [ObservableProperty] private ControlAppearance buttonAppearance;
    [ObservableProperty] private string? buttonContent;
    [ObservableProperty] private bool canSubscribe;

    public Mod Mod { get; }

    private async void LoadImage()
    {
        ProgressVisibility = Visibility.Visible;
        string imagePath = await imageCachingService.GetImagePath(Mod.Logo!.Thumb320x180!);
        ImageSource = new BitmapImage(new Uri(imagePath));
        ProgressVisibility = Visibility.Collapsed;
    }

    [RelayCommand]
    private void Clicked()
    {
        selectedModService.SetSelectedMod(Mod, false);
        navigationService.NavigateWithHierarchy(typeof(ModDetailsPage));
    }

    [RelayCommand]
    private async Task ToggleSubscription()
    {
        if (subscriptionService.IsSubscribed(Mod))
        {
            if (!await subscriptionService.Unsubscribe(Mod))
                return;

            snackbarQueueService.Enqueue("Unsubscribe", $"You have been unsubscribed from '{Mod.Name}'!");
        }
        else
        {
            if (!await subscriptionService.Subscribe(Mod))
                return;

            snackbarQueueService.Enqueue("Subscribe", $"You have been subscribed to '{Mod.Name}'!");
        }

        UpdateView();
    }

    private void UpdateView()
    {
        ButtonAppearance = subscriptionService.IsSubscribed(Mod)
            ? ControlAppearance.Secondary
            : ControlAppearance.Primary;

        ButtonContent = subscriptionService.IsSubscribed(Mod)
            ? "Unsubscribe"
            : "Subscribe";

        CanSubscribe = subscriptionService.IsSubscribed(Mod)
            ? !dependenciesService.IsDependency(Mod)
            : subscriptionService.CanSubscribe;
    }

    private void OnSubscriptionAdded(uint modId)
    {
        if (Mod.Id != modId)
            return;

        UpdateView();
    }

    private void OnSubscriptionRemoved(uint modId)
    {
        if (Mod.Id != modId)
            return;

        UpdateView();
    }

    private void OnDependencyAdded(uint dependencyId)
    {
        if (Mod.Id != dependencyId)
            return;

        UpdateView();
    }

    private void OnDependencyRemoved(uint dependencyId)
    {
        if (Mod.Id != dependencyId)
            return;

        UpdateView();
    }
}
