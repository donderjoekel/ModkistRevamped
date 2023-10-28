using Microsoft.Extensions.DependencyInjection;
using Modio.Models;
using TNRD.Modkist.Views.Controls;

namespace TNRD.Modkist.Factories.Controls;

public class ModCardFactory
{
    private readonly IServiceProvider provider;

    public ModCardFactory(IServiceProvider provider)
    {
        this.provider = provider;
    }

    public ModCard Create(Mod mod)
    {
        return ActivatorUtilities.CreateInstance<ModCard>(provider, mod);
    }
}
