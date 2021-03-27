using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using DeepEqual.Syntax;
using Jobs.Aggregator.Aws.Services.Contracts;
using Jobs.Aggregator.Aws.Services.Implementations;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace UnitTests
{
    public class S3ServiceTests
    {
        private readonly Mock<IAmazonS3> _amazonS3Mock = new();
        private readonly Mock<ILogger<S3Service>> _loggerMock = new();
        private readonly IS3Service _sut;

        public S3ServiceTests()
        {
            _sut = new S3Service(_amazonS3Mock.Object, _loggerMock.Object);
        }

        [Theory]
        [MemberData(nameof(PutObjectAsync_ShouldCallS3PutObjectAsyncWithRightRequest_TestData))]
        public async Task PutObjectAsync_ShouldCallS3PutObjectAsyncWithRightRequest(PutObjectRequest expected,
            string bucketNameData, string key, string body)
        {
            // Arrange
            var actual = new PutObjectRequest();
            _amazonS3Mock.Setup(
                    e => e.PutObjectAsync(It.IsAny<PutObjectRequest>(), new CancellationToken()))
                .Callback<PutObjectRequest, CancellationToken>((e, token) => { actual = e; });

            // Act
            await _sut.PutObjectAsync(bucketNameData, key, body);

            // Assert
            _amazonS3Mock.Verify(e => e.PutObjectAsync(It.IsAny<PutObjectRequest>(), new CancellationToken()),
                Times.AtLeast(1));
            actual.ShouldDeepEqual(expected);
        }

        public static IEnumerable<object[]> PutObjectAsync_ShouldCallS3PutObjectAsyncWithRightRequest_TestData()
        {
            yield return new object[]
            {
                new PutObjectRequest
                {
                    BucketName = "bucketName",
                    Key = "key",
                    ContentType = "application/json",
                    ContentBody = "body"
                },
                "bucketName",
                "key",
                "body"
            };
        }
    }
}