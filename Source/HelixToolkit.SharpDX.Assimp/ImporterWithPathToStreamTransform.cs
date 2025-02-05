using Assimp;

namespace HelixToolkit.SharpDX.Assimp;

public sealed class ImporterWithPathToStreamTransform : Importer
{
    private readonly AssimpContext _context;
    private readonly Func<string, Stream?>? _createStreamForPath;
    private string _rootPath;

    public ImporterWithPathToStreamTransform(Func<string, Stream?>? createStreamForPath = null)
    {
        _createStreamForPath = createStreamForPath;
        _rootPath = string.Empty;
        _context = new AssimpContext();
        _context.SetIOSystem(new CustomReadIOSystem(createStreamForPath, (string rootPath) => _rootPath = rootPath));
        Configuration.ExternalContext = _context;
    }

    protected override TextureModel? OnLoadTexture(string texturePath, out string? actualPath)
    {
        if (_createStreamForPath is not null)
        {
            texturePath = Path.Combine(_rootPath, texturePath);
            actualPath = texturePath;
            Stream? stream = _createStreamForPath(texturePath);

            if (stream is null)
            {
                return null;
            }

            return new TextureModel(stream);
        }

        return base.OnLoadTexture(texturePath, out actualPath);
    }

    protected override void Dispose(bool disposing)
    {
        _context.Dispose();
        base.Dispose(disposing);
    }

    private sealed class CustomReadIOSystem : IOSystem
    {
        private readonly Func<string, Stream?>? _createStreamForPath;
        private readonly Action<string>? _setRootPath;

        public CustomReadIOSystem(Func<string, Stream?>? createStreamForPath, Action<string>? setRootPath)
        {
            _createStreamForPath = createStreamForPath;
            _setRootPath = setRootPath;
        }

        public override IOStream OpenFile(string pathToFile, FileIOMode fileMode)
        {
            Stream? stream;

            if (_createStreamForPath is not null)
            {
                _setRootPath?.Invoke(Path.GetDirectoryName(pathToFile) ?? string.Empty);
                stream = _createStreamForPath(pathToFile);
            }
            else
            {
                stream = File.OpenRead(pathToFile);
            }

            if (stream is null)
            {
                throw new FileNotFoundException("Path not found", pathToFile);
            }

            return new CustomReadIOStream(stream);
        }
    }

    private sealed class CustomReadIOStream : IOStream
    {
        private Stream _stream;

        public CustomReadIOStream(Stream stream)
            : base(string.Empty, FileIOMode.Read)
        {
            _stream = stream;
        }

        public override bool IsValid => true;

        protected override void Dispose(bool disposing)
        {
            _stream.Dispose();
            base.Dispose(disposing);
        }

        public override void Flush()
        {
            _stream.Flush();
        }

        public override long GetFileSize()
        {
            return _stream.Length;
        }

        public override long GetPosition()
        {
            return _stream.Position;
        }

        public override long Read(byte[] dataRead, long count)
        {
            return _stream.Read(dataRead, 0, (int)count);
        }

        public override ReturnCode Seek(long offset, Origin seekOrigin)
        {
            _stream.Seek(offset, (SeekOrigin)seekOrigin);
            return ReturnCode.Success;
        }

        public override long Write(byte[] dataToWrite, long count)
        {
            throw new NotSupportedException();
        }
    }
}
