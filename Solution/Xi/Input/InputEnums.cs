using System.ComponentModel;

namespace Xi
{
    /// <summary>
    /// Represents a four-way direction.
    /// </summary>
    public enum Direction2D
    {
        Up = 0,
        Down,
        Left,
        Right,
        [Browsable(false)]
        Count
    }

    /// <summary>
    /// Represents a six-way direction.
    /// </summary>
    public enum Direction3D
    {
        Up = 0,
        Down,
        Left,
        Right,
        Forward,
        Backward,
        [Browsable(false)]
        Count
    }

    /// <summary>
    /// Represents the type of an input event.
    /// </summary>
    public enum InputType
    {
        ClickDown = 0,
        Down,
        ClickUp,
        //Up, // OPTIMIZATION: can't raise events every frame for every key / button that is up.
        Repeat
    }

    /// <summary>
    /// Represents the type of a game pad button.
    /// </summary>
    public enum GamePadButton
    {
        A = 0,
        B,
        X,
        Y,
        LeftShoulder,
        RightShoulder,
        LeftStick,
        RightStick,
        Back,
        Start
    }

    /// <summary>
    /// Represents the type of semantic input button.
    /// </summary>
    public enum SemanticButtonType
    {
        AffirmButton,
        CancelButton,
        NextPageButton,
        PreviousPageButton,
        [Browsable(false)]
        Count
    }

    /// <summary>
    /// Represents the type of semantic input.
    /// </summary>
    public enum SemanticInputType
    {
        Affirm,
        Cancel,
        NextPage,
        PreviousPage
    }

    /// <summary>
    /// Keyboard modification states.
    /// These are NOT flags.
    /// </summary>
    public enum KeyboardModifier
    {
        None = 0,
        Shift,
        Control,
        ControlShift
    }
}
