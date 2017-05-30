using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Event.Infrastructure
{
    public class GetEventStore : IEventStore
    {
        private readonly IEventStoreConnection _eventStoreConnection;
        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = new[] { new StringEnumConverter() }
        };

        public GetEventStore(IEventStoreConnection eventStoreConnection)
        {
            _eventStoreConnection = eventStoreConnection;
        }

        public Task AppendEventsToStream(IIdentity id, long version, IEnumerable<IEvent> events)
        {
            var newEvents = events.Select(x => (
                                                id: Guid.NewGuid(),
                                                type: x.GetType(),
                                                data: Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(x, _settings))
                                                ))
                .Select(x => new EventData(x.id, x.type.FullName, true, x.data, null));

            return _eventStoreConnection.AppendToStreamAsync($"{id.GetTag()}/{id.GetId()}", (int)version, newEvents);
        }

        public async Task<EventStream> LoadEventStream(IIdentity id)
        {
            var streamEvents = await _eventStoreConnection.ReadStreamEventsForwardAsync(
                                   $"{id.GetTag()}/{id.GetId()}", 0, int.MaxValue, false);

            return new EventStream
            {
                Events = streamEvents.Events.Select(x => (
                                                          type: x.Event.EventType,
                                                          data: Encoding.UTF8.GetString(x.Event.Data)
                                                          )
                                                    )
                    .Select(_ => (IEvent)JsonConvert.DeserializeObject(_.data, Type.GetType(_.type), _settings)).ToList(),
                StreamVersion = streamEvents.LastEventNumber
            };
        }
    }
}
