using System.Windows.Controls;

namespace TNRD.Modkist.Views.Controls.Details.Sidebar.ModInfoSection;

public partial class ModInfoSection : ContentControl
{
    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
        nameof(HeaderProperty),
        typeof(string),
        typeof(ModInfoSection),
        new PropertyMetadata(default(string)));

    public string Header
    {
        get => (string)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }
}
