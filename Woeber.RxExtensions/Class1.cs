using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;

namespace Woeber.RxExtensions
{
    public class KeyedBuffer<T>
    {
        private readonly TimeSpan _delay;
        private readonly IScheduler _scheduler;
        private readonly ConcurrentDictionary<T, DateTimeOffset> _monitors = new ConcurrentDictionary<T, DateTimeOffset>();

        public KeyedBuffer(TimeSpan delay, IScheduler scheduler)
        {
            _delay = delay;
            _scheduler = scheduler;
        }

        public void ReceiveInput(T input)
        {
            if (IsMonitored(input))
                return;
            BeginMonitor(input);
        }

        private bool IsMonitored(T input)
        {
            return _monitors.ContainsKey(input);
        }

        private void BeginMonitor(T input)
        {
            var timeToSignal = _scheduler.Now.Add(_delay);
            _monitors.AddOrUpdate(input, timeToSignal, (arg1, offset) => offset);
            ScheduleWatch();
        }

        private void ScheduleWatch()
        {
            _scheduler.Schedule(_delay, Watch);
        }

        private void Watch()
        {
            DateTimeOffset _;
            var expiredMontiors = _monitors.Where(kvp => kvp.Value < _scheduler.Now);
            foreach (var kvp in expiredMontiors)
            {
                SendSignal(kvp.Key);
                _monitors.TryRemove(kvp.Key, out _);
            }
        }

        private void SendSignal(T key)
        {
            
        }
    }

    public static class KeyedBufferExtensionMethods
    {
        public static IObservable<T> KeyedBuffer<T>(this IObservable<T> observable, TimeSpan delay, IScheduler scheduler = default(IScheduler))
        {
            var keyedObservable = Observable.Create<T>(o =>
            {
                scheduler = scheduler ?? Scheduler.Default;
                var keyedBuffer = new KeyedBuffer<T>(delay, scheduler);
                var subscription = observable.Subscribe(
                    input => keyedBuffer.ReceiveInput(input), 
                    o.OnError, 
                    o.OnCompleted);
                return () => subscription.Dispose();
            });
            return keyedObservable;
        }
    }
}
