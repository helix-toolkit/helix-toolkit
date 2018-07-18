# Change Log
All notable changes to this project will be documented in this file.

## Next Release
### Added
1. Dynamic Buffer Support for geometry data streaming. (WPF.SharpDX and UWP) [Wiki](https://github.com/helix-toolkit/helix-toolkit/wiki/Dynamic-Geometry3D-for-Data-Streaming)
2. New TransformManipulator. See Manipulator Demo. (WPF.SharpDX and UWP)
3. Ply format reader. (All)
4. Support Order Independent Transparency for DiffuseMaterial. (WPF.SharpDX and UWP)
5. Add BatchedMeshGeometryModel3D for mesh batching, supports multiple material color properties.(WPF.SharpDX and UWP)

### Improvement and Changes
1. Move tessellation parameters into PhongMaterial. (WPF.SharpDX and UWP)
2. Obsolete Ray3D. Use SharpDX.Ray instead. (WPF.SharpDX and UWP)
3. Optimize all gesture handlers. (WPF.SharpDX and UWP)
4. Move camera to shared project. (WPF.SharpDX and UWP)
5. Rearrange the order of `DefaultVertex` struct. Separate Texture Cooridnates and Vertex Colors into own buffers. (WPF.SharpDX and UWP)
6. Change to use `CanRenderFlag` in render core instead of calling `CanRender` during rendering. Use `UpdateCanRenderFlag` or `SetAffectsCanRenderFlag` to update the flag during property change. (WPF.SharpDX and UWP)
7. Add Geometry property in HitTestResult. (WPF.SharpDX and UWP)
8. Change shader byte array to lazy loading. (WPF.SharpDX and UWP)

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
