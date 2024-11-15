using System;

using Otter.Utility;

namespace Otter.Components;

public class Countdown(float max) : Component
{

    public float Max = max;
    public float Min;
    public float Decrement = -1;
    public float Value = max;

    public bool IsCompleted => Value <= 0;

    public float Completion => Util.Clamp((Max - Value) / Max, 0, 1);

    public Action OnTrigger = delegate { };
    public bool Triggered;

    public Countdown(float max, float value) : this(max)
    {
        Value = value;
    }

    public override void Update()
    {
        base.Update();
        Tick();
    }

    public void Tick()
    {
        Value += Decrement;
        if (IsCompleted)
        {
            Value = 0;
            if (!Triggered)
            {
                Triggered = true;
                OnTrigger();
            }
        }
    }

    public void Reset()
    {
        Value = Max;
        Triggered = false;
    }
}
