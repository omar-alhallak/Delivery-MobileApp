using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace DeliveryApp.API.Controllers
{
    [ApiController]
    [Route("api/redis-test")]
    public class RedisTestController : ControllerBase
    {
        private readonly IConnectionMultiplexer _redis;

        public RedisTestController(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var db = _redis.GetDatabase();
            db.StringSet("ping:test", "pong", TimeSpan.FromSeconds(10));
            var value = db.StringGet("ping:test");
            return Ok(new { redis = value.ToString() });
        }
    }
}