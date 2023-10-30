using TNRD.Modkist.Services;
using TNRD.Modkist.Services.Rating;
using TNRD.Modkist.Services.Subscription;
using TNRD.Modkist.Views.Pages;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace TNRD.Modkist.ViewModels.Pages;

public class VerifyLoginViewModel : ObservableObject, INavigationAware
{
    private readonly SettingsService settingsService;
    private readonly INavigationService navigationService;
    private readonly ISubscriptionService subscriptionService;
    private readonly IRatingService ratingService;

    public VerifyLoginViewModel(
        SettingsService settingsService,
        INavigationService navigationService,
        ISubscriptionService subscriptionService,
        IRatingService ratingService
    )
    {
        this.settingsService = settingsService;
        this.navigationService = navigationService;
        this.subscriptionService = subscriptionService;
        this.ratingService = ratingService;
    }

    async void INavigationAware.OnNavigatedTo()
    {
        await ratingService.Initialize();
        await subscriptionService.Initialize();

        if (settingsService.HasValidAccessToken() || settingsService.SkippedLogin)
        {
            if (settingsService.SkippedLogin)
            {
                await Task.Delay(Random.Shared.Next(1000, 1500)); // Artificial delay to make it feel better    
            }
            else
            {
                await Task.Delay(Random.Shared.Next(250, 500)); // Artificial delay to make it feel better
            }

            navigationService.Navigate(typeof(BrowsePluginsPage));
        }
        else
        {
            await Task.Delay(Random.Shared.Next(1000, 1500)); // Artificial delay to make it feel better
            navigationService.Navigate(typeof(RequestLoginCodePage));
        }
    }

    void INavigationAware.OnNavigatedFrom()
    {
        // Left empty on purpose
    }
}
