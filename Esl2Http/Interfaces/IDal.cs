using System;

namespace Esl2Http.Interfaces
{
    interface IDal : IDisposable
    {
        // TODO
        ulong? AddNewEvent(DateTime arrived, string jsonevent);
    }
}