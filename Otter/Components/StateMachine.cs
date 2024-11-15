using System;
using System.Collections.Generic;
using System.Reflection;

using Otter.Core;

namespace Otter.Components;

/// <summary>
/// State machine that uses a specific type.  This is really meant for using an enum as your list of states.
/// If an enum is used, the state machine will automatically populate the states using methods in the parent
/// Entity that match the name of the enum values.
/// </summary>
/// <example>
/// Say you have an enum named State, and it has the value "Walking"
/// When the state machine is added to the Entity, it will match any methods named:
/// EnterWalking
/// UpdateWalking
/// ExitWalking
/// And use those to build the states.  This saves a lot of boilerplate set up code.
/// </example>
/// <typeparam name="TState">An enum of states.</typeparam>
public class StateMachine<TState> : Component
{

    #region Private Fields

    private readonly Dictionary<TState, State> states = [];
    private readonly List<TState> stateStack = [];
    private readonly List<TState> pushQueue = [];
    private readonly List<bool> pushPopBuffer = []; // keep track of the order of commands
    private readonly List<float> timers = [];
    private readonly Dictionary<TState, Dictionary<TState, Action>> transitions = [];
    private bool firstChange = true;
    private bool populatedMethods;
    private TState changeTo;
    private bool change;
    private bool updating;
    private bool noState = true;

    private TState TopState
    {
        get
        {
            return stateStack.Count > 0 ? stateStack[0] : CurrentState;
        }
    }

    #endregion

    #region Private Properties

    private State S
    {
        get
        {
            return !states.TryGetValue(CurrentState, out State value) ? null : value;
        }
    }

    #endregion

    #region Public Fields

    /// <summary>
    /// Determines if the StateMachine will autopopulate its states based off of the values of the Enum.
    /// </summary>
    public bool AutoPopulate = true;

    #endregion

    #region Public Properties

    /// <summary>
    /// The current state.
    /// </summary>
    public TState CurrentState { get; private set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Create a new StateMachine.
    /// </summary>
    public StateMachine()
    {
    }

    #endregion

    #region Private Methods

    private void Transition(TState from, TState to)
    {
        if (transitions.ContainsKey(from))
        {
            if (transitions[from].TryGetValue(to, out Action inner))
            {
                inner();
            }
        }
    }

    private void EnsurePopulatedMethods()
    {
        if (AutoPopulate)
        { // Populate the methods on the state change.
            if (!populatedMethods)
            {
                populatedMethods = true;
                if (typeof(TState).IsEnum)
                {
                    foreach (TState value in Enum.GetValues(typeof(TState)))
                    {
                        AddState(value);
                    }
                }
            }
        }
    }

    private void PushStateImmediate(TState state)
    {
        EnsurePopulatedMethods();

        stateStack.Insert(0, state);
        timers.Insert(0, 0);
        var from = CurrentState;
        if (stateStack.Count > 1)
        {
            timers[1] = Timer;
        }
        Timer = 0;
        CurrentState = TopState;
        S.Enter();
        Transition(from, CurrentState);
        noState = false;
    }

    private void PopStateImmediate()
    {
        EnsurePopulatedMethods();

        if (stateStack.Count == 0)
        {
            return; // No states to pop!
        }

        stateStack.RemoveAt(0);
        timers.RemoveAt(0);
        S.Exit();
        var from = CurrentState;
        CurrentState = TopState;
        Transition(from, CurrentState);

        if (stateStack.Count > 0)
        {
            Timer = timers[0];
        }
        else
        {
            noState = true; // No more states... :(
            Timer = 0;
        }
    }

    private void ChangeState()
    {
        if (change)
        {
            var state = changeTo;
            change = false;

            EnsurePopulatedMethods();

            if (!firstChange)
            {
                if (states.ContainsKey(CurrentState))
                {
                    if (states[CurrentState] == states[state])
                    {
                        return; // No change actually happening so return
                    }
                }
            }

            Timer = 0;

            var fromState = CurrentState;

            if (states.ContainsKey(state))
            {
                if (S != null && !firstChange)
                {
                    S.Exit();
                }
                CurrentState = state;
                if (S == null)
                {
                    throw new NullReferenceException("Next state is null.");
                }

                S.Enter();
                noState = false;
                Transition(fromState, CurrentState);
            }

            if (firstChange)
            {
                firstChange = false;
            }
        }
        else
        {
            pushPopBuffer.ForEach(push =>
            {
                if (push)
                { // Push
                    var pushState = pushQueue[0];
                    pushQueue.RemoveAt(0);

                    PushStateImmediate(pushState);
                }
                else
                { // Pop
                    PopStateImmediate();
                }
            });
            pushPopBuffer.Clear();
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Finds methods that match the enum state in the Entity.  This happens in the Added() method of the
    /// component if AutoPopulate is set to true.
    /// </summary>
    /// <param name="e">The Entity to get methods from.</param>
    public void PopulateMethodsFromEntity(Entity e)
    {
        if (typeof(TState).IsEnum)
        {
            foreach (TState value in Enum.GetValues(typeof(TState)))
            {
                AddState(value, e);
            }
        }
    }

    /// <summary>
    /// Finds methods that match the enum state in the Entity.  This happens in the Added() method of the
    /// component if AutoPopulate is set to true.  If no Entity is specified, get the methods from the
    /// Entity that owns this component.
    /// </summary>
    public void PopulateMethodsFromEntity()
    {
        PopulateMethodsFromEntity(Entity);
    }

    /// <summary>
    /// Change the state.  Exit will be called on the current state followed by Enter on the new state.
    /// If the state machine is currently updating then the state change will not occur until after the
    /// update has completed.
    /// </summary>
    /// <param name="state">The state to change to.</param>
    public void ChangeState(TState state)
    {
        pushQueue.Clear();
        stateStack.Clear();

        changeTo = state;
        change = true;

        if (updating)
        {

        }
        else
        {
            ChangeState();
        }
    }

    /// <summary>
    /// Push a state onto a stack of states.  The state machine will always run the top of the stack.
    /// </summary>
    /// <param name="state">The state to push.</param>
    public void PushState(TState state)
    {
        if (updating)
        {
            pushQueue.Add(state);
            pushPopBuffer.Add(true); //true means push
        }
        else
        {
            PushStateImmediate(state);
        }
    }

    /// <summary>
    /// Pop the top state on the stack (if there is a stack.)
    /// </summary>
    public void PopState()
    {
        if (updating)
        {
            pushPopBuffer.Add(false); //false means pop
        }
        else
        {
            PopStateImmediate();
        }
    }

    /// <summary>
    /// Update the State Machine.
    /// </summary>
    public override void Update()
    {
        base.Update();
        if (states.ContainsKey(CurrentState) && !noState)
        {
            updating = true;
            S.Update();
            updating = false;

            ChangeState();
        }
    }

    /// <summary>
    /// Add a transition callback for when going from one state to another.
    /// </summary>
    /// <param name="fromState">The State that is ending.</param>
    /// <param name="toState">The State that is starting.</param>
    /// <param name="function">The Action to run when the machine goes from the fromState to the toState.</param>
    public void AddTransition(TState fromState, TState toState, Action function)
    {
        if (!transitions.TryGetValue(fromState, out Dictionary<TState, Action> value))
        {
            value = ([]);
            transitions.Add(fromState, value);
        }

        value.Add(toState, function);
    }

    /// <summary>
    /// Add a state with three Actions.
    /// </summary>
    /// <param name="key">The key to reference the State with.</param>
    /// <param name="onEnter">The method to call when entering this state.</param>
    /// <param name="onUpdate">The method to call when updating this state.</param>
    /// <param name="onExit">The method to call when exiting this state.</param>
    public void AddState(TState key, Action onEnter, Action onUpdate, Action onExit)
    {
        states.Add(key, new State(onEnter, onUpdate, onExit));
    }

    /// <summary>
    /// Add a state with just an update Action.
    /// </summary>
    /// <param name="key">The key to reference the State with.</param>
    /// <param name="onUpdate">The method to call when updating this state.</param>
    public void AddState(TState key, Action onUpdate)
    {
        states.Add(key, new State(onUpdate));
    }

    /// <summary>
    /// Add a state.
    /// </summary>
    /// <param name="key">The key to reference the State with.</param>
    /// <param name="value">The State to add.</param>
    public void AddState(TState key, State value)
    {
        states.Add(key, value);
    }

    /// <summary>
    /// Add a state using reflection to retrieve the approriate methods on the Entity.
    /// For example, a key with a value of "Idle" will retrieve the methods "EnterIdle" "UpdateIdle" and "ExitIdle" automatically.
    /// </summary>
    /// <param name="key">The key to reference the State with.</param>
    public void AddState(TState key, Entity e = null)
    {
        e ??= Entity; // Use the Component's Entity if no entity specified.

        if (states.ContainsKey(key))
        {
            return; // Dont add duplicate states.
        }

        var state = new State();
        var name = key.ToString();
        //Using reflection to find all the appropriate functions!
        MethodInfo mi;
        mi = e.GetType().GetMethod("Enter" + name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
        if (mi == null)
        {
            e.GetType().GetMethod("Enter" + name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        }

        if (mi != null)
        {
            state.OnEnter = (Action)Delegate.CreateDelegate(typeof(Action), e, mi);
        }

        mi = e.GetType().GetMethod("Update" + name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
        if (mi == null)
        {
            e.GetType().GetMethod("Update" + name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        }

        if (mi != null)
        {
            state.OnUpdate = (Action)Delegate.CreateDelegate(typeof(Action), e, mi);
        }

        mi = e.GetType().GetMethod("Exit" + name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
        if (mi == null)
        {
            e.GetType().GetMethod("Exit" + name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        }

        if (mi != null)
        {
            state.OnExit = (Action)Delegate.CreateDelegate(typeof(Action), e, mi);
        }

        states.Add(key, state);
    }

    #endregion

}
