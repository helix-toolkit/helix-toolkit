using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;
using System.Windows;
using System.Windows.Media;

namespace HelixToolkit.Wpf.SharpDX
{
    /// <summary>
    ///     The VisualWrapper simply integrates a raw Visual child into a tree
    ///     of FrameworkElements.
    ///      https://blogs.msdn.microsoft.com/dwayneneed/2007/04/26/multithreaded-ui-hostvisual/
    /// </summary>
    [ContentProperty("Child")]
    public class VisualWrapper<T> : FrameworkElement where T : Visual
    {
        public T Child
        {
            get
            {
                return _child;
            }

            set
            {
                if (_child != null)
                {
                    RemoveVisualChild(_child);
                }

                _child = value;

                if (_child != null)
                {
                    AddVisualChild(_child);
                }
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (_child != null && index == 0)
            {
                return _child;
            }
            else
            {
                throw new ArgumentOutOfRangeException("index");
            }
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return _child != null ? 1 : 0;
            }
        }

        private T _child;
    }

    /// <summary>
    ///     The VisualWrapper simply integrates a raw Visual child into a tree
    ///     of FrameworkElements.
    /// </summary>
    public class VisualWrapper : VisualWrapper<Visual>
    {
    }
}
