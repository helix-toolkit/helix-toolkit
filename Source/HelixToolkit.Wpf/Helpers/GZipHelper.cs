﻿using System.IO;
using System.IO.Compression;

namespace HelixToolkit.Wpf;

/// <summary>
/// Provides a method for compressing files using the Gzip stream.
/// </summary>
public static class GZipHelper
{
    /// <summary>
    /// Compresses a file using standard zlib compression.
    /// A "z" is added to the extension for the compressed file.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    public static void Compress(string source)
    {
        var ext = Path.GetExtension(source);
        byte[] input;
        using (var infile = File.OpenRead(source))
        {
            input = new byte[infile.Length];
            _ = infile.Read(input, 0, input.Length);
        }

        var dest = Path.ChangeExtension(source, ext + "z");
        using (var outfile = File.OpenWrite(dest))
        {
            var zip = new GZipStream(outfile, CompressionMode.Compress);
            zip.Write(input, 0, input.Length);
        }
    }
}
