# Post-Merge Checklist for Documentation System

This checklist helps repository maintainers complete the setup after this PR is merged.

## ✅ Immediate Actions (Required)

### 1. Enable GitHub Pages
**Time Required**: 2 minutes

1. Go to repository **Settings**
2. Click **Pages** in the left sidebar
3. Under **Build and deployment**:
   - Source: Select **GitHub Actions**
4. Click **Save**

**Verification**: You should see a message "Your site is ready to be published at `https://helix-toolkit.github.io/helix-toolkit/`"

### 2. Configure Workflow Permissions
**Time Required**: 1 minute

1. Go to repository **Settings**
2. Click **Actions** → **General** in the left sidebar
3. Scroll to **Workflow permissions**
4. Select **Read and write permissions**
5. (Optional) Check **Allow GitHub Actions to create and approve pull requests**
6. Click **Save**

**Verification**: The permissions section should show "Read and write permissions" selected.

### 3. Trigger First Build
**Time Required**: 5-10 minutes

Choose one of these options:

**Option A: Wait for next push to main**
- The workflow will automatically run on the next push to main branch
- No action needed

**Option B: Manual trigger**
1. Go to **Actions** tab
2. Click **Documentation** workflow in the left sidebar
3. Click **Run workflow** button (top right)
4. Select branch: **main**
5. Click **Run workflow**

**Verification**: 
- Go to Actions tab
- Find the "Documentation" workflow run
- Wait for it to complete (green checkmark)
- Look for "deploy-docs" job - it should show "Deployment succeeded"

### 4. Verify Documentation Site
**Time Required**: 2 minutes (after build completes)

1. Wait for the workflow to complete (5-10 minutes for first run)
2. Visit: `https://helix-toolkit.github.io/helix-toolkit/`
3. Verify you can see:
   - Documentation homepage with Helix Toolkit branding
   - "API Documentation" link in navigation
   - "Articles" link in navigation
   - Search functionality

**Troubleshooting**: If you see 404:
- Wait 5 more minutes (GitHub Pages can take time to propagate)
- Clear browser cache
- Check workflow logs for errors

## 📋 Optional Actions

### 5. Customize Documentation (Optional)
**Time Required**: 10-30 minutes

**Update Branding:**
- Add logo: Place image in `Source/Documentation/images/`
- Update `docfx.json`: Set `_appLogoPath` to your logo path
- Update `_appFooter` with current year/organization

**Add More Articles:**
- Create `.md` files in `Source/Documentation/articles/`
- Add entries to `Source/Documentation/articles/toc.yml`
- Follow examples in existing articles

**Customize Theme:**
- Edit `docfx.json` → `build.template` section
- Add custom CSS/JS (see DocFX documentation)

### 6. Setup Custom Domain (Optional)
**Time Required**: 15-30 minutes + DNS propagation time

1. In **Settings** → **Pages**
2. Under **Custom domain**, enter your domain (e.g., `docs.helixtoolkit.org`)
3. Add CNAME record in your DNS:
   ```
   docs.helixtoolkit.org  →  helix-toolkit.github.io
   ```
4. Wait for DNS propagation (can take up to 24 hours)
5. Enable **Enforce HTTPS** once certificate is provisioned

### 7. Add Documentation Badge to README (Optional)
**Time Required**: 2 minutes

Add this badge to your README.md:

```markdown
[![Documentation](https://img.shields.io/badge/docs-latest-blue.svg)](https://helix-toolkit.github.io/helix-toolkit/)
```

Result: [![Documentation](https://img.shields.io/badge/docs-latest-blue.svg)](https://helix-toolkit.github.io/helix-toolkit/)

### 8. Announce to Community (Optional)
**Time Required**: 5 minutes

Consider announcing the new documentation to:
- GitHub Discussions (if enabled)
- Gitter chat channel
- Twitter/social media
- Next release notes

Example announcement:
```
🎉 We now have automated API documentation!

Our comprehensive documentation is now automatically generated and deployed:
📚 https://helix-toolkit.github.io/helix-toolkit/

Features:
✓ Complete API reference
✓ Getting started guides
✓ Searchable content
✓ Always up-to-date with latest code

Check it out and let us know what you think!
```

## 🔍 Verification Checklist

Use this checklist to ensure everything is working:

- [ ] GitHub Pages is enabled in repository settings
- [ ] Workflow permissions are set to "Read and write"
- [ ] Documentation workflow has run successfully at least once
- [ ] Documentation website is accessible at GitHub Pages URL
- [ ] Homepage displays correctly with navigation
- [ ] API documentation is visible and browseable
- [ ] Articles section is accessible
- [ ] Search functionality works
- [ ] Mobile view works (test on phone/tablet or use browser dev tools)

## 📊 Monitoring

### Check Workflow Status
- Go to **Actions** tab
- Click **Documentation** workflow
- View recent runs and their status
- Click on any run to see detailed logs

### Update Frequency
Documentation is automatically updated when:
- Code with XML comments is modified
- Documentation files in `Source/Documentation/` are changed
- Commits are pushed to `main` or `develop` branches

### Troubleshooting Common Issues

**Issue**: Workflow fails with permission errors
- **Solution**: Verify workflow permissions are set to "Read and write"

**Issue**: Documentation not updating
- **Solution**: Check workflow logs for build errors
- **Solution**: Ensure XML documentation generation is enabled in project files

**Issue**: 404 on GitHub Pages
- **Solution**: Verify GitHub Pages source is set to "GitHub Actions"
- **Solution**: Check that deploy-docs job completed successfully
- **Solution**: Wait a few more minutes and clear browser cache

## 📚 Additional Resources

- **Detailed Setup Guide**: `Source/Documentation/GITHUB_PAGES_SETUP.md`
- **Developer Guide**: `Source/Documentation/README.md`
- **Quick Reference**: `DOCUMENTATION.md`
- **Implementation Details**: `IMPLEMENTATION_SUMMARY.md`

## 💡 Tips

1. **First build takes longer**: Expect 10-15 minutes for the first documentation build as DocFX needs to process all projects
2. **Incremental builds are faster**: Subsequent builds typically complete in 5-7 minutes
3. **Test locally first**: Before pushing doc changes, test locally with `build-doc.cmd` (Windows) or `./build-doc.sh` (Linux/macOS)
4. **Keep XML comments updated**: Encourage contributors to add/update XML comments in their code

## ✨ You're Done!

Once you've completed the required actions above, your documentation system is fully operational. 

The documentation will automatically stay up-to-date with your codebase without any further manual intervention.

---

**Questions?** See the detailed guides in `Source/Documentation/` or open an issue.
