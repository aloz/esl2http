using System;

namespace Esl2Http.Interfaces
{
    interface IEslEventQueue : IDisposable
    {
        void Enqueue(string EventContent);

        IEslEventQueueItem Dequeue();

        int QueueCount
        {
            get;
        }
    }
}