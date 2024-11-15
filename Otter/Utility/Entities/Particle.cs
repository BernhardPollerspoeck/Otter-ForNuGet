using System;
using System.Collections.Generic;

using Otter.Core;
using Otter.Graphics;
using Otter.Graphics.Drawables;

namespace Otter.Utility.Entities;

/// <summary>
/// Entity that is a quick way to make a particle.  Has lots of parameters that can be set, so
/// use this with that constructor where you do { } and put a bunch of properties inside.
/// <example>
/// var p = new Particle(0, 0, "particle.png", 100, 100) {
///     LifeSpan = 30,
///     Angle = 10,
///     FinalScaleX = 0,
///     LockScaleRatio = true
/// };
/// </example>
/// </summary>
public class Particle : Entity
{
    #region Private Fields

    private Color color;
    private bool hasColor;
    private bool useFrameList;
    private float speedLen;
    private bool hasSpeedLen;
    private float speedDir;
    private bool hasSpeedDir;
    private int frameCount;
    private bool hasFrameCount;
    private float finalSpeedX;
    private bool hasFinalSpeedX;
    private float initSpeedX;
    private float finalSpeedY;
    private bool hasFinalSpeedY;
    private float initSpeedY;
    private float finalScaleX;
    private bool hasFinalScaleX;
    private float initScaleX;
    private float finalScaleY;
    private bool hasFinalScaleY;
    private float initScaleY;
    private float finalAngle;
    private bool hasFinalAngle;
    private float initAngle;
    private float finalX;
    private bool hasFinalX;
    private float initX;
    private float finalY;
    private bool hasFinalY;
    private float initY;
    private float finalAlpha;
    private bool hasFinalAlpha;
    private float initAlpha;
    private float finalColorR;
    private bool hasFinalColorR;
    private float initColorR;
    private float finalColorG;
    private bool hasFinalColorG;
    private float initColorG;
    private float finalColorB;
    private bool hasFinalColorB;
    private float initColorB;
    private Color finalColor;
    private bool hasFinalColor;
    private Color initColor;
    private float finalSpeedLen;
    private bool hasFinalSpeedLen;
    private float initSpeedLen;
    private float finalSpeedDir;
    private bool hasFinalSpeedDir;
    private float initSpeedDir;
    private float originX;
    private float originY;
    private bool useOrigin;
    private bool useSpeedXY = true;
    private bool tweenPosition = false;
    private float xpos, ypos;
    private float delayTimer = 0;
    private float colorLerp;

    #endregion

    #region Public Static Fields

    public static int ActiveCount { get; set; } = 0;

    #endregion

    #region Public Fields

    /// <summary>
    /// The ease function to use when interpolating values.
    /// </summary>
    public Func<float, float> Ease;

    /// <summary>
    /// The ImageSet that the Particle is rendering.
    /// </summary>
    public ImageSet Image;

    /// <summary>
    /// The default life span for all Particle Entities.
    /// </summary>
    public static int DefaultLifeSpan { get; set; } = 60;

    /// <summary>
    /// The initial horizontal speed.
    /// </summary>
    public float SpeedX;

    /// <summary>
    /// The initial vertical speed.
    /// </summary>
    public float SpeedY;

    /// <summary>
    /// The horizontal offset from the spawn position.
    /// </summary>
    public float OffsetX;

    /// <summary>
    /// The vertical offset from the spawn position.
    /// </summary>
    public float OffsetY;

    /// <summary>
    /// The initial X scale of the rendered Image.
    /// </summary>
    public float ScaleX = 1;

    /// <summary>
    /// The initial Y scale fo the rendered Image.
    /// </summary>
    public float ScaleY = 1;

    /// <summary>
    /// The angle of the Image.
    /// </summary>
    public float Angle;

    /// <summary>
    /// The initial alpha of the Image.
    /// </summary>
    public float Alpha = 1;

    /// <summary>
    /// The initial Color R component of the Image.
    /// </summary>
    public float ColorR = 1;

    /// <summary>
    /// The initial Color G component of the Image.
    /// </summary>
    public float ColorG = 1;

    /// <summary>
    /// The initial Color B component of the Image.
    /// </summary>
    public float ColorB = 1;

    /// <summary>
    /// The amount of time that must pass before the Entity is spawned.
    /// </summary>
    public float Delay;

    /// <summary>
    /// How many steps the particle should move by its speed when first created.
    /// </summary>
    public int AdvanceSteps;

    /// <summary>
    /// Determines if the image angle should be locked to the direction of the particle's movement.
    /// </summary>
    public bool MotionAngle;

    /// <summary>
    /// The BlendMode of the Image rendered.
    /// </summary>
    public BlendMode Blend = BlendMode.Alpha;

    /// <summary>
    /// The starting frame in the ImageSet.
    /// </summary>
    public int FrameOffset;

    /// <summary>
    /// How many times to loop the ImageSet animation during the Particle's lifespan.
    /// </summary>
    public int Loops;

    /// <summary>
    /// Determines if the ScaleY will always be locked to the ScaleX.
    /// </summary>
    public bool LockScaleRatio;

    /// <summary>
    /// Determines if the Image will have a centered origin.
    /// </summary>
    public bool CenterOrigin = true;

    /// <summary>
    /// Flip the ImageSet horizontally.
    /// </summary>
    public bool FlipX = false;

    /// <summary>
    /// Flip the ImageSet vertically.
    /// </summary>
    public bool FlipY = false;

    /// <summary>
    /// Determines if the Particle should animate the ImageSet.
    /// </summary>
    public bool Animate;

    /// <summary>
    /// The shader to use on the ImageSet.
    /// </summary>
    public Shader Shader;

    /// <summary>
    /// The specific frames to display for the ImageSet.  If set it will override the default FrameCount.
    /// </summary>
    public List<int> Frames;

    #endregion

    #region Public Properties

    /// <summary>
    /// The Color of the Particle.  Overrides ColorR, ColorG, ColorB.
    /// </summary>
    public Color Color
    {
        set { color = value; hasColor = true; }
        get => color;
    }

    /// <summary>
    /// The magnitude of the particle's movement.  Overrides SpeedX and SpeedY.
    /// </summary>
    public float SpeedLen
    {
        set { speedLen = value; hasSpeedLen = true; }
        get => speedLen;
    }

    /// <summary>
    /// The direction of the particle's movement.  Overrides SpeedX and SpeedY.
    /// </summary>
    public float SpeedDir
    {
        set { speedDir = value; hasSpeedDir = true; }
        get => speedDir;
    }

    /// <summary>
    /// How many frames are present in the ImageSet for the Particle to animate through.
    /// </summary>
    public int FrameCount
    {
        set { frameCount = value; hasFrameCount = true; }
        get => frameCount;
    }

    /// <summary>
    /// The final horizontal speed of the Particle.
    /// </summary>
    public float FinalSpeedX
    {
        set { finalSpeedX = value; hasFinalSpeedX = true; }
        get => finalSpeedX;
    }

    /// <summary>
    /// The final vertical speed of the Particle.
    /// </summary>
    public float FinalSpeedY
    {
        set { finalSpeedY = value; hasFinalSpeedY = true; }
        get => finalSpeedY;
    }

    /// <summary>
    /// The final X scale of the rendered Image.
    /// </summary>
    public float FinalScaleX
    {
        set { finalScaleX = value; hasFinalScaleX = true; }
        get => finalScaleX;
    }

    /// <summary>
    /// The final Y scale of the rendered Image.
    /// </summary>
    public float FinalScaleY
    {
        set { finalScaleY = value; hasFinalScaleY = true; }
        get => finalScaleY;
    }

    /// <summary>
    /// The final angle of the rendered Image.
    /// </summary>
    public float FinalAngle
    {
        set { finalAngle = value; hasFinalAngle = true; }
        get => finalAngle;
    }

    /// <summary>
    /// The final X position of the Particle. If set this overrides speed values.
    /// </summary>
    public float FinalX
    {
        set { finalX = value; hasFinalX = true; }
        get => finalX;
    }

    /// <summary>
    /// The final Y position of the Particle. If set this overrides speed values.
    /// </summary>
    public float FinalY
    {
        set { finalY = value; hasFinalY = true; }
        get => finalY;
    }

    /// <summary>
    /// The final Alpha of the rendered Image.
    /// </summary>
    public float FinalAlpha
    {
        set { finalAlpha = value; hasFinalAlpha = true; }
        get => finalAlpha;
    }

    /// <summary>
    /// The final Color R component of the rendered Image.
    /// </summary>
    public float FinalColorR
    {
        set { finalColorR = value; hasFinalColorR = true; }
        get => finalColorR;
    }

    /// <summary>
    /// The final Color G component of the rendered Image.
    /// </summary>
    public float FinalColorG
    {
        set { finalColorG = value; hasFinalColorG = true; }
        get => finalColorG;
    }

    /// <summary>
    /// The final Color B component of the rendered Image.
    /// </summary>
    public float FinalColorB
    {
        set { finalColorB = value; hasFinalColorB = true; }
        get => finalColorB;
    }

    /// <summary>
    /// The final Color of the rendered Image.  If set will override the R G B components.
    /// </summary>
    public Color FinalColor
    {
        set { finalColor = value; hasFinalColor = true; }
        get => finalColor;
    }

    /// <summary>
    /// The final speed length of the Particle.  If set will override SpeedX and SpeedY.
    /// </summary>
    public float FinalSpeedLen
    {
        set { finalSpeedLen = value; hasFinalSpeedLen = true; }
        get => finalSpeedLen;
    }

    /// <summary>
    /// The final speed direction of the Particle.  If set will override SpeedX and SpeedY.
    /// </summary>
    public float FinalSpeedDir
    {
        set { finalSpeedDir = value; hasFinalSpeedDir = true; }
        get => finalSpeedDir;
    }

    /// <summary>
    /// The X origin of the rendered Image.
    /// </summary>
    public float OriginX
    {
        get => originX;
        set
        {
            originX = value;
            useOrigin = true;
        }
    }

    /// <summary>
    /// The Y origin of the rendered Image.
    /// </summary>
    public float OriginY
    {
        get => originY;
        set
        {
            originY = value;
            useOrigin = true;
        }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Create a new Particle.
    /// </summary>
    /// <param name="x">The x position.</param>
    /// <param name="y">The y position.</param>
    /// <param name="source">The source file path of the ImageSet.</param>
    /// <param name="width">The width of the ImageSet cell.</param>
    /// <param name="height">The height of the ImageSet cell.</param>
    public Particle(float x, float y, string source = null, int width = 0, int height = 0)
        : base(x, y)
    {
        Image = new ImageSet(source, width, height);
    }

    /// <summary>
    /// Create a new Particle.
    /// </summary>
    /// <param name="x">The x position.</param>
    /// <param name="y">The y position.</param>
    /// <param name="texture">The Texture to use for the ImageSet.</param>
    /// <param name="width">The width of the ImageSet cell.</param>
    /// <param name="height">The height of the ImageSet cell.</param>
    public Particle(float x, float y, Texture texture, int width = 0, int height = 0)
        : base(x, y)
    {
        Image = new ImageSet(texture, width, height);
    }

    /// <summary>
    /// Create a new Particle.
    /// </summary>
    /// <param name="x">The x position.</param>
    /// <param name="y">The y position.</param>
    /// <param name="texture"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public Particle(float x, float y, AtlasTexture texture, int width = 0, int height = 0)
        : base(x, y)
    {
        Image = new ImageSet(texture, width, height);
    }

    /// <summary>
    /// Create a blank Particle.
    /// </summary>
    /// <param name="x">The x position.</param>
    /// <param name="y">The y position.</param>
    public Particle(float x, float y)
        : base(x, y)
    {

    }

    /// <summary>
    /// Create a new Particle.
    /// </summary>
    /// <param name="x">The x position.</param>
    /// <param name="y">The y position.</param>
    /// <param name="ImageSet">The ImageSet to use for the Particle.</param>
    public Particle(float x, float y, ImageSet imageSet)
        : base(x, y)
    {
        Image = imageSet;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Added to the Scene.
    /// </summary>
    public override void Added()
    {
        base.Added();

        if (Delay == 0)
        {
            Start();
        }
    }

    /// <summary>
    /// Start the Particle.
    /// </summary>
    public void Start()
    {
        if (LifeSpan == 0)
        {
            LifeSpan = DefaultLifeSpan;
        }

        if (Frames != null)
        {
            useFrameList = true;
            FrameCount = Frames.Count;
        }

        if (FrameCount > 0)
        {
            Animate = true;
        }

        if (hasSpeedLen || hasSpeedDir)
        {
            useSpeedXY = false;
        }
        if (hasFinalX || hasFinalY)
        {
            tweenPosition = true;
        }

        if (!tweenPosition)
        {
            if (useSpeedXY)
            {
                if (!hasFinalSpeedX)
                {
                    FinalSpeedX = SpeedX;
                }
                if (!hasFinalSpeedY)
                {
                    FinalSpeedY = SpeedY;
                }
                SpeedLen = 0;
                FinalSpeedLen = 0;
                SpeedDir = 0;
                FinalSpeedDir = 0;
            }
            else
            {
                FinalSpeedX = 0;
                FinalSpeedY = 0;
                if (!hasFinalSpeedLen)
                {
                    FinalSpeedLen = SpeedLen;
                }
                if (!hasFinalSpeedDir)
                {
                    FinalSpeedDir = SpeedDir;
                }
            }
            FinalX = 0;
            FinalY = 0;
        }
        else
        {
            xpos = X;
            ypos = Y;
            if (!hasFinalX)
            {
                FinalX = X;
            }
            if (!hasFinalY)
            {
                FinalY = Y;
            }
        }
        if (!hasFinalScaleX)
        {
            FinalScaleX = ScaleX;
        }
        if (!hasFinalScaleY)
        {
            FinalScaleY = ScaleY;
        }
        if (!hasFinalAngle)
        {
            FinalAngle = Angle;
        }
        if (!hasFinalAlpha)
        {
            FinalAlpha = Alpha;
        }
        if (!hasFinalColorR)
        {
            FinalColorR = ColorR;
        }
        if (!hasFinalColorG)
        {
            FinalColorG = ColorG;
        }
        if (!hasFinalColorB)
        {
            FinalColorB = ColorB;
        }

        initSpeedX = SpeedX;
        initSpeedY = SpeedY;
        initScaleX = ScaleX;
        initScaleY = ScaleY;
        initAngle = Angle;
        initAlpha = Alpha;
        initColorR = ColorR;
        initColorG = ColorG;
        initColorB = ColorB;
        initSpeedLen = SpeedLen;
        initSpeedDir = SpeedDir;
        initX = X;
        initY = Y;
        initColor = Color;

        X += OffsetX;
        Y += OffsetY;

        AddGraphic(Image);

        if (useOrigin)
        {
            Image.OriginX = originX;
            Image.OriginY = originY;
        }
        else
        {
            if (CenterOrigin)
            {
                Image.CenterOrigin();
            }
        }

        for (var i = 0; i < AdvanceSteps; i++)
        {
            if (!useSpeedXY)
            {
                SpeedX = Util.PolarX(SpeedDir, SpeedLen);
                SpeedY = Util.PolarY(SpeedDir, SpeedLen);
            }
            X += SpeedX;
            Y += SpeedY;
        }

        Ease ??= Glide.Ease.Linear;

        if (Shader != null)
        {
            Image.Shader = Shader;
        }

        Image.Visible = false;

        ActiveCount++;
    }

    /// <summary>
    /// Update the Particle.
    /// </summary>
    public override void Update()
    {
        base.Update();

        // Handle delay
        if (delayTimer < Delay)
        {
            delayTimer += Game.DeltaTime;
            Timer = 0;
            if (delayTimer == Delay)
            {
                Start();
            }
            return;
        }

        // Update values
        var lerp = Ease((float)Timer / LifeSpan);
        SpeedX = Util.Lerp(initSpeedX, finalSpeedX, lerp);
        SpeedY = Util.Lerp(initSpeedY, finalSpeedY, lerp);
        ScaleX = Util.Lerp(initScaleX, finalScaleX, lerp);
        ScaleY = Util.Lerp(initScaleY, finalScaleY, lerp);
        Angle = Util.Lerp(initAngle, finalAngle, lerp);
        Alpha = Util.Lerp(initAlpha, finalAlpha, lerp);
        ColorR = Util.Lerp(initColorR, finalColorR, lerp);
        ColorG = Util.Lerp(initColorG, finalColorG, lerp);
        ColorB = Util.Lerp(initColorB, finalColorB, lerp);
        SpeedLen = Util.Lerp(initSpeedLen, finalSpeedLen, lerp);
        SpeedDir = Util.Lerp(initSpeedDir, finalSpeedDir, lerp);
        xpos = Util.Lerp(initX, finalX, lerp);
        ypos = Util.Lerp(initY, finalY, lerp);
        colorLerp = Util.Lerp(0, 1, lerp);

        Image.Visible = true;

        // If the positions are not controlled by tweens.
        if (!tweenPosition)
        {
            // if SpeedDir and Len are being used
            if (!useSpeedXY)
            {
                SpeedX = Util.PolarX(SpeedDir, SpeedLen);
                SpeedY = Util.PolarY(SpeedDir, SpeedLen);
            }

            // Control the position with the particle speed.
            X += SpeedX;
            Y += SpeedY;
        }
        else
        {
            // Control the position with the interpolated/tweened position.
            X = xpos;
            Y = ypos;
        }

        // Set up animation
        _ = hasFrameCount ? FrameOffset + FrameCount : Image.Frames;

        // Animate the particle
        if (Animate)
        {
            var playCount = Loops + 1;
            var frameIndex = (int)Util.ScaleClamp(Timer, 0, LifeSpan, 0, FrameCount * playCount);
            frameIndex %= FrameCount;
            Image.Frame = useFrameList ? Frames[frameIndex] : frameIndex + FrameOffset;
        }
        else
        {
            Image.Frame = 0;
        }

        // Control the image scale.
        Image.ScaleX = ScaleX;
        Image.ScaleY = LockScaleRatio ? ScaleX : ScaleY;

        // Determines if the particle faces the direction of its speed vector.
        Image.Angle = MotionAngle ? Util.Angle(SpeedX, SpeedY) : Angle;

        // Control the blend mode.
        Image.Blend = Blend;

        // Color the color.
        if (hasColor)
        {
            Image.Color = hasFinalColor ? Util.LerpColor(initColor, finalColor, colorLerp) : Color;
            Image.Alpha = Alpha;
        }
        else
        {
            Image.Color = new Color(ColorR, ColorG, ColorB, Alpha);
        }

        // Flip the image.
        Image.FlippedX = FlipX;
        Image.FlippedY = FlipY;
    }

    /// <summary>
    /// Removed from the Scene.
    /// </summary>
    public override void Removed()
    {
        base.Removed();

        Timer = 0;
        ClearGraphics();
        ActiveCount--;
    }

    public override void SceneEnd()
    {
        base.SceneEnd();

        RemoveSelf();
    }

    #endregion
}
