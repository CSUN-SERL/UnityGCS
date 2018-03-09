// (c) 2016-2018 Martin Cvengros. All rights reserved. Redistribution of source code without permission not allowed.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace AudioStream
{
    /// <summary>
    /// Implements UNET transport for AudioStreamNetworkSource
    /// </summary>
    public class AudioStreamUNETSource : AudioStreamNetworkSource
    {
        // ========================================================================================================================================
        #region Editor
        [Header("[Network source setup]")]
        [Tooltip("Port to listen to for client connections")]
        public int listenPort = AudioStreamUNETSource.listenPortDefault;
        public const int listenPortDefault = 33000;
        [Tooltip("Maximum number of client connected simultaneously")]
        public int maxConnections = 10;
        #endregion

        // ========================================================================================================================================
        #region Non editor
        /// <summary>
        /// Host IP as reported by UNET
        /// </summary>
        public string listenIP
        {
            get;
            protected set;
        }
        /// <summary>
        /// This host's opened socket
        /// </summary>
        int hostId = -1;
        /// <summary>
        /// Channel from config
        /// </summary>
        byte channel_reliableSequenced;
        /// <summary>
        /// Currently connected clients
        /// </summary>
        List<int> clientConnectionIds = new List<int>();
        /// <summary>
        /// 
        /// </summary>
        public int clientsConnectedCount
        {
            get { return this.clientConnectionIds.Count; }
        }
        public uint threadAwakeTimeout { get; private set; }
        #endregion

        // ========================================================================================================================================
        #region Unity lifecycle, networking

        protected override void Awake()
        {
            base.Awake();

            // You can try to increase frame rate to speed up network messaging processing in case of occasional audio frame drops, although it seems to be performing without problems on sufficiently powerful hardware even without this
            // Application.targetFrameRate = 60;
        }

        protected override void Start()
        {
            base.Start();

            this.listenIP = Network.player.ipAddress;

            // Build and configure UNET network

            GlobalConfig gc = new GlobalConfig();
            gc.ReactorModel = ReactorModel.FixRateReactor;
            // FixRateReactor default timeout (10) seems to be working reasonably well, leave it at that since lowering it to some minimum such as 1 yields unstable results
            // TODO: make this configurable
            this.threadAwakeTimeout = 10;
            gc.ThreadAwakeTimeout = this.threadAwakeTimeout;

            LOG(LogLevel.INFO, "Network thread awake timeout: {0} ms", threadAwakeTimeout);

            ConnectionConfig cc = new ConnectionConfig();
            this.channel_reliableSequenced = cc.AddChannel(QosType.ReliableSequenced);

            HostTopology ht = new HostTopology(cc, this.maxConnections);

            NetworkTransport.Init();

            // Open socket for server
            this.hostId = NetworkTransport.AddHost(ht, this.listenPort);

            if (this.hostId < 0)
            {
                LOG(LogLevel.ERROR, "Server socket creation on port {0} failed", this.listenPort);
                return;
            }
            else
            {
                LOG(LogLevel.INFO, "Server socket creation successful on port {0}", this.listenPort);
            }
        }

        protected override void Update()
        {
            base.Update();

            // process network events first
            int recHostId;
            int connectionId;
            int channelId;
            int dataSize;
            byte[] buffer = new byte[1024];
            byte error;

			NetworkEventType networkEvent = NetworkEventType.Nothing;

			// completely drain network queue on iOS/mobiles ( for some reason )
			#if UNITY_IOS || UNITY_ANDROID
			do
			{
			#endif

			networkEvent = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, buffer, 1024, out dataSize, out error);
            NETWORK_ERR_CHECK(error, false);

            switch (networkEvent)
            {
                case NetworkEventType.Nothing:
                    break;

                case NetworkEventType.ConnectEvent:
                    // send config data on connect
                    if (recHostId == this.hostId)
                    {
                        var message = AudioStreamSupport.StringToBytes(string.Format("#{0}#{1}#{2}#{3}#", AudioStreamNetworkSource.AUDIOSTREAMSOURCE_CONFIG_PREFIX, this.serverSampleRate, this.serverchannels, 0 /* reserved - not used for now */));

                        byte _error;
                        NetworkTransport.Send(this.hostId, connectionId, this.channel_reliableSequenced, message, message.Length, out _error);
                        NETWORK_ERR_CHECK(_error, false);

                        if ((NetworkError)_error == NetworkError.Ok)
                        {
                            this.clientConnectionIds.Add(connectionId);
                            LOG(LogLevel.INFO, "Client {0} connected , sent audio config data", connectionId);
                        }
                        else
                        {
                            LOG(LogLevel.ERROR, "Client {0} connected, unable to send config data - client ignored");
                        }
                    }

                    break;

                case NetworkEventType.DataEvent:
                    // dont't care about any received data
                    if (recHostId == this.hostId)
                    {
                    }

                    break;

                case NetworkEventType.DisconnectEvent:
                    // received disconnect
                    if (recHostId == this.hostId)
                    {
                        this.clientConnectionIds.Remove(connectionId);

                        LOG(LogLevel.INFO, "Client {0} disconnected", connectionId);
                    }

                    break;
            }

			#if UNITY_IOS || UNITY_ANDROID
			} while (networkEvent != NetworkEventType.Nothing);
			#endif

			// send output to the clients
            // (Send can only be called from the main thread.)
            byte[] packet = null;
            packet = this.networkQueue.Dequeue();

            if (packet != null)
            {
                foreach (var clientId in this.clientConnectionIds)
                {
                    byte _error;
                    NetworkTransport.Send(this.hostId, clientId, this.channel_reliableSequenced, packet, packet.Length, out _error);
                    NETWORK_ERR_CHECK(_error, false);
                }
            }
        }

        #endregion

        // ========================================================================================================================================
        #region Support
        /// <summary>
        /// UNET
        /// </summary>
        /// <param name="network_error"></param>
        /// <param name="throwOnError"></param>
        public void NETWORK_ERR_CHECK(byte network_error, bool throwOnError = true)
        {
            if ((NetworkError)network_error != NetworkError.Ok)
            {
                this.lastErrorString = ((NetworkError)System.Enum.Parse(typeof(NetworkError), network_error.ToString())).ToString();

                var message = string.Format("Network error: {0}", this.lastErrorString);
                if (throwOnError)
                    throw new System.Exception(message);
                else
                    LOG(LogLevel.ERROR, message);
            }
            else
            {
                this.lastErrorString = NetworkError.Ok.ToString();
            }

        }
        #endregion
    }
}