using System.IO;
using System;
using System.Collections.Generic;

using Otter.Utility;
using Otter.Utility.MonoGame;

namespace Otter.Graphics;

/// <summary>
/// Class representing a shader written in GLSL.
/// Warning: Visual Studio encoding must be set to Western European (Windows) Codepage 1252 when editing shaders!
/// More details here: http://blog.pixelingene.com/2008/07/file-encodings-matter-when-writing-pixel-shaders/
/// </summary>
public class Shader : IDisposable
{
    #region Static Methods

    /// <summary>
    /// Load both the vertex and fragment shaders from source codes in memory
    ///
    /// This function can load both the vertex and the fragment
    /// shaders, or only one of them: pass NULL if you don't want to load
    /// either the vertex shader or the fragment shader.
    /// The sources must be valid shaders in GLSL language. GLSL is
    /// a C-like language dedicated to OpenGL shaders; you'll
    /// probably need to read a good documentation for it before
    /// writing your own shaders.
    /// </summary>
    /// <param name="vertexShader">String containing the source code of the vertex shader</param>
    /// <param name="fragmentShader">String containing the source code of the fragment shader</param>
    /// <returns>New shader instance</returns>
    public static SFML.Graphics.Shader FromString(string vertexShader, string fragmentShader)
    {
        SFML.Graphics.Shader shader = SFML.Graphics.Shader.FromString(vertexShader, null, fragmentShader);

        return shader;
    }

    /// <summary>
    /// Creates a Shader using source code in memory
    /// </summary>
    /// <param name="shaderType">Type of Shader</param>
    /// <param name="shader">GLSL code in memory</param>
    /// <returns>New Shader</returns>
    public static Shader FromString(ShaderType shaderType, string shader)
    {
        return (shaderType == ShaderType.Fragment)
            ? new Shader(FromString(null, shader))
            : new Shader(FromString(shader, null));
    }

    /// <summary>
    /// Store a shader parameter name by an Enum value.  After storing a parameter this way
    /// you can use SetParameter on shader instances with the Enum value and it will retrieve
    /// the parameter name string.
    /// </summary>
    /// <example>
    /// If your shader has a parameter named "overlayColor" you can do this:
    /// Shader.SetParameter(ShaderParams.OverlayColor, "overlayColor");
    /// And then on a shader instance you can do this:
    /// someImageWithAShader.Shader.SetParameter(ShaderParams.OverlayColor, Color.Red);
    /// </example>
    /// <param name="name">The Enum value to use as the key for the shader parameter name.</param>
    /// <param name="nameInShader">The name of the parameter in the shader code.</param>
    public static void AddParameter(Enum name, string nameInShader)
    {
        parameters.Add(Util.EnumValueToString(name), nameInShader);
    }

    /// <summary>
    /// Get the parameter string stored with the Enum key.
    /// </summary>
    /// <param name="name">The Enum name that is the key for the string parameter.</param>
    /// <returns>The string parameter.</returns>
    public static string Parameter(Enum name)
    {
        return parameters[Util.EnumValueToString(name)];
    }
    #endregion

    #region Static Fields

    private static readonly Dictionary<string, string> parameters = [];

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a Shader using a file as the source for the vertex and fragment shader.
    /// </summary>
    /// <param name="vertexFile">The file path to the vertex shader.</param>
    /// <param name="fragmentFile">The file path to the fragment shader.</param>
    public Shader(string vertexFile, string fragmentFile)
    {
        SFMLShader = new SFML.Graphics.Shader(Files.LoadFileStream(vertexFile), null, Files.LoadFileStream(fragmentFile));
    }

    /// <summary>
    /// Create a Shader using a stream as the source of the vertex and fragment shader.
    /// </summary>
    /// <param name="vertexStream">The stream for the vertex shader.</param>
    /// <param name="fragmentStream">The stream for the fragment shader.</param>
    public Shader(Stream vertexStream, Stream fragmentStream)
    {
        SFMLShader = new SFML.Graphics.Shader(vertexStream, null, fragmentStream);
    }

    /// <summary>
    /// Create a shader using a stream as the source and a ShaderType parameter.
    /// </summary>
    /// <param name="shaderType">The shader type (fragment or vertex)</param>
    /// <param name="source">The stream for the shader.</param>
    public Shader(ShaderType shaderType, Stream source)
    {
        SFMLShader = shaderType == ShaderType.Vertex ? new SFML.Graphics.Shader(source, null, null) : new SFML.Graphics.Shader(null, null, source);
    }

    /// <summary>
    /// Creates a Shader using a file path source, and auto detects which type of shader
    /// it is.  If the file path contains ".frag" or ".fs" it is assumed to be a fragment shader.
    /// </summary>
    /// <param name="source">The file path.</param>
    public Shader(string source)
    {
        var str = System.Text.Encoding.Default.GetString(Files.LoadFileBytes(source));
        SFMLShader = source.Contains(".frag") || source.Contains(".fs")
            ? SFML.Graphics.Shader.FromString(null, null, str)
            : new SFML.Graphics.Shader(str, null, null);
    }

    /// <summary>
    /// Creates a shader using a copy of another shader.
    /// </summary>
    /// <param name="copy">The shader to copy.</param>
    public Shader(Shader copy) : this(copy.SFMLShader) { }

    /// <summary>
    /// Creates a shader using a file path and a ShaderType parameter.
    /// </summary>
    /// <param name="shaderType">The shader type (fragment or vertex)</param>
    /// <param name="source">The file path.</param>
    public Shader(ShaderType shaderType, string source)
    {
        SFMLShader = shaderType == ShaderType.Vertex ? new SFML.Graphics.Shader(source, null, null) : new SFML.Graphics.Shader(null, null, source);
    }

    #endregion

    #region Public Properties

    public bool IsDisposed { get; private set; }

    #endregion

    #region Public Methods



    /// <summary>
    /// Set a parameter on the shader.
    /// </summary>
    /// <param name="name">The parameter in the shader to set.</param>
    /// <param name="x">The value to set it to.</param>
    public void SetParameter(string name, float x)
    {
        SFMLShader.SetUniform(name, x);
    }

    /// <summary>
    /// Set a parameter on the shader.
    /// </summary>
    /// <param name="name">The parameter in the shader to set.</param>
    /// <param name="x">The value to set it to.</param>
    public void SetParameter(Enum name, float x)
    {
        SetParameter(Parameter(name), x);
    }

    /// <summary>
    /// Set a parameter on the shader.
    /// </summary>
    /// <param name="name">The parameter in the shader to set.</param>
    /// <param name="x">The first value of a vec2.</param>
    /// <param name="y">The first value of a vec2.</param>
    public void SetParameter(string name, float x, float y)
    {
        SFMLShader.SetUniformArray(name, [x, y]);
    }

    /// <summary>
    /// Set a parameter on the shader.
    /// </summary>
    /// <param name="name">The parameter in the shader to set.</param>
    /// <param name="x">The first value of a vec2.</param>
    /// <param name="y">The first value of a vec2.</param>
    public void SetParameter(Enum name, float x, float y)
    {
        SetParameter(Parameter(name), x, y);
    }

    /// <summary>
    /// Set a parameter on the shader.
    /// </summary>
    /// <param name="name">The parameter in the shader to set.</param>
    /// <param name="xy">A Vector2 to set.</param>
    public void SetParameter(string name, Vector2 xy)
    {
        SFMLShader.SetUniformArray(name, [xy.X, xy.Y]);
    }

    /// <summary>
    /// Set a parameter on the shader.
    /// </summary>
    /// <param name="name">The parameter in the shader to set.</param>
    /// <param name="xy">A Vector2 to set.</param>
    public void SetParameter(Enum name, Vector2 xy)
    {
        SetParameter(Parameter(name), xy.X, xy.Y);
    }

    /// <summary>
    /// Set a parameter on the shader.
    /// </summary>
    /// <param name="name">The parameter in the shader to set.</param>
    /// <param name="xyz">A Vector3 to set.</param>
    public void SetParameter(string name, Vector3 xyz)
    {
        SFMLShader.SetUniformArray(name, [xyz.X, xyz.Y, xyz.Z]);
    }

    /// <summary>
    /// Set a parameter on the shader.
    /// </summary>
    /// <param name="name">The parameter in the shader to set.</param>
    /// <param name="xyz">A Vector3 to set.</param>
    public void SetParameter(Enum name, Vector3 xyz)
    {
        SetParameter(Parameter(name), xyz.X, xyz.Y, xyz.Z);
    }

    /// <summary>
    /// Set a parameter on the shader.
    /// </summary>
    /// <param name="name">The parameter in the shader to set.</param>
    /// <param name="xyzw">A Vector4 to set.</param>
    public void SetParameter(string name, Vector4 xyzw)
    {
        SFMLShader.SetUniformArray(name, [xyzw.X, xyzw.Y, xyzw.Z, xyzw.W]);
    }

    /// <summary>
    /// Set a parameter on the shader.
    /// </summary>
    /// <param name="name">The parameter in the shader to set.</param>
    /// <param name="xyzw">A Vector4 to set.</param>
    public void SetParameter(Enum name, Vector4 xyzw)
    {
        SetParameter(Parameter(name), xyzw.X, xyzw.Y, xyzw.Z, xyzw.W);
    }

    /// <summary>
    /// Set a parameter on the shader.
    /// </summary>
    /// <param name="name">The parameter in the shader to set.</param>
    /// <param name="x">The first value of a vec3.</param>
    /// <param name="y">The second value of a vec3.</param>
    /// <param name="z">The third value of a vec3.</param>
    public void SetParameter(string name, float x, float y, float z)
    {
        SFMLShader.SetUniformArray(name, [x, y, z]);
    }

    /// <summary>
    /// Set a parameter on the shader.
    /// </summary>
    /// <param name="name">The parameter in the shader to set.</param>
    /// <param name="x">The first value of a vec3.</param>
    /// <param name="y">The second value of a vec3.</param>
    /// <param name="z">The third value of a vec3.</param>
    public void SetParameter(Enum name, float x, float y, float z)
    {
        SetParameter(Parameter(name), x, y, z);
    }

    /// <summary>
    /// Set a parameter on the shader.
    /// </summary>
    /// <param name="name">The parameter in the shader to set.</param>
    /// <param name="x">The first value of a vec4.</param>
    /// <param name="y">The second value of a vec4.</param>
    /// <param name="z">The third value of a vec4.</param>
    /// <param name="w">The fourth value of a vec4.</param>
    public void SetParameter(string name, float x, float y, float z, float w)
    {
        SFMLShader.SetUniformArray(name, [x, y, z, w]);
    }

    /// <summary>
    /// Set a parameter on the shader.
    /// </summary>
    /// <param name="name">The parameter in the shader to set.</param>
    /// <param name="x">The first value of a vec4.</param>
    /// <param name="y">The second value of a vec4.</param>
    /// <param name="z">The third value of a vec4.</param>
    /// <param name="w">The fourth value of a vec4.</param>
    public void SetParameter(Enum name, float x, float y, float z, float w)
    {
        SetParameter(Parameter(name), x, y, z, w);
    }

    /// <summary>
    /// Set a parameter on the shader.
    /// </summary>
    /// <param name="name">The parameter in the shader to set.</param>
    /// <param name="texture">The texture to set it to.</param>
    public void SetParameter(string name, Texture texture)
    {
        SFMLShader.SetUniform(name, texture.SFMLTexture);
    }

    /// <summary>
    /// Set a parameter on the shader.
    /// </summary>
    /// <param name="name">The parameter in the shader to set.</param>
    /// <param name="texture">The texture to set it to.</param>
    public void SetParameter(Enum name, Texture texture)
    {
        SetParameter(Parameter(name), texture);
    }

    /// <summary>
    /// Set a parameter on the shader.
    /// </summary>
    /// <param name="name">The parameter in the shader to set.</param>
    /// <param name="textureSource">The path to an image to load as a texture.</param>
    public void SetParameter(string name, string textureSource)
    {
        SFMLShader.SetUniform(name, new Texture(textureSource).SFMLTexture);
    }

    /// <summary>
    /// Set a parameter on the shader.
    /// </summary>
    /// <param name="name">The parameter in the shader to set.</param>
    /// <param name="textureSource">The path to an image to load as a texture.</param>
    public void SetParameter(Enum name, string textureSource)
    {
        SetParameter(Parameter(name), textureSource);
    }

  

    /// <summary>
    /// Disposes the shader to probably clear up memory.
    /// </summary>
    public void Dispose()
    {
        if (SFMLShader == null)
        {
            return;
        }

        if (!IsDisposed)
        {
            SFMLShader.Dispose();
            IsDisposed = true;
        }
        GC.SuppressFinalize(this);
    }

    #endregion

    #region Internal

    internal SFML.Graphics.Shader SFMLShader;

    internal Shader(SFML.Graphics.Shader shader)
    {
        SFMLShader = shader;
    }

    /// <summary>
    /// For when the garbage collector destroys shaders, also
    /// free up the memory on the video card that the shader is using.
    /// </summary>
    ~Shader()
    {
        Dispose();
    }

    #endregion
}
