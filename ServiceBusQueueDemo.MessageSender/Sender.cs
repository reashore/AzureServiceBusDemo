using System.Text;
using Microsoft.Azure.ServiceBus;
using static System.Console;

namespace ServiceBusQueueDemo.MessageSender
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class Sender
    {
        private const string ConnectionString = "";
        private const string QueuePath = "demoqueue";

        internal static void Main()
        {
            QueueClient queueClient = new QueueClient(ConnectionString, QueuePath);

            for (int n = 0; n < 10; n++)
            {
                string contentString = $"Message: {n}";
                byte[] contentBytes = Encoding.UTF8.GetBytes(contentString);
                Message message = new Message(contentBytes);
                queueClient.SendAsync(message).Wait();
                WriteLine($"Sent message: {n}");
            }

            queueClient.CloseAsync().Wait();
            WriteLine("Sent messages...");
            ReadLine();
        }
    }
}
