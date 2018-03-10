using MediaDownload;
using Missions.Queries.QueryTypes.Visual;
using UnityEngine;

namespace Tests.Queries.Detection
{
    public class DetectionJsonTest : MonoBehaviour
    {
        public bool On;
        public string QueryType;

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
                Debug.Log("Sending Test Query");
                DownloadEventManager.OnDownloadMedia(
                    new DownloadMediaEventArgs
                    {
                        fileName = "test.png",
                        MediaType = "texture",
                        query = query
                    });

                //EndpointDataReceiver.OnDataRecieved(QueryJsonFactory.MakeQueryJson(QueryType));
                //DownloadEventManager.OnDownloadMedia(query, data["file_path"], "texture");
            }
        }
    }
}