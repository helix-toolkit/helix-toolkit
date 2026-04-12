# Quick Start: Documentation Generation

## For Contributors

### Writing XML Documentation

Add XML comments to your public APIs:

```csharp
/// <summary>
/// Brief description of what this does.
/// </summary>
/// <param name="paramName">Description of parameter</param>
/// <returns>Description of return value</returns>
/// <exception cref="ExceptionType">When this exception is thrown</exception>
public ReturnType MethodName(ParamType paramName)
{
    // implementation
}
```

### Building Documentation

**Windows:**
```cmd
cd Source
build-doc.cmd
```

**Linux/macOS:**
```bash
cd Source
./build-doc.sh
```

The documentation will be available at http://localhost:8080

### Automated Builds

Documentation is automatically built and deployed via GitHub Actions:
- **Pull Requests**: Documentation is built to verify compilation
- **Main Branch**: Documentation is built and deployed to GitHub Pages

## For Maintainers

### Configuration

The documentation system is configured in:
- `Source/Documentation/docfx.json` - DocFX configuration
- `.github/workflows/documentation.yml` - GitHub Actions workflow

### Deployment

Documentation is automatically deployed to GitHub Pages when changes are pushed to the main branch.

To manually trigger a documentation build, use the workflow dispatch option in the GitHub Actions tab.

### Troubleshooting

**Build fails with compilation errors:**
- The `allowCompilationErrors: true` setting allows the build to continue even if some projects fail to compile
- Windows-specific projects may fail on Linux, but core documentation will still be generated

**Documentation not updating:**
- Ensure XML documentation generation is enabled in project files
- Check GitHub Actions workflow logs for errors
- Verify GitHub Pages is enabled in repository settings

## More Information

See [Source/Documentation/README.md](/Source/Documentation/README.md) for comprehensive documentation.
