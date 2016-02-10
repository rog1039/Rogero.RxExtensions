using System;
using System.Collections.Concurrent;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Woeber.RxExtensions
{
    public class UniqueThrottle<T>
    {
        private readonly TimeSpan _delay;
        private readonly IScheduler _scheduler;
        private readonly Action<T> _sendSignal;
        private readonly ConcurrentDictionary<T,int> _delayedSignals = new ConcurrentDictionary<T, int>(); 

        public UniqueThrottle(TimeSpan delay, IScheduler scheduler, Action<T> sendSignal)
        {
            _delay = delay;
            _scheduler = scheduler;
            _sendSignal = sendSignal;
        }

        public void ReceiveInput(T input)
        {
            if (IsMonitored(input))
                return;
            BeginMonitor(input);
        }

        private bool IsMonitored(T input)
        {
            return _delayedSignals.ContainsKey(input);
        }

        private void BeginMonitor(T input)
        {
            _delayedSignals.TryAdd(input, 1);
            ScheduleSignal(input);
        }

        private void ScheduleSignal(T input)
        {
            _scheduler.Schedule(_delay, () => Signal(input));
        }

        private void Signal(T input)
        {
            int _;
            _delayedSignals.TryRemove(input, out _);
            SendSignal(input);
        }

        private void SendSignal(T key)
        {
            _sendSignal(key);
        }
    }

    public static class UniqueThrottleExtensionMethods
    {
        public static IObservable<T> UniqueThrottle<T>(this IObservable<T> observable, TimeSpan delay, IScheduler scheduler = default(IScheduler))
        {
            var keyedObservable = Observable.Create<T>(o =>
            {
                scheduler = scheduler ?? Scheduler.Default;
                var keyedBuffer = new UniqueThrottle<T>(
                    delay, 
                    scheduler, 
                    o.OnNext);
                var subscription = observable.Subscribe(
                    input => keyedBuffer.ReceiveInput(input), 
                    o.OnError, 
                    o.OnCompleted);
                return () =>
                {
                    subscription.Dispose();
                };
            });
            return keyedObservable;
        }
    }
}
