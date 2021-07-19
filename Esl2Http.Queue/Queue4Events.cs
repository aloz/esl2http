using Esl2Http.Common;
using Esl2Http.Common.Interfaces;
using System.Collections.Generic;

namespace Esl2Http.Queue
{
    public class Queue4Events : IQueue
    {
        #region Private vars

        Queue<QueueItem> _queue;
        static readonly object _objSyncQueue = new object();

        #endregion

        #region ctor

        public Queue4Events()
        {
            _queue = new Queue<QueueItem>();
        }

        #endregion

        #region IQueue members

        public void Enqueue(string EventContent)
        {
            lock (_objSyncQueue)
            {
                lock (_objSyncQueue)
                {
                    _queue.Enqueue(new QueueItem(EventContent));
                }
            }
        }

        public QueueItem Dequeue()
        {
            QueueItem result = null;
            lock (_objSyncQueue)
            {
                lock (_objSyncQueue)
                {
                    if (_queue.Count > 0)
                        result = _queue.Dequeue();
                }
            }
            return result;
        }

        public int QueueCount
        {
            get
            {
                lock (_objSyncQueue)
                {
                    lock (_objSyncQueue)
                    {
                        return _queue.Count;
                    }
                }
            }
        }

        #endregion

        public void Dispose()
        {
            //TODO ???
            _queue.Clear();
        }
    }
}