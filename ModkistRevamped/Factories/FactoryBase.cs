using Microsoft.Extensions.DependencyInjection;

namespace TNRD.Modkist.Factories;

public abstract class FactoryBase<T>
{
    private readonly IServiceProvider provider;

    protected FactoryBase(IServiceProvider provider)
    {
        this.provider = provider;
    }

    public T Create()
    {
        return ActivatorUtilities.CreateInstance<T>(provider);
    }

    public T Create(params object[] args)
    {
        return ActivatorUtilities.CreateInstance<T>(provider, args);
    }
}
