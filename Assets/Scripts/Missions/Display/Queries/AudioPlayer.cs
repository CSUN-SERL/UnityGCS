using System;
using FeedScreen.Experiment.Missions.Broadcasts.Events;
using UnityEngine;

namespace Missions.Display.Queries
{
    public class AudioPlayer : MonoBehaviour
    {
        public AudioSource Source;

        private void OnEnable()
        {
            DisplayEventManager.DisplayAudioClip += OnDisplayAudioClip;
            DisplayEventManager.ClearDisplay += OnClearDisplay;
        }

        private void OnDisable()
        {
            DisplayEventManager.DisplayAudioClip -= OnDisplayAudioClip;
            DisplayEventManager.ClearDisplay -= OnClearDisplay;
        }

        private void OnDisplayAudioClip(object sender, AudioEventArgs e)
        {
            if (Source == null) return;
            Source.clip = e.Clip;
            Source.Play();
            Debug.Log(Source.clip);
        }

        private void OnClearDisplay(object sender, EventArgs e)
        {
            Source.Stop();
        }
    }
}