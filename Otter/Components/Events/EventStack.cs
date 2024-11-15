namespace Otter.Components.Events;

public class EventStack : EventProcessor
{

    public EventProcessorEvent Push(EventProcessorEvent evt)
    {
        Events.Insert(0, evt);
        NextEvent();
        return evt;
    }

    public override void Update()
    {
        base.Update();

        if (RunEvents)
        {
            if (CurrentEvent == null)
            {
                NextEvent();
            }

            while (CurrentEvent != null)
            {
                if (isFreshEvent)
                {
                    isFreshEvent = false;
                    CurrentEvent.EventProcessor = this;
                    CurrentEvent.Start();
                    CurrentEvent.Begin();
                }

                CurrentEvent.Update();
                CurrentEvent.Timer += Entity.Game.DeltaTime;

                if (CurrentEvent.IsFinished)
                {
                    isFreshEvent = true;
                    CurrentEvent.End();
                    CurrentEvent.EventProcessor = null;
                    Events.Remove(CurrentEvent);
                    NextEvent();
                }
                else
                {
                    break;
                }
            }
        }
    }

    private void NextEvent()
    {
        CurrentEvent = HasEvents ? Events[0] : null;
        isFreshEvent = true;
    }
}
