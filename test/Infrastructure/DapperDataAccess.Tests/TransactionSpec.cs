using Moq;
using System.Data.Common;
using Xunit;

namespace DapperDataAccess.Tests
{
    public class TransactionSpec
    {
        public class ConstructorShould
        {
            [Fact]
            public void OpenConnection()
            {
                var connectionMock = new Mock<DbConnection>();

                var sut = new Transaction(connectionMock.Object);

                connectionMock.Verify(x => x.Open());
            }
        }
    }
}
