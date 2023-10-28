using System.Windows.Media;
using System.Windows.Media.Imaging;
using TNRD.Modkist.Services;
using Wpf.Ui.Controls;
using Image = Modio.Models.Image;

namespace TNRD.Modkist.ViewModels.Controls.Details;

public partial class ModImagesViewModel : ObservableObject
{
    private readonly ImageCachingService imageCachingService;
    private readonly SelectedModService selectedModService;
    private readonly List<Uri> images = new();

    public ModImagesViewModel(ImageCachingService imageCachingService, SelectedModService selectedModService)
    {
        this.imageCachingService = imageCachingService;
        this.selectedModService = selectedModService;

        InitializeImages();
        LoadImage();
    }

    [ObservableProperty] private ImageSource? image;
    [ObservableProperty] private bool shouldAnimate;
    [ObservableProperty] private Visibility progressVisibility;

    private int selectedImageIndex;

    private bool mouseOverImage;
    private bool mouseOverButton;

    private bool IsMouseOver => mouseOverButton || mouseOverImage;

#pragma warning disable CA1822
    // ReSharper disable once MemberCanBeMadeStatic.Global
    public SymbolFilled IconLeft => SymbolFilled.CaretLeft24;

    // ReSharper disable once MemberCanBeMadeStatic.Global
    public SymbolFilled IconRight => SymbolFilled.CaretRight24;
#pragma warning restore CA1822

    private void InitializeImages()
    {
        images.Clear();
        images.Add(selectedModService.SelectedMod!.Logo!.Thumb1280x720!);
        foreach (Image mediaImage in selectedModService.SelectedMod!.Media.Images)
        {
            if (mediaImage.Original != null)
                images.Add(mediaImage.Original);
            else if (mediaImage.Thumb320x180 != null)
                images.Add(mediaImage.Thumb320x180);
        }
    }

    private async void LoadImage()
    {
        ProgressVisibility = Visibility.Visible;
        Image = null;

        string imagePath = await imageCachingService.GetImagePath(images[selectedImageIndex]);

        ProgressVisibility = Visibility.Collapsed;
        Image = new BitmapImage(new Uri(imagePath));
    }

    [RelayCommand]
    private void ImageMouseEnter()
    {
        mouseOverImage = true;
        ShouldAnimate = IsMouseOver;
    }

    [RelayCommand]
    private void ImageMouseLeave()
    {
        mouseOverImage = false;
        ShouldAnimate = IsMouseOver;
    }

    [RelayCommand]
    private void ButtonMouseEnter()
    {
        mouseOverButton = true;
        ShouldAnimate = IsMouseOver;
    }

    [RelayCommand]
    private void ButtonMouseLeave()
    {
        mouseOverButton = false;
        ShouldAnimate = IsMouseOver;
    }

    [RelayCommand]
    private void PreviousImage()
    {
        selectedImageIndex = (selectedImageIndex - 1 + images.Count) % images.Count;
        LoadImage();
    }

    [RelayCommand]
    private void NextImage()
    {
        selectedImageIndex = (selectedImageIndex + 1) % images.Count;
        LoadImage();
    }
}
