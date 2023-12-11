using System.Net;
using System.Net.Http.Headers;
using speechToText_api;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Xunit;

namespace Keywords.Tests.Integration;

public class SpeechToTextIntegrationTest : TestFixture, IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public SpeechToTextIntegrationTest(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }
    
    [Fact]
    public void Can_Create_Pronunciation_Assessment()
    {
        _factory.SpeechToTextMockServer
            .Given(Request.Create().UsingPost()
                .WithPath("/speech/recognition/conversation/cognitiveservices/v1")
                .WithHeader("Ocp-Apim-Subscription-Key", _factory.SpeechToTextKey))
                .RespondWith(Response.Create()
                    .WithSuccess().WithBodyAsJson(A<PronunciationAssessmentResponse>()));
        
        var audioContent = new ByteArrayContent(CreateMany<byte>(200).ToArray());

        var form = new MultipartFormDataContent();
        if (form.Headers.ContentType != null) form.Headers.ContentType.MediaType = "multipart/form-data";
        audioContent.Headers.ContentType = MediaTypeHeaderValue.Parse("audio/wav");
        form.Add(audioContent, "body", "body.wav");
        
        var client = _factory.CreateClient();
        var response = client.PostAsync("/speech?language=da-DK&referenceText=chokolade", 
            form);
        
        Assert.Equal(HttpStatusCode.OK, response.Result.StatusCode);
    }
}