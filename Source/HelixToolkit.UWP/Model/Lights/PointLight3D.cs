/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
namespace HelixToolkit.UWP
{
    using Model;
    using Model.Scene;
    using SharpDX;
    using Windows.UI.Xaml;

    public class PointLight3D : Light3D
    {
        public static readonly DependencyProperty AttenuationProperty =
            DependencyProperty.Register("Attenuation", typeof(Vector3), typeof(PointLight3D), new PropertyMetadata(new Vector3(1.0f, 0.0f, 0.0f),
                (d, e) => {
                    ((d as Element3DCore).SceneNode as PointLightNode).Attenuation = ((Vector3)e.NewValue);
                }));

        public static readonly DependencyProperty RangeProperty =
            DependencyProperty.Register("Range", typeof(double), typeof(PointLight3D), new PropertyMetadata(100.0,
                (d, e) => {
                    ((d as Element3DCore).SceneNode as PointLightNode).Range = (float)(double)e.NewValue;
                }));

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(Vector3), typeof(PointLight3D), new PropertyMetadata(new Vector3(),
                (d, e) => {
                    ((d as Element3DCore).SceneNode as PointLightNode).Position = ((Vector3)e.NewValue);
                }));

        /// <summary>
        /// The position of the model in world space.
        /// </summary>
        public Vector3 Position
        {
            get { return (Vector3)this.GetValue(PositionProperty); }
            set { this.SetValue(PositionProperty, value); }
        }

        /// <summary>
        /// Attenuation coefficients:
        /// X = constant attenuation,
        /// Y = linar attenuation,
        /// Z = quadratic attenuation.
        /// For details see: http://msdn.microsoft.com/en-us/library/windows/desktop/bb172279(v=vs.85).aspx
        /// </summary>
        public Vector3 Attenuation
        {
            get { return (Vector3)this.GetValue(AttenuationProperty); }
            set { this.SetValue(AttenuationProperty, value); }
        }

        /// <summary>
        /// Range of this light. This is the maximum distance 
        /// of a pixel being lit by this light.
        /// For details see: http://msdn.microsoft.com/en-us/library/windows/desktop/bb172279(v=vs.85).aspx
        /// </summary>
        public double Range
        {
            get { return (double)this.GetValue(RangeProperty); }
            set { this.SetValue(RangeProperty, value); }
        }

        protected override SceneNode OnCreateSceneNode()
        {
            return new PointLightNode();
        }

        protected override void AssignDefaultValuesToSceneNode(SceneNode core)
        {
            base.AssignDefaultValuesToSceneNode(core);
            if(core is PointLightNode n)
            {
                n.Attenuation = Attenuation;
                n.Range = (float)Range;
                n.Position = Position;
            }
        }
    }
}
