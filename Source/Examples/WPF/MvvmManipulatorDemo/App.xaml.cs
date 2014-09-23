// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MvvmManipulatorDemo
{
    using System.Windows;

    public partial class App : Application
    {
        Bootstrapper bootstrapper;

        public App()
        {
            bootstrapper = new Bootstrapper();
        }
    }
}