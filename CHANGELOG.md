# Change Log
All notable changes to this project will be documented in this file.

## [Pending Release]
### Added
- Gitlink build step (#123)
- HelixViewport3D.CursorPosition (#133)
- FitView method on CameraHelper and HelixViewport3D (#264)
- Add triangle winding orientation, cull mode, depth clip enable dependency properties in MeshGeometryModel3D(#312) and PatchGeometryModel3D(#402).(WPF.SharpDX)
- Hit test for Billboard.(#313)(WPF.SharpDX)
- Add a TorusVisual3D and an Example Project.(#318)(WPF)
- SweepLinePolygonTriangulator added, faster than CuttingEarsTriangulator. (#328)
- Ability to add caps for tube in meshbuilder.(#341)
- Add FixedRotationPoint and FixedRotationPointEnabled properties for Viewport3DX. (#358)(WPF.SharpDX)
- Add BillboardSingleText model. (WPF.SharpDX)
- Add AddBox(Rect3D rectangle, BoxFaces faces) to MeshBuilder. (#363)
- Add BillboardImage3D model. (#373)(WPF.SharpDX)
- Add ReuseVertexArrayBuffer property to reuse existing vertex array. (#379)(WPF.SharpDX)
- Add alpha map to support PNG texture for MaterialModel3D and BillboardModel3D (#401)(WPF.SharpDX)
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
- Fix material file exception in the ExportObj extension method (#303)

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
