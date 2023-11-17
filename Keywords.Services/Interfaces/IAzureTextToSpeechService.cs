namespace Keywords.Services.Interfaces;

public interface IAzureTextToSpeechService
{
    Task ConvertTextToSpeech(Guid videoId);
}