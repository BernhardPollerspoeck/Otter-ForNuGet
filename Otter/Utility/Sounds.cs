using System.Collections.Generic;
using System.IO;

using SFML.Audio;

namespace Otter.Utility;

/// <summary>
/// Class that manages the cache of sounds.
/// </summary>
internal class Sounds
{
    private static readonly Dictionary<string, SoundBuffer> sounds = [];

    public static SoundBuffer Load(string path)
    {
        path = FileHandling.GetAbsoluteFilePath(path);
        if (!Files.FileExists(path))
        {
            throw new FileNotFoundException(path + " not found.");
        }

        if (sounds.TryGetValue(path, out SoundBuffer value))
        {
            return value;
        }
        sounds.Add(path, new SoundBuffer(Files.LoadFileBytes(path)));
        return sounds[path];
    }
}
