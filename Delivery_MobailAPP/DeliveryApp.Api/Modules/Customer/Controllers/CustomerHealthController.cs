using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryApp.API.Modules.Customer.Controllers
{
    [Route("api/customer/health")]
    [ApiController]
    public class CustomerHealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() =>Ok(new {status="ok",module="customer" });
    }
}