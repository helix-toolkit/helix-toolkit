// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HippoDemo
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Media.Media3D;

    using ExampleBrowser;


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Example(null, "CAD-like user interface demo.")]
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string property)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        private Point3D _currentPosition;

        public Point3D CurrentPosition
        {
            get
            {
                return this._currentPosition;
            }
            set
            {
                this._currentPosition = value;
                RaisePropertyChanged("CurrentPosition");
            }
        }
    }
}