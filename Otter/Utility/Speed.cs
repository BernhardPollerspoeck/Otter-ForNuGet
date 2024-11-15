using System;

namespace Otter.Utility;

/// <summary>
/// Class used for tracking an X and Y speed of an object. Speed also has an XMax and YMax that can
/// be used to clamp the X and Y values automatically.
/// </summary>
/// <remarks>
/// Create a new Speed object.
/// </remarks>
/// <param name="x">The initial X value.</param>
/// <param name="y">The initial Y value.</param>
/// <param name="maxX">The maximum X value.</param>
/// <param name="maxY">The maximum Y value.</param>
/// <param name="hardClamp">Determines if the value can exceed the maximum.</param>
public class Speed(float x, float y, float maxX, float maxY, bool hardClamp)
{
    private float x = x, y = y;

    /// <summary>
    /// The maximum X speed.
    /// </summary>
    public float MaxX = maxX;

    /// <summary>
    /// The maximum Y speed.
    /// </summary>
    public float MaxY = maxY;

    /// <summary>
    /// Determines if the maximum values will be hard clamped.
    /// If true, values will never exceed the maximums.
    /// </summary>
    public bool HardClamp = hardClamp;

    /// <summary>
    /// The current X value of the speed.
    /// </summary>
    public float X
    {
        get => HardClamp ? Util.Clamp(x, -MaxX, MaxX) : x;
        set => x = value;
    }

    /// <summary>
    /// The current Y value of the speed.
    /// </summary>
    public float Y
    {
        get => HardClamp ? Util.Clamp(y, -MaxY, MaxY) : y;
        set => y = value;
    }

    /// <summary>
    /// Shortcut to set both MaxX and MaxY.
    /// </summary>
    public float Max
    {
        set
        {
            MaxX = value; MaxY = value;
        }
    }

    /// <summary>
    /// The length of the speed object.
    /// </summary>
    public float Length => (float)Math.Sqrt((X * X) + (Y * Y));

    /// <summary>
    /// Create a new Speed object.
    /// </summary>
    /// <param name="x">The initial X value.</param>
    /// <param name="y">The initial Y value.</param>
    /// <param name="maxX">The maximum X value.</param>
    /// <param name="maxY">The maximum Y value.</param>
    public Speed(float x, float y, float maxX, float maxY) : this(x, y, maxX, maxY, true) { }

    /// <summary>
    /// Create a new Speed object.
    /// </summary>
    /// <param name="maxX">The maximum X value.</param>
    /// <param name="maxY">The maximum Y value.</param>
    public Speed(float maxX, float maxY) : this(0, 0, maxX, maxY, true) { }

    /// <summary>
    /// Create a new Speed object.
    /// </summary>
    /// <param name="maxX">The maximum X value.</param>
    /// <param name="maxY">The maximum Y value.</param>
    /// <param name="hardClamp">Determines if the value can exceed the maximum.</param>
    public Speed(float maxX, float maxY, bool hardClamp) : this(0, 0, maxX, maxY, hardClamp) { }

    /// <summary>
    /// Create a new Speed object.
    /// </summary>
    /// <param name="max">The maximum X and Y values.</param>
    /// <param name="hardClamp">Determines if the value can exceed the maximum.</param>
    public Speed(float max, bool hardClamp) : this(0, 0, max, max, hardClamp) { }

    /// <summary>
    /// Create a new Speed object.
    /// </summary>
    /// <param name="max">The maximum X and Y values.</param>
    public Speed(float max) : this(0, 0, max, max, true) { }

    /// <summary>
    /// Returns a String that represents this instance.
    /// </summary>
    /// <returns>
    /// A String that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return "X: " + X + " Y: " + Y;
    }

}
