// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MorphTargetAnimationDemo
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Media3D = System.Windows.Media.Media3D;
    using DemoCore;
    using HelixToolkit.Wpf.SharpDX;

    public class MainViewModel : BaseViewModel
    {
        public Geometry3D BaseCube { get; private set; }
        public PhongMaterial DefaultMateral { get; private set; }

        public MainViewModel()
        {
            EffectsManager = new DefaultEffectsManager();

            //Setup cube
            MeshBuilder mb = new MeshBuilder();
            mb.AddCube();
            BaseCube = mb.ToMesh();

            //Setup cube's material
            DefaultMateral = new PhongMaterial();
            DefaultMateral.DiffuseColor = new SharpDX.Color4(1, 0, 0, 1);
        }
    }
}