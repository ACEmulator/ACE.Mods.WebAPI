namespace ACE.Mods.WebAPI.WebServices
{
    public class EventManagerService
    {
        //[RequireRole("events")]
        //[ResourceMethod()]
        //public List<Event> Events()
        //{
        //    var eventsToReturn = new List<Event>();

        //    var events = EventManager.Events;

        //    foreach (var evnt in events)
        //    {
        //        eventsToReturn.Add(evnt.Value);
        //    }

        //    return eventsToReturn;
        //}

        [RequireRole("events")]
        [ResourceMethod("")]
        public List<Event> GetEvents(int? state)
        {
            var eventsToReturn = new List<Event>();

            var events = EventManager.Events;

            foreach (var evnt in events)
            {
                if (state is not null && evnt.Value.State != state)
                    continue;

                eventsToReturn.Add(evnt.Value);
            }

            return eventsToReturn;
        }

        [RequireRole("events")]
        [ResourceMethod(":eventName")]
        public Event? Events(string eventName)
        {
            var events = EventManager.Events;

            if (!events.TryGetValue(eventName, out Event? evnt))
                return null;

            return evnt;
        }

        //[RequireRole("events")]
        //[ResourceMethod(":eventName/activation")]
        //public Event? GetEvent(string eventName)
        //{
        //    var events = EventManager.Events;

        //    if (!events.TryGetValue(eventName, out Event evnt))
        //        return null;

        //    return evnt;
        //}

        [RequireRole("events")]
        [ResourceMethod(RequestMethod.Post, ":eventName/activation")]
        public bool Start([FromPath] string eventName)
        {
            var eventStarted = EventManager.StartEvent(eventName, null, null);

            return eventStarted;
        }

        [RequireRole("events")]
        [ResourceMethod(RequestMethod.Delete, ":eventName/activation")]
        public bool Stop([FromPath] string eventName)
        {
            var eventStopped = EventManager.StopEvent(eventName, null, null);

            return eventStopped;
        }

        //public class EventStatusDTO
        //{
        //    /// <summary>
        //    /// Unique Id of this Event
        //    /// </summary>
        //    public uint Id { get; set; }

        //    /// <summary>
        //    /// Unique Event of Quest
        //    /// </summary>
        //    public string Name { get; set; }

        //    /// <summary>
        //    /// Unixtime of Event Start
        //    /// </summary>
        //    public int StartTime { get; set; }

        //    /// <summary>
        //    /// Unixtime of Event End
        //    /// </summary>
        //    public int EndTime { get; set; }

        //    /// <summary>
        //    /// State of Event (GameEventState)
        //    /// </summary>
        //    public int State { get; set; }

        //    public DateTime LastModified { get; set; }
        //}
    }
}
