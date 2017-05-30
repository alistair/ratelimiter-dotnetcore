using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Event.Infrastructure
{
    public interface IEventStore
    {
        Task<EventStream> LoadEventStream(IIdentity id);
        Task AppendEventsToStream(IIdentity id, long version, IEnumerable<IEvent> events);
    }

    public interface IEvent {
    }

    public interface ICommand {

    }
}
