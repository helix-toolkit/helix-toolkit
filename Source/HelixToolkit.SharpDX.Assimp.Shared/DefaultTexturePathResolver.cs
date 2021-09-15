using HelixToolkit.Logger;
using System;
using System.IO;
using System.Runtime.CompilerServices;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    public class DefaultTexturePathResolver : ITexturePathResolver
    {
        private const string ToUpperDictString = @"..\";

        public ILogger Logger { private set; get; }

        public string Resolve(string modelPath, string texturePath, ILogger logger)
        {
            Logger = logger;
            return OnLoadTexture(modelPath, texturePath);
        }

        /// <summary>
        ///     Called when [load texture].
        /// </summary>
        /// <param name="modelPath">The model path</param>
        /// <param name="texturePath">The path.</param>
        /// <returns></returns>
        protected virtual string OnLoadTexture(string modelPath, string texturePath)
        {
            try
            {
                var dict = Path.GetDirectoryName(modelPath);
                if (string.IsNullOrEmpty(dict))
                {
                    dict = Directory.GetCurrentDirectory();
                }
                var p = Path.GetFullPath(Path.Combine(dict, texturePath));
                if (!FileExists(p))
                    p = HandleTexturePathNotFound(dict, texturePath);
                if (!FileExists(p))
                {
                    Log(LogLevel.Warning, $"Load Texture Failed. Texture Path = {texturePath}.");
                    return null;
                }
                return p;
            }
            catch (Exception ex)
            {
                Log(LogLevel.Warning, $"Load Texture Exception. Texture Path = {texturePath}. Exception: {ex.Message}");
            }
            return null;
        }

        /// <summary>
        /// Handles the texture path not found. Override to provide your own handling
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <param name="texturePath">The texture path.</param>
        /// <returns></returns>
        protected virtual string HandleTexturePathNotFound(string dir, string texturePath)
        {
            //If file not found in texture path dir, try to find the file in the same dir as the model file
            if (texturePath.StartsWith(ToUpperDictString))
            {
                var t = texturePath.Remove(0, ToUpperDictString.Length);
                var p = Path.GetFullPath(Path.Combine(dir, t));
                if (FileExists(p))
                    return p;
            }

            //If still not found, try to go one upper level and find
            var upper = Directory.GetParent(dir).FullName;
            try
            {
                upper = Path.GetFullPath(upper + texturePath);
            }
            catch (NotSupportedException ex)
            {
                Log(LogLevel.Warning, $"Exception: {ex}");
            }
            if (FileExists(upper))
                return upper;
            var fileName = Path.GetFileName(texturePath);
            var currentPath = Path.Combine(dir, fileName);
            if (FileExists(currentPath))
            {
                return currentPath;
            }
            return "";
        }

        protected virtual bool FileExists(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// Logs the specified level.
        /// </summary>
        /// <typeparam name="Type">The type of the ype.</typeparam>
        /// <param name="level">The level.</param>
        /// <param name="msg">The MSG.</param>
        /// <param name="caller">The caller.</param>
        /// <param name="sourceLineNumber">The source line number.</param>
        protected void Log<Type>(LogLevel level, Type msg, [CallerMemberName]string caller = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            Logger.Log(level, msg, nameof(EffectsManager), caller, sourceLineNumber);
        }
    }
}
