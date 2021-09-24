using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using AirlineSendAgent.Client;
using AirlineSendAgent.Data;
using AirlineSendAgent.Dtos.FlightDetails;
using AirlineSendAgent.Dtos.Notification;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AirlineSendAgent.App
{
    public interface IAppHost
    {
        void Run();
    }
    public class AppHost : IAppHost
    {
        private readonly SendAgentDbContext _context;
        private readonly IWebhookClient _webhookClient;

        public AppHost(SendAgentDbContext context, IWebhookClient webhookClient)
        {
            _context = context;
            _webhookClient = webhookClient;
        }
        public void Run()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672
            };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.ExchangeDeclare("trigger", ExchangeType.Fanout);
            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queueName, "trigger", "");
            var consumer = new EventingBasicConsumer(channel);
            Console.WriteLine("Listening ion the message bus");
            consumer.Received += async (moduleHandle, ea) =>
            {
                Console.WriteLine("Event triggered");

                var body = ea.Body;
                var message = JsonSerializer.Deserialize<NotificationMessageDto>(
                    Encoding.UTF8.GetString(body.ToArray()));

                if (message != null)
                {
                    var webhookToSend = new FlightDetailChangePayloadDto()
                    {
                        WebhookType = message.WebhookType,
                        WebhookUri = string.Empty,
                        Secret = string.Empty,
                        Publisher = string.Empty,
                        OldPrice = message.OldPrice,
                        NewPrice = message.NewPrice,
                        FlightCode = message.FlightCode
                    };

                    foreach (var webhook in _context.WebhookSubscriptions.Where(s => s.WebhookType == message.WebhookType))
                    {
                        webhookToSend.WebhookUri = webhook.WebhookUri;
                        webhookToSend.Secret = webhook.Secret;
                        webhookToSend.Publisher = webhook.WebhookPublisher;

                        await _webhookClient.SendWebhookNotification(webhookToSend);
                    }
                }
            };

            channel.BasicConsume(queueName, autoAck: true, consumer: consumer);

            Console.ReadLine();
        }
    }
}