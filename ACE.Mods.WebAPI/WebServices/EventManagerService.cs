namespace ACE.Mods.WebAPI.WebServices
{
    public class EventManagerService
    {
        [ResourceMethod()]
        public List<Event> Events()
        {
            var eventsToReturn = new List<Event>();

            var events = EventManager.Events;

            foreach (var evnt in events)
            {
                eventsToReturn.Add(evnt.Value);
            }

            return eventsToReturn;
        }

        [ResourceMethod(":eventName")]
        public Event? Events(string eventName)
        {
            var events = EventManager.Events;

            if (!events.TryGetValue(eventName, out Event evnt))
                return null;

            return evnt;
        }

        [ResourceMethod(":eventName/activations")]
        public Event? GetEvent(string eventName)
        {
            var events = EventManager.Events;

            if (!events.TryGetValue(eventName, out Event evnt))
                return null;

            return evnt;
        }

        [ResourceMethod(RequestMethod.Post, ":eventName/activations")]
        public bool Start([FromPath] string eventName)
        {
            var eventStarted = EventManager.StartEvent(eventName, null, null);

            return eventStarted;
        }

        [ResourceMethod(RequestMethod.Delete, ":eventName/activations")]
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
