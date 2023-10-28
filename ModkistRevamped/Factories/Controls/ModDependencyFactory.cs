using Microsoft.Extensions.DependencyInjection;
using Modio.Models;
using TNRD.Modkist.Views.Controls.Details.Dependencies;

namespace TNRD.Modkist.Factories.Controls;

public class ModDependencyFactory
{
    private readonly IServiceProvider provider;

    public ModDependencyFactory(IServiceProvider provider)
    {
        this.provider = provider;
    }

    public ModDependency Create(Dependency dependency)
    {
        return ActivatorUtilities.CreateInstance<ModDependency>(provider, dependency);
    }
}
