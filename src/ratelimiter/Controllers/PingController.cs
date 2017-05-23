using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ratelimiter.Controllers
{
    [Route("api/[controller]")]
    public class PingController : Controller
    {
        [HttpGet]
        public OkResult Get()
        {
            return Ok();
        }
    }
}
