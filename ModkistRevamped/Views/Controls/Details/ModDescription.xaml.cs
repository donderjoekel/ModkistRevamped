using System.Windows.Controls;
using TNRD.Modkist.Factories.ViewModels;
using TNRD.Modkist.ViewModels.Controls.Details;

namespace TNRD.Modkist.Views.Controls.Details;

public partial class ModDescription : UserControl
{
    public ModDescription()
    {
        ViewModel = App.GetService<ModDescriptionViewModelFactory>().Create();
        DataContext = this;

        InitializeComponent();
    }

    public ModDescriptionViewModel ViewModel { get; set; }
}
