using System;
using System.Threading;
using System.Threading.Tasks;

namespace dotNet_base.Components.Services.BackgroundQueueTask
{
    public interface IBackgroundTaskQueue
    {
        void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);

        Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }
}