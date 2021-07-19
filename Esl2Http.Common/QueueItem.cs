using System;

namespace Esl2Http.Common
{
    public class QueueItem
    {
        string _EventContent;
        DateTime _arrived;

        public QueueItem(string EventContent)
        {
            this.EventContent = EventContent;
            this.Arrived = DateTime.UtcNow;
        }

        public string EventContent
        {
            get
            {
                return _EventContent;
            }
            private set
            {
                _EventContent = value;
            }
        }

        public DateTime Arrived
        {
            get
            {
                return _arrived;
            }
            private set
            {
                _arrived = value;
            }
        }
    }
}
