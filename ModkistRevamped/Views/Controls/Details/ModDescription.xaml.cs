using System.Windows.Controls;
using TNRD.Modkist.Services;
using TNRD.Modkist.ViewModels.Controls.Details;

namespace TNRD.Modkist.Views.Controls.Details;

public partial class ModDescription : UserControl
{
    public ModDescription()
    {
        ViewModel = new ModDescriptionViewModel(App.GetService<SelectedModService>());
        DataContext = this;

        InitializeComponent();
    }

    public ModDescriptionViewModel ViewModel { get; set; }
}
