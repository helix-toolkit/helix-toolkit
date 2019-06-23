// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoreWpfDemo
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    using HelixToolkit.Wpf;

    using Microsoft.Win32;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // http://guide.lugnet.com/color/

        private static readonly Brush[] brushes = new[]
                                               {
                                                   Brushes.Red, Brushes.Green, Brushes.Blue, Brushes.Yellow, Brushes.White,
                                                   Brushes.Gray, Brushes.Black, BrushHelper.ChangeOpacity(Brushes.Red, 0.3)
                                               };

        public static readonly DependencyProperty BrickRowsProperty =
            DependencyProperty.Register("BrickRows", typeof(int), typeof(MainWindow), new UIPropertyMetadata(2));

        public static readonly DependencyProperty BrickColumnsProperty =
            DependencyProperty.Register("BrickColumns", typeof(int), typeof(MainWindow), new UIPropertyMetadata(4));

        public static readonly DependencyProperty BrickHeightProperty =
            DependencyProperty.Register("BrickHeight", typeof(int), typeof(MainWindow), new UIPropertyMetadata(3));

        public static readonly DependencyProperty CurrentBrushProperty =
            DependencyProperty.Register("CurrentBrush", typeof(Brush), typeof(MainWindow),
                                        new UIPropertyMetadata(brushes[0]));

        private int currentBrushIndex;


        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += MainWindow_Loaded;
        }

        public int BrickRows
        {
            get { return (int)GetValue(BrickRowsProperty); }
            set { SetValue(BrickRowsProperty, value); }
        }

        public int BrickColumns
        {
            get { return (int)GetValue(BrickColumnsProperty); }
            set { SetValue(BrickColumnsProperty, value); }
        }

        public int BrickHeight
        {
            get { return (int)GetValue(BrickHeightProperty); }
            set { SetValue(BrickHeightProperty, value); }
        }

        public Brush CurrentBrush
        {
            get { return (Brush)GetValue(CurrentBrushProperty); }
            set { SetValue(CurrentBrushProperty, value); }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            view1.Camera.NearPlaneDistance = LegoVisual3D.GridUnit;
            view1.Camera.FarPlaneDistance = 10;
            view1.ZoomExtents();
        }

        private void view1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var lego = new LegoVisual3D();
            lego.Rows = BrickRows;
            lego.Columns = BrickColumns;
            lego.Height = BrickHeight;
            lego.Fill = CurrentBrush;
            Point3D? pt = view1.FindNearestPoint(e.GetPosition(view1));
            if (pt.HasValue)
            {
                Point3D p = pt.Value;
                double gu = LegoVisual3D.GridUnit;
                double hu = LegoVisual3D.HeightUnit;
                p.X = gu * Math.Floor(p.X / gu);
                p.Y = gu * Math.Floor(p.Y / gu);
                p.Z = hu * Math.Floor(p.Z / hu);

                lego.Transform = new TranslateTransform3D(p.X, p.Y, p.Z);
                view1.Children.Add(lego);
            }
        }

        private void CurrentColor_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            currentBrushIndex++;
            CurrentBrush = brushes[currentBrushIndex % brushes.Length];
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            var d = new SaveFileDialog();
            d.Filter = Exporters.Filter;
            if (d.ShowDialog().Value)
            {
                Export(d.FileName);
            }
        }

        private void Export(string fileName)
        {
            if (fileName != null)
                view1.Export(fileName);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}