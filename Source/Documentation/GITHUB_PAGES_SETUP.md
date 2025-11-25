# Setting up GitHub Pages for Documentation

This guide is for repository maintainers who need to configure GitHub Pages to host the automatically generated documentation.

## Prerequisites

- Admin access to the repository
- GitHub Actions workflow successfully building documentation

## Setup Steps

### 1. Enable GitHub Pages

1. Go to your repository on GitHub
2. Click on **Settings** (top navigation)
3. Scroll down to **Pages** in the left sidebar
4. Under **Build and deployment**, select:
   - **Source**: GitHub Actions
5. Click **Save**

### 2. Verify Workflow Permissions

1. In **Settings**, go to **Actions** → **General**
2. Scroll to **Workflow permissions**
3. Ensure the following is selected:
   - **Read and write permissions**
   - Check **Allow GitHub Actions to create and approve pull requests** (optional but recommended)
4. Click **Save**

### 3. Configure Pages Permissions

1. In **Settings**, go to **Pages**
2. Under **Build and deployment**:
   - Source: GitHub Actions (should already be set)
3. Note the URL where your site will be published (e.g., `https://helix-toolkit.github.io/helix-toolkit/`)

### 4. Trigger Documentation Build

The documentation will be automatically built and deployed when:
- Changes are pushed to the `main` branch
- Changes affect files in `Source/**/*.cs` or `Source/Documentation/**`

To manually trigger a build:
1. Go to **Actions** tab
2. Select **Documentation** workflow
3. Click **Run workflow**
4. Select the branch (usually `main`)
5. Click **Run workflow**

### 5. Verify Deployment

1. Wait for the workflow to complete (check in the Actions tab)
2. Once successful, visit your GitHub Pages URL
3. You should see the generated documentation

## Troubleshooting

### "Pages is not enabled" Error

If you see an error about Pages not being enabled:
1. Go to Settings → Pages
2. Select "GitHub Actions" as the source
3. Save and try again

### "Permission denied" Error

If the workflow fails with permission errors:
1. Go to Settings → Actions → General
2. Under "Workflow permissions", select "Read and write permissions"
3. Save and re-run the workflow

### Documentation Not Updating

1. Check the Actions tab for workflow run status
2. Review workflow logs for errors
3. Ensure the main branch has the latest changes
4. Verify that the workflow file is in `.github/workflows/documentation.yml`

### 404 on GitHub Pages

1. Wait a few minutes after the first deployment (can take 5-10 minutes)
2. Verify the workflow completed successfully
3. Check that files were uploaded in the "Upload artifact for GitHub Pages" step
4. Clear your browser cache and try again

## Custom Domain (Optional)

To use a custom domain for your documentation:

1. In Settings → Pages
2. Under "Custom domain", enter your domain (e.g., `docs.helixtoolkit.org`)
3. Add a CNAME record in your DNS settings pointing to `<username>.github.io`
4. Wait for DNS propagation (can take up to 24 hours)
5. Enable "Enforce HTTPS" once the certificate is provisioned

## Security Considerations

- The documentation workflow only has permissions to deploy to GitHub Pages
- No secrets or sensitive information should be included in documentation
- The workflow uses OIDC tokens for secure authentication with GitHub Pages

## Additional Resources

- [GitHub Pages Documentation](https://docs.github.com/en/pages)
- [GitHub Actions for Pages](https://github.com/actions/deploy-pages)
- [DocFX Documentation](https://dotnet.github.io/docfx/)
