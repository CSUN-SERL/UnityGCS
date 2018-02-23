using System;
using Networking;
using Participant;
using UnityEngine;

namespace Missions
{
    public class MissionLifeCycleController : MonoBehaviour
    {
        public const string INITIALIZE_MISSION = "gcs-initialize-mission";
        public const string MISSION_INITIALIZED = "gcs-mission-initialized";

        public const string START_MISSION = "gcs-start-mission";
        public const string MISSION_STARTED = "gcs-mission-started";

        public const string MISSION_COMPLETED = "gcs-mission-completed";

        public const string STOP_MISSION = "gcs-stop-mission";
        public const string MISSION_STOPPED = "gcs-mission-stopped";

        //public const string CLOSE_MISSION = "gcs-close-mission";
        //public const string MISSION_CLOSED = "gcs-mission-closed";

        public static MissionLifeCycleController Instance;
        public bool Initialized { get; private set; }

        public bool Started { get; private set; }
        public bool Completed { get; private set; }

        public bool Running
        {
            get { return Started && !Completed; }
        }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
                Destroy(this);

            InitializeMission();
        }

        private void OnEnable()
        {
            Missions.Lifecycle.EventManager.Initialized += OnInitialized;
            //EventManager.Completed += OnCompleted;
            Missions.Lifecycle.EventManager.Stopped += OnStopped;
        }

        private void OnDisable()
        {
            Missions.Lifecycle.EventManager.Initialized -= OnInitialized;
            //EventManager.Completed -= OnCompleted;
            Missions.Lifecycle.EventManager.Stopped -= OnStopped;
        }

        public static void InitializeMission()
        {
            if (GCSSocket.Instance.socket == null) return;

            var initializeMissionParameters = string.Format("mission{0}-{1}",
                ParticipantBehavior.Participant.CurrentMission,
                ParticipantBehavior.Participant.Data.Adaptive ? "true" : "false");

            Debug.Log(initializeMissionParameters);
            GCSSocket.Instance.socket.Emit(INITIALIZE_MISSION,
                initializeMissionParameters);
            Missions.Lifecycle.EventManager.OnInitialize(ParticipantBehavior.Participant.CurrentMission);
        }

        private void OnInitialized(object sender, EventArgs e)
        {
            Initialized = true;
        }

        public static void StartMission()
        {
            if (GCSSocket.Instance.socket == null) return;
            GCSSocket.Instance.socket.Emit(START_MISSION,
                ParticipantBehavior.Participant.CurrentMission);
            Lifecycle.EventManager.OnStarted();
        }

        public static void CompleteMission()
        {
            if (GCSSocket.Instance.socket == null) return;
            Missions.Lifecycle.EventManager.OnCompleted();
        }

        public static void StopMission()
        {
            if (GCSSocket.Instance.socket == null) return;
            GCSSocket.Instance.socket.Emit(STOP_MISSION,
                ParticipantBehavior.Participant.CurrentMission);
        }

        private void OnStopped(object sender, EventArgs e)
        {
            Debug.Log("Destroy");
            Destroy(this);
        }
    }
}