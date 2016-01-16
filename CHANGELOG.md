# Change Log
All notable changes to this project will be documented in this file.

## [Unreleased]
### Added
- Gitlink build step (#123)
- HelixViewport3D.CursorPosition (#133)

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
