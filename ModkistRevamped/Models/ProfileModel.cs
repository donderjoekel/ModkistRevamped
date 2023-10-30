namespace TNRD.Modkist.Models;

public class ProfileModel
{
    public string Id { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public ProfileType Type { get; set; }
}
