using System;
using System.Net;
using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using OIData;

namespace OIFinancialUpdateEventSource.Sourcer
{
    public interface ISourcer
    {
        void Source(Mutation mutation);

        Mutation Read();
    }
    public class EventStoreSourcer : ISourcer
    {
        public IEventStoreConnection connection;
        public EventStoreSourcer()
        {
            connection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));
            connection.ConnectAsync().Wait();

        }

        public Mutation Read()
        {
            StreamEventsSlice streamEvents =
                connection.ReadStreamEventsForwardAsync("test-stream", 0, 1, false).Result;

            RecordedEvent returnedEvent = streamEvents.Events[0].Event;

            return JsonConvert.DeserializeObject<Mutation>(Encoding.UTF8.GetString(returnedEvent.Data));
        }

        public void Source(Mutation mutation)
        {
            var myEvent = new EventData(Guid.NewGuid(), "testEvent", false,
                                                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(mutation)),
                                                null);
            connection.AppendToStreamAsync("test-stream",
                                           ExpectedVersion.Any, myEvent).Wait();
        }
    }

    public class InMemorySourcer : ISourcer
    {
        public Mutation Read()
        {
            return new Mutation();
        }

        public void Source(Mutation mutation)
        {
            //Do Nothing
        }
    }

}
