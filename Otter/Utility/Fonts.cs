using System.Collections.Generic;
using System.IO;

namespace Otter.Utility;

/// <summary>
/// Class that manages the cache of fonts.
/// </summary>
internal class Fonts
{
    private static readonly Dictionary<string, SFML.Graphics.Font> fonts = [];
    private static readonly Dictionary<Stream, SFML.Graphics.Font> fontsStreamed = [];

    public static SFML.Graphics.Font DefaultFont
    {
        get
        {
            defaultFont ??= Load("CONSOLA.TTF");
            return defaultFont;
        }
    }

    private static SFML.Graphics.Font defaultFont;

    internal static SFML.Graphics.Font Load(string path)
    {
        path = FileHandling.GetAbsoluteFilePath(path);
        if (!Files.FileExists(path))
        {
            throw new FileNotFoundException(path + " not found.");
        }

        if (fonts.TryGetValue(path, out SFML.Graphics.Font value))
        {
            return value;
        }

        if (Files.IsUsingDataPack(path))
        {
            var stream = new MemoryStream(Files.LoadFileBytes(path));
            fonts.Add(path, new SFML.Graphics.Font(stream)); // SFML fix? Might be memory leaking when you have a lot of fonts.
            //stream.Close();
            //fonts.Add(path, new SFML.Graphics.Font(Files.LoadFileBytes(path))); // SFML fix?
        }
        else
        {
            if (File.Exists(path))
            {
                fonts.Add(path, new SFML.Graphics.Font(path)); // Cant load font with bytes from path?
            }
            else
            { // This should work because we already checked FileExists above
                fonts.Add(path, new SFML.Graphics.Font(Files.AssetsFolderPrefix + path)); // Cant load font with bytes from path?
            }
        }
        return fonts[path];
    }

    internal static SFML.Graphics.Font Load(Stream stream)
    {
        if (stream != null)
        {
            if (fontsStreamed.TryGetValue(stream, out SFML.Graphics.Font value))
            {
                return value;
            }
            else
            {
                fontsStreamed.Add(stream, new SFML.Graphics.Font(stream));
                return fontsStreamed[stream];
            }
        }
        else
        {
            return null;
        }
    }
}
