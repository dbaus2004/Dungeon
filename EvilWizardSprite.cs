using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Dungeon.Collisions;

namespace Dungeon;

public class EvilWizardSprite
{
    private Vector2 position;
    private Texture2D texture;
    private BoundingRectangle bounds;

    public bool Dead { get; private set; } = false;

    public Vector2 Position => position;

    public BoundingRectangle Bounds => bounds;

    public EvilWizardSprite(Vector2 position)
    {
        this.position = position;

        this.bounds = new BoundingRectangle(
            position.X + 16,
            position.Y,
            32,
            64);
    }

    public void LoadContent(ContentManager content)
    {
        texture = content.Load<Texture2D>("EvilWizard");
    }

    public void Hit()
    {
        Dead = true;
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        if (Dead)
            return;

        spriteBatch.Draw(
            texture,
            position,
            null,
            Color.White,
            0f,
            Vector2.Zero,
            2.0f,
            SpriteEffects.None,
            0f);
    }
}