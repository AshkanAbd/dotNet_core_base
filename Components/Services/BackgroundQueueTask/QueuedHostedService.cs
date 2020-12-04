using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace dotNet_base.Components.Services.BackgroundQueueTask
{
    public class QueuedHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        public IBackgroundTaskQueue TaskQueue { get; }

        public QueuedHostedService(IBackgroundTaskQueue taskQueue,
            ILoggerFactory loggerFactory)
        {
            TaskQueue = taskQueue;
            _logger = loggerFactory.CreateLogger<QueuedHostedService>();
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Queued Hosted Service is running");

            await BackgroundProcessing(cancellationToken);
        }

        private async Task BackgroundProcessing(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested) {
                var workItem = await TaskQueue.DequeueAsync(cancellationToken);

                try {
                    await workItem(cancellationToken);
                    
                    // new Timer(async _ => await workItem(cancellationToken), null,
                    //     TimeSpan.Zero, TimeSpan.FromMilliseconds(-1));
                    
                    _logger.LogInformation($"{DateTime.Now}: {workItem} executed successfully");
                }
                catch (Exception ex) {
                    _logger.LogError(ex, $"Error occurred executing {workItem}.", nameof(workItem));
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Queued Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}