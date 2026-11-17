using Dungeon.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Dungeon.Tests;

public class InputActionTests
{
    [Fact]
    public void FirstPressOnlyDoesNotRepeatWhileKeyIsHeld()
    {
        var input = new StubInputState
        {
            KeyPressed = true,
            NewKeyPress = false
        };
        var action = new InputAction(null, new[] { Keys.Space }, firstPressOnly: true);

        Assert.False(action.Occurred(input, null, out _));
    }

    [Fact]
    public void FirstPressOnlyRespondsToNewKeyPress()
    {
        var input = new StubInputState { NewKeyPress = true };
        var action = new InputAction(null, new[] { Keys.Space }, firstPressOnly: true);

        Assert.True(action.Occurred(input, null, out _));
    }

    private sealed class StubInputState : IInputState
    {
        public bool KeyPressed { get; init; }
        public bool NewKeyPress { get; init; }
        public Point MousePosition => Point.Zero;

        public bool IsLeftMouseButtonPressed() => false;
        public bool IsNewLeftMouseButtonPress() => false;
        public bool IsRightMouseButtonPressed() => false;
        public bool IsNewRightMouseButtonPress() => false;
        public bool IsKeyPressed(Keys key) => KeyPressed;
        public bool IsNewKeyPress(Keys key) => NewKeyPress;
        public bool IsButtonPressed(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            playerIndex = controllingPlayer ?? PlayerIndex.One;
            return false;
        }

        public bool IsNewButtonPress(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            playerIndex = controllingPlayer ?? PlayerIndex.One;
            return false;
        }
    }
}
