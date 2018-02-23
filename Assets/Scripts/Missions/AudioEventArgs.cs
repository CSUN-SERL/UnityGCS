using System;
using UnityEngine;

namespace Missions
{
    public class AudioEventArgs : EventArgs
    {
        public AudioClip Clip { get; set; }
    }
}