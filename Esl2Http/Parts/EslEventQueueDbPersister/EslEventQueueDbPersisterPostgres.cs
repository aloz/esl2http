using Esl2Http.Base;
using Esl2Http.Dal;
using Esl2Http.Delegates;
using Esl2Http.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Esl2Http.Delegates.EslClientDelegates;

namespace Esl2Http.Parts.EslEventQueueDbPersister
{
    class EslEventQueueDbPersisterPostgres : BaseThreadWorker, IEslEventQueueDbPersister
    {
        #region Args from Start

        EslClientLogDelegate _LogDelegate;
        IEslEventQueue _queue;
        IDal _dal;

        #endregion

        #region IEslEventQueueDbPersister members

        public void Start(
            EslClientLogDelegate LogDelegate,
            IEslEventQueue queue,
            string ConnectionString)
        {
            _LogDelegate = LogDelegate;
            _queue = queue;
            _dal = new DalPostgres(ConnectionString);
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
            while (!IsEx)
            {
                while (_queue.QueueCount > 0)
                {
                    IEslEventQueueItem EventItem = _queue.Dequeue();
                    if (EventItem != null)
                    {
                        ulong? id= _dal.AddNewEvent(EventItem.Arrived, EventItem.EventContent);
                        if (!id.HasValue)
                            _LogDelegate($"Error to add event: {EventItem.EventContent}");
                    }
                }
            }
            try { _dal.Dispose(); } catch { }
        }
        #endregion

        public void Dispose()
        {
            base.StopWorker();
        }
    }
}