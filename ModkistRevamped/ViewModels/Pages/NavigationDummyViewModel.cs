using Wpf.Ui.Controls;

namespace TNRD.Modkist.ViewModels.Pages;

public class NavigationDummyViewModel : INavigationAware
{
    public delegate void NavigatedToDelegate();

    public delegate void NavigatedFromDelegate();

    public event NavigatedToDelegate? NavigatedTo;
    public event NavigatedFromDelegate? NavigatedFrom;

    public void OnNavigatedTo()
    {
        NavigatedTo?.Invoke();
    }

    public void OnNavigatedFrom()
    {
        NavigatedFrom?.Invoke();
    }
}
