using System;
using FeedScreen.Experiment.Missions.Broadcasts.Events.EventArgs;
using Missions;
using Missions.Broadcasts.Events.EventArgs;
using Missions.Endpoint;
using Missions.Queries;
using UnityEngine;

namespace FeedScreen.Experiment.Missions.Broadcasts.Events
{
    public class DisplayEventManager : MonoBehaviour
    {
        public static event EventHandler<NotificationEventArgs>
            DisplayNotification;

        public static void OnDisplayNotification(Notification notification)
        {
            if (DisplayNotification == null) return;
            DisplayNotification(null,
                new NotificationEventArgs {Notification = notification});
            Debug.Log("Notification : " + notification);
        }

        public static event EventHandler<TextureEventArgs> DisplayImage;

        public static void OnDisplayImage(Texture texture)
        {
            if (DisplayImage == null) return;
            DisplayImage(null, new TextureEventArgs {Texture = texture});
            Debug.Log("Image: " + texture);
        }

        public static event EventHandler<AudioEventArgs> DisplayAudioClip;

        public static void OnDisplayAudioClip(AudioClip audioClip)
        {
            var handler = DisplayAudioClip;
            if (handler != null)
                handler(null, new AudioEventArgs {Clip = audioClip});
        }

        public static event EventHandler<StringEventArgs> DisplayConfidence;

        public static void OnDisplayConfidence(string confidence)
        {
            var handler = DisplayConfidence;
            if (handler != null)
                handler(null, new StringEventArgs {StringArgs = confidence});
        }

        public static event EventHandler<StringEventArgs> DisplayQuestion;

        public static void OnDisplayQuestion(string question)
        {
            var handler = DisplayQuestion;
            if (handler != null)
                handler(null, new StringEventArgs {StringArgs = question});
        }

        public static event EventHandler<QueryEventArgs> BoolQuestion;

        public static void OnBoolQuestion(Query query)
        {
            if (BoolQuestion == null) return;
            BoolQuestion(null, new QueryEventArgs {Query = query});
        }

        public static event EventHandler<QueryEventArgs> TagQuestion;

        public static void OnTagQuestion(Query query)
        {
            if (TagQuestion == null) return;
            TagQuestion(null, new QueryEventArgs {Query = query});
        }

        public static event EventHandler<System.EventArgs> DisplayBlackButton;

        public static void OnDisplayBlackButton()
        {
            var handler = DisplayBlackButton;
            if (handler != null) handler(null, System.EventArgs.Empty);
        }

        public static event EventHandler<System.EventArgs> DisplayRedButton;

        public static void OnDisplayRedButton()
        {
            var handler = DisplayRedButton;
            if (handler != null) handler(null, System.EventArgs.Empty);
        }

        public static event EventHandler<System.EventArgs> DisplayYellowButton;

        public static void OnDisplayYellowButton()
        {
            var handler = DisplayYellowButton;
            if (handler != null) handler(null, System.EventArgs.Empty);
        }

        public static event EventHandler<System.EventArgs> DisplayGreenButton;

        public static void OnDisplayGreenButton()
        {
            var handler = DisplayGreenButton;
            if (handler != null) handler(null, System.EventArgs.Empty);
        }

        public static event EventHandler<System.EventArgs> ClearDisplay;

        public static void OnClearDisplay()
        {
            if (ClearDisplay == null) return;
            ClearDisplay(null, System.EventArgs.Empty);
            //Debug.Log("Clear");
        }
    }
}