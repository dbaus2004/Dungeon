using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Dungeon
{
    class Tilemap
    {
        private readonly string _filename;
        private readonly string _tilesetAssetName;
        private Texture2D _tilesetTexture;
        private Rectangle[] _sourceRectangles = Array.Empty<Rectangle>();

        public int Width { get; private set; }
        public int Height { get; private set; }
        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }
        public bool IsInfinite { get; private set; }
        public string Orientation { get; private set; } = string.Empty;
        public string RenderOrder { get; private set; } = string.Empty;
        public string Version { get; private set; } = string.Empty;
        public string TiledVersion { get; private set; } = string.Empty;

        public List<TilemapLayer> Layers { get; private set; } = new();
        public List<TilemapTileset> Tilesets { get; private set; } = new();

        public Tilemap(string mapFilename, string tilesetAssetName)
        {
            _filename = mapFilename;
            _tilesetAssetName = tilesetAssetName;
        }

        private float _displayScale = 1f;
        private Vector2 _displayOffset;

        public Vector2 DisplayTileSize =>
            new Vector2(TileWidth * _displayScale, TileHeight * _displayScale);

        public void FitToViewport(int viewportWidth, int viewportHeight)
        {
            if (viewportWidth <= 0 || viewportHeight <= 0 || Width <= 0 || Height <= 0)
                throw new ArgumentOutOfRangeException(nameof(viewportWidth));

            _displayScale = MathF.Min(
                (float)viewportWidth / (Width * TileWidth),
                (float)viewportHeight / (Height * TileHeight)
            );

            _displayOffset = new Vector2(
                (viewportWidth - Width * TileWidth * _displayScale) / 2f,
                (viewportHeight - Height * TileHeight * _displayScale) / 2f
            );
        }

        public List<Vector2> GetTilePositions(string layerName, int gid)
        {
            TilemapLayer layer = GetLayer(layerName);
            var positions = new List<Vector2>();

            for (int y = 0; y < layer.Height; y++)
            {
                for (int x = 0; x < layer.Width; x++)
                {
                    if (layer.Data[y * layer.Width + x] != gid)
                        continue;

                    positions.Add(
                        _displayOffset
                            + new Vector2(
                                (layer.X + x) * TileWidth * _displayScale,
                                (layer.Y + y) * TileHeight * _displayScale
                            )
                    );
                }
            }

            return positions;
        }

        public List<int> GetTileGids(string layerName)
        {
            TilemapLayer layer = GetLayer(layerName);
            var gids = new List<int>();
            var seenGids = new HashSet<int>();

            foreach (int gid in layer.Data)
            {
                if (gid != 0 && seenGids.Add(gid))
                    gids.Add(gid);
            }

            return gids;
        }

        private TilemapLayer GetLayer(string layerName)
        {
            foreach (TilemapLayer layer in Layers)
            {
                if (layer.Name == layerName)
                    return layer;
            }

            throw new InvalidDataException($"Map layer '{layerName}' was not found.");
        }

        public void LoadContent(ContentManager content)
        {
            string mapPath = Path.Combine(content.RootDirectory, _filename);
            using JsonDocument document = JsonDocument.Parse(File.ReadAllText(mapPath));

            JsonElement root = document.RootElement;

            Width = root.GetProperty("width").GetInt32();
            Height = root.GetProperty("height").GetInt32();
            TileWidth = root.GetProperty("tilewidth").GetInt32();
            TileHeight = root.GetProperty("tileheight").GetInt32();
            IsInfinite = root.GetProperty("infinite").GetBoolean();
            Orientation = RequiredString(root, "orientation");
            RenderOrder = RequiredString(root, "renderorder");
            Version = RequiredString(root, "version");
            TiledVersion = RequiredString(root, "tiledversion");

            if (TileWidth <= 0 || TileHeight <= 0)
            {
                throw new InvalidDataException(
                    "The map tile width and height must be greater than zero."
                );
            }

            Layers = new List<TilemapLayer>();
            foreach (JsonElement layer in root.GetProperty("layers").EnumerateArray())
            {
                int layerWidth = layer.GetProperty("width").GetInt32();
                int layerHeight = layer.GetProperty("height").GetInt32();
                int[] data = ReadTileData(layer.GetProperty("data"));
                string layerName = RequiredString(layer, "name");

                if (data.Length != layerWidth * layerHeight)
                {
                    throw new InvalidDataException(
                        $"Layer '{layerName}' has an unexpected number of tiles."
                    );
                }

                Layers.Add(
                    new TilemapLayer
                    {
                        Id = layer.GetProperty("id").GetInt32(),
                        Name = layerName,
                        Type = RequiredString(layer, "type"),
                        Width = layerWidth,
                        Height = layerHeight,
                        X = layer.GetProperty("x").GetInt32(),
                        Y = layer.GetProperty("y").GetInt32(),
                        Opacity = layer.GetProperty("opacity").GetDouble(),
                        Visible = layer.GetProperty("visible").GetBoolean(),
                        Data = data,
                    }
                );
            }

            Tilesets = new List<TilemapTileset>();
            foreach (JsonElement tileset in root.GetProperty("tilesets").EnumerateArray())
            {
                Tilesets.Add(
                    new TilemapTileset
                    {
                        FirstGid = tileset.GetProperty("firstgid").GetInt32(),
                        Source = RequiredString(tileset, "source"),
                    }
                );
            }

            string assetName = Path.ChangeExtension(_tilesetAssetName, null);
            _tilesetTexture = content.Load<Texture2D>(assetName);

            int columns = _tilesetTexture.Width / TileWidth;
            int rows = _tilesetTexture.Height / TileHeight;

            if (columns == 0 || rows == 0)
            {
                throw new InvalidDataException(
                    "The tileset texture is smaller than the map's tile dimensions."
                );
            }

            _sourceRectangles = new Rectangle[columns * rows];

            for (int i = 0; i < _sourceRectangles.Length; i++)
            {
                int x = i % columns;
                int y = i / columns;

                _sourceRectangles[i] = new Rectangle(
                    x * TileWidth,
                    y * TileHeight,
                    TileWidth,
                    TileHeight
                );
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_tilesetTexture == null || _sourceRectangles.Length == 0)
            {
                throw new InvalidOperationException("Call LoadContent before drawing the tilemap.");
            }

            if (Tilesets.Count == 0)
            {
                throw new InvalidOperationException("The tilemap has no tileset information.");
            }

            int firstGid = Tilesets[0].FirstGid;
            int nextTilesetGid = int.MaxValue;

            foreach (TilemapTileset tileset in Tilesets)
            {
                if (tileset.FirstGid > firstGid && tileset.FirstGid < nextTilesetGid)
                {
                    nextTilesetGid = tileset.FirstGid;
                }
            }

            foreach (TilemapLayer layer in Layers)
            {
                if (!layer.Visible || layer.Type != "tilelayer")
                    continue;

                for (int y = 0; y < layer.Height; y++)
                {
                    for (int x = 0; x < layer.Width; x++)
                    {
                        int gid = layer.Data[y * layer.Width + x];

                        // Skip empty tiles and tiles belonging to another tileset.
                        if (gid == 0 || gid < firstGid || gid >= nextTilesetGid)
                            continue;

                        int tileIndex = gid - firstGid;
                        if (tileIndex >= _sourceRectangles.Length)
                            continue;

                        spriteBatch.Draw(
                            _tilesetTexture,
                            _displayOffset
                                + new Vector2(
                                    (layer.X + x) * TileWidth * _displayScale,
                                    (layer.Y + y) * TileHeight * _displayScale
                                ),
                            _sourceRectangles[tileIndex],
                            Color.White * (float)layer.Opacity,
                            0f,
                            Vector2.Zero,
                            _displayScale,
                            SpriteEffects.None,
                            0f
                        );
                    }
                }
            }
        }

        private static int[] ReadTileData(JsonElement data)
        {
            var tiles = new List<int>();

            foreach (JsonElement tile in data.EnumerateArray())
            {
                tiles.Add(tile.GetInt32());
            }

            return tiles.ToArray();
        }

        private static string RequiredString(JsonElement element, string propertyName)
        {
            return element.GetProperty(propertyName).GetString()
                ?? throw new InvalidDataException(
                    $"TMJ property '{propertyName}' must be a string."
                );
        }
    }

    class TilemapLayer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Width { get; set; }
        public int Height { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public double Opacity { get; set; }
        public bool Visible { get; set; }
        public int[] Data { get; set; } = Array.Empty<int>();
    }

    class TilemapTileset
    {
        public int FirstGid { get; set; }
        public string Source { get; set; } = string.Empty;
    }
}
