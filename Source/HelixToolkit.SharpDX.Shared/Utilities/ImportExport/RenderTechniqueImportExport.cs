/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
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
    using Shaders;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;

    public static class ShaderExporter
    {
        /// <summary>
        /// Exports the techniques.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="writer">The writer.</param>
        /// <returns></returns>
        public static int ExportTechniques(this IEffectsManager manager, XmlWriter writer)
        {
            var ser = new DataContractSerializer(typeof(List<TechniqueDescription>));
            var techniques = new List<TechniqueDescription>();
            foreach (var techniqueName in manager.RenderTechniques)
            {
                var technique = manager[techniqueName];
                techniques.Add(technique.Description);
            }
            ser.WriteObject(writer, techniques);
            writer.Flush();
            return techniques.Count;
        }
        /// <summary>
        /// Exports the technique.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="techniqueName">Name of the technique.</param>
        /// <param name="writer">The writer.</param>
        public static void ExportTechnique(this IEffectsManager manager, string techniqueName, XmlWriter writer)
        {
            var ser = new DataContractSerializer(typeof(List<TechniqueDescription>));
            var technique = manager[techniqueName].Description;
            var techniques = new List<TechniqueDescription>();
            techniques.Add(technique);
            ser.WriteObject(writer, techniques);
            writer.Flush();
        }
        /// <summary>
        /// Exports the techniques as binary.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        public static int ExportTechniquesAsBinary(this IEffectsManager manager, string filePath)
        {
            using (var memory = new MemoryStream())
            {
                using (var binaryXMLWriter = XmlDictionaryWriter.CreateBinaryWriter(memory))
                {
                    int count = ExportTechniques(manager, binaryXMLWriter);
                    using (var binaryWriter = File.Open(filePath, FileMode.Create))
                    {
                        binaryWriter.Write(memory.ToArray(), 0, (int)memory.Length);
                    }
                    return count;
                }
            }
        }

        /// <summary>
        /// Exports the technique as binary.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="techniqueName">Name of the technique.</param>
        /// <param name="filePath">The file path.</param>
        public static void ExportTechniqueAsBinary(this IEffectsManager manager, string techniqueName, string filePath)
        {
            using (var memory = new MemoryStream())
            {
                using (var binaryXMLWriter = XmlDictionaryWriter.CreateBinaryWriter(memory))
                {
                    ExportTechnique(manager, techniqueName, binaryXMLWriter);
                    using (var binaryWriter = File.Open(filePath, FileMode.Create))
                    {
                        binaryWriter.Write(memory.ToArray(), 0, (int)memory.Length);
                    }
                }
            }
        }

        /// <summary>
        /// Imports the techniques. Make sure to detach effectsManager from viewport before importing
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="append">if set to <c>true</c> [append techniques into effects manager]. Otherwise replace all existing techniques</param>
        /// <returns></returns>
        public static int ImportTechniques(this IEffectsManager manager, XmlReader reader, bool append = true)
        {
            var ser = new DataContractSerializer(typeof(List<TechniqueDescription>));
            var techniques = ser.ReadObject(reader) as List<TechniqueDescription>;
            if (!append)
            {
                manager.RemoveAllTechniques();
            }
            int count = 0;
            foreach(var t in techniques)
            {
                if (append)
                {
                    manager.RemoveTechnique(t.Name);
                }
                manager.AddTechnique(t);
                ++count;
            }
            return count;
        }

        /// <summary>
        /// Imports the techniques. Make sure to detach effectsManager from viewport before importing
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="append">if set to <c>true</c> [append techniques into effects manager]. Otherwise replace all existing techniques</param>
        /// <returns></returns>
        public static int ImportTechniques(this IEffectsManager manager, string filePath, bool append = true)
        {
            using (var reader = File.OpenRead(filePath))
            {
                using (var memory = new MemoryStream())
                {
                    reader.CopyTo(memory);
                    memory.Position = 0;
                    using (var binaryXMLReader = XmlDictionaryReader.CreateBinaryReader(memory, new XmlDictionaryReaderQuotas() { MaxArrayLength = (int)memory.Length }))
                    {
                        return ImportTechniques(manager, binaryXMLReader, append);
                    }
                }               
            }
        }
    }
}
