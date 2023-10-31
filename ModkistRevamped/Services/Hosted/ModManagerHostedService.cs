using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modio.Models;
using TNRD.Modkist.Jobs;
using TNRD.Modkist.Services.Subscription;

namespace TNRD.Modkist.Services.Hosted;

public class ModManagerHostedService : BackgroundService
{
    private readonly ISubscriptionService subscriptionService;
    private readonly IServiceProvider serviceProvider;
    private readonly ConcurrentQueue<JobBase> jobs = new();

    public ModManagerHostedService(ISubscriptionService subscriptionService, IServiceProvider serviceProvider)
    {
        this.subscriptionService = subscriptionService;
        this.serviceProvider = serviceProvider;

        this.subscriptionService.SubscriptionsLoaded += OnSubscriptionsLoaded;
        this.subscriptionService.SubscriptionAdded += OnSubscriptionAdded;
        this.subscriptionService.SubscriptionRemoved += OnSubscriptionRemoved;
    }

    private void OnSubscriptionsLoaded()
    {
        VerifyModsJob verifyModsJob = ActivatorUtilities.CreateInstance<VerifyModsJob>(serviceProvider);
        jobs.Enqueue(verifyModsJob);
    }

    private void OnSubscriptionAdded(Mod mod)
    {
        EnqueueAddModJob(mod);
    }

    private void OnSubscriptionRemoved(Mod mod)
    {
        EnqueueRemoveModJob(mod);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (jobs.IsEmpty)
            {
                await Task.Delay(1000, stoppingToken);
            }
            else
            {
                if (!jobs.TryDequeue(out JobBase? job))
                {
                    await Task.Delay(100, stoppingToken);
                    continue;
                }

                await job.ExecuteAsync(stoppingToken);
            }
        }
    }

    public void EnqueueAddModJob(Mod mod)
    {
        AddModJob addModJob = ActivatorUtilities.CreateInstance<AddModJob>(serviceProvider, mod);
        jobs.Enqueue(addModJob);
    }

    public void EnqueueAddModsJob(Mod[] mods)
    {
        AddModsJob addModsJob = ActivatorUtilities.CreateInstance<AddModsJob>(serviceProvider,
            new object[] { mods });
        jobs.Enqueue(addModsJob);
    }

    public void EnqueueRemoveModJob(Mod mod)
    {
        RemoveModJob removeModJob = ActivatorUtilities.CreateInstance<RemoveModJob>(serviceProvider, mod);
        jobs.Enqueue(removeModJob);
    }

    public void EnqueueRemoveModsJob(Mod[] mods)
    {
        RemoveModsJob removeModsJob =
            ActivatorUtilities.CreateInstance<RemoveModsJob>(serviceProvider, new object[] { mods });
        jobs.Enqueue(removeModsJob);
    }

    public void EnqueueUpdateModJob(Mod mod)
    {
        UpdateModJob updateModJob = ActivatorUtilities.CreateInstance<UpdateModJob>(serviceProvider, mod);
        jobs.Enqueue(updateModJob);
    }

    public void EnqueueUpdateModsJob(Mod[] mods)
    {
        UpdateModsJob updateModsJob =
            ActivatorUtilities.CreateInstance<UpdateModsJob>(serviceProvider, new object[] { mods });
        jobs.Enqueue(updateModsJob);
    }
}
