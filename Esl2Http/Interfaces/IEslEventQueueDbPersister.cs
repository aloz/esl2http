using Esl2Http.Base;
using System;
using static Esl2Http.Delegates.EslClientDelegates;

namespace Esl2Http.Interfaces
{
    interface IEslEventQueueDbPersister : IDisposable
    {
        void Start(
            EslClientLogDelegate LogDelegate,
            IEslEventQueue queue,
            string ConnectionString);

        void Stop();
    }
}