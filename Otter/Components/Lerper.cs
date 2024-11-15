using System;

using Otter.Utility;

namespace Otter.Components;

/// <summary>
/// Component that will slowly interpolate a value toward a target using a speed and acceleration.
/// This component can move the value and does not know about time at all.
/// </summary>
/// <remarks>
/// Create a new Lerper.
/// </remarks>
/// <param name="value">The initial value.</param>
/// <param name="accel">The acceleration for moving toward the target.</param>
/// <param name="maxSpeed">The max speed for moving toward the target.</param>
public class Lerper(float value, float accel, float maxSpeed) : Component
{

    #region Private Fields

    private float targetSpeed, speed;
    private bool endPhase = false;

    #endregion

    #region Public Fields

    /// <summary>
    /// The acceleration for moving toward the target.
    /// </summary>
    public float Acceleration = accel;

    /// <summary>
    /// The maximum speed for moving toward the target.
    /// </summary>
    public float MaxSpeed = maxSpeed;

    #endregion

    #region Public Properties

    /// <summary>
    /// If the Lerper has completed its movement.
    /// </summary>
    public bool Completed { get; private set; } = false;

    /// <summary>
    /// The current target to move toward.
    /// </summary>
    public float Target { get; private set; }

    /// <summary>
    /// The current value.
    /// </summary>
    public float Value { get; private set; } = value;

    #endregion
    #region Public Methods

    /// <summary>
    /// Set the target.
    /// </summary>
    /// <param name="value"></param>
    public void SetTarget(float value)
    {
        if (Target == value)
        {
            return;
        }

        Target = value;

        endPhase = false;
        Completed = false;
    }

    /// <summary>
    /// Force the current value.
    /// </summary>
    /// <param name="value"></param>
    public void SetValue(float value)
    {
        Value = value;
    }

    /// <summary>
    /// Update the Lerper.
    /// </summary>
    public override void Update()
    {
        base.Update();

        if (Completed)
        {
            Value = Target;
            return;
        }

        var currentDistance = Math.Abs(Target - Value);
        var stoppingDistance = MaxSpeed * MaxSpeed / (2 * Acceleration);
        var earlyStopDistance = speed * speed / (2 * Acceleration);

        if (!endPhase)
        {
            targetSpeed = MaxSpeed * Math.Sign(Target - Value);

            if (currentDistance <= stoppingDistance)
            {
                if (speed == MaxSpeed)
                {
                    targetSpeed = 0;
                    endPhase = true;
                }
                if (speed < MaxSpeed)
                {
                    if (currentDistance <= earlyStopDistance)
                    {
                        targetSpeed = 0;
                        endPhase = true;
                    }
                }
            }
        }

        speed = Util.Approach(speed, targetSpeed, Acceleration);

        if (endPhase)
        {
            if (Target > Value)
            {
                if (speed < Acceleration)
                {
                    speed = Acceleration;
                }

                Value = Util.Approach(Value, Target, speed);
            }
            if (Target < Value)
            {
                if (speed > -Acceleration)
                {
                    speed = -Acceleration;
                }

                Value = Util.Approach(Value, Target, -speed);
            }
        }
        else
        {
            Value += speed;
        }

        if (currentDistance <= Acceleration * 5)
        {
            speed = 0;
            Completed = true;
        }
    }

    #endregion

    #region Operators

    public static implicit operator float(Lerper lerper)
    {
        return lerper.Value;
    }
    public static implicit operator int(Lerper lerper)
    {
        return (int)lerper.Value;
    }

    #endregion

}
