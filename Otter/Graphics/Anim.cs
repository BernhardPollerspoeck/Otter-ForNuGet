using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Otter.Core;

namespace Otter.Graphics;

/// <summary>
/// Class used for animations in Spritemap.
/// </summary>
public partial class Anim
{
    #region Static Methods

    /// <summary>
    /// Returns an array of numbers from min to max.  Useful for passing in arguments for long animations.
    /// </summary>
    /// <param name="min">The start of the animation (includes this number.)</param>
    /// <param name="max">The end of the animation (includes this number.)</param>
    /// <returns>The array of ints representing an animation.</returns>
    public static int[] FramesRange(int min, int max)
    {
        var f = new int[max - min + 1];
        for (var i = min; i <= max; i++)
        {
            f[i - min] = i;
        }
        return f;
    }

    /// <summary>
    /// Creates an array of frames from a string expression. The expression must be similar to the following format:
    /// "0,3,7-11,2,5"
    /// Whitespace is permitted, and commas are optional.
    /// <param name="input">A string formatted as above, describing the frames to generate.</param>
    /// </summary>
    public static int[] ParseFrames(string input)
    {
        // Make sure the pattern matches, and alert the user if it doesn't
        var parse = SyntaxCheck.Match(input);
        if (!parse.Success)
        {
            throw new Exception(string.Format("Invalid format: {0}", input));
        }

        // Get all numbers/ranges in the input string.
        var frames = new List<int>();
        foreach (Match match in GetMatches.Matches(input))
        {
            var range = GetRange.Match(match.Value);
            if (range.Success)
            {
                var from = int.Parse(range.Groups[1].Value);
                var to = int.Parse(range.Groups[2].Value);

                // Support ascending and descending ranges
                if (from < to)
                {
                    while (from <= to)
                    {
                        frames.Add(from++);
                    }
                }
                else
                {
                    while (from >= to)
                    {
                        frames.Add(from--);
                    }
                }
            }
            else
            {
                frames.Add(int.Parse(match.Value));
            }
        }

        return [.. frames];
    }

    #endregion

    #region Private Static Functions

    // Matches strings containing only numbers and number ranges, separated by single commas and/or whitespace
    private static readonly Regex SyntaxCheck = MyRegex1();

    // Extracts a number or number range from a string
    private static readonly Regex GetMatches = MyRegex();

    // Extracts two numbers from a string containing a range
    private static readonly Regex GetRange = MyRegex2();

    #endregion

    #region Private Fields

    private bool pingPong;
    private int loopBack = 0;
    private float timer;
    private float delay;
    private int direction;
    private int repeatsCounted = 0;

    #endregion

    #region Public Fields

    /// <summary>
    /// An action to run when the animation finishes playing.
    /// </summary>
    public Action OnComplete = delegate { };

    /// <summary>
    /// An action that is called when the Anim switches to a new frame.
    /// </summary>
    public Action OnNewFrame = delegate { };

    /// <summary>
    /// Determines if the animation is active (playing.)
    /// </summary>
    public bool Active;

    #endregion

    #region Public Properties

    /// <summary>
    /// The overall playback speed of the animation.
    /// </summary>
    public float PlaybackSpeed { get; private set; }

    /// <summary>
    /// The repeat count of the animation.
    /// </summary>
    public int RepeatCount { get; private set; }

    /// <summary>
    /// The frames used in the animation.
    /// </summary>
    public List<int> Frames { get; private set; }

    /// <summary>
    /// The frame delays used in the animation.
    /// </summary>
    public List<float> FrameDelays { get; private set; }

    /// <summary>
    /// The total number of frames in this animation.
    /// </summary>
    public int FrameCount => Frames.Count;

    /// <summary>
    /// The current frame of the animation.
    /// </summary>
    public int CurrentFrame => Frames[CurrentFrameIndex];

    /// <summary>
    /// The current frame index of the animation.
    /// </summary>
    public int CurrentFrameIndex { get; set; }

    /// <summary>
    /// The total duration of the animation.
    /// </summary>
    public float TotalDuration
    {
        get
        {
            float delay = 0;
            foreach (var d in FrameDelays)
            {
                delay += d;
            }
            return delay;
        }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new Anim with an array of ints for frames, and an array of floats for frameDelays.
    /// </summary>
    /// <param name="frames">The frames from the sprite sheet to display.</param>
    /// <param name="frameDelays">The time that each frame should be displayed.</param>
    public Anim(int[] frames, float[] frameDelays = null)
    {
        Initialize(frames, frameDelays);
    }

    /// <summary>
    /// Creates a new Anim with a string of ints for frames, and a string of floats for frameDelays.
    /// </summary>
    /// <param name="frames">A string of frames separated by a delim character.  Example: "0,1,2-7,9,11"</param>
    /// <param name="frameDelays">A string of floats separated by a delim character.  Example: "0.5f,1,0.5f,1"</param>
    /// <param name="delim">The string of characters to parse the string by.  Default is ","</param>
    public Anim(string frames, string frameDelays)
    {
        var frameDelaysParts = MyRegex3().Split(frameDelays.Replace(" ", ""));
        var framedelaysfloat = new float[frameDelaysParts.Length];

        for (var i = 0; i < frameDelaysParts.Length; i++)
        {
            framedelaysfloat[i] = float.Parse(frameDelaysParts[i]);
        }

        Initialize(ParseFrames(frames), framedelaysfloat);
    }

    #endregion

    #region Private Methods

    private void Initialize(int[] frames, float[] frameDelays = null)
    {
        Frames = [];
        FrameDelays = [];

        for (var i = 0; i < frames.Length; i++)
        {
            Frames.Add(frames[i]);
            if (frameDelays != null)
            {
                if (i >= frameDelays.Length)
                {
                    FrameDelays.Add(frameDelays[i % frameDelays.Length]);
                }
                else
                {
                    FrameDelays.Add(frameDelays[i]);
                }
            }
            else
            {
                FrameDelays.Add(1);
            }
        }

        RepeatCount = -1;
        timer = 0;
        delay = 0;
        direction = 1;
        PlaybackSpeed = 1;

        Active = true;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Determines how many times this animation loops.  -1 for infinite.
    /// </summary>
    /// <param name="times">How many times the animation should repeat.</param>
    /// <returns>The anim object.</returns>
    public Anim Repeat(int times = -1)
    {
        RepeatCount = times;
        return this;
    }

    /// <summary>
    /// Disables repeating.  Animations default to repeat on.
    /// </summary>
    /// <returns>The anim object.</returns>
    public Anim NoRepeat()
    {
        RepeatCount = 0;
        return this;
    }

    /// <summary>
    /// Determines if the animation will repeat by going back and forth between the start and end.
    /// </summary>
    /// <param name="pingpong">True for yes, false for no no no.</param>
    /// <returns>The anim object.</returns>
    public Anim PingPong(bool pingpong = true)
    {
        pingPong = pingpong;
        return this;
    }

    /// <summary>
    /// Determines the playback speed of the animation.  1 = 1 frame.
    /// </summary>
    /// <param name="speed">The new speed.</param>
    /// <returns>The anim object.</returns>
    public Anim Speed(float speed)
    {
        PlaybackSpeed = speed;
        return this;
    }

    /// <summary>
    /// Determines which frame the animation will loop back to when it repeats.
    /// </summary>
    /// <param name="frame">The frame to loop back to (from 0 to frame count - 1)</param>
    /// <returns>The anim object.</returns>
    public Anim LoopBackTo(int frame = 0)
    {
        loopBack = frame;
        return this;
    }

    /// <summary>
    /// Stops the animation and returns it to the first frame.
    /// </summary>
    /// <returns>The anim object.</returns>
    public Anim Stop()
    {
        Active = false;
        CurrentFrameIndex = 0;
        return this;
    }

    /// <summary>
    /// Resets the animation back to frame 0 but does not stop it.
    /// </summary>
    /// <returns>The anim object.</returns>
    public Anim Reset()
    {
        timer = 0;
        CurrentFrameIndex = 0;
        return this;
    }

    /// <summary>
    /// Updates the Anim object.  Handled by the Spritemap usually.  If this doesn't run the animation will not play.
    /// </summary>
    /// <param name="t">The time scale.</param>
    public void Update(float t = 1f)
    {
        if (Active)
        {
            timer += PlaybackSpeed * Game.Instance.DeltaTime * t;
        }

        delay = FrameDelays[CurrentFrameIndex];

        while (timer >= delay)
        {
            timer -= delay;
            CurrentFrameIndex += direction;

            if (CurrentFrameIndex == Frames.Count)
            {
                if (repeatsCounted < RepeatCount || RepeatCount < 0)
                {
                    repeatsCounted++;
                    if (pingPong)
                    {
                        direction *= -1;
                        CurrentFrameIndex = Frames.Count - 2;
                    }
                    else
                    {
                        CurrentFrameIndex = loopBack;
                    }
                }
                else
                {
                    if (pingPong)
                    {
                        direction *= -1;
                        CurrentFrameIndex = Frames.Count - 2;
                    }
                    else
                    {
                        OnComplete();
                        Stop();
                        CurrentFrameIndex = Frames.Count - 1;
                    }
                }
            }

            if (CurrentFrameIndex < loopBack)
            {
                if (pingPong)
                {
                    if (repeatsCounted < RepeatCount || RepeatCount < 0)
                    {
                        repeatsCounted++;
                        direction *= -1;
                        CurrentFrameIndex = loopBack + 1;
                    }
                    else
                    {
                        OnComplete();
                        Stop();
                    }
                }
            }

            OnNewFrame();
        }
    }

    [GeneratedRegex(@"((?:\d+\s*-\s*\d+)|(?:\d+))")]
    private static partial Regex MyRegex();
    [GeneratedRegex(@"^(?:\d+\s*-\s*\d+|\d\s*,?\s*)*$")]
    private static partial Regex MyRegex1();
    [GeneratedRegex(@"(\d+)-(\d+)")]
    private static partial Regex MyRegex2();
    [GeneratedRegex(",")]
    private static partial Regex MyRegex3();

    #endregion
}
