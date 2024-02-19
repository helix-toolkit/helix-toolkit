/*
The MIT License (MIT)
Copyright (c) 2022 Helix Toolkit contributors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
 
The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.
 
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.

Original code from:
SharpDX project. https://github.com/sharpdx/SharpDX
SlimMath project. http://code.google.com/p/slimmath/

Copyright (c) 2010-2014 SharpDX - Alexandre Mutel
The MIT License (MIT)
Copyright (c) 2007-2011 SlimDX Group
The MIT License (MIT)
*/
namespace HelixToolkit.Maths
{
    /// <summary>
    /// Defines a frustum which can be used in frustum culling, zoom to Extents (zoom to fit) operations, 
    /// (matrix, frustum, camera) interchange, and many kind of intersection testing.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct BoundingFrustum : IEquatable<BoundingFrustum>
    {
        private Matrix pMatrix_;
        private Plane pNear_;
        private Plane pFar_;
        private Plane pLeft_;
        private Plane pRight_;
        private Plane pTop_;
        private Plane pBottom_;

        /// <summary>
        /// Gets or sets the Matrix that describes this bounding frustum.
        /// </summary>
        public Matrix Matrix
        {
            readonly get
            {
                return pMatrix_;
            }
            set
            {
                pMatrix_ = value;
                GetPlanesFromMatrix(ref pMatrix_, out pNear_, out pFar_, out pLeft_, out pRight_, out pTop_, out pBottom_);
            }
        }
        /// <summary>
        /// Gets the near plane of the BoundingFrustum.
        /// </summary>
        public readonly Plane Near
        {
            get
            {
                return pNear_;
            }
        }
        /// <summary>
        /// Gets the far plane of the BoundingFrustum.
        /// </summary>
        public readonly Plane Far
        {
            get
            {
                return pFar_;
            }
        }
        /// <summary>
        /// Gets the left plane of the BoundingFrustum.
        /// </summary>
        public readonly Plane Left
        {
            get
            {
                return pLeft_;
            }
        }
        /// <summary>
        /// Gets the right plane of the BoundingFrustum.
        /// </summary>
        public readonly Plane Right
        {
            get
            {
                return pRight_;
            }
        }
        /// <summary>
        /// Gets the top plane of the BoundingFrustum.
        /// </summary>
        public readonly Plane Top
        {
            get
            {
                return pTop_;
            }
        }
        /// <summary>
        /// Gets the bottom plane of the BoundingFrustum.
        /// </summary>
        public readonly Plane Bottom
        {
            get
            {
                return pBottom_;
            }
        }

        /// <summary>
        /// Creates a new instance of BoundingFrustum.
        /// </summary>
        /// <param name="matrix">Combined matrix that usually takes view × projection matrix.</param>
        public BoundingFrustum(Matrix matrix)
        {
            pMatrix_ = matrix;
            GetPlanesFromMatrix(ref pMatrix_, out pNear_, out pFar_, out pLeft_, out pRight_, out pTop_, out pBottom_);
        }

        public override readonly int GetHashCode()
        {
            return pMatrix_.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="BoundingFrustum"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="BoundingFrustum"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="BoundingFrustum"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // MethodImplOptions.AggressiveInlining
        public readonly bool Equals(ref BoundingFrustum other)
        {
            return this.pMatrix_ == other.pMatrix_;
        }

        /// <summary>
        /// Determines whether the specified <see cref="BoundingFrustum"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="BoundingFrustum"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="BoundingFrustum"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // MethodImplOptions.AggressiveInlining
        public readonly bool Equals(BoundingFrustum other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override readonly bool Equals(object? obj)
        {
            return obj is BoundingFrustum f && Equals(ref f);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // MethodImplOptions.AggressiveInlining
        public static bool operator ==(BoundingFrustum left, BoundingFrustum right)
        {
            return left.Equals(ref right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // MethodImplOptions.AggressiveInlining
        public static bool operator !=(BoundingFrustum left, BoundingFrustum right)
        {
            return !left.Equals(ref right);
        }

        /// <summary>
        /// Returns one of the 6 planes related to this frustum.
        /// </summary>
        /// <param name="index">Plane index where 0 fro Left, 1 for Right, 2 for Top, 3 for Bottom, 4 for Near, 5 for Far</param>
        /// <returns></returns>
        public readonly Plane GetPlane(int index)
        {
            return index switch
            {
                0 => pLeft_,
                1 => pRight_,
                2 => pTop_,
                3 => pBottom_,
                4 => pNear_,
                5 => pFar_,
                _ => new Plane(),
            };
        }

        private static void GetPlanesFromMatrix(ref Matrix matrix, out Plane near, out Plane far, out Plane left, out Plane right, out Plane top, out Plane bottom)
        {
            //http://www.chadvernon.com/blog/resources/directx9/frustum-culling/

            // Left plane
            left.Normal.X = matrix.M14 + matrix.M11;
            left.Normal.Y = matrix.M24 + matrix.M21;
            left.Normal.Z = matrix.M34 + matrix.M31;
            left.D = matrix.M44 + matrix.M41;
            left = Plane.Normalize(left);

            // Right plane
            right.Normal.X = matrix.M14 - matrix.M11;
            right.Normal.Y = matrix.M24 - matrix.M21;
            right.Normal.Z = matrix.M34 - matrix.M31;
            right.D = matrix.M44 - matrix.M41;
            right = Plane.Normalize(right);

            // Top plane
            top.Normal.X = matrix.M14 - matrix.M12;
            top.Normal.Y = matrix.M24 - matrix.M22;
            top.Normal.Z = matrix.M34 - matrix.M32;
            top.D = matrix.M44 - matrix.M42;
            top = Plane.Normalize(top);

            // Bottom plane
            bottom.Normal.X = matrix.M14 + matrix.M12;
            bottom.Normal.Y = matrix.M24 + matrix.M22;
            bottom.Normal.Z = matrix.M34 + matrix.M32;
            bottom.D = matrix.M44 + matrix.M42;
            bottom = Plane.Normalize(bottom);

            // Near plane
            near.Normal.X = matrix.M13;
            near.Normal.Y = matrix.M23;
            near.Normal.Z = matrix.M33;
            near.D = matrix.M43;
            near = Plane.Normalize(near);

            // Far plane
            far.Normal.X = matrix.M14 - matrix.M13;
            far.Normal.Y = matrix.M24 - matrix.M23;
            far.Normal.Z = matrix.M34 - matrix.M33;
            far.D = matrix.M44 - matrix.M43;
            far = Plane.Normalize(far);
        }

        private static Vector3 Get3PlanesInterPoint(ref Plane p1, ref Plane p2, ref Plane p3)
        {
            //P = -d1 * N2xN3 / N1.N2xN3 - d2 * N3xN1 / N2.N3xN1 - d3 * N1xN2 / N3.N1xN2 
            Vector3 v =
                -p1.D * Vector3.Cross(p2.Normal, p3.Normal) / Vector3.Dot(p1.Normal, Vector3.Cross(p2.Normal, p3.Normal))
                - p2.D * Vector3.Cross(p3.Normal, p1.Normal) / Vector3.Dot(p2.Normal, Vector3.Cross(p3.Normal, p1.Normal))
                - p3.D * Vector3.Cross(p1.Normal, p2.Normal) / Vector3.Dot(p3.Normal, Vector3.Cross(p1.Normal, p2.Normal));

            return v;
        }

        /// <summary>
        /// Creates a new frustum relaying on perspective camera parameters
        /// </summary>
        /// <param name="cameraPos">The camera pos.</param>
        /// <param name="lookDir">The look dir.</param>
        /// <param name="upDir">Up dir.</param>
        /// <param name="fov">The fov.</param>
        /// <param name="znear">The znear.</param>
        /// <param name="zfar">The zfar.</param>
        /// <param name="aspect">The aspect.</param>
        /// <returns>The bounding frustum calculated from perspective camera</returns>
        public static BoundingFrustum FromCamera(Vector3 cameraPos, Vector3 lookDir, Vector3 upDir, float fov, float znear, float zfar, float aspect)
        {
            //http://knol.google.com/k/view-frustum

            lookDir = Vector3.Normalize(lookDir);
            upDir = Vector3.Normalize(upDir);

            Vector3 nearCenter = cameraPos + lookDir * znear;
            Vector3 farCenter = cameraPos + lookDir * zfar;
            float nearHalfHeight = (float)(znear * Math.Tan(fov / 2f));
            float farHalfHeight = (float)(zfar * Math.Tan(fov / 2f));
            float nearHalfWidth = nearHalfHeight * aspect;
            float farHalfWidth = farHalfHeight * aspect;

            Vector3 rightDir = Vector3.Normalize(Vector3.Cross(upDir, lookDir));
            Vector3 Near1 = nearCenter - nearHalfHeight * upDir + nearHalfWidth * rightDir;
            Vector3 Near2 = nearCenter + nearHalfHeight * upDir + nearHalfWidth * rightDir;
            Vector3 Near3 = nearCenter + nearHalfHeight * upDir - nearHalfWidth * rightDir;
            Vector3 Near4 = nearCenter - nearHalfHeight * upDir - nearHalfWidth * rightDir;
            Vector3 Far1 = farCenter - farHalfHeight * upDir + farHalfWidth * rightDir;
            Vector3 Far2 = farCenter + farHalfHeight * upDir + farHalfWidth * rightDir;
            Vector3 Far3 = farCenter + farHalfHeight * upDir - farHalfWidth * rightDir;
            Vector3 Far4 = farCenter - farHalfHeight * upDir - farHalfWidth * rightDir;

            BoundingFrustum result = new()
            {
                pNear_ = Plane.CreateFromVertices(Near1, Near2, Near3),
                pFar_ = Plane.CreateFromVertices(Far3, Far2, Far1),
                pLeft_ = Plane.CreateFromVertices(Near4, Near3, Far3),
                pRight_ = Plane.CreateFromVertices(Far1, Far2, Near2),
                pTop_ = Plane.CreateFromVertices(Near2, Far2, Far3),
                pBottom_ = Plane.CreateFromVertices(Far4, Far1, Near1),
                pMatrix_ = MatrixHelper.LookAtLH(cameraPos, cameraPos + lookDir * 10, upDir)
                * MatrixHelper.PerspectiveFovLH(fov, aspect, znear, zfar)
            };
            //result.pNear.Normalize();
            //result.pFar.Normalize();
            //result.pLeft.Normalize();
            //result.pRight.Normalize();
            //result.pTop.Normalize();
            //result.pBottom.Normalize();                  
            return result;
        }
        /// <summary>
        /// Creates a new frustum relaying on perspective camera parameters
        /// </summary>
        /// <param name="cameraParams">The camera params.</param>
        /// <returns>The bounding frustum from camera params</returns>
        public static BoundingFrustum FromCamera(FrustumCameraParams cameraParams)
        {
            return FromCamera(cameraParams.Position, cameraParams.LookAtDir, cameraParams.UpDir, cameraParams.FOV, cameraParams.ZNear, cameraParams.ZFar, cameraParams.AspectRatio);
        }

        /// <summary>
        /// Returns the 8 corners of the frustum, element0 is Near1 (near right down corner)
        /// , element1 is Near2 (near right top corner)
        /// , element2 is Near3 (near Left top corner)
        /// , element3 is Near4 (near Left down corner)
        /// , element4 is Far1 (far right down corner)
        /// , element5 is Far2 (far right top corner)
        /// , element6 is Far3 (far left top corner)
        /// , element7 is Far4 (far left down corner)
        /// </summary>
        /// <returns>The 8 corners of the frustum</returns>
        public Vector3[] GetCorners()
        {
            Vector3[] corners = new Vector3[8];
            GetCorners(corners);
            return corners;
        }

        /// <summary>
        /// Returns the 8 corners of the frustum, element0 is Near1 (near right down corner)
        /// , element1 is Near2 (near right top corner)
        /// , element2 is Near3 (near Left top corner)
        /// , element3 is Near4 (near Left down corner)
        /// , element4 is Far1 (far right down corner)
        /// , element5 is Far2 (far right top corner)
        /// , element6 is Far3 (far left top corner)
        /// , element7 is Far4 (far left down corner)
        /// </summary>
        /// <returns>The 8 corners of the frustum</returns>
        public void GetCorners(Vector3[] corners)
        {
            corners[0] = Get3PlanesInterPoint(ref pNear_, ref pBottom_, ref pRight_);    //Near1
            corners[1] = Get3PlanesInterPoint(ref pNear_, ref pTop_, ref pRight_);       //Near2
            corners[2] = Get3PlanesInterPoint(ref pNear_, ref pTop_, ref pLeft_);        //Near3
            corners[3] = Get3PlanesInterPoint(ref pNear_, ref pBottom_, ref pLeft_);     //Near3
            corners[4] = Get3PlanesInterPoint(ref pFar_, ref pBottom_, ref pRight_);    //Far1
            corners[5] = Get3PlanesInterPoint(ref pFar_, ref pTop_, ref pRight_);       //Far2
            corners[6] = Get3PlanesInterPoint(ref pFar_, ref pTop_, ref pLeft_);        //Far3
            corners[7] = Get3PlanesInterPoint(ref pFar_, ref pBottom_, ref pLeft_);     //Far3
        }

        /// <summary>
        /// Extracts perspective camera parameters from the frustum, doesn't work with orthographic frustums.
        /// </summary>
        /// <returns>Perspective camera parameters from the frustum</returns>
        public FrustumCameraParams GetCameraParams()
        {
            Vector3[] corners = GetCorners();
            FrustumCameraParams cameraParam = new()
            {
                Position = Get3PlanesInterPoint(ref pRight_, ref pTop_, ref pLeft_),
                LookAtDir = pNear_.Normal,
                UpDir = Vector3.Normalize(Vector3.Cross(pRight_.Normal, pNear_.Normal)),
                FOV = (float)((Math.PI / 2.0 - Math.Acos(Vector3.Dot(pNear_.Normal, pTop_.Normal))) * 2),
                AspectRatio = (corners[6] - corners[5]).Length() / (corners[4] - corners[5]).Length()
            };
            cameraParam.ZNear = (cameraParam.Position + (pNear_.Normal * pNear_.D)).Length();
            cameraParam.ZFar = (cameraParam.Position + (pFar_.Normal * pFar_.D)).Length();
            return cameraParam;
        }

        /// <summary>
        /// Checks whether a point lay inside, intersects or lay outside the frustum.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>Type of the containment</returns>
        public readonly ContainmentType Contains(ref Vector3 point)
        {
            PlaneIntersectionType result = PlaneIntersectionType.Front;
            PlaneIntersectionType planeResult = PlaneIntersectionType.Front;
            for (int i = 0; i < 6; i++)
            {
                switch (i)
                {
                    case 0: planeResult = pNear_.Intersects(ref point); break;
                    case 1: planeResult = pFar_.Intersects(ref point); break;
                    case 2: planeResult = pLeft_.Intersects(ref point); break;
                    case 3: planeResult = pRight_.Intersects(ref point); break;
                    case 4: planeResult = pTop_.Intersects(ref point); break;
                    case 5: planeResult = pBottom_.Intersects(ref point); break;
                }
                switch (planeResult)
                {
                    case PlaneIntersectionType.Back:
                        return ContainmentType.Disjoint;
                    case PlaneIntersectionType.Intersecting:
                        result = PlaneIntersectionType.Intersecting;
                        break;
                }
            }
            return result switch
            {
                PlaneIntersectionType.Intersecting => ContainmentType.Intersects,
                _ => ContainmentType.Contains,
            };
        }

        /// <summary>
        /// Checks whether a point lay inside, intersects or lay outside the frustum.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>Type of the containment</returns>
        public readonly ContainmentType Contains(Vector3 point)
        {
            return Contains(ref point);
        }

        private static void GetBoxToPlanePVertexNVertex(ref BoundingBox box, ref Vector3 planeNormal, out Vector3 p, out Vector3 n)
        {
            p = box.Minimum;
            if (planeNormal.X >= 0)
            {
                p.X = box.Maximum.X;
            }

            if (planeNormal.Y >= 0)
            {
                p.Y = box.Maximum.Y;
            }

            if (planeNormal.Z >= 0)
            {
                p.Z = box.Maximum.Z;
            }

            n = box.Maximum;
            if (planeNormal.X >= 0)
            {
                n.X = box.Minimum.X;
            }

            if (planeNormal.Y >= 0)
            {
                n.Y = box.Minimum.Y;
            }

            if (planeNormal.Z >= 0)
            {
                n.Z = box.Minimum.Z;
            }
        }

        /// <summary>
        /// Determines the intersection relationship between the frustum and a bounding box.
        /// </summary>
        /// <param name="box">The box.</param>
        /// <returns>Type of the containment</returns>
        public readonly ContainmentType Contains(ref BoundingBox box)
        {
            Plane plane;
            ContainmentType result = ContainmentType.Contains;
            for (int i = 0; i < 6; i++)
            {
                plane = GetPlane(i);
                BoundingFrustum.GetBoxToPlanePVertexNVertex(ref box, ref plane.Normal, out Vector3 p, out Vector3 n);
                if (Collision.PlaneIntersectsPoint(ref plane, ref p) == PlaneIntersectionType.Back)
                {
                    return ContainmentType.Disjoint;
                }

                if (Collision.PlaneIntersectsPoint(ref plane, ref n) == PlaneIntersectionType.Back)
                {
                    result = ContainmentType.Intersects;
                }
            }
            return result;
        }

        /// <summary>
        /// Determines the intersection relationship between the frustum and a bounding box.
        /// </summary>
        /// <param name="box">The box.</param>
        /// <returns>Type of the containment</returns>
        public readonly ContainmentType Contains(BoundingBox box)
        {
            return Contains(ref box);
        }

        /// <summary>
        /// Determines the intersection relationship between the frustum and a bounding box.
        /// </summary>
        /// <param name="box">The box.</param>
        /// <param name="result">Type of the containment.</param>
        public readonly void Contains(ref BoundingBox box, out ContainmentType result)
        {
            result = Contains(ref box);
        }
        /// <summary>
        /// Determines the intersection relationship between the frustum and a bounding sphere.
        /// </summary>
        /// <param name="sphere">The sphere.</param>
        /// <returns>Type of the containment</returns>
        public readonly ContainmentType Contains(ref BoundingSphere sphere)
        {
            PlaneIntersectionType result = PlaneIntersectionType.Front;
            PlaneIntersectionType planeResult = PlaneIntersectionType.Front;
            for (int i = 0; i < 6; i++)
            {
                switch (i)
                {
                    case 0: planeResult = pNear_.Intersects(ref sphere); break;
                    case 1: planeResult = pFar_.Intersects(ref sphere); break;
                    case 2: planeResult = pLeft_.Intersects(ref sphere); break;
                    case 3: planeResult = pRight_.Intersects(ref sphere); break;
                    case 4: planeResult = pTop_.Intersects(ref sphere); break;
                    case 5: planeResult = pBottom_.Intersects(ref sphere); break;
                }
                switch (planeResult)
                {
                    case PlaneIntersectionType.Back:
                        return ContainmentType.Disjoint;
                    case PlaneIntersectionType.Intersecting:
                        result = PlaneIntersectionType.Intersecting;
                        break;
                }
            }
            return result switch
            {
                PlaneIntersectionType.Intersecting => ContainmentType.Intersects,
                _ => ContainmentType.Contains,
            };
        }

        /// <summary>
        /// Determines the intersection relationship between the frustum and a bounding sphere.
        /// </summary>
        /// <param name="sphere">The sphere.</param>
        /// <returns>Type of the containment</returns>
        public readonly ContainmentType Contains(BoundingSphere sphere)
        {
            return Contains(ref sphere);
        }

        /// <summary>
        /// Determines the intersection relationship between the frustum and a bounding sphere.
        /// </summary>
        /// <param name="sphere">The sphere.</param>
        /// <param name="result">Type of the containment.</param>
        public readonly void Contains(ref BoundingSphere sphere, out ContainmentType result)
        {
            result = Contains(ref sphere);
        }

        /// <summary>
        /// Checks whether the current BoundingFrustum intersects a BoundingSphere.
        /// </summary>
        /// <param name="sphere">The sphere.</param>
        /// <returns>Type of the containment</returns>
        public readonly bool Intersects(ref BoundingSphere sphere)
        {
            return Contains(ref sphere) != ContainmentType.Disjoint;
        }
        /// <summary>
        /// Checks whether the current BoundingFrustum intersects a BoundingSphere.
        /// </summary>
        /// <param name="sphere">The sphere.</param>
        /// <param name="result">Set to <c>true</c> if the current BoundingFrustum intersects a BoundingSphere.</param>
        public readonly void Intersects(ref BoundingSphere sphere, out bool result)
        {
            result = Contains(ref sphere) != ContainmentType.Disjoint;
        }
        /// <summary>
        /// Checks whether the current BoundingFrustum intersects a BoundingBox.
        /// </summary>
        /// <param name="box">The box.</param>
        /// <returns><c>true</c> if the current BoundingFrustum intersects a BoundingSphere.</returns>
        public readonly bool Intersects(ref BoundingBox box)
        {
            return Contains(ref box) != ContainmentType.Disjoint;
        }
        /// <summary>
        /// Checks whether the current BoundingFrustum intersects a BoundingBox.
        /// </summary>
        /// <param name="box">The box.</param>
        /// <param name="result"><c>true</c> if the current BoundingFrustum intersects a BoundingSphere.</param>
        public readonly void Intersects(ref BoundingBox box, out bool result)
        {
            result = Contains(ref box) != ContainmentType.Disjoint;
        }

        private static PlaneIntersectionType PlaneIntersectsPoints(ref Plane plane, Vector3[] points)
        {
            PlaneIntersectionType result = Collision.PlaneIntersectsPoint(ref plane, ref points[0]);
            for (int i = 1; i < points.Length; i++)
            {
                if (Collision.PlaneIntersectsPoint(ref plane, ref points[i]) != result)
                {
                    return PlaneIntersectionType.Intersecting;
                }
            }

            return result;
        }

        /// <summary>
        /// Checks whether the current BoundingFrustum intersects the specified Plane.
        /// </summary>
        /// <param name="plane">The plane.</param>
        /// <returns>Plane intersection type.</returns>
        public PlaneIntersectionType Intersects(ref Plane plane)
        {
            return BoundingFrustum.PlaneIntersectsPoints(ref plane, GetCorners());
        }
        /// <summary>
        /// Checks whether the current BoundingFrustum intersects the specified Plane.
        /// </summary>
        /// <param name="plane">The plane.</param>
        /// <param name="result">Plane intersection type.</param>
        public void Intersects(ref Plane plane, out PlaneIntersectionType result)
        {
            result = BoundingFrustum.PlaneIntersectsPoints(ref plane, GetCorners());
        }

        /// <summary>
        /// Get the width of the frustum at specified depth.
        /// </summary>
        /// <param name="depth">the depth at which to calculate frustum width.</param>
        /// <returns>With of the frustum at the specified depth</returns>
        public readonly float GetWidthAtDepth(float depth)
        {
            float hAngle = (float)(Math.PI / 2.0 - Math.Acos(Vector3.Dot(pNear_.Normal, pLeft_.Normal)));
            return (float)(Math.Tan(hAngle) * depth * 2);
        }

        /// <summary>
        /// Get the height of the frustum at specified depth.
        /// </summary>
        /// <param name="depth">the depth at which to calculate frustum height.</param>
        /// <returns>Height of the frustum at the specified depth</returns>
        public readonly float GetHeightAtDepth(float depth)
        {
            float vAngle = (float)(Math.PI / 2.0 - Math.Acos(Vector3.Dot(pNear_.Normal, pTop_.Normal)));
            return (float)(Math.Tan(vAngle) * depth * 2);
        }

        private readonly BoundingFrustum GetInsideOutClone()
        {
            BoundingFrustum frustum = this;
            frustum.pNear_.Normal = -frustum.pNear_.Normal;
            frustum.pFar_.Normal = -frustum.pFar_.Normal;
            frustum.pLeft_.Normal = -frustum.pLeft_.Normal;
            frustum.pRight_.Normal = -frustum.pRight_.Normal;
            frustum.pTop_.Normal = -frustum.pTop_.Normal;
            frustum.pBottom_.Normal = -frustum.pBottom_.Normal;
            return frustum;
        }

        /// <summary>
        /// Checks whether the current BoundingFrustum intersects the specified Ray.
        /// </summary>
        /// <param name="ray">The ray.</param>
        /// <returns><c>true</c> if the current BoundingFrustum intersects the specified Ray.</returns>
        public readonly bool Intersects(ref Ray ray)
        {
            return Intersects(ref ray, out float? _, out float? _);
        }
        /// <summary>
        /// Checks whether the current BoundingFrustum intersects the specified Ray.
        /// </summary>
        /// <param name="ray">The Ray to check for intersection with.</param>
        /// <param name="inDistance">The distance at which the ray enters the frustum if there is an intersection and the ray starts outside the frustum.</param>
        /// <param name="outDistance">The distance at which the ray exits the frustum if there is an intersection.</param>
        /// <returns><c>true</c> if the current BoundingFrustum intersects the specified Ray.</returns>
        public readonly bool Intersects(ref Ray ray, out float? inDistance, out float? outDistance)
        {
            if (Contains(ray.Position) != ContainmentType.Disjoint)
            {
                float nearstPlaneDistance = float.MaxValue;
                for (int i = 0; i < 6; i++)
                {
                    Plane plane = GetPlane(i);
                    if (Collision.RayIntersectsPlane(ref ray, ref plane, out float distance) && distance < nearstPlaneDistance)
                    {
                        nearstPlaneDistance = distance;
                    }
                }

                inDistance = nearstPlaneDistance;
                outDistance = null;
                return true;
            }
            else
            {
                //We will find the two points at which the ray enters and exists the frustum
                //These two points make a line which center inside the frustum if the ray intersects it
                //Or outside the frustum if the ray intersects frustum planes outside it.
                float minDist = float.MaxValue;
                float maxDist = float.MinValue;
                for (int i = 0; i < 6; i++)
                {
                    Plane plane = GetPlane(i);
                    if (Collision.RayIntersectsPlane(ref ray, ref plane, out float distance))
                    {
                        minDist = Math.Min(minDist, distance);
                        maxDist = Math.Max(maxDist, distance);
                    }
                }

                Vector3 minPoint = ray.Position + ray.Direction * minDist;
                Vector3 maxPoint = ray.Position + ray.Direction * maxDist;
                Vector3 center = (minPoint + maxPoint) / 2f;
                if (Contains(ref center) != ContainmentType.Disjoint)
                {
                    inDistance = minDist;
                    outDistance = maxDist;
                    return true;
                }
                else
                {
                    inDistance = null;
                    outDistance = null;
                    return false;
                }
            }
        }

        /// <summary>
        /// Get the distance which when added to camera position along the lookat direction will do the effect of zoom to extents (zoom to fit) operation,
        /// so all the passed points will fit in the current view.
        /// if the returned value is positive, the camera will move toward the lookat direction (ZoomIn).
        /// if the returned value is negative, the camera will move in the reverse direction of the lookat direction (ZoomOut).
        /// </summary>
        /// <param name="points">The points.</param>
        /// <returns>The zoom to fit distance</returns>
        public readonly float GetZoomToExtentsShiftDistance(Vector3[] points)
        {
            float vAngle = (float)(Math.PI / 2.0 - Math.Acos(Vector3.Dot(pNear_.Normal, pTop_.Normal)));
            float vSin = (float)Math.Sin(vAngle);
            float hAngle = (float)(Math.PI / 2.0 - Math.Acos(Vector3.Dot(pNear_.Normal, pLeft_.Normal)));
            float hSin = (float)Math.Sin(hAngle);
            float horizontalToVerticalMapping = vSin / hSin;

            BoundingFrustum ioFrustrum = GetInsideOutClone();

            float maxPointDist = float.MinValue;
            for (int i = 0; i < points.Length; i++)
            {
                float pointDist = Collision.DistancePlanePoint(ref ioFrustrum.pTop_, ref points[i]);
                pointDist = Math.Max(pointDist, Collision.DistancePlanePoint(ref ioFrustrum.pBottom_, ref points[i]));
                pointDist = Math.Max(pointDist, Collision.DistancePlanePoint(ref ioFrustrum.pLeft_, ref points[i]) * horizontalToVerticalMapping);
                pointDist = Math.Max(pointDist, Collision.DistancePlanePoint(ref ioFrustrum.pRight_, ref points[i]) * horizontalToVerticalMapping);

                maxPointDist = Math.Max(maxPointDist, pointDist);
            }
            return -maxPointDist / vSin;
        }

        /// <summary>
        /// Get the distance which when added to camera position along the lookat direction will do the effect of zoom to extents (zoom to fit) operation,
        /// so all the passed points will fit in the current view.
        /// if the returned value is positive, the camera will move toward the lookat direction (ZoomIn).
        /// if the returned value is negative, the camera will move in the reverse direction of the lookat direction (ZoomOut).
        /// </summary>
        /// <param name="boundingBox">The bounding box.</param>
        /// <returns>The zoom to fit distance</returns>
        public readonly float GetZoomToExtentsShiftDistance(ref BoundingBox boundingBox)
        {
            return GetZoomToExtentsShiftDistance(boundingBox.GetCorners());
        }

        /// <summary>
        /// Get the vector shift which when added to camera position will do the effect of zoom to extents (zoom to fit) operation,
        /// so all the passed points will fit in the current view.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <returns>The zoom to fit vector</returns>
        public readonly Vector3 GetZoomToExtentsShiftVector(Vector3[] points)
        {
            return GetZoomToExtentsShiftDistance(points) * pNear_.Normal;
        }
        /// <summary>
        /// Get the vector shift which when added to camera position will do the effect of zoom to extents (zoom to fit) operation,
        /// so all the passed points will fit in the current view.
        /// </summary>
        /// <param name="boundingBox">The bounding box.</param>
        /// <returns>The zoom to fit vector</returns>
        public readonly Vector3 GetZoomToExtentsShiftVector(ref BoundingBox boundingBox)
        {
            return GetZoomToExtentsShiftDistance(boundingBox.GetCorners()) * pNear_.Normal;
        }

        /// <summary>
        /// Indicate whether the current BoundingFrustrum is Orthographic.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the current BoundingFrustrum is Orthographic; otherwise, <c>false</c>.
        /// </value>
        public readonly bool IsOrthographic
        {
            get
            {
                return (pLeft_.Normal == -pRight_.Normal) && (pTop_.Normal == -pBottom_.Normal);
            }
        }
    }
}
