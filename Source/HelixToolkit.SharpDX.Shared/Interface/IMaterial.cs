/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.ComponentModel;
using System.IO;
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
    using Model;
    /// <summary>
    /// 
    /// </summary>
    public interface IMaterial : INotifyPropertyChanged
    {
        string Name { set; get; }
        Guid Guid { get; }
        MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique);
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IMaterialVariablePool
    {
        int Count { get; }
        MaterialVariable Register(IMaterial material, IRenderTechnique technique);
    }
}
