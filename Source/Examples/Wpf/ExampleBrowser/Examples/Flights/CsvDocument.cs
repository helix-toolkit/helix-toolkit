using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System;

namespace Flights;

public sealed class CsvDocument<T> where T : class
{
    public IList<T> Items { get; private set; }

    public CsvDocument()
    {
        Items = new List<T>();
    }

    public void Load(string path, char separator = ';')
    {
        using var stream = File.OpenRead(path);

        Load(stream, separator);
    }

    public void Load(Stream stream, char separator = ';')
    {
        using var r = new StreamReader(stream);

        string? header = r.ReadLine();
        string[] headerfields = header?.Split(separator) ?? Array.Empty<string>();
        var itemType = typeof(T);
        var pi = new PropertyInfo?[headerfields.Length];
        for (int i = 0; i < headerfields.Length; i++)
        {
            pi[i] = itemType.GetProperty(headerfields[i]);
        }
        while (!r.EndOfStream)
        {
            var line = r.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(line))
                continue;
            if (line!.StartsWith("%") || line!.StartsWith("//"))
                continue;
            var fields = line.Split(separator);
            var item = Activator.CreateInstance(typeof(T)) as T;
            for (int i = 0; i < fields.Length && i < pi.Length; i++)
            {
                if (pi[i] == null)
                    continue;
                object value = fields[i];
                if (pi[i]?.PropertyType == typeof(double))
                {
                    if (double.TryParse(fields[i], NumberStyles.Number, CultureInfo.InvariantCulture, out double d))
                        value = d;
                    else
                    {
                        Trace.WriteLine("Could not parse '" + fields[i] + "'.");
                    }
                }
                pi[i]?.SetValue(item, value, null);
            }

            if (item is not null)
            {
                Items.Add(item);
            }
        }
    }
}
