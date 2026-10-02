using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Dungeon.Input;
using Dungeon.Objects;

namespace Dungeon;

public class EntityDescription : GameScreen
{
    private readonly Entity _entity;
    private readonly IInputState _inputState;
    private readonly ScreenManager _screenManager;
    private readonly InputAction _returnAction =
        new InputAction(null, new[] { Keys.Enter }, true);

    private SpriteFont _font;

    public EntityDescription(
        Entity entity,
        IInputState inputState,
        ScreenManager screenManager)
    {
        _entity = entity;
        _inputState = inputState;
        _screenManager = screenManager;
    }

    public override void LoadContent(ContentManager content)
    {
        _font = content.Load<SpriteFont>("arial");
    }

    public override void Update(GameTime gameTime)
    {
        if (_returnAction.Occured(_inputState, null, out _))
            _screenManager.PopScreen();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        const float scale = 0.65f;
        const float lineHeight = 25f;
        const float leftX = 80f;
        const float rightX = 660f;
        const float topY = 210f;

        DrawScaledString(spriteBatch, _entity.Name, new Vector2(leftX, 40), Color.White, scale);
        DrawScaledString(spriteBatch, _entity.Discription, new Vector2(leftX, 75), Color.LightGray, scale);
        DrawScaledString(spriteBatch, $"HP: {_entity.HP}", new Vector2(leftX, 110), Color.White, scale);
        DrawScaledString(spriteBatch, "Resistances", new Vector2(leftX, 155), Color.Gold, scale);

        string[] leftColumn =
        {
        $"Normal: {_entity.Normal}",
        $"Magical: {_entity.Magical}",
        $"True: {_entity.True}",
        $"Physical: {_entity.PhysicalPercent}%",
        $"Magical: {_entity.PhysicalPercent}%"
    };

        string[] rightColumn =
        {
        $"Bludgeoning: {_entity.Bludgeoning} ({_entity.BludgeoningPercent}%)",
        $"Piercing: {_entity.Piercing} ({_entity.PiercingPercent}%)",
        $"Slashing: {_entity.Slashing} ({_entity.SlashingPercent}%)",
        $"Heat: {_entity.Heat} ({_entity.HeatPercent}%)",
        $"Cold: {_entity.Cold} ({_entity.ColdPercent}%)",
        $"Energy: {_entity.Energy} ({_entity.EnergyPercent}%)",
        $"Necrotic: {_entity.Necrotic} ({_entity.NecroticPercent}%)",
        $"Psychic: {_entity.Psychic} ({_entity.PsychicPercent}%)"
    };

        for (int i = 0; i < leftColumn.Length; i++)
            DrawScaledString(
                spriteBatch,
                leftColumn[i],
                new Vector2(leftX, topY + i * lineHeight),
                Color.White,
                scale);

        for (int i = 0; i < rightColumn.Length; i++)
            DrawScaledString(
                spriteBatch,
                rightColumn[i],
                new Vector2(rightX - 200, topY + i * lineHeight),
                Color.White,
                scale);

        DrawScaledString(
            spriteBatch,
            "Press Enter to return",
            new Vector2(leftX, 350),
            Color.Yellow,
            scale);
    }

    private void DrawScaledString(
        SpriteBatch spriteBatch,
        string text,
        Vector2 position,
        Color color,
        float scale)
    {
        spriteBatch.DrawString(
            _font,
            text,
            position,
            color,
            0f,
            Vector2.Zero,
            scale,
            SpriteEffects.None,
            0f);
    }
}