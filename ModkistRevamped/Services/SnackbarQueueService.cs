using System.Windows.Controls;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace TNRD.Modkist.Services;

public class SnackbarQueueService
{
    private readonly ISnackbarService snackbarService;

    public SnackbarQueueService(ISnackbarService snackbarService)
    {
        this.snackbarService = snackbarService;
    }

    public void Enqueue(
        string title,
        string message,
        ControlAppearance appearance = ControlAppearance.Secondary,
        SymbolIcon? icon = null,
        TimeSpan timeout = default
    )
    {
        SnackbarPresenter snackbarPresenter = snackbarService.GetSnackbarPresenter();
        Snackbar snackbar = new(snackbarPresenter);
        snackbar.SetCurrentValue(Snackbar.TitleProperty, title);
        snackbar.SetCurrentValue(ContentControl.ContentProperty, message);
        snackbar.SetCurrentValue(Snackbar.AppearanceProperty, appearance);
        snackbar.SetCurrentValue(Snackbar.IconProperty, icon);
        snackbar.SetCurrentValue(Snackbar.TimeoutProperty,
            timeout.TotalSeconds == 0.0 ? snackbarService.DefaultTimeOut : timeout);

        snackbarPresenter.AddToQue(snackbar);
    }

    public void EnqueueRateLimitMessage()
    {
        Enqueue("Oops!",
            "You've been rate limited for a minute, please try again later.",
            ControlAppearance.Danger,
            new SymbolIcon(SymbolRegular.ErrorCircle24));
    }
}
