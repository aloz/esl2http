using Esl2Http.Interfaces;
using System;

namespace Esl2Http.Parts.EslEventQueue
{
    class EslEventQueueItem : IEslEventQueueItem
    {
        string _EventContent;
        DateTime _arrived;

        protected EslEventQueueItem() { }

        public EslEventQueueItem(string EventContent)
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
