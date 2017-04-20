using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            var connectionString = Configuration.GetValue<string>("RedisConnection") ?? "127.0.0.1";

            ConnectionMultiplexer conn = ConnectionMultiplexer.Connect(connectionString);

            app.Use(async (context, next) => {
                    var key = context.Request.Headers["THROTTLE"].FirstOrDefault() ?? "test";
                    var allowed = await CheckRequestRate(conn, key);

                    if (!allowed) {
                        context.Response.StatusCode = 429;
                        return;
                    }

                    await next();
            });

            app.UseMvc();
        }

        public static async Task<bool> CheckRequestRate(ConnectionMultiplexer conn, string user) {
            var prefix = $"request_rate_limiter.{user}";

            var args = new {
                tokens_key = (RedisKey)$"{prefix}.tokens",
                timestamp_key = (RedisKey)$"{prefix}.timestamp",
                rate = REFRESH_RATE,
                capacity = CAPACITY,
                now = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                requested = 1,
            };

            var prepared = LuaScript.Prepare(LUA_SCRIPT1);
            var db = conn.GetDatabase();
            var loaded = prepared.Load(conn.GetServer(conn.GetEndPoints()[0]));
            RedisValue[] result = (RedisValue[])await loaded.EvaluateAsync(db, args);

            bool allowed = (bool)result.First();
            return allowed;
        }

        public const int REFRESH_RATE = 1;
        public const int CAPACITY = 5 * REFRESH_RATE;

        public const string LUA_SCRIPT1 = @"
local tokens_key = @tokens_key
local timestamp_key = @timestamp_key

local rate = tonumber(@rate)
local capacity = tonumber(@capacity)
local now = tonumber(@now)
local requested = tonumber(@requested)

local fill_time = capacity/rate
local ttl = math.floor(fill_time*2)

local last_tokens = tonumber(redis.call(""get"", tokens_key))
if last_tokens == nil then
  last_tokens = capacity
end

local last_refreshed = tonumber(redis.call(""get"", timestamp_key))
if last_refreshed == nil then
  last_refreshed = 0
end

local delta = math.max(0, now-last_refreshed)
local filled_tokens = math.min(capacity, last_tokens+(delta*rate))
local allowed = filled_tokens >= requested
local new_tokens = filled_tokens
if allowed then
  new_tokens = filled_tokens - requested
end

redis.call(""setex"", tokens_key, ttl, new_tokens)
redis.call(""setex"", timestamp_key, ttl, now)

return { allowed, new_tokens }
";

    }
}
