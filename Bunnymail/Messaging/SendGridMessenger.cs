using Bunnymail.Configuration;
using Bunnymail.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Text;
using System.Text.Json;

namespace Bunnymail.Messaging
{
    public class SendGridMessenger : IMessenger
    {
        const string exchangeName = "mail.out";
        const string queueName = "mailout";

        EventingBasicConsumer mailConsumer;

        IModel? channel;

        string apiKey;

        public SendGridMessenger(IConfiguration configuration)
        {
            apiKey = configuration["sendgridapikey"];
        }

        public void Register(IModel channel)
        {
            channel.ExchangeDeclare(exchangeName, "fanout", false, false, null);
            channel.QueueDeclare(queueName, true, false, false, null);

            //TODO: retry queue?
            mailConsumer = new(channel);

            mailConsumer.Received += MailConsumer_Received;

            this.channel = channel;
        }

        private void MailConsumer_Received(object? sender, BasicDeliverEventArgs e)
        {
            var strbody = Encoding.UTF8.GetString(e.Body.ToArray());
            MailOutMessage? body = JsonSerializer.Deserialize<MailOutMessage>(strbody, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });
            if (body == null)
            {
                channel?.BasicAck(e.DeliveryTag, false);
                return;
            }


            //Get the detail...
            var db = ConfigDbContextFactory.Instance.CreateDbContext();
            var template = db.EventTemplates.Where(et => et.Event == body.Event)
                .FirstOrDefault();
            if (template == default)
            {
                channel?.BasicReject(e.DeliveryTag, false);
                return;
            }

            EmailAddress from = new(template.FromAddress, template.FromName);
            EmailAddress to = new(body.Recipient);

            SendGridClient client = new(apiKey);

            var msg = MailHelper.CreateSingleTemplateEmail(from, to, template.TemplateID, body.Data);
            client.SendEmailAsync(msg).Wait();

            // then
            channel?.BasicAck(e.DeliveryTag, false);
        }
    }
}
