using Microsoft.UI.Dispatching;
using System;
using System.Threading.Tasks;

namespace kafi.Helpers;

/// <summary>
/// Helper methods for working with the DispatcherQueue
/// </summary>
public static class DispatcherQueueExtensions
{
    /// <summary>
    /// Executes the given action on the dispatcher queue asynchronously
    /// </summary>
    public static Task EnqueueAsync(this DispatcherQueue dispatcher, Func<Task> action)
    {
        var taskCompletionSource = new TaskCompletionSource();

        if (!dispatcher.TryEnqueue(async () =>
        {
            try
            {
                await action();
                taskCompletionSource.SetResult();
            }
            catch (Exception ex)
            {
                taskCompletionSource.SetException(ex);
            }
        }))
        {
            taskCompletionSource.SetException(new InvalidOperationException("Failed to enqueue task on dispatcher"));
        }

        return taskCompletionSource.Task;
    }
}