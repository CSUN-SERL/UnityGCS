using System;
using UnityEngine;

namespace FeedScreen.Experiment.Missions.Broadcasts.Events.EventArgs
{
    public class TextureEventArgs : System.EventArgs
    {
        public Guid Guid { get; set; }
        public Texture Texture { get; set; }
    }
}