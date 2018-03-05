using System;
using FeedScreen.Experiment.Missions.Broadcasts.Events;
using Missions;
using Missions.Endpoint;
using Quobject.SocketIoClientDotNet.Client;
using UnityEngine;
using EventManager = Missions.Lifecycle.EventManager;

namespace Networking
{
    public class GCSSocket : MonoBehaviour
    {
        public static bool Connected;

        public static GCSSocket Instance;

        public Socket socket;

        private void OnEnable()
        {
            SocketEventManager.DisconnectSocket += OnDisconnectSocket;
        }

        private void OnDisable()
        {
            SocketEventManager.DisconnectSocket -= OnDisconnectSocket;
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                ConnectToSocket();
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (socket == null) return;
            socket.Emit("testee", "GCS Disconnecting.");
            socket.Disconnect();
        }

        public void ConnectToSocket()
        {
            socket = IO.Socket(ServerURL.SOCKET_IO);
            if (socket != null)
            {
                socket.On(Socket.EVENT_CONNECT, () =>
                {
                    socket.Emit("testee", "GCS Connected.");
                    Connected = true;
                    SocketEventManager.OnSocketConnected();
                    Debug.Log("Connected Event Received.");
                });

                socket.On(Socket.EVENT_DISCONNECT, () =>
                {
                    Connected = false;
                    Debug.Log("Disconnect Event Received.");
                });

                // Add listener for test data.
                socket.On("tester",
                    data => { Debug.Log("Testing Data Received:" + data); });

                socket.On(ServerURL.QUERY_RECEIVED, data =>
                {
                    Debug.Log(data);
                    SocketEventManager.OnDataRecieved(
                        new StringEventArgs {StringArgs = data.ToString()});
                });

                socket.On(MissionLifeCycleController.MISSION_INITIALIZED,
                    data =>
                    {
                        EventManager.OnInitialized();
                        Debug.Log("Mission Initialized Event Received.");
                    });

                socket.On(MissionLifeCycleController.MISSION_STARTED, data =>
                {
                    EventManager.OnStarted();
                    Debug.Log("Mission Started Event Received.");
                });

                socket.On(MissionLifeCycleController.MISSION_STOPPED, data =>
                {
                    EventManager.OnStopped();
                    Debug.Log("Mission Stopped Event Received.");
                });

                socket.On(ServerURL.NOTIFICATION_RECEIVED, data =>
                {
                    //SocketEventManager.OnNotificationRecieved(new NotificationEventArgs { Notification = new Notification(data.ToString()) });
                    Debug.Log("Notification Event Reveicved.");
                });

                // gcs-automated-query means that a query was handled autonomously by IRIS.
                socket.On("gcs-automated-query", data =>
                {
                    QueryCounters.OnIncrementCount();
                    Debug.Log("Handled query autonomously");
                    //Debug.Log(data);
                });

                // iris-generated-query means that a query was generated, data contains the robotID
                socket.On("gcs-generated-query", data =>
                {
                    QueryIndicator.OnQueryGenerated();
                    Debug.Log("A query was generated.");
                });

            }
        }

        private void OnDisconnectSocket(object sender, EventArgs e)
        {
            if (socket == null) return;
            socket.Emit("testee", "GCS Disconnecting.");
            socket.Disconnect();
            SocketEventManager.OnSocketDisconnected();
            socket = null;
            Connected = false;
        }
    }
}