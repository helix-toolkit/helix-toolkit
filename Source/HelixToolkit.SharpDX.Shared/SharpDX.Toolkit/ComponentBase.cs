/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
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
            get
            {
                return name;
            }
            set
            {
                if (isNameImmutable)
                    throw new ArgumentException("Name property is immutable for this instance", "value");
                if (name == value)
                    return;
                name = value;
                OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// Gets or sets the tag associated to this object.
        /// </summary>
        /// <value>The tag.</value>
#if !CORE
        [Browsable(false)]
#endif
        [DefaultValue(null)]
        public object Tag
        {
            get
            {
                return tag;
            }
            set
            {
                if (ReferenceEquals(tag, value))
                    return;
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
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}