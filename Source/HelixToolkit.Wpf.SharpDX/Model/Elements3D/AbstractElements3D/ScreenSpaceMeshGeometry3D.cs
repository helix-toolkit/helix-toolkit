// <copyright file="ScreenSpaceMeshGeometry3D.cs" company="Helix Toolkit">
//   Copyright (c) 2017 Helix Toolkit contributors
//   Author: Lunci Hua
// </copyright>
using System.Windows;
using Media3D = System.Windows.Media.Media3D;
namespace HelixToolkit.Wpf.SharpDX
{
    using Core;
    using global::SharpDX;
    using HelixToolkit.Wpf.SharpDX.Elements2D;
    using Render;
    using System;
    using System.Windows.Data;
    using System.Windows.Media;

    /// <summary>
    /// Base class for screen space rendering, such as Coordinate System or ViewBox
    /// </summary>
    public abstract class ScreenSpacedElement3D : GroupModel3D
    {        
        /// <summary>
        /// <see cref="RelativeScreenLocationX"/>
        /// </summary>
        public static readonly DependencyProperty RelativeScreenLocationXProperty = DependencyProperty.Register("RelativeScreenLocationX", typeof(double), typeof(ScreenSpacedElement3D),
            new PropertyMetadata(-0.8,
                (d, e) =>
                {
                   ((d as ScreenSpacedElement3D).RenderCore as IScreenSpacedRenderParams).RelativeScreenLocationX = (float)(double)e.NewValue;
                }));
        /// <summary>
        /// <see cref="RelativeScreenLocationY"/>
        /// </summary>
        public static readonly DependencyProperty RelativeScreenLocationYProperty = DependencyProperty.Register("RelativeScreenLocationY", typeof(double), typeof(ScreenSpacedElement3D),
            new PropertyMetadata(-0.8,
                (d, e) =>
                {
                    ((d as ScreenSpacedElement3D).RenderCore as IScreenSpacedRenderParams).RelativeScreenLocationY = (float)(double)e.NewValue;
                }));
        /// <summary>
        /// <see cref="SizeScale"/>
        /// </summary>
        public static readonly DependencyProperty SizeScaleProperty = DependencyProperty.Register("SizeScale", typeof(double), typeof(ScreenSpacedElement3D),
            new PropertyMetadata(1.0,
                (d, e) =>
                {
                    ((d as ScreenSpacedElement3D).RenderCore as IScreenSpacedRenderParams).SizeScale = (float)(double)e.NewValue;
                }));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty UpDirectionProperty = DependencyProperty.Register("UpDirection", typeof(Media3D.Vector3D), typeof(ScreenSpacedElement3D),
            new PropertyMetadata(new Media3D.Vector3D(0, 1, 0),
            (d, e) =>
            {
                (d as ScreenSpacedElement3D).UpdateModel(((Media3D.Vector3D)e.NewValue).ToVector3());
            }));

        public Media3D.Vector3D UpDirection
        {
            set
            {
                SetValue(UpDirectionProperty, value);
            }
            get
            {
                return (Media3D.Vector3D)GetValue(UpDirectionProperty);
            }
        }

        public static readonly DependencyProperty LeftHandedProperty = DependencyProperty.Register("LeftHanded", typeof(bool), typeof(ScreenSpacedElement3D),
            new PropertyMetadata(false,
            (d, e) =>
            {
                (d as ScreenSpacedElement3D).screenSpaceCore.IsRightHand = !(bool)e.NewValue;
            }));

        public bool LeftHanded
        {
            set
            {
                SetValue(LeftHandedProperty, value);
            }
            get
            {
                return (bool)GetValue(LeftHandedProperty);
            }
        }
        /// <summary>
        /// Relative Location X on screen. Range from -1~1
        /// </summary>
        public double RelativeScreenLocationX
        {
            set
            {
                SetValue(RelativeScreenLocationXProperty, value);
            }
            get
            {
                return (double)GetValue(RelativeScreenLocationXProperty);
            }
        }

        /// <summary>
        /// Relative Location Y on screen. Range from -1~1
        /// </summary>
        public double RelativeScreenLocationY
        {
            set
            {
                SetValue(RelativeScreenLocationYProperty, value);
            }
            get
            {
                return (double)GetValue(RelativeScreenLocationYProperty);
            }
        }

        /// <summary>
        /// Size scaling
        /// </summary>
        public double SizeScale
        {
            set
            {
                SetValue(SizeScaleProperty, value);
            }
            get
            {
                return (double)GetValue(SizeScaleProperty);
            }
        }

        protected bool NeedClearDepthBuffer { set; get; } = true;


        protected IScreenSpacedRenderParams screenSpaceCore { get { return (IScreenSpacedRenderParams)RenderCore; } }

        public ScreenSpacedElement3D()
        {
            InitializeMover();
        }

        protected abstract void UpdateModel(Vector3 upDirection);

        protected override IRenderCore OnCreateRenderCore()
        {
            return new ScreenSpacedMeshRenderCore();
        }      

        protected override bool OnAttach(IRenderHost host)
        {
            RenderCore.Attach(renderTechnique);
            screenSpaceCore.RelativeScreenLocationX = (float)this.RelativeScreenLocationX;
            screenSpaceCore.RelativeScreenLocationY = (float)this.RelativeScreenLocationY;
            screenSpaceCore.SizeScale = (float)this.SizeScale;
            return base.OnAttach(host);
        }

        protected override void OnDetach()
        {
            RenderCore.Detach();
            base.OnDetach();
        }

        #region 2D stuffs
        public RelativePositionCanvas2D MoverCanvas { private set; get; } 
            = new RelativePositionCanvas2D() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
        private ScreenSpacePositionMover mover = new ScreenSpacePositionMover();
        private void InitializeMover()
        {
            MoverCanvas.Children.Add(mover);
            SetBinding(nameof(RelativeScreenLocationX), mover, RelativePositionCanvas2D.RelativeXProperty, this, BindingMode.TwoWay);
            SetBinding(nameof(RelativeScreenLocationY), mover, RelativePositionCanvas2D.RelativeYProperty, this, BindingMode.TwoWay);
            mover.OnMoveClicked += Mover_OnMoveClicked;
        }

        private void Mover_OnMoveClicked(object sender, ScreenSpacePositionMover.MoveDirection e)
        {
            switch (e)
            {
                case ScreenSpacePositionMover.MoveDirection.LeftTop:
                    this.RelativeScreenLocationX = -Math.Abs(RelativeScreenLocationX);
                    this.RelativeScreenLocationY = Math.Abs(RelativeScreenLocationY);
                    break;
                case ScreenSpacePositionMover.MoveDirection.LeftBottom:
                    this.RelativeScreenLocationX = -Math.Abs(RelativeScreenLocationX);
                    this.RelativeScreenLocationY = -Math.Abs(RelativeScreenLocationY);
                    break;
                case ScreenSpacePositionMover.MoveDirection.RightTop:
                    this.RelativeScreenLocationX = Math.Abs(RelativeScreenLocationX);
                    this.RelativeScreenLocationY = Math.Abs(RelativeScreenLocationY);
                    break;
                case ScreenSpacePositionMover.MoveDirection.RightBottom:
                    this.RelativeScreenLocationX = Math.Abs(RelativeScreenLocationX);
                    this.RelativeScreenLocationY = -Math.Abs(RelativeScreenLocationY);
                    break;
                default:
                    break;
            }
        }

        private static void SetBinding(string path, DependencyObject dobj, DependencyProperty property, object viewModel, BindingMode mode = BindingMode.TwoWay)
        {
            var binding = new Binding(path);
            binding.Source = viewModel;
            binding.Mode = mode;
            BindingOperations.SetBinding(dobj, property, binding);
        }
        #endregion
    }

    public class ScreenSpacePositionMover : Panel2D
    {
        public enum MoveDirection
        {
            LeftTop, LeftBottom, RightTop, RightBottom
        };
        public event EventHandler<MoveDirection> OnMoveClicked;
        private Button2D MoveLeftTop, MoveLeftBottom, MoveRightTop, MoveRightBottom;
        private Button2D[] buttons = new Button2D[4];

        public ScreenSpacePositionMover()
        {
            MoveLeftTop = new Button2D() { Width = 10, Height = 10, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, Content2D = new RectangleModel2D() { Width = 8, Height = 8, Fill = new SolidColorBrush(Colors.Yellow) } };
            MoveLeftBottom = new Button2D() { Width = 10, Height = 10, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Bottom, Content2D = new RectangleModel2D() { Width = 8, Height = 8, Fill = new SolidColorBrush(Colors.Yellow) } };
            MoveRightTop = new Button2D() { Width = 10, Height = 10, HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Top, Content2D = new RectangleModel2D() { Width = 8, Height = 8, Fill = new SolidColorBrush(Colors.Yellow) } };
            MoveRightBottom = new Button2D() { Width = 10, Height = 10, HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom, Content2D = new RectangleModel2D() { Width = 8, Height = 8, Fill = new SolidColorBrush(Colors.Yellow) } };

            buttons[0] = MoveLeftTop;
            buttons[1] = MoveLeftBottom;
            buttons[2] = MoveRightTop;
            buttons[3] = MoveRightBottom;

            Width = 90;
            Height = 90;
            
            foreach(var b in buttons)
            {
                b.Visibility = Visibility.Hidden;
                Children.Add(b);
            }

            MoveLeftTop.Clicked2D += (s, e) => { OnMoveClicked?.Invoke(MoveLeftTop, MoveDirection.LeftTop); };
            MoveLeftBottom.Clicked2D += (s, e) => { OnMoveClicked?.Invoke(MoveLeftBottom, MoveDirection.LeftBottom); };
            MoveRightTop.Clicked2D += (s, e) => { OnMoveClicked?.Invoke(MoveRightTop, MoveDirection.RightTop); };
            MoveRightBottom.Clicked2D += (s, e) => { OnMoveClicked?.Invoke(MoveRightBottom, MoveDirection.RightBottom); };
        }

        protected override bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult hitResult)
        {
            hitResult = null;
            if (LayoutBoundWithTransform.Contains(mousePoint))
            {             
                foreach(var b in buttons)
                {
                    b.Visibility = Visibility.Visible;
                }
                return base.OnHitTest(ref mousePoint, out hitResult);
            }
            else
            {
                foreach (var b in buttons)
                {
                    b.Visibility = Visibility.Hidden;
                }
                return false;
            }
        }
    }
}
