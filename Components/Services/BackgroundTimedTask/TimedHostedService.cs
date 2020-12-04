using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace dotNet_base.Components.Services.BackgroundTimedTask
{
    internal class TimedHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        public IBackgroundTimedTask TaskQueue { get; }
        private Timer _timer;

        public TimedHostedService(IBackgroundTimedTask taskQueue,
            ILoggerFactory loggerFactory)
        {
            TaskQueue = taskQueue;
            _logger = loggerFactory.CreateLogger<TimedHostedService>();
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
                    _timer = new Timer(async _ => {
                        await workItem(cancellationToken);
                        if (_timer != null) await _timer.DisposeAsync();

                        _logger.LogInformation($"{DateTime.Now}: {workItem} executed successfully");
                    }, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(-1));
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