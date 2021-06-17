using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using static System.Console;

namespace ServiceBusQueueDemo.ChatConsole
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class Chat
    {
        private const string ConnectionString = "";
        private const string TopicPath = "chattopic";

        internal static void Main()
        {
            WriteLine("Enter name:");
            string userName = ReadLine();

            ManagementClient managementClient = new (ConnectionString);

            if (!managementClient.TopicExistsAsync(TopicPath).Result)
            {
                managementClient.CreateTopicAsync(TopicPath).Wait();
            }

            SubscriptionDescription subscriptionDescription = new (TopicPath, userName)
            {
                AutoDeleteOnIdle = TimeSpan.FromMinutes(5)
            };            
            managementClient.CreateSubscriptionAsync(subscriptionDescription).Wait();

            TopicClient topicClient = new (ConnectionString, TopicPath);
            SubscriptionClient subscriptionClient = new (ConnectionString, TopicPath, userName);

            subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, ExceptionReceivedHandler);

            Message helloMessage = new(Encoding.UTF8.GetBytes("Starting chat session..."))
            {
                Label = userName
            };
            topicClient.SendAsync(helloMessage).Wait();
            byte[] contentBytes;

            while (true)
            {
                string text = ReadLine();

                if (text == null)
                {
                    continue;
                }

                if (text.Equals("exit"))
                {
                    break;
                }

                contentBytes = Encoding.UTF8.GetBytes(text);
                Message chatMessage = new(contentBytes)
                {
                    Label = userName
                };

                topicClient.SendAsync(chatMessage).Wait();
            }

            contentBytes = Encoding.UTF8.GetBytes("Ending chat session...");
            Message goodbyeMessage = new(contentBytes)
            {
                Label = userName
            };
            topicClient.SendAsync(goodbyeMessage).Wait();

            topicClient.CloseAsync().Wait();
            subscriptionClient.CloseAsync().Wait();
        }

        private static async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            string text = Encoding.UTF8.GetString(message.Body);
            WriteLine($"{message.Label} > {text}");
        }

        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            return Task.CompletedTask;
        }
    }
}
