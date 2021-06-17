using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace ServiceBusQueueDemo.MessageReceiver
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class Receiver
    {
        private const string ConnectionString = "";
        private const string QueuePath = "demoqueue";
        
        internal static void Main()
        {
            QueueClient queueClient = new QueueClient(ConnectionString, QueuePath);
            queueClient.RegisterMessageHandler(ProcessMessagesAsync, ExceptionReceivedHandler);

            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();

            queueClient.CloseAsync().Wait();
        }

        private static async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            byte[] contentBytes = message.Body;
            string contentString = Encoding.UTF8.GetString(contentBytes);
            Console.WriteLine($"Received: { contentString }");
        }

        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            return Task.CompletedTask;
        }
    }
}
