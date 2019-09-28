using MinimalEventBus.Aws;
using MinimalEventBus.JustSaying;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MinimalEventBus
{
    public class MessageBus : IMessageBus
    {
        private readonly ISnsService snsService;
        private readonly ISqsService sqsService;
        private readonly IQueueNamingStrategy queueNamingStrategy;
        private List<HandlerResolver> handlerResolvers = new List<HandlerResolver>();

        public Dictionary<string, QueueInfo> Queues { get; } = new Dictionary<string, QueueInfo>();

        public MessageBus(ISnsService snsService, ISqsService sqsService, IQueueNamingStrategy queueNamingStrategy)
        {
            this.snsService = snsService;
            this.sqsService = sqsService;
            this.queueNamingStrategy = queueNamingStrategy;
        }

        public async Task<string> SetupQueue(Message message)
        {
            var queueName = this.queueNamingStrategy.GetName(message);

            if (!Queues.ContainsKey(queueName))
            {
                await SetupSubscription(message.GetType(), queueName);
                Thread.Sleep(10);
            }

            return queueName;
        }

        private async Task SetupSubscription(Type type, string queueName)
        {
            var createQueueResponse = await this.sqsService.CreateQueue(queueName);

            var errorQueueName = $"{queueName}_error";
            var createErrorQueueResponse = await this.sqsService.CreateQueue(errorQueueName);

            var createTopicResponse = await this.snsService.CreateTopic(queueName);

            await this.snsService.SubscribeQueueAsync(createTopicResponse.TopicArn, createQueueResponse.QueueUrl);

            Queues.Add(queueName, new QueueInfo(type, createQueueResponse.QueueUrl));
            Queues.Add(errorQueueName, new QueueInfo(type, createErrorQueueResponse.QueueUrl));
        }

        public void Subscribe<TMessage, THandler>(IHandlerAsync<TMessage> handler)
                where TMessage : Message
                where THandler : class, IHandlerAsync<TMessage>
        {
            var queueName = this.queueNamingStrategy.GetName(typeof(TMessage));
            var queueUrl = this.sqsService.GetQueueUrl(queueName);
            Queues.Add(queueName, new QueueInfo(typeof(TMessage), queueUrl));

            var handlerResolver = new HandlerResolver();
            handlerResolver.Add(new HandlerWrapper<TMessage>(handler));
            handlerResolvers.Add(handlerResolver);
        }

        public IHandlerAsync<T> GetHandler<T>() where T : Message
        {
            foreach (var resolver in handlerResolvers)
            {
                var handler = resolver.Resolve<T>();

                if (handler != null)
                    return handler;
            }

            return null;
        }
    }
}
