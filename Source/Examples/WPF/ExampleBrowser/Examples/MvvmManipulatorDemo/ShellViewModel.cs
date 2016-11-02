// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShellViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MvvmManipulatorDemo
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class ShellViewModel : INotifyPropertyChanged
    {

        private double translateValue;
        public double TranslateValue
        {
            get { return translateValue; }
            set
            {
                if (translateValue != value)
                {
                    translateValue = value;
                    this.OnPropertyChanged();

                }
            }
        }

        private double rotateValue;
        public double RotateValue
        {
            get { return rotateValue; }
            set
            {
                if (rotateValue != value)
                {
                    rotateValue = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}