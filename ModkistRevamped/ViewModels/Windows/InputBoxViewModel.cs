using TNRD.Modkist.Views.Windows;

namespace TNRD.Modkist.ViewModels.Windows;

public partial class InputBoxViewModel : ObservableObject
{
    private readonly InputBox inputBox;

    public InputBoxViewModel(InputBox inputBox, string labelText, bool hasPlaceholder, string placeholderText)
    {
        this.inputBox = inputBox;
        LabelText = labelText;
        HasPlaceholder = hasPlaceholder;
        PlaceholderText = placeholderText;

        OkButtonEnabled = false;
    }

    [ObservableProperty] private string? input;
    [ObservableProperty] private bool okButtonEnabled;

    public string? LabelText { get; private set; }
    public bool HasPlaceholder { get; private set; }
    public string? PlaceholderText { get; private set; }

    [RelayCommand]
    private void Cancel()
    {
        inputBox.DialogResult = false;
    }

    [RelayCommand]
    private void Ok()
    {
        inputBox.DialogResult = true;
    }

    partial void OnInputChanged(string? value)
    {
        OkButtonEnabled = !string.IsNullOrWhiteSpace(value);
    }
}
