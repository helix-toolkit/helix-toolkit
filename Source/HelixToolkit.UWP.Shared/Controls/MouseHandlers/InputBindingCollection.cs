// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InputBindingCollection.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   An <see cref="ObservableCollection{InputBinding}"/>.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#if WINUI
namespace HelixToolkit.WinUI
#else
namespace HelixToolkit.UWP
#endif
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// An <see cref="ObservableCollection{InputBinding}"/>.
    /// </summary>
    public sealed class InputBindingCollection : ObservableCollection<InputBinding>
    {
    }
}
