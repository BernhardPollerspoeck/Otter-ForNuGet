#region License
/*
MIT License
Copyright Â© 2006 The Mono.Xna Team

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

namespace Otter.Utility.MonoGame;

/// <summary>
/// Struct for representing a Matrix.
/// </summary>
public struct Matrix(float m11, float m12, float m13, float m14, float m21, float m22, float m23, float m24, float m31,
              float m32, float m33, float m34, float m41, float m42, float m43, float m44) : IEquatable<Matrix>
{

    #region Public Constructors

    #endregion Public Constructors

    #region Public Fields

    public float M11 = m11;

    public float M12 = m12;

    public float M13 = m13;

    public float M14 = m14;

    public float M21 = m21;

    public float M22 = m22;

    public float M23 = m23;

    public float M24 = m24;

    public float M31 = m31;

    public float M32 = m32;

    public float M33 = m33;

    public float M34 = m34;

    public float M41 = m41;

    public float M42 = m42;

    public float M43 = m43;

    public float M44 = m44;

    #endregion Public Fields

    #region Indexers

    public float this[int index]
    {
        readonly get => index switch
        {
            0 => M11,
            1 => M12,
            2 => M13,
            3 => M14,
            4 => M21,
            5 => M22,
            6 => M23,
            7 => M24,
            8 => M31,
            9 => M32,
            10 => M33,
            11 => M34,
            12 => M41,
            13 => M42,
            14 => M43,
            15 => M44,
            _ => throw new Exception(),
        };

        set
        {
            switch (index)
            {
                case 0: M11 = value; break;
                case 1: M12 = value; break;
                case 2: M13 = value; break;
                case 3: M14 = value; break;
                case 4: M21 = value; break;
                case 5: M22 = value; break;
                case 6: M23 = value; break;
                case 7: M24 = value; break;
                case 8: M31 = value; break;
                case 9: M32 = value; break;
                case 10: M33 = value; break;
                case 11: M34 = value; break;
                case 12: M41 = value; break;
                case 13: M42 = value; break;
                case 14: M43 = value; break;
                case 15: M44 = value; break;
                default: throw new Exception();
            }
        }
    }

    public float this[int row, int column]
    {
        readonly get => this[(row * 4) + column];

        set => this[(row * 4) + column] = value;
    }

    #endregion

    #region Private Members
    private static Matrix identity = new(1f, 0f, 0f, 0f,
                                                0f, 1f, 0f, 0f,
                                                0f, 0f, 1f, 0f,
                                                0f, 0f, 0f, 1f);
    #endregion Private Members

    #region Public Properties

    public Vector3 Backward
    {
        readonly get => new(M31, M32, M33);
        set
        {
            M31 = value.X;
            M32 = value.Y;
            M33 = value.Z;
        }
    }


    public Vector3 Down
    {
        readonly get => new(-M21, -M22, -M23);
        set
        {
            M21 = -value.X;
            M22 = -value.Y;
            M23 = -value.Z;
        }
    }


    public Vector3 Forward
    {
        readonly get => new(-M31, -M32, -M33);
        set
        {
            M31 = -value.X;
            M32 = -value.Y;
            M33 = -value.Z;
        }
    }


    public static Matrix Identity => identity;


    // required for OpenGL 2.0 projection matrix stuff
    // TODO: have this work correctly for 3x3 Matrices. Needs to return
    // a float[9] for a 3x3, and a float[16] for a 4x4
    public static float[] ToFloatArray(Matrix mat)
    {
        float[] matarray = [
                                mat.M11, mat.M12, mat.M13, mat.M14,
                                mat.M21, mat.M22, mat.M23, mat.M24,
                                mat.M31, mat.M32, mat.M33, mat.M34,
                                mat.M41, mat.M42, mat.M43, mat.M44
                            ];
        return matarray;
    }

    public Vector3 Left
    {
        readonly get => new(-M11, -M12, -M13);
        set
        {
            M11 = -value.X;
            M12 = -value.Y;
            M13 = -value.Z;
        }
    }


    public Vector3 Right
    {
        readonly get => new(M11, M12, M13);
        set
        {
            M11 = value.X;
            M12 = value.Y;
            M13 = value.Z;
        }
    }


    public Vector3 Translation
    {
        readonly get => new(M41, M42, M43);
        set
        {
            M41 = value.X;
            M42 = value.Y;
            M43 = value.Z;
        }
    }


    public Vector3 Up
    {
        readonly get => new(M21, M22, M23);
        set
        {
            M21 = value.X;
            M22 = value.Y;
            M23 = value.Z;
        }
    }
    #endregion Public Properties

    #region Public Methods

    public static Matrix Add(Matrix matrix1, Matrix matrix2)
    {
        matrix1.M11 += matrix2.M11;
        matrix1.M12 += matrix2.M12;
        matrix1.M13 += matrix2.M13;
        matrix1.M14 += matrix2.M14;
        matrix1.M21 += matrix2.M21;
        matrix1.M22 += matrix2.M22;
        matrix1.M23 += matrix2.M23;
        matrix1.M24 += matrix2.M24;
        matrix1.M31 += matrix2.M31;
        matrix1.M32 += matrix2.M32;
        matrix1.M33 += matrix2.M33;
        matrix1.M34 += matrix2.M34;
        matrix1.M41 += matrix2.M41;
        matrix1.M42 += matrix2.M42;
        matrix1.M43 += matrix2.M43;
        matrix1.M44 += matrix2.M44;
        return matrix1;
    }


    public static void Add(ref Matrix matrix1, ref Matrix matrix2, out Matrix result)
    {
        result.M11 = matrix1.M11 + matrix2.M11;
        result.M12 = matrix1.M12 + matrix2.M12;
        result.M13 = matrix1.M13 + matrix2.M13;
        result.M14 = matrix1.M14 + matrix2.M14;
        result.M21 = matrix1.M21 + matrix2.M21;
        result.M22 = matrix1.M22 + matrix2.M22;
        result.M23 = matrix1.M23 + matrix2.M23;
        result.M24 = matrix1.M24 + matrix2.M24;
        result.M31 = matrix1.M31 + matrix2.M31;
        result.M32 = matrix1.M32 + matrix2.M32;
        result.M33 = matrix1.M33 + matrix2.M33;
        result.M34 = matrix1.M34 + matrix2.M34;
        result.M41 = matrix1.M41 + matrix2.M41;
        result.M42 = matrix1.M42 + matrix2.M42;
        result.M43 = matrix1.M43 + matrix2.M43;
        result.M44 = matrix1.M44 + matrix2.M44;

    }


    public static Matrix CreateBillboard(Vector3 objectPosition, Vector3 cameraPosition,
        Vector3 cameraUpVector, Nullable<Vector3> cameraForwardVector)
    {

        // Delegate to the other overload of the function to do the work
        CreateBillboard(ref objectPosition, ref cameraPosition, ref cameraUpVector, cameraForwardVector, out Matrix result);

        return result;
    }


    public static void CreateBillboard(ref Vector3 objectPosition, ref Vector3 cameraPosition,
        ref Vector3 cameraUpVector, Vector3? cameraForwardVector, out Matrix result)
    {
        Vector3 vector;
        vector.X = objectPosition.X - cameraPosition.X;
        vector.Y = objectPosition.Y - cameraPosition.Y;
        vector.Z = objectPosition.Z - cameraPosition.Z;
        var num = vector.LengthSquared();
        if (num < 0.0001f)
        {
            vector = cameraForwardVector.HasValue ? -cameraForwardVector.Value : Vector3.Forward;
        }
        else
        {
            Vector3.Multiply(ref vector, (float)(1f / ((float)Math.Sqrt((double)num))), out vector);
        }
        Vector3.Cross(ref cameraUpVector, ref vector, out Vector3 vector3);
        vector3.Normalize();
        Vector3.Cross(ref vector, ref vector3, out Vector3 vector2);
        result.M11 = vector3.X;
        result.M12 = vector3.Y;
        result.M13 = vector3.Z;
        result.M14 = 0;
        result.M21 = vector2.X;
        result.M22 = vector2.Y;
        result.M23 = vector2.Z;
        result.M24 = 0;
        result.M31 = vector.X;
        result.M32 = vector.Y;
        result.M33 = vector.Z;
        result.M34 = 0;
        result.M41 = objectPosition.X;
        result.M42 = objectPosition.Y;
        result.M43 = objectPosition.Z;
        result.M44 = 1;
    }


    public static Matrix CreateConstrainedBillboard(Vector3 objectPosition, Vector3 cameraPosition,
        Vector3 rotateAxis, Nullable<Vector3> cameraForwardVector, Nullable<Vector3> objectForwardVector)
    {
        CreateConstrainedBillboard(ref objectPosition, ref cameraPosition, ref rotateAxis,
            cameraForwardVector, objectForwardVector, out Matrix result);
        return result;
    }


    public static void CreateConstrainedBillboard(ref Vector3 objectPosition, ref Vector3 cameraPosition,
        ref Vector3 rotateAxis, Vector3? cameraForwardVector, Vector3? objectForwardVector, out Matrix result)
    {
        Vector3 vector;
        Vector3 vector2;
        Vector3 vector3;
        vector2.X = objectPosition.X - cameraPosition.X;
        vector2.Y = objectPosition.Y - cameraPosition.Y;
        vector2.Z = objectPosition.Z - cameraPosition.Z;
        var num2 = vector2.LengthSquared();
        if (num2 < 0.0001f)
        {
            vector2 = cameraForwardVector.HasValue ? -cameraForwardVector.Value : Vector3.Forward;
        }
        else
        {
            Vector3.Multiply(ref vector2, (float)(1f / ((float)Math.Sqrt((double)num2))), out vector2);
        }
        Vector3 vector4 = rotateAxis;
        Vector3.Dot(ref rotateAxis, ref vector2, out var num);
        if (Math.Abs(num) > 0.9982547f)
        {
            if (objectForwardVector.HasValue)
            {
                vector = objectForwardVector.Value;
                Vector3.Dot(ref rotateAxis, ref vector, out num);
                if (Math.Abs(num) > 0.9982547f)
                {
                    num = (rotateAxis.X * Vector3.Forward.X) + (rotateAxis.Y * Vector3.Forward.Y) + (rotateAxis.Z * Vector3.Forward.Z);
                    vector = (Math.Abs(num) > 0.9982547f) ? Vector3.Right : Vector3.Forward;
                }
            }
            else
            {
                num = (rotateAxis.X * Vector3.Forward.X) + (rotateAxis.Y * Vector3.Forward.Y) + (rotateAxis.Z * Vector3.Forward.Z);
                vector = (Math.Abs(num) > 0.9982547f) ? Vector3.Right : Vector3.Forward;
            }
            Vector3.Cross(ref rotateAxis, ref vector, out vector3);
            vector3.Normalize();
            Vector3.Cross(ref vector3, ref rotateAxis, out vector);
            vector.Normalize();
        }
        else
        {
            Vector3.Cross(ref rotateAxis, ref vector2, out vector3);
            vector3.Normalize();
            Vector3.Cross(ref vector3, ref vector4, out vector);
            vector.Normalize();
        }
        result.M11 = vector3.X;
        result.M12 = vector3.Y;
        result.M13 = vector3.Z;
        result.M14 = 0;
        result.M21 = vector4.X;
        result.M22 = vector4.Y;
        result.M23 = vector4.Z;
        result.M24 = 0;
        result.M31 = vector.X;
        result.M32 = vector.Y;
        result.M33 = vector.Z;
        result.M34 = 0;
        result.M41 = objectPosition.X;
        result.M42 = objectPosition.Y;
        result.M43 = objectPosition.Z;
        result.M44 = 1;

    }


    public static Matrix CreateFromAxisAngle(Vector3 axis, float angle)
    {
        CreateFromAxisAngle(ref axis, angle, out Matrix result);
        return result;
    }


    public static void CreateFromAxisAngle(ref Vector3 axis, float angle, out Matrix result)
    {
        var x = axis.X;
        var y = axis.Y;
        var z = axis.Z;
        var num2 = (float)Math.Sin((double)angle);
        var num = (float)Math.Cos((double)angle);
        var num11 = x * x;
        var num10 = y * y;
        var num9 = z * z;
        var num8 = x * y;
        var num7 = x * z;
        var num6 = y * z;
        result.M11 = num11 + (num * (1f - num11));
        result.M12 = num8 - (num * num8) + (num2 * z);
        result.M13 = num7 - (num * num7) - (num2 * y);
        result.M14 = 0;
        result.M21 = num8 - (num * num8) - (num2 * z);
        result.M22 = num10 + (num * (1f - num10));
        result.M23 = num6 - (num * num6) + (num2 * x);
        result.M24 = 0;
        result.M31 = num7 - (num * num7) + (num2 * y);
        result.M32 = num6 - (num * num6) - (num2 * x);
        result.M33 = num9 + (num * (1f - num9));
        result.M34 = 0;
        result.M41 = 0;
        result.M42 = 0;
        result.M43 = 0;
        result.M44 = 1;
    }


    public static Matrix CreateFromQuaternion(Quaternion quaternion)
    {
        CreateFromQuaternion(ref quaternion, out Matrix result);
        return result;
    }


    public static void CreateFromQuaternion(ref Quaternion quaternion, out Matrix result)
    {
        var num9 = quaternion.X * quaternion.X;
        var num8 = quaternion.Y * quaternion.Y;
        var num7 = quaternion.Z * quaternion.Z;
        var num6 = quaternion.X * quaternion.Y;
        var num5 = quaternion.Z * quaternion.W;
        var num4 = quaternion.Z * quaternion.X;
        var num3 = quaternion.Y * quaternion.W;
        var num2 = quaternion.Y * quaternion.Z;
        var num = quaternion.X * quaternion.W;
        result.M11 = 1f - (2f * (num8 + num7));
        result.M12 = 2f * (num6 + num5);
        result.M13 = 2f * (num4 - num3);
        result.M14 = 0f;
        result.M21 = 2f * (num6 - num5);
        result.M22 = 1f - (2f * (num7 + num9));
        result.M23 = 2f * (num2 + num);
        result.M24 = 0f;
        result.M31 = 2f * (num4 + num3);
        result.M32 = 2f * (num2 - num);
        result.M33 = 1f - (2f * (num8 + num9));
        result.M34 = 0f;
        result.M41 = 0f;
        result.M42 = 0f;
        result.M43 = 0f;
        result.M44 = 1f;
    }

    public static Matrix CreateFromYawPitchRoll(float yaw, float pitch, float roll)
    {
        CreateFromYawPitchRoll(yaw, pitch, roll, out Matrix matrix);
        return matrix;
    }

    public static void CreateFromYawPitchRoll(float yaw, float pitch, float roll, out Matrix result)
    {
        Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll, out Quaternion quaternion);
        CreateFromQuaternion(ref quaternion, out result);
    }

    public static Matrix CreateLookAt(Vector3 cameraPosition, Vector3 cameraTarget, Vector3 cameraUpVector)
    {
        CreateLookAt(ref cameraPosition, ref cameraTarget, ref cameraUpVector, out Matrix matrix);
        return matrix;
    }

    public static void CreateLookAt(ref Vector3 cameraPosition, ref Vector3 cameraTarget, ref Vector3 cameraUpVector, out Matrix result)
    {
        var vector = Vector3.Normalize(cameraPosition - cameraTarget);
        var vector2 = Vector3.Normalize(Vector3.Cross(cameraUpVector, vector));
        var vector3 = Vector3.Cross(vector, vector2);
        result.M11 = vector2.X;
        result.M12 = vector3.X;
        result.M13 = vector.X;
        result.M14 = 0f;
        result.M21 = vector2.Y;
        result.M22 = vector3.Y;
        result.M23 = vector.Y;
        result.M24 = 0f;
        result.M31 = vector2.Z;
        result.M32 = vector3.Z;
        result.M33 = vector.Z;
        result.M34 = 0f;
        result.M41 = -Vector3.Dot(vector2, cameraPosition);
        result.M42 = -Vector3.Dot(vector3, cameraPosition);
        result.M43 = -Vector3.Dot(vector, cameraPosition);
        result.M44 = 1f;
    }


    public static Matrix CreateOrthographic(float width, float height, float zNearPlane, float zFarPlane)
    {
        CreateOrthographic(width, height, zNearPlane, zFarPlane, out Matrix matrix);
        return matrix;
    }


    public static void CreateOrthographic(float width, float height, float zNearPlane, float zFarPlane, out Matrix result)
    {
        result.M11 = 2f / width;
        result.M12 = result.M13 = result.M14 = 0f;
        result.M22 = 2f / height;
        result.M21 = result.M23 = result.M24 = 0f;
        result.M33 = 1f / (zNearPlane - zFarPlane);
        result.M31 = result.M32 = result.M34 = 0f;
        result.M41 = result.M42 = 0f;
        result.M43 = zNearPlane / (zNearPlane - zFarPlane);
        result.M44 = 1f;
    }


    public static Matrix CreateOrthographicOffCenter(float left, float right, float bottom, float top, float zNearPlane, float zFarPlane)
    {
        CreateOrthographicOffCenter(left, right, bottom, top, zNearPlane, zFarPlane, out Matrix matrix);
        return matrix;
    }


    public static void CreateOrthographicOffCenter(float left, float right, float bottom, float top, float zNearPlane, float zFarPlane, out Matrix result)
    {
        result.M11 = (float)(2.0 / ((double)right - (double)left));
        result.M12 = 0.0f;
        result.M13 = 0.0f;
        result.M14 = 0.0f;
        result.M21 = 0.0f;
        result.M22 = (float)(2.0 / ((double)top - (double)bottom));
        result.M23 = 0.0f;
        result.M24 = 0.0f;
        result.M31 = 0.0f;
        result.M32 = 0.0f;
        result.M33 = (float)(1.0 / ((double)zNearPlane - (double)zFarPlane));
        result.M34 = 0.0f;
        result.M41 = (float)(((double)left + (double)right) / ((double)left - (double)right));
        result.M42 = (float)(((double)top + (double)bottom) / ((double)bottom - (double)top));
        result.M43 = (float)((double)zNearPlane / ((double)zNearPlane - (double)zFarPlane));
        result.M44 = 1.0f;
    }


    public static Matrix CreatePerspective(float width, float height, float nearPlaneDistance, float farPlaneDistance)
    {
        CreatePerspective(width, height, nearPlaneDistance, farPlaneDistance, out Matrix matrix);
        return matrix;
    }


    public static void CreatePerspective(float width, float height, float nearPlaneDistance, float farPlaneDistance, out Matrix result)
    {
        if (nearPlaneDistance <= 0f)
        {
            throw new ArgumentException("nearPlaneDistance <= 0");
        }
        if (farPlaneDistance <= 0f)
        {
            throw new ArgumentException("farPlaneDistance <= 0");
        }
        if (nearPlaneDistance >= farPlaneDistance)
        {
            throw new ArgumentException("nearPlaneDistance >= farPlaneDistance");
        }
        result.M11 = 2f * nearPlaneDistance / width;
        result.M12 = result.M13 = result.M14 = 0f;
        result.M22 = 2f * nearPlaneDistance / height;
        result.M21 = result.M23 = result.M24 = 0f;
        result.M33 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
        result.M31 = result.M32 = 0f;
        result.M34 = -1f;
        result.M41 = result.M42 = result.M44 = 0f;
        result.M43 = nearPlaneDistance * farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
    }


    public static Matrix CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
    {
        CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance, out Matrix result);
        return result;
    }


    public static void CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance, out Matrix result)
    {
        if (fieldOfView is <= 0f or >= 3.141593f)
        {
            throw new ArgumentException("fieldOfView <= 0 or >= PI");
        }
        if (nearPlaneDistance <= 0f)
        {
            throw new ArgumentException("nearPlaneDistance <= 0");
        }
        if (farPlaneDistance <= 0f)
        {
            throw new ArgumentException("farPlaneDistance <= 0");
        }
        if (nearPlaneDistance >= farPlaneDistance)
        {
            throw new ArgumentException("nearPlaneDistance >= farPlaneDistance");
        }
        var num = 1f / ((float)Math.Tan((double)(fieldOfView * 0.5f)));
        var num9 = num / aspectRatio;
        result.M11 = num9;
        result.M12 = result.M13 = result.M14 = 0;
        result.M22 = num;
        result.M21 = result.M23 = result.M24 = 0;
        result.M31 = result.M32 = 0f;
        result.M33 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
        result.M34 = -1;
        result.M41 = result.M42 = result.M44 = 0;
        result.M43 = nearPlaneDistance * farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
    }


    public static Matrix CreatePerspectiveOffCenter(float left, float right, float bottom, float top, float nearPlaneDistance, float farPlaneDistance)
    {
        CreatePerspectiveOffCenter(left, right, bottom, top, nearPlaneDistance, farPlaneDistance, out Matrix result);
        return result;
    }


    public static void CreatePerspectiveOffCenter(float left, float right, float bottom, float top, float nearPlaneDistance, float farPlaneDistance, out Matrix result)
    {
        if (nearPlaneDistance <= 0f)
        {
            throw new ArgumentException("nearPlaneDistance <= 0");
        }
        if (farPlaneDistance <= 0f)
        {
            throw new ArgumentException("farPlaneDistance <= 0");
        }
        if (nearPlaneDistance >= farPlaneDistance)
        {
            throw new ArgumentException("nearPlaneDistance >= farPlaneDistance");
        }
        result.M11 = 2f * nearPlaneDistance / (right - left);
        result.M12 = result.M13 = result.M14 = 0;
        result.M22 = 2f * nearPlaneDistance / (top - bottom);
        result.M21 = result.M23 = result.M24 = 0;
        result.M31 = (left + right) / (right - left);
        result.M32 = (top + bottom) / (top - bottom);
        result.M33 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
        result.M34 = -1;
        result.M43 = nearPlaneDistance * farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
        result.M41 = result.M42 = result.M44 = 0;
    }


    public static Matrix CreateRotationX(float radians)
    {
        CreateRotationX(radians, out Matrix result);
        return result;
    }


    public static void CreateRotationX(float radians, out Matrix result)
    {
        result = Identity;

        var val1 = (float)Math.Cos(radians);
        var val2 = (float)Math.Sin(radians);

        result.M22 = val1;
        result.M23 = val2;
        result.M32 = -val2;
        result.M33 = val1;
    }

    public static Matrix CreateRotationY(float radians)
    {
        CreateRotationY(radians, out Matrix result);
        return result;
    }


    public static void CreateRotationY(float radians, out Matrix result)
    {
        result = Identity;

        var val1 = (float)Math.Cos(radians);
        var val2 = (float)Math.Sin(radians);

        result.M11 = val1;
        result.M13 = -val2;
        result.M31 = val2;
        result.M33 = val1;
    }


    public static Matrix CreateRotationZ(float radians)
    {
        CreateRotationZ(radians, out Matrix result);
        return result;
    }


    public static void CreateRotationZ(float radians, out Matrix result)
    {
        result = Identity;

        var val1 = (float)Math.Cos(radians);
        var val2 = (float)Math.Sin(radians);

        result.M11 = val1;
        result.M12 = val2;
        result.M21 = -val2;
        result.M22 = val1;
    }


    public static Matrix CreateScale(float scale)
    {
        CreateScale(scale, scale, scale, out Matrix result);
        return result;
    }


    public static void CreateScale(float scale, out Matrix result)
    {
        CreateScale(scale, scale, scale, out result);
    }


    public static Matrix CreateScale(float xScale, float yScale, float zScale)
    {
        CreateScale(xScale, yScale, zScale, out Matrix result);
        return result;
    }


    public static void CreateScale(float xScale, float yScale, float zScale, out Matrix result)
    {
        result.M11 = xScale;
        result.M12 = 0;
        result.M13 = 0;
        result.M14 = 0;
        result.M21 = 0;
        result.M22 = yScale;
        result.M23 = 0;
        result.M24 = 0;
        result.M31 = 0;
        result.M32 = 0;
        result.M33 = zScale;
        result.M34 = 0;
        result.M41 = 0;
        result.M42 = 0;
        result.M43 = 0;
        result.M44 = 1;
    }


    public static Matrix CreateScale(Vector3 scales)
    {
        CreateScale(ref scales, out Matrix result);
        return result;
    }


    public static void CreateScale(ref Vector3 scales, out Matrix result)
    {
        result.M11 = scales.X;
        result.M12 = 0;
        result.M13 = 0;
        result.M14 = 0;
        result.M21 = 0;
        result.M22 = scales.Y;
        result.M23 = 0;
        result.M24 = 0;
        result.M31 = 0;
        result.M32 = 0;
        result.M33 = scales.Z;
        result.M34 = 0;
        result.M41 = 0;
        result.M42 = 0;
        result.M43 = 0;
        result.M44 = 1;
    }

    /*
    /// <summary>
    /// Creates a Matrix that flattens geometry into a specified Plane as if casting a shadow from a specified light source.
    /// </summary>
    /// <param name="lightDirection">A Vector3 specifying the direction from which the light that will cast the shadow is coming.</param>
    /// <param name="plane">The Plane onto which the new matrix should flatten geometry so as to cast a shadow.</param>
    /// <returns>A Matrix that can be used to flatten geometry onto the specified plane from the specified direction. </returns>
    public static Matrix CreateShadow(Vector3 lightDirection, Plane plane) {
        Matrix result;
        CreateShadow(ref lightDirection, ref plane, out result);
        return result;
    }
    */

    /*
    /// <summary>
    /// Creates a Matrix that flattens geometry into a specified Plane as if casting a shadow from a specified light source.
    /// </summary>
    /// <param name="lightDirection">A Vector3 specifying the direction from which the light that will cast the shadow is coming.</param>
    /// <param name="plane">The Plane onto which the new matrix should flatten geometry so as to cast a shadow.</param>
    /// <param name="result">A Matrix that can be used to flatten geometry onto the specified plane from the specified direction. </param>
    public static void CreateShadow(ref Vector3 lightDirection, ref Plane plane, out Matrix result) {
        float dot = (plane.Normal.X * lightDirection.X) + (plane.Normal.Y * lightDirection.Y) + (plane.Normal.Z * lightDirection.Z);
        float x = -plane.Normal.X;
        float y = -plane.Normal.Y;
        float z = -plane.Normal.Z;
        float d = -plane.D;

        result.M11 = (x * lightDirection.X) + dot;
        result.M12 = x * lightDirection.Y;
        result.M13 = x * lightDirection.Z;
        result.M14 = 0;
        result.M21 = y * lightDirection.X;
        result.M22 = (y * lightDirection.Y) + dot;
        result.M23 = y * lightDirection.Z;
        result.M24 = 0;
        result.M31 = z * lightDirection.X;
        result.M32 = z * lightDirection.Y;
        result.M33 = (z * lightDirection.Z) + dot;
        result.M34 = 0;
        result.M41 = d * lightDirection.X;
        result.M42 = d * lightDirection.Y;
        result.M43 = d * lightDirection.Z;
        result.M44 = dot;
    }
    */

    public static Matrix CreateTranslation(float xPosition, float yPosition, float zPosition)
    {
        CreateTranslation(xPosition, yPosition, zPosition, out Matrix result);
        return result;
    }


    public static void CreateTranslation(ref Vector3 position, out Matrix result)
    {
        result.M11 = 1;
        result.M12 = 0;
        result.M13 = 0;
        result.M14 = 0;
        result.M21 = 0;
        result.M22 = 1;
        result.M23 = 0;
        result.M24 = 0;
        result.M31 = 0;
        result.M32 = 0;
        result.M33 = 1;
        result.M34 = 0;
        result.M41 = position.X;
        result.M42 = position.Y;
        result.M43 = position.Z;
        result.M44 = 1;
    }


    public static Matrix CreateTranslation(Vector3 position)
    {
        CreateTranslation(ref position, out Matrix result);
        return result;
    }


    public static void CreateTranslation(float xPosition, float yPosition, float zPosition, out Matrix result)
    {
        result.M11 = 1;
        result.M12 = 0;
        result.M13 = 0;
        result.M14 = 0;
        result.M21 = 0;
        result.M22 = 1;
        result.M23 = 0;
        result.M24 = 0;
        result.M31 = 0;
        result.M32 = 0;
        result.M33 = 1;
        result.M34 = 0;
        result.M41 = xPosition;
        result.M42 = yPosition;
        result.M43 = zPosition;
        result.M44 = 1;
    }

    /*
    public static Matrix CreateReflection(Plane value) {
        Matrix result;
        CreateReflection(ref value, out result);
        return result;
    }
    */

    /*
    public static void CreateReflection(ref Plane value, out Matrix result) {
        Plane plane;
        Plane.Normalize(ref value, out plane);
        value.Normalize();
        float x = plane.Normal.X;
        float y = plane.Normal.Y;
        float z = plane.Normal.Z;
        float num3 = -2f * x;
        float num2 = -2f * y;
        float num = -2f * z;
        result.M11 = (num3 * x) + 1f;
        result.M12 = num2 * x;
        result.M13 = num * x;
        result.M14 = 0;
        result.M21 = num3 * y;
        result.M22 = (num2 * y) + 1;
        result.M23 = num * y;
        result.M24 = 0;
        result.M31 = num3 * z;
        result.M32 = num2 * z;
        result.M33 = (num * z) + 1;
        result.M34 = 0;
        result.M41 = num3 * plane.D;
        result.M42 = num2 * plane.D;
        result.M43 = num * plane.D;
        result.M44 = 1;
    }
    */

    public static Matrix CreateWorld(Vector3 position, Vector3 forward, Vector3 up)
    {
        CreateWorld(ref position, ref forward, ref up, out Matrix ret);
        return ret;
    }

    public static void CreateWorld(ref Vector3 position, ref Vector3 forward, ref Vector3 up, out Matrix result)
    {
        Vector3.Normalize(ref forward, out Vector3 z);
        Vector3.Cross(ref forward, ref up, out Vector3 x);
        Vector3.Cross(ref x, ref forward, out Vector3 y);
        x.Normalize();
        y.Normalize();

        result = new Matrix
        {
            Right = x,
            Up = y,
            Forward = z,
            Translation = position,
            M44 = 1f
        };
    }

    public readonly bool Decompose(out Vector3 scale, out Quaternion rotation, out Vector3 translation)
    {
        translation.X = M41;
        translation.Y = M42;
        translation.Z = M43;

        float xs = (Math.Sign(M11 * M12 * M13 * M14) < 0) ? -1 : 1;
        float ys = (Math.Sign(M21 * M22 * M23 * M24) < 0) ? -1 : 1;
        float zs = (Math.Sign(M31 * M32 * M33 * M34) < 0) ? -1 : 1;

        scale.X = xs * (float)Math.Sqrt((M11 * M11) + (M12 * M12) + (M13 * M13));
        scale.Y = ys * (float)Math.Sqrt((M21 * M21) + (M22 * M22) + (M23 * M23));
        scale.Z = zs * (float)Math.Sqrt((M31 * M31) + (M32 * M32) + (M33 * M33));

        if (scale.X == 0.0 || scale.Y == 0.0 || scale.Z == 0.0)
        {
            rotation = Quaternion.Identity;
            return false;
        }

        Matrix m1 = new(M11 / scale.X, M12 / scale.X, M13 / scale.X, 0,
                               M21 / scale.Y, M22 / scale.Y, M23 / scale.Y, 0,
                               M31 / scale.Z, M32 / scale.Z, M33 / scale.Z, 0,
                               0, 0, 0, 1);

        rotation = Quaternion.CreateFromRotationMatrix(m1);
        return true;
    }

    public readonly float Determinant()
    {
        var num22 = M11;
        var num21 = M12;
        var num20 = M13;
        var num19 = M14;
        var num12 = M21;
        var num11 = M22;
        var num10 = M23;
        var num9 = M24;
        var num8 = M31;
        var num7 = M32;
        var num6 = M33;
        var num5 = M34;
        var num4 = M41;
        var num3 = M42;
        var num2 = M43;
        var num = M44;
        var num18 = (num6 * num) - (num5 * num2);
        var num17 = (num7 * num) - (num5 * num3);
        var num16 = (num7 * num2) - (num6 * num3);
        var num15 = (num8 * num) - (num5 * num4);
        var num14 = (num8 * num2) - (num6 * num4);
        var num13 = (num8 * num3) - (num7 * num4);
        return (num22 * ((num11 * num18) - (num10 * num17) + (num9 * num16))) - (num21 * ((num12 * num18) - (num10 * num15) + (num9 * num14))) + (num20 * ((num12 * num17) - (num11 * num15) + (num9 * num13))) - (num19 * ((num12 * num16) - (num11 * num14) + (num10 * num13)));
    }


    public static Matrix Divide(Matrix matrix1, Matrix matrix2)
    {
        matrix1.M11 /= matrix2.M11;
        matrix1.M12 /= matrix2.M12;
        matrix1.M13 /= matrix2.M13;
        matrix1.M14 /= matrix2.M14;
        matrix1.M21 /= matrix2.M21;
        matrix1.M22 /= matrix2.M22;
        matrix1.M23 /= matrix2.M23;
        matrix1.M24 /= matrix2.M24;
        matrix1.M31 /= matrix2.M31;
        matrix1.M32 /= matrix2.M32;
        matrix1.M33 /= matrix2.M33;
        matrix1.M34 /= matrix2.M34;
        matrix1.M41 /= matrix2.M41;
        matrix1.M42 /= matrix2.M42;
        matrix1.M43 /= matrix2.M43;
        matrix1.M44 /= matrix2.M44;
        return matrix1;
    }


    public static void Divide(ref Matrix matrix1, ref Matrix matrix2, out Matrix result)
    {
        result.M11 = matrix1.M11 / matrix2.M11;
        result.M12 = matrix1.M12 / matrix2.M12;
        result.M13 = matrix1.M13 / matrix2.M13;
        result.M14 = matrix1.M14 / matrix2.M14;
        result.M21 = matrix1.M21 / matrix2.M21;
        result.M22 = matrix1.M22 / matrix2.M22;
        result.M23 = matrix1.M23 / matrix2.M23;
        result.M24 = matrix1.M24 / matrix2.M24;
        result.M31 = matrix1.M31 / matrix2.M31;
        result.M32 = matrix1.M32 / matrix2.M32;
        result.M33 = matrix1.M33 / matrix2.M33;
        result.M34 = matrix1.M34 / matrix2.M34;
        result.M41 = matrix1.M41 / matrix2.M41;
        result.M42 = matrix1.M42 / matrix2.M42;
        result.M43 = matrix1.M43 / matrix2.M43;
        result.M44 = matrix1.M44 / matrix2.M44;
    }


    public static Matrix Divide(Matrix matrix1, float divider)
    {
        var num = 1f / divider;
        matrix1.M11 *= num;
        matrix1.M12 *= num;
        matrix1.M13 *= num;
        matrix1.M14 *= num;
        matrix1.M21 *= num;
        matrix1.M22 *= num;
        matrix1.M23 *= num;
        matrix1.M24 *= num;
        matrix1.M31 *= num;
        matrix1.M32 *= num;
        matrix1.M33 *= num;
        matrix1.M34 *= num;
        matrix1.M41 *= num;
        matrix1.M42 *= num;
        matrix1.M43 *= num;
        matrix1.M44 *= num;
        return matrix1;
    }


    public static void Divide(ref Matrix matrix1, float divider, out Matrix result)
    {
        var num = 1f / divider;
        result.M11 = matrix1.M11 * num;
        result.M12 = matrix1.M12 * num;
        result.M13 = matrix1.M13 * num;
        result.M14 = matrix1.M14 * num;
        result.M21 = matrix1.M21 * num;
        result.M22 = matrix1.M22 * num;
        result.M23 = matrix1.M23 * num;
        result.M24 = matrix1.M24 * num;
        result.M31 = matrix1.M31 * num;
        result.M32 = matrix1.M32 * num;
        result.M33 = matrix1.M33 * num;
        result.M34 = matrix1.M34 * num;
        result.M41 = matrix1.M41 * num;
        result.M42 = matrix1.M42 * num;
        result.M43 = matrix1.M43 * num;
        result.M44 = matrix1.M44 * num;
    }


    public readonly bool Equals(Matrix other)
    {
        return (M11 == other.M11) && (M22 == other.M22) && (M33 == other.M33) && (M44 == other.M44) && (M12 == other.M12) && (M13 == other.M13) && (M14 == other.M14) && (M21 == other.M21) && (M23 == other.M23) && (M24 == other.M24) && (M31 == other.M31) && (M32 == other.M32) && (M34 == other.M34) && (M41 == other.M41) && (M42 == other.M42) && (M43 == other.M43);
    }


    public override readonly bool Equals(object obj)
    {
        var flag = false;
        if (obj is Matrix matrix)
        {
            flag = Equals(matrix);
        }
        return flag;
    }


    public override readonly int GetHashCode()
    {
        return M11.GetHashCode() + M12.GetHashCode() + M13.GetHashCode() + M14.GetHashCode() + M21.GetHashCode() + M22.GetHashCode() + M23.GetHashCode() + M24.GetHashCode() + M31.GetHashCode() + M32.GetHashCode() + M33.GetHashCode() + M34.GetHashCode() + M41.GetHashCode() + M42.GetHashCode() + M43.GetHashCode() + M44.GetHashCode();
    }


    public static Matrix Invert(Matrix matrix)
    {
        Invert(ref matrix, out matrix);
        return matrix;
    }


    public static void Invert(ref Matrix matrix, out Matrix result)
    {
        var num1 = matrix.M11;
        var num2 = matrix.M12;
        var num3 = matrix.M13;
        var num4 = matrix.M14;
        var num5 = matrix.M21;
        var num6 = matrix.M22;
        var num7 = matrix.M23;
        var num8 = matrix.M24;
        var num9 = matrix.M31;
        var num10 = matrix.M32;
        var num11 = matrix.M33;
        var num12 = matrix.M34;
        var num13 = matrix.M41;
        var num14 = matrix.M42;
        var num15 = matrix.M43;
        var num16 = matrix.M44;
        var num17 = (float)(((double)num11 * (double)num16) - ((double)num12 * (double)num15));
        var num18 = (float)(((double)num10 * (double)num16) - ((double)num12 * (double)num14));
        var num19 = (float)(((double)num10 * (double)num15) - ((double)num11 * (double)num14));
        var num20 = (float)(((double)num9 * (double)num16) - ((double)num12 * (double)num13));
        var num21 = (float)(((double)num9 * (double)num15) - ((double)num11 * (double)num13));
        var num22 = (float)(((double)num9 * (double)num14) - ((double)num10 * (double)num13));
        var num23 = (float)(((double)num6 * (double)num17) - ((double)num7 * (double)num18) + ((double)num8 * (double)num19));
        var num24 = (float)-(((double)num5 * (double)num17) - ((double)num7 * (double)num20) + ((double)num8 * (double)num21));
        var num25 = (float)(((double)num5 * (double)num18) - ((double)num6 * (double)num20) + ((double)num8 * (double)num22));
        var num26 = (float)-(((double)num5 * (double)num19) - ((double)num6 * (double)num21) + ((double)num7 * (double)num22));
        var num27 = (float)(1.0 / (((double)num1 * (double)num23) + ((double)num2 * (double)num24) + ((double)num3 * (double)num25) + ((double)num4 * (double)num26)));

        result.M11 = num23 * num27;
        result.M21 = num24 * num27;
        result.M31 = num25 * num27;
        result.M41 = num26 * num27;
        result.M12 = (float)-(((double)num2 * (double)num17) - ((double)num3 * (double)num18) + ((double)num4 * (double)num19)) * num27;
        result.M22 = (float)(((double)num1 * (double)num17) - ((double)num3 * (double)num20) + ((double)num4 * (double)num21)) * num27;
        result.M32 = (float)-(((double)num1 * (double)num18) - ((double)num2 * (double)num20) + ((double)num4 * (double)num22)) * num27;
        result.M42 = (float)(((double)num1 * (double)num19) - ((double)num2 * (double)num21) + ((double)num3 * (double)num22)) * num27;
        var num28 = (float)(((double)num7 * (double)num16) - ((double)num8 * (double)num15));
        var num29 = (float)(((double)num6 * (double)num16) - ((double)num8 * (double)num14));
        var num30 = (float)(((double)num6 * (double)num15) - ((double)num7 * (double)num14));
        var num31 = (float)(((double)num5 * (double)num16) - ((double)num8 * (double)num13));
        var num32 = (float)(((double)num5 * (double)num15) - ((double)num7 * (double)num13));
        var num33 = (float)(((double)num5 * (double)num14) - ((double)num6 * (double)num13));
        result.M13 = (float)(((double)num2 * (double)num28) - ((double)num3 * (double)num29) + ((double)num4 * (double)num30)) * num27;
        result.M23 = (float)-(((double)num1 * (double)num28) - ((double)num3 * (double)num31) + ((double)num4 * (double)num32)) * num27;
        result.M33 = (float)(((double)num1 * (double)num29) - ((double)num2 * (double)num31) + ((double)num4 * (double)num33)) * num27;
        result.M43 = (float)-(((double)num1 * (double)num30) - ((double)num2 * (double)num32) + ((double)num3 * (double)num33)) * num27;
        var num34 = (float)(((double)num7 * (double)num12) - ((double)num8 * (double)num11));
        var num35 = (float)(((double)num6 * (double)num12) - ((double)num8 * (double)num10));
        var num36 = (float)(((double)num6 * (double)num11) - ((double)num7 * (double)num10));
        var num37 = (float)(((double)num5 * (double)num12) - ((double)num8 * (double)num9));
        var num38 = (float)(((double)num5 * (double)num11) - ((double)num7 * (double)num9));
        var num39 = (float)(((double)num5 * (double)num10) - ((double)num6 * (double)num9));
        result.M14 = (float)-(((double)num2 * (double)num34) - ((double)num3 * (double)num35) + ((double)num4 * (double)num36)) * num27;
        result.M24 = (float)(((double)num1 * (double)num34) - ((double)num3 * (double)num37) + ((double)num4 * (double)num38)) * num27;
        result.M34 = (float)-(((double)num1 * (double)num35) - ((double)num2 * (double)num37) + ((double)num4 * (double)num39)) * num27;
        result.M44 = (float)(((double)num1 * (double)num36) - ((double)num2 * (double)num38) + ((double)num3 * (double)num39)) * num27;


        /*


        ///
        // Use Laplace expansion theorem to calculate the inverse of a 4x4 matrix
        //
        // 1. Calculate the 2x2 determinants needed the 4x4 determinant based on the 2x2 determinants
        // 3. Create the adjugate matrix, which satisfies: A * adj(A) = det(A) * I
        // 4. Divide adjugate matrix with the determinant to find the inverse

        float det1, det2, det3, det4, det5, det6, det7, det8, det9, det10, det11, det12;
        float detMatrix;
        FindDeterminants(ref matrix, out detMatrix, out det1, out det2, out det3, out det4, out det5, out det6,
                         out det7, out det8, out det9, out det10, out det11, out det12);

        float invDetMatrix = 1f / detMatrix;

        Matrix ret; // Allow for matrix and result to point to the same structure

        ret.M11 = (matrix.M22*det12 - matrix.M23*det11 + matrix.M24*det10) * invDetMatrix;
        ret.M12 = (-matrix.M12*det12 + matrix.M13*det11 - matrix.M14*det10) * invDetMatrix;
        ret.M13 = (matrix.M42*det6 - matrix.M43*det5 + matrix.M44*det4) * invDetMatrix;
        ret.M14 = (-matrix.M32*det6 + matrix.M33*det5 - matrix.M34*det4) * invDetMatrix;
        ret.M21 = (-matrix.M21*det12 + matrix.M23*det9 - matrix.M24*det8) * invDetMatrix;
        ret.M22 = (matrix.M11*det12 - matrix.M13*det9 + matrix.M14*det8) * invDetMatrix;
        ret.M23 = (-matrix.M41*det6 + matrix.M43*det3 - matrix.M44*det2) * invDetMatrix;
        ret.M24 = (matrix.M31*det6 - matrix.M33*det3 + matrix.M34*det2) * invDetMatrix;
        ret.M31 = (matrix.M21*det11 - matrix.M22*det9 + matrix.M24*det7) * invDetMatrix;
        ret.M32 = (-matrix.M11*det11 + matrix.M12*det9 - matrix.M14*det7) * invDetMatrix;
        ret.M33 = (matrix.M41*det5 - matrix.M42*det3 + matrix.M44*det1) * invDetMatrix;
        ret.M34 = (-matrix.M31*det5 + matrix.M32*det3 - matrix.M34*det1) * invDetMatrix;
        ret.M41 = (-matrix.M21*det10 + matrix.M22*det8 - matrix.M23*det7) * invDetMatrix;
        ret.M42 = (matrix.M11*det10 - matrix.M12*det8 + matrix.M13*det7) * invDetMatrix;
        ret.M43 = (-matrix.M41*det4 + matrix.M42*det2 - matrix.M43*det1) * invDetMatrix;
        ret.M44 = (matrix.M31*det4 - matrix.M32*det2 + matrix.M33*det1) * invDetMatrix;

        result = ret;
        */
    }


    public static Matrix Lerp(Matrix matrix1, Matrix matrix2, float amount)
    {
        matrix1.M11 += ((matrix2.M11 - matrix1.M11) * amount);
        matrix1.M12 += ((matrix2.M12 - matrix1.M12) * amount);
        matrix1.M13 += ((matrix2.M13 - matrix1.M13) * amount);
        matrix1.M14 += ((matrix2.M14 - matrix1.M14) * amount);
        matrix1.M21 += ((matrix2.M21 - matrix1.M21) * amount);
        matrix1.M22 += ((matrix2.M22 - matrix1.M22) * amount);
        matrix1.M23 += ((matrix2.M23 - matrix1.M23) * amount);
        matrix1.M24 += ((matrix2.M24 - matrix1.M24) * amount);
        matrix1.M31 += ((matrix2.M31 - matrix1.M31) * amount);
        matrix1.M32 += ((matrix2.M32 - matrix1.M32) * amount);
        matrix1.M33 += ((matrix2.M33 - matrix1.M33) * amount);
        matrix1.M34 += ((matrix2.M34 - matrix1.M34) * amount);
        matrix1.M41 += ((matrix2.M41 - matrix1.M41) * amount);
        matrix1.M42 += ((matrix2.M42 - matrix1.M42) * amount);
        matrix1.M43 += ((matrix2.M43 - matrix1.M43) * amount);
        matrix1.M44 += ((matrix2.M44 - matrix1.M44) * amount);
        return matrix1;
    }


    public static void Lerp(ref Matrix matrix1, ref Matrix matrix2, float amount, out Matrix result)
    {
        result.M11 = matrix1.M11 + ((matrix2.M11 - matrix1.M11) * amount);
        result.M12 = matrix1.M12 + ((matrix2.M12 - matrix1.M12) * amount);
        result.M13 = matrix1.M13 + ((matrix2.M13 - matrix1.M13) * amount);
        result.M14 = matrix1.M14 + ((matrix2.M14 - matrix1.M14) * amount);
        result.M21 = matrix1.M21 + ((matrix2.M21 - matrix1.M21) * amount);
        result.M22 = matrix1.M22 + ((matrix2.M22 - matrix1.M22) * amount);
        result.M23 = matrix1.M23 + ((matrix2.M23 - matrix1.M23) * amount);
        result.M24 = matrix1.M24 + ((matrix2.M24 - matrix1.M24) * amount);
        result.M31 = matrix1.M31 + ((matrix2.M31 - matrix1.M31) * amount);
        result.M32 = matrix1.M32 + ((matrix2.M32 - matrix1.M32) * amount);
        result.M33 = matrix1.M33 + ((matrix2.M33 - matrix1.M33) * amount);
        result.M34 = matrix1.M34 + ((matrix2.M34 - matrix1.M34) * amount);
        result.M41 = matrix1.M41 + ((matrix2.M41 - matrix1.M41) * amount);
        result.M42 = matrix1.M42 + ((matrix2.M42 - matrix1.M42) * amount);
        result.M43 = matrix1.M43 + ((matrix2.M43 - matrix1.M43) * amount);
        result.M44 = matrix1.M44 + ((matrix2.M44 - matrix1.M44) * amount);
    }

    public static Matrix Multiply(Matrix matrix1, Matrix matrix2)
    {
        var m11 = (matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21) + (matrix1.M13 * matrix2.M31) + (matrix1.M14 * matrix2.M41);
        var m12 = (matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22) + (matrix1.M13 * matrix2.M32) + (matrix1.M14 * matrix2.M42);
        var m13 = (matrix1.M11 * matrix2.M13) + (matrix1.M12 * matrix2.M23) + (matrix1.M13 * matrix2.M33) + (matrix1.M14 * matrix2.M43);
        var m14 = (matrix1.M11 * matrix2.M14) + (matrix1.M12 * matrix2.M24) + (matrix1.M13 * matrix2.M34) + (matrix1.M14 * matrix2.M44);
        var m21 = (matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21) + (matrix1.M23 * matrix2.M31) + (matrix1.M24 * matrix2.M41);
        var m22 = (matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22) + (matrix1.M23 * matrix2.M32) + (matrix1.M24 * matrix2.M42);
        var m23 = (matrix1.M21 * matrix2.M13) + (matrix1.M22 * matrix2.M23) + (matrix1.M23 * matrix2.M33) + (matrix1.M24 * matrix2.M43);
        var m24 = (matrix1.M21 * matrix2.M14) + (matrix1.M22 * matrix2.M24) + (matrix1.M23 * matrix2.M34) + (matrix1.M24 * matrix2.M44);
        var m31 = (matrix1.M31 * matrix2.M11) + (matrix1.M32 * matrix2.M21) + (matrix1.M33 * matrix2.M31) + (matrix1.M34 * matrix2.M41);
        var m32 = (matrix1.M31 * matrix2.M12) + (matrix1.M32 * matrix2.M22) + (matrix1.M33 * matrix2.M32) + (matrix1.M34 * matrix2.M42);
        var m33 = (matrix1.M31 * matrix2.M13) + (matrix1.M32 * matrix2.M23) + (matrix1.M33 * matrix2.M33) + (matrix1.M34 * matrix2.M43);
        var m34 = (matrix1.M31 * matrix2.M14) + (matrix1.M32 * matrix2.M24) + (matrix1.M33 * matrix2.M34) + (matrix1.M34 * matrix2.M44);
        var m41 = (matrix1.M41 * matrix2.M11) + (matrix1.M42 * matrix2.M21) + (matrix1.M43 * matrix2.M31) + (matrix1.M44 * matrix2.M41);
        var m42 = (matrix1.M41 * matrix2.M12) + (matrix1.M42 * matrix2.M22) + (matrix1.M43 * matrix2.M32) + (matrix1.M44 * matrix2.M42);
        var m43 = (matrix1.M41 * matrix2.M13) + (matrix1.M42 * matrix2.M23) + (matrix1.M43 * matrix2.M33) + (matrix1.M44 * matrix2.M43);
        var m44 = (matrix1.M41 * matrix2.M14) + (matrix1.M42 * matrix2.M24) + (matrix1.M43 * matrix2.M34) + (matrix1.M44 * matrix2.M44);
        matrix1.M11 = m11;
        matrix1.M12 = m12;
        matrix1.M13 = m13;
        matrix1.M14 = m14;
        matrix1.M21 = m21;
        matrix1.M22 = m22;
        matrix1.M23 = m23;
        matrix1.M24 = m24;
        matrix1.M31 = m31;
        matrix1.M32 = m32;
        matrix1.M33 = m33;
        matrix1.M34 = m34;
        matrix1.M41 = m41;
        matrix1.M42 = m42;
        matrix1.M43 = m43;
        matrix1.M44 = m44;
        return matrix1;
    }


    public static void Multiply(ref Matrix matrix1, ref Matrix matrix2, out Matrix result)
    {
        var m11 = (matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21) + (matrix1.M13 * matrix2.M31) + (matrix1.M14 * matrix2.M41);
        var m12 = (matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22) + (matrix1.M13 * matrix2.M32) + (matrix1.M14 * matrix2.M42);
        var m13 = (matrix1.M11 * matrix2.M13) + (matrix1.M12 * matrix2.M23) + (matrix1.M13 * matrix2.M33) + (matrix1.M14 * matrix2.M43);
        var m14 = (matrix1.M11 * matrix2.M14) + (matrix1.M12 * matrix2.M24) + (matrix1.M13 * matrix2.M34) + (matrix1.M14 * matrix2.M44);
        var m21 = (matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21) + (matrix1.M23 * matrix2.M31) + (matrix1.M24 * matrix2.M41);
        var m22 = (matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22) + (matrix1.M23 * matrix2.M32) + (matrix1.M24 * matrix2.M42);
        var m23 = (matrix1.M21 * matrix2.M13) + (matrix1.M22 * matrix2.M23) + (matrix1.M23 * matrix2.M33) + (matrix1.M24 * matrix2.M43);
        var m24 = (matrix1.M21 * matrix2.M14) + (matrix1.M22 * matrix2.M24) + (matrix1.M23 * matrix2.M34) + (matrix1.M24 * matrix2.M44);
        var m31 = (matrix1.M31 * matrix2.M11) + (matrix1.M32 * matrix2.M21) + (matrix1.M33 * matrix2.M31) + (matrix1.M34 * matrix2.M41);
        var m32 = (matrix1.M31 * matrix2.M12) + (matrix1.M32 * matrix2.M22) + (matrix1.M33 * matrix2.M32) + (matrix1.M34 * matrix2.M42);
        var m33 = (matrix1.M31 * matrix2.M13) + (matrix1.M32 * matrix2.M23) + (matrix1.M33 * matrix2.M33) + (matrix1.M34 * matrix2.M43);
        var m34 = (matrix1.M31 * matrix2.M14) + (matrix1.M32 * matrix2.M24) + (matrix1.M33 * matrix2.M34) + (matrix1.M34 * matrix2.M44);
        var m41 = (matrix1.M41 * matrix2.M11) + (matrix1.M42 * matrix2.M21) + (matrix1.M43 * matrix2.M31) + (matrix1.M44 * matrix2.M41);
        var m42 = (matrix1.M41 * matrix2.M12) + (matrix1.M42 * matrix2.M22) + (matrix1.M43 * matrix2.M32) + (matrix1.M44 * matrix2.M42);
        var m43 = (matrix1.M41 * matrix2.M13) + (matrix1.M42 * matrix2.M23) + (matrix1.M43 * matrix2.M33) + (matrix1.M44 * matrix2.M43);
        var m44 = (matrix1.M41 * matrix2.M14) + (matrix1.M42 * matrix2.M24) + (matrix1.M43 * matrix2.M34) + (matrix1.M44 * matrix2.M44);
        result.M11 = m11;
        result.M12 = m12;
        result.M13 = m13;
        result.M14 = m14;
        result.M21 = m21;
        result.M22 = m22;
        result.M23 = m23;
        result.M24 = m24;
        result.M31 = m31;
        result.M32 = m32;
        result.M33 = m33;
        result.M34 = m34;
        result.M41 = m41;
        result.M42 = m42;
        result.M43 = m43;
        result.M44 = m44;
    }

    public static Matrix Multiply(Matrix matrix1, float factor)
    {
        matrix1.M11 *= factor;
        matrix1.M12 *= factor;
        matrix1.M13 *= factor;
        matrix1.M14 *= factor;
        matrix1.M21 *= factor;
        matrix1.M22 *= factor;
        matrix1.M23 *= factor;
        matrix1.M24 *= factor;
        matrix1.M31 *= factor;
        matrix1.M32 *= factor;
        matrix1.M33 *= factor;
        matrix1.M34 *= factor;
        matrix1.M41 *= factor;
        matrix1.M42 *= factor;
        matrix1.M43 *= factor;
        matrix1.M44 *= factor;
        return matrix1;
    }


    public static void Multiply(ref Matrix matrix1, float factor, out Matrix result)
    {
        result.M11 = matrix1.M11 * factor;
        result.M12 = matrix1.M12 * factor;
        result.M13 = matrix1.M13 * factor;
        result.M14 = matrix1.M14 * factor;
        result.M21 = matrix1.M21 * factor;
        result.M22 = matrix1.M22 * factor;
        result.M23 = matrix1.M23 * factor;
        result.M24 = matrix1.M24 * factor;
        result.M31 = matrix1.M31 * factor;
        result.M32 = matrix1.M32 * factor;
        result.M33 = matrix1.M33 * factor;
        result.M34 = matrix1.M34 * factor;
        result.M41 = matrix1.M41 * factor;
        result.M42 = matrix1.M42 * factor;
        result.M43 = matrix1.M43 * factor;
        result.M44 = matrix1.M44 * factor;

    }


    public static Matrix Negate(Matrix matrix)
    {
        matrix.M11 = -matrix.M11;
        matrix.M12 = -matrix.M12;
        matrix.M13 = -matrix.M13;
        matrix.M14 = -matrix.M14;
        matrix.M21 = -matrix.M21;
        matrix.M22 = -matrix.M22;
        matrix.M23 = -matrix.M23;
        matrix.M24 = -matrix.M24;
        matrix.M31 = -matrix.M31;
        matrix.M32 = -matrix.M32;
        matrix.M33 = -matrix.M33;
        matrix.M34 = -matrix.M34;
        matrix.M41 = -matrix.M41;
        matrix.M42 = -matrix.M42;
        matrix.M43 = -matrix.M43;
        matrix.M44 = -matrix.M44;
        return matrix;
    }


    public static void Negate(ref Matrix matrix, out Matrix result)
    {
        result.M11 = -matrix.M11;
        result.M12 = -matrix.M12;
        result.M13 = -matrix.M13;
        result.M14 = -matrix.M14;
        result.M21 = -matrix.M21;
        result.M22 = -matrix.M22;
        result.M23 = -matrix.M23;
        result.M24 = -matrix.M24;
        result.M31 = -matrix.M31;
        result.M32 = -matrix.M32;
        result.M33 = -matrix.M33;
        result.M34 = -matrix.M34;
        result.M41 = -matrix.M41;
        result.M42 = -matrix.M42;
        result.M43 = -matrix.M43;
        result.M44 = -matrix.M44;
    }


    public static Matrix operator +(Matrix matrix1, Matrix matrix2)
    {
        Add(ref matrix1, ref matrix2, out matrix1);
        return matrix1;
    }


    public static Matrix operator /(Matrix matrix1, Matrix matrix2)
    {
        matrix1.M11 /= matrix2.M11;
        matrix1.M12 /= matrix2.M12;
        matrix1.M13 /= matrix2.M13;
        matrix1.M14 /= matrix2.M14;
        matrix1.M21 /= matrix2.M21;
        matrix1.M22 /= matrix2.M22;
        matrix1.M23 /= matrix2.M23;
        matrix1.M24 /= matrix2.M24;
        matrix1.M31 /= matrix2.M31;
        matrix1.M32 /= matrix2.M32;
        matrix1.M33 /= matrix2.M33;
        matrix1.M34 /= matrix2.M34;
        matrix1.M41 /= matrix2.M41;
        matrix1.M42 /= matrix2.M42;
        matrix1.M43 /= matrix2.M43;
        matrix1.M44 /= matrix2.M44;
        return matrix1;
    }


    public static Matrix operator /(Matrix matrix, float divider)
    {
        var num = 1f / divider;
        matrix.M11 *= num;
        matrix.M12 *= num;
        matrix.M13 *= num;
        matrix.M14 *= num;
        matrix.M21 *= num;
        matrix.M22 *= num;
        matrix.M23 *= num;
        matrix.M24 *= num;
        matrix.M31 *= num;
        matrix.M32 *= num;
        matrix.M33 *= num;
        matrix.M34 *= num;
        matrix.M41 *= num;
        matrix.M42 *= num;
        matrix.M43 *= num;
        matrix.M44 *= num;
        return matrix;
    }


    public static bool operator ==(Matrix matrix1, Matrix matrix2)
    {
        return
            matrix1.M11 == matrix2.M11 &&
            matrix1.M12 == matrix2.M12 &&
            matrix1.M13 == matrix2.M13 &&
            matrix1.M14 == matrix2.M14 &&
            matrix1.M21 == matrix2.M21 &&
            matrix1.M22 == matrix2.M22 &&
            matrix1.M23 == matrix2.M23 &&
            matrix1.M24 == matrix2.M24 &&
            matrix1.M31 == matrix2.M31 &&
            matrix1.M32 == matrix2.M32 &&
            matrix1.M33 == matrix2.M33 &&
            matrix1.M34 == matrix2.M34 &&
            matrix1.M41 == matrix2.M41 &&
            matrix1.M42 == matrix2.M42 &&
            matrix1.M43 == matrix2.M43 &&
            matrix1.M44 == matrix2.M44
            ;
    }


    public static bool operator !=(Matrix matrix1, Matrix matrix2)
    {
        return
            matrix1.M11 != matrix2.M11 ||
            matrix1.M12 != matrix2.M12 ||
            matrix1.M13 != matrix2.M13 ||
            matrix1.M14 != matrix2.M14 ||
            matrix1.M21 != matrix2.M21 ||
            matrix1.M22 != matrix2.M22 ||
            matrix1.M23 != matrix2.M23 ||
            matrix1.M24 != matrix2.M24 ||
            matrix1.M31 != matrix2.M31 ||
            matrix1.M32 != matrix2.M32 ||
            matrix1.M33 != matrix2.M33 ||
            matrix1.M34 != matrix2.M34 ||
            matrix1.M41 != matrix2.M41 ||
            matrix1.M42 != matrix2.M42 ||
            matrix1.M43 != matrix2.M43 ||
            matrix1.M44 != matrix2.M44
            ;
    }


    public static Matrix operator *(Matrix matrix1, Matrix matrix2)
    {
        var m11 = (matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21) + (matrix1.M13 * matrix2.M31) + (matrix1.M14 * matrix2.M41);
        var m12 = (matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22) + (matrix1.M13 * matrix2.M32) + (matrix1.M14 * matrix2.M42);
        var m13 = (matrix1.M11 * matrix2.M13) + (matrix1.M12 * matrix2.M23) + (matrix1.M13 * matrix2.M33) + (matrix1.M14 * matrix2.M43);
        var m14 = (matrix1.M11 * matrix2.M14) + (matrix1.M12 * matrix2.M24) + (matrix1.M13 * matrix2.M34) + (matrix1.M14 * matrix2.M44);
        var m21 = (matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21) + (matrix1.M23 * matrix2.M31) + (matrix1.M24 * matrix2.M41);
        var m22 = (matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22) + (matrix1.M23 * matrix2.M32) + (matrix1.M24 * matrix2.M42);
        var m23 = (matrix1.M21 * matrix2.M13) + (matrix1.M22 * matrix2.M23) + (matrix1.M23 * matrix2.M33) + (matrix1.M24 * matrix2.M43);
        var m24 = (matrix1.M21 * matrix2.M14) + (matrix1.M22 * matrix2.M24) + (matrix1.M23 * matrix2.M34) + (matrix1.M24 * matrix2.M44);
        var m31 = (matrix1.M31 * matrix2.M11) + (matrix1.M32 * matrix2.M21) + (matrix1.M33 * matrix2.M31) + (matrix1.M34 * matrix2.M41);
        var m32 = (matrix1.M31 * matrix2.M12) + (matrix1.M32 * matrix2.M22) + (matrix1.M33 * matrix2.M32) + (matrix1.M34 * matrix2.M42);
        var m33 = (matrix1.M31 * matrix2.M13) + (matrix1.M32 * matrix2.M23) + (matrix1.M33 * matrix2.M33) + (matrix1.M34 * matrix2.M43);
        var m34 = (matrix1.M31 * matrix2.M14) + (matrix1.M32 * matrix2.M24) + (matrix1.M33 * matrix2.M34) + (matrix1.M34 * matrix2.M44);
        var m41 = (matrix1.M41 * matrix2.M11) + (matrix1.M42 * matrix2.M21) + (matrix1.M43 * matrix2.M31) + (matrix1.M44 * matrix2.M41);
        var m42 = (matrix1.M41 * matrix2.M12) + (matrix1.M42 * matrix2.M22) + (matrix1.M43 * matrix2.M32) + (matrix1.M44 * matrix2.M42);
        var m43 = (matrix1.M41 * matrix2.M13) + (matrix1.M42 * matrix2.M23) + (matrix1.M43 * matrix2.M33) + (matrix1.M44 * matrix2.M43);
        var m44 = (matrix1.M41 * matrix2.M14) + (matrix1.M42 * matrix2.M24) + (matrix1.M43 * matrix2.M34) + (matrix1.M44 * matrix2.M44);
        matrix1.M11 = m11;
        matrix1.M12 = m12;
        matrix1.M13 = m13;
        matrix1.M14 = m14;
        matrix1.M21 = m21;
        matrix1.M22 = m22;
        matrix1.M23 = m23;
        matrix1.M24 = m24;
        matrix1.M31 = m31;
        matrix1.M32 = m32;
        matrix1.M33 = m33;
        matrix1.M34 = m34;
        matrix1.M41 = m41;
        matrix1.M42 = m42;
        matrix1.M43 = m43;
        matrix1.M44 = m44;
        return matrix1;
    }


    public static Matrix operator *(Matrix matrix, float scaleFactor)
    {
        matrix.M11 *= scaleFactor;
        matrix.M12 *= scaleFactor;
        matrix.M13 *= scaleFactor;
        matrix.M14 *= scaleFactor;
        matrix.M21 *= scaleFactor;
        matrix.M22 *= scaleFactor;
        matrix.M23 *= scaleFactor;
        matrix.M24 *= scaleFactor;
        matrix.M31 *= scaleFactor;
        matrix.M32 *= scaleFactor;
        matrix.M33 *= scaleFactor;
        matrix.M34 *= scaleFactor;
        matrix.M41 *= scaleFactor;
        matrix.M42 *= scaleFactor;
        matrix.M43 *= scaleFactor;
        matrix.M44 *= scaleFactor;
        return matrix;
    }


    public static Matrix operator -(Matrix matrix1, Matrix matrix2)
    {
        matrix1.M11 -= matrix2.M11;
        matrix1.M12 -= matrix2.M12;
        matrix1.M13 -= matrix2.M13;
        matrix1.M14 -= matrix2.M14;
        matrix1.M21 -= matrix2.M21;
        matrix1.M22 -= matrix2.M22;
        matrix1.M23 -= matrix2.M23;
        matrix1.M24 -= matrix2.M24;
        matrix1.M31 -= matrix2.M31;
        matrix1.M32 -= matrix2.M32;
        matrix1.M33 -= matrix2.M33;
        matrix1.M34 -= matrix2.M34;
        matrix1.M41 -= matrix2.M41;
        matrix1.M42 -= matrix2.M42;
        matrix1.M43 -= matrix2.M43;
        matrix1.M44 -= matrix2.M44;
        return matrix1;
    }


    public static Matrix operator -(Matrix matrix)
    {
        matrix.M11 = -matrix.M11;
        matrix.M12 = -matrix.M12;
        matrix.M13 = -matrix.M13;
        matrix.M14 = -matrix.M14;
        matrix.M21 = -matrix.M21;
        matrix.M22 = -matrix.M22;
        matrix.M23 = -matrix.M23;
        matrix.M24 = -matrix.M24;
        matrix.M31 = -matrix.M31;
        matrix.M32 = -matrix.M32;
        matrix.M33 = -matrix.M33;
        matrix.M34 = -matrix.M34;
        matrix.M41 = -matrix.M41;
        matrix.M42 = -matrix.M42;
        matrix.M43 = -matrix.M43;
        matrix.M44 = -matrix.M44;
        return matrix;
    }


    public static Matrix Subtract(Matrix matrix1, Matrix matrix2)
    {
        matrix1.M11 -= matrix2.M11;
        matrix1.M12 -= matrix2.M12;
        matrix1.M13 -= matrix2.M13;
        matrix1.M14 -= matrix2.M14;
        matrix1.M21 -= matrix2.M21;
        matrix1.M22 -= matrix2.M22;
        matrix1.M23 -= matrix2.M23;
        matrix1.M24 -= matrix2.M24;
        matrix1.M31 -= matrix2.M31;
        matrix1.M32 -= matrix2.M32;
        matrix1.M33 -= matrix2.M33;
        matrix1.M34 -= matrix2.M34;
        matrix1.M41 -= matrix2.M41;
        matrix1.M42 -= matrix2.M42;
        matrix1.M43 -= matrix2.M43;
        matrix1.M44 -= matrix2.M44;
        return matrix1;
    }


    public static void Subtract(ref Matrix matrix1, ref Matrix matrix2, out Matrix result)
    {
        result.M11 = matrix1.M11 - matrix2.M11;
        result.M12 = matrix1.M12 - matrix2.M12;
        result.M13 = matrix1.M13 - matrix2.M13;
        result.M14 = matrix1.M14 - matrix2.M14;
        result.M21 = matrix1.M21 - matrix2.M21;
        result.M22 = matrix1.M22 - matrix2.M22;
        result.M23 = matrix1.M23 - matrix2.M23;
        result.M24 = matrix1.M24 - matrix2.M24;
        result.M31 = matrix1.M31 - matrix2.M31;
        result.M32 = matrix1.M32 - matrix2.M32;
        result.M33 = matrix1.M33 - matrix2.M33;
        result.M34 = matrix1.M34 - matrix2.M34;
        result.M41 = matrix1.M41 - matrix2.M41;
        result.M42 = matrix1.M42 - matrix2.M42;
        result.M43 = matrix1.M43 - matrix2.M43;
        result.M44 = matrix1.M44 - matrix2.M44;
    }


    public override readonly string ToString()
    {
        return "{" + String.Format("M11:{0} M12:{1} M13:{2} M14:{3}", M11, M12, M13, M14) + "}"
            + " {" + String.Format("M21:{0} M22:{1} M23:{2} M24:{3}", M21, M22, M23, M24) + "}"
            + " {" + String.Format("M31:{0} M32:{1} M33:{2} M34:{3}", M31, M32, M33, M34) + "}"
            + " {" + String.Format("M41:{0} M42:{1} M43:{2} M44:{3}", M41, M42, M43, M44) + "}";
    }


    public static Matrix Transpose(Matrix matrix)
    {
        Transpose(ref matrix, out Matrix ret);
        return ret;
    }


    public static void Transpose(ref Matrix matrix, out Matrix result)
    {
        Matrix ret;

        ret.M11 = matrix.M11;
        ret.M12 = matrix.M21;
        ret.M13 = matrix.M31;
        ret.M14 = matrix.M41;

        ret.M21 = matrix.M12;
        ret.M22 = matrix.M22;
        ret.M23 = matrix.M32;
        ret.M24 = matrix.M42;

        ret.M31 = matrix.M13;
        ret.M32 = matrix.M23;
        ret.M33 = matrix.M33;
        ret.M34 = matrix.M43;

        ret.M41 = matrix.M14;
        ret.M42 = matrix.M24;
        ret.M43 = matrix.M34;
        ret.M44 = matrix.M44;

        result = ret;
    }
    #endregion Public Methods

}
