using TNRD.Modkist.Services;
using TNRD.Modkist.Views.Pages;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace TNRD.Modkist.ViewModels.Pages;

public class VerifyLoginViewModel : ObservableObject, INavigationAware
{
    private readonly SettingsService settingsService;
    private readonly INavigationService navigationService;

    public VerifyLoginViewModel(SettingsService settingsService, INavigationService navigationService)
    {
        this.settingsService = settingsService;
        this.navigationService = navigationService;
    }

    async void INavigationAware.OnNavigatedTo()
    {
        Type pageToNavigateTo = settingsService.HasValidAccessToken()
            ? typeof(BrowsePluginsPage)
            : typeof(RequestLoginCodePage);

        await Task.Delay(Random.Shared.Next(1000, 1500)); // Artificial delay to make it feel better

        navigationService.Navigate(pageToNavigateTo);
    }

    void INavigationAware.OnNavigatedFrom()
    {
        // Left empty on purpose
    }
}
