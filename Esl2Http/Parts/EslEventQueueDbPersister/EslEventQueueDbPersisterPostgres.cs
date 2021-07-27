using Esl2Http.Base;
using Esl2Http.Dal;
using Esl2Http.Interfaces;
using System;
using static Esl2Http.Delegates.Log;

namespace Esl2Http.Parts.EslEventQueueDbPersister
{
    class EslEventQueueDbPersisterPostgres : BaseThreadWorker, IEslEventQueueDbPersister
    {
        #region Args from Start

        LogDelegate _LogDelegate;
        IEslEventQueue _queue;
        string _ConnectionString;

        #endregion

        #region IEslEventQueueDbPersister members

        public void Start(
            LogDelegate LogDelegate,
            IEslEventQueue queue,
            string ConnectionString)
        {
            _LogDelegate = LogDelegate;
            _queue = queue;
            _ConnectionString = ConnectionString;
            base.StartWorker();
        }

        public void Stop()
        {
            base.StopWorker();
        }

        #endregion

        #region BaseThreadWorker members

        protected override void Worker()
        {
            using (IDal dal = new DalPostgres(_ConnectionString))
            {
                while (!IsEx)
                {
                    try
                    {
                        while (_queue.QueueCount > 0)
                        {
                            IEslEventQueueItem EventItem = _queue.Dequeue();
                            if (EventItem != null)
                            {
                                long? id = dal.AddNewEvent(EventItem.Arrived, EventItem.EventContent);
                                if (!id.HasValue)
                                    _LogDelegate(this.GetType(), $"Error to add event: {EventItem.EventContent}");
                            }
                            _LogDelegate(this.GetType(), $"{_queue.QueueCount} event(s) in memory Queue");
                        }
                    }
                    catch (Exception ex)
                    {
                        _LogDelegate(this.GetType(), ex.ToString(), LogType.Error);
                        System.Threading.Thread.Sleep(1000);
                    }
                }
            }
        }

        #endregion

        public void Dispose()
        {
            base.StopWorker();
        }
    }
}