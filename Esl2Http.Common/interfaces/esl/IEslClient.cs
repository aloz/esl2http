using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Esl2Http.Common.EslClientDelegates;

namespace Esl2Http.Common.Interfaces.Esl
{
    public interface IEslClient : IDisposable
    {
        public void Start(
            EslClientLogDelegate LogDelegate,
            EslClientResponseEslEventDelegate ResponseEslEventDelegate,
            string EslEventsList,
            int RxBufferSize);

        public void Stop();
    }
}
