// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GZipHelper.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.IO;
    using System.IO.Compression;

    /// <summary>
    /// Provides a method for compressing files using the Gzip stream.
    /// </summary>
    public class GZipHelper
    {
        #region Public Methods

        /// <summary>
        /// Compresses a file using standard zlib compression. 
        ///   A "z" is added to the extension for the compressed file.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        public static void Compress(string source)
        {
            var ext = Path.GetExtension(source);
            var infile = File.OpenRead(source);
            var input = new byte[infile.Length];
            infile.Read(input, 0, input.Length);
            infile.Close();

            var dest = Path.ChangeExtension(source, ext + "z");
            var outfile = File.OpenWrite(dest);
            var zip = new GZipStream(outfile, CompressionMode.Compress);
            zip.Write(input, 0, input.Length);
            zip.Close();
            outfile.Close();
        }

        #endregion
    }
}