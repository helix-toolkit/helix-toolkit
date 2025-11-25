# Getting Started with Helix Toolkit

Welcome to the Helix Toolkit articles section! Here you'll find guides, tutorials, and best practices for working with Helix Toolkit.

## Introduction

Helix Toolkit is a comprehensive 3D graphics library for .NET applications. Whether you're building a CAD application, scientific visualization tool, or interactive 3D experience, Helix Toolkit provides the components you need.

## Choosing the Right Package

### For WPF Applications

**Using Built-in WPF 3D (Media3D)**
- Package: `HelixToolkit.Wpf`
- Best for: Simple 3D visualizations, compatibility with existing WPF 3D code
- Pros: Easy integration, no external dependencies beyond WPF
- Cons: Limited performance, fewer features

**Using DirectX 11 via SharpDX**
- Package: `HelixToolkit.Wpf.SharpDX`
- Best for: High-performance 3D rendering, complex scenes
- Pros: Better performance, more features (shaders, post-processing, etc.)
- Cons: Requires DirectX 11 capable hardware

### For WinUI Applications

- Package: `HelixToolkit.WinUI.SharpDX`
- Modern Windows app development with DirectX 11 rendering

### For Avalonia Applications

- Package: `HelixToolkit.Avalonia.SharpDX`
- Cross-platform applications (Windows, macOS, Linux) with DirectX rendering on Windows

### Core Packages

- `HelixToolkit`: Core functionality shared across all packages
- `HelixToolkit.Maths`: Math utilities and types
- `HelixToolkit.Geometry`: Geometry builders and utilities

## Installation

### Via NuGet Package Manager Console

```powershell
# For WPF with SharpDX
Install-Package HelixToolkit.Wpf.SharpDX

# For WPF with built-in 3D
Install-Package HelixToolkit.Wpf

# For WinUI
Install-Package HelixToolkit.WinUI.SharpDX

# For Avalonia
Install-Package HelixToolkit.Avalonia.SharpDX
```

### Via .NET CLI

```bash
# For WPF with SharpDX
dotnet add package HelixToolkit.Wpf.SharpDX

# For WPF with built-in 3D
dotnet add package HelixToolkit.Wpf
```

## Basic Usage Example

### WPF with SharpDX

```csharp
// MainWindow.xaml.cs
using HelixToolkit.Wpf.SharpDX;
using SharpDX;

namespace MyApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            // Create a viewport
            var viewport = new Viewport3DX();
            
            // Create a camera
            var camera = new PerspectiveCamera
            {
                Position = new System.Windows.Media.Media3D.Point3D(5, 5, 5),
                LookDirection = new System.Windows.Media.Media3D.Vector3D(-5, -5, -5)
            };
            viewport.Camera = camera;
            
            // Add to your UI
            Content = viewport;
        }
    }
}
```

## Next Steps

- Explore the [API Documentation](../api/index.md) for detailed class and method references
- Check out example applications in the source repository
- Join the [community chat](https://gitter.im/helix-toolkit/helix-toolkit) for questions and discussions

## Additional Resources

- [GitHub Repository](https://github.com/helix-toolkit/helix-toolkit)
- [Wiki](https://github.com/helix-toolkit/helix-toolkit/wiki)
- [Performance Tips](https://github.com/helix-toolkit/helix-toolkit/wiki/Tips-on-performance-optimization-(WPF.SharpDX-and-UWP))

## Contributing to Documentation

This documentation is generated using DocFX. To contribute:

1. Fork the repository
2. Update XML documentation comments in source code
3. Add or modify articles in `Source/Documentation/articles/`
4. Submit a pull request

See the [Documentation README](../README.md) for more details on building and contributing to the documentation.
