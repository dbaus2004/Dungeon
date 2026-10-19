
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Dungeon.Objects;

namespace Dungeon.Menu;

public class EntityDescription : Menu
{
    private readonly Entity _entity;

    public EntityDescription(
        Entity entity,
        SpriteBatch spriteBatch,
        SpriteFont font,
        GraphicsDevice graphicsDevice)
        : base(spriteBatch, font, graphicsDevice)
    {
        _entity = entity;
    }
    protected override void HandleInput(
        GameTime gameTime,
        KeyboardState keyboard)
    {
        // Input handling can be added later if needed.
    }

    protected override void DrawContent(GameTime gameTime)
    {
        Viewport viewport = GraphicsDevice.Viewport;
        float scale = viewport.Height < 800 ? 0.75f : 0.9f;
        float columnWidth = Math.Min(480f, viewport.Width * 0.42f);
        float x = viewport.Width - columnWidth - 32f;
        float y = 40f;

        y = DrawWrappedText(_entity.Name, x, y, columnWidth, scale, Color.Gold);
        y += 8f;

        y = DrawWrappedText(_entity.Discription, x, y, columnWidth, scale, Color.LightGray);
        y += 12f;

        y = DrawWrappedText($"HP: {_entity.HP}", x, y, columnWidth, scale, Color.White);
        y += 16f;

        y = DrawWrappedText("Resistances", x, y, columnWidth, scale, Color.Gold);
        y += 4f;

        string[] resistances =
        {
        $"Physical: {_entity.PhysicalPercent}%",
        $"Physical: {_entity.MagicalPercent}%",
        $"Bludgeoning: {_entity.Bludgeoning} ({_entity.BludgeoningPercent}%)",
        $"Piercing: {_entity.Piercing} ({_entity.PiercingPercent}%)",
        $"Slashing: {_entity.Slashing} ({_entity.SlashingPercent}%)",
        $"Heat: {_entity.Heat} ({_entity.HeatPercent}%)",
        $"Cold: {_entity.Cold} ({_entity.ColdPercent}%)",
        $"Energy: {_entity.Energy} ({_entity.EnergyPercent}%)",
        $"Necrotic: {_entity.Necrotic} ({_entity.NecroticPercent}%)",
        $"Psychic: {_entity.Psychic} ({_entity.PsychicPercent}%)"
    };

        foreach (string resistance in resistances)
            y = DrawWrappedText(resistance, x, y, columnWidth, scale, Color.White);
    }

    private float DrawWrappedText(
        string text,
        float x,
        float y,
        float maxWidth,
        float scale,
        Color color)
    {
        string line = string.Empty;
        float lineHeight = Font.LineSpacing * scale + 4f;

        foreach (string word in text.Split(' '))
        {
            string candidate = line.Length == 0 ? word : $"{line} {word}";

            if (line.Length > 0 && Font.MeasureString(candidate).X * scale > maxWidth)
            {
                DrawReadableLine(line, x, y, scale, color);
                y += lineHeight;
                line = word;
            }
            else
            {
                line = candidate;
            }
        }

        if (line.Length > 0)
        {
            DrawReadableLine(line, x, y, scale, color);
            y += lineHeight;
        }

        return y;
    }

    private void DrawReadableLine(string text, float x, float y, float scale, Color color)
    {
        Vector2 position = new Vector2(x, y);

        SpriteBatch.DrawString(
            Font, text, position + new Vector2(2f, 2f), Color.Black,
            0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

        SpriteBatch.DrawString(
            Font, text, position, color,
            0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
    }

    private void DrawScaledString(
        string text,
        Vector2 position,
        Color color,
        float scale)
    {
        SpriteBatch.DrawString(
            Font,
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
