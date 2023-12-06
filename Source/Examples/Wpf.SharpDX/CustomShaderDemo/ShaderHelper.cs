using System;
using System.IO;

namespace CustomShaderDemo;

public static class ShaderHelper
{
    public static byte[] LoadShaderCode(string path)
    {
        if (!File.Exists(path))
        {
            throw new ArgumentException($"Shader File not found: {path}");
        }

        return File.ReadAllBytes(path);
    }
}
