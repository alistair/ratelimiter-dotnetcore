using StoryTeller;
using System.Threading.Tasks;
using System.Net.Http;
using System;

namespace functionaltests.ApiFixtures
{
    public class BasicApiChecksFixture : Fixture
    {
        Task<HttpResponseMessage> _response;
        HttpClient client;

        public void RunningSystem()
        {
            client = new HttpClient();
        }

        public void HealthCheck()
        {
            var url = Environment.GetEnvironmentVariable("APP_URL") ?? "http://localhost:8081";
            _response = client.GetAsync($"{url}/api/ping");
        }

        public void Values()
        {
            var url = Environment.GetEnvironmentVariable("APP_URL") ?? "http://localhost:8081";
            _response = client.GetAsync($"{url}/api/values");
        }

        public Task ValidStatus()
        {
            var res = _response.Result;
            res.EnsureSuccessStatusCode();
            return Task.CompletedTask;
        }
    }
}
