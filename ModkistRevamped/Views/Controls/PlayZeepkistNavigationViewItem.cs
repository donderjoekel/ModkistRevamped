using System.Diagnostics;
using Wpf.Ui.Controls;

namespace TNRD.Modkist.Views.Controls;

public class PlayZeepkistNavigationViewItem : NavigationViewItem
{
    public PlayZeepkistNavigationViewItem()
        : base("Zeepkist", SymbolRegular.Play24, null!)
    {
    }

    protected override void OnClick()
    {
        base.OnClick();

        ProcessStartInfo processStartInfo = new()
        {
            FileName = "cmd.exe",
            Arguments = "/c start steam://rungameid/1440670",
            CreateNoWindow = true
        };

        Process process = new();
        process.StartInfo = processStartInfo;
        process.Start();

        Application.Current.Shutdown();
    }
}
