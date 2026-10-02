using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
namespace Dungeon.Input;
public interface IInputState
{
    Point MousePosition { get; }
    bool IsLeftMouseButtonPressed();
    bool IsNewLeftMouseButtonPress();
    bool IsRightMouseButtonPressed();
    bool IsNewRightMouseButtonPress();
    bool IsKeyPressed(Keys key);
    bool IsNewKeyPress(Keys key);
    bool IsButtonPressed(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex);
    bool IsNewButtonPress(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex);
}