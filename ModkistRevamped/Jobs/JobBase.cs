namespace TNRD.Modkist.Jobs;

public abstract class JobBase
{
    public abstract Task ExecuteAsync(CancellationToken cancellationToken);
}
