using Missions.Queries;

namespace Missions.Broadcasts.Events.EventArgs
{
    public class QueryEventArgs : System.EventArgs
    {
        public Query Query { get; set; }
    }
}