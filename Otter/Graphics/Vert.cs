using System;

using SFML.Graphics;
using SFML.System;

using Otter.Utility.MonoGame;

namespace Otter.Graphics;

/// <summary>
/// Class that represents a Vertex.  Just a wrapper for an SFML Vertex.
/// </summary>
/// <remarks>
/// Create a new Vert.
/// </remarks>
/// <param name="x">The X position.</param>
/// <param name="y">The Y position.</param>
/// <param name="color">The Color.</param>
/// <param name="u">The X position on the Texture.</param>
/// <param name="v">The Y position on the Texture.</param>
public class Vert(float x, float y, Color color, float u, float v)
{
    #region Private Fields

    private Vertex vertex = new(new Vector2f(x, y), color.SFMLColor, new Vector2f(u, v));

    #endregion

    #region Public Properties

    /// <summary>
    /// The Color of the Vert.
    /// </summary>
    public Color Color
    {
        get => new(vertex.Color);
        set => vertex.Color = value.SFMLColor;
    }

    /// <summary>
    /// The X position.
    /// </summary>
    public float X
    {
        get => vertex.Position.X;
        set => vertex.Position = new Vector2f(value, vertex.Position.Y);
    }

    /// <summary>
    /// The Y position.
    /// </summary>
    public float Y
    {
        get => vertex.Position.Y;
        set => vertex.Position = new Vector2f(vertex.Position.X, value);
    }

    /// <summary>
    /// The X, Y position as a Vector2.
    /// </summary>
    public Vector2 Position
    {
        get => new(vertex.Position.X, vertex.Position.Y);
        set => vertex.Position = new Vector2f((float)value.X, (float)value.Y);
    }

    /// <summary>
    /// The X, Y position of the Texture as a Vector2.
    /// </summary>
    public Vector2 TexCoords
    {
        get => new(vertex.TexCoords.X, vertex.TexCoords.Y);
        set => vertex.TexCoords = new Vector2f((float)value.X, (float)value.Y);
    }

    /// <summary>
    /// The X position of the Texture.
    /// </summary>
    public float U
    {
        get => vertex.TexCoords.X;
        set => vertex.TexCoords = new Vector2f(value, vertex.TexCoords.Y);
    }

    /// <summary>
    /// The Y position of the Texture.
    /// </summary>
    public float V
    {
        get => vertex.TexCoords.Y;
        set => vertex.TexCoords = new Vector2f(vertex.TexCoords.X, value);
    }

    #endregion

    #region Public Methods

    public override string ToString()
    {
        return String.Format("X: {0} Y: {1} Color: {2} U: {3} V: {4}", X, Y, Color, U, V);
    }

    #endregion
    #region Constructors

    /// <summary>
    /// Create a new Vert.
    /// </summary>
    /// <param name="copy">A source Vert to copy.</param>
    public Vert(Vert copy) : this(copy.X, copy.Y, copy.Color, copy.U, copy.V) { }

    /// <summary>
    /// Create a new white Vert at 0, 0.
    /// </summary>
    public Vert() : this(0, 0, Color.White, 0, 0) { }

    /// <summary>
    /// Create a new Vert.
    /// </summary>
    /// <param name="x">The X position.</param>
    /// <param name="y">The Y position.</param>
    public Vert(float x, float y) : this(x, y, Color.White, 0, 0) { }

    /// <summary>
    /// Create a new Vert.
    /// </summary>
    /// <param name="x">The X position.</param>
    /// <param name="y">The Y position.</param>
    /// <param name="u">The X position on the Texture.</param>
    /// <param name="v">The Y position on the Texture.</param>
    public Vert(float x, float y, float u, float v) : this(x, y, Color.White, u, v) { }

    /// <summary>
    /// Create a new Vert.
    /// </summary>
    /// <param name="x">The X position.</param>
    /// <param name="y">The Y position.</param>
    /// <param name="color">The Color.</param>
    public Vert(float x, float y, Color color) : this(x, y, color, 0, 0) { }

    #endregion

    #region Internal

    internal Vertex SFMLVertex => vertex;

    #endregion
}
