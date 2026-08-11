namespace PracticaPalabras;

public interface ISpeechService
{
    Task SpeakAsync(string text, CancellationToken cancellationToken = default);
}