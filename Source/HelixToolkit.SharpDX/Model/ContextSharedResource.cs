﻿using HelixToolkit.SharpDX.Utilities;

namespace HelixToolkit.SharpDX.Model;

public sealed class ContextSharedResource : IDisposable
{
    public ShaderResourceViewProxy? ShadowView
    {
        set; get;
    }

    public ShaderResourceViewProxy? EnvironementMap
    {
        set; get;
    }

    public ShaderResourceViewProxy? SSAOMap
    {
        set; get;
    }

    public int EnvironmentMapMipLevels
    {
        set; get;
    }
    #region IDisposable Support
    private bool disposedValue = false; // To detect redundant calls

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                ShadowView = null;
                EnvironementMap = null;
                SSAOMap = null;
                // TODO: dispose managed state (managed objects).
            }

            // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
            // TODO: set large fields to null.

            disposedValue = true;
        }
    }

    // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
    // ~ContextSharedResource() {
    //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
    //   Dispose(false);
    // }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(true);
        // TODO: uncomment the following line if the finalizer is overridden above.
        // GC.SuppressFinalize(this);
    }
    #endregion
}
