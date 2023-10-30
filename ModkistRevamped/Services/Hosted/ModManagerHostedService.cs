using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

    private void OnSubscriptionAdded(uint modId)
    {
        EnqueueAddModJob(modId);
    }

    private void OnSubscriptionRemoved(uint modId)
    {
        EnqueueRemoveModJob(modId);
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

    public void EnqueueAddModJob(uint modId)
    {
        AddModJob addModJob = ActivatorUtilities.CreateInstance<AddModJob>(serviceProvider, modId);
        jobs.Enqueue(addModJob);
    }

    public void EnqueueRemoveModJob(uint modId)
    {
        RemoveModJob removeModJob = ActivatorUtilities.CreateInstance<RemoveModJob>(serviceProvider, modId);
        jobs.Enqueue(removeModJob);
    }

    public void EnqueueUpdateModJob(uint modId)
    {
        UpdateModJob updateModJob = ActivatorUtilities.CreateInstance<UpdateModJob>(serviceProvider, modId);
        jobs.Enqueue(updateModJob);
    }
}
