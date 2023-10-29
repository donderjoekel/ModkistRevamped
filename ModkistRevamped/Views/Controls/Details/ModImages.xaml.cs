using System.Windows.Controls;
using TNRD.Modkist.Factories.ViewModels;
using TNRD.Modkist.ViewModels.Controls.Details;

namespace TNRD.Modkist.Views.Controls.Details;

public partial class ModImages : UserControl
{
    public ModImages()
    {
        ViewModel = App.GetService<ModImagesViewModelFactory>().Create();
        DataContext = this;

        InitializeComponent();
    }

    public ModImagesViewModel ViewModel { get; set; }
}
