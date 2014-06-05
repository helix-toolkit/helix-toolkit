namespace HelixToolkit.Wpf.SharpDX.Core
{
    using System;
    using System.Collections.Generic;

    using global::SharpDX;

    /// <summary>
    /// Struct for a 3dimensional Box
    /// </summary>
    [Obsolete("We do not need own structures, since SharpDX does it all for us.")]
    public struct Box3
    {
        /// <summary>
        /// 3D Vectors defininge the Box
        /// </summary>
        public Vector3 Min, Max;
        /// <summary>
        /// Constructor for the Box
        /// </summary>
        /// <param name="min">Minimum Box-Values</param>
        /// <param name="max">Maximum Box-Values</param>
        public Box3(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Calculates size of the box.
        /// </summary>
        public Vector3 Size
        {
            get { return Max - Min; }
            set { Max = Min + value; }
        }

        public Vector3 Center
        {
            get { return ((Min + Max) / 2); }
        }

        public float SizeX
        {
            get { return Max.X - Min.X; }
            set { Max.X = Min.X + value; }
        }

        public float SizeY
        {
            get { return Max.Y - Min.Y; }
            set { Max.Y = Min.Y + value; }
        }

        public float SizeZ
        {
            get { return Max.Z - Min.Z; }
            set { Max.Z = Min.Z + value; }
        }

        /// <summary>
        /// Returns true if the box contains the given point.
        /// </summary>
        public bool Contains(Vector3 p)
        {
            return
                p.X >= Min.X && p.X <= Max.X &&
                p.Y >= Min.Y && p.Y <= Max.Y &&
                p.Z >= Min.Z && p.Z <= Max.Z;
        }

        /// <summary>
        /// Returns true if the box completely contains the given box.
        /// </summary>
        public bool Contains(Box3 b)
        {
            return
                b.Min.X >= Min.X && b.Max.X <= Max.X &&
                b.Min.Y >= Min.Y && b.Max.Y <= Max.Y &&
                b.Min.Z >= Min.Z && b.Max.Z <= Max.Z;
        }

        /// <summary>
        /// Extends the box to contain the supplied box.
        /// </summary>
        public Box3 ExtendBy(Box3 box)
        {
            if (box.Min.X < Min.X) Min.X = box.Min.X;
            if (box.Max.X > Max.X) Max.X = box.Max.X;
            if (box.Min.Y < Min.Y) Min.Y = box.Min.Y;
            if (box.Max.Y > Max.Y) Max.Y = box.Max.Y;
            if (box.Min.Z < Min.Z) Min.Z = box.Min.Z;
            if (box.Max.Z > Max.Z) Max.Z = box.Max.Z;
            return this;
        }

        /// <summary>
        /// Extends the box to contain the supplied value.
        /// </summary>
        public Box3 ExtendBy(Vector3 point)
        {
            if (point.X < Min.X) Min.X = point.X;
            if (point.X > Max.X) Max.X = point.X;
            if (point.Y < Min.Y) Min.Y = point.Y;
            if (point.Y > Max.Y) Max.Y = point.Y;
            if (point.Z < Min.Z) Min.Z = point.Z;
            if (point.Z > Max.Z) Max.Z = point.Z;
            return this;
        }
        /// <summary>
        /// Extends the box to contain the supplied value.
        /// </summary>
        public Box3 ExtendXBy(float x)
        {
            if (x < Min.X) Min.X = x;
            if (x > Max.X) Max.X = x;
            return this;
        }
        /// <summary>
        /// Extends the box to contain the supplied value.
        /// </summary>
        public Box3 ExtendYBy(float y)
        {
            if (y < Min.Y) Min.Y = y;
            if (y > Max.Y) Max.Y = y;
            return this;
        }
        /// <summary>
        /// Extends the box to contain the supplied value.
        /// </summary>
        public Box3 ExtendZBy(float z)
        {
            if (z < Min.Z) Min.Z = z;
            if (z > Max.Z) Max.Z = z;
            return this;
        }

        /// <summary>
        /// Returns true if 2 boxes intersect each other.
        /// </summary>
        public bool Intersects(Box3 box)
        {
            if (Min.X > box.Max.X) return false;
            if (Max.X < box.Min.X) return false;
            if (Min.Y > box.Max.Y) return false;
            if (Max.Y < box.Min.Y) return false;
            if (Min.Z > box.Max.Z) return false;
            if (Max.Z < box.Min.Z) return false;
            return true;
        }

        /// <summary>
        /// Returns true if 2 boxes intersect each other with tolerance parameter.
        /// </summary>
        public bool Intersects(Box3 box, Vector3 eps)
        {
            if (Min.X - eps.X > box.Max.X) return false;
            if (Max.X + eps.X < box.Min.X) return false;
            if (Min.Y - eps.Y > box.Max.Y) return false;
            if (Max.Y + eps.Y < box.Min.Y) return false;
            if (Min.Z - eps.Z > box.Max.Z) return false;
            if (Max.Z + eps.Z < box.Min.Z) return false;
            return true;
        }

        /// <summary>
        /// Returns true if 2 boxes intersect each other with tolerance parameter.
        /// </summary>
        public bool Intersects(Box3 box, float eps)
        {
            if (Min.X - eps > box.Max.X) return false;
            if (Max.X + eps < box.Min.X) return false;
            if (Min.Y - eps > box.Max.Y) return false;
            if (Max.Y + eps < box.Min.Y) return false;
            if (Min.Z - eps > box.Max.Z) return false;
            if (Max.Z + eps < box.Min.Z) return false;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public Box3 Union(Box3 b)
        {
            return Union(this, b);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public Box3 Intersection(Box3 b)
        {
            return Intersection(this, b);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Box3 Union(Box3 a, Box3 b)
        {
            return new Box3(VectorComparisonExtensions.ComponentMin(a.Min, b.Min), VectorComparisonExtensions.ComponentMax(a.Max, b.Max));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Box3 Intersection(Box3 a, Box3 b)
        {
            return new Box3(VectorComparisonExtensions.ComponentMax(a.Min, b.Min), VectorComparisonExtensions.ComponentMin(a.Max, b.Max));
        }

        /// <summary>
        /// True if the box is valid.
        /// </summary>
        public bool IsValid
        {
            //get { return Min.AllSmallerOrEqual(Max); }
            get { return Min.AllSmaller(Max); }
        }


        /// <summary>
        /// Creates box from 2 points which need not be Min and Max.
        /// </summary>
        public static Box3 FromPoints(Vector3 p0, Vector3 p1)
        {
            return new Box3(VectorComparisonExtensions.ComponentMin(p0, p1), VectorComparisonExtensions.ComponentMax(p0, p1));
        }

        /// <summary>
        /// Creates box from a set points which need not be Min and Max.
        /// </summary>
        /// <param name="points"></param>        
        public static Box3 FromPoints(IEnumerable<Vector3> points)
        {
            var box = new Box3();
            foreach (var p in points)           
                box.ExtendBy(p);
            return box;
        }
    }

    
}
