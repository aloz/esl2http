using System;

namespace Esl2Http.Interfaces
{
    interface IDal : IDisposable
    {
        // TODO
        ulong? AddEvent(DateTime arrived, string jsonevent);
    }
}