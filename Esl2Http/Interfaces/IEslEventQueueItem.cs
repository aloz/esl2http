using System;

namespace Esl2Http.Interfaces
{
    interface IEslEventQueueItem
    {
        string EventContent { get; }
        DateTime Arrived { get; }
    }
}