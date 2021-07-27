using System;
using static Esl2Http.Delegates.Log;

namespace Esl2Http.Interfaces
{
    interface IEslEventQueueDbPersister : IDisposable
    {
        void Start(
            LogDelegate LogDelegate,
            IEslEventQueue queue,
            string ConnectionString);

        void Stop();
    }
}