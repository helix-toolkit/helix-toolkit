/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace HelixToolkit.UWP.CommonDX
{
    /// <summary>
    /// https://evanl.wordpress.com/2009/12/06/efficient-optimal-per-frame-eventing-in-wpf/
    /// </summary>
    public sealed class CompositionTargetEx : IDisposable
    {
        private TimeSpan _last = TimeSpan.Zero;
        private event EventHandler<RenderingEventArgs> _FrameUpdating;
        public event EventHandler<RenderingEventArgs> Rendering
        {
            add
            {
                if (_FrameUpdating == null)
                    CompositionTarget.Rendering += CompositionTarget_Rendering;
                _FrameUpdating += value;
            }
            remove
            {
                _FrameUpdating -= value;
                if (_FrameUpdating == null)
                    CompositionTarget.Rendering -= CompositionTarget_Rendering;
            }
        }

        private void CompositionTarget_Rendering(object sender, object e)
        {
            RenderingEventArgs args = (RenderingEventArgs)e;
            if (args.RenderingTime == _last)
                return;
            _last = args.RenderingTime;
            _FrameUpdating?.Invoke(sender, args);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        void Dispose(bool disposing)
        {
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~CompositionTargetEx()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
