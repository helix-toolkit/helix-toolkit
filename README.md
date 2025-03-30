[![HelixToolkit](https://img.shields.io/badge/-Helix%20Toolkit-blue)](https://github.com/helix-toolkit/helix-toolkit) 

<img src='https://avatars3.githubusercontent.com/u/8432523?s=200&v=4' width='64' />

# Helix Toolkit

**Helix Toolkit is a collection of 3D components for .NET Framework.**

[**HelixToolkit.WPF:**](/Source/HelixToolkit.Wpf)
Adds variety of functionalities/models on the top of internal WPF 3D models (Media3D namespace).

[**HelixToolkit.SharpDX.WPF:**](/Source/HelixToolkit.Wpf.SharpDX)
Custom 3D Engine and XAML/MVVM compatible Scene Graphs based on [SharpDX](https://github.com/sharpdx/SharpDX)(DirectX 11) for WPF.

[**HelixToolkit.SharpDX.WinUI:**](/Source/HelixToolkit.WinUI.SharpDX)
Custom 3D Engine and XAML/MVVM compatible Scene Graphs based on [SharpDX](https://github.com/sharpdx/SharpDX)(DirectX 11) for WinUI.

[**HelixToolkit.SharpDX.Assimp:**](/Source/HelixToolkit.SharpDX.Assimp)
[Assimp.Net](https://bitbucket.org/Starnick/assimpnet/src/master/) 3D model importer/expoter support for HelixToolkit.SharpDX Components.

[**Examples:**](/Source/Examples)
Please download full source code to run examples.

[![License: MIT](https://img.shields.io/github/license/helix-toolkit/helix-toolkit)](https://github.com/helix-toolkit/helix-toolkit/blob/develop/LICENSE)
[![Build status](https://ci.appveyor.com/api/projects/status/vbrornad55ln8tp4?svg=true)](https://ci.appveyor.com/project/holance/helix-toolkit-qqf1e)
[![Release](https://img.shields.io/github/release/helix-toolkit/helix-toolkit.svg?style=popout)](https://www.nuget.org/packages?q=Helix-Toolkit)
[![Chat](https://img.shields.io/gitter/room/helix-toolkit/helix-toolkit.svg)](https://gitter.im/helix-toolkit/helix-toolkit)

Description         | Value
--------------------|-----------------------
Web page            | http://helix-toolkit.github.io/
Wiki                | https://github.com/helix-toolkit/helix-toolkit/wiki
Documentation       | http://helix-toolkit.readthedocs.io/
Chat                | https://gitter.im/helix-toolkit/helix-toolkit
Source repository   | http://github.com/helix-toolkit/helix-toolkit
Latest build        | http://ci.appveyor.com/project/holance/helix-toolkit
Issue tracker       | http://github.com/helix-toolkit/helix-toolkit/issues
NuGet packages      | http://www.nuget.org/packages?q=HelixToolkit
Nightly build       | https://www.myget.org/F/helixtoolkit-nightly
StackOverflow       | http://stackoverflow.com/questions/tagged/helix-3d-toolkit
Twitter             | https://twitter.com/hashtag/Helix3DToolkit

## Project Build

**Visual Studio 2022.**

## Notes

#### 1. Right-handed Cartesian coordinate system and row major matrix by default
HelixToolkit default is using right-handed Cartesian coordinate system, including Meshbuilder etc. To use left-handed Cartesian coordinate system (Camera.CreateLeftHandedSystem = true), user must manually correct the triangle winding order or IsFrontCounterClockwise in raster state description if using SharpDX. Matrices are row major by default.

#### 2. Performance [Topics](https://github.com/helix-toolkit/helix-toolkit/wiki/Tips-on-performance-optimization-(WPF.SharpDX-and-UWP)) for WPF.SharpDX and UWP.

#### 3. Following features are not supported currently on FeatureLevel 10 graphics card:
FXAA, Order Independant Transparent Rendering, Particle system, Tessellation.

#### 4. [Wiki](https://github.com/helix-toolkit/helix-toolkit/wiki) and useful [External Resources](https://github.com/helix-toolkit/helix-toolkit/wiki/External-References) on Computer Graphics.

## HelixToolkit Package Dependencies

```mermaid
flowchart TD
 subgraph s1["DirectX 11 Engine"]
        n6["HelixToolkit.SharpDX"]
        n7["HelixToolkit.SharpDX.Assimp"]
        n8["HelixToolkit.Wpf.SharpDX"]
        n9["HelixToolkit.WinUI.SharpDX"]
  end
 subgraph s2["WPF 3D Engine"]
        n10["HelixToolkit.Wpf"]
        n11["HelixToolkit.Wpf.TDxInput"]
  end
    hx["HelixToolkit"] --> n1["HelixToolkit.Maths"]
    n1 --> n2["HelixToolkit.Geometry"]
    n6 --> n7 & n8 & n9
    n10 --> n11
    n2 --> s2
    n2 --> s1

    n2@{ shape: rect}
```

## Bug Report
Please use the following template to report bugs.

- Version: [Example: 2.20]
- Package: [Example: Helixtoolkit.Wpf]
- Issue: 
- Reproduce Steps:
- Sample Code:

## News
#### 2025-03-30
[v2.27.0](https://github.com/helix-toolkit/helix-toolkit/releases/tag/v2.27.0) releases are available on nuget. [Release Note](/CHANGELOG.md)

:bangbang: This is the final release of HelixToolkit v2. Moving forward, our focus will shift to the development of v3.

- [WPF](https://www.nuget.org/packages/HelixToolkit.Wpf/2.27.0)
- [Core.WPF](https://www.nuget.org/packages/HelixToolkit.Core.Wpf/2.27.0)
- [WPF.Input](https://www.nuget.org/packages/HelixToolkit.Wpf.Input/2.27.0)
- [WPF.SharpDX](https://www.nuget.org/packages/HelixToolkit.Wpf.SharpDX/2.27.0)
- [UWP](https://www.nuget.org/packages/HelixToolkit.UWP/2.27.0)
- [SharpDX.Core](https://www.nuget.org/packages/HelixToolkit.SharpDX.Core/2.27.0)
- [SharpDX.Core.Wpf](https://www.nuget.org/packages/HelixToolkit.SharpDX.Core.Wpf/2.27.0)
- [WinUI](https://www.nuget.org/packages/HelixToolkit.WinUI/2.27.0)
- [SharpDX.Assimp](https://www.nuget.org/packages/HelixToolkit.SharpDX.Assimp/2.27.0)

#### Changes (Please refer to [Release Note](https://github.com/helix-toolkit/helix-toolkit/blob/master/CHANGELOG.md) for details)

#### 2023-03-17
Nightly build myget feed link has been updated to: https://www.myget.org/F/helixtoolkit-nightly

