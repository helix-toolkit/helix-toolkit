// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InputGesture.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Defines an input gesture that can be used to invoke a command.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#if WINUI
using Microsoft.UI.Xaml;
namespace HelixToolkit.WinUI
#else
using Windows.UI.Xaml;
namespace HelixToolkit.UWP
#endif
{

    /// <summary>
    /// Defines an input gesture that can be used to invoke a command.
    /// </summary>
    public abstract class InputGesture
    {
        public abstract bool Matches(object targetElement, RoutedEventArgs inputEventArgs);
    }
}
