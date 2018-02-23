using Missions.Queries;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Networking
{
    public class EndpointDataSender : MonoBehaviour
    {

        public static void UploadQueryAnswer(Query query)
        {
            var jObject =
                new JObject(
                    new JProperty("query_id", query.QueryId),
                    new JProperty("response", query.Response)
                );
            Debug.Log(jObject);
            GCSSocket.Instance.socket.Emit(ServerURL.SEND_ANSWER_QUERY,
                jObject);
        }
    }
}