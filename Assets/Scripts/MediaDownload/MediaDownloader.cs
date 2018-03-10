using System.Collections;
using System.Collections.Generic;
using Missions.Broadcasts.Events.EventArgs;
using Missions.Endpoint;
using Missions.Queries.QueryTypes.Audio;
using Missions.Queries.QueryTypes.Visual;
using Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace MediaDownload
{
    public class MediaDownloader : MonoBehaviour
    {
        public static readonly Queue<DownloadMediaEventArgs> ExecuteOnMainThread
            = new Queue<DownloadMediaEventArgs>();

        private void OnEnable()
        {
            DownloadEventManager.ImageDownloaded += OnImageDownloaded;
            DownloadEventManager.AudioDownloaded += OnAudioDownloaded;
            DownloadEventManager.DownloadMedia += OnDownloadMedia;
        }

        private void OnDisable()
        {
            DownloadEventManager.ImageDownloaded -= OnImageDownloaded;
            DownloadEventManager.AudioDownloaded -= OnAudioDownloaded;
            DownloadEventManager.DownloadMedia -= OnDownloadMedia;
        }

        public virtual void Update()
        {
            // dispatch stuff on main thread
            while (ExecuteOnMainThread.Count > 0)
            {
                var media = ExecuteOnMainThread.Dequeue();
                switch (media.MediaType)
                {
                    case "texture":
                        StartCoroutine(RequestImage(media));
                        break;
                    case "audio":
                        StartCoroutine(RequestAudio(media));
                        break;
                }
            }
        }

        private void OnDownloadMedia(object sender, DownloadMediaEventArgs e)
        {
            ExecuteOnMainThread.Enqueue(e);
        }

        private void OnImageDownloaded(object sender, QueryEventArgs e)
        {
            SocketEventManager.OnQueryRecieved(e.Query);
        }

        private void OnAudioDownloaded(object sender, QueryEventArgs e)
        {
            var q = (AudioDetectionQuery) e.Query;
            SocketEventManager.OnQueryRecieved(e.Query);
        }

        public IEnumerator RequestImage(DownloadMediaEventArgs media)
        {
            Debug.Log("Requesting Image");

            if (media.fileName.Length <= 0 || media.fileName == null)
                yield break;
            var www =
                UnityWebRequestTexture.GetTexture(
                    ServerURL.DownloadMediaUrl(media.fileName));
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                yield break;
            }

            Debug.Log("Image Download Successful.");

            var query = (VisualDetectionQuery) media.query;

            System.Diagnostics.Debug.Assert(query != null, "query != null");

            var texture =
                ((DownloadHandlerTexture) www.downloadHandler).texture;

            if (texture != null)
            {
                query.Texture = ((DownloadHandlerTexture) www.downloadHandler)
                    .texture;
                DownloadEventManager.OnImageDownloaded(query);
            }
            else
            {
                Debug.Log("Could not download Media");
            }
        }

        public IEnumerator RequestAudio(DownloadMediaEventArgs media)
        {
            if (media.fileName.Length <= 0 || media.fileName == null)
                yield break;
            var www = UnityWebRequestMultimedia.GetAudioClip(
                ServerURL.DownloadMediaUrl(media.fileName),
                AudioType.OGGVORBIS);
            yield return www.SendWebRequest();

            //PCM
            //Make audio clip
            // Convert byte array to float array system.bitconverter
            // Audioclip.setData(bullshit)

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                yield break;
            }

            var query = media.query as AudioDetectionQuery;
            //var query = (AudioDetectionQuery)media.query;
            //System.Diagnostics.Debug.Assert(query != null, "query != null");

            var audioClip = ((DownloadHandlerAudioClip) www.downloadHandler)
                .audioClip;
            Debug.Log(audioClip.length);
            if (query != null)
            {
                query.Audio = audioClip;

                DownloadEventManager.OnAudioDownloaded(query);
            }
            else
            {
                Debug.Log("Problem");
            }
        }
    }
}