using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HelixToolkit.Wpf.SharpDX;

namespace CustomViewCubeDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MeshGeometryModel3D_Mouse3DDown(object sender, MouseDown3DEventArgs e)
        {
            var normal = e.HitTestResult.NormalAtHit;
            normal.Normalize();
            var upDirection = Vector3.Zero;
            var lookDirection = -normal;
            if (Vector3.Cross(normal, view1.ModelUpDirection.ToVector3()).LengthSquared() < 1e-5)
            {
                var vecLeft = new Vector3(-normal.Y, -normal.Z, -normal.X);
                upDirection = vecLeft;
            }
            else
            {
                upDirection = view1.ModelUpDirection.ToVector3();
            }

            var target = view1.Camera.Position + view1.Camera.LookDirection;
            var distance = view1.Camera.LookDirection.Length;
            lookDirection *= (float)distance;
            var newPosition = target.ToVector3() - lookDirection;
            view1.Camera.AnimateTo(newPosition.ToPoint3D(), lookDirection.ToVector3D(), upDirection.ToVector3D(), 500);
        }
    }
}
