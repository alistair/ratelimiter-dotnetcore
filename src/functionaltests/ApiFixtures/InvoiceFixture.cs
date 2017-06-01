using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using StoryTeller;
using StoryTeller.Grammars.API;
using StoryTeller.Json;

namespace functionaltests.ApiFixtures
{
    public class Invoice
    {
        public decimal Amount { get; set; }
        public string Number { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
    }

    public class InvoiceCreated
    {
        public Guid Id { get; set; }
        public List<LineItem> Items { get; set; }
    }

    public class LineItem
    {
        public string Number { get; set; }
        public decimal Amount { get; set; }
        public decimal Total { get; set; }
        public int Quantity { get; set; }
    }


    public class ApiResponse<T>
    {
        public HttpStatusCode Status { get; set; }

        public string Body { get; set; }

        public T Response { get; set; }
    }

    public class InvoiceFixture : Fixture
    {
        public IGrammar CreateInvoice() {
            return Embed<BasicApiFixture<Invoice, InvoiceCreated>>("Create Invoice in api");
        }

        public IGrammar CheckJsonResponse() {
            return Embed<InvoiceCreatedJsonComparisonFixture>("Ensure response body is");
        }

        public IGrammar CheckEventStore() {
            return Embed<EventStoreFixture>("And Check an event was published").Before(x => {
                var invoiceCreated = x.State.Retrieve<ApiResponse<InvoiceCreated>>();
                x.State.CurrentObject = $"invoices/{invoiceCreated.Response.Id}";
            });
        }
    }

    public class BasicApiFixture<T, TOut> : ApiFixture<T, ApiResponse<TOut>> where T : class
    {
        private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = new List<JsonConverter> { new StringEnumConverter { CamelCaseText = false }}
        };

        protected override Task<ApiResponse<TOut>> execute(T input)
        {
            var body = JsonConvert.SerializeObject(input, _settings);
            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var httpClient = new HttpClient();

            var url = Environment.GetEnvironmentVariable("APP_URL") ?? "http://localhost:8081";

            Console.WriteLine($"URL={url}");

            var response = httpClient.PostAsync(
                $"{url}/api/invoice", content).Result;
            var responsebody = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine($"Response is {response.StatusCode}");

            var responseObject = JsonConvert.DeserializeObject<TOut>(responsebody, _settings);

            var result = new ApiResponse<TOut>
            {
                Status = response.StatusCode,
                Body = responsebody,
                Response = responseObject
            };
            Context.State.Store(result);

            return Task.FromResult(result);
        }
    }

    public class InvoiceCreatedJsonComparisonFixture : JsonComparisonFixture {

        public override void SetUp() {
            var output = Context.State.Retrieve<ApiResponse<InvoiceCreated>>();
            Console.WriteLine($"JsonCmp: {output.Body}");
            CurrentObject = output.Body;
            StoreJson(output.Body);
        }

        public IGrammar AmountIs() {
            return CheckJsonValue<decimal>("$.items[0].amount", "The Amount should be {amount}");
        }
    }

    public class EventStoreFixture : JsonComparisonFixture {
        public object EventStoreConnectionSetting { get; private set; }

        public override void SetUp() {
            var stream = (string)CurrentObject;

            var cString = Environment.GetEnvironmentVariable("EventStoreConnection")
                ?? "ConnectTo=tcp://admin:changeit@localhost:1113";
            var connection = EventStoreConnection.Create(cString);
            connection.ConnectAsync().Wait();

            var result = connection.ReadStreamEventsForwardAsync(stream, 0, 100, false).Result;
            Console.WriteLine($"Hello Events:");
            var a = string.Join(",", result.Events.Select(x => Encoding.UTF8.GetString(x.Event.Data)));
            base.StoreJson("{ results: [" + a + "]}");
        }
    }

}
