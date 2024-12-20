using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Otter.Utility;

/// <summary>
/// Class that is used for storing strings, ints, floats, or bools with keys of enum or string.  The
/// saver can output data in an semi-encrypted format, and also an editable config file format.
/// </summary>
/// <remarks>
/// Initializes a new instance of the DataSaver class.
/// </remarks>
/// <param name="defaultPath">The default path.</param>
public partial class DataSaver(string defaultPath = "")
{

    /// <summary>
    /// The string to use when delimiting key data in data exports.
    /// </summary>
    public string KeyDelim = "::OTTERK::";

    /// <summary>
    /// The string to use when delimiting value data in data exports.
    /// </summary>
    public string ValueDelim = "::OTTERV::";

    /// <summary>
    /// The phrase to use as a salt when encrypting the data exports.
    /// </summary>
    public string EncryptionSalt = "otter";

    /// <summary>
    /// The guide to salt the data string.  {S} is the salt, {D} is the data.
    /// It is recommended to change this from the default for your game, but
    /// only if you really care about hacking save data.
    /// </summary>
    public string SaltGuide = "{S}{D}{S}";

    /// <summary>
    /// The default path that the files will be imported from and exported to.
    /// </summary>
    public string DefaultPath = defaultPath;

    /// <summary>
    /// The default file name that the data will export as.
    /// </summary>
    public string DefaultFilename = "data";

    /// <summary>
    /// The export mode for the data.
    /// Data: Semi-encrypted uneditable data.
    /// Config: Easy to hand edit unencrypted data.
    /// </summary>
    public DataExportMode ExportMode = DataExportMode.Data;

    /// <summary>
    /// The export modes for the data.
    /// </summary>
    public enum DataExportMode
    {
        Data,
        Config
    }

    private Dictionary<string, string> data = [];

    /// <summary>
    /// Verifies the specified string data.  Only applies to DataExportMode.Data.
    /// </summary>
    /// <param name="stringData">The string data.</param>
    /// <returns>True if the data is successfully verified.</returns>
    public bool Verify(string stringData)
    {
        var split = MyRegex().Split(stringData);

        if (split.Length != 2)
        {
            return false;
        }

        var dataToHash = SaltGuide;
        dataToHash = dataToHash.Replace("{S}", EncryptionSalt);
        dataToHash = dataToHash.Replace("{D}", split[1]);

        var hash = Util.MD5Hash(dataToHash);

        return hash == split[0];
    }

    /// <summary>
    /// Deletes the exported file for this save data.
    /// </summary>
    /// <param name="filename">The filename to delete (usually you don't have to set this.)</param>
    public void ClearFile(string filename = "")
    {
        if (filename == "")
        {
            filename = DefaultFilename;
        }

        filename = DefaultPath + filename;
        if (!File.Exists(filename))
        {
            return;
        }

        File.Delete(filename);
    }

    /// <summary>
    /// Clears the data.
    /// </summary>
    public void Clear()
    {
        data = [];
    }

    /// <summary>
    /// Check if an exported file exists for this data.
    /// </summary>
    /// <param name="filename">The filename to check.</param>
    /// <returns>True if the exported file exists, and is verified for encrypted files.</returns>
    public bool FileExists(string filename = "")
    {
        if (filename == "")
        {
            filename = DefaultFilename;
        }

        filename = DefaultPath + filename;
        if (File.Exists(filename))
        {
            var loaded = File.ReadAllText(filename);

            return ExportMode != DataExportMode.Data || Verify(loaded);
        }

        return false;
    }

    /// <summary>
    /// Imports the data in the specified file.
    /// </summary>
    /// <param name="filename">The filename.</param>
    /// <param name="verify">if set to true verify the data before importing.</param>
    public void Import(string filename = "", bool verify = true)
    {
        if (filename == "")
        {
            filename = DefaultFilename;
        }

        filename = DefaultPath + filename;
        if (!File.Exists(filename))
        {
            return;
        }

        var loaded = File.ReadAllText(filename);

        if (ExportMode == DataExportMode.Data)
        {
            if (Verify(loaded) || !verify)
            {
                var split = MyRegex().Split(loaded);
                loaded = Util.DecompressString(split[1]);

                var splitData = Regex.Split(loaded, ValueDelim);

                foreach (var s in splitData)
                {
                    var entry = Regex.Split(s, KeyDelim);
                    var key = entry[0];
                    var value = entry[1];
                    if (!data.TryAdd(key, value))
                    {
                        data[key] = value;
                    }
                }
            }
            else
            {
                Util.Log("Data load failed: corrupt or modified data.");
            }
        }
        else
        {
            var split = loaded.Split('\n');
            foreach (var s in split)
            {
                var entry = s.Split('=');
                if (entry.Length == 2)
                {
                    SetData(entry[0].Replace((char)16, '='), entry[1].Replace((char)16, '='));
                }
            }
        }
    }

    /// <summary>
    /// Exports the data to the specified file.
    /// </summary>
    /// <param name="filename">The filename.</param>
    public void Export(string filename = "")
    {
        if (filename == "")
        {
            filename = DefaultFilename;
        }

        filename = DefaultPath + filename;

        if (ExportMode == DataExportMode.Data)
        {
            var str = Util.CompressString(Util.DictionaryToString(data, KeyDelim, ValueDelim));

            var dataToHash = SaltGuide;
            dataToHash = dataToHash.Replace("{S}", EncryptionSalt);
            dataToHash = dataToHash.Replace("{D}", str);

            str = Util.MD5Hash(dataToHash) + ":" + str;

            File.WriteAllText(filename, str);
        }
        else
        {
            var str = "";
            foreach (var e in data)
            {
                str += "" + e.Key.Replace('=', (char)16) + "=" + e.Value.Replace('=', (char)16) + "\n";
            }

            str = str.Trim();

            File.WriteAllText(filename, str);
        }
    }

    /// <summary>
    /// Gets the string with the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>The string data or null.</returns>
    public string this[string key]
    {
        get
        {
            return data.TryGetValue(key, out var value) ? value : null;
        }
    }

    /// <summary>
    /// Gets the string with the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>The string data or null.</returns>
    public string this[Enum key]
    {
        get
        {
            return this[Util.EnumValueToString(key)];
        }
    }

    /// <summary>
    /// Gets a float from the data.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>A float from the specified key.</returns>
    public float GetFloat(string key)
    {
        return float.Parse(data[key]);
    }

    /// <summary>
    /// Gets a float from the data.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>A float from the specified key.</returns>
    public float GetFloat(Enum key)
    {
        return GetFloat(Util.EnumValueToString(key));
    }

    /// <summary>
    /// Gets a float or a default float value.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="defaultIfNotFound">The default if not found.</param>
    /// <returns>The value or the default if a value is not found.</returns>
    public float GetFloatOrDefault(string key, float defaultIfNotFound = default)
    {
        if (data.ContainsKey(key))
        {
            if (string.IsNullOrEmpty(data[key]) || !float.TryParse(data[key], out var _))
            {
                SetData(key, defaultIfNotFound);
                return defaultIfNotFound;
            }

            return GetFloat(key);
        }
        SetData(key, defaultIfNotFound);
        return defaultIfNotFound;
    }

    /// <summary>
    /// Gets a float or a default float value.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="defaultIfNotFound">The default if not found.</param>
    /// <returns>The value or the default if the key is not found.</returns>
    public float GetFloatOrDefault(Enum key, float defaultIfNotFound = default)
    {
        return GetFloatOrDefault(Util.EnumValueToString(key), defaultIfNotFound);
    }

    /// <summary>
    /// Gets an int from the data.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>An int from the specified key.</returns>
    public int GetInt(string key)
    {
        return int.Parse(data[key]);
    }

    /// <summary>
    /// Gets an int from the data.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>An int from the specified key.</returns>
    public int GetInt(Enum key)
    {
        return GetInt(Util.EnumValueToString(key));
    }

    /// <summary>
    /// Gets an int or a default int value.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="defaultIfNotFound">The default if not found.</param>
    /// <returns>The value or the default if the key is not found.</returns>
    public int GetIntOrDefault(string key, int defaultIfNotFound = default)
    {
        if (data.ContainsKey(key))
        {
            if (string.IsNullOrEmpty(data[key]))
            {
                SetData(key, defaultIfNotFound);
                return defaultIfNotFound;
            }
            if (!int.TryParse(data[key], out var _))
            {
                SetData(key, defaultIfNotFound);
                return defaultIfNotFound;
            }
            return GetInt(key);
        }
        SetData(key, defaultIfNotFound);
        return defaultIfNotFound;
    }

    /// <summary>
    /// Gets an int or a default int value.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="defaultIfNotFound">The default if not found.</param>
    /// <returns>The value or the default if the key is not found.</returns>
    public int GetIntOrDefault(Enum key, int defaultIfNotFound = default)
    {
        return GetIntOrDefault(Util.EnumValueToString(key), defaultIfNotFound);
    }

    /// <summary>
    /// Gets an ulong from the data.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>An ulong from the specified key.</returns>
    public ulong GetUlong(string key)
    {
        return ulong.Parse(data[key]);
    }

    /// <summary>
    /// Gets an ulong from the data.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>An ulong from the specified key.</returns>
    public ulong GetUlong(Enum key)
    {
        return GetUlong(Util.EnumValueToString(key));
    }

    /// <summary>
    /// Gets an ulongor a default int value.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="defaultIfNotFound">The default if not found.</param>
    /// <returns>The value or the default if the key is not found.</returns>
    public ulong GetUlongOrDefault(string key, ulong defaultIfNotFound = default)
    {
        if (data.ContainsKey(key))
        {
            if (string.IsNullOrEmpty(data[key]))
            {
                SetData(key, defaultIfNotFound);
                return defaultIfNotFound;
            }
            if (!ulong.TryParse(data[key], out var _))
            {
                SetData(key, defaultIfNotFound);
                return defaultIfNotFound;
            }
            return GetUlong(key);
        }
        SetData(key, defaultIfNotFound);
        return defaultIfNotFound;
    }

    /// <summary>
    /// Gets an ulong or a default int value.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="defaultIfNotFound">The default if not found.</param>
    /// <returns>The value or the default if the key is not found.</returns>
    public ulong GetUlongOrDefault(Enum key, ulong defaultIfNotFound = default)
    {
        return GetUlongOrDefault(Util.EnumValueToString(key), defaultIfNotFound);
    }

    /// <summary>
    /// Gets a string from the data.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>An int from the specified key.</returns>
    public string GetString(string key)
    {
        return this[key];
    }

    /// <summary>
    /// Gets a string from the data.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>An int from the specified key.</returns>
    public string GetString(Enum key)
    {
        return this[key];
    }

    /// <summary>
    /// Gets a string or a default string value.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="defaultIfNotFound">The default if not found.</param>
    /// <returns>The value or the default if the key is not found.</returns>
    public string GetStringOrDefault(string key, string defaultIfNotFound = default)
    {
        if (data.ContainsKey(key))
        {
            if (string.IsNullOrEmpty(data[key]))
            {
                SetData(key, defaultIfNotFound);
                return defaultIfNotFound;
            }
            return GetString(key);
        }
        SetData(key, defaultIfNotFound);
        return defaultIfNotFound;
    }

    /// <summary>
    /// Gets a string or a default string value.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="defaultIfNotFound">The default if not found.</param>
    /// <returns>The value or the default if the key is not found.</returns>
    public string GetStringOrDefault(Enum key, string defaultIfNotFound = default)
    {
        return GetStringOrDefault(Util.EnumValueToString(key), defaultIfNotFound);
    }

    /// <summary>
    /// Gets a bool from the data.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>An int from the specified key.</returns>
    public bool GetBool(string key)
    {
        return bool.Parse(data[key]);
    }

    /// <summary>
    /// Gets a bool from the data.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>An int from the specified key.</returns>
    public bool GetBool(Enum key)
    {
        return GetBool(Util.EnumValueToString(key));
    }

    /// <summary>
    /// Gets a bool or a default bool value.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="defaultIfNotFound">The default if not found.</param>
    /// <returns>The value or the default if the key is not found.</returns>
    public bool GetBoolOrDefault(string key, bool defaultIfNotFound = default)
    {
        if (data.ContainsKey(key))
        {
            if (string.IsNullOrEmpty(data[key]))
            {
                SetData(key, defaultIfNotFound);
                return defaultIfNotFound;
            }
            if (!bool.TryParse(data[key], out var _))
            {
                SetData(key, defaultIfNotFound);
                return defaultIfNotFound;
            }
            return GetBool(key);
        }
        SetData(key, defaultIfNotFound);
        return defaultIfNotFound;
    }

    /// <summary>
    /// Gets a bool or a default bool value.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="defaultIfNotFound">The default if not found.</param>
    /// <returns>The value or the default if the key is not found.</returns>
    public bool GetBoolOrDefault(Enum key, bool defaultIfNotFound = default)
    {
        return GetBoolOrDefault(Util.EnumValueToString(key), defaultIfNotFound);
    }

    /// <summary>
    /// Sets the data.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="obj">The object.</param>
    public void SetData(string key, object obj)
    {
        if (data.ContainsKey(key))
        {
            data[key] = obj.ToString();
        }
        else
        {
            data.Add(key, obj.ToString());
        }
    }

    /// <summary>
    /// Sets the data.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="obj">The object.</param>
    public void SetData(Enum key, object obj)
    {
        SetData(Util.EnumValueToString(key), obj);
    }

    [GeneratedRegex(":")]
    private static partial Regex MyRegex();
}
