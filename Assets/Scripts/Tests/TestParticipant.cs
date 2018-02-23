using Participant;
using UnityEngine;

namespace Tests
{
    public class TestParticipant : MonoBehaviour
    {
        public int GroupNumber;
        public int Id;

        public int MissionNumber;

        public bool On;
        public string ProctorName;

        // Use this for initialization
        private void Awake()
        {
            if (!On) return;
            ParticipantBehavior.Participant.CurrentMission = MissionNumber;
            ParticipantBehavior.Participant.Data = new ParticipantData
            {
                Id = Id,
                ProctorName = ProctorName,
                Group = GroupNumber
            };
        }
    }
}