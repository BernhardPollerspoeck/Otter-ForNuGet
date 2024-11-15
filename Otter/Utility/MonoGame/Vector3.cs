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
using System.Diagnostics;
using System.Text;

namespace Otter.Utility.MonoGame;

/// <summary>
/// Struct for representing a Vector3.
/// </summary>
public struct Vector3 : IEquatable<Vector3>
{

    #region Private Fields

    private static Vector3 zero = new(0f, 0f, 0f);
    private static Vector3 one = new(1f, 1f, 1f);
    private static Vector3 unitX = new(1f, 0f, 0f);
    private static Vector3 unitY = new(0f, 1f, 0f);
    private static Vector3 unitZ = new(0f, 0f, 1f);
    private static Vector3 up = new(0f, 1f, 0f);
    private static Vector3 down = new(0f, -1f, 0f);
    private static Vector3 right = new(1f, 0f, 0f);
    private static Vector3 left = new(-1f, 0f, 0f);
    private static Vector3 forward = new(0f, 0f, -1f);
    private static Vector3 backward = new(0f, 0f, 1f);

    #endregion Private Fields

    #region Public Fields

    public float X;

    public float Y;

    public float Z;

    #endregion Public Fields

    #region Properties

    public static Vector3 Zero => zero;

    public static Vector3 One => one;

    public static Vector3 UnitX => unitX;

    public static Vector3 UnitY => unitY;

    public static Vector3 UnitZ => unitZ;

    public static Vector3 Up => up;

    public static Vector3 Down => down;

    public static Vector3 Right => right;

    public static Vector3 Left => left;

    public static Vector3 Forward => forward;

    public static Vector3 Backward => backward;

    #endregion Properties

    #region Constructors

    public Vector3(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }


    public Vector3(float value)
    {
        X = value;
        Y = value;
        Z = value;
    }


    public Vector3(Vector2 value, float z)
    {
        X = value.X;
        Y = value.Y;
        Z = z;
    }


    #endregion Constructors

    #region Public Methods

    public static Vector3 Add(Vector3 value1, Vector3 value2)
    {
        value1.X += value2.X;
        value1.Y += value2.Y;
        value1.Z += value2.Z;
        return value1;
    }

    public static void Add(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
    {
        result.X = value1.X + value2.X;
        result.Y = value1.Y + value2.Y;
        result.Z = value1.Z + value2.Z;
    }

    public static Vector3 Barycentric(Vector3 value1, Vector3 value2, Vector3 value3, float amount1, float amount2)
    {
        return new Vector3(
            MathHelper.Barycentric(value1.X, value2.X, value3.X, amount1, amount2),
            MathHelper.Barycentric(value1.Y, value2.Y, value3.Y, amount1, amount2),
            MathHelper.Barycentric(value1.Z, value2.Z, value3.Z, amount1, amount2));
    }

    public static void Barycentric(ref Vector3 value1, ref Vector3 value2, ref Vector3 value3, float amount1, float amount2, out Vector3 result)
    {
        result = new Vector3(
            MathHelper.Barycentric(value1.X, value2.X, value3.X, amount1, amount2),
            MathHelper.Barycentric(value1.Y, value2.Y, value3.Y, amount1, amount2),
            MathHelper.Barycentric(value1.Z, value2.Z, value3.Z, amount1, amount2));
    }

    public static Vector3 CatmullRom(Vector3 value1, Vector3 value2, Vector3 value3, Vector3 value4, float amount)
    {
        return new Vector3(
            MathHelper.CatmullRom(value1.X, value2.X, value3.X, value4.X, amount),
            MathHelper.CatmullRom(value1.Y, value2.Y, value3.Y, value4.Y, amount),
            MathHelper.CatmullRom(value1.Z, value2.Z, value3.Z, value4.Z, amount));
    }

    public static void CatmullRom(ref Vector3 value1, ref Vector3 value2, ref Vector3 value3, ref Vector3 value4, float amount, out Vector3 result)
    {
        result = new Vector3(
            MathHelper.CatmullRom(value1.X, value2.X, value3.X, value4.X, amount),
            MathHelper.CatmullRom(value1.Y, value2.Y, value3.Y, value4.Y, amount),
            MathHelper.CatmullRom(value1.Z, value2.Z, value3.Z, value4.Z, amount));
    }

    public static Vector3 Clamp(Vector3 value1, Vector3 min, Vector3 max)
    {
        return new Vector3(
            MathHelper.Clamp(value1.X, min.X, max.X),
            MathHelper.Clamp(value1.Y, min.Y, max.Y),
            MathHelper.Clamp(value1.Z, min.Z, max.Z));
    }

    public static void Clamp(ref Vector3 value1, ref Vector3 min, ref Vector3 max, out Vector3 result)
    {
        result = new Vector3(
            MathHelper.Clamp(value1.X, min.X, max.X),
            MathHelper.Clamp(value1.Y, min.Y, max.Y),
            MathHelper.Clamp(value1.Z, min.Z, max.Z));
    }

    public static Vector3 Cross(Vector3 vector1, Vector3 vector2)
    {
        Cross(ref vector1, ref vector2, out vector1);
        return vector1;
    }

    public static void Cross(ref Vector3 vector1, ref Vector3 vector2, out Vector3 result)
    {
        result = new Vector3((vector1.Y * vector2.Z) - (vector2.Y * vector1.Z),
                             -((vector1.X * vector2.Z) - (vector2.X * vector1.Z)),
                             (vector1.X * vector2.Y) - (vector2.X * vector1.Y));
    }

    public static float Distance(Vector3 vector1, Vector3 vector2)
    {
        DistanceSquared(ref vector1, ref vector2, out var result);
        return (float)Math.Sqrt(result);
    }

    public static void Distance(ref Vector3 value1, ref Vector3 value2, out float result)
    {
        DistanceSquared(ref value1, ref value2, out result);
        result = (float)Math.Sqrt(result);
    }

    public static float DistanceSquared(Vector3 value1, Vector3 value2)
    {
        DistanceSquared(ref value1, ref value2, out var result);
        return result;
    }

    public static void DistanceSquared(ref Vector3 value1, ref Vector3 value2, out float result)
    {
        result = ((value1.X - value2.X) * (value1.X - value2.X)) +
                 ((value1.Y - value2.Y) * (value1.Y - value2.Y)) +
                 ((value1.Z - value2.Z) * (value1.Z - value2.Z));
    }

    public static Vector3 Divide(Vector3 value1, Vector3 value2)
    {
        value1.X /= value2.X;
        value1.Y /= value2.Y;
        value1.Z /= value2.Z;
        return value1;
    }

    public static Vector3 Divide(Vector3 value1, float value2)
    {
        var factor = 1 / value2;
        value1.X *= factor;
        value1.Y *= factor;
        value1.Z *= factor;
        return value1;
    }

    public static void Divide(ref Vector3 value1, float divisor, out Vector3 result)
    {
        var factor = 1 / divisor;
        result.X = value1.X * factor;
        result.Y = value1.Y * factor;
        result.Z = value1.Z * factor;
    }

    public static void Divide(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
    {
        result.X = value1.X / value2.X;
        result.Y = value1.Y / value2.Y;
        result.Z = value1.Z / value2.Z;
    }

    public static float Dot(Vector3 vector1, Vector3 vector2)
    {
        return (vector1.X * vector2.X) + (vector1.Y * vector2.Y) + (vector1.Z * vector2.Z);
    }

    public static void Dot(ref Vector3 vector1, ref Vector3 vector2, out float result)
    {
        result = (vector1.X * vector2.X) + (vector1.Y * vector2.Y) + (vector1.Z * vector2.Z);
    }

    public override readonly bool Equals(object obj)
    {
        if (obj is not Vector3)
        {
            return false;
        }

        var other = (Vector3)obj;
        return X == other.X &&
                Y == other.Y &&
                Z == other.Z;
    }

    public readonly bool Equals(Vector3 other)
    {
        return X == other.X &&
                Y == other.Y &&
                Z == other.Z;
    }

    public override readonly int GetHashCode()
    {
        return (int)(X + Y + Z);
    }

    public static Vector3 Hermite(Vector3 value1, Vector3 tangent1, Vector3 value2, Vector3 tangent2, float amount)
    {
        Hermite(ref value1, ref tangent1, ref value2, ref tangent2, amount, out var result);
        return result;
    }

    public static void Hermite(ref Vector3 value1, ref Vector3 tangent1, ref Vector3 value2, ref Vector3 tangent2, float amount, out Vector3 result)
    {
        result.X = MathHelper.Hermite(value1.X, tangent1.X, value2.X, tangent2.X, amount);
        result.Y = MathHelper.Hermite(value1.Y, tangent1.Y, value2.Y, tangent2.Y, amount);
        result.Z = MathHelper.Hermite(value1.Z, tangent1.Z, value2.Z, tangent2.Z, amount);
    }

    public float Length()
    {
        DistanceSquared(ref this, ref zero, out var result);
        return (float)Math.Sqrt(result);
    }

    public float LengthSquared()
    {
        DistanceSquared(ref this, ref zero, out var result);
        return result;
    }

    public static Vector3 Lerp(Vector3 value1, Vector3 value2, float amount)
    {
        return new Vector3(
            MathHelper.Lerp(value1.X, value2.X, amount),
            MathHelper.Lerp(value1.Y, value2.Y, amount),
            MathHelper.Lerp(value1.Z, value2.Z, amount));
    }

    public static void Lerp(ref Vector3 value1, ref Vector3 value2, float amount, out Vector3 result)
    {
        result = new Vector3(
            MathHelper.Lerp(value1.X, value2.X, amount),
            MathHelper.Lerp(value1.Y, value2.Y, amount),
            MathHelper.Lerp(value1.Z, value2.Z, amount));
    }

    public static Vector3 Max(Vector3 value1, Vector3 value2)
    {
        return new Vector3(
            MathHelper.Max(value1.X, value2.X),
            MathHelper.Max(value1.Y, value2.Y),
            MathHelper.Max(value1.Z, value2.Z));
    }

    public static void Max(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
    {
        result = new Vector3(
            MathHelper.Max(value1.X, value2.X),
            MathHelper.Max(value1.Y, value2.Y),
            MathHelper.Max(value1.Z, value2.Z));
    }

    public static Vector3 Min(Vector3 value1, Vector3 value2)
    {
        return new Vector3(
            MathHelper.Min(value1.X, value2.X),
            MathHelper.Min(value1.Y, value2.Y),
            MathHelper.Min(value1.Z, value2.Z));
    }

    public static void Min(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
    {
        result = new Vector3(
            MathHelper.Min(value1.X, value2.X),
            MathHelper.Min(value1.Y, value2.Y),
            MathHelper.Min(value1.Z, value2.Z));
    }

    public static Vector3 Multiply(Vector3 value1, Vector3 value2)
    {
        value1.X *= value2.X;
        value1.Y *= value2.Y;
        value1.Z *= value2.Z;
        return value1;
    }

    public static Vector3 Multiply(Vector3 value1, float scaleFactor)
    {
        value1.X *= scaleFactor;
        value1.Y *= scaleFactor;
        value1.Z *= scaleFactor;
        return value1;
    }

    public static void Multiply(ref Vector3 value1, float scaleFactor, out Vector3 result)
    {
        result.X = value1.X * scaleFactor;
        result.Y = value1.Y * scaleFactor;
        result.Z = value1.Z * scaleFactor;
    }

    public static void Multiply(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
    {
        result.X = value1.X * value2.X;
        result.Y = value1.Y * value2.Y;
        result.Z = value1.Z * value2.Z;
    }

    public static Vector3 Negate(Vector3 value)
    {
        value = new Vector3(-value.X, -value.Y, -value.Z);
        return value;
    }

    public static void Negate(ref Vector3 value, out Vector3 result)
    {
        result = new Vector3(-value.X, -value.Y, -value.Z);
    }

    public void Normalize()
    {
        Normalize(ref this, out this);
    }

    public static Vector3 Normalize(Vector3 vector)
    {
        Normalize(ref vector, out vector);
        return vector;
    }

    public static void Normalize(ref Vector3 value, out Vector3 result)
    {
        Distance(ref value, ref zero, out var factor);
        factor = 1f / factor;
        result.X = value.X * factor;
        result.Y = value.Y * factor;
        result.Z = value.Z * factor;
    }

    public static Vector3 Reflect(Vector3 vector, Vector3 normal)
    {
        // I is the original array
        // N is the normal of the incident plane
        // R = I - (2 * N * ( DotProduct[ I,N] ))
        Vector3 reflectedVector;
        // inline the dotProduct here instead of calling method
        var dotProduct = (vector.X * normal.X) + (vector.Y * normal.Y) + (vector.Z * normal.Z);
        reflectedVector.X = vector.X - (2.0f * normal.X * dotProduct);
        reflectedVector.Y = vector.Y - (2.0f * normal.Y * dotProduct);
        reflectedVector.Z = vector.Z - (2.0f * normal.Z * dotProduct);

        return reflectedVector;
    }

    public static void Reflect(ref Vector3 vector, ref Vector3 normal, out Vector3 result)
    {
        // I is the original array
        // N is the normal of the incident plane
        // R = I - (2 * N * ( DotProduct[ I,N] ))

        // inline the dotProduct here instead of calling method
        var dotProduct = (vector.X * normal.X) + (vector.Y * normal.Y) + (vector.Z * normal.Z);
        result.X = vector.X - (2.0f * normal.X * dotProduct);
        result.Y = vector.Y - (2.0f * normal.Y * dotProduct);
        result.Z = vector.Z - (2.0f * normal.Z * dotProduct);

    }

    public static Vector3 SmoothStep(Vector3 value1, Vector3 value2, float amount)
    {
        return new Vector3(
            MathHelper.SmoothStep(value1.X, value2.X, amount),
            MathHelper.SmoothStep(value1.Y, value2.Y, amount),
            MathHelper.SmoothStep(value1.Z, value2.Z, amount));
    }

    public static void SmoothStep(ref Vector3 value1, ref Vector3 value2, float amount, out Vector3 result)
    {
        result = new Vector3(
            MathHelper.SmoothStep(value1.X, value2.X, amount),
            MathHelper.SmoothStep(value1.Y, value2.Y, amount),
            MathHelper.SmoothStep(value1.Z, value2.Z, amount));
    }

    public static Vector3 Subtract(Vector3 value1, Vector3 value2)
    {
        value1.X -= value2.X;
        value1.Y -= value2.Y;
        value1.Z -= value2.Z;
        return value1;
    }

    public static void Subtract(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
    {
        result.X = value1.X - value2.X;
        result.Y = value1.Y - value2.Y;
        result.Z = value1.Z - value2.Z;
    }

    public override readonly string ToString()
    {
        StringBuilder sb = new(32);
        sb.Append("{X:");
        sb.Append(X);
        sb.Append(" Y:");
        sb.Append(Y);
        sb.Append(" Z:");
        sb.Append(Z);
        sb.Append('}');
        return sb.ToString();
    }

    public static Vector3 Transform(Vector3 position, Matrix matrix)
    {
        Transform(ref position, ref matrix, out position);
        return position;
    }

    public static void Transform(ref Vector3 position, ref Matrix matrix, out Vector3 result)
    {
        result = new Vector3((position.X * matrix.M11) + (position.Y * matrix.M21) + (position.Z * matrix.M31) + matrix.M41,
                             (position.X * matrix.M12) + (position.Y * matrix.M22) + (position.Z * matrix.M32) + matrix.M42,
                             (position.X * matrix.M13) + (position.Y * matrix.M23) + (position.Z * matrix.M33) + matrix.M43);
    }

    public static void Transform(Vector3[] sourceArray, ref Matrix matrix, Vector3[] destinationArray)
    {
        Debug.Assert(destinationArray.Length >= sourceArray.Length, "The destination array is smaller than the source array.");

        // TODO: Are there options on some platforms to implement a vectorized version of this?

        for (var i = 0; i < sourceArray.Length; i++)
        {
            var position = sourceArray[i];
            destinationArray[i] =
                new Vector3(
                    (position.X * matrix.M11) + (position.Y * matrix.M21) + (position.Z * matrix.M31) + matrix.M41,
                    (position.X * matrix.M12) + (position.Y * matrix.M22) + (position.Z * matrix.M32) + matrix.M42,
                    (position.X * matrix.M13) + (position.Y * matrix.M23) + (position.Z * matrix.M33) + matrix.M43);
        }
    }

    public static void Transform(Vector3[] sourceArray, int sourceIndex, ref Matrix matrix, Vector3[] destinationArray, int destinationIndex, int length)
    {
        Debug.Assert(sourceArray.Length - sourceIndex >= length,
            "The source array is too small for the given sourceIndex and length.");
        Debug.Assert(destinationArray.Length - destinationIndex >= length,
            "The destination array is too small for the given destinationIndex and length.");

        // TODO: Are there options on some platforms to implement a vectorized version of this?

        for (var i = 0; i < length; i++)
        {
            var position = sourceArray[sourceIndex + i];
            destinationArray[destinationIndex + i] =
                new Vector3(
                    (position.X * matrix.M11) + (position.Y * matrix.M21) + (position.Z * matrix.M31) + matrix.M41,
                    (position.X * matrix.M12) + (position.Y * matrix.M22) + (position.Z * matrix.M32) + matrix.M42,
                    (position.X * matrix.M13) + (position.Y * matrix.M23) + (position.Z * matrix.M33) + matrix.M43);
        }
    }

    /// <summary>
    /// Transforms a vector by a quaternion rotation.
    /// </summary>
    /// <param name="vec">The vector to transform.</param>
    /// <param name="quat">The quaternion to rotate the vector by.</param>
    /// <returns>The result of the operation.</returns>
    public static Vector3 Transform(Vector3 vec, Quaternion quat)
    {
        Transform(ref vec, ref quat, out Vector3 result);
        return result;
    }

    ///// <summary>
    ///// Transforms a vector by a quaternion rotation.
    ///// </summary>
    ///// <param name="vec">The vector to transform.</param>
    ///// <param name="quat">The quaternion to rotate the vector by.</param>
    ///// <param name="result">The result of the operation.</param>
    //        public static void Transform(ref Vector3 vec, ref Quaternion quat, out Vector3 result)
    //        {
    //		// Taken from the OpentTK implementation of Vector3
    //            // Since vec.W == 0, we can optimize quat * vec * quat^-1 as follows:
    //            // vec + 2.0 * cross(quat.xyz, cross(quat.xyz, vec) + quat.w * vec)
    //            Vector3 xyz = quat.Xyz, temp, temp2;
    //            Vector3.Cross(ref xyz, ref vec, out temp);
    //            Vector3.Multiply(ref vec, quat.W, out temp2);
    //            Vector3.Add(ref temp, ref temp2, out temp);
    //            Vector3.Cross(ref xyz, ref temp, out temp);
    //            Vector3.Multiply(ref temp, 2, out temp);
    //            Vector3.Add(ref vec, ref temp, out result);
    //        }

    /// <summary>
    /// Transforms a vector by a quaternion rotation.
    /// </summary>
    /// <param name="value">The vector to transform.</param>
    /// <param name="rotation">The quaternion to rotate the vector by.</param>
    /// <param name="result">The result of the operation.</param>
    public static void Transform(ref Vector3 value, ref Quaternion rotation, out Vector3 result)
    {
        var x = 2 * ((rotation.Y * value.Z) - (rotation.Z * value.Y));
        var y = 2 * ((rotation.Z * value.X) - (rotation.X * value.Z));
        var z = 2 * ((rotation.X * value.Y) - (rotation.Y * value.X));

        result.X = value.X + (x * rotation.W) + ((rotation.Y * z) - (rotation.Z * y));
        result.Y = value.Y + (y * rotation.W) + ((rotation.Z * x) - (rotation.X * z));
        result.Z = value.Z + (z * rotation.W) + ((rotation.X * y) - (rotation.Y * x));
    }

    /// <summary>
    /// Transforms an array of vectors by a quaternion rotation.
    /// </summary>
    /// <param name="sourceArray">The vectors to transform</param>
    /// <param name="rotation">The quaternion to rotate the vector by.</param>
    /// <param name="destinationArray">The result of the operation.</param>
    public static void Transform(Vector3[] sourceArray, ref Quaternion rotation, Vector3[] destinationArray)
    {
        Debug.Assert(destinationArray.Length >= sourceArray.Length, "The destination array is smaller than the source array.");

        // TODO: Are there options on some platforms to implement a vectorized version of this?

        for (var i = 0; i < sourceArray.Length; i++)
        {
            var position = sourceArray[i];

            var x = 2 * ((rotation.Y * position.Z) - (rotation.Z * position.Y));
            var y = 2 * ((rotation.Z * position.X) - (rotation.X * position.Z));
            var z = 2 * ((rotation.X * position.Y) - (rotation.Y * position.X));

            destinationArray[i] =
                new Vector3(
                    position.X + (x * rotation.W) + ((rotation.Y * z) - (rotation.Z * y)),
                    position.Y + (y * rotation.W) + ((rotation.Z * x) - (rotation.X * z)),
                    position.Z + (z * rotation.W) + ((rotation.X * y) - (rotation.Y * x)));
        }
    }

    /// <summary>
    /// Transforms an array of vectors within a given range by a quaternion rotation.
    /// </summary>
    /// <param name="sourceArray">The vectors to transform.</param>
    /// <param name="sourceIndex">The starting index in the source array.</param>
    /// <param name="rotation">The quaternion to rotate the vector by.</param>
    /// <param name="destinationArray">The array to store the result of the operation.</param>
    /// <param name="destinationIndex">The starting index in the destination array.</param>
    /// <param name="length">The number of vectors to transform.</param>
    public static void Transform(Vector3[] sourceArray, int sourceIndex, ref Quaternion rotation, Vector3[] destinationArray, int destinationIndex, int length)
    {
        Debug.Assert(sourceArray.Length - sourceIndex >= length,
            "The source array is too small for the given sourceIndex and length.");
        Debug.Assert(destinationArray.Length - destinationIndex >= length,
            "The destination array is too small for the given destinationIndex and length.");

        // TODO: Are there options on some platforms to implement a vectorized version of this?

        for (var i = 0; i < length; i++)
        {
            var position = sourceArray[sourceIndex + i];

            var x = 2 * ((rotation.Y * position.Z) - (rotation.Z * position.Y));
            var y = 2 * ((rotation.Z * position.X) - (rotation.X * position.Z));
            var z = 2 * ((rotation.X * position.Y) - (rotation.Y * position.X));

            destinationArray[destinationIndex + i] =
                new Vector3(
                    position.X + (x * rotation.W) + ((rotation.Y * z) - (rotation.Z * y)),
                    position.Y + (y * rotation.W) + ((rotation.Z * x) - (rotation.X * z)),
                    position.Z + (z * rotation.W) + ((rotation.X * y) - (rotation.Y * x)));
        }
    }


    public static Vector3 TransformNormal(Vector3 normal, Matrix matrix)
    {
        TransformNormal(ref normal, ref matrix, out normal);
        return normal;
    }

    public static void TransformNormal(ref Vector3 normal, ref Matrix matrix, out Vector3 result)
    {
        result = new Vector3((normal.X * matrix.M11) + (normal.Y * matrix.M21) + (normal.Z * matrix.M31),
                             (normal.X * matrix.M12) + (normal.Y * matrix.M22) + (normal.Z * matrix.M32),
                             (normal.X * matrix.M13) + (normal.Y * matrix.M23) + (normal.Z * matrix.M33));
    }

    #endregion Public methods

    #region Operators

    public static bool operator ==(Vector3 value1, Vector3 value2)
    {
        return value1.X == value2.X
            && value1.Y == value2.Y
            && value1.Z == value2.Z;
    }

    public static bool operator !=(Vector3 value1, Vector3 value2)
    {
        return !(value1 == value2);
    }

    public static Vector3 operator +(Vector3 value1, Vector3 value2)
    {
        value1.X += value2.X;
        value1.Y += value2.Y;
        value1.Z += value2.Z;
        return value1;
    }

    public static Vector3 operator -(Vector3 value)
    {
        value = new Vector3(-value.X, -value.Y, -value.Z);
        return value;
    }

    public static Vector3 operator -(Vector3 value1, Vector3 value2)
    {
        value1.X -= value2.X;
        value1.Y -= value2.Y;
        value1.Z -= value2.Z;
        return value1;
    }

    public static Vector3 operator *(Vector3 value1, Vector3 value2)
    {
        value1.X *= value2.X;
        value1.Y *= value2.Y;
        value1.Z *= value2.Z;
        return value1;
    }

    public static Vector3 operator *(Vector3 value, float scaleFactor)
    {
        value.X *= scaleFactor;
        value.Y *= scaleFactor;
        value.Z *= scaleFactor;
        return value;
    }

    public static Vector3 operator *(float scaleFactor, Vector3 value)
    {
        value.X *= scaleFactor;
        value.Y *= scaleFactor;
        value.Z *= scaleFactor;
        return value;
    }

    public static Vector3 operator /(Vector3 value1, Vector3 value2)
    {
        value1.X /= value2.X;
        value1.Y /= value2.Y;
        value1.Z /= value2.Z;
        return value1;
    }

    public static Vector3 operator /(Vector3 value, float divider)
    {
        var factor = 1 / divider;
        value.X *= factor;
        value.Y *= factor;
        value.Z *= factor;
        return value;
    }

    public static explicit operator SFML.System.Vector3f(Vector3 vector)
    {
        return new SFML.System.Vector3f(vector.X, vector.Y, vector.Z);
    }

    public static explicit operator Vector3(SFML.System.Vector3f vector)
    {
        return new Vector3(vector.X, vector.Y, vector.Z);
    }

    #endregion

}
