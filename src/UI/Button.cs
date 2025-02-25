using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace incremental.UI;

public class Button
{
    public Rectangle Bounds { get; set; }
    public string Text { get; set; }
    public bool WasPressed { get; private set; }
    public bool IsHovered { get; private set; }
    private bool _isPressed;

    public Button(Rectangle bounds, string text)
    {
        Bounds = bounds;
        Text = text;
        WasPressed = false;
        IsHovered = false;
        _isPressed = false;
    }

    public bool Update(MouseState mouseState)
    {
        bool wasPressed = false;
        IsHovered = Bounds.Contains(mouseState.Position);
        bool isDown = mouseState.LeftButton == ButtonState.Pressed;

        if (IsHovered && isDown && !_isPressed)
        {
            _isPressed = true;
        }
        else if (!isDown && _isPressed)
        {
            if (IsHovered)
            {
                wasPressed = true;
            }
            _isPressed = false;
        }

        WasPressed = wasPressed;
        return wasPressed;
    }
} 