using Bunnymail.Configuration;
using Bunnymail.Messaging;
using Bunnymail.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace Bunnymail.Controllers
{
    [Route("test")]
    [ApiController]
    [Authorize]
    public class TestController : ControllerBase
    {
        
        public TestController()
        {
        }

        [HttpPost]
        public IActionResult SendEmail([FromBody]MailOutMessage msg)
        {
            string json = JsonSerializer.Serialize(msg);
            var data = Encoding.UTF8.GetBytes(json);

            //Dump it into the message queue
            RabbitMQClient.Instance.Channel.BasicPublish("mail.out", "", true, null, data);

            return Ok();
        }
    }
}
