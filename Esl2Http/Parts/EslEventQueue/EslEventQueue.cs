using Esl2Http.Interfaces;
using System.Collections.Generic;

namespace Esl2Http.Parts.EslEventQueue
{
    class EslEventQueue : IEslEventQueue
    {
        #region Private vars

        Queue<EslEventQueueItem> _queue;
        static readonly object _objSyncQueue = new object();

        #endregion

        #region ctor

        public EslEventQueue()
        {
            _queue = new Queue<EslEventQueueItem>();
        }

        #endregion

        #region EslEventQueue members

        public void Enqueue(string EventContent)
        {
            lock (_objSyncQueue)
            {
                lock (_objSyncQueue)
                {
                    _queue.Enqueue(new EslEventQueueItem(EventContent));
                }
            }
        }

        public IEslEventQueueItem Dequeue()
        {
            EslEventQueueItem result = null;
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