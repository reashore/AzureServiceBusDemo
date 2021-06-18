using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using static System.Console;

namespace ServiceBusQueueDemo.MessageSender
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class Sender
    {
        private const string ConnectionString = "";
        private const string QueuePath = "demoqueue";

        internal static async Task Main()
        {
            QueueClient queueClient = new QueueClient(ConnectionString, QueuePath);
            const int numberMessages = 10;

            for (int n = 0; n < numberMessages; n++)
            {
                Message message = CreateMessage($"Message: {n}");
                await queueClient.SendAsync(message);
                WriteLine($"Sent message: {n}");
            }

            await queueClient.CloseAsync();
            WriteLine("Sent messages...");
            ReadKey();
        }

        private static Message CreateMessage(string content)
        {
            byte[] contentBytes = Encoding.UTF8.GetBytes(content);
            Message message = new Message(contentBytes);
            return message;
        }
    }
}
