using System;
using Missions;
using Missions.Endpoint;
using Missions.Lifecycle;
using Participant;
using UnityEngine;

namespace Tests
{
    public class MissionLifeCycleTest : MonoBehaviour
    {
        public bool Automated;

        public bool On;

        private void OnEnable()
        {
            SocketEventManager.SocketConnected += OnSocketConnected;
            Missions.Lifecycle.EventManager.MetaDataLoaded += OnMetaDataLoaded;
            Missions.Lifecycle.EventManager.Completed += OnCompleted;
            Missions.Lifecycle.EventManager.Stopped += OnStopped;

            if (!Automated || !On) return;
            SocketEventManager.OnConnectSocket();
        }

        private void OnDisable()
        {
            SocketEventManager.SocketConnected -= OnSocketConnected;
            Missions.Lifecycle.EventManager.MetaDataLoaded -= OnMetaDataLoaded;
            Missions.Lifecycle.EventManager.Completed -= OnCompleted;
            Missions.Lifecycle.EventManager.Stopped -= OnStopped;
        }

        private void OnSocketConnected(object sender, EventArgs e)
        {
            Missions.Lifecycle.EventManager.OnLoadMetaData();
        }

        private void OnMetaDataLoaded(object sender, MissionEventArgs e)
        {
            Missions.Lifecycle.EventManager.OnInitialize(ParticipantBehavior.Participant.CurrentMission);
        }

        private void OnCompleted(object sender, EventArgs e)
        {
            Missions.Lifecycle.EventManager.OnClose();
        }

        private void OnStopped(object sender, EventArgs e)
        {
            SocketEventManager.OnDisconnectSocket();
        }

        // Update is called once per frame
        private void Update()
        {
            if (!On) return;

            //Mission Ended
            if (Input.GetKeyDown("m"))
            {
                Debug.Log("connect-to-socket");
                SocketEventManager.OnConnectSocket();
            }

            //Initialize Mission
            if (Input.GetKeyDown("i"))
            {
                Debug.Log(MissionLifeCycleController.INITIALIZE_MISSION);
                Missions.Lifecycle.EventManager.OnInitialize(1);
            }

            //Mission Initialized
            if (Input.GetKeyDown("o"))
            {
                Debug.Log("gcs-mission-intitialized");
                Missions.Lifecycle.EventManager.OnInitialized();
            }

            //Start Mission
            if (Input.GetKeyDown("s"))
            {
                Debug.Log("gcs-start-mission");
                Missions.Lifecycle.EventManager.OnStart();
            }

            //Mission Started
            if (Input.GetKeyDown("d"))
            {
                Debug.Log("gcs-mission-started");
                Missions.Lifecycle.EventManager.OnStarted();
            }

            //Mission Completed
            if (Input.GetKeyDown("c"))
            {
                Debug.Log("gcs-mission-completed");
                Missions.Lifecycle.EventManager.OnCompleted();
            }

            //End Mission
            if (Input.GetKeyDown("e"))
            {
                Debug.Log("gcs-close-mission");
                Missions.Lifecycle.EventManager.OnClose();
            }

            //Mission Ended
            if (Input.GetKeyDown("t"))
            {
                Debug.Log("gcs-stop-mission");
                MissionLifeCycleController.StopMission();
            }

            //Mission Ended
            if (Input.GetKeyDown("r"))
            {
                Debug.Log("gcs-mission-stopped");
                Missions.Lifecycle.EventManager.OnStopped();
            }
        }
    }
}