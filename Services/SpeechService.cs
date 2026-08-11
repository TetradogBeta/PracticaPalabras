using System.Diagnostics;

namespace PracticaPalabras;

public class SpeechService : ISpeechService
{
    private static TtsBackend? _backend;

    public async Task SpeakAsync(string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        var backend = GetBackend();
        try
        {
            await backend.Speak(text, Language.LangCode, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TTS error] {GetBackend().GetType().Name}: {ex.Message}");
            await Task.Delay(180, cancellationToken);
        }
    }

    private static TtsBackend GetBackend()
    {
        if (_backend != null)
        {
            return _backend;
        }

        if (HasBinary("spd-say"))
        {
            _backend = new SpdSayBackend();
        }
        else if (HasBinary("espeak-ng"))
        {
            _backend = new EspeakNgBackend();
        }
        else if (HasBinary("espeak"))
        {
            _backend = new EspeakBackend();
        }
        else if (HasBinary("say"))
        {
            _backend = new SayBackend();
        }
        else if (OperatingSystem.IsWindows())
        {
            _backend = new WindowsOneCoreBackend();
        }
        else
        {
            _backend = new ConsoleBackend();
        }

        Console.WriteLine($"[TTS] using {_backend.GetType().Name}");
        return _backend;
    }

    private static bool HasBinary(string name)
    {
        try
        {
            using var p = Process.Start(new ProcessStartInfo
            {
                FileName = "which",
                Arguments = name,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            });
            return p != null && p.WaitForExit(1000) && p.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }
}

internal abstract class TtsBackend
{
    public abstract Task Speak(string text, string langCode, CancellationToken ct);

    protected static async Task<int> RunAsync(string fileName, string arguments, CancellationToken ct)
    {
        var psi = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        using var process = Process.Start(psi);
        if (process == null)
        {
            return -1;
        }
        await process.WaitForExitAsync(ct);
        return process.ExitCode;
    }

    protected static string MapLanguage(string langCode) => langCode switch
    {
        "en-UK" => "en",
        "en-US" => "en",
        "es-ES" => "es",
        "es-CA" => "ca",
        _ => langCode.Split('-')[0].ToLowerInvariant()
    };

    protected static string EscapeForShell(string text) => text.Replace("\"", "\\\"").Replace("$", "\\$");
}

internal sealed class SpdSayBackend : TtsBackend
{
    public override async Task Speak(string text, string langCode, CancellationToken ct)
    {
        string lang = MapLanguage(langCode);
        await RunAsync("spd-say", $"-l {lang} -w \"{EscapeForShell(text)}\"", ct);
    }
}

internal sealed class EspeakNgBackend : TtsBackend
{
    public override async Task Speak(string text, string langCode, CancellationToken ct)
    {
        string voice = MapEspeakVoice(langCode);
        await RunAsync("espeak-ng", $"-v {voice} \"{EscapeForShell(text)}\"", ct);
    }

    private static string MapEspeakVoice(string langCode) => langCode switch
    {
        "en-UK" => "en+uk",
        "en-US" => "en+us",
        "es-ES" => "es",
        "es-CA" => "ca",
        _ => langCode.Split('-')[0].ToLowerInvariant()
    };
}

internal sealed class EspeakBackend : TtsBackend
{
    public override async Task Speak(string text, string langCode, CancellationToken ct)
    {
        string voice = langCode switch
        {
            "en-UK" => "en+uk",
            "en-US" => "en+us",
            "es-ES" => "es",
            "es-CA" => "ca",
            _ => langCode.Split('-')[0].ToLowerInvariant()
        };
        await RunAsync("espeak", $"-v {voice} \"{EscapeForShell(text)}\"", ct);
    }
}

internal sealed class SayBackend : TtsBackend
{
    public override async Task Speak(string text, string langCode, CancellationToken ct)
    {
        string voice = MapLanguage(langCode);
        await RunAsync("say", $"-v {voice} \"{EscapeForShell(text)}\"", ct);
    }
}

internal sealed class WindowsOneCoreBackend : TtsBackend
{
    public override Task Speak(string text, string langCode, CancellationToken ct)
    {
        Console.WriteLine($"[TTS win] {text}");
        return Task.Delay(180, ct);
    }
}

internal sealed class ConsoleBackend : TtsBackend
{
    public override Task Speak(string text, string langCode, CancellationToken ct)
    {
        Console.WriteLine($"[TTS] {text}");
        return Task.Delay(180, ct);
    }
}