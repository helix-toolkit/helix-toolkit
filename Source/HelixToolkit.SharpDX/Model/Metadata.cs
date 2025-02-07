﻿/*
* Copyright (c) 2012-2018 AssimpNet - Nicholas Woodfield
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

using SharpDX;

namespace HelixToolkit.SharpDX.Model;

/// <summary>
/// Represents a container for holding metadata, representing as key-value pairs.
/// </summary>
public sealed class Metadata : Dictionary<string, Metadata.Entry>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Metadata"/> class.
    /// </summary>
    public Metadata()
    {

    }
    /// <summary>
    /// Initializes a new instance of the <see cref="Metadata"/> class.
    /// </summary>
    /// <param name="capacity">The capacity.</param>
    public Metadata(int capacity) : base(capacity)
    {

    }
    /// <summary>
    /// 
    /// </summary>
    public readonly struct Entry : IEquatable<Entry>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Entry"/> struct.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="data">The data.</param>
        public Entry(MetaDataType dataType, object data)
        {
            DataType = dataType;
            Data = data;
        }
        /// <summary>
        /// Gets the type of the data.
        /// </summary>
        /// <value>
        /// The type of the data.
        /// </value>
        public MetaDataType DataType
        {
            get;
        }
        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public object Data
        {
            get;
        }

        public T? DataAs<T>() where T : unmanaged
        {
            Type? dataTypeType = null;
            switch (DataType)
            {
                case MetaDataType.Bool:
                    dataTypeType = typeof(bool);
                    break;
                case MetaDataType.Float:
                    dataTypeType = typeof(float);
                    break;
                case MetaDataType.Double:
                    dataTypeType = typeof(double);
                    break;
                case MetaDataType.Int32:
                    dataTypeType = typeof(int);
                    break;
                case MetaDataType.String:
                    dataTypeType = typeof(string);
                    break;
                case MetaDataType.UInt64:
                    dataTypeType = typeof(ulong);
                    break;
                case MetaDataType.Vector3D:
                    dataTypeType = typeof(Vector3);
                    break;
            }

            if (dataTypeType == typeof(T))
                return (T)Data;

            return null;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Entry e)
            {
                return e.Equals(this);
            }
            return false;
        }

        public bool Equals(Entry other)
        {
            return other.DataType == DataType && other.Data.Equals(Data);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = (hash * 31) + Data.GetHashCode();
                hash = (hash * 31) + ((Data == null) ? 0 : Data.GetHashCode());

                return hash;
            }
        }

        public override string ToString()
        {
            return $"Type:{DataType}; Value:{Data}";
        }

        public static bool operator ==(Entry a, Entry b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Entry a, Entry b)
        {
            return !a.Equals(b);
        }
    }
}
