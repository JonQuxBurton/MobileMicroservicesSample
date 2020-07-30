using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SimCards.EventHandlers.BackgroundServices;
using SimCards.EventHandlers.Data;
using SimCards.EventHandlers.Domain;
using System;
using Xunit;

namespace SimCards.EventHandlers.Tests.BackgroundServices
{
    public class CompletedOrderPollingHostedServiceSpec
    {
        public class DoWorkShould
        {
            [Fact]
            public void CallCompletedOrderCheker()
            {
                var config = new Config
                {
                    ExternalSimCardsProviderApiUrl = "http://api:5000"
                };
                var expectedOrder = new SimCardOrder()
                {
                    MobileOrderId = Guid.NewGuid(),
                    Name = "Neil Armstrong",
                    Status = "Sent"
                };
                var dataStoreMock = new Mock<ISimCardOrdersDataStore>();
                dataStoreMock.Setup(x => x.GetSent())
                    .Returns(new[] { expectedOrder });
                var completedOrderChecker = new Mock<ICompletedOrderChecker>();

                var sut = new CompletedOrderPollingHostedService(Options.Create(config), Mock.Of<ILogger<CompletedOrderPollingHostedService>>(),
                    dataStoreMock.Object,
                    completedOrderChecker.Object);

                sut.DoWork();

                completedOrderChecker.Verify(x => x.Check(expectedOrder));
            }
        }
    }
}
