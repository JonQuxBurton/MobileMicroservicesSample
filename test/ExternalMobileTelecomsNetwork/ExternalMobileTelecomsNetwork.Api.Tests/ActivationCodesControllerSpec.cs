using ExternalMobileTelecomsNetwork.Api.Controllers;
using ExternalMobileTelecomsNetwork.Api.Data;
using ExternalMobileTelecomsNetwork.Api.Resources;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using Utils.DateTimes;
using Xunit;

namespace ExternalMobileTelecomsNetwork.Api.Tests
{
    public class ActivationCodesControllerSpec
    {
        public class Post_When_Inserting_Should
        {
            private Mock<IDataStore> dataStoreMock;
            private Mock<IDateTimeCreator> dateTimeCreatorMock;
            private ActivationCodeToAdd expected;
            private ActivationCodesController sut;

            public Post_When_Inserting_Should()
            {
                dateTimeCreatorMock = new Mock<IDateTimeCreator>();

                expected = new ActivationCodeToAdd
                {
                    Reference = Guid.NewGuid(),
                    ActivationCode = "BAC132"
                };
                dataStoreMock = new Mock<IDataStore>();
                dataStoreMock.Setup(x => x.InsertActivationCode(
                        It.Is<ActivationCode>(y => y.Reference == expected.Reference
                                                   && y.Code == expected.ActivationCode)))
                             .Returns(true);

                sut = new ActivationCodesController(dataStoreMock.Object, dateTimeCreatorMock.Object);
            }

            [Fact]
            public void InsertNewActivationCode()
            {
                sut.Post(expected);

                dataStoreMock.VerifyAll();
            }

            [Fact]
            public void ReturnOkWhenSuccess()
            {
                var actual = sut.Post(expected);

                actual.Should().BeOfType<OkResult>();
            }

            [Fact]
            public void ReturnErrorWhenFails()
            {
                dataStoreMock.Setup(x => x.InsertActivationCode(
                        It.IsAny<ActivationCode>()))
                             .Returns(false);

                var actual = sut.Post(expected);

                actual.Should().BeOfType<StatusCodeResult>();
                (actual as StatusCodeResult).StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            }
        }

        public class Post_When_Existing_Should
        {
            private Mock<IDataStore> dataStoreMock;
            private Mock<IDateTimeCreator> dateTimeCreatorMock;
            private ActivationCodeToAdd expected;
            private ActivationCodesController sut;

            public Post_When_Existing_Should()
            {
                dateTimeCreatorMock = new Mock<IDateTimeCreator>();
                expected = new ActivationCodeToAdd
                {
                    Reference = Guid.NewGuid(),
                    ActivationCode = "BAC132"
                };
                var existing = new ActivationCode
                {
                    Id = 101,
                    Reference = expected.Reference,
                    Code = "ABC0405"
                };
                var expectedDateTime = new DateTime(2001, 5, 4);
                dataStoreMock = new Mock<IDataStore>();
                dataStoreMock.Setup(x => x.GetActivationCode(expected.Reference))
                    .Returns(existing);
                dataStoreMock.Setup(x => x.UpdateActivationCode(
                        It.Is<ActivationCode>(y => y.Id == existing.Id &&
                        y.Reference == expected.Reference &&
                        y.Code == expected.ActivationCode &&
                        y.UpdatedAt == expectedDateTime)))
                             .Returns(true);
                dateTimeCreatorMock.Setup(x => x.Create())
                    .Returns(expectedDateTime);

                sut = new ActivationCodesController(dataStoreMock.Object, dateTimeCreatorMock.Object);
            }

            [Fact]
            public void UpdateExistingActivationCode()
            {
                sut.Post(expected);

                dataStoreMock.VerifyAll();
            }

            [Fact]
            public void ReturnOkSuccess()
            {
                var actual = sut.Post(expected);

                actual.Should().BeOfType<OkResult>();
            }

            [Fact]
            public void ReturnErrorWhenFails()
            {
                var existing = new ActivationCode
                {
                    Id = 101,
                    Reference = expected.Reference,
                    Code = "ABC0405"
                };
                dataStoreMock.Setup(x => x.GetActivationCode(expected.Reference))
                    .Returns(existing);
                dataStoreMock.Setup(x => x.UpdateActivationCode(
                        It.IsAny<ActivationCode>()))
                             .Returns(false);

                var actual = sut.Post(expected);

                actual.Should().BeOfType<StatusCodeResult>();
                (actual as StatusCodeResult).StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
