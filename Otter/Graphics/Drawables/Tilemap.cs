using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using Otter.Colliders;
using Otter.Utility;

namespace Otter.Graphics.Drawables;

/// <summary>
/// Graphic used for loading and rendering a tilemap.  Renders tiles using a vertex array.
/// </summary>
public class Tilemap : Graphic
{
    #region Public Properties

    /// <summary>
    /// The width in pixels of each tile.
    /// </summary>
    public int TileWidth { get; private set; }

    /// <summary>
    /// The height in pixels of each tile.
    /// </summary>
    public int TileHeight { get; private set; }

    /// <summary>
    /// The number of rows in the entire tilemap.
    /// </summary>
    public int TileRows { get; private set; }

    /// <summary>
    /// The number of columsn in the entire tilemap.
    /// </summary>
    public int TileColumns { get; private set; }

    /// <summary>
    /// The tile layers to render.
    /// </summary>
    public SortedDictionary<int, List<TileInfo>> TileLayers { get; private set; }

    #endregion

    #region Public Fields

    /// <summary>
    /// Determines if the X and Y positions of tiles are interpreted as pixels or tile coords.
    /// </summary>
    public bool UsePositions;

    /// <summary>
    /// The default layer name to use.
    /// </summary>
    public string DefaultLayerName = "base";

    #endregion

    #region Private Fields

    private readonly Dictionary<string, int> layerNames = [];
    private readonly Dictionary<int, Dictionary<int, Dictionary<int, TileInfo>>> tileTable = [];
    private Dictionary<int, List<int>> autoTileTable;

    #region Auto Tile Data

    private readonly string autoTileDefaultData = @"0:
    ? 1 ?
    0 x 0
    ? 0 ?
=
1:
    ? 0 ?
    0 x 1
    ? 0 ?
=
2:
    ? 0 ?
    0 x 0
    ? 1 ?
=
3:
    ? 0 ?
    1 x 0
    ? 0 ?
=
4:
    ? 1 0
    0 x 1
    ? 0 ?
=
5:
    ? 0 ?
    0 x 1
    ? 1 0
=
6:
    ? 0 ?
    1 x 0
    0 1 ?
=
7:
    0 1 ?
    1 x 0
    ? 0 ?
=
8:
    ? 1 1
    0 x 1
    ? 0 ?
=
9:
    ? 0 ?
    0 x 1
    ? 1 1
=
10:
    ? 0 ?
    1 x 0
    1 1 ?
=
11:
    1 1 ?
    1 x 0
    ? 0 ?
=
12:
    ? 1 0
    0 x 1
    ? 1 0
=
13:
    ? 0 ?
    1 x 1
    0 1 0
=
14:
    0 1 ?
    1 x 0
    0 1 ?
=
15:
    0 1 0
    1 x 1
    ? 0 ?
=
16:
    ? 1 1
    0 x 1
    ? 1 0
=
17:
    ? 1 0
    0 x 1
    ? 1 1
=
18:
    ? 1 1
    0 x 1
    ? 1 1
=
19:
    ? 0 ?
    1 x 1
    0 1 1
=
20:
    ? 0 ?
    1 x 1
    1 1 0
=
21:
    ? 0 ?
    1 x 1
    1 1 1
=
22:
    0 1 ?
    1 x 0
    1 1 ?
=
23:
    1 1 ?
    1 x 0
    0 1 ?
=
24:
    1 1 ?
    1 x 0
    1 1 ?
=
25:
    0 1 1
    1 x 1
    ? 0 ?
=
26:
    1 1 0
    1 x 1
    ? 0 ?
=
27:
    1 1 1
    1 x 1
    ? 0 ?
=
28:
    0 1 0
    1 x 1
    0 1 0
=
29:
    0 1 1
    1 x 1
    0 1 0
=
30:
    0 1 0
    1 x 1
    0 1 1
=
31:
    0 1 0
    1 x 1
    1 1 0
=
32:
    1 1 0
    1 x 1
    0 1 0
=
33:
    0 1 1
    1 x 1
    0 1 1
=
34:
    0 1 0
    1 x 1
    1 1 1
=
35:
    1 1 0
    1 x 1
    1 1 0
=
36:
    1 1 1
    1 x 1
    0 1 0
=
37:
    0 1 1
    1 x 1
    1 1 0
=
38:
    1 1 0
    1 x 1
    0 1 1
=
39:
    0 1 1
    1 x 1
    1 1 1
=
40:
    1 1 1
    1 x 1
    0 1 1
=
41:
    1 1 1
    1 x 1
    1 1 0
=
42:
    1 1 0
    1 x 1
    1 1 1
=
43:
    1 1 1
    1 x 1
    1 1 1
=
44:
    ? 0 ?
    0 x 0
    ? 0 ?
=
45:
    ? 1 ?
    0 x 0
    ? 1 ?
=
46:
    ? 0 ?
    1 x 1
    ? 0 ?";

    #endregion

    private int sourceColumns;

    #endregion

    #region Constructors

    /// <summary>
    /// Create a new Tilemap using the path of a texture.
    /// </summary>
    /// <param name="source">The file path to the texture to use.</param>
    /// <param name="width">The width of the Tilemap in pixels.</param>
    /// <param name="height">The height of the Tilemap in pixels.</param>
    /// <param name="tileWidth">The width of each tile in pixels.</param>
    /// <param name="tileHeight">The height of each tile in pixels.</param>
    public Tilemap(string source, int width, int height, int tileWidth, int tileHeight) : base()
    {
        source = FileHandling.GetAbsoluteFilePath(source);
        SetTexture(new Texture(source));
        Initialize(width, height, tileWidth, tileHeight);
    }

    /// <summary>
    /// Create a new Tilemap using the path of a texture.
    /// </summary>
    /// <param name="source">The file path to the texture to use.</param>
    /// <param name="size">The width and height of the Tilemap in pixels.</param>
    /// <param name="tileSize">The width and height of each tile in pixels.</param>
    public Tilemap(string source, int size, int tileSize) : this(source, size, size, tileSize, tileSize) { }

    /// <summary>
    /// Create a new Tilemap using a Texture.
    /// </summary>
    /// <param name="texture">The Texture to use.</param>
    /// <param name="width">The width of the Tilemap in pixels.</param>
    /// <param name="height">The height of the Tilemap in pixels.</param>
    /// <param name="tileWidth">The width of each tile in pixels.</param>
    /// <param name="tileHeight">The height of each tile in pixels.</param>
    public Tilemap(Texture texture, int width, int height, int tileWidth, int tileHeight) : base()
    {
        SetTexture(texture);
        Initialize(width, height, tileWidth, tileHeight);
    }

    /// <summary>
    /// Create a new Tilemap using a Texture.
    /// </summary>
    /// <param name="texture">The Texture to use.</param>
    /// <param name="size">The width and height of the Tilemap in pixels.</param>
    /// <param name="tileSize">The width and height of each tile in pixels.</param>
    public Tilemap(Texture texture, int size, int tileSize) : this(texture, size, size, tileSize, tileSize) { }

    /// <summary>
    /// Create a new Tilemap using an AtlasTexture.
    /// </summary>
    /// <param name="texture">The AtlasTexture to use.</param>
    /// <param name="width">The width of the Tilemap in pixels.</param>
    /// <param name="height">The height of the Tilemap in pixels.</param>
    /// <param name="tileWidth">The width of each tile in pixels.</param>
    /// <param name="tileHeight">The height of each tile in pixels.</param>
    public Tilemap(AtlasTexture texture, int width, int height, int tileWidth, int tileHeight)
    {
        SetTexture(texture);
        Initialize(width, height, tileWidth, tileHeight);
    }

    /// <summary>
    /// Create a new Tilemap using an AtlasTexture.
    /// </summary>
    /// <param name="texture">The AtlasTexture to use.</param>
    /// <param name="size">The width and height of the Tilemap in pixels.</param>
    /// <param name="tileSize">The width and height of each tile in pixels.</param>
    public Tilemap(AtlasTexture texture, int size, int tileSize) : this(texture, size, size, tileSize, tileSize) { }

    /// <summary>
    /// Create a new Tilemap without any texture.  Tiles will be solid colors instead.
    /// </summary>
    /// <param name="width">The width of the Tilemap in pixels.</param>
    /// <param name="height">The height of the Tilemap in pixels.</param>
    /// <param name="tileWidth">The width of each tile in pixels.</param>
    /// <param name="tileHeight">The height of each tile in pixels.</param>
    public Tilemap(int width, int height, int tileWidth, int tileHeight)
    {
        Initialize(width, height, tileWidth, tileHeight);
    }

    /// <summary>
    /// Create a new Tilemap without any texture.  Tiles will be solid colors instead.
    /// </summary>
    /// <param name="size">The width and height of the Tilemap in pixels.</param>
    /// <param name="tileSize">The width and height of each tile in pixels.</param>
    public Tilemap(int size, int tileSize) : this(size, size, tileSize, tileSize) { }

    #endregion

    #region Private Methods

    private void Initialize(int width, int height, int tileWidth, int tileHeight)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(width);
        ArgumentOutOfRangeException.ThrowIfNegative(height);

        TileLayers = [];

        AddLayer(DefaultLayerName);

        TileWidth = tileWidth;
        TileHeight = tileHeight;

        TileColumns = (int)Util.Ceil((float)width / tileWidth);
        TileRows = (int)Util.Ceil((float)height / tileHeight);

        sourceColumns = (int)(TextureRegion.Width / tileWidth);

        Width = width;
        Height = height;
    }

    protected override void UpdateDrawable()
    {
        base.UpdateDrawable();

        SFMLVertices.Clear();

        foreach (var layer in TileLayers.Reverse())
        {
            //for (int i = TileLayers.Count - 1; i >= 0; i--) {
            //    var layer = TileLayers.Values[i];
            foreach (var tile in layer.Value)
            {
                //tile.Alpha = Alpha;
                tile.tilemapColor.R = Color.R;
                tile.tilemapColor.G = Color.G;
                tile.tilemapColor.B = Color.B;
                tile.tilemapColor.A = Color.A;
                tile.AppendVertices(SFMLVertices);
            }
        }
    }

    private void RegisterTile(int x, int y, int layer, TileInfo tile)
    {
        if (!tileTable.TryGetValue(layer, out Dictionary<int, Dictionary<int, TileInfo>> value1))
        {
            value1 = ([]);
            tileTable.Add(layer, value1);
        }
        if (!value1.TryGetValue(x, out Dictionary<int, TileInfo> value))
        {
            value = ([]);
            value1.Add(x, value);
        }

        value.Add(y, tile);
    }

    private void RemoveTile(int x, int y, int layer)
    {
        if (tileTable.ContainsKey(layer))
        {
            if (tileTable[layer].TryGetValue(x, out Dictionary<int, TileInfo> value))
            {
                value.Remove(y);
            }
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Set a tile to a specific color.
    /// </summary>
    /// <param name="tileX">The tile's x position on the map.</param>
    /// <param name="tileY">The tile's y position on the map.</param>
    /// <param name="color">The tile's color.</param>
    /// <param name="layer">The tile's layer.</param>
    /// <returns>The TileInfo of the altered tile.</returns>
    public TileInfo SetTile(int tileX, int tileY, Color color, string layer = "")
    {
        if (layer == "")
        {
            layer = DefaultLayerName;
        }

        // Clear the tile there first so tiles do not stack.
        ClearTile(tileX, tileY, layer);

        // Update arguments after calling other tile methods.
        if (!UsePositions)
        {
            tileX *= TileWidth;
            tileY *= TileHeight;
        }

        // Clamp tile inside tilemap.
        tileX = (int)Util.Clamp(tileX, 0, Width - TileWidth);
        tileY = (int)Util.Clamp(tileY, 0, Height - TileHeight);

        var t = new TileInfo(tileX, tileY, -1, -1, TileWidth, TileHeight, color);
        TileLayers[layerNames[layer]].Add(t);

        // Register tile for look ups.
        RegisterTile(tileX, tileY, layerNames[layer], t);

        NeedsUpdate = true;

        return t;
    }

    /// <summary>
    /// Set a tile to a specific color.
    /// </summary>
    /// <param name="tileX">The tile's x position on the map.</param>
    /// <param name="tileY">The tile's y position on the map.</param>
    /// <param name="color">The tile's color.</param>
    /// <param name="layer">The tile's layer.</param>
    /// <returns>The TileInfo of the altered tile.</returns>
    public TileInfo SetTile(int tileX, int tileY, Color color, Enum layer)
    {
        return SetTile(tileX, tileY, color, Util.EnumValueToString(layer));
    }

    /// <summary>
    /// Set a tile to a specific tile from the source texture.
    /// </summary>
    /// <param name="tileX">The tile's X position on the map.</param>
    /// <param name="tileY">The tile's Y position on the map.</param>
    /// <param name="sourceX">The source X position from the tile map in pixels.</param>
    /// <param name="sourceY">The source Y position from the tile map in pixels.</param>
    /// <param name="layer">The tile's layer.</param>
    /// <returns>The TileInfo for the altered tile.</returns>
    public TileInfo SetTile(int tileX, int tileY, int sourceX, int sourceY, string layer = "")
    {
        sourceX += TextureLeft;
        sourceY += TextureTop;

        if (layer == "")
        {
            layer = DefaultLayerName;
        }

        // Clear the tile there first so tiles do not stack.
        ClearTile(tileX, tileY, layer);

        // Update arguments after calling other tile methods.
        if (!UsePositions)
        {
            tileX *= TileWidth;
            tileY *= TileHeight;
        }

        tileX = (int)Util.Clamp(tileX, 0, Width - TileWidth);
        tileY = (int)Util.Clamp(tileY, 0, Height - TileHeight);

        var t = new TileInfo(tileX, tileY, sourceX, sourceY, TileWidth, TileHeight);
        TileLayers[layerNames[layer]].Add(t);

        RegisterTile(tileX, tileY, layerNames[layer], t);

        NeedsUpdate = true;

        return t;
    }

    /// <summary>
    /// Set a tile to a specific tile from the source texture.
    /// </summary>
    /// <param name="tileX">The tile's X position on the map.</param>
    /// <param name="tileY">The tile's Y position on the map.</param>
    /// <param name="sourceX">The source X position from the tile map in pixels.</param>
    /// <param name="sourceY">The source Y position from the tile map in pixels.</param>
    /// <param name="layer">The tile's layer.</param>
    /// <returns>The TileInfo for the altered tile.</returns>
    public TileInfo SetTile(int tileX, int tileY, int sourceX, int sourceY, Enum layer)
    {
        return SetTile(tileX, tileY, sourceX, sourceY, Util.EnumValueToString(layer));
    }

    /// <summary>
    /// Load tile data from an XmlElement.
    /// </summary>
    /// <param name="e">An XmlElement containing attributes x, y, tx, and ty.</param>
    /// <returns>The TileInfo for the loaded tile.</returns>
    public TileInfo SetTile(XmlElement e)
    {
        int x, y, tx, ty;
        if (UsePositions)
        {
            x = e.AttributeInt("x") * TileWidth;
            y = e.AttributeInt("y") * TileHeight;
        }
        else
        {
            x = e.AttributeInt("x");
            y = e.AttributeInt("y");
        }
        tx = e.AttributeInt("tx") * TileWidth;
        ty = e.AttributeInt("ty") * TileHeight;
        return SetTile(x, y, tx, ty);
    }

    /// <summary>
    /// Set a tile on the Tilemap to a specific tile.
    /// </summary>
    /// <param name="tileX">The X position of the tile to change.</param>
    /// <param name="tileY">The Y position of the tile to change.</param>
    /// <param name="tileIndex">The index of the tile to change to.</param>
    /// <param name="layer"></param>
    /// <returns>The TileInfo from the altered tile.</returns>
    public TileInfo SetTile(int tileX, int tileY, int tileIndex, string layer = "")
    {
        var sourceX = (int)(Util.TwoDeeX((int)tileIndex, (int)sourceColumns) * TileWidth);
        var sourceY = (int)(Util.TwoDeeY((int)tileIndex, (int)sourceColumns) * TileHeight);
        return SetTile(tileX, tileY, sourceX, sourceY, layer);
    }

    /// <summary>
    /// Set a tile on the Tilemap to a specific tile.
    /// </summary>
    /// <param name="tileX">The X position of the tile to change.</param>
    /// <param name="tileY">The Y position of the tile to change.</param>
    /// <param name="tileIndex">The index of the tile to change to.</param>
    /// <returns>The TileInfo from the altered tile.</returns>
    public TileInfo SetTile(int tileX, int tileY, int tileIndex, Enum layer)
    {
        return SetTile(tileX, tileY, tileIndex, Util.EnumValueToString(layer));
    }

    /// <summary>
    /// Set a tile on the Tilemap to be flipped horizontally and/or vertically.
    /// </summary>
    /// <param name="tileX">The X position of the tile to change.</param>
    /// <param name="tileY">The Y position of the tile to change.</param>
    /// <param name="flipX">Whether the tile should be horizontally flipped.</param>
    /// <param name="flipY">Whether the tile should be vertically flipped.</param>
    /// <returns>The TileInfo from the altered tile.</returns>
    public TileInfo SetTile(int tileX, int tileY, bool flipX, bool flipY)
    {
        GetTile(tileX, tileY).FlipX = flipX;
        GetTile(tileX, tileY).FlipY = flipY;
        return GetTile(tileX, tileY);
    }

    /// <summary>
    /// Set a rectangle area of tiles to a defined color.
    /// </summary>
    /// <param name="tileX">The X position of the tile to change.</param>
    /// <param name="tileY">The Y position of the tile to change.</param>
    /// <param name="tileWidth">The width of tiles to change.</param>
    /// <param name="tileHeight">The height of tiles to change.</param>
    /// <param name="color">The color to change the colors to.</param>
    /// <param name="layer">The layer to place the tiles on.</param>
    public void SetRect(int tileX, int tileY, int tileWidth, int tileHeight, Color color, string layer = "")
    {
        for (var xx = tileX; xx < tileX + tileWidth; xx++)
        {
            for (var yy = tileY; yy < tileY + tileHeight; yy++)
            {
                SetTile(xx, yy, color, layer);
            }
        }
    }

    /// <summary>
    /// Set a rectangle area of tiles to a defined color.
    /// </summary>
    /// <param name="tileX">The X position of the tile to change.</param>
    /// <param name="tileY">The Y position of the tile to change.</param>
    /// <param name="tileWidth">The width of tiles to change.</param>
    /// <param name="tileHeight">The height of tiles to change.</param>
    /// <param name="color">The color to change the colors to.</param>
    /// <param name="layer">The layer to place the tiles on.</param>
    public void SetRect(int tileX, int tileY, int tileWidth, int tileHeight, Color color, Enum layer)
    {
        SetRect(tileX, tileY, tileWidth, tileHeight, color, Util.EnumValueToString(layer));
    }

    /// <summary>
    /// Set a rectangle of tiles to a tile defined by texture coordinates.
    /// </summary>
    /// <param name="tileX">The X position of the rectangle to change.</param>
    /// <param name="tileY">The Y position of the rectangle to change.</param>
    /// <param name="tileWidth">The width of tiles to change.</param>
    /// <param name="tileHeight">The height of tiles to change.</param>
    /// <param name="sourceX">The X position in the source Texture to use to draw the tiles.</param>
    /// <param name="sourceY">The Y position in the source Texture to use to draw the tiles.</param>
    /// <param name="layer">The layer to place the tiles on.</param>
    public void SetRect(int tileX, int tileY, int tileWidth, int tileHeight, int sourceX, int sourceY, string layer = "")
    {
        for (var xx = tileX; xx < tileX + tileWidth; xx++)
        {
            for (var yy = tileY; yy < tileY + tileHeight; yy++)
            {
                SetTile(xx, yy, sourceX, sourceY, layer);
            }
        }
    }

    /// <summary>
    /// Set a rectangle of tiles to a tile defined by texture coordinates.
    /// </summary>
    /// <param name="tileX">The X position of the rectangle to change.</param>
    /// <param name="tileY">The Y position of the rectangle to change.</param>
    /// <param name="tileWidth">The width of tiles to change.</param>
    /// <param name="tileHeight">The height of tiles to change.</param>
    /// <param name="sourceX">The X position in the source Texture to use to draw the tiles.</param>
    /// <param name="sourceY">The Y position in the source Texture to use to draw the tiles.</param>
    /// <param name="layer">The layer to place the tiles on.</param>
    public void SetRect(int tileX, int tileY, int tileWidth, int tileHeight, int sourceX, int sourceY, Enum layer)
    {
        SetRect(tileX, tileY, tileWidth, tileHeight, sourceX, sourceY, Util.EnumValueToString(layer));
    }

    /// <summary>
    /// Set a rectangle of tiles to a tile defined by an index.
    /// </summary>
    /// <param name="tileX">The X position of the rectangle to change.</param>
    /// <param name="tileY">The Y position of the rectangle to change.</param>
    /// <param name="tileWidth">The width of tiles to change.</param>
    /// <param name="tileHeight">The height of tiles to change.</param>
    /// <param name="tileIndex">The index of the tile to change the tiles to.</param>
    /// <param name="layer">The layer to place the tiles on.</param>
    public void SetRect(int tileX, int tileY, int tileWidth, int tileHeight, int tileIndex, string layer = "")
    {
        for (var xx = tileX; xx < tileX + tileWidth; xx++)
        {
            for (var yy = tileY; yy < tileY + tileHeight; yy++)
            {
                SetTile(xx, yy, tileIndex, layer);
            }
        }
    }

    /// <summary>
    /// Set a rectangle of tiles to a tile defined by an index.
    /// </summary>
    /// <param name="tileX">The X position of the rectangle to change.</param>
    /// <param name="tileY">The Y position of the rectangle to change.</param>
    /// <param name="tileWidth">The width of tiles to change.</param>
    /// <param name="tileHeight">The height of tiles to change.</param>
    /// <param name="tileIndex">The index of the tile to change the tiles to.</param>
    /// <param name="layer">The layer to place the tiles on.</param>
    public void SetRect(int tileX, int tileY, int tileWidth, int tileHeight, int tileIndex, Enum layer)
    {
        SetRect(tileX, tileY, tileWidth, tileHeight, tileIndex, Util.EnumValueToString(layer));
    }

    /// <summary>
    /// Set all tiles of a specific layer.
    /// </summary>
    /// <param name="tileIndex">The index of the tile to change the tiles to.</param>
    /// <param name="layer">The layer to change.</param>
    public void SetLayer(int tileIndex, string layer = "")
    {
        if (layer == "")
        {
            layer = DefaultLayerName;
        }

        SetRect(0, 0, TileColumns, TileRows, tileIndex, layer);
    }

    /// <summary>
    /// Set all tiles of a specific layer.
    /// </summary>
    /// <param name="sourceX">The X position in the source Texture to use to draw the tiles.</param>
    /// <param name="sourceY">The Y position in the source Texture to use to draw the tiles.</param>
    /// <param name="layer">The layer to change.</param>
    public void SetLayer(int sourceX, int sourceY, string layer = "")
    {
        if (layer == "")
        {
            layer = DefaultLayerName;
        }

        SetRect(0, 0, TileColumns, TileRows, sourceX, sourceY, layer);
    }

    /// <summary>
    /// Set all tiles of a specific layer.
    /// </summary>
    /// <param name="color">The color to change the tile to.</param>
    /// <param name="layer">The layer to change.</param>
    public void SetLayer(Color color, string layer = "")
    {
        if (layer == "")
        {
            layer = DefaultLayerName;
        }

        SetRect(0, 0, TileColumns, TileRows, color, layer);
    }

    /// <summary>
    /// Set all tiles of a specific layer.
    /// </summary>
    /// <param name="tileIndex">The index of the tile to change the tiles to.</param>
    /// <param name="layer">The layer to change.</param>
    public void SetLayer(int tileIndex, Enum layer)
    {
        SetLayer(tileIndex, Util.EnumValueToString(layer));
    }

    /// <summary>
    /// Set all tiles of a specific layer.
    /// </summary>
    /// <param name="sourceX">The X position in the source Texture to use to draw the tiles.</param>
    /// <param name="sourceY">The Y position in the source Texture to use to draw the tiles.</param>
    /// <param name="layer">The layer to change.</param>
    public void SetLayer(int sourceX, int sourceY, Enum layer)
    {
        SetLayer(sourceX, sourceY, Util.EnumValueToString(layer));
    }

    /// <summary>
    /// Set all tiles of a specific layer.
    /// </summary>
    /// <param name="color">The color to change the tile to.</param>
    /// <param name="layer">The layer to change.</param>
    public void SetLayer(Color color, Enum layer)
    {
        SetLayer(color, Util.EnumValueToString(layer));
    }

    /// <summary>
    /// Get the TileInfo of a specific tile on the tilemap.
    /// </summary>
    /// <param name="tileX">The X position of the tile to retrieve.</param>
    /// <param name="tileY">The Y position of the tile to retrieve.</param>
    /// <param name="layer">The layer to search through for the tile.</param>
    /// <returns>The TileInfo for the found tile.</returns>
    public TileInfo GetTile(int tileX, int tileY, string layer = "")
    {
        if (layer == "")
        {
            layer = DefaultLayerName;
        }

        if (!UsePositions)
        {
            tileX *= TileWidth;
            tileY *= TileHeight;
        }

        tileX = (int)Util.Clamp(tileX, 0, Width - TileWidth);
        tileY = (int)Util.Clamp(tileY, 0, Height - TileHeight);

        var layerDepth = layerNames[layer];
        return !tileTable.TryGetValue(layerDepth, out Dictionary<int, Dictionary<int, TileInfo>> value1)
            ? null
            : !value1.TryGetValue(tileX, out Dictionary<int, TileInfo> value2)
                ? null
                : !value2.TryGetValue(tileY, out TileInfo value)
                    ? null
                    : value;
    }

    /// <summary>
    /// Get the TileInfo of a specific tile on the tilemap.
    /// </summary>
    /// <param name="tileX">The X position of the tile to retrieve.</param>
    /// <param name="tileY">The Y position of the tile to retrieve.</param>
    /// <param name="layer">The layer to search through for the tile.</param>
    /// <returns>The TileInfo for the found tile.</returns>
    public TileInfo GetTile(int tileX, int tileY, Enum layer)
    {
        return GetTile(tileX, tileY, Util.EnumValueToString(layer));
    }

    /// <summary>
    /// Load tiles in from a GridCollider.
    /// </summary>
    /// <param name="grid">The GridCollider to reference.</param>
    /// <param name="color">The color to set tiles that are collidable on the grid.</param>
    /// <param name="layer">The layer to place the tiles on.</param>
    public void LoadGrid(GridCollider grid, Color color, string layer = "")
    {
        for (var i = 0; i < grid.TileColumns; i++)
        {
            for (var j = 0; j < grid.TileRows; j++)
            {
                if (grid.GetTile(i, j))
                {
                    SetTile(i, j, color, layer);
                }
            }
        }
    }

    /// <summary>
    /// Assign the tile data to use for LoadGridAutoTile.
    /// </summary>
    /// <param name="tileData">The tile data file.</param>
    public void SetAutoTileData(string tileData)
    {
        // Parse tile data
        autoTileTable = [];

        var dataSplit = tileData.Split('=');

        var tileValues = new List<int>() {
            128, 1, 16, 8, 0, 2, 64, 4, 32
        };

        var tileNumber = 0;

        foreach (var d in dataSplit)
        {
            var split = d.Split(':');

            var tileListString = split[0].ClearWhitespace();
            var neighborString = split[1].ClearWhitespace();

            var tileList = new List<int>();

            foreach (var t in tileListString.Split(','))
            {
                tileList.Add(int.Parse(t));
            }

            // find all the 1s, and add those up and put it as the first entry

            var tileValue = 0;
            var i = 0;
            foreach (var c in neighborString)
            {
                if (c == '1')
                {
                    tileValue += tileValues[i];
                }
                i++;
            }

            autoTileTable.TryAdd(tileValue, tileList);

            // find each ? and add that to the tile value and add that as next entries

            i = 0;
            var listOfNeighbors = new List<int>();
            foreach (var c in neighborString)
            {
                if (c == '?')
                {
                    listOfNeighbors.Add(tileValues[i]);
                }
                i++;
            }

            // Find every combination of the list of neighbors

            var powerset = Util.GetPowerSet<int>(listOfNeighbors);

            foreach (var set in powerset)
            {
                var t = tileValue;

                foreach (var n in set)
                {
                    t += n;
                }

                autoTileTable.TryAdd(t, tileList);
            }

            tileNumber++;
        }
    }

    /// <summary>
    /// Load tiles in from a GridCollider and choose tiles based on the shape of the grid.
    /// </summary>
    /// <param name="grid">The GridCollider to reference.</param>
    /// <param name="layer">The layer to place the tiles on.</param>
    public void LoadGridAutoTile(GridCollider grid, string layer = "")
    {
        if (autoTileTable == null)
        {
            SetAutoTileData(autoTileDefaultData);
        }

        for (var i = 0; i < grid.TileColumns; i++)
        {
            for (var j = 0; j < grid.TileRows; j++)
            {
                if (grid.GetTile(i, j))
                {
                    var tileValue = 0;

                    /*
                        * auto tiling grid
                        *
                        * 128 001 016
                        * 008 ___ 002
                        * 064 004 032
                        *
                        */

                    if (grid.GetTile(i - 1, j - 1))
                    {
                        tileValue += 128;
                    }
                    if (grid.GetTile(i, j - 1))
                    {
                        tileValue += 1;
                    }
                    if (grid.GetTile(i + 1, j - 1))
                    {
                        tileValue += 16;
                    }
                    if (grid.GetTile(i + 1, j))
                    {
                        tileValue += 2;
                    }
                    if (grid.GetTile(i + 1, j + 1))
                    {
                        tileValue += 32;
                    }
                    if (grid.GetTile(i, j + 1))
                    {
                        tileValue += 4;
                    }
                    if (grid.GetTile(i - 1, j + 1))
                    {
                        tileValue += 64;
                    }
                    if (grid.GetTile(i - 1, j))
                    {
                        tileValue += 8;
                    }

                    if (autoTileTable.TryGetValue(tileValue, out List<int> value))
                    {
                        tileValue = Rand.ChooseElement<int>(value);
                    }

                    SetTile(i, j, tileValue, layer);
                }
            }
        }
    }

    /// <summary>
    /// Load tiles in from a GridCollider.
    /// </summary>
    /// <param name="grid">The GridCollider to reference.</param>
    /// <param name="color">The color to set tiles that are collidable on the grid.</param>
    /// <param name="layer">The layer to place the tiles on.</param>
    public void LoadGrid(GridCollider grid, Color color, Enum layer)
    {
        LoadGrid(grid, color, Util.EnumValueToString(layer));
    }

    /// <summary>
    /// Get the layer name for a specific layer on the tilemap.
    /// </summary>
    /// <param name="layer">The layer depth id.</param>
    /// <returns>The string name of the layer.</returns>
    public string LayerName(int layer)
    {
        foreach (var l in layerNames)
        {
            if (l.Value == layer)
            {
                return l.Key;
            }
        }
        return null;
    }

    /// <summary>
    /// Get the layer depth of a layer on the tilemap by name.
    /// </summary>
    /// <param name="layer">The string layer name.</param>
    /// <returns>The layer depth id.</returns>
    public int LayerDepth(string layer)
    {
        return layerNames[layer];
    }

    /// <summary>
    /// Get the layer depth of a layer on the tilemap by enum value.
    /// </summary>
    /// <param name="layer">The enum value layer.</param>
    /// <returns>The layer depth id.</returns>
    public int LayerDepth(Enum layer)
    {
        return LayerDepth(Util.EnumValueToString(layer));
    }

    /// <summary>
    /// Load the tilemap with a color based on a string.
    /// </summary>
    /// <param name="source">The string data to load.</param>
    /// <param name="color">The color to fill occupied tiles with.</param>
    /// <param name="empty">The character that represents an empty tile.</param>
    /// <param name="filled">The character that represents a filled tile.</param>
    /// <param name="layer">The layer to place the tiles on.</param>
    public void LoadString(string source, Color color = null, char empty = '0', char filled = '1', string layer = "")
    {
        int xx = 0, yy = 0;

        color ??= Color.Red;

        for (var i = 0; i < source.Length; i++)
        {
            if (source[i] != empty && source[i] != filled)
            {
                continue;
            }

            if (xx == TileColumns)
            {
                xx = 0;
                yy++;
            }

            if (source[i] == filled)
            {
                SetTile(xx, yy, color, layer);
            }

            xx++;
        }
    }

    /// <summary>
    /// Load the tilemap with a color based on a string.
    /// </summary>
    /// <param name="source">The string data to load.</param>
    /// <param name="color">The color to fill occupied tiles with.</param>
    /// <param name="empty">The character that represents an empty tile.</param>
    /// <param name="filled">The character that represents a filled tile.</param>
    /// <param name="layer">The layer to place the tiles on.</param>
    public void LoadString(string source, Color color, char empty, char filled, Enum layer)
    {
        LoadString(source, color, empty, filled, Util.EnumValueToString(layer));
    }

    /// <summary>
    /// Load the tilemap from a CSV formatted string.
    /// </summary>
    /// <param name="str">The string data to load.</param>
    /// <param name="columnSep">The character that separates columns in the CSV.</param>
    /// <param name="rowSep">The character that separates rows in the CSV.</param>
    /// <param name="layer">The layer to place the tiles on.</param>
    public void LoadCSV(string str, char columnSep = ',', char rowSep = '\n', string layer = "")
    {
        var u = UsePositions;
        UsePositions = false;

        var row = str.Split(rowSep);
        var rows = row.Length;
        string[] col;
        int cols;
        int x;
        int y;

        for (y = 0; y < rows; y++)
        {
            if (row[y] == "")
            {
                continue;
            }

            col = row[y].Split(columnSep);
            cols = col.Length;
            for (x = 0; x < cols; x++)
            {
                if (col[x].Equals("") || Convert.ToInt32(col[x]) < 0)
                {
                    continue;
                }

                SetTile(x, y, Convert.ToInt16(col[x]), layer);
            }
        }

        UsePositions = u;
    }

    /// <summary>
    /// Load the tilemap from a CSV formatted string.
    /// </summary>
    /// <param name="str">The string data to load.</param>
    /// <param name="columnSep">The character that separates columns in the CSV.</param>
    /// <param name="rowSep">The character that separates rows in the CSV.</param>
    /// <param name="layer">The layer to place the tiles on.</param>
    public void LoadCSV(string str, char columnSep, char rowSep, Enum layer)
    {
        LoadCSV(str, columnSep, rowSep, Util.EnumValueToString(layer));
    }

    /// <summary>
    /// Remove a tile from the tilemap.
    /// </summary>
    /// <param name="tileX">The tile's X position on the map.</param>
    /// <param name="tileY">The tile's Y position on the map.</param>
    /// <param name="layer">The tile's layer.</param>
    /// <returns>The TileInfo for the cleared tile.</returns>
    public TileInfo ClearTile(int tileX, int tileY, string layer = "")
    {
        if (layer == "")
        {
            layer = DefaultLayerName;
        }

        var t = GetTile(tileX, tileY, layer);

        if (!UsePositions)
        {
            tileX *= TileWidth;
            tileY *= TileHeight;
        }

        tileX = (int)Util.Clamp(tileX, 0, Width - TileWidth);
        tileY = (int)Util.Clamp(tileY, 0, Height - TileHeight);

        RemoveTile(tileX, tileY, layerNames[layer]);

        if (t != null)
        {
            TileLayers[layerNames[layer]].Remove(t);
        }

        NeedsUpdate = true;

        return t;
    }

    /// <summary>
    /// Remove a tile from the tilemap.
    /// </summary>
    /// <param name="tileX">The tile's X position on the map.</param>
    /// <param name="tileY">The tile's Y position on the map.</param>
    /// <param name="layer">The tile's layer.</param>
    /// <returns>The TileInfo for the cleared tile.</returns>
    public TileInfo ClearTile(int tileX, int tileY, Enum layer)
    {
        return ClearTile(tileX, tileY, Util.EnumValueToString(layer));
    }

    /// <summary>
    /// Clear all tiles on a specific layer.
    /// </summary>
    /// <param name="layer">The string layer name.</param>
    public void ClearLayer(string layer = "")
    {
        if (layer == "")
        {
            layer = DefaultLayerName;
        }

        TileLayers[layerNames[layer]].Clear();

        NeedsUpdate = true;
    }

    /// <summary>
    /// Clear all tiles on a specific layer.
    /// </summary>
    /// <param name="layer">The enum value layer.</param>
    public void ClearLayer(Enum layer)
    {
        ClearLayer(Util.EnumValueToString(layer));
    }

    /// <summary>
    /// Clear all tiles on all layers.
    /// </summary>
    public void ClearAll()
    {
        foreach (var kv in TileLayers)
        {
            kv.Value.Clear();
        }

        NeedsUpdate = true;
    }

    /// <summary>
    /// Clear all tiles inside a specified rectangle.
    /// </summary>
    /// <param name="tileX">The X position of the rectangle to clear.</param>
    /// <param name="tileY">The Y position of the rectangle to clear.</param>
    /// <param name="tileWidth">The width of tiles to clear.</param>
    /// <param name="tileHeight">The height of tiles to clear.</param>
    /// <param name="layer">The layer to clear tiles from.</param>
    public void ClearRect(int tileX, int tileY, int tileWidth, int tileHeight, string layer = "")
    {
        for (var xx = tileX; xx < tileX + tileWidth; xx++)
        {
            for (var yy = tileY; yy < tileY + tileHeight; yy++)
            {
                ClearTile(xx, yy, layer);
            }
        }
    }

    /// <summary>
    /// Add a new layer to the Tilemap.
    /// </summary>
    /// <param name="name">The string name of the layer.</param>
    /// <param name="depth">The depth of the tiles.</param>
    /// <returns>The depth id of the layer.</returns>
    public int AddLayer(string name, int depth = 0)
    {
        layerNames.Add(name, depth);
        TileLayers.Add(depth, []);

        return TileLayers.Count - 1;
    }

    /// <summary>
    /// Add a new layer to the Tilemap.
    /// </summary>
    /// <param name="name">The enum value of the layer.</param>
    /// <param name="depth">The depth of the tiles.</param>
    /// <returns>The depth id of the layer.</returns>
    public int AddLayer(Enum name, int depth = 0)
    {
        return AddLayer(Util.EnumValueToString(name), depth);
    }

    /// <summary>
    /// Remove a layer from the Tilemap and delete that layer's tiles.
    /// </summary>
    /// <param name="name">The name of the layer to delete.</param>
    public void RemoveLayer(string name)
    {
        ClearLayer(name);
        if (name != DefaultLayerName)
        {
            TileLayers.Remove(layerNames[name]);
            layerNames.Remove(name);
        }
    }

    /// <summary>
    /// Remove a layer from the Tilemap and delete that layer's tiles.
    /// </summary>
    /// <param name="name">The name of the layer to delete.</param>
    public void RemoveLayer(Enum name)
    {
        RemoveLayer(Util.EnumValueToString(name));
    }

    /// <summary>
    /// Check if a layer exists.
    /// </summary>
    /// <param name="name">The string name of the layer.</param>
    /// <returns>True if the layer exists.</returns>
    public bool LayerExists(string name)
    {
        return TileLayers.ContainsKey(layerNames[name]);
    }

    /// <summary>
    /// Check if a layer exists.
    /// </summary>
    /// <param name="name">The enum value of the layer.</param>
    /// <returns>True if the layer exists.</returns>
    public bool LayerExists(Enum name)
    {
        return LayerExists(Util.EnumValueToString(name));
    }

    /// <summary>
    /// Merges another tilemap into this one.
    /// </summary>
    /// <param name="other">The tilemap to merge into this one.</param>
    /// <param name="above">True if the other tilemap's base layer should be above this one's base layer.</param>
    public void MergeTilemap(Tilemap other, string layerPrefix = "", int layerOffset = -1)
    {
        foreach (var layer in other.TileLayers)
        {
            AddLayer(layerPrefix + other.LayerName(layer.Key), layer.Key + layerOffset);
            TileLayers[layer.Key + layerOffset] = new List<TileInfo>(other.TileLayers[layer.Key]);
        }
    }

    /// <summary>
    /// Get the list of tiles on a specific layer.
    /// </summary>
    /// <param name="layerName">The name of the layer.</param>
    /// <returns>A list of tiles on that layer.</returns>
    public List<TileInfo> GetTiles(string layerName)
    {
        return TileLayers[LayerDepth(layerName)];
    }

    /// <summary>
    /// Get the list of tiles on a specific layer.
    /// </summary>
    /// <param name="layerDepth"></param>
    /// <returns>A list of tiles on that layer.</returns>
    public List<TileInfo> GetTiles(int layerDepth)
    {
        return TileLayers[layerDepth];
    }

    /// <summary>
    /// Get the list of tiles on a specific layer.
    /// </summary>
    /// <param name="layerName">The enum value of the layer.</param>
    /// <returns>A list of tiles on that layer.</returns>
    public List<TileInfo> GetTiles(Enum layerName)
    {
        return TileLayers[LayerDepth(layerName)];
    }

    /// <summary>
    /// Get the index of a specific tile on the source Texture.
    /// </summary>
    /// <param name="tile">The tile to get the index of.</param>
    /// <returns>The index of the tile.</returns>
    public int GetTileIndex(TileInfo tile)
    {
        return tile.GetIndex(this);
    }

    #endregion

}
