using System.Collections;
using Missions.Lifecycle;
using Networking;
using Participant;
using Tobii.Plugins;
using UnityEngine;
using UnityEngine.Networking;

namespace Missions
{
    /// <summary>
    ///     Responsible for loading and keeping track of the mission metadata.
    /// </summary>
    public class MissionMetaData : MonoBehaviour
    {
        public static MissionMetaData MetaDataInstance;

        public static int MissionNumber { get; set; }
        public static float MissionLength { get; set; }
        public static string MissionBrief { get; set; }

        private void Awake()
        {
            if (MetaDataInstance == null)
                MetaDataInstance = this;
            else if (MetaDataInstance != this)
                Destroy(this);
        }

        private void Start()
        {
            Load();
        }

        private void OnEnable()
        {
            Missions.Lifecycle.EventManager.MetaDataLoaded += OnMetaDataLoaded;
        }

        private void OnDisable()
        {
            Missions.Lifecycle.EventManager.MetaDataLoaded -= OnMetaDataLoaded;
        }

        public static void Load()
        {
            MetaDataInstance.StartCoroutine(MetaDataInstance.LoadMetaData());
        }

        private void OnMetaDataLoaded(object sender, MissionEventArgs e)
        {
            MissionLength = e.MissionLength;
            MissionBrief = e.MissionBrief;
            MissionNumber = e.MissionNumber;
        }

        private IEnumerator LoadMetaData()
        {
            var form = new WWWForm();
            form.AddField("mission_number",
                ParticipantBehavior.Participant.CurrentMission);
            using (var www = UnityWebRequest.Post(ServerURL.LOAD_MISSION, form))
            {
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    var result = JSON.Parse(www.downloadHandler.text);
                    if (result["failed"].AsBool)
                        Debug.Log("Could not load Mission Metadata.");

                    var data = result["data"][0];

                    var missionId = data["mission_id"].AsInt;
                    var missionLength = data["mission_length"].AsFloat;
                    var missionBrief = data["mission_brief"];

                    Missions.Lifecycle.EventManager.OnMetaDataLoaded(missionId, missionLength,
                        missionBrief);
                }
            }
        }
    }
}