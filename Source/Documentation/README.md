# Documentation System

This directory contains the automated documentation generation system for Helix Toolkit using [DocFX](https://dotnet.github.io/docfx/).

## Overview

The documentation system automatically generates:
- **API Documentation**: Extracted from XML documentation comments in C# source code
- **Articles**: Conceptual documentation and guides (stored in `articles/`)
- **Static Site**: A browseable website with search functionality

## Building Documentation Locally

### Prerequisites

- [.NET SDK 6.0 or later](https://dotnet.microsoft.com/download)
- Git (for cloning the repository)

### Quick Start

#### On Windows
```cmd
cd Source
build-doc.cmd
```

#### On Linux/macOS
```bash
cd Source
./build-doc.sh
```

The documentation will be built and served locally at `http://localhost:8080`. Press `Ctrl+C` to stop the server.

### Building Without Serving

To build the documentation without starting the local server (useful in CI environments):

#### Windows
```cmd
set CI=True
build-doc.cmd
```

#### Linux/macOS
```bash
CI=true ./build-doc.sh
```

The generated documentation will be in `Documentation/_site/`.

## Automated Documentation Generation

### GitHub Actions Workflow

Documentation is automatically built and deployed via GitHub Actions:

1. **On Pull Requests**: Documentation is built to verify it compiles correctly
2. **On Push to main/develop**: Documentation is built and deployed to GitHub Pages

### Viewing Published Documentation

Once deployed, the documentation is available at:
- **GitHub Pages**: `https://helix-toolkit.github.io/helix-toolkit/`

## Documentation Structure

```
Documentation/
├── api/              # Auto-generated API documentation
├── articles/         # Hand-written articles and guides
│   ├── intro.md
│   └── toc.yml
├── images/           # Images used in documentation
├── docfx.json        # DocFX configuration
├── index.md          # Documentation homepage
└── toc.yml           # Top-level table of contents
```

## Writing Documentation

### API Documentation

Add XML documentation comments to your C# code:

```csharp
/// <summary>
/// Represents a 3D model in the scene.
/// </summary>
/// <remarks>
/// This class provides functionality for loading and rendering 3D models.
/// </remarks>
public class Model3D
{
    /// <summary>
    /// Gets or sets the position of the model in 3D space.
    /// </summary>
    /// <value>A Vector3 representing the position.</value>
    public Vector3 Position { get; set; }
    
    /// <summary>
    /// Loads a 3D model from the specified file.
    /// </summary>
    /// <param name="filename">The path to the model file.</param>
    /// <returns>True if the model was loaded successfully; otherwise, false.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the file does not exist.</exception>
    public bool Load(string filename)
    {
        // Implementation
    }
}
```

### Adding Articles

1. Create a new `.md` file in the `articles/` directory
2. Add the article to `articles/toc.yml`:

```yaml
- name: Getting Started
  href: getting-started.md
- name: Your New Article
  href: your-new-article.md
```

### Markdown Features

DocFX supports:
- Standard Markdown syntax
- Code blocks with syntax highlighting
- Cross-references to API documentation using `@ApiNamespace.ClassName`
- Alerts: `> [!NOTE]`, `> [!WARNING]`, `> [!TIP]`
- Tables, images, and more

Example:
```markdown
# My Article

This article explains how to use @HelixToolkit.Wpf.HelixViewport3D.

> [!TIP]
> Make sure to initialize the viewport before adding models.

## Code Example

'''csharp
var viewport = new HelixViewport3D();
viewport.Camera = new PerspectiveCamera();
'''
```

## Configuration

The documentation system is configured in `docfx.json`. Key settings:

- **metadata**: Specifies which projects to include in API documentation
- **build.content**: Defines which content files to include
- **build.template**: Specifies the documentation theme (default + modern)
- **build.globalMetadata**: Site-wide settings like title, footer, etc.

## Troubleshooting

### Build Errors

If the documentation build fails:

1. **Check XML Documentation**: Ensure your project files have `<GenerateDocumentationFile>true</GenerateDocumentationFile>`
2. **Restore NuGet Packages**: Run `dotnet restore` in the `Source/` directory
3. **Clear DocFX Cache**: Delete `Documentation/_site/` and rebuild

### Missing API Documentation

If some classes/methods are missing from the documentation:

1. Ensure they are `public` or `protected` (private members are not documented by default)
2. Check that the project is included in `docfx.json` under `metadata.src.files`
3. Verify XML comments exist for the member

### Links Not Working

- Use relative paths for internal links: `[Link](../api/Namespace.Class.yml)`
- Use `@` for API cross-references: `@Namespace.ClassName`

## Additional Resources

- [DocFX Documentation](https://dotnet.github.io/docfx/)
- [Markdown Syntax](https://dotnet.github.io/docfx/docs/markdown.html)
- [DocFX Configuration](https://dotnet.github.io/docfx/docs/config.html)
