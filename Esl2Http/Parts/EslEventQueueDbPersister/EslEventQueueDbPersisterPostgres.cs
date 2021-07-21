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
                        string jsonevent = EventItem.EventContent;
                        bool IsHeartbeat;
                        if (IsEventContentValid(jsonevent, out IsHeartbeat))
                        {
                            if (!IsHeartbeat)
                                _dal.AddEvent(EventItem.Arrived, jsonevent);
                        }
                        else
                        {
                            if (!IsHeartbeat)
                                _LogDelegate($"Event content is invalid: {jsonevent}", EslClientLogType.Error);
                        }
                    }
                }
            }
            try { _dal.Dispose(); } catch { }
        }

        private bool IsEventContentValid(string jsonevent, out bool IsHeartbeat)
        {
            bool result = false;
            IsHeartbeat = false;
            if (!string.IsNullOrEmpty(jsonevent))
            {
                try
                {
                    JObject jobj = JObject.Parse(jsonevent);
                    IsHeartbeat = jobj["Event-Name"].Value<string>() == "HEARTBEAT";
                    result = true;
                }
                catch { }
            }
            return result;
        }

        #endregion

        public void Dispose()
        {
            base.StopWorker();
        }
    }
}