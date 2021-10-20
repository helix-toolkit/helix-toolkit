/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System.Runtime.Serialization;

#if NETFX_CORE
using  Windows.UI.Xaml;

namespace HelixToolkit.UWP
#elif WINUI
using Microsoft.UI.Xaml;
using HelixToolkit.SharpDX.Core.Model;
namespace HelixToolkit.WinUI
#else
using System.ComponentModel;
using System.Windows;
#if COREWPF
using HelixToolkit.SharpDX.Core.Model;
#endif
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model;

    [DataContract]
#if NETFX_CORE || WINUI
    public abstract class Material : DependencyObject
#else
    public abstract class Material : Freezable
#endif
    {
        private MaterialCore core;

        public MaterialCore Core
        {
            get
            {
                if (core == null)
                {
                    core = OnCreateCore();
                }
                return core;
            }
        }

        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(Material), new PropertyMetadata(null,
                (d, e) => { (d as Material).Core.Name = (string)e.NewValue; }));

        public string Name
        {
            get
            {
                return (string)this.GetValue(NameProperty);
            }
            set
            {
                this.SetValue(NameProperty, value);
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Material"/> class.
        /// </summary>
        public Material()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Material"/> class.
        /// </summary>
        /// <param name="core">The core.</param>
        public Material(MaterialCore core)
        {
            this.core = core;
            Name = core.Name;
        }

        public override string ToString()
        {
            return Name;
        }

        protected abstract MaterialCore OnCreateCore();

        public static implicit operator MaterialCore(Material m)
        {
            return m?.Core;
        }
    }
}
