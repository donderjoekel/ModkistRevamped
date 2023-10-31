using System.Windows.Media;
using System.Windows.Media.Imaging;
using Modio.Models;
using TNRD.Modkist.Services;

namespace TNRD.Modkist.ViewModels.Controls.Details.Sidebar;

public partial class ModCreatorViewModel : ObservableObject
{
    private readonly SelectedModService selectedModService;
    private readonly ImageCachingService imageCachingService;

    public ModCreatorViewModel(SelectedModService selectedModService, ImageCachingService imageCachingService)
    {
        this.selectedModService = selectedModService;
        this.imageCachingService = imageCachingService;

        User user = selectedModService.SelectedMod!.SubmittedBy!;
        Author = user.Username!;

        LoadImage();
    }

    [ObservableProperty] private string author;
    [ObservableProperty] private ImageSource? icon;

    private async void LoadImage()
    {
        User user = selectedModService.SelectedMod!.SubmittedBy!;

        Uri? uri = null;

        if (user.Avatar?.Thumb100x100 != null)
            uri = user.Avatar?.Thumb100x100;
        else if (user.Avatar?.Thumb50x50 != null)
            uri = user.Avatar?.Thumb50x50;
        else if (user.Avatar?.Original != null)
            uri = user.Avatar?.Original;

        if (uri == null)
            return;

        string imagePath = await imageCachingService.GetImagePath(uri);
        Icon = new BitmapImage(new Uri(imagePath));
    }
}
