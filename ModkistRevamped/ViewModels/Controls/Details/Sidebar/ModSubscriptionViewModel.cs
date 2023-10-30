using TNRD.Modkist.Services;
using TNRD.Modkist.Services.Subscription;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace TNRD.Modkist.ViewModels.Controls.Details.Sidebar;

public partial class ModSubscriptionViewModel : ObservableObject
{
    private readonly SelectedModService selectedModService;
    private readonly ISubscriptionService subscriptionService;
    private readonly SnackbarQueueService snackbarQueueService;
    private readonly DependenciesService dependenciesService;

    public ModSubscriptionViewModel(
        SelectedModService selectedModService,
        ISubscriptionService subscriptionService,
        SnackbarQueueService snackbarQueueService,
        DependenciesService dependenciesService
    )
    {
        this.selectedModService = selectedModService;
        this.subscriptionService = subscriptionService;
        this.snackbarQueueService = snackbarQueueService;
        this.dependenciesService = dependenciesService;

        UpdateView();
    }

    [ObservableProperty] private ControlAppearance appearance;
    [ObservableProperty] private string? content;
    [ObservableProperty] private bool canSubscribe;

    [RelayCommand]
    private async Task ToggleSubscription()
    {
        if (subscriptionService.IsSubscribed(selectedModService.SelectedMod!))
        {
            if (!await subscriptionService.Unsubscribe(selectedModService.SelectedMod!))
                return;

            snackbarQueueService.Enqueue("Unsubscribe",
                $"You have been unsubscribed from '{selectedModService.SelectedMod!.Name}'!");
        }
        else
        {
            if (!await subscriptionService.Subscribe(selectedModService.SelectedMod!))
                return;

            snackbarQueueService.Enqueue("Subscribe",
                $"You have been subscribed to '{selectedModService.SelectedMod!.Name}'!");
        }

        UpdateView();
    }

    private void UpdateView()
    {
        Appearance = subscriptionService.IsSubscribed(selectedModService.SelectedMod!)
            ? ControlAppearance.Secondary
            : ControlAppearance.Primary;

        Content = subscriptionService.IsSubscribed(selectedModService.SelectedMod!)
            ? "Unsubscribe"
            : "Subscribe";

        CanSubscribe = subscriptionService.IsSubscribed(selectedModService.SelectedMod!)
            ? !dependenciesService.IsDependency(selectedModService.SelectedMod!)
            : subscriptionService.CanSubscribe;
    }
}
