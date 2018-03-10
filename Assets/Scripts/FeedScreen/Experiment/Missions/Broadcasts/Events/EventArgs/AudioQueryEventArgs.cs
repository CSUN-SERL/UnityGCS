using Missions.Queries;

namespace FeedScreen.Experiment.Missions.Broadcasts.Events.EventArgs
{
    public class AudioQueryEventArgs : System.EventArgs
    {
        public Query AudQuery { get; set; }
        public string filePath { get; set; }
    }
}