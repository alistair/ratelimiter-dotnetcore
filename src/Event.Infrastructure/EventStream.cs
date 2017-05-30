namespace Event.Infrastructure
{
    using System.Collections.Generic;

    public class EventStream {
        public long StreamVersion;

        public List<IEvent> Events = new List<IEvent>();
    }
}
