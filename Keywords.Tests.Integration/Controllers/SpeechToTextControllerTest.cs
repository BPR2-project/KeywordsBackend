using System.Net;
using Keywords.API.Swagger.Controllers.Generated;
using Keywords.Data;
using Xunit;

namespace Keywords.Tests.Integration.Controller;

public class SpeechToTextControllerTest : TestFixture, IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public SpeechToTextControllerTest(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Create_Audio_NotFound()
    {
        // Arrange
        var testKeywordId = A<Guid>();

        // Act
        var client = _factory.CreateClient();
        var response = await client.PostAsync($"/text/audio/{testKeywordId}", null);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}