using System.Collections.Generic;

namespace Otter.Graphics.Drawables;

public class GraphicList : Graphic
{
    public List<Graphic> Graphics = [];

    public GraphicList(params Graphic[] graphics)
    {
        Graphics.AddRange(graphics);
    }

    public Graphic this[int index]
    {
        get => Graphics[index];
        set => Graphics[index] = value;
    }

    public T GetGraphic<T>(int index) where T : Graphic
    {
        return (T)this[index];
    }

    public T Add<T>(T graphic) where T : Graphic
    {
        Graphics.Add(graphic);
        return graphic;
    }

    public T Remove<T>(T graphic) where T : Graphic
    {
        Graphics.Remove(graphic);
        return graphic;
    }

    public Graphic RemoveAt(int index)
    {
        var g = Graphics[index];
        Graphics.RemoveAt(index);
        return g;
    }

    public void Clear()
    {
        Graphics.Clear();
    }

    public void AddRange(params Graphic[] graphics)
    {
        Graphics.AddRange(graphics);
    }

    public void AddRange(IEnumerable<Graphic> graphics)
    {
        Graphics.AddRange(graphics);
    }

    public int Count => Graphics.Count;

    public override void Update()
    {
        base.Update();

        foreach (var g in Graphics)
        {
            g.Update();
        }
    }

    public override void Render(float x = 0, float y = 0)
    {
        base.Render(x, y);

        if (!Visible)
        {
            return;
        }

        foreach (var g in Graphics)
        {
            g.Render(x + X, y + Y);
        }
    }

}
