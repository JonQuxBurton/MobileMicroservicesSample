using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Options;
using MinimalEventBus.JustSaying;

namespace MinimalEventBus.Aws
{
    public class SnsService : ISnsService
    {
        private readonly Dictionary<string, string> topics = new Dictionary<string, string>();
        private readonly AmazonSimpleNotificationServiceClient snsClient;
        private readonly IJsonSerializer jsonSerializer;
        private readonly ISqsService sqsService;

        public SnsService(IOptions<EventBusConfig> config, AWSCredentials credentials, ISqsService sqsService)
        {
            var snsConfig = new AmazonSimpleNotificationServiceConfig
            {
                ServiceURL = config?.Value.SnsServiceUrl
            };
            this.snsClient = new AmazonSimpleNotificationServiceClient(credentials, snsConfig);
            this.jsonSerializer = new DefaultJsonSerializer();
            this.sqsService = sqsService;
        }

        public SnsService(IOptions<EventBusConfig> config, AWSCredentials credentials, ISqsService sqsService, IJsonSerializer jsonSerializer)
        {
            var snsConfig = new AmazonSimpleNotificationServiceConfig
            {
                ServiceURL = config?.Value.SnsServiceUrl
            };
            this.snsClient = new AmazonSimpleNotificationServiceClient(credentials, snsConfig);
            this.jsonSerializer = jsonSerializer;
            this.sqsService = sqsService;
        }

        public async Task<CreateTopicResponse> CreateTopic(string topicName)
        {
            var createTopicRequest = new CreateTopicRequest(topicName);
            var response = await snsClient.CreateTopicAsync(createTopicRequest);
            topics.Add(topicName, response.TopicArn);

            return response;
        }

        public async Task<ListTopicsResponse> ListTopics()
        {
            var request = new ListTopicsRequest();
            var response = await snsClient.ListTopicsAsync(request);

            return response;
        }

        public async Task<bool> PublishAsync(string eventName, Message message)
        {
            var topicArn = topics[eventName];
            var json = this.jsonSerializer.Serialize(message);
            var publishResponse = await snsClient.PublishAsync(topicArn, json);

            return publishResponse.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        public async Task<string> SubscribeQueueAsync(string topicArn, string sqsQueueUrl)
        {
            return await snsClient.SubscribeQueueAsync(topicArn, this.sqsService.GetSqsClient(), sqsQueueUrl);
        }
    }
}
