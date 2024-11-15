using System.Collections.Generic;

namespace Otter.Components.Events;

public class EventProcessor : Component
{
    /// <summary>
    /// The list of EventQueueEvents to execute.
    /// </summary>
    public List<EventProcessorEvent> Events = [];

    /// <summary>
    /// The current event that is being executed.
    /// </summary>
    public EventProcessorEvent CurrentEvent { get; protected set; }

    /// <summary>
    /// Determines if the events will be run.  Defaults to true.
    /// </summary>
    public bool RunEvents = true;

    /// <summary>
    /// True if the number of events in the queue is greater than zero.
    /// </summary>
    public bool HasEvents => Events.Count > 0;

    protected bool isFreshEvent = true;
}
