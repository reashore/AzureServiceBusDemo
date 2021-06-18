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

        internal static async Task Main()
        {
            WriteLine("Enter name:");
            string userName = ReadLine();

            ManagementClient managementClient = new (ConnectionString);
            TopicClient topicClient = await CreateTopicClient(managementClient);
            SubscriptionClient subscriptionClient = await CreateSubscriptionClient(managementClient, userName);

            await RunMessageLoop(topicClient, userName);

            #region Old message loop

            //-----------------------------

            //Message startMessage = CreateMessage("Starting chat session...", userName);
            //topicClient.SendAsync(startMessage).Wait();

            //while (true)
            //{
            //    string text = ReadLine();

            //    if (text == null)
            //    {
            //        continue;
            //    }

            //    if (text.Equals("exit"))
            //    {
            //        break;
            //    }

            //    Message chatMessage = CreateMessage(text, userName);
            //    await topicClient.SendAsync(chatMessage);
            //}

            //Message endMessage = CreateMessage("Ending chat session...", userName);
            //topicClient.SendAsync(endMessage).Wait();

            #endregion

            await topicClient.CloseAsync();
            await subscriptionClient.CloseAsync();
            await managementClient.CloseAsync();
        }

        private static async Task<TopicClient> CreateTopicClient(ManagementClient managementClient)
        {
            if (!managementClient.TopicExistsAsync(TopicPath).Result)
            {
                await managementClient.CreateTopicAsync(TopicPath);
            }

            TopicClient topicClient = new (ConnectionString, TopicPath);
            return topicClient;
        }

        private static async Task<SubscriptionClient> CreateSubscriptionClient(ManagementClient managementClient, string userName)
        {
            SubscriptionDescription subscriptionDescription = new (TopicPath, userName)
            {
                AutoDeleteOnIdle = TimeSpan.FromMinutes(5)
            };            
            SubscriptionDescription foo = await managementClient.CreateSubscriptionAsync(subscriptionDescription);

            SubscriptionClient subscriptionClient = new (ConnectionString, TopicPath, userName);

            subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, ExceptionReceivedHandler);

            return subscriptionClient;
        }

        private static Message CreateMessage(string content, string label)
        {
            byte[] contentBytes = Encoding.UTF8.GetBytes(content);
            Message message = new(contentBytes)
            {
                Label = label
            };

            return message;
        }

        private static async Task RunMessageLoop(TopicClient topicClient, string userName)
        {
            Message startMessage = CreateMessage("Starting chat session...", userName);
            await topicClient.SendAsync(startMessage);

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

                Message chatMessage = CreateMessage(text, userName);
                await topicClient.SendAsync(chatMessage);
            }

            Message endMessage = CreateMessage("Ending chat session...", userName);
            await topicClient.SendAsync(endMessage);
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
