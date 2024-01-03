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
    /// Represents a unit independent angle using a single-precision floating-point
    /// internal representation.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct AngleSingle : IComparable, IComparable<AngleSingle>, IEquatable<AngleSingle>, IFormattable
    {
        /// <summary>
        /// A value that specifies the size of a single degree.
        /// </summary>
        public const float Degree = 0.002777777777777778f;

        /// <summary>
        /// A value that specifies the size of a single minute.
        /// </summary>
        public const float Minute = 0.000046296296296296f;

        /// <summary>
        /// A value that specifies the size of a single second.
        /// </summary>
        public const float Second = 0.000000771604938272f;

        /// <summary>
        /// A value that specifies the size of a single radian.
        /// </summary>
        public const float Radian = 0.159154943091895336f;

        /// <summary>
        /// A value that specifies the size of a single milliradian.
        /// </summary>
        public const float Milliradian = 0.0001591549431f;

        /// <summary>
        /// A value that specifies the size of a single gradian.
        /// </summary>
        public const float Gradian = 0.0025f;

        /// <summary>
        /// The internal representation of the angle.
        /// </summary>
        [FieldOffset(0)]
        float radians_;

        [FieldOffset(0)]
        private readonly int radiansInt_;

        /// <summary>
        /// Initializes a new instance of the SharpDX.AngleSingle structure with the
        /// given unit dependant angle and unit type.
        /// </summary>
        /// <param name="angle">A unit dependant measure of the angle.</param>
        /// <param name="type">The type of unit the angle argument is.</param>
        public AngleSingle(float angle, AngleType type)
        {
            radiansInt_ = 0;
            radians_ = type switch
            {
                AngleType.Revolution => MathUtil.RevolutionsToRadians(angle),
                AngleType.Degree => MathUtil.DegreesToRadians(angle),
                AngleType.Radian => angle,
                AngleType.Gradian => MathUtil.GradiansToRadians(angle),
                _ => 0.0f,
            };
        }

        /// <summary>
        /// Initializes a new instance of the SharpDX.AngleSingle structure using the
        /// arc length formula (θ = s/r).
        /// </summary>
        /// <param name="arcLength">The measure of the arc.</param>
        /// <param name="radius">The radius of the circle.</param>
        public AngleSingle(float arcLength, float radius)
        {
            radiansInt_ = 0;
            radians_ = arcLength / radius;
        }

        /// <summary>
        /// Wraps this SharpDX.AngleSingle to be in the range [π, -π].
        /// </summary>
        public void Wrap()
        {
            float newangle = (float)Math.IEEERemainder(radians_, MathUtil.TwoPi);

            if (newangle <= -MathUtil.Pi)
            {
                newangle += MathUtil.TwoPi;
            }
            else if (newangle > MathUtil.Pi)
            {
                newangle -= MathUtil.TwoPi;
            }

            radians_ = newangle;
        }

        /// <summary>
        /// Wraps this SharpDX.AngleSingle to be in the range [0, 2π).
        /// </summary>
        public void WrapPositive()
        {
            float newangle = radians_ % MathUtil.TwoPi;

            if (newangle < 0.0)
            {
                newangle += MathUtil.TwoPi;
            }

            radians_ = newangle;
        }

        /// <summary>
        /// Gets or sets the total number of revolutions this SharpDX.AngleSingle represents.
        /// </summary>
        public float Revolutions
        {
            readonly get { return MathUtil.RadiansToRevolutions(radians_); }
            set { radians_ = MathUtil.RevolutionsToRadians(value); }
        }

        /// <summary>
        /// Gets or sets the total number of degrees this SharpDX.AngleSingle represents.
        /// </summary>
        public float Degrees
        {
            readonly get { return MathUtil.RadiansToDegrees(radians_); }
            set { radians_ = MathUtil.DegreesToRadians(value); }
        }

        /// <summary>
        /// Gets or sets the minutes component of the degrees this SharpDX.AngleSingle represents.
        /// When setting the minutes, if the value is in the range (-60, 60) the whole degrees are
        /// not changed; otherwise, the whole degrees may be changed. Fractional values may set
        /// the seconds component.
        /// </summary>
        public float Minutes
        {
            readonly get
            {
                float degrees = MathUtil.RadiansToDegrees(radians_);

                if (degrees < 0)
                {
                    float degreesfloor = (float)Math.Ceiling(degrees);
                    return (degrees - degreesfloor) * 60.0f;
                }
                else
                {
                    float degreesfloor = (float)Math.Floor(degrees);
                    return (degrees - degreesfloor) * 60.0f;
                }
            }
            set
            {
                float degrees = MathUtil.RadiansToDegrees(radians_);
                float degreesfloor = (float)Math.Floor(degrees);

                degreesfloor += value / 60.0f;
                radians_ = MathUtil.DegreesToRadians(degreesfloor);
            }
        }

        /// <summary>
        /// Gets or sets the seconds of the degrees this SharpDX.AngleSingle represents.
        /// When setting the seconds, if the value is in the range (-60, 60) the whole minutes
        /// or whole degrees are not changed; otherwise, the whole minutes or whole degrees
        /// may be changed.
        /// </summary>
        public float Seconds
        {
            readonly get
            {
                float degrees = MathUtil.RadiansToDegrees(radians_);

                if (degrees < 0)
                {
                    float degreesfloor = (float)Math.Ceiling(degrees);

                    float minutes = (degrees - degreesfloor) * 60.0f;
                    float minutesfloor = (float)Math.Ceiling(minutes);

                    return (minutes - minutesfloor) * 60.0f;
                }
                else
                {
                    float degreesfloor = (float)Math.Floor(degrees);

                    float minutes = (degrees - degreesfloor) * 60.0f;
                    float minutesfloor = (float)Math.Floor(minutes);

                    return (minutes - minutesfloor) * 60.0f;
                }
            }
            set
            {
                float degrees = MathUtil.RadiansToDegrees(radians_);
                float degreesfloor = (float)Math.Floor(degrees);

                float minutes = (degrees - degreesfloor) * 60.0f;
                float minutesfloor = (float)Math.Floor(minutes);

                minutesfloor += value / 60.0f;
                degreesfloor += minutesfloor / 60.0f;
                radians_ = MathUtil.DegreesToRadians(degreesfloor);
            }
        }
        
        /// <summary>
        /// Gets or sets the total number of radians this SharpDX.AngleSingle represents.
        /// </summary>
        public float Radians
        {
            readonly get { return radians_; }
            set { radians_ = value; }
        }

        /// <summary>
        /// Gets or sets the total number of milliradians this SharpDX.AngleSingle represents.
        /// One milliradian is equal to 1/(2000π).
        /// </summary>
        public float Milliradians
        {
            readonly get { return radians_ / (Milliradian * MathUtil.TwoPi); }
            set { radians_ = value * (Milliradian * MathUtil.TwoPi); }
        }

        /// <summary>
        /// Gets or sets the total number of gradians this SharpDX.AngleSingle represents.
        /// </summary>
        public float Gradians
        {
            readonly get { return MathUtil.RadiansToGradians(radians_); }
            set { radians_ = MathUtil.RadiansToGradians(value); }
        }

        /// <summary>
        /// Gets a System.Boolean that determines whether this SharpDX.Angle
        /// is a right angle (i.e. 90° or π/2).
        /// </summary>
        public readonly bool IsRight
        {
            get { return radians_ == MathUtil.PiOverTwo; }
        }

        /// <summary>
        /// Gets a System.Boolean that determines whether this SharpDX.Angle
        /// is a straight angle (i.e. 180° or π).
        /// </summary>
        public readonly bool IsStraight
        {
            get { return radians_ == MathUtil.Pi; }
        }

        /// <summary>
        /// Gets a System.Boolean that determines whether this SharpDX.Angle
        /// is a full rotation angle (i.e. 360° or 2π).
        /// </summary>
        public readonly bool IsFullRotation
        {
            get { return radians_ == MathUtil.TwoPi; }
        }

        /// <summary>
        /// Gets a System.Boolean that determines whether this SharpDX.Angle
        /// is an oblique angle (i.e. is not 90° or a multiple of 90°).
        /// </summary>
        public readonly bool IsOblique
        {
            get { return WrapPositive(this).radians_ != MathUtil.PiOverTwo; }
        }

        /// <summary>
        /// Gets a System.Boolean that determines whether this SharpDX.Angle
        /// is an acute angle (i.e. less than 90° but greater than 0°).
        /// </summary>
        public readonly bool IsAcute
        {
            get { return radians_ > 0.0 && radians_ < MathUtil.PiOverTwo; }
        }

        /// <summary>
        /// Gets a System.Boolean that determines whether this SharpDX.Angle
        /// is an obtuse angle (i.e. greater than 90° but less than 180°).
        /// </summary>
        public readonly bool IsObtuse
        {
            get { return radians_ > MathUtil.PiOverTwo && radians_ < MathUtil.Pi; }
        }

        /// <summary>
        /// Gets a System.Boolean that determines whether this SharpDX.Angle
        /// is a reflex angle (i.e. greater than 180° but less than 360°).
        /// </summary>
        public readonly bool IsReflex
        {
            get { return radians_ > MathUtil.Pi && radians_ < MathUtil.TwoPi; }
        }

        /// <summary>
        /// Gets a SharpDX.AngleSingle instance that complements this angle (i.e. the two angles add to 90°).
        /// </summary>
        public readonly AngleSingle Complement
        {
            get { return new AngleSingle(MathUtil.PiOverTwo - radians_, AngleType.Radian); }
        }

        /// <summary>
        /// Gets a SharpDX.AngleSingle instance that supplements this angle (i.e. the two angles add to 180°).
        /// </summary>
        public readonly AngleSingle Supplement
        {
            get { return new AngleSingle(MathUtil.Pi - radians_, AngleType.Radian); }
        }

        /// <summary>
        /// Wraps the SharpDX.AngleSingle given in the value argument to be in the range [π, -π].
        /// </summary>
        /// <param name="value">A SharpDX.AngleSingle to wrap.</param>
        /// <returns>The SharpDX.AngleSingle that is wrapped.</returns>
        public static AngleSingle Wrap(AngleSingle value)
        {
            value.Wrap();
            return value;
        }

        /// <summary>
        /// Wraps the SharpDX.AngleSingle given in the value argument to be in the range [0, 2π).
        /// </summary>
        /// <param name="value">A SharpDX.AngleSingle to wrap.</param>
        /// <returns>The SharpDX.AngleSingle that is wrapped.</returns>
        public static AngleSingle WrapPositive(AngleSingle value)
        {
            value.WrapPositive();
            return value;
        }

        /// <summary>
        /// Compares two SharpDX.AngleSingle instances and returns the smaller angle.
        /// </summary>
        /// <param name="left">The first SharpDX.AngleSingle instance to compare.</param>
        /// <param name="right">The second SharpDX.AngleSingle instance to compare.</param>
        /// <returns>The smaller of the two given SharpDX.AngleSingle instances.</returns>
        public static AngleSingle Min(AngleSingle left, AngleSingle right)
        {
            return left.radians_ < right.radians_ ? left : right;
        }

        /// <summary>
        /// Compares two SharpDX.AngleSingle instances and returns the greater angle.
        /// </summary>
        /// <param name="left">The first SharpDX.AngleSingle instance to compare.</param>
        /// <param name="right">The second SharpDX.AngleSingle instance to compare.</param>
        /// <returns>The greater of the two given SharpDX.AngleSingle instances.</returns>
        public static AngleSingle Max(AngleSingle left, AngleSingle right)
        {
            return left.radians_ > right.radians_ ? left : right;
        }

        /// <summary>
        /// Adds two SharpDX.AngleSingle objects and returns the result.
        /// </summary>
        /// <param name="left">The first object to add.</param>
        /// <param name="right">The second object to add.</param>
        /// <returns>The value of the two objects added together.</returns>
        public static AngleSingle Add(AngleSingle left, AngleSingle right)
        {
            return new AngleSingle(left.radians_ + right.radians_, AngleType.Radian);
        }

        /// <summary>
        /// Subtracts two SharpDX.AngleSingle objects and returns the result.
        /// </summary>
        /// <param name="left">The first object to subtract.</param>
        /// <param name="right">The second object to subtract.</param>
        /// <returns>The value of the two objects subtracted.</returns>
        public static AngleSingle Subtract(AngleSingle left, AngleSingle right)
        {
            return new AngleSingle(left.radians_ - right.radians_, AngleType.Radian);
        }

        /// <summary>
        /// Multiplies two SharpDX.AngleSingle objects and returns the result.
        /// </summary>
        /// <param name="left">The first object to multiply.</param>
        /// <param name="right">The second object to multiply.</param>
        /// <returns>The value of the two objects multiplied together.</returns>
        public static AngleSingle Multiply(AngleSingle left, AngleSingle right)
        {
            return new AngleSingle(left.radians_ * right.radians_, AngleType.Radian);
        }

        /// <summary>
        /// Divides two SharpDX.AngleSingle objects and returns the result.
        /// </summary>
        /// <param name="left">The numerator object.</param>
        /// <param name="right">The denominator object.</param>
        /// <returns>The value of the two objects divided.</returns>
        public static AngleSingle Divide(AngleSingle left, AngleSingle right)
        {
            return new AngleSingle(left.radians_ / right.radians_, AngleType.Radian);
        }

        /// <summary>
        /// Gets a new SharpDX.AngleSingle instance that represents the zero angle (i.e. 0°).
        /// </summary>
        public static AngleSingle ZeroAngle
        {
            get { return new AngleSingle(0.0f, AngleType.Radian); }
        }

        /// <summary>
        /// Gets a new SharpDX.AngleSingle instance that represents the right angle (i.e. 90° or π/2).
        /// </summary>
        public static AngleSingle RightAngle
        {
            get { return new AngleSingle(MathUtil.PiOverTwo, AngleType.Radian); }
        }

        /// <summary>
        /// Gets a new SharpDX.AngleSingle instance that represents the straight angle (i.e. 180° or π).
        /// </summary>
        public static AngleSingle StraightAngle
        {
            get { return new AngleSingle(MathUtil.Pi, AngleType.Radian); }
        }

        /// <summary>
        /// Gets a new SharpDX.AngleSingle instance that represents the full rotation angle (i.e. 360° or 2π).
        /// </summary>
        public static AngleSingle FullRotationAngle
        {
            get { return new AngleSingle(MathUtil.TwoPi, AngleType.Radian); }
        }

        /// <summary>
        /// Returns a System.Boolean that indicates whether the values of two SharpDX.Angle
        /// objects are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>True if the left and right parameters have the same value; otherwise, false.</returns>
        public static bool operator ==(AngleSingle left, AngleSingle right)
        {
            return left.radians_ == right.radians_;
        }

        /// <summary>
        /// Returns a System.Boolean that indicates whether the values of two SharpDX.Angle
        /// objects are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>True if the left and right parameters do not have the same value; otherwise, false.</returns>
        public static bool operator !=(AngleSingle left, AngleSingle right)
        {
            return left.radians_ != right.radians_;
        }

        /// <summary>
        /// Returns a System.Boolean that indicates whether a SharpDX.Angle
        /// object is less than another SharpDX.AngleSingle object.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>True if left is less than right; otherwise, false.</returns>
        public static bool operator <(AngleSingle left, AngleSingle right)
        {
            return left.radians_ < right.radians_;
        }

        /// <summary>
        /// Returns a System.Boolean that indicates whether a SharpDX.Angle
        /// object is greater than another SharpDX.AngleSingle object.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>True if left is greater than right; otherwise, false.</returns>
        public static bool operator >(AngleSingle left, AngleSingle right)
        {
            return left.radians_ > right.radians_;
        }

        /// <summary>
        /// Returns a System.Boolean that indicates whether a SharpDX.Angle
        /// object is less than or equal to another SharpDX.AngleSingle object.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>True if left is less than or equal to right; otherwise, false.</returns>
        public static bool operator <=(AngleSingle left, AngleSingle right)
        {
            return left.radians_ <= right.radians_;
        }

        /// <summary>
        /// Returns a System.Boolean that indicates whether a SharpDX.Angle
        /// object is greater than or equal to another SharpDX.AngleSingle object.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>True if left is greater than or equal to right; otherwise, false.</returns>
        public static bool operator >=(AngleSingle left, AngleSingle right)
        {
            return left.radians_ >= right.radians_;
        }

        /// <summary>
        /// Returns the value of the SharpDX.AngleSingle operand. (The sign of
        /// the operand is unchanged.)
        /// </summary>
        /// <param name="value">A SharpDX.AngleSingle object.</param>
        /// <returns>The value of the value parameter.</returns>
        public static AngleSingle operator +(AngleSingle value)
        {
            return value;
        }

        /// <summary>
        /// Returns the the negated value of the SharpDX.AngleSingle operand.
        /// </summary>
        /// <param name="value">A SharpDX.AngleSingle object.</param>
        /// <returns>The negated value of the value parameter.</returns>
        public static AngleSingle operator -(AngleSingle value)
        {
            return new AngleSingle(-value.radians_, AngleType.Radian);
        }

        /// <summary>
        /// Adds two SharpDX.AngleSingle objects and returns the result.
        /// </summary>
        /// <param name="left">The first object to add.</param>
        /// <param name="right">The second object to add.</param>
        /// <returns>The value of the two objects added together.</returns>
        public static AngleSingle operator +(AngleSingle left, AngleSingle right)
        {
            return new AngleSingle(left.radians_ + right.radians_, AngleType.Radian);
        }

        /// <summary>
        /// Subtracts two SharpDX.AngleSingle objects and returns the result.
        /// </summary>
        /// <param name="left">The first object to subtract</param>
        /// <param name="right">The second object to subtract.</param>
        /// <returns>The value of the two objects subtracted.</returns>
        public static AngleSingle operator -(AngleSingle left, AngleSingle right)
        {
            return new AngleSingle(left.radians_ - right.radians_, AngleType.Radian);
        }

        /// <summary>
        /// Multiplies two SharpDX.AngleSingle objects and returns the result.
        /// </summary>
        /// <param name="left">The first object to multiply.</param>
        /// <param name="right">The second object to multiply.</param>
        /// <returns>The value of the two objects multiplied together.</returns>
        public static AngleSingle operator *(AngleSingle left, AngleSingle right)
        {
            return new AngleSingle(left.radians_ * right.radians_, AngleType.Radian);
        }

        /// <summary>
        /// Divides two SharpDX.AngleSingle objects and returns the result.
        /// </summary>
        /// <param name="left">The numerator object.</param>
        /// <param name="right">The denominator object.</param>
        /// <returns>The value of the two objects divided.</returns>
        public static AngleSingle operator /(AngleSingle left, AngleSingle right)
        {
            return new AngleSingle(left.radians_ / right.radians_, AngleType.Radian);
        }

        /// <summary>
        /// Compares this instance to a specified object and returns an integer that
        /// indicates whether the value of this instance is less than, equal to, or greater
        /// than the value of the specified object.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        /// <returns>
        /// A signed integer that indicates the relationship of the current instance
        /// to the obj parameter. If the value is less than zero, the current instance
        /// is less than the other. If the value is zero, the current instance is equal
        /// to the other. If the value is greater than zero, the current instance is
        /// greater than the other.
        /// </returns>
        public readonly int CompareTo(object? other)
        {
            if (other == null)
            {
                return 1;
            }

            if (other is not AngleSingle angle)
            {
                throw new ArgumentException("Argument must be of type Angle.", nameof(other));
            }

            float radians = angle.radians_;

            return this.radians_ > radians ? 1 : this.radians_ < radians ? -1 : 0;
        }

        /// <summary>
        /// Compares this instance to a second SharpDX.AngleSingle and returns
        /// an integer that indicates whether the value of this instance is less than,
        /// equal to, or greater than the value of the specified object.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        /// <returns>
        /// A signed integer that indicates the relationship of the current instance
        /// to the obj parameter. If the value is less than zero, the current instance
        /// is less than the other. If the value is zero, the current instance is equal
        /// to the other. If the value is greater than zero, the current instance is
        /// greater than the other.
        /// </returns>
        public readonly int CompareTo(AngleSingle other)
        {
            return this.radians_ > other.radians_ ? 1 : this.radians_ < other.radians_ ? -1 : 0;
        }

        /// <summary>
        /// Returns a value that indicates whether the current instance and a specified
        /// SharpDX.AngleSingle object have the same value.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        /// <returns>
        /// Returns true if this SharpDX.AngleSingle object and another have the same value;
        /// otherwise, false.
        /// </returns>
        public readonly bool Equals(AngleSingle other)
        {
            return this == other;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override readonly string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, MathUtil.RadiansToDegrees(radians_).ToString("0.##°"));
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public readonly string ToString(string format)
        {
            return format == null
                ? ToString()
                : string.Format(CultureInfo.CurrentCulture, "{0}°", MathUtil.RadiansToDegrees(radians_).ToString(format, CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public readonly string ToString(IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, MathUtil.RadiansToDegrees(radians_).ToString("0.##°"));
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public readonly string ToString(string? format, IFormatProvider? formatProvider)
        {
            return format == null && formatProvider == null
                ? string.Empty
                : format == null
                ? ToString(formatProvider!)
                : string.Format(formatProvider, "{0}°", MathUtil.RadiansToDegrees(radians_).ToString(format, CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Returns a hash code for this SharpDX.AngleSingle instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override readonly int GetHashCode()
        {
            return radiansInt_;
        }

        /// <summary>
        /// Returns a value that indicates whether the current instance and a specified
        /// object have the same value.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>
        /// Returns true if the obj parameter is a SharpDX.AngleSingle object or a type
        /// capable of implicit conversion to a SharpDX.AngleSingle value, and
        /// its value is equal to the value of the current SharpDX.Angle
        /// object; otherwise, false.
        /// </returns>
        public override readonly bool Equals(object? obj)
        {
            return (obj is AngleSingle) && (this == (AngleSingle)obj);
        }
    }
}
