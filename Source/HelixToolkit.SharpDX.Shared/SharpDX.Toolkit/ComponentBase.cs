// Copyright (c) 2010-2014 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.ComponentModel;

namespace SharpDX.Toolkit
{
    /// <summary>
    /// A lightweight Component base class.
    /// </summary>
    public abstract class ComponentBase : IComponent, INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs while this component is disposing and before it is disposed.
        /// </summary>
        //internal event EventHandler<EventArgs> Disposing;
        private string name;

        /// <summary>
        /// Gets or sets a value indicating whether the name of this instance is immutable.
        /// </summary>
        /// <value><c>true</c> if this instance is name immutable; otherwise, <c>false</c>.</value>
        private readonly bool isNameImmutable;

        private object tag;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentBase" /> class with a mutable name.
        /// </summary>
        protected ComponentBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentBase" /> class with an immutable name.
        /// </summary>
        /// <param name="name">The name.</param>
        protected ComponentBase(string name)
        {
            if (name != null)
            {
                this.name = name;
                this.isNameImmutable = true;
            }
        }

        /// <summary>
        /// Gets the name of this component.
        /// </summary>
        /// <value>The name.</value>
        [DefaultValue(null)]
        public string Name
        {
            get { return name; }
            set
            {
                if (isNameImmutable)
                    throw new ArgumentException("Name property is immutable for this instance", "value");
                if (name == value) return;
                name = value;
                OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// Gets or sets the tag associated to this object.
        /// </summary>
        /// <value>The tag.</value>
        [Browsable(false)]
        [DefaultValue(null)]
        public object Tag
        {
            get
            {
                return tag;
            }
            set
            {
                if (ReferenceEquals(tag, value)) return;
                tag = value;
                OnPropertyChanged("Tag");
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}