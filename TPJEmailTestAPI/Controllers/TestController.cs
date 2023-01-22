using Microsoft.AspNetCore.Mvc;
using TPJ.Email;

namespace TPJEmailTestAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IEmailer _emailer;
        public TestController(IEmailer emailer)
        {
            _emailer = emailer;
        }

        [HttpGet]
        public void Get(string toEmail)
        {
            _emailer.Send(toEmail, "Test TPJ Email", "<h1>This is a test sent from a API</h1>", true);
        }
    }
}