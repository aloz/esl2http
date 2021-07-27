using System;
using System.Collections.Generic;

namespace Esl2Http.Interfaces
{
    interface IDal : IDisposable
    {
        long? AddNewEvent(DateTime arrived, string jsonevent);

        string[] GetHttpHandlers();

        string[] GetHttpHandlersToRepost();

        Tuple<int?> GetConfig();

        List<Tuple<long, string>> GetEventsToPost(string url);

        List<Tuple<long, string>> GetEventsToRepost(string url);

        long? SetEventAsPosted(long event_id, string url, int? statuscode, string reason_phrase);

        bool? IsResendAvailable(string url);
    }
}