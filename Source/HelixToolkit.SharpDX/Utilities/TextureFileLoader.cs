using Microsoft.Extensions.Logging;

namespace HelixToolkit.SharpDX.Utilities;

public class TextureFileLoader : ITextureInfoLoader
{
    static readonly ILogger logger = Logger.LogManager.Create<TextureFileLoader>();

    public string FilePath
    {
        get;
    }

    private Stream fileStream = Stream.Null;
    public TextureFileLoader(string filePath)
    {
        FilePath = filePath;
    }

    public void Complete(Guid id, TextureInfo? info, bool succeeded)
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug("Disposing file stream: {0}.", FilePath);
        }
        fileStream.Dispose();
    }

    public TextureInfo Load(Guid id)
    {
        logger.LogInformation("Loading texture file: {0}", FilePath);

        if (!File.Exists(FilePath))
        {
            return TextureInfo.Null;
        }

        fileStream = File.OpenRead(FilePath);
        return new TextureInfo(fileStream);
    }
}
