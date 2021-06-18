using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using static System.Console;

namespace ServiceBusQueueDemo.MessageReceiver
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class Receiver
    {
        private const string ConnectionString = "";
        private const string QueuePath = "demoqueue";
        
        internal static async Task Main()
        {
            await ReceiveMessagesAsync();
            WriteLine("Sent messages...");
            ReadKey();
        }

        private static async Task ReceiveMessagesAsync()
        {
            QueueClient queueClient = new QueueClient(ConnectionString, QueuePath);
            queueClient.RegisterMessageHandler(ProcessMessagesAsync, ExceptionReceivedHandler);
            await queueClient.CloseAsync();
        }

        private static Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            string contentString = Encoding.UTF8.GetString(message.Body);
            WriteLine($"Received: {contentString}");
            return Task.CompletedTask;
        }

        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            WriteLine($"exception: {exceptionReceivedEventArgs.Exception}");
            return Task.CompletedTask;
        }
    }
}
