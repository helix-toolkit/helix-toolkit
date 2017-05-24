// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParticleStormModel3D.cs" company="Helix Toolkit">
//   Copyright (c) 2017 Helix Toolkit contributors
// </copyright>
// <author>Lunci Hua</author>
// <summary>
//  Particle system.
//  References: https://github.com/spazzarama/Direct3D-Rendering-Cookbook
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using SharpDX;
using System.Windows;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;
using HelixToolkit.Wpf.SharpDX.Randoms;
using System.IO;
using Media3D = System.Windows.Media.Media3D;
using Media = System.Windows.Media;
using System.Diagnostics;
using HelixToolkit.Wpf.SharpDX.Utilities;

namespace HelixToolkit.Wpf.SharpDX
{
    public class ParticleStormModel3D : Model3D
    {
        #region Dependency Properties
        public static DependencyProperty ParticleCountProperty = DependencyProperty.Register("ParticleCount", typeof(int), typeof(ParticleStormModel3D),
            new PropertyMetadata(ParticleParameters.DefaultParticleCount,
            (d, e) =>
            {
                (d as ParticleStormModel3D).OnInitialParticleChanged(System.Math.Max(8, (int)e.NewValue));
            }
            ));

        public int ParticleCount
        {
            set
            {
                SetValue(ParticleCountProperty, value);
            }
            get
            {
                return (int)GetValue(ParticleCountProperty);
            }
        }

        public static DependencyProperty EmitterLocationProperty = DependencyProperty.Register("EmitterLocation", typeof(Media3D.Point3D), typeof(ParticleStormModel3D),
            new PropertyMetadata(ParticleParameters.DefaultEmitterLocation.ToPoint3D(),
            (d, e) =>
            {
                (d as ParticleStormModel3D).parameters.insertVariables.EmitterLocation = (((Media3D.Point3D)e.NewValue).ToVector3());
            }
            ));

        public Media3D.Point3D EmitterLocation
        {
            set
            {
                SetValue(EmitterLocationProperty, value);
            }
            get
            {
                return (Media3D.Point3D)GetValue(EmitterLocationProperty);
            }
        }

        public static DependencyProperty EmitterRadiusProperty = DependencyProperty.Register("EmitterRadius", typeof(float), typeof(ParticleStormModel3D),
            new PropertyMetadata(ParticleParameters.DefaultConsumerRadius,
            (d, e) =>
            {
                (d as ParticleStormModel3D).parameters.insertVariables.EmitterRadius = ((float)e.NewValue);
            }
            ));

        public float EmitterRadius
        {
            set
            {
                SetValue(EmitterRadiusProperty, value);
            }
            get
            {
                return (float)GetValue(EmitterRadiusProperty);
            }
        }

        public static DependencyProperty ConsumerLocationProperty = DependencyProperty.Register("ConsumerLocation", typeof(Media3D.Point3D), typeof(ParticleStormModel3D),
            new PropertyMetadata(ParticleParameters.DefaultConsumerLocation.ToPoint3D(),
            (d, e) =>
            {
                (d as ParticleStormModel3D).parameters.frameVariables.ConsumerLocation = (((Media3D.Point3D)e.NewValue).ToVector3());
            }
            ));

        public Media3D.Point3D ConsumerLocation
        {
            set
            {
                SetValue(ConsumerLocationProperty, value);
            }
            get
            {
                return (Media3D.Point3D)GetValue(ConsumerLocationProperty);
            }
        }

        public static DependencyProperty ConsumerGravityProperty = DependencyProperty.Register("ConsumerGravity", typeof(float), typeof(ParticleStormModel3D),
            new PropertyMetadata(ParticleParameters.DefaultConsumerGravity,
            (d, e) =>
            {
                (d as ParticleStormModel3D).parameters.frameVariables.ConsumerGravity = ((float)e.NewValue);
            }
            ));

        public float ConsumerGravity
        {
            set
            {
                SetValue(ConsumerGravityProperty, value);
            }
            get
            {
                return (float)GetValue(ConsumerGravityProperty);
            }
        }

        public static DependencyProperty ConsumerRadiusProperty = DependencyProperty.Register("ConsumerRadius", typeof(float), typeof(ParticleStormModel3D),
            new PropertyMetadata(ParticleParameters.DefaultConsumerRadius,
            (d, e) =>
            {
                (d as ParticleStormModel3D).parameters.frameVariables.ConsumerRadius = ((float)e.NewValue);
            }
            ));

        public float ConsumerRadius
        {
            set
            {
                SetValue(ConsumerRadiusProperty, value);
            }
            get
            {
                return (float)GetValue(ConsumerRadiusProperty);
            }
        }

        public static DependencyProperty InitialEnergyProperty = DependencyProperty.Register("InitialEnergy", typeof(float), typeof(ParticleStormModel3D),
            new PropertyMetadata(ParticleParameters.DefaultInitialEnergy,
            (d, e) =>
            {
                (d as ParticleStormModel3D).parameters.insertVariables.InitialEnergy = System.Math.Max(1f, (float)e.NewValue);
                (d as ParticleStormModel3D).parameters.UpdateInsertThrottle();
            }
            ));

        public float InitialEnergy
        {
            set
            {
                SetValue(InitialEnergyProperty, value);
            }
            get
            {
                return (float)GetValue(InitialEnergyProperty);
            }
        }

        public static DependencyProperty EnergyDissipationRateProperty = DependencyProperty.Register("EnergyDissipationRate", typeof(float), typeof(ParticleStormModel3D),
            new PropertyMetadata(ParticleParameters.DefaultEnergyDissipationRate,
            (d, e) =>
            {
                (d as ParticleStormModel3D).parameters.insertVariables.EnergyDissipationRate = System.Math.Max(1f, (float)e.NewValue);
            }
            ));

        public float EnergyDissipationRate
        {
            set
            {
                SetValue(EnergyDissipationRateProperty, value);
            }
            get
            {
                return (float)GetValue(EnergyDissipationRateProperty);
            }
        }

        public static DependencyProperty RandomVectorGeneratorProperty = DependencyProperty.Register("RandomVectorGenerator", typeof(IRandomVector), typeof(ParticleStormModel3D),
            new PropertyMetadata(new UniformRandomVectorGenerator(),
            (d, e) =>
            {
                (d as ParticleStormModel3D).parameters.vectorGenerator = (IRandomVector)e.NewValue;
            }
            ));

        public IRandomVector RandomVectorGenerator
        {
            set
            {
                SetValue(RandomVectorGeneratorProperty, value);
            }
            get
            {
                return (IRandomVector)GetValue(RandomVectorGeneratorProperty);
            }
        }

        public static DependencyProperty ParticleTextureProperty = DependencyProperty.Register("ParticleTexture", typeof(Stream), typeof(ParticleStormModel3D),
            new AffectsRenderPropertyMetadata(null,
            (d, e) =>
            {
                (d as ParticleStormModel3D).isTextureChanged = true;
            }
            ));

        public Stream ParticleTexture
        {
            set
            {
                SetValue(ParticleTextureProperty, value);
            }
            get
            {
                return (Stream)GetValue(ParticleTextureProperty);
            }
        }

        public static DependencyProperty NumTextureColumnProperty = DependencyProperty.Register("NumTextureColumn", typeof(int), typeof(ParticleStormModel3D),
            new PropertyMetadata(1,
            (d, e) =>
            {
                (d as ParticleStormModel3D).parameters.NumTextureColumn = (uint)System.Math.Max(1, (int)e.NewValue);
            }
            ));

        public int NumTextureColumn
        {
            set
            {
                SetValue(NumTextureColumnProperty, value);
            }
            get
            {
                return (int)GetValue(NumTextureColumnProperty);
            }
        }

        public static DependencyProperty NumTextureRowProperty = DependencyProperty.Register("NumTextureRow", typeof(int), typeof(ParticleStormModel3D),
            new PropertyMetadata(1,
            (d, e) =>
            {
                (d as ParticleStormModel3D).parameters.NumTextureRow = (uint)System.Math.Max(1, (int)e.NewValue);
            }
            ));

        public int NumTextureRow
        {
            set
            {
                SetValue(NumTextureRowProperty, value);
            }
            get
            {
                return (int)GetValue(NumTextureRowProperty);
            }
        }

        public static DependencyProperty ParticleSizeProperty = DependencyProperty.Register("ParticleSize", typeof(Size), typeof(ParticleStormModel3D),
            new AffectsRenderPropertyMetadata(new Size(ParticleParameters.DefaultParticleSize.X, ParticleParameters.DefaultParticleSize.Y),
                (d, e) =>
                {
                    var size = (Size)e.NewValue;
                    (d as ParticleStormModel3D).parameters.particleSize = new Vector2((float)size.Width, (float)size.Height);
                }));

        public Size ParticleSize
        {
            set
            {
                SetValue(ParticleSizeProperty, value);
            }
            get
            {
                return (Size)GetValue(ParticleSizeProperty);
            }
        }


        public static DependencyProperty InitialVelocityProperty = DependencyProperty.Register("InitialVelocity", typeof(float), typeof(ParticleStormModel3D),
            new PropertyMetadata(ParticleParameters.DefaultInitialVelocity,
            (d, e) =>
            {
                (d as ParticleStormModel3D).parameters.insertVariables.InitialVelocity = (float)e.NewValue;
            }
            ));

        public float InitialVelocity
        {
            set
            {
                SetValue(InitialVelocityProperty, value);
            }
            get
            {
                return (float)GetValue(InitialVelocityProperty);
            }
        }

        public static DependencyProperty AccelerationProperty = DependencyProperty.Register("Acceleration", typeof(Media3D.Vector3D), typeof(ParticleStormModel3D),
            new PropertyMetadata(ParticleParameters.DefaultAcceleration.ToVector3D(),
            (d, e) =>
            {
                (d as ParticleStormModel3D).parameters.insertVariables.InitialAcceleration = ((Media3D.Vector3D)e.NewValue).ToVector3();
            }
            ));

        public Media3D.Vector3D Acceleration
        {
            set
            {
                SetValue(AccelerationProperty, value);
            }
            get
            {
                return (Media3D.Vector3D)GetValue(AccelerationProperty);
            }
        }

        public static DependencyProperty ParticleBoundsProperty = DependencyProperty.Register("ParticleBounds", typeof(Media3D.Rect3D), typeof(ParticleStormModel3D),
            new PropertyMetadata(ParticleParameters.DefaultBound,
            (d, e) =>
            {
                var bound = (Media3D.Rect3D)e.NewValue;
                (d as ParticleStormModel3D).parameters.frameVariables.DomainBoundsMax = new Vector3((float)(bound.SizeX / 2 + bound.Location.X), (float)(bound.SizeY / 2 + bound.Location.Y), (float)(bound.SizeZ / 2 + bound.Location.Z));
                (d as ParticleStormModel3D).parameters.frameVariables.DomainBoundsMin = new Vector3((float)(bound.Location.X - bound.SizeX / 2), (float)(bound.Location.Y - bound.SizeY / 2), (float)(bound.Location.Z - bound.SizeZ / 2));
            }));

        public Media3D.Rect3D ParticleBounds
        {
            set
            {
                SetValue(ParticleBoundsProperty, value);
            }
            get
            {
                return (Media3D.Rect3D)GetValue(ParticleBoundsProperty);
            }
        }

        public static DependencyProperty CumulateAtBoundProperty = DependencyProperty.Register("CumulateAtBound", typeof(bool), typeof(ParticleStormModel3D),
            new PropertyMetadata(false,
                (d, e) =>
                {
                    (d as ParticleStormModel3D).parameters.frameVariables.CumulateAtBound = (bool)e.NewValue ? 1u : 0;
                }));

        public bool CumulateAtBound
        {
            set
            {
                SetValue(CumulateAtBoundProperty, value);
            }
            get
            {
                return (bool)GetValue(CumulateAtBoundProperty);
            }
        }

        public static DependencyProperty BlendColorProperty = DependencyProperty.Register("BlendColor", typeof(Media.Color), typeof(ParticleStormModel3D),
            new PropertyMetadata(Media.Colors.White,
                (d, e) =>
                {
                    (d as ParticleStormModel3D).parameters.insertVariables.ParticleBlendColor = ((Media.Color)e.NewValue).ToColor4();
                }));

        public Media.Color BlendColor
        {
            set
            {
                SetValue(BlendColorProperty, value);
            }
            get
            {
                return (Media.Color)GetValue(BlendColorProperty);
            }
        }

        public static DependencyProperty AnimateSpriteByEnergyBoundProperty = DependencyProperty.Register("AnimateSpriteByEnergy", typeof(bool), typeof(ParticleStormModel3D),
            new PropertyMetadata(false,
                (d, e) =>
                {
                    (d as ParticleStormModel3D).parameters.animateSpriteByEnergy = (bool)e.NewValue ? 1u : 0;
                }));

        public bool AnimateSpriteByEnergy
        {
            set
            {
                SetValue(AnimateSpriteByEnergyBoundProperty, value);
            }
            get
            {
                return (bool)GetValue(AnimateSpriteByEnergyBoundProperty);
            }
        }

        public static DependencyProperty BlendProperty = DependencyProperty.Register("Blend", typeof(BlendOperation), typeof(ParticleStormModel3D),
            new PropertyMetadata(BlendOperation.Add, (d, e) =>
            {
                (d as ParticleStormModel3D).isBlendChanged = true;
            }));

        public BlendOperation Blend
        {
            set
            {
                SetValue(BlendProperty, value);
            }
            get
            {
                return (BlendOperation)GetValue(BlendProperty);
            }
        }

        public static DependencyProperty AlphaBlendProperty = DependencyProperty.Register("AlphaBlend", typeof(BlendOperation), typeof(ParticleStormModel3D),
            new PropertyMetadata(BlendOperation.Add, (d, e) =>
            {
                (d as ParticleStormModel3D).isBlendChanged = true;
            }));

        public BlendOperation AlphaBlend
        {
            set
            {
                SetValue(AlphaBlendProperty, value);
            }
            get
            {
                return (BlendOperation)GetValue(AlphaBlendProperty);
            }
        }

        public static DependencyProperty SourceBlendProperty = DependencyProperty.Register("SourceBlend", typeof(BlendOption), typeof(ParticleStormModel3D),
            new PropertyMetadata(BlendOption.One, (d, e) =>
            {
                (d as ParticleStormModel3D).isBlendChanged = true;
            }));

        public BlendOption SourceBlend
        {
            set
            {
                SetValue(SourceBlendProperty, value);
            }
            get
            {
                return (BlendOption)GetValue(SourceBlendProperty);
            }
        }

        public static DependencyProperty DestBlendProperty = DependencyProperty.Register("DestBlend", typeof(BlendOption), typeof(ParticleStormModel3D),
            new PropertyMetadata(BlendOption.One, (d, e) =>
            {
                (d as ParticleStormModel3D).isBlendChanged = true;
            }));

        public BlendOption DestBlend
        {
            set
            {
                SetValue(DestBlendProperty, value);
            }
            get
            {
                return (BlendOption)GetValue(DestBlendProperty);
            }
        }

        public static DependencyProperty SourceAlphaBlendProperty = DependencyProperty.Register("SourceAlphaBlend", typeof(BlendOption), typeof(ParticleStormModel3D),
            new PropertyMetadata(BlendOption.One, (d, e) =>
            {
                (d as ParticleStormModel3D).isBlendChanged = true;
            }));

        public BlendOption SourceAlphaBlend
        {
            set
            {
                SetValue(SourceAlphaBlendProperty, value);
            }
            get
            {
                return (BlendOption)GetValue(SourceAlphaBlendProperty);
            }
        }

        public static DependencyProperty DestAlphaBlendProperty = DependencyProperty.Register("DestAlphaBlend", typeof(BlendOption), typeof(ParticleStormModel3D),
            new PropertyMetadata(BlendOption.Zero, (d, e) =>
            {
                (d as ParticleStormModel3D).isBlendChanged = true;
            }));

        public BlendOption DestAlphaBlend
        {
            set
            {
                SetValue(DestAlphaBlendProperty, value);
            }
            get
            {
                return (BlendOption)GetValue(DestAlphaBlendProperty);
            }
        }
        #endregion
        #region variables

        private float totalElapsed = 0;

        private bool isInitialParticleChanged = true;

        private bool isRestart = true;

        protected bool isTextureChanged = true;

        protected bool hasTexture = false;

        private bool isBlendChanged = true;

        protected ParticleParameters parameters = new ParticleParameters();

        protected EffectScalarVariable bHasTextureVar;

        protected EffectShaderResourceVariable textureViewVar;

        protected ShaderResourceView textureView;

        protected EffectTransformVariables effectTransforms;

        private BufferDescription bufferDesc = new BufferDescription()
        {
            BindFlags = BindFlags.UnorderedAccess | BindFlags.ShaderResource,
            OptionFlags = ResourceOptionFlags.BufferStructured,
            StructureByteStride = Particle.SizeInBytes,
            CpuAccessFlags = CpuAccessFlags.None,
            Usage = ResourceUsage.Default
        };

        //Buffer indirectArgsBuffer;
        private readonly ConstantBufferProxy<ParticleCountIndirectArgs> particleCountGSIABuffer = new ConstantBufferProxy<ParticleCountIndirectArgs>(ParticleCountIndirectArgs.SizeInBytes, 
            BindFlags.None, CpuAccessFlags.None, ResourceOptionFlags.DrawIndirectArguments);
        private readonly ConstantBufferProxy<ParticlePerFrame> frameConstBuffer = new ConstantBufferProxy<ParticlePerFrame>(ParticlePerFrame.SizeInBytes);
        private readonly ConstantBufferProxy<ParticleInsertParameters> particleInsertBuffer = new ConstantBufferProxy<ParticleInsertParameters>(ParticleInsertParameters.SizeInBytes);
#if DEBUG
        private Buffer particleCountStaging;
#endif
        private UnorderedAccessViewDescription UAVBufferViewDesc = new UnorderedAccessViewDescription()
        {
            Dimension = UnorderedAccessViewDimension.Buffer,
            Format = global::SharpDX.DXGI.Format.Unknown,
            Buffer = new UnorderedAccessViewDescription.BufferResource { FirstElement = 0, Flags = UnorderedAccessViewBufferFlags.Append }
        };

        private ShaderResourceViewDescription SRVBufferViewDesc = new ShaderResourceViewDescription()
        {
            Dimension = ShaderResourceViewDimension.Buffer
        };

        private BufferDescription renderIndirectArgsBufDesc = new BufferDescription
        {
            BindFlags = BindFlags.None,
            SizeInBytes = 4 * global::SharpDX.Utilities.SizeOf<uint>(),
            StructureByteStride = 0,
            Usage = ResourceUsage.Default,
            CpuAccessFlags = CpuAccessFlags.None,
            OptionFlags = ResourceOptionFlags.DrawIndirectArguments
        };

        protected readonly BufferViewProxy[] BufferProxies = new BufferViewProxy[2];

        protected EffectTechnique effectTechnique;

        protected BlendState blendState;
        protected BlendStateDescription blendDesc = new BlendStateDescription() { IndependentBlendEnable = false, AlphaToCoverageEnable = false };
        #endregion

        private void OnBlendStateChanged()
        {
            if (isBlendChanged)
            {
                Disposer.RemoveAndDispose(ref blendState);
                blendDesc.RenderTarget[0] = new RenderTargetBlendDescription
                {
                    IsBlendEnabled = true,
                    BlendOperation = this.Blend,
                    AlphaBlendOperation = this.AlphaBlend,
                    SourceBlend = this.SourceBlend,
                    DestinationBlend = this.DestBlend,
                    SourceAlphaBlend = this.SourceAlphaBlend,
                    DestinationAlphaBlend = this.DestAlphaBlend,
                    RenderTargetWriteMask = ColorWriteMaskFlags.All
                };
                blendState = new BlendState(this.Device, blendDesc);
                isBlendChanged = false;
            }
        }

        private void OnInitialParticleChanged(int count)
        {
            isInitialParticleChanged = true;
            parameters.particleCountInternal = count;
            if (count <= 0 || !IsAttached)
            {
                return;
            }
            else if (bufferDesc.SizeInBytes < count * Particle.SizeInBytes) // Create new buffer, otherwise reuse existing buffers
            {
                Debug.WriteLine("Create buffers");
                DisposeBuffers();
                InitializeBuffers(count);
            }
            parameters.UpdateInsertThrottle();
            isInitialParticleChanged = false;
            isRestart = true;
        }



        private void DisposeBuffers()
        {
            particleCountGSIABuffer.Dispose();
            frameConstBuffer.Dispose();
            particleInsertBuffer.Dispose();
#if DEBUG
            Disposer.RemoveAndDispose(ref particleCountStaging);
#endif
            if (BufferProxies != null)
            {
                for (int i = 0; i < BufferProxies.Length; ++i)
                {
                    BufferProxies[i]?.Dispose();
                    BufferProxies[i] = null;
                }
            }
        }

        private void InitializeBuffers(int count)
        {
            bufferDesc.SizeInBytes = parameters.particleCountInternal * Particle.SizeInBytes;
            UAVBufferViewDesc.Buffer.ElementCount = parameters.particleCountInternal;

            for (int i = 0; i < BufferProxies.Length; ++i)
            {
                BufferProxies[i] = new BufferViewProxy(Device, ref bufferDesc, ref UAVBufferViewDesc, ref SRVBufferViewDesc);
            }

#if DEBUG
            var stagingbufferDesc = new BufferDescription()
            {
                BindFlags = BindFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = 4 * global::SharpDX.Utilities.SizeOf<uint>(),
                CpuAccessFlags = CpuAccessFlags.Read,
                Usage = ResourceUsage.Staging
            };
            particleCountStaging = new Buffer(this.Device, stagingbufferDesc);
#endif
            particleCountGSIABuffer.CreateBuffer(this.Device);
            ParticleCountIndirectArgs args = new ParticleCountIndirectArgs();
            args.InstanceCount = 1;
            Device.ImmediateContext.UpdateSubresource(ref args, particleCountGSIABuffer.Buffer);
            frameConstBuffer.CreateBuffer(this.Device);
            particleInsertBuffer.CreateBuffer(this.Device);
        }

        private void OnTextureChanged()
        {
            if (isTextureChanged)
            {
                Disposer.RemoveAndDispose(ref textureView);
                if (!IsAttached)
                {
                    return;
                }
                if (ParticleTexture == null)
                {
                    hasTexture = false;
                }
                else
                {
                    textureView = TextureLoader.FromMemoryAsShaderResourceView(this.Device, ParticleTexture);
                    hasTexture = true;
                }
                isTextureChanged = false;
            }
        }

        protected override bool OnAttach(IRenderHost host)
        {
            parameters.OnAttach(this.effect);
            this.effectTransforms = new EffectTransformVariables(effect);
            effectTechnique = effect.GetTechniqueByName(this.renderTechnique.Name);
            bHasTextureVar = effect.GetVariableByName("bHasDiffuseMap").AsScalar();
            textureViewVar = effect.GetVariableByName("texDiffuseMap").AsShaderResource();
            System.Windows.Media.CompositionTarget.Rendering += CompositionTarget_Rendering;
            return true;
        }

        private void CompositionTarget_Rendering(object sender, System.EventArgs e)
        {
            InvalidateRender();
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            if (isInitialParticleChanged)
            {
                OnInitialParticleChanged(ParticleCount);
            }
        }

        protected override void OnDetach()
        {
            System.Windows.Media.CompositionTarget.Rendering -= CompositionTarget_Rendering;
            parameters.OnDettach();
            Disposer.RemoveAndDispose(ref effectTransforms);
            Disposer.RemoveAndDispose(ref bHasTextureVar);
            Disposer.RemoveAndDispose(ref textureViewVar);
            Disposer.RemoveAndDispose(ref blendState);
            DisposeBuffers();
            base.OnDetach();
        }

        protected override RenderTechnique SetRenderTechnique(IRenderHost host)
        {
            return host.RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.ParticleStorm];
        }

        protected override bool CanRender(RenderContext context)
        {
            return base.CanRender(context) && BufferProxies != null && !isInitialParticleChanged;
        }

        private void SetVariables()
        {
            bHasTextureVar.Set(hasTexture);
            textureViewVar.SetResource(hasTexture ? textureView : null);
            parameters.SetRenderVariables();
        }

        protected override void OnRender(RenderContext context)
        {
            var worldMatrix = this.modelMatrix * context.worldMatrix;
            this.effectTransforms.mWorld.SetMatrix(ref worldMatrix);

            OnTextureChanged();
            OnBlendStateChanged();
            SetVariables();

            parameters.UpdateTime(context, ref totalElapsed);

            EffectPass pass;

            if (isRestart)
            {
                pass = this.effectTechnique.GetPassByIndex(1);
                pass.Apply(context.DeviceContext);
                // Reset Both UAV buffers
                context.DeviceContext.ComputeShader.SetUnorderedAccessView(0, BufferProxies[0].UAV, 0);
                context.DeviceContext.ComputeShader.SetUnorderedAccessView(1, BufferProxies[1].UAV, 0);
                context.DeviceContext.ComputeShader.SetConstantBuffer(1, frameConstBuffer.Buffer);
                // Call ComputeShader to add initial particles
                context.DeviceContext.Dispatch(1, 1, 1);
                isRestart = false;
            }
            else
            {
                //upload framebuffer
                context.DeviceContext.UpdateSubresource(ref parameters.frameVariables, frameConstBuffer.Buffer);
                // Get consume buffer count
                context.DeviceContext.CopyStructureCount(frameConstBuffer.Buffer, ParticlePerFrame.NumParticlesOffset, BufferProxies[0].UAV);
                // Calculate existing particles
                pass = this.effectTechnique.GetPassByIndex(1);
                pass.Apply(context.DeviceContext);
                context.DeviceContext.ComputeShader.SetUnorderedAccessView(0, BufferProxies[0].UAV);
                context.DeviceContext.ComputeShader.SetUnorderedAccessView(1, BufferProxies[1].UAV, 0);
                context.DeviceContext.ComputeShader.SetConstantBuffer(1, frameConstBuffer.Buffer);
                context.DeviceContext.Dispatch(System.Math.Max(1, parameters.particleCountInternal / 512), 1, 1);
                // Get append buffer count
                context.DeviceContext.CopyStructureCount(particleCountGSIABuffer.Buffer, 0, BufferProxies[1].UAV);
            }

            //#if DEBUG
            //            DebugCount("UAV 0", context.DeviceContext, BufferProxies[0].UAV);
            //#endif


            if (totalElapsed > parameters.insertThrottle)
            {
                context.DeviceContext.UpdateSubresource(ref parameters.insertVariables, particleInsertBuffer.Buffer);
                // Add more particles 
                pass = this.effectTechnique.GetPassByIndex(0);
                pass.Apply(context.DeviceContext);
                context.DeviceContext.ComputeShader.SetUnorderedAccessView(1, BufferProxies[1].UAV);
                context.DeviceContext.ComputeShader.SetConstantBuffer(1, particleInsertBuffer.Buffer);
                context.DeviceContext.Dispatch(1, 1, 1);
                totalElapsed = 0;
#if DEBUG
                DebugCount("UAV 1", context.DeviceContext, BufferProxies[1].UAV);
#endif
            }

            // Clear
            context.DeviceContext.ComputeShader.SetUnorderedAccessView(0, null);
            context.DeviceContext.ComputeShader.SetUnorderedAccessView(1, null);

            // Swap UAV buffers for next frame
            var bproxy = BufferProxies[0];
            BufferProxies[0] = BufferProxies[1];
            BufferProxies[1] = bproxy;

            // Render existing particles
            parameters.simulationStateVar.SetResource(BufferProxies[0].SRV);
            context.DeviceContext.InputAssembler.InputLayout = null;
            context.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.PointList;
            pass = this.effectTechnique.GetPassByIndex(2);
            pass.Apply(context.DeviceContext);
            context.DeviceContext.OutputMerger.SetBlendState(blendState, null, 0xFFFFFFFF);
            context.DeviceContext.DrawInstancedIndirect(particleCountGSIABuffer.Buffer, 0);
        }

#if DEBUG
        private void DebugCount(string src, DeviceContext context, UnorderedAccessView uav)
        {
            context.CopyStructureCount(particleCountStaging, 0, uav);
            DataStream ds;
            var db = context.MapSubresource(particleCountStaging, MapMode.Read, MapFlags.None, out ds);
            int CurrentParticleCount = ds.ReadInt();
            Debug.WriteLine("{0}: {1}", src, CurrentParticleCount);
            context.UnmapSubresource(particleCountStaging, 0);
        }
#endif
        protected class ParticleParameters : IParameterVariables
        {
            public static readonly int DefaultParticleCount = 512;
            public static readonly float DefaultInitialVelocity = 1f;
            public static readonly Vector3 DefaultAcceleration = new Vector3(0, 0.1f, 0);
            public static readonly Vector2 DefaultParticleSize = new Vector2(1, 1);
            public static readonly Vector3 DefaultEmitterLocation = Vector3.Zero;
            public static readonly Vector3 DefaultConsumerLocation = new Vector3(0, 10, 0);
            public static readonly float DefaultConsumerGravity = 0;
            public static readonly float DefaultConsumerRadius = 0;
            public static readonly Media3D.Rect3D DefaultBound = new Media3D.Rect3D(0, 0, 0, 10, 10, 10);
            public static readonly Vector3 DefaultBoundMaximum = new Vector3(5, 5, 5);
            public static readonly Vector3 DefaultBoundMinimum = new Vector3(-5, -5, -5);
            public static readonly float DefaultInitialEnergy = 5;
            public static readonly float DefaultEnergyDissipationRate = 1f;

            public uint NumTextureColumn = 1;

            public uint NumTextureRow = 1;

            public int particleCountInternal = DefaultParticleCount;

            public float insertThrottle = 0;

            public float prevTimeMillis = 0;

            public uint animateSpriteByEnergy = 0;

            public IRandomVector vectorGenerator = new UniformRandomVectorGenerator();

            public Vector2 particleSize = DefaultParticleSize;

            public ParticlePerFrame frameVariables = new ParticlePerFrame() { ExtraAcceleration = DefaultAcceleration, CumulateAtBound = 0, DomainBoundsMax = DefaultBoundMaximum, DomainBoundsMin = DefaultBoundMinimum, ConsumerGravity = DefaultConsumerGravity, ConsumerLocation = DefaultConsumerLocation, ConsumerRadius = DefaultConsumerRadius };

            public ParticleInsertParameters insertVariables = new ParticleInsertParameters() { EmitterLocation = DefaultEmitterLocation, EnergyDissipationRate = DefaultEnergyDissipationRate, InitialAcceleration = DefaultAcceleration, InitialEnergy = DefaultInitialEnergy, InitialVelocity = DefaultInitialVelocity, ParticleBlendColor = Color.White.ToColor4() };

            public EffectVectorVariable particleSizeVar;

            public EffectVectorVariable randomVectorVar;

            public EffectScalarVariable randomSeedVar;

            public EffectScalarVariable numTextureColumnVar;
            public EffectScalarVariable numTextureRowVar;

            public EffectUnorderedAccessViewVariable currentSimulationStateVar;

            public EffectUnorderedAccessViewVariable newSimulationStateVar;

            public EffectShaderResourceVariable simulationStateVar;

            public EffectScalarVariable animateSpriteByEnergyVar;

            public virtual void OnAttach(Effect effect)
            {
                currentSimulationStateVar = effect.GetVariableByName("CurrentSimulationState").AsUnorderedAccessView();
                newSimulationStateVar = effect.GetVariableByName("NewSimulationState").AsUnorderedAccessView();
                simulationStateVar = effect.GetVariableByName("SimulationState").AsShaderResource();
                particleSizeVar = effect.GetVariableByName("ParticleSize").AsVector();
                randomVectorVar = effect.GetVariableByName("RandomVector").AsVector();
                randomSeedVar = effect.GetVariableByName("RandomSeed").AsScalar();
                numTextureColumnVar = effect.GetVariableByName("NumTexCol").AsScalar();
                numTextureRowVar = effect.GetVariableByName("NumTexRow").AsScalar();
                animateSpriteByEnergyVar = effect.GetVariableByName("AnimateByEnergyLevel").AsScalar();
            }

            public virtual void OnDettach()
            {
                Disposer.RemoveAndDispose(ref currentSimulationStateVar);
                Disposer.RemoveAndDispose(ref newSimulationStateVar);
                Disposer.RemoveAndDispose(ref simulationStateVar);
                Disposer.RemoveAndDispose(ref particleSizeVar);
                Disposer.RemoveAndDispose(ref randomVectorVar);
                Disposer.RemoveAndDispose(ref randomSeedVar);
                Disposer.RemoveAndDispose(ref numTextureColumnVar);
                Disposer.RemoveAndDispose(ref numTextureRowVar);
                Disposer.RemoveAndDispose(ref animateSpriteByEnergyVar);
            }

            public void SetRenderVariables()
            {
                particleSizeVar.Set(particleSize);
                randomVectorVar.Set(vectorGenerator.RandomVector3);
                randomSeedVar.Set(vectorGenerator.Seed);
                numTextureColumnVar.Set(NumTextureColumn);
                numTextureRowVar.Set(NumTextureRow);
                animateSpriteByEnergyVar.Set(animateSpriteByEnergy);
            }

            public void UpdateInsertThrottle()
            {
                insertThrottle = (8.0f * insertVariables.InitialEnergy / insertVariables.EnergyDissipationRate / System.Math.Max(0, (particleCountInternal + 8)));
            }

            public void UpdateTime(RenderContext context, ref float totalElapsed)
            {
                float timeElapsed = ((float)context.TimeStamp.TotalMilliseconds - prevTimeMillis) / 1000;
                prevTimeMillis = (float)context.TimeStamp.TotalMilliseconds;
                totalElapsed += timeElapsed;
                //Update perframe variables
                frameVariables.TimeFactors = timeElapsed;
            }
        }
    }

    public sealed class BufferViewProxy : System.IDisposable
    {
        public Buffer Buffer;
        public UnorderedAccessView UAV;
        public ShaderResourceView SRV;

        public BufferViewProxy(Device device, ref BufferDescription bufferDesc, ref UnorderedAccessViewDescription uavDesc, ref ShaderResourceViewDescription srvDesc)
        {
            Buffer = new Buffer(device, bufferDesc);
            SRV = new ShaderResourceView(device, Buffer);
            UAV = new UnorderedAccessView(device, Buffer, uavDesc);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Disposer.RemoveAndDispose(ref UAV);
                    Disposer.RemoveAndDispose(ref SRV);
                    Disposer.RemoveAndDispose(ref Buffer);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~BufferViewProxy() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
