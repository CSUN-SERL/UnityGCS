using FeedScreen.Experiment.Missions.Broadcasts.Events;
using UnityEngine;

namespace Missions.Queries.QueryTypes.Visual
{
    public class VisualQuery : ConfidenceQuery
    {
        public Texture Texture { get; set; }
        public string ImageName { get; set; }

        public override void Display()
        {
            base.Display();
            DisplayEventManager.OnDisplayImage(Texture);
        }

        public override string GetDisplayName()
        {
            return "Visual Query";
        }
    }
}