using Missions.Queries.QueryTypes.Audio;
using Networking;
using UnityEngine;

namespace Tests.Queries
{
    public class QueryAnswerTest : MonoBehaviour
    {
        public bool On;

        private void Update()
        {
            if (Input.GetKeyDown("space") && On)
                EndpointDataSender.UploadQueryAnswer(new AudioDetectionQuery
                {
                    QueryId = 0,
                    Response = 1
                });
        }
    }
}