using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Runtime.SharedInterfaces;
using Amazon.SQS.Model;

namespace MinimalEventBus.Aws
{
    public interface ISqsService
    {
        ICoreAmazonSQS GetSqsClient();
        Task<CreateQueueResponse> CreateQueue(string queueName);
        Task<ReceiveMessageResponse> ReceiveMessageAsync(string queueUrl);
        Task<List<string>> GetQueueUrls();
        Task<DeleteMessageResponse> DeleteMessageAsync(string queueUrl, string receiptHandle);
        string GetQueueUrl(string queueName);
    }
}
