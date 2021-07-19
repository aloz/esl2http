using System;
using static Esl2Http.Common.EslClientDelegates;

namespace Esl2Http.Common.Interfaces
{
    public interface IEslClient : IDisposable
    {
        void Start(
            EslClientLogDelegate LogDelegate,
            EslClientResponseEslEventDelegate ResponseEslEventDelegate,
            string EslEventsList,
            int RxBufferSize);

        void Stop();
    }
}