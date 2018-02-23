using Missions.Queries.QueryTypes.Visual;

namespace FeedScreen.Experiment.Missions.Broadcasts.Events.EventArgs
{
    public class DoubleDetectionQueryEventArgs : System.EventArgs
    {
        public DoubleDetectionQuery DoubleDetectionQuery { get; set; }
    }
}