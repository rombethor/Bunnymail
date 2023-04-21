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
    [Route("bunnymail/template")]
    [ApiController]
    [Authorize]
    public class TemplateController : ControllerBase
    {
        
        public TemplateController()
        {
        }


        [HttpGet]
        [Authorize(AuthenticationSchemes = "Basic")]
        public IActionResult GetTemplates(int limit = 100, int offset = 0)
        {
            ConfigFile.Load();
            return Ok(ConfigFile.Data);
        }

        [HttpGet("{name}")]
        public IActionResult GetTemplate(string name)
        {
            if (ConfigFile.Data.TryGetValue(name, out TemplateOptions data))
                return Ok(data);
            return NotFound();
        }

        [HttpPut("{name}")]
        public IActionResult SetTemplate(string name, TemplateOptions options)
        {
            ConfigFile.Data[name] = options;
            ConfigFile.Save();

            return Ok();
        }

        [HttpDelete("{name}")]
        public IActionResult RemoveTemplate(string name)
        {
            if (ConfigFile.Data.Remove(name))
                return Ok();
            return NotFound();
        }

        [HttpPost("test")]
        public IActionResult SendTest(MailOutMessage msg)
        {
            //Drop message detail into queue
            string json = JsonSerializer.Serialize(msg);
            var bytes = Encoding.UTF8.GetBytes(json);
            RabbitMQClient.Instance.Channel.BasicPublish("mail.out", "", true, null, bytes);
            return Ok();
        }

    }
}
