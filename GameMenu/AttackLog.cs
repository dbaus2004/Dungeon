using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dungeon.Menu;

public sealed class AttackLog : IDisposable
{
    private const float Margin = 20f;
    private const float Padding = 12f;
    private const float Scale = 0.7f;

    private readonly SpriteBatch _spriteBatch;
    private readonly SpriteFont _font;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly Texture2D _pixel;

    public AttackLog(SpriteBatch spriteBatch, SpriteFont font, GraphicsDevice graphicsDevice)
    {
        _spriteBatch = spriteBatch;
        _font = font;
        _graphicsDevice = graphicsDevice;

        _pixel = new Texture2D(graphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    public void Draw(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        Viewport viewport = _graphicsDevice.Viewport;
        float maxTextWidth = Math.Min(500f, viewport.Width * 0.4f) - Padding * 2f;
        List<string> lines = WrapText(text, maxTextWidth);

        float lineHeight = _font.LineSpacing * Scale;
        int maxLines = Math.Max(1, (int)((viewport.Height - Margin * 2f - Padding * 2f) / lineHeight));

        if (lines.Count > maxLines)
            lines = lines.GetRange(lines.Count - maxLines, maxLines);

        float panelWidth = Math.Min(500f, viewport.Width * 0.4f);
        float panelHeight = lines.Count * lineHeight + Padding * 2f;
        float panelX = viewport.Width - panelWidth - Margin;
        float panelY = viewport.Height - panelHeight - Margin;

        _spriteBatch.Draw(
            _pixel,
            new Rectangle(
                (int)panelX,
                (int)panelY,
                (int)panelWidth,
                (int)panelHeight),
            Color.Black * 0.75f);

        float y = panelY + Padding;

        foreach (string line in lines)
        {
            float textWidth = _font.MeasureString(line).X * Scale;
            Vector2 position = new Vector2(
                viewport.Width - Margin - Padding - textWidth,
                y);

            _spriteBatch.DrawString(
                _font,
                line,
                position + new Vector2(1f, 1f),
                Color.Black,
                0f,
                Vector2.Zero,
                Scale,
                SpriteEffects.None,
                0f);

            _spriteBatch.DrawString(
                _font,
                line,
                position,
                Color.White,
                0f,
                Vector2.Zero,
                Scale,
                SpriteEffects.None,
                0f);

            y += lineHeight;
        }
    }

    private List<string> WrapText(string text, float maxWidth)
    {
        var lines = new List<string>();

        foreach (string paragraph in text.Replace("\r", string.Empty).Split('\n'))
        {
            string line = string.Empty;

            foreach (string word in paragraph.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                string candidate = line.Length == 0 ? word : $"{line} {word}";

                if (line.Length > 0 && _font.MeasureString(candidate).X * Scale > maxWidth)
                {
                    lines.Add(line);
                    line = word;
                }
                else
                {
                    line = candidate;
                }
            }

            if (line.Length > 0)
                lines.Add(line);
        }

        return lines;
    }

    public void Dispose()
    {
        _pixel.Dispose();
    }
}