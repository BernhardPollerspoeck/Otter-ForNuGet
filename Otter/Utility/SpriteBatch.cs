using SFML.Graphics;

namespace Otter.Utility;

/// <summary>
/// NOT YET SUPPORTED
/// Very WIP class, probably internal for Otter, don't use it yet.
/// </summary>
internal class SpriteBatch
{
    private RenderStates renderStates = RenderStates.Default;

    public int MaxSprites = 100;

    public int DrawCount { get; private set; }

    private readonly VertexArray vertexArray = new(PrimitiveType.Quads);
    private bool drawing;
    public SpriteBatch()
    {
    }

    public void Begin()
    {
        if (drawing == false)
        {
            drawing = true;
            vertexArray.Clear();
            DrawCount = 0;
        }
    }

    public void Draw(VertexArray vertices, RenderStates states)
    {
        if (!drawing)
        {
            return;
        }

        // Render states have changed, so flush and clear.
        if (!CompareStates(renderStates, states))
        {
            Flush();
            vertexArray.Clear();
            renderStates = states;
        }

        // Exceeding maximum sprites, so flush and clear.
        if (DrawCount >= MaxSprites)
        {
            Flush();
            DrawCount = 0;
            vertexArray.Clear();
        }

        // Apply the transform to the four points
        var transform = states.Transform;

        // Append the vertices
        for (uint i = 0; i < vertices.VertexCount; i++)
        {
            var vertex = vertices[i];
            var pos = transform.TransformPoint(vertex.Position.X, vertex.Position.Y);
            vertex.Position.X = pos.X;
            vertex.Position.Y = pos.Y;
            vertexArray.Append(vertex);
        }

        DrawCount++;
        // I think that's it?
    }

    private static bool CompareStates(RenderStates a, RenderStates b)
    {
        if (a.Texture != b.Texture)
        {
            return false;
        }

        if (a.Shader != b.Shader)
        {
            return false;
        }

        if (a.BlendMode != b.BlendMode)
        {
            return false;
        }
        // Dont care about transform right now.

        return true;
    }

    public void SetRenderState(RenderStates states)
    {
        renderStates = states;
    }

    public void End()
    {
        if (drawing)
        {
            drawing = false;
            Flush();
            vertexArray.Clear();
            //Console.WriteLine("Spritebatch took {0}ms", stopwatch.ElapsedMilliseconds);
        }
    }

    private void Flush()
    {
        if (vertexArray.VertexCount > 0)
        {
            renderStates.Transform = Transform.Identity;
            Utility.Draw.Spritebatch(vertexArray, renderStates);
        }
    }

}
