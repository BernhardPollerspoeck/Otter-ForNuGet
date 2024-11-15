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

namespace Otter.Utility.MonoGame;

/// <summary>
/// Struct for representing a Point.
/// </summary>
public struct Point(int x, int y) : IEquatable<Point>
{

    #region Private Fields

    private static Point zeroPoint = new();

    #endregion Private Fields

    #region Public Fields

    public int X = x;
    public int Y = y;

    #endregion Public Fields

    #region Properties

    public static Point Zero => zeroPoint;

    #endregion Properties
    #region Public methods

    public static bool operator ==(Point a, Point b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(Point a, Point b)
    {
        return !a.Equals(b);
    }

    public readonly bool Equals(Point other)
    {
        return (X == other.X) && (Y == other.Y);
    }

    public override readonly bool Equals(object obj)
    {
        return (obj is Point point) && Equals(point);
    }

    public override readonly int GetHashCode()
    {
        return X ^ Y;
    }

    public override readonly string ToString()
    {
        return string.Format("{{X:{0} Y:{1}}}", X, Y);
    }

    #endregion

}


