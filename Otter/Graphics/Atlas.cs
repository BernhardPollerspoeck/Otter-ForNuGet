using System.Collections.Generic;
using System.IO;
using System.Xml;

using Otter.Graphics.Drawables;
using Otter.Utility;

namespace Otter.Graphics;

/// <summary>
/// Class used for loading textures from an Atlas, or a set of Atlases. This class is built to support
/// atlases created with Sparrow/Starling exporting from TexturePacker http://www.codeandweb.com/texturepacker
/// </summary>
public class Atlas
{
    #region Private Fields

    private readonly Dictionary<string, AtlasTexture> subtextures = [];

    #endregion

    #region Constructors

    /// <summary>
    /// Designed for Sparrow/Starling exporting from TexturePacker http://www.codeandweb.com/texturepacker
    /// </summary>
    /// <param name="source">The reltive path to the atlas data file.  The png should also be in the same directory.</param>
    public Atlas(string source = "")
    {
        if (source != "")
        {
            Add(source);
        }
    }

    #endregion

    #region Indexers

    /// <summary>
    /// Get an AtlasTexture by name.
    /// </summary>
    /// <param name="name">The name of the image in the atlas data.</param>
    /// <returns>An AtlasTexture.</returns>
    public AtlasTexture this[string name]
    {
        get
        {
            return GetTexture(name);
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Add another atlas to the collection of textures.  Duplicate names will destroy this.
    /// </summary>
    /// <param name="source">The relative path to the data file.  The png should be in the same directory.</param>
    public Atlas Add(string source)
    {
        var xml = new XmlDocument();
        xml.Load(source);
        _ = xml.GetElementsByTagName("TextureAtlas");

        var imagePath = Path.GetDirectoryName(source) + "/";

        if (imagePath == "/")
        {
            imagePath = "";
        }

        foreach (XmlElement a in xml.GetElementsByTagName("TextureAtlas"))
        {
            foreach (XmlElement e in xml.GetElementsByTagName("SubTexture"))
            {
                var name = e.AttributeString("name");
                var uniqueName = true;

                foreach (var atest in subtextures.Values)
                {
                    if (atest.Name == name)
                    {
                        uniqueName = false;
                        break;
                    }
                }

                if (uniqueName)
                {
                    var atext = new AtlasTexture
                    {
                        X = e.AttributeInt("x"),
                        Y = e.AttributeInt("y"),
                        Width = e.AttributeInt("width"),
                        Height = e.AttributeInt("height")
                    };
                    atext.FrameHeight = e.AttributeInt("frameHeight", atext.Height);
                    atext.FrameWidth = e.AttributeInt("frameWidth", atext.Width);
                    atext.FrameX = e.AttributeInt("frameX", 0);
                    atext.FrameY = e.AttributeInt("frameY", 0);
                    atext.Name = name;
                    atext.Source = imagePath + a.AttributeString("imagePath");
                    atext.Texture = new Texture(atext.Source);
                    subtextures.Add(e.AttributeString("name"), atext);
                }
            }
        }
        return this;
    }

    /// <summary>
    /// Add multiple sources to the Atlas.
    /// </summary>
    /// <param name="sources">The file path to the sources.</param>
    /// <returns>The Atlas.</returns>
    public Atlas AddMultiple(params string[] sources)
    {
        foreach (var s in sources)
        {
            Add(s);
        }
        return this;
    }

    /// <summary>
    /// Add multiple atlases from a set created by texture packer.
    /// Note: This only supports up to 10 atlases (0 - 9)
    /// </summary>
    /// <param name="source">The path until the number.  For example: "assets/atlas" if the path is "assets/atlas0.xml"</param>
    /// <param name="extension">The extension of the source without a dot</param>
    /// <returns>The Atlas.</returns>
    public Atlas AddNumbered(string source, string extension = "xml")
    {
        var i = 0;

        while (File.Exists(source + i + "." + extension))
        {
            Add(source + i + "." + extension);
            i++;
        }

        return this;
    }

    /// <summary>
    /// Creates a new Image from an AtlasTexture.
    /// </summary>
    /// <param name="name">The name of the texture in the atlas data.</param>
    /// <returns>The created Image.</returns>
    public Image CreateImage(string name)
    {
        return new Image(this[name]);
    }

    /// <summary>
    /// Creates a new Spritemap from an AtlasTexture.
    /// </summary>
    /// <typeparam name="T">The type to use to reference animations.</typeparam>
    /// <param name="name">The name of the texture in the atlas data.</param>
    /// <param name="width">The width of the cell on the sprite sheet.</param>
    /// <param name="height">The height of the cell on the sprite sheet.</param>
    /// <returns>The new Spritemap.</returns>
    public Spritemap<T> CreateSpritemap<T>(string name, int width, int height)
    {
        return new Spritemap<T>(this[name], width, height);
    }



    /// <summary>
    /// Get an AtlasTexture by name.
    /// </summary>
    /// <param name="name">The name of the image in the atlas data.</param>
    /// <returns>An AtlasTexture.</returns>
    public AtlasTexture GetAtlasTexture(string name)
    {
        return GetTexture(name);
    }

    /// <summary>
    /// Tests if a texture by the specified name exists in the atlas data.
    /// </summary>
    /// <param name="name">The name of the texture to test.</param>
    /// <returns>True if the atlas data contains a texture by the specified name.</returns>
    public bool Exists(string name)
    {
        return subtextures.ContainsKey(name);
    }

    #endregion

    #region Internal

    internal AtlasTexture GetTexture(string name)
    {
        var a = subtextures[name];

        return a;
    }

    #endregion
}
