using System;
using Missions.Broadcasts.Events.EventArgs;
using Missions.Queries;
using UnityEngine;

namespace MediaDownload
{
    public class DownloadEventManager : MonoBehaviour
    {
        public static event EventHandler<DownloadMediaEventArgs> DownloadMedia;

        public static void OnDownloadMedia(DownloadMediaEventArgs e)
        {
            Debug.Log("Media download event triggered.");
            var handler = DownloadMedia;
            if (handler != null)
                handler(null, e);
        }

        public static event EventHandler<QueryEventArgs> ImageDownloaded;

        public static void OnImageDownloaded(Query query)
        {
            Debug.Log("Image downloaded event triggered.");
            var handler = ImageDownloaded;
            if (handler != null)
                handler(null, new QueryEventArgs {Query = query});
        }

        public static event EventHandler<QueryEventArgs> AudioDownloaded;

        public static void OnAudioDownloaded(Query query)
        {
            var handler = AudioDownloaded;
            if (handler != null)
                handler(null, new QueryEventArgs {Query = query});
        }
    }
}