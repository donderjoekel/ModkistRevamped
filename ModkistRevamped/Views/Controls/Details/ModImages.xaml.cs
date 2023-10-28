using System.Windows.Controls;
using TNRD.Modkist.Services;
using ModImagesViewModel = TNRD.Modkist.ViewModels.Controls.Details.ModImagesViewModel;

namespace TNRD.Modkist.Views.Controls.Details;

public partial class ModImages : UserControl
{
    public ModImages()
    {
        ViewModel = new ModImagesViewModel(App.GetService<ImageCachingService>(),
            App.GetService<SelectedModService>());
        DataContext = this;

        InitializeComponent();
    }

    public ModImagesViewModel ViewModel { get; set; }
}
