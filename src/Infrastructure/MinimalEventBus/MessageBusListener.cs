using MinimalEventBus.Aws;
using MinimalEventBus.JustSaying;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MinimalEventBus
{
    public class MessageBusListener : IMessageBusListener
    {
        private readonly TimeSpan PollingFrequency = TimeSpan.FromSeconds(5);
        private readonly IMessageBus messageBus;
        private readonly ISqsService sqsService;
        private readonly IMessageDeserializer messageDeserializer;

        public MessageBusListener(IMessageBus messageBus, ISqsService sqsService, IMessageDeserializer messageDeserializer)
        {
            this.messageBus = messageBus;
            this.sqsService = sqsService;
            this.messageDeserializer = messageDeserializer;
        }

        public async Task StartListening()
        {
            while (true)
            {
                foreach (var queueItem in messageBus.Queues)
                {
                    var queueInfo = queueItem.Value;
                    MethodInfo method = this.GetType().GetMethod("CheckForMessages");
                    MethodInfo genericMethod = method.MakeGenericMethod(queueInfo.MessageType);
                    genericMethod.Invoke(this, new object[] { queueInfo });
                }

                await Task.Delay((int)PollingFrequency.TotalMilliseconds);
            }
        }

        public async Task CheckForMessages<T>(QueueInfo queue) where T : Message
        {
            var queueUrl = queue.Url;
            var response = this.sqsService.ReceiveMessageAsync(queueUrl);

            if (response.Result.Messages.Count > 0)
            {
                var busMessage = response.Result.Messages.First();
                T message = messageDeserializer.Deserialize<T>(busMessage);

                IHandlerAsync<T> handler = this.messageBus.GetHandler<T>();

                if (handler != null)
                {
                    var isSuccess = await handler.Handle(message);

                    if (isSuccess)
                    {
                        await this.DeleteMessageAsync(queueUrl, busMessage.ReceiptHandle);
                    }
                }
            }
        }

        private async Task<bool> DeleteMessageAsync(string queueUrl, string receiptHandle)
        {
            var response = await this.sqsService.DeleteMessageAsync(queueUrl, receiptHandle);

            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}
