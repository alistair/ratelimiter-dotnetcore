using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Event.Infrastructure;
using EventStore.ClientAPI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;

namespace ratelimiter
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc().AddJsonOptions(_ => {
                  _.SerializerSettings.Formatting = Formatting.Indented;
                  _.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                  _.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                  _.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                  _.SerializerSettings.Converters = new List<JsonConverter> { new StringEnumConverter { CamelCaseText = false }};
                });

            var connectionString = Configuration.GetValue<string>("RedisConnection") ?? "127.0.0.1";

            var conn = ConnectionMultiplexer.Connect(connectionString);
            services.AddSingleton<ConnectionMultiplexer>(conn);

            var eventstoreconnection = Configuration.GetValue<string>("EventStoreConnection");

            var connection = EventStoreConnection.Create(eventstoreconnection);

            services.AddSingleton<IEventStoreConnection>(connection);
            services.AddScoped<IEventStore, GetEventStore>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMiddleware<RateLimiterMiddleware>();
            app.UseMvc();
        }
    }
}
