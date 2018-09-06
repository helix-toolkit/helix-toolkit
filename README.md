<img src='https://avatars3.githubusercontent.com/u/8432523?s=200&v=4' width='64' />

# Helix Toolkit

**Helix Toolkit is a collection of 3D components for .NET Framework.**

[**HelixToolkit.WPF:**](https://github.com/helix-toolkit/helix-toolkit/tree/develop/Source/HelixToolkit.Wpf) 
Adds variety of functionalities/models on the top of internal WPF 3D model (Media3D namespace). 

[**HelixToolkit.SharpDX.WPF:**](https://github.com/helix-toolkit/helix-toolkit/tree/develop/Source/HelixToolkit.Wpf.SharpDX) 
3D Components and XAML/MVVM compatible Scene Graphs based on [SharpDX](https://github.com/sharpdx/SharpDX)(DirectX 11) for high performance usage.

[**HelixToolkit.UWP:**](https://github.com/helix-toolkit/helix-toolkit/tree/develop/Source/HelixToolkit.UWP) 
3D Components and XAML/MVVM compatible Scene Graphs based on [SharpDX](https://github.com/sharpdx/SharpDX)(DirectX 11) for Universal Windows App.

[**Examples:**](https://github.com/helix-toolkit/helix-toolkit/tree/develop/Source/Examples)
Please download full source code to run examples. Or download [compiled version](https://ci.appveyor.com/project/objorke/helix-toolkit/branch/develop/artifacts)

[![Build status](https://ci.appveyor.com/api/projects/status/tmqafdk9p7o98gw7?svg=true)](https://ci.appveyor.com/project/objorke/helix-toolkit)

Description         | Value
--------------------|-----------------------
License             | The MIT License (MIT)
Web page            | http://helix-toolkit.org/
Documentation       | http://docs.helix-toolkit.org/
Forum               | http://forum.helix-toolkit.org/
Chat                | https://gitter.im/helix-toolkit/helix-toolkit
Source repository   | http://github.com/helix-toolkit/helix-toolkit
Latest build        | http://ci.appveyor.com/project/objorke/helix-toolkit
Issue tracker       | http://github.com/helix-toolkit/helix-toolkit/issues
NuGet packages      | http://www.nuget.org/packages?q=HelixToolkit
MyGet feed          | https://www.myget.org/F/helix-toolkit
StackOverflow       | http://stackoverflow.com/questions/tagged/helix-3d-toolkit
Twitter             | https://twitter.com/hashtag/Helix3DToolkit

## Project Build

**Visual Studio 2017. Windows 10 SDK (Min Ver.10.0.10586.0).**

Windows 10 SDK **Ver.10.0.10586.0** can be selected and installed using Visual Studio 2017 installer.If you installed the higher version only, please change the target version in **HelixToolkit.Native.ShaderBuilder** property to the proper version installed on your machine.

## Notes

#### 1. Right-handed Cartesian coordinate system and row major matrix by default
HelixToolkit default is using right-handed Cartesian coordinate system, including Meshbuilder etc. To use left-handed Cartesian coordinate system (Camera.CreateLeftHandedSystem = true), user must manually correct the triangle winding order or IsFrontCounterClockwise in raster state description if using SharpDX. Matrices are row major by default.

#### 2. Performance [Topics](https://github.com/helix-toolkit/helix-toolkit/wiki/Tips-on-performance-optimization-(WPF.SharpDX-and-UWP)) for WPF.SharpDX and UWP.

#### 3. Following features are not supported currently on FeatureLevel 10 graphics card:
FXAA, Order Independant Transparent Rendering, Particle system, Tessellation.

#### 4. [Wiki](https://github.com/helix-toolkit/helix-toolkit/wiki)

## News

#### 2018-08-26
[V2.4.0](https://github.com/helix-toolkit/helix-toolkit/tree/release/2.4.0) releases are available on nuget. [Release Note](https://github.com/helix-toolkit/helix-toolkit/blob/master/CHANGELOG.md)
- [WPF](https://www.nuget.org/packages/HelixToolkit.Wpf/2.4.0)
- [WPF.SharpDX](https://www.nuget.org/packages/HelixToolkit.Wpf.SharpDX/2.4.0)
- [UWP](https://www.nuget.org/packages/HelixToolkit.UWP/2.4.0)

#### Changes (Please refer to [Release Note](https://github.com/helix-toolkit/helix-toolkit/blob/master/CHANGELOG.md) for details)

#### 2018-07-22
[V2.3.0](https://github.com/helix-toolkit/helix-toolkit/tree/release/2.3.0) releases are available on nuget. [Release Note](https://github.com/helix-toolkit/helix-toolkit/blob/master/CHANGELOG.md)
- [WPF](https://www.nuget.org/packages/HelixToolkit.Wpf/2.3.0)
- [WPF.SharpDX](https://www.nuget.org/packages/HelixToolkit.Wpf.SharpDX/2.3.0)
- [UWP](https://www.nuget.org/packages/HelixToolkit.UWP/2.3.0)

#### 2018-06-17
[V2.2.0](https://github.com/helix-toolkit/helix-toolkit/tree/release/2.2.0) releases are available on nuget. [Release Note](https://github.com/helix-toolkit/helix-toolkit/blob/master/CHANGELOG.md)
- [WPF](https://www.nuget.org/packages/HelixToolkit.Wpf/2.2.0)
- [WPF.SharpDX](https://www.nuget.org/packages/HelixToolkit.Wpf.SharpDX/2.2.0)
- [UWP](https://www.nuget.org/packages/HelixToolkit.UWP/2.2.0)

##### Note: 2.0 Breaking changes from version 1.x.x. (HelixToolkit.SharpDX only) see [ChangeLog](https://github.com/helix-toolkit/helix-toolkit/blob/develop/CHANGELOG.md)
