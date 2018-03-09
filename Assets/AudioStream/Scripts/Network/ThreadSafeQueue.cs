// (c) 2016-2018 Martin Cvengros. All rights reserved. Redistribution of source code without permission not allowed.

using System.Collections.Generic;

namespace AudioStream
{
    public class ThreadSafeQueue<T>
    {
        object @lock = new object();
        Queue<T> queue = new Queue<T>();

        // TODO: make customizable
        /// <summary>
        /// 100 should be around a second or so of audio so should be enough
        /// </summary>
        const int maxCapacity = 100;

        public T Dequeue()
        {
            lock (this.@lock)
            {
                if (this.queue.Count > 0)
                    return this.queue.Dequeue();
                else
                    return default(T);
            }
        }

        public void Enqueue(T value)
        {
            // start dropping packets after max capacity limit is reached
            if (this.queue.Count > maxCapacity)
                return;

            lock (this.@lock)
            {
                this.queue.Enqueue(value);
            }
        }

        public int Size()
        {
            lock (this.@lock)
            {
                return this.queue.Count;
            }
        }
    }
}