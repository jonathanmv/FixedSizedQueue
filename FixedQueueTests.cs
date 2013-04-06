using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jonathanmv.Foundation.Test
{
    [TestClass]
    public class FixedQueueTests
    {
        private const int QueueCapacity = 2;
        public FixedQueue<object> Target { get; set; }
        
        [TestInitialize]
        public void TestInitialize()
        {
            Target = new FixedQueue<object>(QueueCapacity);
        }

        [TestMethod]
        public void ClearWithoutDispatchingEventTest()
        {
            FillQueue();

            Target.Dequeued += (object sender, object e) =>
            {
                //Fail is the dequeued event is dispatched
                Assert.Fail();
            };

            Target.Clear();
            Assert.AreEqual(0, Target.Count);
        }

        [TestMethod]
        public void ClearDispatchingEventTest()
        {
            FillQueue();

            int objectsToDequeue = QueueCapacity;
            Target.Dequeued += (object sender, object e) =>
            {
                objectsToDequeue--;
            };

            Target.Clear(true);
            Assert.AreEqual(0, Target.Count);
            Assert.AreEqual(0, objectsToDequeue);
        }

        [TestMethod]
        public void EnqueueDipatchingEvent()
        {
            FillQueue();

            var objectToEnqueue = new object();
            int objectsDequeued = 0;
            Target.Dequeued += (object sender, object e) =>
            {
                objectsDequeued++;
            };

            var dequeued = Target.Enqueue(objectToEnqueue);
            Assert.IsNotNull(dequeued);
            Assert.AreNotEqual(objectToEnqueue, dequeued);
            Assert.AreEqual(1, objectsDequeued);
            Assert.AreEqual(QueueCapacity, Target.Count);
        }

        [TestMethod]
        public void DequeueDipatchingEvent()
        {
            FillQueue();

            int objectsDequeued = 0;
            Target.Dequeued += (object sender, object e) =>
            {
                objectsDequeued++;
            };
            var dequeued = Target.Dequeue();
            Assert.IsNotNull(dequeued);
            Assert.AreEqual(1, objectsDequeued);
            Assert.AreEqual(QueueCapacity - 1, Target.Count);
        }

        private void FillQueue()
        {
            for (int i = 0; i < QueueCapacity; i++)
            {
                Target.Enqueue(new object());
            }
            Assert.AreEqual(Target.Count, QueueCapacity);
        }
    }
}
