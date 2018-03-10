using FeedScreen.Experiment.Missions.Broadcasts.Events;
using Missions.Queries.QueryTypes.Visual;
using UnityEngine;

namespace Missions.Queries.QueryTypes.Audio
{
    public class AudioDetectionQuery : VisualQuery
    {
        public const string QueryType = "audio-detection";

        public AudioClip Audio { get; set; }

        public override void Display()
        {
            base.Display();
            DisplayEventManager.OnDisplayAudioClip(Audio);
            DisplayEventManager.OnDisplayImage(Texture);
            DisplayEventManager.OnDisplayQuestion("Can you hear a human?");
            DisplayEventManager.OnBoolQuestion(this);
        }

        public override string GetDisplayName()
        {
            return "Audio Detection";
        }
    }
}