// (c) 2016-2018 Martin Cvengros. All rights reserved. Redistribution of source code without permission not allowed.

using UnityEngine;
using UnityEngine.Networking;

namespace AudioStream
{
    /// <summary>
    /// Implements UNET transport for AudioStreamNetworkClient
    /// </summary>
    public class AudioStreamUNETClient : AudioStreamNetworkClient
    {
        // ========================================================================================================================================
        #region Required implementation
        /// <summary>
        /// Provides network buffer for base
        /// </summary>
        protected override ThreadSafeQueue<byte[]> networkQueue { get; set; }
        #endregion

        // ========================================================================================================================================
        #region Editor
        [Header("[Network client setup]")]
        [Tooltip("IP address of the AudioStreamNetworkSource to connect to")]
        public string serverIP = "0.0.0.0";
        [Tooltip("Port to connect to")]
        public int serverTransferPort = AudioStreamUNETSource.listenPortDefault;
        [Tooltip("Automatically connect and play on Start with given parameters")]
        public bool autoConnect = true;
        #endregion

        // ========================================================================================================================================
        #region Non editor
        /// <summary>
        /// This host's opened socket
        /// </summary>
        int hostId = -1;
        /// <summary>
        /// Connection id sent by server
        /// </summary>
		public int serverConnectionId { get; private set; }

        public int networkQueueSize
        {
            get
            {
                if (this.networkQueue != null)
                    return this.networkQueue.Size();
                else
                    return 0;
            }
        }
        public uint threadAwakeTimeout { get; private set; }
        #endregion

        // ========================================================================================================================================
        #region Networking
        byte[] networkReceivedBuffer = new byte[2000];

        public void Connect()
        {
            this.networkQueue = new ThreadSafeQueue<byte[]>();

            // Build and configure UNET network

            GlobalConfig gc = new GlobalConfig();
            gc.ReactorModel = ReactorModel.FixRateReactor;
            // FixRateReactor default timeout (10) seems to be working reasonably well, leave it at that since lowering it to some minimum such as 1 yields unstable results
            // TODO: make this configurable
            this.threadAwakeTimeout = 10;
            gc.ThreadAwakeTimeout = this.threadAwakeTimeout;

            LOG(LogLevel.INFO, "Network thread awake timeout: {0} ms", threadAwakeTimeout);

            ConnectionConfig cc = new ConnectionConfig();
            cc.AddChannel(QosType.ReliableSequenced);

            HostTopology ht = new HostTopology(cc, 1);

            NetworkTransport.Init();

            // Open sockets for client
            this.hostId = NetworkTransport.AddHost(ht);

            if (this.hostId < 0)
            {
                LOG(LogLevel.ERROR, "Client socket creation for {0} port {1} failed", this.serverIP, this.serverTransferPort);
                return;
            }
            else
            {
                LOG(LogLevel.INFO, "Created client socket for {0} port {1}, connecting...", this.serverIP, this.serverTransferPort);
            }

			this.serverConnectionId = -1;

            // Connect
            byte error;
            NetworkTransport.Connect(this.hostId, this.serverIP, this.serverTransferPort, 0, out error);
            NETWORK_ERR_CHECK(error);
        }

        public void Disconnect()
        {
            base.StopDecoder();

            if (NetworkTransport.IsStarted)
            {
				if (this.lastErrorString == NetworkError.Ok.ToString() && this.serverConnectionId > 0)
                {
                    byte error;
                    NetworkTransport.Disconnect(this.hostId, this.serverConnectionId, out error);
                    NETWORK_ERR_CHECK(error, false);

					LOG (LogLevel.INFO, "Disconnected as {0}", this.serverConnectionId);
                }

                NetworkTransport.Shutdown();
            }

			this.serverConnectionId = -1;
        }
        #endregion

        // ========================================================================================================================================
        #region Unity lifecycle, networking

        protected override void Awake()
        {
            base.Awake();

            // You can try to increase frame rate to speed up network messaging processing in case of occasional audio frame drops, although it seems to be performing without problems on sufficiently powerful hardware
			// Application.targetFrameRate = 60;
        }

        protected override void Start()
        {
            base.Start();

            if (this.autoConnect)
                this.Connect();
        }

        protected override void Update()
        {
            base.Update();

            int recHostId;
            int connectionId;
            int channelId;
            int dataSize;
            byte error;

            if (!NetworkTransport.IsStarted)
                return;

            NetworkEventType networkEvent = NetworkEventType.Nothing;

			// completely drain network queue on iOS/mobiles ( for some reason )
			#if UNITY_IOS || UNITY_ANDROID
			do
			{
			#endif

            // Receive can only be called from the main thread.
            networkEvent = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, this.networkReceivedBuffer, this.networkReceivedBuffer.Length, out dataSize, out error);
            NETWORK_ERR_CHECK(error, false);

            switch (networkEvent)
            {
                case NetworkEventType.Nothing:
                    break;

                case NetworkEventType.ConnectEvent:
                    if (recHostId == this.hostId)
                    {
                        this.serverConnectionId = connectionId;

                        LOG(LogLevel.INFO, "Connected");
                    }

                    break;

                case NetworkEventType.DataEvent:
                    if (recHostId == this.hostId)
                    {
                        // create and start decoder based on received server channel properties if it's configuration message

                        bool audioData = true;

                        if (dataSize <= 80)
                        {
                            var response = AudioStreamSupport.BytesToString(this.networkReceivedBuffer).Split('#');

                            if (response.Length == 6 && response[1] == AudioStreamNetworkSource.AUDIOSTREAMSOURCE_CONFIG_PREFIX)
                            {
                                int srate, schannels;
                                if (int.TryParse(response[2], out srate)
                                    && int.TryParse(response[3], out schannels)
                                    )
                                {
                                    this.serverSampleRate = srate;
                                    this.serverChannels = schannels;

                                    base.StartDecoder();

                                    audioData = false;
                                }
                            }
                        }

                        // add incoming audio to queue
                        if (audioData)
                        {
                            var packet = new byte[dataSize];
                            System.Array.Copy(this.networkReceivedBuffer, packet, dataSize);

                            this.networkQueue.Enqueue(packet);
                        }
                    }
                    break;

                case NetworkEventType.DisconnectEvent:
                    if (recHostId == this.hostId && connectionId == this.serverConnectionId)
                    {
                        LOG(LogLevel.INFO, "Disconnected");
                        this.Disconnect();
                    }

                    break;
            }

            if ((NetworkError)error == NetworkError.Timeout)
            {
                this.Disconnect();
            }

			#if UNITY_IOS || UNITY_ANDROID
			} while (networkEvent != NetworkEventType.Nothing);
			#endif
        }

        protected override void OnDestroy()
        {
            this.Disconnect();

            base.OnDestroy();
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
            if (network_error != (byte)NetworkError.Ok)
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