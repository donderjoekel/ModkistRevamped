﻿using TNRD.Modkist.Services;
using TNRD.Modkist.Services.Subscription;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace TNRD.Modkist.ViewModels.Controls.Details.Sidebar;

public partial class ModSubscriptionViewModel : ObservableObject
{
    private readonly SelectedModService selectedModService;
    private readonly ISubscriptionService subscriptionService;
    private readonly ISnackbarService snackbarService;

    public ModSubscriptionViewModel(
        SelectedModService selectedModService,
        ISubscriptionService subscriptionService,
        ISnackbarService snackbarService
    )
    {
        this.selectedModService = selectedModService;
        this.subscriptionService = subscriptionService;
        this.snackbarService = snackbarService;

        UpdateView();
    }

    [ObservableProperty] private ControlAppearance appearance;
    [ObservableProperty] private string? content;

    public bool CanSubscribe => subscriptionService.CanSubscribe;

    [RelayCommand]
    private async Task ToggleSubscription()
    {
        if (subscriptionService.IsSubscribed(selectedModService.SelectedMod!))
        {
            await subscriptionService.Unsubscribe(selectedModService.SelectedMod!);

            snackbarService.Show("Unsubscribe",
                $"You have been unsubscribed from '{selectedModService.SelectedMod!.Name}'!",
                ControlAppearance.Secondary,
                null,
                TimeSpan.FromSeconds(2.5d));
        }
        else
        {
            await subscriptionService.Subscribe(selectedModService.SelectedMod!);

            snackbarService.Show("Subscribe",
                $"You have been subscribed to '{selectedModService.SelectedMod!.Name}'!",
                ControlAppearance.Secondary,
                null,
                TimeSpan.FromSeconds(2.5d));
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
    }
}