using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Woeber.RxExtensions.Tests
{
    public class KeyedBufferTests
    {
        [Fact()]
        [Trait("Category", "Instant")]
        public void MethodName()
        {
            var receivedSignals = new List<int>();
            var subject = new Subject<int>();
            subject.KeyedBuffer(TimeSpan.FromTicks(25)).Subscribe(x => receivedSignals.Add(x));
            subject.Throttle()
            subject.
        }
    }
}
