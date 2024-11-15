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
/// Struct for representing a Rectangle.
/// </summary>
public struct Rectangle(int x, int y, int width, int height) : IEquatable<Rectangle>
{

    #region Private Fields

    private static Rectangle emptyRectangle = new();

    #endregion Private Fields

    #region Public Fields

    public int X = x;
    public int Y = y;
    public int Width = width;
    public int Height = height;

    #endregion Public Fields

    #region Public Properties

    public static Rectangle Empty => emptyRectangle;

    public readonly int Left => X;

    public readonly int Right => X + Width;

    public readonly int Top => Y;

    public readonly int Bottom => Y + Height;

    #endregion Public Properties
    #region Constructors

    #endregion Constructors

    #region Public Methods

    public static bool operator ==(Rectangle a, Rectangle b)
    {
        return (a.X == b.X) && (a.Y == b.Y) && (a.Width == b.Width) && (a.Height == b.Height);
    }

    public readonly bool Contains(int x, int y)
    {
        return (X <= x) && (x < (X + Width)) && (Y <= y) && (y < (Y + Height));
    }

    public readonly bool Contains(Point value)
    {
        return (X <= value.X) && (value.X < (X + Width)) && (Y <= value.Y) && (value.Y < (Y + Height));
    }

    public readonly bool Contains(Rectangle value)
    {
        return (X <= value.X) && ((value.X + value.Width) <= (X + Width)) && (Y <= value.Y) && ((value.Y + value.Height) <= (Y + Height));
    }

    public static bool operator !=(Rectangle a, Rectangle b)
    {
        return !(a == b);
    }

    public void Offset(Point offset)
    {
        X += offset.X;
        Y += offset.Y;
    }

    public void Offset(int offsetX, int offsetY)
    {
        X += offsetX;
        Y += offsetY;
    }

    public Point Location
    {
        readonly get => new(X, Y);
        set
        {
            X = value.X;
            Y = value.Y;
        }
    }

    public readonly Point Center =>
            // This is incorrect
            //return new Point( (this.X + this.Width) / 2,(this.Y + this.Height) / 2 );
            // What we want is the Center of the rectangle from the X and Y Origins
            new(X + (Width / 2), Y + (Height / 2));

    public void Inflate(int horizontalValue, int verticalValue)
    {
        X -= horizontalValue;
        Y -= verticalValue;
        Width += horizontalValue * 2;
        Height += verticalValue * 2;
    }

    public readonly Rectangle Inflated(int horizontalValue, int verticalValue)
    {
        var rect = new Rectangle(X, Y, Width, Height);
        rect.Inflate(horizontalValue, verticalValue);
        return rect;
    }

    public readonly bool IsEmpty => (Width == 0) && (Height == 0) && (X == 0) && (Y == 0);

    public readonly bool Equals(Rectangle other)
    {
        return this == other;
    }

    public override readonly bool Equals(object obj)
    {
        return (obj is Rectangle rectangle) && this == rectangle;
    }

    public override readonly string ToString()
    {
        return string.Format("{{X:{0} Y:{1} Width:{2} Height:{3}}}", X, Y, Width, Height);
    }

    public override readonly int GetHashCode()
    {
        return X ^ Y ^ Width ^ Height;
    }

    public readonly bool Intersects(Rectangle value)
    {
        return value.Left < Right &&
               Left < value.Right &&
               value.Top < Bottom &&
               Top < value.Bottom;
    }


    public readonly void Intersects(ref Rectangle value, out bool result)
    {
        result = value.Left < Right &&
                 Left < value.Right &&
                 value.Top < Bottom &&
                 Top < value.Bottom;
    }

    public static Rectangle Intersect(Rectangle value1, Rectangle value2)
    {
        Intersect(ref value1, ref value2, out Rectangle rectangle);
        return rectangle;
    }


    public static void Intersect(ref Rectangle value1, ref Rectangle value2, out Rectangle result)
    {
        if (value1.Intersects(value2))
        {
            var right_side = Math.Min(value1.X + value1.Width, value2.X + value2.Width);
            var left_side = Math.Max(value1.X, value2.X);
            var top_side = Math.Max(value1.Y, value2.Y);
            var bottom_side = Math.Min(value1.Y + value1.Height, value2.Y + value2.Height);
            result = new Rectangle(left_side, top_side, right_side - left_side, bottom_side - top_side);
        }
        else
        {
            result = new Rectangle(0, 0, 0, 0);
        }
    }

    public static Rectangle Union(Rectangle value1, Rectangle value2)
    {
        var x = Math.Min(value1.X, value2.X);
        var y = Math.Min(value1.Y, value2.Y);
        return new Rectangle(x, y,
                             Math.Max(value1.Right, value2.Right) - x,
                                 Math.Max(value1.Bottom, value2.Bottom) - y);
    }

    public static void Union(ref Rectangle value1, ref Rectangle value2, out Rectangle result)
    {
        result.X = Math.Min(value1.X, value2.X);
        result.Y = Math.Min(value1.Y, value2.Y);
        result.Width = Math.Max(value1.Right, value2.Right) - result.X;
        result.Height = Math.Max(value1.Bottom, value2.Bottom) - result.Y;
    }

    #endregion Public Methods

}
