using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

using Otter.Core;
using Otter.Colliders;
using Otter.Graphics;
using Otter.Graphics.Drawables;
using Otter.Utility.MonoGame;

namespace Otter.Utility;

/// <summary>
/// Class used for importing OgmoProject files quickly, and loading levels created in Ogmo Editor
/// (http://ogmoeditor.com)  Currently OgmoProjects must export in XML Co-ords for Tiles and Entities,
/// and Bitstring for Grids.
/// </summary>
public class OgmoProject
{

    #region Private Fields

    private readonly Dictionary<string, int> ColliderTags = [];
    private readonly Dictionary<string, string> levelValueTypes = [];
    private readonly Dictionary<string, string> assetMappings = [];

    #endregion

    #region Public Fields

    /// <summary>
    /// Determines if grid layers will render in the game.  Only applies at loading time.
    /// </summary>
    public bool DisplayGrids = true;

    /// <summary>
    /// The default image path to search for tilemaps in.
    /// </summary>
    public string ImagePath;

    /// <summary>
    /// Determines if loaded levels will use camera bounds in the Scene.
    /// </summary>
    public bool UseCameraBounds = true;

    /// <summary>
    /// Determines if tilemaps are located in an Atlas.
    /// </summary>
    public bool UseAtlas;

    /// <summary>
    /// The default background color of the Ogmo Project.
    /// </summary>
    public Color BackgroundColor;

    /// <summary>
    /// The default background grid color of the Ogmo Project.
    /// </summary>
    public Color GridColor;

    /// <summary>
    /// The known layers loaded from the Ogmo Editor oep file.
    /// </summary>
    public Dictionary<string, OgmoLayer> Layers = [];

    /// <summary>
    /// Mapping the tile layers to file paths.
    /// </summary>
    public Dictionary<string, string> TileMaps = [];

    /// <summary>
    /// The entities stored to create tilemaps and grids.  Cleared every time LoadLevel is called.
    /// </summary>
    public Dictionary<string, Entity> Entities = [];

    /// <summary>
    /// The name of the method to use for creating Entities when loading an .oel file into a Scene.
    /// </summary>
    public string CreationMethodName = "CreateFromXml";

    /// <summary>
    /// The drawing layer to place the first loaded tile map on.
    /// </summary>
    public int BaseTileDepth;

    /// <summary>
    /// Determines the drawing layers for each subsequently loaded tile map.  For example, the first
    /// tilemap will be at Layer 0, the second at Layer 100, the third at Layer 200, etc.
    /// </summary>
    public int TileDepthIncrement = 100;

    #endregion

    #region Public Properties

    /// <summary>
    /// The level data last loaded with LoadLevel()
    /// </summary>
    public string CurrentLevel { get; private set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Create an OgmoProject from a source .oep file.
    /// </summary>
    /// <param name="source">The path to the .oep file.</param>
    /// <param name="imagePath">The default image path to use for loading tilemaps.</param>
    public OgmoProject(string source, string imagePath = "")
    {
        source = FileHandling.GetAbsoluteFilePath(source);
        if (!File.Exists(source))
        {
            throw new ArgumentException("Ogmo project file could not be found.");
        }

        if (imagePath == "")
        {
            UseAtlas = true;
        }
        ImagePath = imagePath;

        var xmlDoc = new XmlDocument();
        xmlDoc.Load(source);

        BackgroundColor = new Color(xmlDoc["project"]["BackgroundColor"]);
        GridColor = new Color(xmlDoc["project"]["GridColor"]);

        var xmlLayers = xmlDoc.GetElementsByTagName("LayerDefinition");
        foreach (XmlElement x in xmlLayers)
        {
            var layer = new OgmoLayer(x);
            Layers.Add(layer.Name, layer);
        }

        //I dont know if I need to do this
        var xmlEntities = xmlDoc.GetElementsByTagName("EntityDefinition");
        foreach (XmlElement _ in xmlEntities)
        {

        }

        var xmlTilesets = xmlDoc.GetElementsByTagName("Tileset");
        foreach (XmlElement x in xmlTilesets)
        {
            TileMaps.Add(x["Name"].InnerText, x["FilePath"].InnerText);
        }

        //var xmlLevelValues = xmlDoc.GetElementsByTagName("ValueDefinitions");
        //dirty dirty hack because there should only be one element with that name
        //and for SOME REASON I can't just grab an XmlElement, I have to grab a NodeList and enumerate it for my element. What gives, microsoft?
        var xmlLevelValues = xmlDoc.GetElementsByTagName("LevelValueDefinitions")[0] as XmlElement;
        foreach (XmlElement x in xmlLevelValues.GetElementsByTagName("ValueDefinition"))
        {
            levelValueTypes.Add(x.Attributes["Name"].Value, x.Attributes["xsi:type"].Value);
        }
    }

    #endregion

    #region Private Methods

    private void CreateEntity(XmlElement e, Scene scene)
    {
        var arguments = new object[2];
        arguments[0] = scene;
        arguments[1] = e.Attributes;

        var entityTypes = Util.GetTypesFromAllAssemblies<Entity>(e.Name).ToList();
        var creationMethods = entityTypes
            .Select<Type, Action>(entityType =>
            {
                var staticMethod = entityType.GetMethod(CreationMethodName,
                    BindingFlags.Static | BindingFlags.Public);
                if (staticMethod != null)
                {
                    return () => staticMethod.Invoke(null, arguments);
                }

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0046 // Convert to conditional expression
                if (HasBasicConstructor(entityType) || HasDataConstructor(entityType))
                {
                    return () =>
                    {
                        // Attempt to create with just constructor
                        var x = e.AttributeInt("x");
                        var y = e.AttributeInt("y");
                        Entity entity = null;
                        if (HasBasicConstructor(entityType))
                        {
                            entity = (Entity)Activator.CreateInstance(entityType, x, y);
                        }
                        else if (HasDataConstructor(entityType))
                        {
                            entity = (Entity)Activator.CreateInstance(entityType, x, y, new OgmoData(e.Attributes));
                        }
                        if (entity != null)
                        {
                            scene.Add(entity);
                        }
                    };
                }
#pragma warning restore IDE0046 // Convert to conditional expression
#pragma warning restore IDE0079 // Remove unnecessary suppression
                return null;
            });

        creationMethods.FirstOrDefault()?.Invoke();
    }

    private static bool HasDataConstructor(Type entityType)
    {
        return entityType.GetConstructor([typeof(int), typeof(int), typeof(OgmoData)]) != null;
    }

    private static bool HasBasicConstructor(Type entityType)
    {
        return entityType.GetConstructor([typeof(int), typeof(int)]) != null;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Assign a replacement asset for a Tilemap when LoadLevel is called.
    /// </summary>
    /// <param name="searchPath">The asset path to find (searches at the end of the string!)</param>
    /// <param name="replacement">The full path to replace the matching asset with.</param>
    public void RemapAsset(string searchPath, string replacement)
    {
        assetMappings.Add(searchPath, replacement);
    }

    /// <summary>
    /// Get a value from an Ogmo level.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="name">The name of the value.</param>
    /// <param name="data">The level data to use.  If left blank will use the CurrentLevel.</param>
    /// <returns>The value cast to type T.</returns>
    public T GetValue<T>(string name, string data = "")
    {
        if (data == "")
        {
            data = CurrentLevel;
        }

        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(data);

        var xmlLevel = xmlDoc["level"];

        var value = xmlLevel.Attributes[name].Value;

        return typeof(T) == typeof(Color)
            ? (T)Activator.CreateInstance(typeof(T), value.Substring(1, 6))
            : (T)Convert.ChangeType(value, typeof(T));
    }

    /// <summary>
    /// Get a value from an Ogmo level.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="name">The name of the value.</param>
    /// <param name="source">The level data to use.  If left blank will use the CurrentLevel.</param>
    /// <returns>The value cast to type T.</returns>
    public T GetValue<T>(Enum name, string source = "")
    {
        return GetValue<T>(Util.EnumValueToBasicString(name), source);
    }

    public T GetValueFromFile<T>(string name, string path)
    {
        return (T)GetValue<T>(name, File.ReadAllText(path));
    }

    public string GetLayerData(string name, string data = "")
    {
        if (data == "")
        {
            data = CurrentLevel;
        }

        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(data);

        var xmlLevel = xmlDoc["level"];

        return xmlLevel[name].InnerText;
    }

    /// <summary>
    /// Load level data from a string into a Scene.
    /// </summary>
    /// <param name="data">The level data to load.</param>
    /// <param name="scene">The Scene to load into.</param>
    public void LoadLevel(string data, Scene scene)
    {
        data = FileHandling.GetAbsoluteFilePath(data);
        Entities.Clear();

        CurrentLevel = data;

        var xmlDoc = new XmlDocument();
        xmlDoc.Load(data);

        var xmlLevel = xmlDoc["level"];

        scene.Width = int.Parse(xmlDoc["level"].Attributes["width"].Value);
        scene.Height = int.Parse(xmlDoc["level"].Attributes["height"].Value);

        var i = 0;

        foreach (var layer in Layers.Values)
        {
            if (layer.Type == "GridLayerDefinition")
            {
                var Entity = new Entity();

                var grid = new GridCollider(scene.Width, scene.Height, layer.GridWidth, layer.GridHeight);

                grid.LoadString(xmlLevel[layer.Name].InnerText);
                if (ColliderTags.TryGetValue(layer.Name, out var value))
                {
                    grid.AddTag(value);
                }

                if (DisplayGrids)
                {
                    var tilemap = new Tilemap(scene.Width, scene.Height, layer.GridWidth, layer.GridHeight);
                    tilemap.LoadString(xmlLevel[layer.Name].InnerText, layer.Color);
                    Entity.AddGraphic(tilemap);
                }

                Entity.AddCollider(grid);

                scene.Add(Entity);
                Entities.Add(layer.Name, Entity);
            }
            if (layer.Type == "TileLayerDefinition")
            {
                var Entity = new Entity();

                var xmlTiles = xmlLevel[layer.Name];

                var tileset = xmlTiles.Attributes["tileset"].Value;

                var tilepath = ImagePath + TileMaps[tileset];

                foreach (var kv in assetMappings)
                {
                    var find = kv.Key;
                    var replace = kv.Value;

                    if (tilepath.EndsWith(find))
                    {
                        tilepath = replace;
                        break;
                    }
                }

                var tilemap = new Tilemap(tilepath, scene.Width, scene.Height, layer.GridWidth, layer.GridHeight);

                var exportMode = xmlTiles.Attributes["exportMode"].Value;
                switch (exportMode)
                {
                    case "CSV":
                        tilemap.LoadCSV(xmlTiles.InnerText);
                        break;
                    case "XMLCoords":
                        foreach (XmlElement t in xmlTiles)
                        {
                            tilemap.SetTile(t);
                        }
                        break;
                }

                tilemap.Update();

                Entity.AddGraphic(tilemap);

                Entity.Layer = BaseTileDepth - (i * TileDepthIncrement);
                i++;

                scene.Add(Entity);
                Entities.Add(layer.Name, Entity);
            }
            if (layer.Type == "EntityLayerDefinition")
            {
                var xmlEntities = xmlLevel[layer.Name];

                if (xmlEntities != null)
                {
                    foreach (XmlElement e in xmlEntities)
                    {
                        CreateEntity(e, scene);
                    }
                }
            }

        }

        if (UseCameraBounds)
        {
            scene.CameraBounds = new Rectangle(0, 0, scene.Width, scene.Height);
            scene.UseCameraBounds = true;
        }
    }

    /// <summary>
    /// Load data into a Scene from a source .oel file.
    /// </summary>
    /// <param name="path">The oel to load.</param>
    /// <param name="scene">The Scene to load into.</param>
    public void LoadLevelFromFile(string path, Scene scene)
    {
        path = FileHandling.GetAbsoluteFilePath(path);
        LoadLevel(File.ReadAllText(path), scene);
    }

    /// <summary>
    /// Register a collision tag on a grid layer loaded from the oel file.
    /// </summary>
    /// <param name="tag">The tag to use.</param>
    /// <param name="layerName">The layer name that should use the tag.</param>
    public void RegisterTag(int tag, string layerName)
    {
        ColliderTags.Add(layerName, tag);
    }

    /// <summary>
    /// Register a collision tag on a grid layer loaded from the oel file.
    /// </summary>
    /// <param name="tag">The enum tag to use. (Casts to int!)</param>
    /// <param name="layerName">The layer name that should use the tag.</param>
    public void RegisterTag(Enum tag, string layerName)
    {
        RegisterTag(Convert.ToInt32(tag), layerName);
    }

    /// <summary>
    /// Get the Entity that was created for a specific Ogmo layer.
    /// </summary>
    /// <param name="layerName">The name of the layer to find.</param>
    /// <returns>The Entity created for that layer.</returns>
    public Entity GetEntityFromLayerName(string layerName)
    {
        return Entities[layerName];
    }

    /// <summary>
    /// Get a list of all the known layer names from the .oep file.
    /// </summary>
    /// <returns></returns>
    public List<string> GetLayerNames()
    {
        var s = new List<string>();

        foreach (var l in Layers)
        {
            s.Add(l.Key);
        }

        return s;
    }

    #endregion

}
