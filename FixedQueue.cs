using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagleEye.Foundation
{
    /// <summary>
    /// A fixed size queue that returns the dequeued object if the queue is full when a new object
    /// is added to the queue. Please notice that null values are not supported
    /// </summary>
    /// <typeparam name="T">The type of the objects to be enqueued</typeparam>
    public class FixedQueue<T> : IQueryable<T> where T : class, new()
    {
        private int _size;
        private Queue<T> _queue;

        /// <summary>
        /// Gets the number of elements contained in the queue.
        /// </summary>
        public int Count { get { return _queue.Count; } }

        /// <summary>
        /// Dispatched when an object has been dequeued
        /// </summary>
        public event EventHandler<T> Dequeued;

        /// <summary>
        /// Initializes a new instance of the FixedQueue class that is empty and has the specified capacity.
        /// </summary>
        /// <param name="capacity"></param>
        public FixedQueue(int capacity)
        {
            if (capacity < 1) throw new ArgumentOutOfRangeException("Capacity has to be at least 1");
            _size = capacity;
            _queue = new Queue<T>(capacity);
        }

        /// <summary>
        /// Enqueues an object and returns the dequeued object if the queue was full.
        /// It dispatches the dequeued event if an object was dequeued
        /// </summary>
        /// <param name="obj">Object to enqueue</param>
        /// <returns>The dequeued object if the queue was full. Null otherwise. Please notice that 
        /// the returned object is not the same being enqueued</returns>
        public T Enqueue(T obj)
        {
            if (obj == null) throw new ArgumentNullException("This queue does not support null values");

            T dequeued = null;
            if (_queue.Count == _size)
            {
                dequeued = Dequeue();
            }
            _queue.Enqueue(obj);

            return dequeued;
        }

        /// <summary>
        /// Dequeues the first object in the queue if any. Dispatches a Dequeued event if an object
        /// was dequeued
        /// </summary>
        /// <returns>The dequeued object if any or null</returns>
        public T Dequeue()
        {
            if (_queue.Count == 0) return null;

            T dequeued = _queue.Dequeue();
            WhenObjectDequeued(dequeued);
            return dequeued;
        }

        /// <summary>
        /// Removes all the objects from the queue optionally dispatching the Dequeued event if 
        /// it was specified so.
        /// </summary>
        /// <param name="dispatchEvent">Whether or not to dispatche the Deque</param>
        public void Clear(bool dispatchEvent = false)
        {
            if (Count == 0) return;

            if (!dispatchEvent)
            {
                _queue.Clear();
                return;
            }

            while(Count>0)
            {
                Dequeue();
            }
        }

        private void WhenObjectDequeued(T dequeued)
        {
            EventHandler<T> handler = Dequeued;
            if (handler != null)
                handler(this, dequeued);
        }

        #region IQueryable<T> implementation

        public IEnumerator<T> GetEnumerator()
        {
            return _queue.AsQueryable().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _queue.AsQueryable().GetEnumerator();
        }

        public Type ElementType
        {
            get { return _queue.AsQueryable().ElementType; }
        }

        public System.Linq.Expressions.Expression Expression
        {
            get { return _queue.AsQueryable().Expression; }
        }

        public IQueryProvider Provider
        {
            get { return _queue.AsQueryable().Provider; }
        }

        #endregion
    }
}
