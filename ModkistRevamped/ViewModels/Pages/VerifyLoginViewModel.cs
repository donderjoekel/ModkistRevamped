using TNRD.Modkist.Services;
using TNRD.Modkist.Views.Pages;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace TNRD.Modkist.ViewModels.Pages;

public class VerifyLoginViewModel : ObservableObject, INavigationAware
{
    private readonly SettingsService settingsService;
    private readonly INavigationService navigationService;
    private readonly SubscriptionService subscriptionService;
    private readonly RatingService ratingService;

    public VerifyLoginViewModel(
        SettingsService settingsService,
        INavigationService navigationService,
        SubscriptionService subscriptionService,
        RatingService ratingService
    )
    {
        this.settingsService = settingsService;
        this.navigationService = navigationService;
        this.subscriptionService = subscriptionService;
        this.ratingService = ratingService;
    }

    async void INavigationAware.OnNavigatedTo()
    {
        if (settingsService.HasValidAccessToken() || settingsService.SkippedLogin)
        {
            if (settingsService.HasValidAccessToken())
            {
                await ratingService.LoadAuthenticatedUserData();
                await subscriptionService.LoadAuthenticatedUserData();
            }
            else
            {
                await Task.Delay(Random.Shared.Next(1000, 1500)); // Artificial delay to make it feel better    
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
