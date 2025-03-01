namespace WelcomeService.RabbitMQ
{
    public interface IMessageBus
    {
        void Publish<T>(string queue, T message);
    }
}
