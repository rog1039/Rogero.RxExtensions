using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Xunit;

namespace Woeber.RxExtensions.Tests
{
    public class KeyedBufferTests
    {
        [Fact()]
        [Trait("Category", "Instant")]
        public void SimpleDelay()
        {
            var receivedSignals = new List<int>();
            var subject = new Subject<int>();
            var scheduler = new TestScheduler();
            subject.UniqueThrottle(TimeSpan.FromTicks(25), scheduler).Subscribe(x => receivedSignals.Add(x));


            subject.OnNext(1);
            receivedSignals.Count.Should().Be(0);

            scheduler.AdvanceTo(24);
            receivedSignals.Count.Should().Be(0);

            scheduler.AdvanceTo(25);
            receivedSignals.Count.Should().Be(1);

            scheduler.AdvanceTo(100);
            receivedSignals.Count.Should().Be(1);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void SimpleDelayWith2Inputs()
        {
            var receivedSignals = new List<int>();
            var subject = new Subject<int>();
            var scheduler = new TestScheduler();
            subject.UniqueThrottle(TimeSpan.FromTicks(25), scheduler).Subscribe(x => receivedSignals.Add(x));


            subject.OnNext(1);
            receivedSignals.Count.Should().Be(0);

            scheduler.AdvanceTo(10);
            subject.OnNext(2);
            subject.OnNext(2);
            subject.OnNext(2);
            subject.OnNext(1);
            subject.OnNext(1);
            subject.OnNext(1);

            scheduler.AdvanceTo(24);
            receivedSignals.Count.Should().Be(0);

            scheduler.AdvanceTo(25);
            receivedSignals.Count.Should().Be(1);

            scheduler.AdvanceTo(34);
            subject.OnNext(2);
            receivedSignals.Count.Should().Be(1);

            scheduler.AdvanceTo(35);
            receivedSignals.Count.Should().Be(2);

            scheduler.AdvanceTo(100);
            receivedSignals.Count.Should().Be(2);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void SimpleDelayWithDrop()
        {
            var receivedSignals = new List<int>();
            var subject = new Subject<int>();
            var scheduler = new TestScheduler();
            subject.UniqueThrottle(TimeSpan.FromTicks(25), scheduler).Subscribe(x => receivedSignals.Add(x));


            subject.OnNext(1);
            subject.OnNext(1);
            subject.OnNext(1);
            receivedSignals.Count.Should().Be(0);
            scheduler.AdvanceTo(24);
            subject.OnNext(1);
            subject.OnNext(1);
            subject.OnNext(1);
            receivedSignals.Count.Should().Be(0);

            scheduler.AdvanceTo(25);
            receivedSignals.Count.Should().Be(1);

            scheduler.AdvanceTo(50);
            subject.OnNext(1);
            receivedSignals.Count.Should().Be(1);

            scheduler.AdvanceTo(100);
            receivedSignals.Count.Should().Be(2);
        }
    }

    public class KeyedBufferObservableTests
    {
        [Fact()]
        [Trait("Category", "Instant")]
        public void SimpleDelay()
        {
            var receivedSignals = new List<int>();
            var subject = new Subject<int>();
            var scheduler = new TestScheduler();
            subject.UniqueThrottle(25.Ticks(), scheduler).Subscribe(receivedSignals.Add);

            subject.OnNext(1);
            receivedSignals.Count.Should().Be(0);

            scheduler.AdvanceBy(24);
            receivedSignals.Count.Should().Be(0);

            scheduler.AdvanceBy(1);
            receivedSignals.Count.Should().Be(1);

            scheduler.AdvanceBy(100);
            receivedSignals.Count.Should().Be(1);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void SimpleDelayWithDrop()
        {
            var receivedSignals = new List<int>();
            var subject = new Subject<int>();
            var scheduler = new TestScheduler();
            subject.UniqueThrottle(25.Ticks(), scheduler).Subscribe(receivedSignals.Add);

            subject.OnNext(1);
            subject.OnNext(1);
            subject.OnNext(1);
            receivedSignals.Count.Should().Be(0);
            scheduler.AdvanceBy(24);
            subject.OnNext(1);
            subject.OnNext(1);
            subject.OnNext(1);
            receivedSignals.Count.Should().Be(0);

            scheduler.AdvanceBy(1);
            receivedSignals.Count.Should().Be(1);

            scheduler.AdvanceBy(100);
            receivedSignals.Count.Should().Be(1);
        }
    }
}
