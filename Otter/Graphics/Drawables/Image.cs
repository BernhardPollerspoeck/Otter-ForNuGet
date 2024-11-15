using SFML.Graphics;
using SFML.System;

using Otter.Core;
using Otter.Utility;
using Otter.Utility.MonoGame;

namespace Otter.Graphics.Drawables;

/// <summary>
/// Graphic type that is used to represent a static image.
/// </summary>
public class Image : Graphic
{
    #region Static Fields

    /// <summary>
    /// The amount of points to use when rendering a circle shape.
    /// </summary>
    private readonly static int CirclePointCount = 24;

    #endregion

    #region Static Methods

    /// <summary>
    /// Creates a rectangle.
    /// </summary>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    /// <param name="color">The color of the rectangle.</param>
    /// <returns>A new image containing the rectangle.</returns>
    static public Image CreateRectangle(int width, int height, Color color)
    {
        Image img = new(width, height)
        {
            Color = color,

            Width = width,
            Height = height,

            rectWidth = width,
            rectHeight = height,

            isShape = true,
            Batchable = false, // Cant batch rectangles for now

            OutlineColor = Color.None
        };

        return img;
    }

    /// <summary>
    /// Creates a rectangle the size of the active Game window.
    /// </summary>
    /// <param name="color">The color of the rectangle.</param>
    /// <returns>A new image containing the rectangle.</returns>
    static public Image CreateRectangle(Color color)
    {
        var w = Game.Instance.Width;
        var h = Game.Instance.Height;
        return CreateRectangle(w, h, color);
    }

    /// <summary>
    /// Creates a simple black rectangle the size of the active Game window.
    /// </summary>
    /// <returns>A new image containing the rectangle.</returns>
    static public Image CreateRectangle()
    {
        return CreateRectangle(Color.Black);
    }

    /// <summary>
    /// Creates a rectangle.
    /// </summary>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    /// <returns>A new image containing the rectangle.</returns>
    static public Image CreateRectangle(int width, int height)
    {
        return CreateRectangle(width, height, Color.White);
    }

    /// <summary>
    /// Creates a rectangle.
    /// </summary>
    /// <param name="size">The width and height of the rectangle.</param>
    /// <returns>A new image containing the rectangle.</returns>
    static public Image CreateRectangle(int size)
    {
        return CreateRectangle(size, size);
    }

    /// <summary>
    /// Creates a rectangle.
    /// </summary>
    /// <param name="size">The width and height of the rectangle.</param>
    /// <param name="color">The color of the rectangle.</param>
    /// <returns>A new image containing the rectangle.</returns>
    static public Image CreateRectangle(int size, Color color)
    {
        return CreateRectangle(size, size, color);
    }

    /// <summary>
    /// Create a circle.
    /// </summary>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="color">The color of the circle.</param>
    /// <returns>A new image containing the circle.</returns>
    static public Image CreateCircle(int radius, Color color)
    {
        Image img = new()
        {
            Width = radius * 2,
            Height = radius * 2,
            Color = color,

            radius = radius,

            OutlineColor = Color.None,

            isCircle = true,
            isShape = true,
            Batchable = false
        };

        return img;
    }

    /// <summary>
    /// Create a white circle.
    /// </summary>
    /// <param name="radius">The radius of the circle.</param>
    /// <returns>A new image containing the circle.</returns>
    static public Image CreateCircle(int radius)
    {
        return CreateCircle(radius, Color.White);
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Defines which area of the Image to show.
    /// </summary>
    public Rectangle ClippingRegion
    {
        get => clippingRegion;
        set
        {
            clippingRegion = value;
            NeedsUpdate = true;
        }
    }

    /// <summary>
    /// Flip the texture coordinates on the X axis.
    /// </summary>
    public bool FlippedX
    {
        get => flippedX;
        set
        {
            flippedX = value;
            NeedsUpdate = true;
        }
    }

    /// <summary>
    /// Flip the texture coordinates on the Y axis.
    /// </summary>
    public bool FlippedY
    {
        get => flippedY;
        set
        {
            flippedY = value;
            NeedsUpdate = true;
        }
    }

    /// <summary>
    /// The outline color of the Image (only applies to circles and rectangles.)
    /// </summary>
    public Color OutlineColor
    {
        get => outlineColor switch
        {
            null => Color.None,
            _ => outlineColor
        };
        set
        {
            OutlineColor.Graphic = null;
            outlineColor = value;
            outlineColor.Graphic = this;
            NeedsUpdate = true;
        }
    }

    /// <summary>
    /// The outline thickness of the Image (only applies to circles and rectangles.)
    /// </summary>
    public float OutlineThickness
    {
        get => outlineThickness;
        set
        {
            outlineThickness = value;
            NeedsUpdate = true;
        }
    }

    #endregion

    #region Private Fields

    private bool flippedX;
    private bool flippedY;
    private Rectangle clippingRegion;

    protected bool
        flipX = false,
        flipY = false;
    private bool isCircle;
    private bool isShape;

    // Keep track of radius becuase of outline thickness weirdness.
    private int radius;

    // Keep track of shape w and h because of outline thickness weirdness.
    private int rectWidth;
    private int rectHeight;
    private Color outlineColor;
    private float outlineThickness;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new Image using a filepath to a texture.
    /// </summary>
    /// <param name="source">The filepath of the texture.</param>
    /// <param name="clip">Where to clip the texture.</param>
    public Image(string source = null) : base()
    {
        SetTexture(new Texture(source));
        Initialize();
    }

    protected Image() : base()
    {
        Initialize();
    }

    /// <summary>
    /// Creates a new Image using a Texture.
    /// </summary>
    /// <param name="texture">The Texture to use.</param>
    public Image(Texture texture) : base()
    {
        SetTexture(texture);
        Initialize();
    }

    /// <summary>
    /// Creates a new Image using an AtlasTexture.
    /// </summary>
    /// <param name="texture">The AtlasTexture to use.</param>
    public Image(AtlasTexture texture) : base()
    {
        SetTexture(texture);
        Initialize();
    }

    /// <summary>
    /// Creates a new Image using a specified width and height.
    /// </summary>
    /// <param name="width">The width of the Image.</param>
    /// <param name="height">The hight of the Image.</param>
    public Image(int width, int height) : base()
    {
        Initialize();
        Width = width;
        Height = height;
    }

    #endregion

    #region Private Methods

    private void Initialize()
    {
        Name = "Image";

        // Proper sprite batching coming soon.
        //Batchable = true;

        Width = TextureRegion.Width;
        Height = TextureRegion.Height;

        ClippingRegion = TextureRegion;
    }

    protected override void TextureChanged()
    {
        base.TextureChanged();

        Width = TextureRegion.Width;
        Height = TextureRegion.Height;

        ClippingRegion = TextureRegion;
    }

    protected override void UpdateDrawable()
    {
        base.UpdateDrawable();

        if (isCircle)
        {
            var circle = new SFML.Graphics.CircleShape(radius)
            {
                OutlineThickness = OutlineThickness,
                OutlineColor = OutlineColor.SFMLColor,
                FillColor = Color.SFMLColor
            };
            circle.SetPointCount((uint)CirclePointCount);
            SFMLDrawable = circle;
            Width = (int)circle.GetLocalBounds().Width;
            Height = (int)circle.GetLocalBounds().Height;
        }
        else
        {
            if (isShape)
            {
                var rect = new SFML.Graphics.RectangleShape(new Vector2f(rectWidth, rectHeight))
                {
                    OutlineColor = OutlineColor.SFMLColor,
                    OutlineThickness = OutlineThickness,
                    FillColor = Color.SFMLColor
                };
                SFMLDrawable = rect;
                Width = (int)rect.GetLocalBounds().Width;
                Height = (int)rect.GetLocalBounds().Height;
            }
            else
            {
                SFMLVertices.Clear();

                float x1, y1, x2, y2, u1, v1, u2, v2, cx1, cy1, cx2, cy2;

                cx1 = ClippingRegion.Left;
                cy1 = ClippingRegion.Top;
                cx2 = ClippingRegion.Right;
                cy2 = ClippingRegion.Bottom;

                x1 = Util.Max(0, cx1);
                u1 = TextureLeft + x1;

                if (FlippedX)
                {
                    u1 = TextureRegion.Width - u1 + TextureLeft + TextureRegion.Left;
                }

                y1 = Util.Max(0, cy1);
                v1 = TextureTop + y1;

                if (FlippedY)
                {
                    v1 = TextureRegion.Height - v1 + TextureTop + TextureRegion.Top;
                }

                x2 = Util.Min(TextureRegion.Right, cx2);
                u2 = TextureLeft + x2;

                if (FlippedX)
                {
                    u2 = TextureRegion.Width - u2 + TextureLeft + TextureRegion.Left;
                }

                y2 = Util.Min(TextureRegion.Bottom, cy2);
                v2 = TextureTop + y2;

                if (FlippedY)
                {
                    v2 = TextureRegion.Height - v2 + TextureTop + TextureRegion.Top;
                }

                SFMLVertices.Append(x1, y1, Color, u1, v1);
                SFMLVertices.Append(x1, y2, Color, u1, v2);
                SFMLVertices.Append(x2, y2, Color, u2, v2);
                SFMLVertices.Append(x2, y1, Color, u2, v1);

                Width = TextureRegion.Width;
                Height = TextureRegion.Height;
            }
        }
    }

    #endregion

    #region Internal

    internal VertexArray GetVertices()
    {
        return SFMLVertices;
    }

    #endregion
}
