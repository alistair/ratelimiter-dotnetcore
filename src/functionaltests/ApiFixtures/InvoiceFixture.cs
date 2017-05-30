using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using StoryTeller;
using StoryTeller.Grammars.API;

namespace functionaltests.ApiFixtures
{
    public class Invoice
    {
        public decimal Amount { get; set; }
        public string Number { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
    }


    public class ApiResponse
    {
        public HttpStatusCode Status { get; set; }

        public string Body { get; set; }
    }

    public class InvoiceFixture : Fixture
    {
        public IGrammar CreateInvoice() {
            return Embed<BasicApiFixture<Invoice>>("Create Invoice in api");
        }
    }

    public class BasicApiFixture<T> : ApiFixture<T, ApiResponse> where T : class
    {
        private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = new List<JsonConverter> { new StringEnumConverter { CamelCaseText = false }}
        };

        protected override async Task<ApiResponse> execute(T input)
        {
            var body = JsonConvert.SerializeObject(input, _settings);
            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var httpClient = new HttpClient();

            var response = await httpClient.PostAsync(
                "http://localhost:8081/api/invoice", content);
            var responsebody = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Response.StatusCode={response.StatusCode}");
            Console.WriteLine(responsebody);
            return new ApiResponse
            {
                Status = response.StatusCode,
                Body = responsebody
            };
        }
    }
}
