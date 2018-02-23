using Networking;
using UnityEngine;

namespace Tests
{
    public class JsonStressTest : MonoBehaviour
    {
        public bool On;

        // Use this for initialization
        private void Start()
        {
        }

        private void Update()
        {
            if (Input.GetKeyDown("space") && On)
            {
                var p = QueryJsonFactory.MakeQueryJson("visual-detection");

                GCSSocket.Instance.socket.Emit("gcs-query-received", p);
            }

            ;
        }
    }
}