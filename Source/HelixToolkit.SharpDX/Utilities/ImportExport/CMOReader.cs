﻿/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
// Port code from https://github.com/spazzarama/Direct3D-Rendering-Cookbook
// And https://raw.githubusercontent.com/wiki/Microsoft/DirectXMesh/cmodump.cpp
// Copyright (c) 2013 Justin Stenning
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// -----------------------------------------------------------------------------
// Portions of code are ported from DirectXTk http://directxtk.codeplex.com
// -----------------------------------------------------------------------------
// Microsoft Public License (Ms-PL)
//
// This license governs use of the accompanying software. If you use the 
// software, you accept this license. If you do not accept the license, do not
// use the software.
//
// 1. Definitions
// The terms "reproduce," "reproduction," "derivative works," and 
// "distribution" have the same meaning here as under U.S. copyright law.
// A "contribution" is the original software, or any additions or changes to 
// the software.
// A "contributor" is any person that distributes its contribution under this 
// license.
// "Licensed patents" are a contributor's patent claims that read directly on 
// its contribution.
//
// 2. Grant of Rights
// (A) Copyright Grant- Subject to the terms of this license, including the 
// license conditions and limitations in section 3, each contributor grants 
// you a non-exclusive, worldwide, royalty-free copyright license to reproduce
// its contribution, prepare derivative works of its contribution, and 
// distribute its contribution or any derivative works that you create.
// (B) Patent Grant- Subject to the terms of this license, including the license
// conditions and limitations in section 3, each contributor grants you a 
// non-exclusive, worldwide, royalty-free license under its licensed patents to
// make, have made, use, sell, offer for sale, import, and/or otherwise dispose
// of its contribution in the software or derivative works of the contribution 
// in the software.
//
// 3. Conditions and Limitations
// (A) No Trademark License- This license does not grant you rights to use any 
// contributors' name, logo, or trademarks.
// (B) If you bring a patent claim against any contributor over patents that 
// you claim are infringed by the software, your patent license from such 
// contributor to the software ends automatically.
// (C) If you distribute any portion of the software, you must retain all 
// copyright, patent, trademark, and attribution notices that are present in the
// software.
// (D) If you distribute any portion of the software in source code form, you 
// may do so only under this license by including a complete copy of this 
// license with your distribution. If you distribute any portion of the software
// in compiled or object code form, you may only do so under a license that 
// complies with this license.
// (E) The software is licensed "as-is." You bear the risk of using it. The
// contributors give no express warranties, guarantees or conditions. You may
// have additional consumer rights under your local laws which this license 
// cannot change. To the extent permitted under your local laws, the 
// contributors exclude the implied warranties of merchantability, fitness for a
// particular purpose and non-infringement.
#region CMO file structure
/*
 .CMO files

 UINT - Mesh count
 { [Mesh count]
      UINT - Length of name
      wchar_t[] - Name of mesh (if length > 0)
      UINT - Material count
      { [Material count]
          UINT - Length of material name
          wchar_t[] - Name of material (if length > 0)
          public Vector4 Ambient;
          public Vector4 Diffuse;
          public Vector4 Specular;
          public float SpecularPower;
          public Vector4 Emissive;
          public Matrix UVTransform;
          UINT - Length of pixel shader name
          wchar_t[] - Name of pixel shader (if length > 0)
          { [8]
              UINT - Length of texture name
              wchar_t[] - Name of texture (if length > 0)
          }
      }
      BYTE - 1 if there is skeletal animation data present
      UINT - SubMesh count
      { [SubMesh count]
          SubMesh structure
      }
      UINT - IB Count
      { [IB Count]
          UINT - Number of USHORTs in IB
          USHORT[] - Array of indices
      }
      UINT - VB Count
      { [VB Count]
          UINT - Number of verts in VB
          Vertex[] - Array of vertices
      }
      UINT - Skinning VB Count
      { [Skinning VB Count]
          UINT - Number of verts in Skinning VB
          SkinningVertex[] - Array of skinning verts
      }
      MeshExtents structure
      [If skeleton animation data is not present, file ends here]
      UINT - Bone count
      { [Bone count]
          UINT - Length of bone name
          wchar_t[] - Bone name (if length > 0)
          Bone structure
      }
      UINT - Animation clip count
      { [Animation clip count]
          UINT - Length of clip name
          wchar_t[] - Clip name (if length > 0)
          float - Start time
          float - End time
          UINT - Keyframe count
          { [Keyframe count]
              Keyframe structure
          }
      }
 }
*/
#endregion

using HelixToolkit.SharpDX.Animations;
using SharpDX;
using System.Numerics;
using System.Runtime.InteropServices;
using PhongMaterial = HelixToolkit.SharpDX.Model.PhongMaterialCore;

namespace HelixToolkit.SharpDX;

public class CMOReader : IModelReader
{
    public readonly List<Object3D> Meshes = new();
    /// <summary>
    /// The animation hirarchy
    /// </summary>
    public readonly List<AnimationHierarchy> AnimationHierarchy = new();
    /// <summary>
    /// The unique animations by animation name and corresponding animations by their guid
    /// </summary>
    public readonly Dictionary<string, List<Guid>> UniqueAnimations = new();

    public const int MaxBoneInfluences = 4; // 4 bone influences are supported
    public const int MaxTextures = 8;  // 8 unique textures are supported.
    /// <summary>
    /// Gets or sets the path to the textures.
    /// </summary>
    /// <value>The texture path.</value>
    public string TexturePath
    {
        get; set;
    } = string.Empty;

    /// <summary>
    /// Additional info how to treat the model
    /// </summary>
    public ModelInfo ModelInfo
    {
        get; private set;
    }
    public List<Object3D>? Read(string path, ModelInfo info = default)
    {
        this.TexturePath = Path.GetDirectoryName(path) ?? string.Empty;
        this.ModelInfo = info;

        using var s = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        return this.Read(s);
    }

    public List<Object3D>? Read(Stream s, ModelInfo info = default)
    {
        using (var br = new BinaryReader(s))
        {
            var meshCount = br.ReadUInt32();
            for (var i = 0; i < meshCount; ++i)
            {
                Meshes.AddRange(Load(br));
            }
        }
        return Meshes;
    }

    private IList<Object3D> Load(BinaryReader reader)
    {
        var name = reader.ReadCMO_wchar();
        var numMaterials = (int)reader.ReadUInt32();
        var materials = new List<Tuple<PhongMaterial, IList<string>>>(numMaterials);
        for (var i = 0; i < numMaterials; ++i)
        {
            var material = new PhongMaterial
            {
                Name = reader.ReadCMO_wchar(),
                AmbientColor = reader.ReadStructure<Color4>(),
                DiffuseColor = reader.ReadStructure<Color4>(),
                SpecularColor = reader.ReadStructure<Color4>(),
                SpecularShininess = reader.ReadSingle(),
                EmissiveColor = reader.ReadStructure<Color4>()
            };
            var uvTransform = reader.ReadStructure<Matrix>();
            if (uvTransform == MatrixHelper.Zero)
            {
                uvTransform = Matrix.Identity;
            }
            Matrix4x4.Decompose(uvTransform, out var s, out var r, out var tra);
            material.UVTransform = new UVTransform(r.Angle(), new Vector2(s.X, s.Y), new Vector2(tra.X, tra.Y));
            var pixelShaderName = reader.ReadCMO_wchar();//Not used
            var textures = new List<string>();
            for (var t = 0; t < MaxTextures; ++t)
            {
                textures.Add(reader.ReadCMO_wchar());
            }
            materials.Add(new Tuple<PhongMaterial, IList<string>>(material, textures));
        }

        //      BYTE - 1 if there is skeletal animation data present

        // is there skeletal animation data present?
        var isAnimationData = reader.ReadByte() == 1;
        //      UINT - SubMesh count
        //      { [SubMesh count]
        //          SubMesh structure
        //      }

        // load sub meshes if any

        var mesh = new MeshGeometry3D();
        var subMeshCount = (int)reader.ReadUInt32();

        var subMesh = new List<SubMesh>(subMeshCount);
        for (var i = 0; i < subMeshCount; i++)
        {
            subMesh.Add(reader.ReadStructure<SubMesh>());
        }

        //      UINT - IB Count
        //      { [IB Count]
        //          UINT - Number of USHORTs in IB
        //          USHORT[] - Array of indices
        //      }

        // load triangle indices
        var indexBufferCount = (int)reader.ReadUInt32();
        var indices = new List<ushort[]>(indexBufferCount);
        for (var i = 0; i < indexBufferCount; i++)
        {
            indices.Add(reader.ReadUInt16((int)reader.ReadUInt32()));
        }

        //      UINT - VB Count
        //      { [VB Count]
        //          UINT - Number of verts in VB
        //          Vertex[] - Array of vertices
        //      }

        // load vertex positions
        var vertexBufferCount = (int)reader.ReadUInt32();
        var vertexBuffers = new List<Vertex[]>(vertexBufferCount);
        for (var i = 0; i < vertexBufferCount; i++)
        {
            vertexBuffers.Add(reader.ReadStructure<Vertex>((int)reader.ReadUInt32()));
        }
        //      UINT - Skinning VB Count
        //      { [Skinning VB Count]
        //          UINT - Number of verts in Skinning VB
        //          SkinningVertex[] - Array of skinning verts
        //      }

        // load vertex skinning parameters
        var skinningVertexBufferCount = (int)reader.ReadUInt32();
        var skinningVertexBuffers = new List<SkinningVertex[]>(skinningVertexBufferCount);
        for (var i = 0; i < skinningVertexBufferCount; i++)
        {
            skinningVertexBuffers.Add(reader.ReadStructure<SkinningVertex>((int)reader.ReadUInt32()));
        }
        // load mesh extent
        var extent = reader.ReadStructure<MeshExtent>();
        var animationHierarchy = new AnimationHierarchy();
        IList<string>? boneNames = null;
        if (isAnimationData)
        {
            //      UINT - Bone count
            //      { [Bone count]
            //          UINT - Length of bone name
            //          wchar_t[] - Bone name (if length > 0)
            //          Bone structure
            //      }
            var boneCount = (int)reader.ReadUInt32();
            boneNames = new string[boneCount];
            for (var i = 0; i < boneCount; i++)
            {
                boneNames[i] = reader.ReadCMO_wchar();
                animationHierarchy.Bones.Add(reader.ReadStructure<BoneStruct>());
            }

            //      UINT - Animation clip count
            //      { [Animation clip count]
            //          UINT - Length of clip name
            //          wchar_t[] - Clip name (if length > 0)
            //          float - Start time
            //          float - End time
            //          UINT - Keyframe count
            //          { [Keyframe count]
            //              Keyframe structure
            //          }
            //      }
            var animationCount = (int)reader.ReadUInt32();
            for (var i = 0; i < animationCount; i++)
            {
                var animation = new Animation(AnimationType.Keyframe);
                var animationName = reader.ReadCMO_wchar();
                animation.StartTime = reader.ReadSingle();
                animation.EndTime = reader.ReadSingle();
                animation.Name = animationName;
                var keyframeCount = (int)reader.ReadUInt32();
                for (var j = 0; j < keyframeCount; j++)
                {
                    var keyframe = reader.ReadStructure<KeyframeCMO>();
                    Matrix4x4.Decompose(keyframe.Transform, out var s, out var q, out var t);
                    animation.Keyframes.Add(new Keyframe() { Translation = t, Rotation = q, Scale = s, Time = keyframe.Time });
                }
                animationHierarchy.Animations.Add(animation.Name, animation);
                if (!UniqueAnimations.ContainsKey(animation.Name))
                {
                    UniqueAnimations.Add(animation.Name, new List<Guid>());
                }
                UniqueAnimations[animation.Name].Add(animation.GUID);
            }
        }
        var obj3Ds = new List<Object3D>(subMeshCount);

        for (var i = 0; i < subMesh.Count; ++i)
        {
            var sub = subMesh[i];
            var material = materials.Count == 0 ? new PhongMaterial() : materials[(int)sub.MaterialIndex].Item1;
            var vertexCollection = new Vector3Collection(vertexBuffers[(int)sub.VertexDataIndex].Select(x => x.Position));
            var normal = new Vector3Collection(vertexBuffers[(int)sub.VertexDataIndex].Select(x => x.Normal));
            var tex = new Vector2Collection(vertexBuffers[(int)sub.VertexDataIndex].Select(x => x.UV));
            var tangent = new Vector3Collection(vertexBuffers[(int)sub.VertexDataIndex].Select(x => x.Tangent.ToHomogeneousVector3()));
            var biTangent = new Vector3Collection(normal.Zip(tangent, (x, y) => { return Vector3.Cross(x, y); }));
            var indexCollection = new IntCollection(indices[(int)sub.IndexDataIndex].Select(x => (int)x));
            var meshGeo = new MeshGeometry3D()
            {
                Positions = vertexCollection,
                Indices = indexCollection,
                Normals = normal,
                Tangents = tangent,
                BiTangents = biTangent,
                TextureCoordinates = tex
            };
            if (isAnimationData)
            {
                var boneskinmesh = new BoneSkinnedMeshGeometry3D(meshGeo)
                {
                    VertexBoneIds = new List<BoneIds>(skinningVertexBuffers[(int)sub.VertexDataIndex]
                    .Select(x => new BoneIds()
                    {
                        Bone1 = (int)x.BoneIndex0,
                        Bone2 = (int)x.BoneIndex1,
                        Bone3 = (int)x.BoneIndex2,
                        Bone4 = (int)x.BoneIndex3,
                        Weights = new Vector4(x.BoneWeight0, x.BoneWeight1, x.BoneWeight2, x.BoneWeight3),
                    }))
                };
                meshGeo = boneskinmesh;
            }
            //Todo Load textures
            obj3Ds.Add(new Object3D() { Geometry = meshGeo, Material = material, Name = name });
            animationHierarchy.Meshes.Add(obj3Ds.Last());
        }
        AnimationHierarchy.Add(animationHierarchy);
        return obj3Ds;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct SubMesh
    {
        public uint MaterialIndex;
        public uint IndexDataIndex;
        public uint VertexDataIndex;
        public uint StartIndex;
        public uint PrimCount;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector4 Tangent;
        public Color Color;
        public Vector2 UV;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct SkinningVertex
    {
        public uint BoneIndex0;
        public uint BoneIndex1;
        public uint BoneIndex2;
        public uint BoneIndex3;
        public float BoneWeight0;
        public float BoneWeight1;
        public float BoneWeight2;
        public float BoneWeight3;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MeshExtent
    {
        public Vector3 Center;
        public float Radius;

        public Vector3 Min;
        public Vector3 Max;
    };
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct KeyframeCMO
    {
        public int BoneIndex;// Used only for array based bones
        public float Time;
        public Matrix Transform;
    };
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BoneStruct
    {
        public int ParentIndex;// Used only for array based bones
        public Matrix InvBindPose;
        public Matrix BindPose;
        public Matrix BoneLocalTransform;

        public static implicit operator Bone(BoneStruct bone)
        {
            return new Bone()
            {
                ParentIndex = bone.ParentIndex,
                BindPose = bone.BindPose,
                InvBindPose = bone.InvBindPose,
                BoneLocalTransform = bone.BoneLocalTransform
            };
        }
    };
}
