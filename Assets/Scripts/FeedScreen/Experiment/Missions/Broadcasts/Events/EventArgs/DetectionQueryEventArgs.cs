using Missions.Queries.QueryTypes.Visual;

namespace FeedScreen.Experiment.Missions.Broadcasts.Events.EventArgs
{
    public class DetectionQueryEventArgs : System.EventArgs
    {
        public VisualDetectionQuery VisualDetectionQuery { get; set; }
    }
}