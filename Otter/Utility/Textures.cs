using System.Collections.Generic;
using System.IO;

namespace Otter.Utility;

/// <summary>
/// Class that manages the cache of Textures.
/// </summary>
public class Textures
{
    #region Static Fields

    private static readonly Dictionary<string, SFML.Graphics.Texture> textures = [];
    private static readonly Dictionary<Stream, SFML.Graphics.Texture> texturesStreamed = [];

    #endregion

    #region Static Methods

    /// <summary>
    /// This doesn't really work right now.  Textures in images wont update
    /// if you do this.
    /// </summary>
    /// <param name="path"></param>
    public static void Reload(string path)
    {
        textures.Remove(path);
        Load(path);
    }

    /// <summary>
    /// This doesn't work right now.  Textures in images wont update if you
    /// do this.
    /// </summary>
    public static void ReloadAll()
    {
        var keys = textures.Keys;
        textures.Clear();
        foreach (var k in keys)
        {
            Load(k);
        }
    }

    #endregion

    #region Internal

    internal static SFML.Graphics.Texture Load(string path)
    {
        path = FileHandling.GetAbsoluteFilePath(path);
        //if (!File.Exists(source)) throw new FileNotFoundException("Texture path " + source + " not found.");
        if (!Files.FileExists(path))
        {
            throw new FileNotFoundException("Texture path " + path + " not found.");
        }

        if (textures.TryGetValue(path, out SFML.Graphics.Texture value))
        {
            return value;
        }
        textures.Add(path, new SFML.Graphics.Texture(Files.LoadFileBytes(path)));
        return textures[path];
    }

    internal static SFML.Graphics.Texture Load(Stream stream)
    {
        if (stream != null)
        {
            if (texturesStreamed.TryGetValue(stream, out SFML.Graphics.Texture value))
            {
                return value;
            }
            else
            {
                texturesStreamed.Add(stream, new SFML.Graphics.Texture(stream));
                return texturesStreamed[stream];
            }
        }
        else
        {
            return null;
        }
    }

    #endregion
}
