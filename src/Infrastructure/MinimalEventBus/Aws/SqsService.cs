using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.Runtime.SharedInterfaces;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Options;

namespace MinimalEventBus.Aws
{
    public class SqsService : ISqsService
    {
        private AmazonSQSClient sqsClient;

        public SqsService(IOptions<EventBusConfig> config, AWSCredentials credentials)
        {
            var sqsConfig = new AmazonSQSConfig();
            sqsConfig.ServiceURL = config.Value.SqsServiceUrl;

            this.sqsClient = new AmazonSQSClient(credentials, sqsConfig);
        }

        public ICoreAmazonSQS GetSqsClient()
        {
            return this.sqsClient;
        }

        public async Task<CreateQueueResponse> CreateQueue(string queueName)
        {
            var createQueueRequest = new CreateQueueRequest();
            createQueueRequest.QueueName = queueName;
            var attrs = new Dictionary<string, string>();
            attrs.Add(QueueAttributeName.VisibilityTimeout, "10");
            createQueueRequest.Attributes = attrs;

            return await sqsClient.CreateQueueAsync(createQueueRequest);
        }

        public async Task<ReceiveMessageResponse> ReceiveMessageAsync(string queueUrl)
        {
            return await sqsClient.ReceiveMessageAsync(queueUrl);
        }

        public async Task<List<string>> GetQueueUrls()
        {
            ListQueuesResponse response = await sqsClient.ListQueuesAsync(new ListQueuesRequest());
            var queues = new List<string>();
            foreach (var queueUrl in response.QueueUrls)
            {
                queues.Add($"{queueUrl}");
            }

            return queues;
        }
        public async Task<DeleteMessageResponse> DeleteMessageAsync(string queueUrl, string receiptHandle)
        {
            var deleteMessageRequest = new DeleteMessageRequest();
            deleteMessageRequest.QueueUrl = queueUrl;
            deleteMessageRequest.ReceiptHandle = receiptHandle;

            return await sqsClient.DeleteMessageAsync(deleteMessageRequest);
        }

        public string GetQueueUrl(string queueName)
        {
            return $"{this.sqsClient.Config.ServiceURL}/queue/{queueName}";
        }
    }
}
