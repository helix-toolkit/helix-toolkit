<img src='https://avatars3.githubusercontent.com/u/8432523?s=200&v=4' width='64' />

# Helix Toolkit

**Helix Toolkit is a collection of 3D components for .NET Framework.**

[**HelixToolkit.WPF:**](https://github.com/helix-toolkit/helix-toolkit/tree/develop/Source/HelixToolkit.Wpf) 
Adds variety of functionalities/models on the top of internal WPF 3D model (Media3D namespace). 

[**HelixToolkit.SharpDX.WPF:**](https://github.com/helix-toolkit/helix-toolkit/tree/develop/Source/HelixToolkit.Wpf.SharpDX) 
3D Components and XAML/MVVM compatible Scene Graphs based on [SharpDX](https://github.com/sharpdx/SharpDX)(DirectX 11) for high performance usage.

[**HelixToolkit.UWP:**](https://github.com/helix-toolkit/helix-toolkit/tree/develop/Source/HelixToolkit.UWP) 
3D Components and XAML/MVVM compatible Scene Graphs based on [SharpDX](https://github.com/sharpdx/SharpDX)(DirectX 11) for Universal Windows App.

[![Build status](https://ci.appveyor.com/api/projects/status/tmqafdk9p7o98gw7)](https://ci.appveyor.com/project/objorke/helix-toolkit)

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

**Visual Studio 2017. Windows 10 SDK.**

Missing **fxc.exe** issue with newest Windows 10 SDK:

Copy **fxc.exe** in **C:\Program Files (x86)\Windows Kits\10\Bin\10.0.xxx\x86** to **C:\Program Files (x86)\Windows Kits\10\bin\x86** to fix this issue. Because the HLSL compile tool hard coded the path.

## Notes

#### 1. Laptops with Nvidia Optimus (Dual graphics card)(HelixToolkit.SharpDX Only)
Auto adapter selection in EffectsManager does not guarantee to use external Nvidia graphic card for rendering. To make sure using the Nvidia graphic card, add *`static NVOptimusEnabler nvEnabler = new NVOptimusEnabler();`* in *`MainWindow.xaml.cs`*.

#### 2. Using [RenderDoc](https://github.com/baldurk/renderdoc) for SharpDX render profiling
To use RenderDoc, following settings must be set on Viewport3DX. 
```
EnableSwapChainRendering = true;
EnableD2DRendering = false;
```

## News

We are currently working on HelixToolkit 2.0 under develop branch. Mainly focus on HelixToolkit.SharpDX.WPF and HelixToolkit.UWP.

Unstable prereleased UWP nuget package is available in [MyGet Feed](https://www.myget.org/F/helix-toolkit).

All 1.x.x related pull requests, please use [1.1.0/Release](https://github.com/helix-toolkit/helix-toolkit/tree/release/1.1.0) branch.

#### Note: 2.0 Breaking changes from version 1.x.x. (HelixToolkit.SharpDX only)
1. New architecture for backend rendering and shader management. No more dependency from obsoleted Effects framework. EffectsManager is mandatory to be provided from ViewModel for resource live cycle management by user to avoid memory leak.
2. Many performance improvements. Viewports binding with same EffectsManager will share common resources. Models binding with same geometry3D will share same geometry buffers. Materials binding with same texture will share same resources.
3. Support basic direct2d rendering and layouts arrangement. (Still needs a lot of implementations)
4. No more HelixToolkit.WPF project dependency.
5. Unify dependency property types. All WPF.SharpDx model's dependency properties are using class under System.Windows.Media. Such as Vector3D and Color. More Xaml friendly.
6. Post effect support.
7. Supports transparent meshes rendered after opaque meshes. IsTransparent property is added in MaterialGeometryModel3D.
8. Rendering order by RenderType flag: 
    ##### Pre(such as shadow map)->Opaque->Particle->Transparent->Post(post effects)->ScreenSpaced(ViewBox/CoordinateSystem).
9. Core implementation are separated from platform dependent controls(Element3D) into its own Scene Node classes. Scene Node serves as complete Scene Graph for traversal inside render host. Element3D will only be used as a wrapper to manipulate scene node properties from XAML.
10. Other on going changes.

#### 2018-02-06

V1.1.0 release is available.

https://www.nuget.org/packages/HelixToolkit.Wpf/1.1.0

https://www.nuget.org/packages/HelixToolkit.Wpf.SharpDX/1.1.0

V1.1.0 Relase source code is under : https://github.com/helix-toolkit/helix-toolkit/tree/release/1.1.0
