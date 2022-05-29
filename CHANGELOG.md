# Change Log
All notable changes to this project will be documented in this file.

## [2.21.0] - 2022-05-28
We did some major code refactoring and changes in this release.
### New
1. Support Depth peeling based order independent transparency rendering. (WPF.SharpDX/UWP/Core)

### Improvement
1. Improved constant buffer array management in `MaterialVariable` and `ConstantBufferComponent`. Use single array to hold all structs used by material with same size.(WPF.SharpDX/UWP/Core)
1. Simplified `DisposeObject` base class. Remove `Collect` function and object dictionary. All graphics resources must call `RemoveAndDispose` to free either on `OnDispose` if the resource is created in constructor or `OnDetach` if the resource is created in `OnAttach`. (WPF.SharpDX/UWP/Core)
1. Improved all buffer upload functions to avoid creating `DataStream` object to reduce GC during rendering.(WPF.SharpDX/UWP/Core)
1. Upgraded the Dot Net minimum requirements from **4.5** to **4.6** on `HelixToolkit.SharpDX.Wpf`. (WPF.SharpDX)
1. Upgraded net standard from **1.1** to **1.3** on `Helixtoolkit.SharpDX.Core`. (Core)
1. Improved resource pool implementation. (WPF.SharpDX/UWP/Core)
1. Relax manipulator binding target type to Elements instead of GeometryModel3D (WPF.SharpDX)

### Fixed
1. Fix bone skin rendering crash. (WPF.SharpDX/UWP/Core)
1. Update HelixToolkit.WinUI to support Windows App SDK 1.0 (WinUI)

## [2.20.2] - 2022-02-01
### Fixed
1. Fix viewcube edge hit test is not working properly. #1702 (WPF.SharpDX/UWP/Core)
1. Fix environment map getting black area under certain conditions when using orthographic camera. (WPF.SharpDX/UWP/Core)
1. Fix null pointer exception if effects manager is not being set on viewport3DX. (WPF.SharpDX/UWP/Core)

## [2.20.1] - 2022-01-19
### Fixed
1. Fixed stl reader failed to read any ASCii files on Dot net 5. (WPF)

## [2.20.0] - 2021-10-31
### Added
1. Post effect support in screen spaced group. (WPF.SharpDX/UWP/Core)
1. Supports software rendering as config for effects manager. (WPF.SharpDX/UWP/Core)

## [2.19.0] - 2021-10-11
### Fixed
1. Fix shadow is showing on the back of the object #1649 (WPF.SharpDX/UWP/Core)
1. Fix UWP memory leak due to event handlers are not being released properly. #1185 (UWP)

## [2.18.0] - 2021-09-04
### Added
1. Add hit test support for AxisPlaneGridNode.(WPF.SharpDX/UWP/Core)
1. Add walkaround camera mode support for Orthographic camera. (WPF.SharpDX/UWP/Core)
1. Cursor position and Cursor On Element position added to viewport3DX (WPF.SharpDX)

### Improvement
1. Adds Batched Mesh hit test result to include more batched mesh specific properties. (WPF.SharpDX/UWP/Core)

### Fixed
1. Fix zooming flips by enabling ZoomAroundMouseDownPoint in UWP.(UWP/Core)
1. Fix topmost group is not rendered correctly if screenspaced group is being rendered before it.(WPF.SharpDX/UWP/Core)
1. Fix duplicate key exception while importing glb file with embedded texture. (WPF.SharpDX/UWP/Core)
1. Fix embedded texture is not loaded from FBX. (WPF.SharpDX/UWP/Core)
1. Missing property in HitTest.ShallowCopy.  (WPF.SharpDX/UWP/Core)

## [2.17.0] - 2021-06-20
### Added
1. Adds `TopMostGroupNode` and `TopMostGroup3D` to support top most rendering. Ref: #1572 (***Limitations:*** Top most meshes are rendered at same level as screen spaced items, which are not supported by post effects and render ordering.) (WPF.SharpDX/UWP/Core)
1. Add supprot for shadow map to automatically cover complete scene. (***Limitations:*** May not be able to properly cover the scene if contains boneskinned animation.) (WPF.SharpDX/UWP/Core) 

### Fixed
1. Added null check in IRenderMetricesExtensions.UnProject().(WPF.SharpDX/UWP/Core)
1. Fix BatchedMeshGeometryModel3D blinking when BatchedGeometries update.(WPF.SharpDX/UWP/Core)
1. Small problem in PointAndLinesBinding example, SetPoints() method.(WPF)
1. Fixed glitches in various examples and project build.(WPF, WPF.SharpDX)
1. Fix memory leak if same reference counted object gets collected multiple times.(WPF.SharpDX/UWP/Core)

## [2.16.1] - 2021-05-02
### Fixed
1. Fix RenderContext.BoundingFrustum for non-perspective cameras.  (WPF.SharpDX/UWP/Core) 
1. Handle too many SurfaceD3D_IsFrontBufferAvailableChanged.  (WPF.SharpDX/UWP/Core) 

## [2.16.0] - 2021-04-24
### Added
1. Adds coordinate system axis color dependency properties for Viewport3DX. (WPF.SharpDX/UWP)
1. Support for loading .obj and .mtl from stream. (WPF)

### Improvement
1. Avoid GPU resources getting destroyed and re-created unnecessarily.  (WPF.SharpDX/UWP/Core) 
1. Improves texture loading. Re-implemented `TextureModel` and provides `ITextureInfoLoader` interface to allow user defined texture repository.(WPF.SharpDX/UWP/Core) 
1. Auto caching `Stream` and `TextureModel` pair to avoid duplicated texture resources.(WPF.SharpDX/UWP/Core) 
1. `TextureModel` changes to be `Guid` based. `TextureModel` with same `Guid` will be treated as same texture.(WPF.SharpDX/UWP/Core) 
1. Aggregate hit test function parameters into single hit test context. (WPF.SharpDX/UWP/Core) 
1. Move FXAA to the end of rendering, so FXAA applies onto screen spaced object. (WPF.SharpDX/UWP/Core)
1. Add preliminary hit check with hit thickness for PointNode. (WPF.SharpDX/UWP/Core)

### Fixed
1. Fixed viewport crash during display configuration change #1531. (WPF.SharpDX)
1. Fixed cursor is wrong after pressing multiple mouse button simultaneously (WPF.SharpDX/UWP) 
1. Bugfix export without material (Assimp)
1. Fixed bounding box is not updated properly. #1555 (WPF.SharpDX/UWP/Core) 
1. Fixed Frustum test bug. (WPF.SharpDX/UWP/Core) 
1. Fixed shadow map OrthoWidth dependency property is setting to wrong property in scene node.(WPF.SharpDX/UWP)

### Breaking Change
1. Hit test function signature has been changed.

## [2.15.0] - 2021-02-20
### Added
1. Supports morph target animation. (WPF.SharpDX/UWP/Core)
2. Supports animation playback speed. (WPF.SharpDX/UWP/Core)
3. Supports releasing geometry data after loading into GPU. Call `Geometry3D.SetAsTransient()` to enable this feature. (WPF.SharpDX/UWP/Core) 
(Restrictions: View only; no hit test support; geometry must not be shared with multiple models; Must enable before attaching geometry3D onto a Model3D/Node, or before the Model3D/node being attached to a viewport.)
4. Supports billboard alignment relative to the origin. (WPF.SharpDX/UWP/Core)
5. Supports animation updater group. (WPF.SharpDX/UWP/Core)
6. Added extension helper method to create animation updaters from animation list. (WPF.SharpDX/UWP/Core)

### Improvement
1. Improved thread buffer management. (WPF.SharpDX/UWP/Core)
2. Changed return type for SceneNodeGroupModel3D `AddNode` `RemoveNode`. #1443 (WPF.SharpDX/UWP/Core)

### Fixed
1. Fixed border highlights and outline blur Post Effect blending issues #1491. (WPF.SharpDX/UWP/Core)
2. Fixed environment map is still being used on object after disabling it. (WPF.SharpDX/UWP/Core)
3. Fixed UWP assimp nuget spec is missing files. #1505 (UWP)
4. Fixed UnmapSubresource is not called after MapSubresource during hit test for bone skinning mesh. #1499 (WPF.SharpDX/UWP/Core)
5. Fixed wrong padding(bottom/right) in billboard single text. #1520 (WPF.SharpDX/UWP/Core)

## [2.14.0] - 2021-01-09
### Added
1. Added `CameraType` property for screen space group. Allows to use orthographic camera for screen space group under `RelativeScreenSpaced` mode.

### Improvement
1. Make projects in new format (vs2017) also use the global AssemblyInfo.cs.
2. Updated NuGet version to v5.8.
3. Updated UWP min version to Win SDK 1903. (UWP)
4. Updated Cyotek.Drawing.BitmapFont to 2.0.0. (WPF.SharpDX/UWP/Core)
5. Supports group model under screen space group. (WPF.SharpDX/UWP/Core)

### Fixed
1. ZoomExtents: confusion between horizontal and vertical fov. #1441 (WPF.SharpDX/UWP/Core)
2. Render bitmap custom size. #1439  (WPF.SharpDX/UWP/Core)
3. Fixed UWP nuget package missing .cso shader files. (UWP)
4. Fixed AssimpNet version in nuget spec. (WPF.SharpDX/UWP/Core)
5. Fixed mesh outline post effect not visible under white background #1466  (WPF.SharpDX/UWP/Core)
6. Fixed data binding fails on button2D #1385 (WPF.SharpDX)
7. Fixed billboard/line/point hit test not working properly with Dpi scaling enabled. (WPF.SharpDX/UWP/Core)
8. Fixed billboard/line/point not able to do hit test inside screen space group. (WPF.SharpDX/UWP/Core)
9. Fixed DataTemplate3D not supporting Binding-elements #1480 (Wpf)

### Breaking Change
1. `RenderContext` has been changed to `IRenderMetrices` on hit test related function signature. (WPF.SharpDX/UWP/Core)

## [2.13.1] - 2020-10-17
### Fixed
1. Fixed small triangle hit test is not working correctly in octree. #1428 (WPF.SharpDX/UWP/Core)
2. Fixed PointVisual3D and LinesVisual3D invisible on mirrored transformation. #1340 (WPF)

## [2.13.0] - 2020-10-10
### Added
1. Added `AlwaysHittable` property for scene nodes. Allow mesh to be hittable even it is not being rendered(Visible = false). Ref #1393 (WPF.SharpDX/UWP/Core)
2. Implemented high DPI rendering under DPI scaling to improve rendering quality. #1404 (WPF.SharpDX/UWP/Core)
   
   To turn this feature off, set `Viewport3DX.EnableDpiScale = false`.
   
### Improvement and Changes
1. Improved small triangle hit test. Ref #1353 (WPF.SharpDX/UWP/Core)
2. Supports up to 8 clipping planes(cross section). Ref #1396 (WPF.SharpDX/UWP/Core)
3. Supports `Dot Net Core 3.1`. (Core)
4. Upgrades `Assimp.net` to 5.0 beta.

### Fixed
1. Fixed hit test in `CrossSectionGeometryModel3D` when uses octree or set `CuttingOperation = Substract`. Ref #1396 (WPF.SharpDX/UWP/Core)
2. Fixed keybinding issue in Viewport3DX. Ref #1390 (WPF.SharpDX)
3. Fixed UWP runtime error due to dependency property naming conflicts. #1365
   
   In order to fix the issue, following breaking changes have to be made:  (UWP)
   * `Transform3D` renames to `HxTransform3D`. The `Transform3D` is a DP in UWP `UIElement`, which is not able to be overridden.
   * Gesture bindings for UWP `Viewport3DX` have been moved from `Viewport3DX.InputBindings` into `Viewport3DX.ManipulationBindings`.
4. Fixed small error in the calculation of the animation. #1405  (WPF.SharpDX/UWP/Core)
5. Fixed typo in `TextInfo`. #1415  (WPF.SharpDX/UWP/Core)

## [2.12.0] - 2020-05-25
### Added
1. Support Vertex Color blending for Phong/PBR/Diffuse materials with new `VertexColorBlendingFactor` property. (WPF.SharpDX/UWP/Core)

### Fixed
1. Fix billboardText not being call to initialize. (WPF.SharpDX/UWP/Core)
2. Empty BillboardSingleText3D Causes Crash. (WPF.SharpDX/UWP/Core)
3. Argument exception when using View.RenderTargetBitmap. (WPF.SharpDX/UWP/Core)

## [2.11.0] - 2020-02-08
### Added
1. Add IsTopBottomViewOrientedToFrontBack property to view cube #1263. (WPF)
2. Support color linear blending mode for Point Rendering. (WPF.SharpDX/UWP/Core)

### Improvement and Changes
1. Improve volume rendering. Properly renders other mesh along with volume. (WPF.SharpDX/UWP/Core)
2. Properly render volume when camera is inside the volume cube. (WPF.SharpDX/UWP/Core)

### Fixed
1. Fix dependency property on HitTestThickness on both line/point model3D. Ref #1257 (WPF.SharpDX/UWP/Core)
2. Fix MeshGeometryHelper.FindSharpEdges() not working properly. (MeshBuilder)
3. Fix CreateView overloading wrong function. (WPF.SharpDX/UWP/Core)
4. Fix GeometryBoundManager.GeometryValid is not updated when calling Geometry3D.UpdateVertices. (WPF.SharpDX/UWP/Core)
5. Fix Assimp dll reference issue.  (WPF.SharpDX/UWP/Core)

## [2.10.0] - 2019-11-10
### Added
1. Added HelixToolkit.Core.Wpf nuget package to support .net core 3.0 WPF.
2. Added HelixToolkit.SharpDX.Core.Wpf nuget package to support .net core 3.0 WPF.
3. Supports hit test on bone skinned mesh. (WPF.SharpDX/UWP/Core) (`Note: Implementation copies skinned vertices from GPU, it does not do bounding box check and may introduce potential performance hit. Please use cautiously. Make sure to disable hit test on non-hit testable models.`)

### Improvement and Changes
1. Upgrade to use Visual Studio 2019 (Required by .net core 3.0).
2. Upgrade minimum Windows SDK version to 10.0.17763 on shader builder project. (Required by Visual Studio 2019).

### Fixed
1. Fixed single point hit test in SharpDX version #1225. (WPF.SharpDX/UWP/Core)
2. Fixed nuget dependencies not getting installed. (WPF.SharpDX/UWP/Core)
3. Fixed wrong distance comparison on mesh hit test. (WPF.SharpDX/UWP/Core)

## [2.9.0] - 2019-08-24
### Added
1. Assimp Metadata #1195 (WPF.SharpDX/UWP/Core)
2. Added Helixtoolkit.Wpf.Input as nuget package.

### Improvement and Changes

### Fixed
1. FindHits not working with large scale transfroms #1193 (WPF.SharpDX/UWP/Core)
2. Fix line arrow head transform not correct #1205. (WPF.SharpDX/UWP/Core)
3. ItemsModel3D doesn't implement ItemTemplateSelector #1203 (UWP)
4. 2DControl crash with SharpDX #1125 (WPF.SharpDX/UWP/Core)

## [2.8.0] - 2019-06-22

### Added
1. Implement HelixToolkit.Wpf and HelixToolkit.Wpf.SharpDX supports for .Net Core 3.0 WPF. (WPF/WPF.SharpDX)
2. Supports absolute 3D position mode in ScreenSpacedNode. Helps to have multiple coordinate system in world space and zooming does not affect the coordinate system size. See [CustomViewCubeDemo](/Source/Examples/WPF.SharpDX/CustomViewCubeDemo) for details. Ref #1165 (WPF.SharpDX/UWP/Core)
3. Supports GPU generated arrow head/tail for line rendeirng. Detail refer to [LineShadingDemo](/Source/Examples/WPF.SharpDX/LineShadingDemo/) (WPF.SharpDX/UWP/Core)
4. Supports Line Texture in LineMaterial. (WPF.SharpDX/UWP/Core)

### Improvement and Changes
1. Supports 2D color array texture for TextureModel. Ref #1156 (WPF.SharpDX/UWP/Core)

### Fixed
1. OrthoCam width getting clipped on zooming #1164 (WPF.SharpDX/UWP/Core)
2. Fix panning speed too huge causes object flying too far. Ref #1161 (WPF.SharpDX/UWP/Core)
3. Orthogaphical Camera (width calc) SharpDX #1158  (WPF.SharpDX/UWP/Core)
4. Fix symbol link issue for nuget packages.
5. Fix EnvironmentMap projection under perspective camera. #1177

## [2.7.0] - 2019-05-12

### Added
1. Make it possible to receive touch/manipulation events from swap chain render control (WinForms).(WPF.SharpDX)
2. ManipulationBinding allows to map ManipulationGesture for UWP (UWP)
3. Added TimeStamp in default shader global variable for time based shader animation. (WPF.SharpDX/UWP/Core)

### Improvement and Changes
1. Mouse3DEventArgs now also store the mouse/touch/pen event that caused the Mouse3DEvent #1111 (WPF.SharpDX/UWP)

### Fixed
1. Fix RenderTechnicsExample SharpDX crash SharpDX bug #1130 (WPF.SharpDX/UWP/Core)
2. Fix find 3d point on single axis aligned 3d line. #1114 (WPF.SharpDX/UWP/Core)
3. Fix PInvokeStackImbalance #1149 (WPF.SharpDX)
4. Fix STLReader can not handle"NaN" values. #1150

## [2.6.1] - 2019-02-16

### Added
1. Flat Normal Shading Mode for Phong/PBR/Diffuse Material. Use `EnableFlatShading = true` to enable this mode.(WPF.SharpDX/UWP/Core)
2. Add LimitPFS option to prevent FPS over shoot on mouse move (WPF)
3. ManipulationBinding allows to map ManipulationGesture for Wpf.SharpDX (WPF.SharpDX)

### Improvement and Changes
1. Add LimitFPS option in HelixToolkit.WPF Viewport3D. Prevents mouse movement causes FPS overshooting. (WPF)
2. Add SnapMouseDownPoint for CameraController and Viewport #1082. (WPF)
3. Prevent zoom lock if look direction is too small (WPF.SharpDX/UWP/Core)

### Fixed
1. Fix Camera Rotation with ZoomAroundMouseDownpoint=true (SharpDX) #1068 (WPF.SharpDX/UWP/Core)
2. Fix exception in Closest3DPointHitTester #1085 (WPF)
3. Fix panning sometimes not working issue.(WPF.SharpDX/UWP/Core)
4. ViewportExtensions.Project does not get correct projection #1090. (WPF.SharpDX/UWP/Core)
5. Fix cube map auto mipmap generation #1087.  (WPF.SharpDX/UWP/Core)
6. Fix Transform3DHelper.CombineTransform creates too nested Transform3DGroup #1089 (WPF)
7. Fix local transform not getting calculated for UIElement3D in Visual3DHelper.GetTransform(). This solve mouse rotate getting reversed issue in Rotate Manipulator. (WPF)
8. Fix Bound one dimension is zero on single point or single axis align line and causes hit test failed.(WPF.SharpDX/UWP/Core)
9. Fix rounding issue in TextGroupVisual3D CreateTextMaterial #1075 (WPF)
10. Fix Assimp importer user specified material type in configuration not working properly. (WPF.SharpDX/UWP/Core)

## [2.6.0] - 2019-01-01
### Potential Breaking Changes:
1. Material UV Transform has been changed from  `Matrix`  to  `UVTransform`  struct. (WPF.SharpDX/UWP/Core)
2. Material Texture has been changed from `Stream` to `TextureModel`. This change allows more powerful texture support in future. In most cases, `Stream` will be implicit convert to `TextureModel` to reduce breaking changes. However, it may have issue if you are using XAML binding to a Texture stream in ViewModel. (WPF.SharpDX/UWP/Core)
3. `GroupModel3D` and `ItemsModel3D` will no longer support using XAML Children and ItemsSource at the same time (To be consistent with other WPF controls such as ListView).(WPF.SharpDX/UWP/Core)
4. PBR material `RMAMap` property has been renamed and separated into `RoughnessMetallicMap` and `AmbientOcclusionMap`. 
5. `AmbientColor` in InstancingParams has been removed.(WPF.SharpDX/UWP/Core)
6. Remove `Core` namespace on Vector3Collection/Vector2Collection/IntCollection. Base class is changed to `FastList<T>`.

### Added
1. Volume 3D Texture Rendering. [Demo](https://github.com/helix-toolkit/helix-toolkit/tree/develop/Source/Examples/WPF.SharpDX/VolumeRendering) is added.  (WPF.SharpDX/UWP/Core)
2. Supports [ImGui](https://github.com/ocornut/imgui) (using [ImGui.NET](https://github.com/mellinoe/ImGui.NET)) for SharpDX.Core. Details refer to [CoreTest demo](https://github.com/helix-toolkit/helix-toolkit/tree/develop/Source/Examples/SharpDX.Core/CoreTest).  (WPF.SharpDX and UWP)
3. Supports Line/Point non-fixedSize thickness rendering.  (WPF.SharpDX/UWP/Core)
4. Supports SSAO.  (WPF.SharpDX/UWP/Core)
5. Adds Assimp Import/Export support for SharpDX versions. (WPF.SharpDX/UWP/Core)
6. Demo Winform Integration. [CoreTest demo](https://github.com/helix-toolkit/helix-toolkit/tree/develop/Source/Examples/SharpDX.Core/CoreTest)

### Improvement and Changes
1. Add `FastList` and change `Vector3Collection`/`IntCollection` base class to FastList for direct underlying array access. (WPF.SharpDX and UWP)
2. Improved off-screen texture pooling. (WPF.SharpDX/UWP/Core)
3. Improved post effects quality. (WPF.SharpDX/UWP/Core)
4. Fixed Material creation performance issue. #1015, #1022  (WPF.SharpDX/UWP/Core)
5. Adding BeginAnimation function to SharpDX Camera #1039  (WPF.SharpDX and UWP)
6. Improves scene node for direct usage. (WPF.SharpDX/UWP/Core)
7. Merge common Viewport3DX extension functions into shared project.  (WPF.SharpDX/UWP/Core)
8. Improve unnecessary graphics resource dispose/recreate after switching tab in TabControl. Ref #1013  (WPF.SharpDX/UWP)
9. Improve rotation around mouse down point #1028 (WPF)
10. SortingVisual causes lag when using large models #1036 (WPF)
11. GroupModel3D and ItemsModel3D supports ObservableCollection.Move. Ref #1048 (WPF.SharpDX and UWP)

### Fixed
1. Make DPFCanvas work over Remote Desktop again #998. (WPF.SharpDX and UWP)
2. Transparent sorting and materials (SharpDX) #994. (WPF.SharpDX and UWP)
3. Fixed manual render order not working issue. (WPF.SharpDX/UWP/Core)
4. TaskCanceledException not caught in OnDetached #988. (WPF.SharpDX/UWP/Core)
5. ViewCube is acting on Mouse Move #969. (WPF.SharpDX and UWP)
6. LookDirection length in FitView #1009 (WPF) 
7. D3D Counter is negative (SharpDX) SharpDX bug #1040(WPF.SharpDX and UWP)
8. Zooming at small look directions causes camera shaking. #1032 (WPF)

## [2.5.1] - 2018-10-24
Hot fix for v2.5.0.
### Fixed
1. Fixed wrong type cast while using custom ViewBox Texture in HelixToolkit.SharpDX and UWP. (WPF.SharpDX and UWP)

## [2.5.0] - 2018-10-19
### Added
1. Physics Based Rendering Material. (WPF.SharpDX and UWP)
2. ScreenQuadModel3D for background full screen texture rendering.(WPF.SharpDX and UWP)
3. Supports EmissiveMap for PhongMaterial. (WPF.SharpDX and UWP)
4. Supports Billboard 2D Rotation. Added Angle property in TextInfo and BillboardSingleImage. (WPF.SharpDX and UWP)
5. Add BillboardImage3D to support sub image texture billboard. (WPF.SharpDX and UWP)
6. Supports custom bitmap font for BillboardText3D. (WPF.SharpDX and UWP)
7. Supports Billboard Text Batching [Wiki](https://github.com/helix-toolkit/helix-toolkit/wiki/Billboard-Types-and-Usage). (WPF.SharpDX and UWP)
7. Supports UnLit for DiffuseMaterial. (WPF.SharpDX and UWP)

### Improvement and Changes
1. Clean up render core. Obsolete RenderCoreBase. Move to material based rendering. (WPF.SharpDX and UWP)
2. Remove "On" prefix from events. #924 (WPF.SharpDX and UWP)
3. Upgrade SharpDX version to 4.2.0. (WPF.SharpDX and UWP)
4. Shader common buffer and sampler changes. Use single surface sampler for all surface maps in pixel shader. Obsolete NormalMapSampler, AlphaMapSampler etc. in PhongMaterial. Please update common.hlsl if you are using custom shaders. (WPF.SharpDX and UWP)
5. Improve MaterialVariable. (WPF.SharpDX and UWP)

### Fixed
1. Transparency of the material SharpDX UWP bug #925  (WPF.SharpDX and UWP)
2. stl material issue #917 (WPF)
3. Is it possible to render the content of a Viewport3DX to an image (png/bmp) with higher DPI? #920 (WPF.SharpDX and UWP)
4. NullRef Exception in BufferComponent (SharpDX) SharpDX UWP bug taken #966  (WPF.SharpDX and UWP)
5. Recover from DXGI_ERROR_DEVICE_REMOVED / DXGI_ERROR_DEVICE_RESET #963 (WPF.SharpDX and UWP)

## [2.4.0] - 2018-8-26
### Added
1. Axis aligned plane grid. (WPF.SharpDX and UWP)
2. CMO Reader. (WPF.SharpDX and UWP)
3. Animation KeyframeUpdater. (WPF.SharpDX and UWP)
4. Added UV Transform in PhongMaterial/DiffuseMaterial (WPF.SharpDX and UWP).
5. Added custom billboard texture sampler.

### Improvement and Changes
1. Move render environment map and render shadow map properties into PhongMaterial. (WPF.SharpDX and UWP)
2. Includes material sorting if EnableRenderOrder = true. Update RenderOrder to ushort. Update sorting key to be uint = [RenderOrder, MaterialID]. (WPF.SharpDX and UWP)
3. MaterialVariable pooling. (WPF.SharpDX and UWP)
4. Obsolete BoneMatrices struct. Improve BoneSkinnedGeomerty3D. Directly Martix array binding for bones. Add BoneGroupModel3D for bone sharing. Implemented basic key frame animation support and [Demo](https://github.com/helix-toolkit/helix-toolkit/tree/develop/Source/Examples/WPF.SharpDX/BoneSkinDemo). (WPF.SharpDX and UWP)
5. Performance improvement.

### Fixed
1. Fix bug on DisposeAndClear not called during detaching scene node. (WPF.SharpDX and UWP)
2. Fix bug on invalidate scene graph not working on detaching scene node. (WPF.SharpDX and UWP)
3. Instanced models are not properly exported using ObjExporter #902 (WPF.SharpDX)
4. Coordinate system and view cube are clipped when resizing the Viewport3DX #892 (WPF.SharpDX and UWP)
5. Wrong Y texture coordinate #870 (WPF.SharpDX and UWP)
6. ObjExporter export MeshGeometryModel3D fails #857 (WPF.SharpDX)

## [2.3.0] - 2018-7-22
### Added
1. Dynamic Buffer Support for geometry data streaming. (WPF.SharpDX and UWP) [Wiki](https://github.com/helix-toolkit/helix-toolkit/wiki/Dynamic-Geometry3D-for-Data-Streaming)
2. New TransformManipulator. See Manipulator Demo. (WPF.SharpDX and UWP)
3. Ply format reader. (All)
4. Support Order Independent Transparency for DiffuseMaterial. (WPF.SharpDX and UWP)
5. Add BatchedMeshGeometryModel3D for mesh batching, supports multiple material color properties.(WPF.SharpDX and UWP)
6. Support Manual Render Ordering. (WPF.SharpDX and UWP)

### Improvement and Changes
1. Move tessellation parameters into PhongMaterial. (WPF.SharpDX and UWP)
2. Obsolete Ray3D. Use SharpDX.Ray instead. (WPF.SharpDX and UWP)
3. Optimize all gesture handlers. (WPF.SharpDX and UWP)
4. Move camera to shared project. (WPF.SharpDX and UWP)
5. Rearrange the order of `DefaultVertex` struct. Separate Texture Cooridnates and Vertex Colors into own buffers. (WPF.SharpDX and UWP)
6. Change to use `CanRenderFlag` in render core instead of calling `CanRender` during rendering. Use `UpdateCanRenderFlag` or `SetAffectsCanRenderFlag` to update the flag during property change. (WPF.SharpDX and UWP)
7. Add Geometry property in HitTestResult. (WPF.SharpDX and UWP)
8. Change shader byte array to lazy loading. (WPF.SharpDX and UWP)
9. Obsolete Viewport3DX.WorldMatrix.

### Fixed
1. Fixed model transform matrix multiplication wrong sequence on HitTest with GroupModel and Instancing.(WPF.SharpDX and UWP)
2. Rectangle selection: why returns Models3D instead of Visuals3D? #841 (WPF)
3. ZoomExtentsWhenLoaded not work when model transformed. #832 (WPF.SharpDX and UWP)
4. stl import error. #827 (All)
5. 3ds import error. #816 (All)

## [2.2.0] - 2018-6-17
### Added
1. Add per-frame draw call in RenderDetail.
2. Add pingpong buffer for post effects. Add depth stencil buffer pooling
3. Add RenderTechnique serialization/deserialization.
4. Add BlendFactor/SampleMask/StencilRef in ShaderPassDescription.
5. Port DynamicCodeSurface3D to SharpDX version.

### Improvement and Changes
1. Move FXAA before post effect.
 ##### Pre(such as shadow map)->Opaque->Particle->Transparent->FXAA->Post(post effects)->ScreenSpaced(ViewBox/CoordinateSystem).
2. Change to use specific material for mesh shading technique change. Added DiffuseMaterial(Render DiffuseColor/DiffuseMap only), NormalMaterial(Render Normals as Color), PositionColorMaterial(Render Vertex Position as Color), VertColorMaterial(Render Per Vertex Color). Remove separated techniques for normal/position/color rendering. Merge these passes into default BlinnPhong techniques.
3. Encapsulate D3D11.DeviceContext functions into DeviceContextProxy. Skip redundant state bindings/shaderpass bindings.
4. Re-implement shader class.

### Fixed.
- Fixed and improved state object and shader resource proxy pooling.
- Errors building project (FeatureLevel10 issue). #754 (WPF.SharpDX & UWP)
- Inertia does not work correctly when camera has lefthand coordinate system #760 (WPF.SharpDX & UWP)
- Viewport not updating when rotating #767 (WPF.SharpDX & UWP)
- error in LineIntersection #770
- Viewport3DX Title and Subtitle are not rendered correctly #773 (WPF.SharpDX & UWP)
- Billboard questions (transparency + adding billboards after initialize) #774 (WPF.SharpDX & UWP)
- ZoomDistanceLimitNear/-Far does not work correctly #777 (WPF.SharpDX & UWP)
- CameraTarget still showing up when TouchRotate disabled #782 (WPF.SharpDX & UWP)
- SharpDX: Camera movement using keyboard doesn't work when mouse is not moving. #796 (WPF.SharpDX & UWP)
- SharpDX: Instanced Models do not follow the viewports current render technique #802 (WPF.SharpDX & UWP) Note: Use material for switching
- EnvironmentMap3D not working when using LeftHandSystem SharpDX #533 (SharpDX & UWP)

## [2.1.0] - 2018-5-4
### Improvement and Changes
1. New architecture for backend rendering and shader management. No more dependency from obsoleted Effects framework. EffectsManager is mandatory to be provided from ViewModel for resource live cycle management by user to avoid memory leak.
2. Many performance improvements. Viewports binding with same EffectsManager will share common resources. Models binding with same geometry3D will share same geometry buffers. Materials binding with same texture will share same resources.
3. Support basic direct2d rendering and layouts arrangement. (Still needs a lot of implementations)
4. No more HelixToolkit.WPF project dependency.
5. Unify dependency property types. All WPF.SharpDx model's dependency properties are using class under System.Windows.Media. Such as Vector3D and Color. More Xaml friendly.
6. Post effect support. Note: Post effect elements does not recommend to be used in ModelContainer3DX for model sharing between viewports.
7. Supports transparent meshes rendered after opaque meshes. IsTransparent property is added in MaterialGeometryModel3D.
8. Rendering order by RenderType flag: 
    ##### Pre(such as shadow map)->Opaque->Particle->Transparent->Post(post effects)->ScreenSpaced(ViewBox/CoordinateSystem).
9. Core implementation are separated from platform dependent controls(Element3D) into its own Scene Node classes. Scene Node serves as complete Scene Graph for traversal inside render host. Element3D will only be used as a wrapper to manipulate scene node properties from XAML.
10. Supports [Order independent transparent(OIT)](https://developer.nvidia.com/content/transparency-or-translucency-rendering) rendering.
11. Supports [FXAA](https://docs.nvidia.com/gameworks/content/gameworkslibrary/graphicssamples/d3d_samples/fxaa311sample.htm). Prefer FXAA over MSAA if using OIT or post effects. Note: FXAA does not support transparent background for now.
12. High performance static octree for Mesh/Point/Line/Instancing Models hit test.

### Fixed
- 10000 GDI Object limit with BillboardTextSingle3D #607 (WPF.SharpDX)
- How to turn off blurring for ImageMaterial? #581 (WPF.SharpDX)
- ZoomExtents currently not work #623 (WPF.SharpDX)
- PointGeometryModel3D not working #649 (WPF.SharpDX)
- Create Image from Viewport #636 (WPF.SharpDX)
- BillboardTextModel3D / ViewBoxModel3D text problem with Windows10 Display Scaling #652 (WPF.SharpDX)
- Transform Update when GroupModel3D changed #656 (WPF.SharpDX)
- RenderTechnique Property doesn't work for Viewport3DX #655 (WPF.SharpDX)
- How to load STL file in Helixtoolkit.SharpDX? #509 (WPF.SharpDX)
- LineShadingDemo Grid/Lines switching #671 (WPF.SharpDX)
- OctreeDemo - Octree Wireframe is invisible #673 (WPF.SharpDX)
- Add and Remove Transparent Models #679 (WPF.SharpDX)
- Missing triangle using SweepLinePolygonTriangulator #664 (WPF.SharpDX)
- ParticleSystemDemo screen output #665(WPF.SharpDX)
- Render to bitmap image? #692 (WPF.SharpDX)
- How to attach EnvironmentMap3D from code? #702 (WPF.SharpDX)
- GetScreenViewProjectionMatrix3D returns all zero in HelixToolkit.Wpf.SharpDX #708 (WPF.SharpDX)
- ModelUpDirection doesn't update in UWP #718 (UWP)
- Change actions of touch gestures #716 (UWP, WPF.SharpDX)
- Get bounds of model in MVVM #648 (WPF.SharpDX)
- BUG: HelixToolkit.SharpDX MeshGeometryModel3D bounds #296 (WPF.SharpDX)
- How do I control the frame rate? HelixToolkit UWP #334 (UWP)
- SharpDX.DefaultEffectsManager: Not possible to set device / device selection #258  (WPF.SharpDX)
- Reduce Sharp-DX version touch input lag #115 (WPF.SharpDX)
- Bindings in content of Viewport3DX do not react on DataContextChanged #731 (WPF.SharpDX)
- SortingGroupModel3D and Children #724 (WPF.SharpDX)

## [1.1.0] - 2018-2-6
### Added
### Improvement and Changes
- More smooth render loop. Refer to #611 (WPF.SharpDX)
- Call update bound automatically if not updated. Refer to #571 (WPF.SharpDX)
- GroupElement3D itemsSource supports IList<Element3D> #572.(WPF.SharpDX)
- Implement SetTarget Command #577.(WPF.SharpDX)

### Fixed
- SharpDX GetBestAdapter method will not always get (The Best) one for .NET 4.5 (#282)
- BillboardTextVisual3D with Emissive material still affected by lights (#127)
- Fixed FPS counter not current under multiple viewport (#599)
- Fixed default viewbox text (#599)
- Viewcube not clickable after modelupdirection change while running (#586)

## [1.0.0] - 2017-10-15
### Added
- Gitlink build step (#123)
- HelixViewport3D.CursorPosition (#133)
- FitView method on CameraHelper and HelixViewport3D (#264)
- Add triangle winding orientation, cull mode, depth clip enable dependency properties in MeshGeometryModel3D(#312) and PatchGeometryModel3D(#402). (WPF.SharpDX)
- Hit test for Billboard.(#313)(WPF.SharpDX)
- Add a TorusVisual3D and an Example Project.(#318)(WPF)
- SweepLinePolygonTriangulator added, faster than CuttingEarsTriangulator. (#328)
- Ability to add caps for tube in meshbuilder.(#341)
- Add FixedRotationPoint and FixedRotationPointEnabled properties for Viewport3DX. (#358)(WPF.SharpDX)
- Add BillboardSingleText model. (WPF.SharpDX)
- Add AddBox(Rect3D rectangle, BoxFaces faces) to MeshBuilder. (#363)
- Add BillboardImage3D model. (#373)(WPF.SharpDX)
- Add ReuseVertexArrayBuffer property to reuse existing vertex array. (#379)(WPF.SharpDX)
- Add alpha map to support PNG texture for MaterialModel3D and BillboardModel3D. (#401)(WPF.SharpDX)
- Add Octree for Geometry3D. Speedup hit test for all models. (#408)(WPF.SharpDX)
- Support complex template objects as DataTemplate3D with bindings on any level. (#420)(WPF)
- Add InstancingMeshGeometryModel3D and InstancingBillboardModel3D. (#432)(WPF.SharpDX)
- Support bone skinning, add BoneSkinMeshGeometryModel3D. (#446)(WPF.SharpDX)
- Enable and demonstrate per-vertex line colors. (#452)(WPF.SharpDX)
- Support Non-Fixed Sized billboarding. (#463)(WPF.SharpDX)
- Multi-Viewports support with model sharing. (#475)(WPF.SharpDX)
- Add Particle system. (#480)(WPF.SharpDX)
- Add OutLineMeshGeometryModel3D and XRayMeshGeometryModel3D. (#492)(WPF.SharpDX)
- Port Fast-Quadric-Mesh-Simplification. (#511)
- Add CrossSectionMeshGeometry3D to provide plane cut like in CAD tool.(#543)(WPF.SharpDX)
- Add InvertNormal property for MeshGeometryModel3D. (#554)(WPF.SharpDX)

### Improvement and Changes
- Rendering performance improvement, architecture optimization, code cleanup. (WPF.SharpDX)
- Memory leak fixes.
- TubeVisual3D, TorusVisual3D and MeshBuilder enhanced.(#335)(WPF)
- Move MeshBuilder to shared project.(#360)
- Update SharpDX version to v4.0.1.
- Supports different rendering mode(deferred, swapchain).(WPF.SharpDX)
- Hit Test performance improvement. (WPF.SharpDX)
- Disable MSAA by default. (WPF.SharpDX)
- Numerous bug fixes.

### Fixed
- ScreenGeometryBuilder (#106)
- Lost render target (#112)
- SharpDX idle load (#113)
- SharpDX event driven rendering (#115)
- Obj import smoothing group to long (#118)
- SharpDX crash if no DX10 GPU is present (#120)
- ContourHelper null reference (#122)
- Obj export wrong texture type (#132)
- SharpDX DPFCanvas safety check (#137)
- GridLinesVisual3D normal issue (#136)
- Switch left and right side of ViewCube (#183)
- ViewCube not available after ModelUpDirection change (#4)
- Migrate automatic package restore (#189)
- Threshold of Polygon3D.GetNormal() changed to 1e-10 (#246)
- Disable hit testing on adorner layer (#250)
- Frozen ScreenSpaceVisual3D.Points (#275)
- Zoom to mouse location with wheel help wanted unconfirmed bug you take it (#289)
- Fix material file exception in the ExportObj extension method (#303)
- Any tips on implement hit test for billboard text in Helix SharpDx? (#308)
- Incorrect search for best multisampling configuration SharpDX (#311)
- Rotate geometry is sluggish (#327)
- MeshBuilder doesn't compile in UWP project (#369)
- AddRectangularMesh doesn't reflect light - SharpDX (#374)
- What about an Octree enhancement (#376)
- LightIndex is incorrect under multiple UI thread in Light3D (SharpDX) (#384)
- Duplicate implementation in Viewport3DX and CameraController(#386)
- Vector3Collection.Array exposes full internal array, causes incorrect boundingbox (#406)
- Issue with lighting after update (#390)
- DPFCanvas OnRenderSizeChanged crash Application on Startup (#415)
- Drawing a lot of lines of different color question SharpDX (#439)
- Data Template example: Cannot find governing FrameworkElement (#442)
- Wrong PointHit returned by LineGeometryModel3D.HitTest (#443)
- OrthographicCamera is broken (SharpDX) (#467)
- HelixToolkit.Wpf.SharpDX refers SharpDX.Mathematics v4.0.1.0 but default installation refers v3.1.1.0 (#514)
- NotImplementedException in ModelContainer3DX (#522)
- Some problem about model share after update (#556)

## [2014.2.452] - 2014-12-16
### Added
- PointSelectionCommand (Wpf)
- Support subtract mode in CuttingPlaneGroup (Wpf)
- Handle collection changes for ScreenSpaceVisual3D.Points (Wpf.SharpDX)
- Ignore visuals that implement IBoundsIgnoredVisual3D in the bounds calculation (#229)

### Changed
- XAML namespace prefix changed to `http://helix-toolkit.org/wpf` (Wpf)
- XAML namespace prefix changed to `http://helix-toolkit.org/wpf/SharpDX` (Wpf.SharpDX)

### Fixed
- Memory leak in DX11ImageSource (Wpf.SharpDX)

[Unreleased]: https://github.com/oxyplot/oxyplot/compare/v2015.1.453...HEAD
[2014.2]: https://github.com/oxyplot/oxyplot/compare/v2014.2.10...v2014.2.452
