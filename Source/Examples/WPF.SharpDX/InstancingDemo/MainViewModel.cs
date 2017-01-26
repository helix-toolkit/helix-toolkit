// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace InstancingDemo
{
    using System.Collections.Generic;

    using DemoCore;

    using HelixToolkit.Wpf.SharpDX;
    using SharpDX;
    using Media3D = System.Windows.Media.Media3D;
    using Point3D = System.Windows.Media.Media3D.Point3D;
    using Vector3D = System.Windows.Media.Media3D.Vector3D;
    using HelixToolkit.Wpf;
    using System;
    using System.IO;

    public class MainViewModel : BaseViewModel
    {
        public MeshGeometry3D Model { get; private set; }
        public LineGeometry3D Lines { get; private set; }
        public LineGeometry3D Grid { get; private set; }
        public IEnumerable<Matrix> ModelInstances { get; private set; }

        public IEnumerable<InstanceParameter> InstanceParam { get; private set; }

        public PhongMaterial ModelMaterial { get; private set; }        
        public Media3D.Transform3D ModelTransform { get; private set; }

        public Vector3 DirectionalLightDirection { get; private set; }
        public Color4 DirectionalLightColor { get; private set; }
        public Color4 AmbientLightColor { get; private set; }


        public MainViewModel()
        {
            Title = "Instancing Demo";            

            // camera setup
            Camera = new PerspectiveCamera { Position = new Point3D(3, 3, 5), LookDirection = new Vector3D(-3, -3, -5), UpDirection = new Vector3D(0, 1, 0) };

            // setup lighting            
            this.AmbientLightColor = new Color4(0.1f, 0.1f, 0.1f, 1.0f);
            this.DirectionalLightColor = (Color4)Color.White;
            this.DirectionalLightDirection = new Vector3(-2, -5, -2);

            // scene model3d
            var b1 = new MeshBuilder(true, true, true); 
            b1.AddBox(new Vector3(0, 0, 0), 1, 1, 1, BoxFaces.All);
            Model = b1.ToMeshGeometry3D();
            for(int i=0; i<Model.TextureCoordinates.Count; ++i)
            {
                var tex = Model.TextureCoordinates[i];
                Model.TextureCoordinates[i] = new Vector2(tex.X * 0.5f, tex.Y * 0.5f);
            }
            var l1 = new LineBuilder();
            l1.AddBox(new Vector3(0, 0, 0), 0.8, 0.8, 0.5);
            Lines = l1.ToLineGeometry3D();   

            int num = 10;
            var instances = new List<Matrix>();
            var parameters = new List<InstanceParameter>();
            var rnd = new Random();
            for (int i = -num; i < num; i++)
            {
                for (int j = -num; j < num; j++)
                {
                    var matrix = Matrix.RotationAxis(new Vector3(rnd.NextFloat(0,1), rnd.NextFloat(0, 1), rnd.NextFloat(0, 1)), rnd.NextFloat(-1, 1)) * Matrix.Translation(new Vector3(i*1.2f / 1.0f, j*1.2f / 1.0f, i*j/2.0f));
                    var color = new Color4((float)Math.Abs(i)/num, (float)Math.Abs(j)/num, (float)Math.Abs(i+j)/(2*num), 1);
                  //  var emissiveColor = new Color4( rnd.NextFloat(0,1) , rnd.NextFloat(0, 1), rnd.NextFloat(0, 1), rnd.NextFloat(0, 0.2f));
                    var k = Math.Abs(i + j) % 4;
                    Vector2 offset;
                    if (k == 0)
                    {
                        offset = new Vector2(0, 0);
                    }
                    else if (k == 1)
                    {
                        offset = new Vector2(0.5f, 0);
                    }
                    else if (k == 2)
                    {
                        offset = new Vector2(0.5f, 0.5f);
                    }
                    else
                    {
                        offset = new Vector2(0, 0.5f);
                    }

                    parameters.Add(new InstanceParameter() { DiffuseColor = color, TexCoordOffset = offset });
                    instances.Add(matrix);
                }
            }
            InstanceParam = parameters;
            ModelInstances = instances;
            SubTitle = "Number of Instances: " + parameters.Count.ToString();

            // model trafo
            ModelTransform = Media3D.Transform3D.Identity;// new Media3D.RotateTransform3D(new Media3D.AxisAngleRotation3D(new Vector3D(0, 0, 1), 45));

            // model material
            ModelMaterial = PhongMaterials.Glass;
            ModelMaterial.DiffuseMap = new FileStream(new System.Uri(@"TextureCheckerboard2.jpg", System.UriKind.RelativeOrAbsolute).ToString(), FileMode.Open);
            ModelMaterial.NormalMap = new FileStream(new System.Uri(@"TextureCheckerboard2_dot3.jpg", System.UriKind.RelativeOrAbsolute).ToString(), FileMode.Open);
            RenderTechniquesManager = new DefaultRenderTechniquesManager();
            RenderTechnique = RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Blinn];
            EffectsManager = new DefaultEffectsManager(RenderTechniquesManager);
        }
    }
}