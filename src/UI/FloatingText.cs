using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace incremental.UI;

public class FloatingText
{
    public Vector2 Position { get; private set; }
    public string Text { get; }
    public Color Color { get; }
    public float TimeLeft { get; private set; }
    public float Scale { get; private set; }
    public float Alpha { get; private set; }

    private readonly Vector2 _velocity;
    private const float LIFETIME = 1.5f;
    private const float FADE_START = 0.5f;

    public FloatingText(string text, Vector2 position, Color color)
    {
        Text = text;
        Position = position;
        Color = color;
        TimeLeft = LIFETIME;
        Scale = 1.0f;
        Alpha = 1.0f;
        _velocity = new Vector2(0, -50f); // Move up
    }

    public bool Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        TimeLeft -= deltaTime;

        if (TimeLeft <= 0)
            return false;

        Position += _velocity * deltaTime;
        
        if (TimeLeft < FADE_START)
        {
            Alpha = TimeLeft / FADE_START;
            Scale = 0.5f + (TimeLeft / FADE_START) * 0.5f;
        }

        return true;
    }

    public void Draw(SpriteBatch spriteBatch, SpriteFont font)
    {
        Color colorWithAlpha = Color * Alpha;
        Vector2 origin = font.MeasureString(Text) * 0.5f;
        spriteBatch.DrawString(font, Text, Position, colorWithAlpha, 0f, origin, Scale, SpriteEffects.None, 0f);
    }
} 