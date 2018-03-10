using Missions.Queries.QueryTypes.Visual;

namespace FeedScreen.Experiment.Missions.Broadcasts.Events.EventArgs
{
    public class TaggingQueryEventArgs : System.EventArgs
    {
        public TaggingQuery TaggingQuery { get; set; }
    }
}