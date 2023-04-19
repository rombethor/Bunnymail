using Bunnymail.Configuration;
using Bunnymail.Entities;
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
        ConfigDbContext _db;

        public TemplateController(ConfigDbContext db)
        {
            _db = db;
        }


        [HttpGet]
        [Authorize(AuthenticationSchemes = "Basic")]
        public IActionResult GetTemplates(int limit = 100, int offset = 0)
        {
            var templateList = _db.EventTemplates
                .OrderBy(et => et.Event)
                .Skip(offset).Take(limit)
                .Select(et => new
                {
                    et.Event,
                    et.TemplateID,
                    et.FromName,
                    et.FromAddress
                }).ToList();

            return Ok(new
            {
                Offset = offset,
                Limit = limit,
                Data = templateList
            });
        }

        [HttpGet("{name}")]
        public IActionResult GetTemplate(string name)
        {
            var template = _db.EventTemplates.Where(et => et.Event == name)
                .Select(et => new
                {
                    et.Event,
                    et.TemplateID,
                    et.FromName,
                    et.FromAddress
                }).FirstOrDefault();

            if (template == null)
                return NotFound();

            return Ok(template);
        }

        [HttpPut("{name}")]
        public IActionResult SetTemplate(string name, SetTemplateOptions options)
        {
            EventTemplate? template = _db.EventTemplates.FirstOrDefault(et => et.Event == name);
            if (template == null)
            {
                template = new()
                {
                    Event = name,
                    FromAddress = options.FromAddress,
                    FromName = options.FromName,
                    TemplateID = options.TemplateId
                };
                _db.EventTemplates.Add(template);
            }
            else
            {
                template.TemplateID = options.TemplateId;
                template.FromName = options.FromName;
                template.FromAddress = options.FromAddress;
            }
            _db.SaveChanges();

            return Ok();
        }

        [HttpDelete("{name}")]
        public IActionResult RemoveTemplate(string name)
        {
            EventTemplate? template = _db.EventTemplates.FirstOrDefault(et => et.Event == name);
            if (template == null)
                return NotFound();

            _db.EventTemplates.Remove(template);
            _db.SaveChanges();

            return Ok();
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
