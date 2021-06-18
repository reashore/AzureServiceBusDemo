using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using static System.Console;

namespace ServiceBusQueueDemo.MessageSender
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class Sender
    {
        private const string ConnectionString = "Endpoint=sb://servicebusdemo321.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=/7M98fCxr1qN/QR2mqaFMDhRuvlzlwHiyabAXKPKSp0=";
        private const string QueuePath = "demoqueue";

        internal static async Task Main()
        {
            await SendMessagesAsync();
            WriteLine("Sent messages...");
            ReadKey();
        }

        private static async Task SendMessagesAsync()
        {
            QueueClient queueClient = new (ConnectionString, QueuePath);
            const int numberMessages = 10;

            for (int n = 0; n < numberMessages; n++)
            {
                Message message = CreateMessage($"Message: {n}");
                await queueClient.SendAsync(message);
                WriteLine($"Sent message: {n}");
            }

            await queueClient.CloseAsync();
        }

        private static Message CreateMessage(string content)
        {
            byte[] contentBytes = Encoding.UTF8.GetBytes(content);
            Message message = new (contentBytes);
            return message;
        }
    }
}
