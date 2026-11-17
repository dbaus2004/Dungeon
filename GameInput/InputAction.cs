// Adapted from the Game State Management sample from Microsoft XNA Game Studio 4.0
// Archived at https://github.com/SimonDarksideJ/GameStateManagementSample

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Dungeon.Input;

/// <summary>
/// Represents an input action that can be triggered by specific buttons or keys.
/// </summary>  
/// <remarks>
/// An input action is a set of buttons or keys that translate to a single action in the
/// game. This is useful for mapping multiple input methods to a single game action
/// (i.e. "W" and Up Arrow and up on theD-Pad).
/// </remarks>
public class InputAction
{
    private readonly bool _leftMouseButton;
    private readonly bool _rightMouseButton;
    private readonly Buttons[] _buttons;
    private readonly Keys[] _keys;
    private readonly bool _firstPressOnly;

    private delegate bool ButtonPress(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex);
    private delegate bool KeyPress(Keys key);

    public InputAction(
        Buttons[] triggerButtons,
        Keys[] triggerKeys,
        bool firstPressOnly,
        bool leftMouseButton = false,
        bool rightMouseButton = false
    )
    {
        _buttons = triggerButtons is null ? Array.Empty<Buttons>() : (Buttons[])triggerButtons.Clone();
        _keys = triggerKeys is null ? Array.Empty<Keys>() : (Keys[])triggerKeys.Clone();
        _firstPressOnly = firstPressOnly;
        _leftMouseButton = leftMouseButton;
        _rightMouseButton = rightMouseButton;
    }

    /// <summary>
    /// Determines if the input action has occurred based on the current input state.
    /// </summary>
    /// <param name="inputState">Input state to check</param>
    /// <param name="controllingPlayer">The player who is controlling the input</param>
    /// <param name="playerIndex">The index of the player who triggered the input</param>
    /// <returns>True if the input action has occurred, false otherwise</returns>
    public bool Occurred(IInputState inputState, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
    {
        ButtonPress buttonTest = _firstPressOnly
            ? inputState.IsNewButtonPress
            : inputState.IsButtonPressed;
        KeyPress keyTest = _firstPressOnly
            ? inputState.IsNewKeyPress
            : inputState.IsKeyPressed;

        bool leftMousePressed = _firstPressOnly
            ? inputState.IsNewLeftMouseButtonPress()
            : inputState.IsLeftMouseButtonPressed();
        bool rightMousePressed = _firstPressOnly
            ? inputState.IsNewRightMouseButtonPress()
            : inputState.IsRightMouseButtonPressed();

        if ((_leftMouseButton && leftMousePressed) || (_rightMouseButton && rightMousePressed))
        {
            playerIndex = PlayerIndex.One;
            return true;
        }

        foreach (Buttons button in _buttons)
        {
            if (buttonTest(button, controllingPlayer, out playerIndex))
                return true;
        }
        foreach (Keys key in _keys)
        {
            if (keyTest(key))
            {
                playerIndex = PlayerIndex.One; // Keyboard is always player one
                return true;
            }
        }

        // If we get here, no button or key was pressed
        playerIndex = PlayerIndex.One; // Default to player one if no input is detected
        return false;
    }
}