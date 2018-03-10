using System;
using FeedScreen.Experiment.Missions.Broadcasts.Events;
using FeedScreen.Experiment.Missions.Broadcasts.Events.EventArgs;
using UnityEngine;
using UnityEngine.UI;

namespace Missions.Display.Queries
{
    public class ImageDisplay : MonoBehaviour
    {
        private RawImage _image;

        public Texture DefaultImage;

        // Use this for initialization
        private void Start()
        {
            _image = gameObject.GetComponent<RawImage>();

            // Clear Display.
            DisplayEventManager.OnClearDisplay();
        }

        private void OnEnable()
        {
            DisplayEventManager.DisplayImage += DisplayImage;
            DisplayEventManager.ClearDisplay += ClearDisplay;
        }

        private void OnDisable()
        {
            DisplayEventManager.DisplayImage -= DisplayImage;
            DisplayEventManager.ClearDisplay -= ClearDisplay;
        }

        private void DisplayImage(object source, TextureEventArgs e)
        {
            _image.texture = e.Texture;
        }

        private void ClearDisplay(object source, EventArgs e)
        {
            _image.texture = DefaultImage;
        }
    }
}