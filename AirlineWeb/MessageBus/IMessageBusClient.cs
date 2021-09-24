using AirlineWeb.Dtos.Notification;

namespace AirlineWeb.MessageBus
{
    public interface IMessageBusClient
    {
        void SendMessage(NotificationMessageDto notificationMessageDto);
    }
}