using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using SFML.System;
using SFML.Window;

using Otter.Utility;

namespace Otter.Core;

/// <summary>
/// Class used for managing input from the keyboard, mouse, and joysticks. Updated by the active Game.
/// </summary>
public class Input
{

    #region Static Fields

    /// <summary>
    /// A reference to the current active instance.
    /// </summary>
    public static Input Instance { get; set; }

    #endregion

    #region Static Properties

    /// <summary>
    /// The current number of joysticks connected.
    /// </summary>
    public static int JoysticksConnected
    {
        get
        {
            var count = 0;
            for (uint i = 0; i < Joystick.Count; i++)
            {
                if (Joystick.IsConnected(i))
                {
                    count++;
                }
            }
            return count;
        }
    }

    /// <summary>
    /// The maximum possible amount of joysticks that can be connected.
    /// </summary>
    public static int JoysticksSupported => (int)Joystick.Count;

    #endregion

    #region Static Methods

    /// <summary>
    /// Convert a char to a Key code.
    /// </summary>
    /// <param name="key">The char to convert into a Key.</param>
    /// <returns>The Key.  Returns Key.Unknown if no match is found.</returns>
    public static Key CharToKey(char key)
    {
        key = char.ToUpper(key);
        return key switch
        {
            'A' => Key.A,
            'B' => Key.B,
            'C' => Key.C,
            'D' => Key.D,
            'E' => Key.E,
            'F' => Key.F,
            'G' => Key.G,
            'H' => Key.H,
            'I' => Key.I,
            'J' => Key.J,
            'K' => Key.K,
            'L' => Key.L,
            'M' => Key.M,
            'N' => Key.N,
            'O' => Key.O,
            'P' => Key.P,
            'Q' => Key.Q,
            'R' => Key.R,
            'S' => Key.S,
            'T' => Key.T,
            'U' => Key.U,
            'V' => Key.V,
            'W' => Key.W,
            'X' => Key.X,
            'Y' => Key.Y,
            'Z' => Key.Z,
            '0' => Key.Num0,
            '1' => Key.Num1,
            '2' => Key.Num2,
            '3' => Key.Num3,
            '4' => Key.Num4,
            '5' => Key.Num5,
            '6' => Key.Num6,
            '7' => Key.Num7,
            '8' => Key.Num8,
            '9' => Key.Num9,
            '[' => Key.LBracket,
            ']' => Key.RBracket,
            ';' => Key.SemiColon,
            ',' => Key.Comma,
            '.' => Key.Period,
            '/' => Key.Slash,
            '\\' => Key.BackSlash,
            '~' => Key.Tilde,
            '=' => Key.Equal,
            '+' => Key.Add,
            '-' => Key.Dash,
            ' ' => Key.Space,
            _ => Key.Unknown,
        };
    }

    /// <summary>
    /// Get the name of the Joystick.
    /// </summary>
    /// <param name="id">The connection id of the Joystick.</param>
    /// <returns>The name of the Joystick.</returns>
    public static string GetJoystickName(int id)
    {
        return Joystick.GetIdentification((uint)id).Name;
    }

    /// <summary>
    /// Get the vendor id of the Joystick.
    /// </summary>
    /// <param name="id">The connection id of the Joystick.</param>
    /// <returns>The vendor id of the Joystick.</returns>
    public static int GetJoystickVendorId(int id)
    {
        return (int)Joystick.GetIdentification((uint)id).VendorId;
    }

    /// <summary>
    /// Get the product id of the Joystick.
    /// </summary>
    /// <param name="id">The connection id of the Joystick.</param>
    /// <returns>The name of the Joystick.</returns>
    public static int GetJoystickProductId(int id)
    {
        return (int)Joystick.GetIdentification((uint)id).ProductId;
    }

    #endregion

    #region Private Fields

    private float currentMouseWheelDelta;
    private int
        keysPressed,
        currentKeysPressed,
        prevKeysPressed,
        mouseButtonsPressed,
        currentMouseButtonsPressed,
        prevMouseButtonsPressed;
    private readonly List<int>
        buttonsPressed = [],
        prevButtonsPressed = [];

    internal bool bufferReleases = true;
    private readonly Dictionary<Key, bool> activeKeys = [];
    private Dictionary<Key, bool> currentKeys = [];
    private Dictionary<Key, bool> previousKeys = [];
    private readonly Dictionary<MouseButton, bool> activeMouseButtons = [];
    private Dictionary<MouseButton, bool> currentMouseButtons = [];
    private Dictionary<MouseButton, bool> previousMouseButtons = [];
    private readonly List<Dictionary<uint, bool>> activeButtons = [];
    private readonly List<Dictionary<uint, bool>> currentButtons = [];
    private readonly List<Dictionary<uint, bool>> previousButtons = [];
    private readonly List<Key> keyReleaseBuffer = [];
    private readonly List<MouseButton> mouseReleaseBuffer = [];
    private readonly List<List<uint>> buttonReleaseBuffer = [];
    private readonly Dictionary<JoyAxis, float> axisThreshold = [];
    private readonly Dictionary<JoyAxis, AxisSet> axisSet = [];

    private struct AxisSet
    {
        public AxisButton Plus;
        public AxisButton Minus;
    };

    private int mouseDeltaBufferX;
    private int mouseDeltaBufferY;

    #endregion

    #region Public Fields

    /// <summary>
    /// The maximum size of the string of recent key presses.
    /// </summary>
    public static int KeystringSize { get; set; } = 500;

    /// <summary>
    /// Determines if the mouse should be locked to the center of the screen.
    /// </summary>
    public static bool CenteredMouse { get; set; } = false;

    /// <summary>
    /// The current string of keys that were pressed.
    /// </summary>
    public string KeyString = "";

    #endregion

    #region Public Properties

    /// <summary>
    /// The reference to the game that owns this class.
    /// </summary>
    public Game Game { get; internal set; }

    /// <summary>
    /// The last known key that was pressed.
    /// </summary>
    public Key LastKey { get; private set; }

    /// <summary>
    /// The last known mouse button that was pressed.
    /// </summary>
    public MouseButton LastMouseButton { get; private set; }

    /// <summary>
    /// The last known button pressed on each joystick.
    /// </summary>
    public List<int> LastButton { get; private set; }

    /// <summary>
    /// The X movement of the mouse since the last update.  Only updates if the mouse is locked inside the Game window.
    /// </summary>
    public int MouseDeltaX { get; private set; }

    /// <summary>
    /// The Y movement of the mouse since the last update.  Only updates if the mouse is locked inside the Game window.
    /// </summary>
    public int MouseDeltaY { get; private set; }

    /// <summary>
    /// The current X position of the mouse.
    /// </summary>
    public int MouseX
    {
        get
        {
            float pos;
            if (Game.LockMouseCenter)
            {
                pos = GameMouseX;
            }
            else
            {
                pos = Mouse.GetPosition(Game.Window).X;
                pos -= Game.Surface.X - (Game.Surface.ScaledWidth * 0.5f);
                pos /= Game.Surface.ScaleX;
            }

            return (int)pos;
        }
    }

    /// <summary>
    /// The current Y position of the mouse.
    /// </summary>
    public int MouseY
    {
        get
        {
            float pos;
            if (Game.LockMouseCenter)
            {
                pos = GameMouseY;
            }
            else
            {
                pos = Mouse.GetPosition(Game.Window).Y;
                pos -= Game.Surface.Y - (Game.Surface.ScaledHeight * 0.5f);
                pos /= Game.Surface.ScaleY;
            }

            return (int)pos;
        }
    }

    /// <summary>
    /// The raw X position of the mouse.  This can be set.
    /// </summary>
    public int MouseRawX
    {
        get => Game.LockMouseCenter ? GameMouseX : Mouse.GetPosition(Game.Window).X;
        set
        {
            if (Game.LockMouseCenter)
            {
                GameMouseX = value;
            }
            else
            {
                Mouse.SetPosition(new Vector2i(value, MouseRawY), Game.Window);
            }
        }
    }

    /// <summary>
    /// The raw Y position of the mouse.  This can be set.
    /// </summary>
    public int MouseRawY
    {
        get => Game.LockMouseCenter ? GameMouseY : Mouse.GetPosition(Game.Window).Y;
        set
        {
            if (Game.LockMouseCenter)
            {
                GameMouseY = value;
            }
            else
            {
                Mouse.SetPosition(new Vector2i(MouseRawX, value), Game.Window);
            }
        }
    }

    /// <summary>
    /// The X position of the mouse in screen space.
    /// </summary>
    public float MouseScreenX => MouseX + Game.Scene.CameraX;

    /// <summary>
    /// The Y position of the mouse in screen space.
    /// </summary>
    public float MouseScreenY => MouseY + Game.Scene.CameraY;

    /// <summary>
    /// The change in the mouse wheel value this update.
    /// </summary>
    public float MouseWheelDelta { get; private set; }

    /// <summary>
    /// The X position of the mouse in the game.  Use if the mouse is locked in the game window.
    /// This can be set if the mouse is locked inside the game window.
    /// </summary>
    public int GameMouseX { get; set; }

    /// <summary>
    /// The Y position of the mouse in the game.  Use if the mouse is locked in the game window.
    /// This can be set if the mouse is locked inside the game window.
    /// </summary>
    public int GameMouseY { get; set; }

    #endregion

    #region Constructors

    internal Input(Game game)
    {
        Game = game;
        Instance = this;
        Init();
    }

    #endregion

    #region Private Methods

    private void OnJoystickConnected(object sender, JoystickConnectEventArgs e)
    {
    }

    private void OnTextEntered(object sender, TextEventArgs e)
    {
        //convert unicode to ascii to check range later
        var hexValue = Encoding.ASCII.GetBytes(e.Unicode)[0].ToString("X");
        var ascii = int.Parse(hexValue, NumberStyles.HexNumber);

        if (e.Unicode == "\b")
        {
            if (KeyString.Length > 0)
            {
                KeyString = KeyString.Remove(KeyString.Length - 1, 1);
            }
        }
        else if (e.Unicode == "\r")
        {
            KeyString += "\n";
        }
        else if (ascii is >= 32 and < 128)
        { //only add to keystring if actual character
            KeyString += e.Unicode;
        }
    }

    private void OnKeyPressed(object sender, KeyEventArgs e)
    {
        if (Game.Debugger != null)
        {
            // Ignore presses from the debug toggle key.
            if ((Key)e.Code == Game.Debugger.ToggleKey)
            {
                return;
            }
        }

        if (!activeKeys[(Key)e.Code])
        {
            keysPressed++;
        }
        activeKeys[(Key)e.Code] = true;
        LastKey = (Key)e.Code;
    }

    private void OnKeyReleased(object sender, KeyEventArgs e)
    {
        if (bufferReleases)
        {
            keyReleaseBuffer.Add((Key)e.Code);
        }
        else
        {
            activeKeys[(Key)e.Code] = false;
        }
        keysPressed--;
    }

    private void OnMousePressed(object sender, MouseButtonEventArgs e)
    {
        activeMouseButtons[(MouseButton)e.Button] = true;
        mouseButtonsPressed++;
        LastMouseButton = (MouseButton)e.Button;
    }

    private void OnMouseReleased(object sender, MouseButtonEventArgs e)
    {
        if (bufferReleases)
        {
            mouseReleaseBuffer.Add((MouseButton)e.Button);
        }
        else
        {
            activeMouseButtons[(MouseButton)e.Button] = false;
        }
        mouseButtonsPressed--;
    }

    private void OnButtonPressed(object sender, JoystickButtonEventArgs e)
    {
        activeButtons[(int)e.JoystickId][e.Button] = true;
        buttonsPressed[(int)e.JoystickId]++;
        LastButton[(int)e.JoystickId] = (int)e.Button;
        //Console.WriteLine("{0} pressed on joy {1}", e.Button, e.JoystickId);
    }

    private void OnButtonReleased(object sender, JoystickButtonEventArgs e)
    {
        if (bufferReleases)
        {
            buttonReleaseBuffer[(int)e.JoystickId].Add(e.Button);
        }
        else
        {
            activeButtons[(int)e.JoystickId][e.Button] = false;
        }
        buttonsPressed[(int)e.JoystickId]--;
    }

    private void OnJoystickMoved(object sender, JoystickMoveEventArgs e)
    {
        //Console.WriteLine("Joystick " + e.JoystickId + " moved axis " + e.Axis + " to " + e.Position);
    }

    private void OnMouseWheelScrolled(object sender, MouseWheelScrollEventArgs e)
    {
        currentMouseWheelDelta = e.Delta;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Check if a key has been pressed this update.
    /// </summary>
    /// <param name="k">The key to check.</param>
    /// <returns>True if the key has been pressed.</returns>
    public bool KeyPressed(Key k)
    {
        return k switch
        {
            Key.Any => keysPressed > prevKeysPressed,
            _ => currentKeys[k] && !previousKeys[k]
        };
    }

    /// <summary>
    /// Check if a key has been pressed this update.
    /// </summary>
    /// <param name="c">The key to check.</param>
    /// <returns>True if the key has been pressed.</returns>
    public bool KeyPressed(char c)
    {
        return KeyPressed(CharToKey(c));
    }

    /// <summary>
    /// Check if a key has been released this update.
    /// </summary>
    /// <param name="k">The key to check.</param>
    /// <returns>True if the key has been released.</returns>
    public bool KeyReleased(Key k)
    {
        return k switch
        {
            Key.Any => keysPressed < prevKeysPressed,
            _ => !currentKeys[k] && previousKeys[k]
        };
    }

    /// <summary>
    /// Check if a key has been released this update.
    /// </summary>
    /// <param name="c">The key to check.</param>
    /// <returns>True if the key has been released.</returns>
    public bool KeyReleased(char c)
    {
        return KeyReleased(CharToKey(c));
    }

    /// <summary>
    /// Check if a key is currently down.
    /// </summary>
    /// <param name="k">The key to check.</param>
    /// <returns>True if the key is down.</returns>
    public bool KeyDown(Key k)
    {
        return k switch
        {
            Key.Any => keysPressed > 0,
            _ => currentKeys[k]
        };
    }

    /// <summary>
    /// Check if a key is currently down.
    /// </summary>
    /// <param name="c">The key to check.</param>
    /// <returns>True if the key is down.</returns>
    public bool KeyDown(char c)
    {
        return KeyDown(CharToKey(c));
    }

    /// <summary>
    /// Check if a key is currently up.
    /// </summary>
    /// <param name="k">The key to check.</param>
    /// <returns>True if the key is up.</returns>
    public bool KeyUp(Key k)
    {
        return !KeyDown(k);
    }

    /// <summary>
    /// Check if a key is currently up.
    /// </summary>
    /// <param name="c">The key to check.</param>
    /// <returns>True if the key is up.</returns>
    public bool KeyUp(char c)
    {
        return KeyUp(CharToKey(c));
    }

    /// <summary>
    /// Check if a joystick button is pressed.
    /// </summary>
    /// <param name="button">The button to check.</param>
    /// <param name="joystick">The joystick to check.</param>
    /// <returns>True if the button is pressed.</returns>
    public bool ButtonPressed(int button, int joystick = 0)
    {
        return currentButtons[joystick][(uint)button] && !previousButtons[joystick][(uint)button];
    }

    /// <summary>
    /// Check if a joystick AxisButton is pressed.
    /// </summary>
    /// <param name="button">The AxisButton to check.</param>
    /// <param name="joystick">The joystick to check.</param>
    /// <returns>True if the button is pressed.</returns>
    public bool ButtonPressed(AxisButton button, int joystick = 0)
    {
        return ButtonPressed((int)button, joystick);
    }

    /// <summary>
    /// Check if the joystick button is released.
    /// </summary>
    /// <param name="button">The button to check.</param>
    /// <param name="joystick">The joystick to check.</param>
    /// <returns>True if the button is released.</returns>
    public bool ButtonReleased(int button, int joystick = 0)
    {
        return !currentButtons[joystick][(uint)button] && previousButtons[joystick][(uint)button];
    }

    /// <summary>
    /// Check if the joystick AxisButton is released.
    /// </summary>
    /// <param name="button">The AxisButton to check.</param>
    /// <param name="joystick">The joystick to check.</param>
    /// <returns>True if the AxisButton is released.</returns>
    public bool ButtonReleased(AxisButton button, int joystick = 0)
    {
        return ButtonReleased((int)button, joystick);
    }

    /// <summary>
    /// Check if the joystick button is down.
    /// </summary>
    /// <param name="button">The button to check.</param>
    /// <param name="joystick">The joystick to check.</param>
    /// <returns>True if the button is down.</returns>
    public bool ButtonDown(int button, int joystick = 0)
    {
        return currentButtons[joystick][(uint)button];
    }

    /// <summary>
    /// Check if the joystick AxisButton is down.
    /// </summary>
    /// <param name="button">The AxisButton to check.</param>
    /// <param name="joystick">The joystick to check.</param>
    /// <returns>True if the AxisButton is down.</returns>
    public bool ButtonDown(AxisButton button, int joystick = 0)
    {
        return ButtonDown((int)button, joystick);
    }

    /// <summary>
    /// Check if the joystick button is up.
    /// </summary>
    /// <param name="button">The button to check.</param>
    /// <param name="joystick">The joystick to check.</param>
    /// <returns>True if the button is up.</returns>
    public bool ButtonUp(int button, int joystick = 0)
    {
        return !currentButtons[joystick][(uint)button];
    }

    /// <summary>
    /// Check if the joystick button is up.
    /// </summary>
    /// <param name="button">The AxisButton to check.</param>
    /// <param name="joystick">The joystick to check.</param>
    /// <returns>True if the AxisButton is up.</returns>
    public bool ButtonUp(AxisButton button, int joystick = 0)
    {
        return ButtonUp((int)button, joystick);
    }

    /// <summary>
    /// Get the value of a joystick axis from -100 to 100.
    /// </summary>
    /// <param name="axis">The axis to check.</param>
    /// <param name="joystick">The joystick to check.</param>
    /// <returns>The axis value from -100 to 100.</returns>
    public static float GetAxis(JoyAxis axis, int joystick = 0)
    {
        return !Joystick.HasAxis((uint)joystick, (Joystick.Axis)axis)
            ? 0
            : axis switch
            {
                JoyAxis.PovY => Joystick.GetAxisPosition((uint)joystick, (Joystick.Axis)axis) * -1,
                _ => Joystick.GetAxisPosition((uint)joystick, (Joystick.Axis)axis)
            };
    }

    /// <summary>
    /// Set the threshold for an axis to act as an AxisButton.  Defaults to 50 or one half of the joystick's total range.
    /// </summary>
    /// <param name="axis">The JoyAxis to set.</param>
    /// <param name="threshold">The threshold that the axis must pass to act as a button press.</param>
    public void SetAxisThreshold(JoyAxis axis, float threshold)
    {
        axisThreshold[axis] = threshold;
    }

    /// <summary>
    /// Gets the axis threshold for an axis to act as an AxisButton.
    /// </summary>
    /// <param name="axis">The JoyAxis.</param>
    public float GetAxisThreshold(JoyAxis axis)
    {
        return axisThreshold[axis];
    }

    /// <summary>
    /// Check if a MouseButton is pressed.
    /// </summary>
    /// <param name="b">The MouseButton to check.</param>
    /// <returns>True if the MouseButton is pressed.</returns>
    public bool MouseButtonPressed(MouseButton b)
    {
        return b switch
        {
            MouseButton.Any => mouseButtonsPressed > prevMouseButtonsPressed,
            _ => currentMouseButtons[b] && !previousMouseButtons[b]
        };
    }

    /// <summary>
    /// Check if a MouseButton is pressed.
    /// </summary>
    /// <param name="b">The MouseButton to check.</param>
    /// <returns>True if the MouseButton is pressed.</returns>
    public bool MouseButtonReleased(MouseButton b)
    {
        return b switch
        {
            MouseButton.Any => mouseButtonsPressed < prevMouseButtonsPressed,
            _ => !currentMouseButtons[b] && previousMouseButtons[b]
        };
    }

    /// <summary>
    /// Check if a MouseButton is pressed.
    /// </summary>
    /// <param name="b">The MouseButton to check.</param>
    /// <returns>True if the MouseButton is pressed.</returns>
    public bool MouseButtonDown(MouseButton b)
    {
        return b switch
        {
            MouseButton.Any => mouseButtonsPressed > 0,
            _ => currentMouseButtons[b]
        };
    }

    /// <summary>
    /// Check if a MouseButton is pressed.
    /// </summary>
    /// <param name="b">The MouseButton to check.</param>
    /// <returns>True if the MouseButton is pressed.</returns>
    public bool MouseButtonUp(MouseButton b)
    {
        return !MouseButtonDown(b);
    }

    /// <summary>
    /// Clear the string of recently pressed keys.
    /// </summary>
    public void ClearKeystring()
    {
        KeyString = "";
    }

    #endregion

    #region Internal

    internal void WindowInit()
    {
        Game.Window.KeyPressed += OnKeyPressed;
        Game.Window.TextEntered += OnTextEntered;
        Game.Window.MouseButtonPressed += OnMousePressed;
        Game.Window.KeyReleased += OnKeyReleased;
        Game.Window.MouseButtonReleased += OnMouseReleased;
        Game.Window.JoystickButtonPressed += OnButtonPressed;
        Game.Window.JoystickButtonReleased += OnButtonReleased;
        Game.Window.JoystickConnected += OnJoystickConnected;
        Game.Window.JoystickMoved += OnJoystickMoved;
        Game.Window.MouseWheelScrolled += OnMouseWheelScrolled;
    }

    internal void GameMouseUpdate(int x, int y)
    {
        mouseDeltaBufferX += x;
        mouseDeltaBufferY += y;
        GameMouseX += x;
        GameMouseY += y;
        GameMouseX = (int)Util.Clamp(GameMouseX, 0, Game.Width);
        GameMouseY = (int)Util.Clamp(GameMouseY, 0, Game.Height);
    }

    internal void Init()
    {
        LastButton = [];

        for (uint i = 0; i < Joystick.Count; i++)
        {
            activeButtons.Add([]);
            currentButtons.Add([]);
            previousButtons.Add([]);

            for (uint j = 0; j < Joystick.ButtonCount; j++)
            {
                activeButtons[(int)i][j] = false;
                currentButtons[(int)i][j] = false;
                previousButtons[(int)i][j] = false;
            }
            foreach (AxisButton axisButton in Enum.GetValues<AxisButton>())
            {
                activeButtons[(int)i][(uint)axisButton] = false;
                currentButtons[(int)i][(uint)axisButton] = false;
                previousButtons[(int)i][(uint)axisButton] = false;
            }

            buttonsPressed.Add(0);
            prevButtonsPressed.Add(0);

            LastButton.Add(-1);

            buttonReleaseBuffer.Add([]);

            Joystick.Update();
        }

        foreach (Key key in Enum.GetValues<Key>())
        {
            activeKeys.Add(key, false);
            currentKeys.Add(key, false);
            previousKeys.Add(key, false);
        }

        foreach (MouseButton button in Enum.GetValues<MouseButton>())
        {
            activeMouseButtons.Add(button, false);
            currentMouseButtons.Add(button, false);
            previousMouseButtons.Add(button, false);
        }

        foreach (JoyAxis axis in Enum.GetValues<JoyAxis>())
        {
            axisThreshold.Add(axis, 0.5f);
        }

        AxisSet ax;

        ax.Plus = AxisButton.XPlus;
        ax.Minus = AxisButton.XMinus;
        axisSet.Add(JoyAxis.X, ax);

        ax.Plus = AxisButton.YPlus;
        ax.Minus = AxisButton.YMinus;
        axisSet.Add(JoyAxis.Y, ax);

        ax.Plus = AxisButton.ZPlus;
        ax.Minus = AxisButton.ZMinus;
        axisSet.Add(JoyAxis.Z, ax);

        ax.Plus = AxisButton.RPlus;
        ax.Minus = AxisButton.RMinus;
        axisSet.Add(JoyAxis.R, ax);

        ax.Plus = AxisButton.UPlus;
        ax.Minus = AxisButton.UMinus;
        axisSet.Add(JoyAxis.U, ax);

        ax.Plus = AxisButton.VPlus;
        ax.Minus = AxisButton.VMinus;
        axisSet.Add(JoyAxis.V, ax);

        ax.Plus = AxisButton.PovXPlus;
        ax.Minus = AxisButton.PovXMinus;
        axisSet.Add(JoyAxis.PovX, ax);

        ax.Plus = AxisButton.PovYPlus;
        ax.Minus = AxisButton.PovYMinus;
        axisSet.Add(JoyAxis.PovY, ax);

    }

    internal void Update()
    {
        // Set instance pointer to this object.
        Instance = this;

        // Do mouse delta stuff for when the mouse is locked in the game window.
        MouseDeltaX = mouseDeltaBufferX;
        MouseDeltaY = mouseDeltaBufferY;

        mouseDeltaBufferX = 0;
        mouseDeltaBufferY = 0;

        // Force update all joysticks.
        Joystick.Update();

        // Update the previous button dictionaries.
        previousKeys = new Dictionary<Key, bool>(currentKeys);
        previousMouseButtons = new Dictionary<MouseButton, bool>(currentMouseButtons);
        for (var i = 0; i < previousButtons.Count; i++)
        {
            previousButtons[i] = new Dictionary<uint, bool>(currentButtons[i]);
        }

        // Update the previous press counts
        prevKeysPressed = currentKeysPressed;
        prevMouseButtonsPressed = currentMouseButtonsPressed;
        for (var i = 0; i < prevButtonsPressed.Count; i++)
        {
            prevButtonsPressed[i] = buttonsPressed[i];
        }

        // Update the current to the active keys.
        currentKeys = new Dictionary<Key, bool>(activeKeys);
        currentMouseButtons = new Dictionary<MouseButton, bool>(activeMouseButtons);
        for (var i = 0; i < currentButtons.Count; i++)
        {
            currentButtons[i] = new Dictionary<uint, bool>(activeButtons[i]);
        }

        foreach (var k in keyReleaseBuffer)
        {
            activeKeys[k] = false;
        }

        currentKeysPressed = keysPressed;

        keyReleaseBuffer.Clear();

        foreach (var m in mouseReleaseBuffer)
        {
            activeMouseButtons[m] = false;
        }

        currentMouseButtonsPressed = mouseButtonsPressed;

        mouseReleaseBuffer.Clear();

        for (var i = 0; i < Joystick.Count; i++)
        {
            foreach (var b in buttonReleaseBuffer[i])
            {
                activeButtons[i][b] = false;
            }

            buttonReleaseBuffer[i].Clear();
        }

        // Update the Joystick axes to use as buttons
        for (var i = 0; i < Joystick.Count; i++)
        {
            if (Joystick.IsConnected((uint)i))
            {
                foreach (JoyAxis axis in Enum.GetValues<JoyAxis>())
                {
                    var a = GetAxis(axis, i) * 0.01f;
                    if (a >= axisThreshold[axis])
                    {
                        if (!currentButtons[i][(uint)axisSet[axis].Plus])
                        {
                            buttonsPressed[i]++;
                        }
                        currentButtons[i][(uint)axisSet[axis].Plus] = true;
                    }
                    else
                    {
                        if (currentButtons[i][(uint)axisSet[axis].Plus])
                        {
                            buttonsPressed[i]--;
                        }
                        currentButtons[i][(uint)axisSet[axis].Plus] = false;
                    }

                    if (a <= -axisThreshold[axis])
                    {
                        if (!currentButtons[i][(uint)axisSet[axis].Minus])
                        {
                            buttonsPressed[i]++;
                        }
                        currentButtons[i][(uint)axisSet[axis].Minus] = true;
                    }
                    else
                    {
                        if (currentButtons[i][(uint)axisSet[axis].Minus])
                        {
                            buttonsPressed[i]--;
                        }
                        currentButtons[i][(uint)axisSet[axis].Minus] = false;
                    }
                }
            }
        }

        MouseWheelDelta = currentMouseWheelDelta;
        currentMouseWheelDelta = 0;
    }

    #endregion

}
