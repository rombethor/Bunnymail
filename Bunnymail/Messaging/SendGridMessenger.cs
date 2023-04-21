using Bunnymail.Configuration;
using Bunnymail.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Dynamic;
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
        bool logMessages = false;
        public SendGridMessenger(IConfiguration configuration)
        {
            apiKey = configuration["sendgridapikey"];
            string logmsg = configuration["logmessages"];
            if (logmsg != null && logmsg == "yes")
            {
                logMessages = true;
            }
        }

        public void Register(IModel channel)
        {
            channel.ExchangeDeclare(exchangeName, "fanout", false, false, null);
            channel.QueueDeclare(queueName, true, false, false, null);
            channel.QueueBind(queueName, exchangeName, string.Empty, null);

            mailConsumer = new(channel);

            mailConsumer.Received += MailConsumer_Received;
            channel.BasicConsume(queueName, false, mailConsumer);

            this.channel = channel;
        }

        private void MailConsumer_Received(object? sender, BasicDeliverEventArgs e)
        {
            try
            {
                var strbody = Encoding.UTF8.GetString(e.Body.ToArray());
                //Optional logging
                if (logMessages)
                {
                    Console.WriteLine($"@{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
                    Console.WriteLine($">{strbody}");
                }
                
                //Deserialise message
                MailOutMessage? body = JsonSerializer.Deserialize<MailOutMessage>(strbody, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
                if (body == null)
                {
                    channel?.BasicAck(e.DeliveryTag, false);
                    return;
                }

                // Load event template
                ConfigFile.Load();
                if (!ConfigFile.Data.TryGetValue(body.Event, out TemplateOptions? template))
                {
                    channel?.BasicReject(e.DeliveryTag, false);
                    return;
                }

                // Prepare meesage and client
                EmailAddress from = new(template.FromAddress, template.FromName);
                EmailAddress to = new(body.Recipient);
                SendGridClient client = new(apiKey);

                // Prepare dynamic custom data object
                var exo = new ExpandoObject() as IDictionary<string, object>;
                if (body.Data != null)
                {
                    foreach(var p in body.Data)
                    {
                        exo.Add(p.Key, p.Value);
                    }
                }
                dynamic dobj = exo;

                // Send
                var msg = MailHelper.CreateSingleTemplateEmail(from, to, template.TemplateId, dobj);

                client.SendEmailAsync(msg).Wait();

                // then
                channel?.BasicAck(e.DeliveryTag, false);
            }
            catch(Exception ex)
            {
                channel?.BasicReject(e.DeliveryTag, false);
                Console.WriteLine($"@{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
                Console.WriteLine($">{ex}");
            }
        }
    }
}
