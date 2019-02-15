// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManipulationBinding.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Binds a <see cref="ManipulationGesture"/> to a <see cref="RoutedCommand"/> (or another <see cref="ICommand"/>
//   implementation).
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.ComponentModel;
    using System.Windows.Input;

    /// <summary>
    /// Binds a <see cref="ManipulationGesture"/> to a <see cref="RoutedCommand"/> (or another <see cref="ICommand"/>
    /// implementation).
    /// </summary>
    public class ManipulationBinding : InputBinding
    {
        public int FingerCount => ((ManipulationGesture) this.Gesture).FingerCount;

        [TypeConverter(typeof(ManipulationGestureConverter))]
        public override InputGesture Gesture
        {
            get
            {
                return base.Gesture;
            }
        
            set
            {
                var oldGesture = this.Gesture;
                if (value is ManipulationGesture newGesture)
                {
                    if (oldGesture != newGesture)
                    {
                        base.Gesture = newGesture;
                    }
                }
                else
                {
                    throw new ArgumentException(nameof(value));
                }
            }
        }

        public ManipulationBinding()
        {
        }

        public ManipulationBinding(ICommand command, ManipulationGesture gesture)
          : base(command, gesture)
        {
        }
    }
}