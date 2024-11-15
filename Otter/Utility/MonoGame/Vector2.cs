#region License
/*
MIT License
Copyright © 2006 The Mono.Xna Team

All rights reserved.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#endregion License

using System;
using System.Globalization;

namespace Otter.Utility.MonoGame;

/// <summary>
/// Struct for representing a Vector2.
/// </summary>
public struct Vector2 : IEquatable<Vector2>
{

    #region Private Fields

    private static Vector2 zeroVector = new(0f, 0f);
    private static Vector2 unitVector = new(1f, 1f);
    private static Vector2 unitXVector = new(1f, 0f);
    private static Vector2 unitYVector = new(0f, 1f);

    #endregion Private Fields

    #region Public Fields

    public float X;

    public float Y;

    #endregion Public Fields

    #region Properties

    public static Vector2 Zero => zeroVector;

    public static Vector2 One => unitVector;

    public static Vector2 UnitX => unitXVector;

    public static Vector2 UnitY => unitYVector;

    public static Vector2 MaxValue => new(float.MaxValue, float.MaxValue);

    #endregion Properties

    #region Constructors

    public Vector2(float x, float y)
    {
        X = x;
        Y = y;
    }

    public Vector2(float value)
    {
        X = value;
        Y = value;
    }

    #endregion Constructors

    #region Public Methods

    public static Vector2 CreateFromAngleLength(float angle, float length)
    {
        return new Vector2(
            (float)Math.Cos(angle * (float)Math.PI / 180f) * length,
            (float)Math.Sin(angle * (float)Math.PI / 180f) * -length
            );
    }

    public static Vector2 Add(Vector2 value1, Vector2 value2)
    {
        value1.X += value2.X;
        value1.Y += value2.Y;
        return value1;
    }

    public static void Add(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
    {
        result.X = value1.X + value2.X;
        result.Y = value1.Y + value2.Y;
    }

    public static Vector2 Barycentric(Vector2 value1, Vector2 value2, Vector2 value3, float amount1, float amount2)
    {
        return new Vector2(
            MathHelper.Barycentric(value1.X, value2.X, value3.X, amount1, amount2),
            MathHelper.Barycentric(value1.Y, value2.Y, value3.Y, amount1, amount2));
    }

    public static void Barycentric(ref Vector2 value1, ref Vector2 value2, ref Vector2 value3, float amount1, float amount2, out Vector2 result)
    {
        result = new Vector2(
            MathHelper.Barycentric(value1.X, value2.X, value3.X, amount1, amount2),
            MathHelper.Barycentric(value1.Y, value2.Y, value3.Y, amount1, amount2));
    }

    public static Vector2 CatmullRom(Vector2 value1, Vector2 value2, Vector2 value3, Vector2 value4, float amount)
    {
        return new Vector2(
            MathHelper.CatmullRom(value1.X, value2.X, value3.X, value4.X, amount),
            MathHelper.CatmullRom(value1.Y, value2.Y, value3.Y, value4.Y, amount));
    }

    public static void CatmullRom(ref Vector2 value1, ref Vector2 value2, ref Vector2 value3, ref Vector2 value4, float amount, out Vector2 result)
    {
        result = new Vector2(
            MathHelper.CatmullRom(value1.X, value2.X, value3.X, value4.X, amount),
            MathHelper.CatmullRom(value1.Y, value2.Y, value3.Y, value4.Y, amount));
    }

    public static Vector2 Clamp(Vector2 value1, Vector2 min, Vector2 max)
    {
        return new Vector2(
            MathHelper.Clamp(value1.X, min.X, max.X),
            MathHelper.Clamp(value1.Y, min.Y, max.Y));
    }

    public static void Clamp(ref Vector2 value1, ref Vector2 min, ref Vector2 max, out Vector2 result)
    {
        result = new Vector2(
            MathHelper.Clamp(value1.X, min.X, max.X),
            MathHelper.Clamp(value1.Y, min.Y, max.Y));
    }

    public static float Distance(Vector2 value1, Vector2 value2)
    {
        float v1 = value1.X - value2.X, v2 = value1.Y - value2.Y;
        return (float)Math.Sqrt((v1 * v1) + (v2 * v2));
    }

    public static void Distance(ref Vector2 value1, ref Vector2 value2, out float result)
    {
        float v1 = value1.X - value2.X, v2 = value1.Y - value2.Y;
        result = (float)Math.Sqrt((v1 * v1) + (v2 * v2));
    }

    public static float DistanceSquared(Vector2 value1, Vector2 value2)
    {
        float v1 = value1.X - value2.X, v2 = value1.Y - value2.Y;
        return (v1 * v1) + (v2 * v2);
    }

    public static void DistanceSquared(ref Vector2 value1, ref Vector2 value2, out float result)
    {
        float v1 = value1.X - value2.X, v2 = value1.Y - value2.Y;
        result = (v1 * v1) + (v2 * v2);
    }

    public static Vector2 Divide(Vector2 value1, Vector2 value2)
    {
        value1.X /= value2.X;
        value1.Y /= value2.Y;
        return value1;
    }

    public static void Divide(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
    {
        result.X = value1.X / value2.X;
        result.Y = value1.Y / value2.Y;
    }

    public static Vector2 Divide(Vector2 value1, float divider)
    {
        var factor = 1 / divider;
        value1.X *= factor;
        value1.Y *= factor;
        return value1;
    }

    public static void Divide(ref Vector2 value1, float divider, out Vector2 result)
    {
        var factor = 1 / divider;
        result.X = value1.X * factor;
        result.Y = value1.Y * factor;
    }

    public static float Dot(Vector2 value1, Vector2 value2)
    {
        return (value1.X * value2.X) + (value1.Y * value2.Y);
    }

    public static void Dot(ref Vector2 value1, ref Vector2 value2, out float result)
    {
        result = (value1.X * value2.X) + (value1.Y * value2.Y);
    }

    public override readonly bool Equals(object obj)
    {
        return obj switch
        {
            Vector2 => Equals((Vector2)obj),
            _ => false
        };
    }

    public readonly bool Equals(Vector2 other)
    {
        return (X == other.X) && (Y == other.Y);
    }

    public static Vector2 Reflect(Vector2 vector, Vector2 normal)
    {
        Vector2 result;
        var val = 2.0f * ((vector.X * normal.X) + (vector.Y * normal.Y));
        result.X = vector.X - (normal.X * val);
        result.Y = vector.Y - (normal.Y * val);
        return result;
    }

    public static void Reflect(ref Vector2 vector, ref Vector2 normal, out Vector2 result)
    {
        var val = 2.0f * ((vector.X * normal.X) + (vector.Y * normal.Y));
        result.X = vector.X - (normal.X * val);
        result.Y = vector.Y - (normal.Y * val);
    }

    public override readonly int GetHashCode()
    {
        return X.GetHashCode() + Y.GetHashCode();
    }

    public static Vector2 Hermite(Vector2 value1, Vector2 tangent1, Vector2 value2, Vector2 tangent2, float amount)
    {
        Hermite(ref value1, ref tangent1, ref value2, ref tangent2, amount, out var result);
        return result;
    }

    public static void Hermite(ref Vector2 value1, ref Vector2 tangent1, ref Vector2 value2, ref Vector2 tangent2, float amount, out Vector2 result)
    {
        result.X = MathHelper.Hermite(value1.X, tangent1.X, value2.X, tangent2.X, amount);
        result.Y = MathHelper.Hermite(value1.Y, tangent1.Y, value2.Y, tangent2.Y, amount);
    }

    public float Length
    {
        readonly get => (float)Math.Sqrt((X * X) + (Y * Y));
        set => Normalize(value);
    }

    public readonly float LengthSquared()
    {
        return (X * X) + (Y * Y);
    }

    public static Vector2 Lerp(Vector2 value1, Vector2 value2, float amount)
    {
        return new Vector2(
            MathHelper.Lerp(value1.X, value2.X, amount),
            MathHelper.Lerp(value1.Y, value2.Y, amount));
    }

    public static void Lerp(ref Vector2 value1, ref Vector2 value2, float amount, out Vector2 result)
    {
        result = new Vector2(
            MathHelper.Lerp(value1.X, value2.X, amount),
            MathHelper.Lerp(value1.Y, value2.Y, amount));
    }

    public static Vector2 Max(Vector2 value1, Vector2 value2)
    {
        return new Vector2(value1.X > value2.X ? value1.X : value2.X,
                           value1.Y > value2.Y ? value1.Y : value2.Y);
    }

    public static void Max(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
    {
        result.X = value1.X > value2.X ? value1.X : value2.X;
        result.Y = value1.Y > value2.Y ? value1.Y : value2.Y;
    }

    public static Vector2 Min(Vector2 value1, Vector2 value2)
    {
        return new Vector2(value1.X < value2.X ? value1.X : value2.X,
                           value1.Y < value2.Y ? value1.Y : value2.Y);
    }

    public static void Min(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
    {
        result.X = value1.X < value2.X ? value1.X : value2.X;
        result.Y = value1.Y < value2.Y ? value1.Y : value2.Y;
    }

    public static Vector2 Multiply(Vector2 value1, Vector2 value2)
    {
        value1.X *= value2.X;
        value1.Y *= value2.Y;
        return value1;
    }

    public static Vector2 Multiply(Vector2 value1, float scaleFactor)
    {
        value1.X *= scaleFactor;
        value1.Y *= scaleFactor;
        return value1;
    }

    public static void Multiply(ref Vector2 value1, float scaleFactor, out Vector2 result)
    {
        result.X = value1.X * scaleFactor;
        result.Y = value1.Y * scaleFactor;
    }

    public static void Multiply(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
    {
        result.X = value1.X * value2.X;
        result.Y = value1.Y * value2.Y;
    }

    public static Vector2 Negate(Vector2 value)
    {
        value.X = -value.X;
        value.Y = -value.Y;
        return value;
    }

    public static void Negate(ref Vector2 value, out Vector2 result)
    {
        result.X = -value.X;
        result.Y = -value.Y;
    }

    public void Normalize()
    {
        if (X == 0 && Y == 0)
        {
            return;
        }

        var val = 1.0f / (float)Math.Sqrt((X * X) + (Y * Y));
        X *= val;
        Y *= val;
    }

    public readonly Vector2 Normalized()
    {
        var v = new Vector2(X, Y);
        v.Normalize();
        return v;
    }

    public readonly Vector2 Normalized(float value)
    {
        var v = new Vector2(X, Y);
        v.Normalize(value);
        return v;
    }

    public void Normalize(float value)
    {
        if (value == 0)
        {
            X = 0;
            Y = 0;
            return;
        }

        Normalize();
        X *= value;
        Y *= value;
    }

    public static Vector2 Normalize(Vector2 value)
    {
        var val = 1.0f / (float)Math.Sqrt((value.X * value.X) + (value.Y * value.Y));
        value.X *= val;
        value.Y *= val;
        return value;
    }

    public static void Normalize(ref Vector2 value, out Vector2 result)
    {
        var val = 1.0f / (float)Math.Sqrt((value.X * value.X) + (value.Y * value.Y));
        result.X = value.X * val;
        result.Y = value.Y * val;
    }

    public static Vector2 SmoothStep(Vector2 value1, Vector2 value2, float amount)
    {
        return new Vector2(
            MathHelper.SmoothStep(value1.X, value2.X, amount),
            MathHelper.SmoothStep(value1.Y, value2.Y, amount));
    }

    public static void SmoothStep(ref Vector2 value1, ref Vector2 value2, float amount, out Vector2 result)
    {
        result = new Vector2(
            MathHelper.SmoothStep(value1.X, value2.X, amount),
            MathHelper.SmoothStep(value1.Y, value2.Y, amount));
    }

    public static Vector2 Subtract(Vector2 value1, Vector2 value2)
    {
        value1.X -= value2.X;
        value1.Y -= value2.Y;
        return value1;
    }

    public static void Subtract(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
    {
        result.X = value1.X - value2.X;
        result.Y = value1.Y - value2.Y;
    }

    public static Vector2 Transform(Vector2 position, Matrix matrix)
    {
        Transform(ref position, ref matrix, out position);
        return position;
    }

    public static void Transform(ref Vector2 position, ref Matrix matrix, out Vector2 result)
    {
        result = new Vector2((position.X * matrix.M11) + (position.Y * matrix.M21) + matrix.M41,
                             (position.X * matrix.M12) + (position.Y * matrix.M22) + matrix.M42);
    }

    public static Vector2 Transform(Vector2 position, Quaternion quat)
    {
        Transform(ref position, ref quat, out position);
        return position;
    }

    public static void Transform(ref Vector2 position, ref Quaternion quat, out Vector2 result)
    {
        Quaternion v = new(position.X, position.Y, 0, 0);
        Quaternion.Inverse(ref quat, out Quaternion i);
        Quaternion.Multiply(ref quat, ref v, out Quaternion t);
        Quaternion.Multiply(ref t, ref i, out v);

        result = new Vector2(v.X, v.Y);
    }

    public static void Transform(
        Vector2[] sourceArray,
        ref Matrix matrix,
        Vector2[] destinationArray)
    {
        Transform(sourceArray, 0, ref matrix, destinationArray, 0, sourceArray.Length);
    }


    public static void Transform(
        Vector2[] sourceArray,
        int sourceIndex,
        ref Matrix matrix,
        Vector2[] destinationArray,
        int destinationIndex,
        int length)
    {
        for (var x = 0; x < length; x++)
        {
            var position = sourceArray[sourceIndex + x];
            var destination = destinationArray[destinationIndex + x];
            destination.X = (position.X * matrix.M11) + (position.Y * matrix.M21) + matrix.M41;
            destination.Y = (position.X * matrix.M12) + (position.Y * matrix.M22) + matrix.M42;
            destinationArray[destinationIndex + x] = destination;
        }
    }

    public static Vector2 TransformNormal(Vector2 normal, Matrix matrix)
    {
        TransformNormal(ref normal, ref matrix, out normal);
        return normal;
    }

    public static void TransformNormal(ref Vector2 normal, ref Matrix matrix, out Vector2 result)
    {
        result = new Vector2((normal.X * matrix.M11) + (normal.Y * matrix.M21),
                             (normal.X * matrix.M12) + (normal.Y * matrix.M22));
    }

    public override readonly string ToString()
    {
        CultureInfo currentCulture = CultureInfo.CurrentCulture;
        return string.Format(currentCulture, "{{X:{0} Y:{1}}}", new object[] {
            X.ToString(currentCulture), Y.ToString(currentCulture) });
    }

    #endregion Public Methods

    #region Operators

    public static Vector2 operator -(Vector2 value)
    {
        value.X = -value.X;
        value.Y = -value.Y;
        return value;
    }


    public static bool operator ==(Vector2 value1, Vector2 value2)
    {
        return value1.X == value2.X && value1.Y == value2.Y;
    }


    public static bool operator !=(Vector2 value1, Vector2 value2)
    {
        return value1.X != value2.X || value1.Y != value2.Y;
    }


    public static Vector2 operator +(Vector2 value1, Vector2 value2)
    {
        value1.X += value2.X;
        value1.Y += value2.Y;
        return value1;
    }


    public static Vector2 operator -(Vector2 value1, Vector2 value2)
    {
        value1.X -= value2.X;
        value1.Y -= value2.Y;
        return value1;
    }


    public static Vector2 operator *(Vector2 value1, Vector2 value2)
    {
        value1.X *= value2.X;
        value1.Y *= value2.Y;
        return value1;
    }


    public static Vector2 operator *(Vector2 value, float scaleFactor)
    {
        value.X *= scaleFactor;
        value.Y *= scaleFactor;
        return value;
    }


    public static Vector2 operator *(float scaleFactor, Vector2 value)
    {
        value.X *= scaleFactor;
        value.Y *= scaleFactor;
        return value;
    }


    public static Vector2 operator /(Vector2 value1, Vector2 value2)
    {
        value1.X /= value2.X;
        value1.Y /= value2.Y;
        return value1;
    }


    public static Vector2 operator /(Vector2 value1, float divider)
    {
        var factor = 1 / divider;
        value1.X *= factor;
        value1.Y *= factor;
        return value1;
    }

    #endregion Operators

}
