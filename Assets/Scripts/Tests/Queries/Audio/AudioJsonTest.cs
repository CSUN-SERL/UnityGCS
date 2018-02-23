using MediaDownload;
using Missions.Queries.QueryTypes.Visual;
using UnityEngine;

namespace Tests.Queries.Audio
{
    public class AudioJsonTest : MonoBehaviour
    {
        public bool On;

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown("space") && On)
            {
                var query = new VisualDetectionQuery
                {
                    ArrivalTime = 0,
                    Confidence = 0f,
                    QueryId = 0,
                    RobotId = 0,
                    UserId = 0
                };
                DownloadEventManager.OnDownloadMedia(
                    new DownloadMediaEventArgs
                    {
                        fileName = "test.ogg",
                        MediaType = "audio",
                        query = query
                    });
            }
        }
    }
}