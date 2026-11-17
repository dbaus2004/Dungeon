using Dungeon.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Dungeon.Sprites;

public class EvilWizardSprite : ISprite, IScalableSprite
{
    private Vector2 position;
    private Texture2D texture;
    public Vector2 Position
    {
        get { return position; }
        set { position = value; }
    }
    public Vector2 DisplaySize { get; set; } = new Vector2(16, 16);

    public Vector2 Center => Position + DisplaySize / 2f;

    public Bounds HitBox =>
        new BoundingRectangle(position.X, position.Y, DisplaySize.X, DisplaySize.Y);

    public EvilWizardSprite(Vector2 position)
    {
        this.position = position;
    }

    public void LoadContent(ContentManager content)
    {
        texture = content.Load<Texture2D>("Textures/EvilWizard");
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        Vector2 scale = new Vector2(DisplaySize.X / texture.Width, DisplaySize.Y / texture.Height);

        spriteBatch.Draw(
            texture,
            position,
            null,
            Color.White,
            0f,
            Vector2.Zero,
            scale,
            SpriteEffects.None,
            0f
        );
    }
}
