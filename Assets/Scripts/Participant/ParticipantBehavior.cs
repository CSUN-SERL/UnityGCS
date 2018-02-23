using System.Collections;
using Assets.Scripts.DataCollection.Physiological;
using FeedScreen.Experiment;
using Menu_Navigation.Button_Logic;
using Networking;
using Tobii.Plugins;
using UnityEngine;
using UnityEngine.Networking;

namespace Participant
{
    [RequireComponent(typeof(Recorder))]
    public class ParticipantBehavior : MonoBehaviour
    {
        public static ParticipantBehavior Instance;

        public static Participant Participant;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else if (Instance != this)
                Destroy(gameObject);
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void MakeNewParicipant(int group)
        {
            var participantData = new ParticipantData
            {
                Group = group,
                ProctorName = GroupSelection.InputField.text
            };
            StartCoroutine(NewParticipantEnumerator(participantData));

            Debug.Log(string.Format(
                "Attempting to make New Participant: Transparency={0} Adaptive={1} Proctor={2}",
                participantData.Transparent, participantData.Adaptive,
                GroupSelection.InputField.text));
        }

        private IEnumerator NewParticipantEnumerator(ParticipantData data)
        {
            var form = new WWWForm();
            form.AddField("adaptive", data.Adaptive ? "1" : "0");
            form.AddField("transparent", data.Transparent ? "1" : "0");
            form.AddField("group_number", data.Group);
            form.AddField("proctor_name", data.ProctorName);

            var www = UnityWebRequest.Post(ServerURL.INSERT_PARTICIPANT, form);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                SceneFlowController.LoadErrorScene();
            }
            else
            {
                var result = JSON.Parse(www.downloadHandler.text);

                if (result["failed"].AsBool)
                    SceneFlowController.LoadErrorScene();
                else
                    data.Id = result["data"].AsInt;


                Participant = new Participant
                {
                    Data = data,
                    CurrentSurvey = 1,
                    CurrentMission = 1
                };

                Debug.Log(string.Format(
                    "New Participant Made: Transparency={0} Adaptive={1} Proctor={2}",
                    Participant.Data.Transparent, Participant.Data.Adaptive,
                    Participant.Data.ProctorName));
            }
        }
    }
}