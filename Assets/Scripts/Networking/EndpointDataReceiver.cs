using FeedScreen.Experiment.Missions.Broadcasts.Events;
using MediaDownload;
using Missions;
using Missions.Endpoint;
using Missions.Queries;
using Missions.Queries.QueryTypes.Audio;
using Missions.Queries.QueryTypes.Visual;
using Participant;
using Tobii.Plugins;
using UnityEngine;

namespace Networking
{
    public class EndpointDataReceiver : MonoBehaviour
    {
        public static EndpointDataReceiver Instance;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
                Destroy(gameObject);
        }

        private void OnEnable()
        {
            SocketEventManager.DataRecieved += OnDataRecieved;
        }

        private void OnDisable()
        {
            SocketEventManager.DataRecieved -= OnDataRecieved;
        }

        private void OnDataRecieved(object sender, StringEventArgs e)
        {
            OnDataRecieved(e.StringArgs);
        }

        public static void OnDataRecieved(string queryJson)
        {
            Debug.Log(queryJson);
            var json = JSON.Parse(queryJson);

            var data = json["data"];


            Query query;
            switch (json["type"])
            {
                case VisualDetectionQuery.QueryType:
                    query = new VisualDetectionQuery
                    {
                        ArrivalTime = MissionTimer.CurrentTime,
                        Confidence = data["confidence"].AsFloat,
                        QueryId = data["query_id"].AsInt,
                        RobotId = data["robot_id"].AsInt,
                        UserId = ParticipantBehavior.Participant.CurrentMission
                    };
                    DownloadEventManager.OnDownloadMedia(
                        new DownloadMediaEventArgs
                        {
                            fileName = data["file_path"],
                            MediaType = "texture",
                            query = query
                        });
                    break;

                case AudioDetectionQuery.QueryType:
                    query = new AudioDetectionQuery
                    {
                        ArrivalTime = MissionTimer.CurrentTime,
                        Confidence = data["confidence"].AsFloat,
                        QueryId = data["query_id"].AsInt,
                        RobotId = data["robot_id"].AsInt,
                        UserId = ParticipantBehavior.Participant.CurrentMission
                    };
                    DownloadEventManager.OnDownloadMedia(
                        new DownloadMediaEventArgs
                        {
                            fileName = data["file_path"],
                            MediaType = "audio",
                            query = query
                        });
                    break;
                default:
                    Debug.Log("Query Type Not Recognized.");
                    break;
            }
        }
    }
}