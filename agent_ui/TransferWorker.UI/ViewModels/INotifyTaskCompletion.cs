using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Nito.AsyncEx
{
    //
    // Summary:
    //     Watches a task and raises property-changed notifications when the task completes.
    public interface INotifyTaskCompletion : INotifyPropertyChanged
    {
        //
        // Summary:
        //     Gets the task being watched. This property never changes and is never null.
        Task Task { get; }
        //
        // Summary:
        //     Gets a task that completes successfully when Nito.AsyncEx.INotifyTaskCompletion.Task
        //     completes (successfully, faulted, or canceled). This property never changes and
        //     is never null.
        Task TaskCompleted { get; }
        //
        // Summary:
        //     Gets the current task status. This property raises a notification when the task
        //     completes.
        TaskStatus Status { get; }
        //
        // Summary:
        //     Gets whether the task has completed. This property raises a notification when
        //     the value changes to true.
        bool IsCompleted { get; }
        //
        // Summary:
        //     Gets whether the task is busy (not completed). This property raises a notification
        //     when the value changes to false.
        bool IsNotCompleted { get; }
        //
        // Summary:
        //     Gets whether the task has completed successfully. This property raises a notification
        //     when the value changes to true.
        bool IsSuccessfullyCompleted { get; }
        //
        // Summary:
        //     Gets whether the task has been canceled. This property raises a notification
        //     only if the task is canceled (i.e., if the value changes to true).
        bool IsCanceled { get; }
        //
        // Summary:
        //     Gets whether the task has faulted. This property raises a notification only if
        //     the task faults (i.e., if the value changes to true).
        bool IsFaulted { get; }
        //
        // Summary:
        //     Gets the wrapped faulting exception for the task. Returns null if the task is
        //     not faulted. This property raises a notification only if the task faults (i.e.,
        //     if the value changes to non-null).
        AggregateException Exception { get; }
        //
        // Summary:
        //     Gets the original faulting exception for the task. Returns null if the task is
        //     not faulted. This property raises a notification only if the task faults (i.e.,
        //     if the value changes to non-null).
        Exception InnerException { get; }
        //
        // Summary:
        //     Gets the error message for the original faulting exception for the task. Returns
        //     null if the task is not faulted. This property raises a notification only if
        //     the task faults (i.e., if the value changes to non-null).
        string ErrorMessage { get; }
    }
}