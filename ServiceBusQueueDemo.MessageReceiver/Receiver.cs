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
            QueueClient queueClient = new QueueClient(ConnectionString, QueuePath);
            queueClient.RegisterMessageHandler(ProcessMessagesAsync, ExceptionReceivedHandler);

            WriteLine("Press enter to exit.");
            ReadKey();

            await queueClient.CloseAsync();
        }

        private static async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            string contentString = Encoding.UTF8.GetString(message.Body);
            WriteLine($"Received: {contentString}");
        }

        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            WriteLine($"exception: {exceptionReceivedEventArgs.Exception}");
            return Task.CompletedTask;
        }
    }
}
