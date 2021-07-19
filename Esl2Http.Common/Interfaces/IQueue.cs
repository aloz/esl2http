using System;

namespace Esl2Http.Common.Interfaces
{
    public interface IQueue : IDisposable
    {
        void Enqueue(string EventContent);

        QueueItem Dequeue();

        int QueueCount
        {
            get;
        }
    }
}
