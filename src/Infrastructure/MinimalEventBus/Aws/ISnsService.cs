using System.Threading.Tasks;
using Amazon.SimpleNotificationService.Model;
using MinimalEventBus.JustSaying;

namespace MinimalEventBus.Aws
{
    public interface ISnsService
    {
        Task<CreateTopicResponse> CreateTopic(string topicName);
        Task<ListTopicsResponse> ListTopics();
        Task<bool> PublishAsync(string eventName, Message message);
        Task<string> SubscribeQueueAsync(string topicArn, string sqsQueueUrl);
    }
}
