using System;
using System.Threading;

namespace Esl2Http.Common
{
    public abstract class BaseThreadWorker
    {
        Thread _thWorker;

        bool _isEx;
        static readonly object _objSyncIsEx = new object();

        protected void StartWorker()
        {
            if (_thWorker != null && _thWorker.IsAlive)
                throw new InvalidOperationException("Can't start the worker");

            _thWorker = new Thread(Worker);
            _thWorker.Start();
        }

        protected void StopWorker()
        {
            if (_thWorker == null || (_thWorker != null && !_thWorker.IsAlive))
                throw new InvalidOperationException("Can't stop the worker");

            this.IsEx = true;
            _thWorker.Join(8 * 1000);
        }

        protected abstract void Worker();

        protected bool IsEx
        {
            get
            {
                lock (_objSyncIsEx)
                {
                    lock (_objSyncIsEx)
                    {
                        return _isEx;
                    }
                }
            }
            set
            {
                lock (_objSyncIsEx)
                {
                    lock (_objSyncIsEx)
                    {
                        _isEx = value;
                    }
                }
            }
        }

    }
}
