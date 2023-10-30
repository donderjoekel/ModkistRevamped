using TNRD.Modkist.ViewModels.Windows;

namespace TNRD.Modkist.Views.Windows;

public partial class InputBox
{
    public InputBox(string labelText = "", bool hasPlaceholder = false, string placeholderText = "")
    {
        ViewModel = new InputBoxViewModel(this, labelText, hasPlaceholder, placeholderText);
        DataContext = this;

        InitializeComponent();
    }

    public InputBoxViewModel ViewModel { get; set; }
}
