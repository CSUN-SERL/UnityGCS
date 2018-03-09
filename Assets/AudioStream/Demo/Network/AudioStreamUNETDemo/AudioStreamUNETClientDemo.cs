using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace AudioStream
{
    [ExecuteInEditMode()]
    public class AudioStreamUNETClientDemo : MonoBehaviour
    {
        public AudioStreamUNETClient audioStreamUNETClient;

        /// <summary>
        /// try to make font visible on high DPI resolutions
        /// </summary>
        int dpiMult = 1;

        void Start()
        {
            if (Screen.dpi > 300) // ~~ retina
                this.dpiMult = 2;
        }

        GUIStyle guiStyleLabelSmall = null;
        GUIStyle guiStyleLabelMiddle = null;
        System.Text.StringBuilder gauge = new System.Text.StringBuilder(10);

        void OnGUI()
        {
            if (this.guiStyleLabelSmall == null)
            {
                this.guiStyleLabelSmall = new GUIStyle(GUI.skin.GetStyle("Label"));
                this.guiStyleLabelSmall.fontSize = 7 * this.dpiMult;
                this.guiStyleLabelSmall.margin = new RectOffset(0, 0, 0, 0);
            }

            if (this.guiStyleLabelMiddle == null)
            {
                this.guiStyleLabelMiddle = new GUIStyle(GUI.skin.GetStyle("Label"));
                this.guiStyleLabelMiddle.fontSize = 8 * this.dpiMult;
            }

            GUILayout.Label("", this.guiStyleLabelSmall); // statusbar on mobile overlay
            GUILayout.Label("", this.guiStyleLabelSmall);
            GUILayout.Label("AudioStream v " + About.version + " © 2016-2018 Martin Cvengros", this.guiStyleLabelMiddle);

            GUILayout.Label("==== Decoder");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Decoder thread priority: ", GUILayout.MaxWidth(Screen.width / 4));
            this.audioStreamUNETClient.decoderThreadPriority = (System.Threading.ThreadPriority)GUILayout.SelectionGrid((int)this.audioStreamUNETClient.decoderThreadPriority, System.Enum.GetNames(typeof(System.Threading.ThreadPriority)), 5, GUILayout.MaxWidth(Screen.width / 4 * 3));
            GUILayout.EndHorizontal();

            if (!NetworkTransport.IsStarted)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Resampler quality: ", GUILayout.MaxWidth(Screen.width / 4));
                this.audioStreamUNETClient.resamplerQuality = (int)GUILayout.HorizontalSlider(this.audioStreamUNETClient.resamplerQuality, 0, 10, GUILayout.MaxWidth(Screen.width / 2));
                GUILayout.Label(this.audioStreamUNETClient.resamplerQuality.ToString(), GUILayout.MaxWidth(Screen.width / 4));
                GUILayout.EndHorizontal();

                GUILayout.Space(10);

                GUILayout.Label("==== Network");

                GUILayout.BeginHorizontal();
                GUILayout.Label("Server IP: ", GUILayout.MaxWidth(Screen.width / 4));
                this.audioStreamUNETClient.serverIP = GUILayout.TextField(this.audioStreamUNETClient.serverIP, GUILayout.MaxWidth(Screen.width / 4));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Server port: ", GUILayout.MaxWidth(Screen.width / 4));
                this.audioStreamUNETClient.serverTransferPort = int.Parse(GUILayout.TextField(this.audioStreamUNETClient.serverTransferPort.ToString(), GUILayout.MaxWidth(Screen.width / 4)));
                GUILayout.EndHorizontal();

                if (GUILayout.Button("Connect"))
                    this.audioStreamUNETClient.Connect();
            }
            else
            {
                GUILayout.Label("Decode info:");
                GUILayout.Label(string.Format("Current frame size: {0}", this.audioStreamUNETClient.frameSize));
                GUILayout.Label(string.Format("Bandwidth: {0}", this.audioStreamUNETClient.opusBandwidth));
                GUILayout.Label(string.Format("Mode: {0}", this.audioStreamUNETClient.opusMode));
                GUILayout.Label(string.Format("Channels: {0}", this.audioStreamUNETClient.serverChannels));
                GUILayout.Label(string.Format("Frames per packet: {0}", this.audioStreamUNETClient.opusNumFramesPerPacket));
                GUILayout.Label(string.Format("Samples per frame: {0}", this.audioStreamUNETClient.opusNumSamplesPerFrame));

                GUILayout.Space(10);

                GUILayout.Label("==== Audio source");

                GUILayout.Label(string.Format("Output sample rate: {0}, channels: {1}", this.audioStreamUNETClient.clientSampleRate, this.audioStreamUNETClient.clientChannels));

                GUILayout.BeginHorizontal();
                GUILayout.Label("Volume: ", GUILayout.MaxWidth(Screen.width / 4));
                this.audioStreamUNETClient.volume = GUILayout.HorizontalSlider(this.audioStreamUNETClient.volume, 0f, 1f, GUILayout.MaxWidth(Screen.width / 2));
                GUILayout.Label(Mathf.RoundToInt(this.audioStreamUNETClient.volume * 100f) + " %", GUILayout.MaxWidth(Screen.width / 4));
                GUILayout.EndHorizontal();

                this.audioStreamUNETClient.listenHere = GUILayout.Toggle(this.audioStreamUNETClient.listenHere, "Listen here");

                GUILayout.Space(10);

                GUILayout.Label("==== Network");

                GUILayout.Label(string.Format("Connected to: {0}:{1} (id:{2}) ", this.audioStreamUNETClient.serverIP, this.audioStreamUNETClient.serverTransferPort, this.audioStreamUNETClient.serverConnectionId));
                GUILayout.Label(string.Format("Server sample rate: {0}, channels: {1}", this.audioStreamUNETClient.serverSampleRate, this.audioStreamUNETClient.serverChannels));
                GUILayout.Label(string.Format("Network buffer size: {0}, fixed rate reactor timeout: {1} ms", this.audioStreamUNETClient.networkQueueSize, this.audioStreamUNETClient.threadAwakeTimeout));

                GUILayout.Space(10);

                GUILayout.Label("==== Status");

                GUILayout.Label(string.Format("State = {0} {1}"
                    , this.audioStreamUNETClient.decoderRunning ? "Playing" : "Stopped"
                    , this.audioStreamUNETClient.lastErrorString
                    )
                    );

                GUILayout.BeginHorizontal();

                GUILayout.Label(string.Format("Audio buffer size: {0} / available: {1}", this.audioStreamUNETClient.dspBufferSize, this.audioStreamUNETClient.capturedAudioSamples), GUILayout.MaxWidth(Screen.width / 3));

                var r = Mathf.CeilToInt(((float)this.audioStreamUNETClient.capturedAudioSamples / (float)this.audioStreamUNETClient.dspBufferSize) * 10f);
                var c = Mathf.Min(r, 10);

                GUI.color = this.audioStreamUNETClient.capturedAudioFrame ? Color.Lerp(Color.red, Color.green, c / 10f) : Color.red;

                this.gauge.Length = 0;
                for (int i = 0; i < c; ++i) this.gauge.Append("#");
                GUILayout.Label(this.gauge.ToString(), GUILayout.MaxWidth(Screen.width / 2));

                GUILayout.EndHorizontal();

                GUI.color = Color.white;

                GUILayout.Space(20);
                
                if (GUILayout.Button("Disconnect"))
                    this.audioStreamUNETClient.Disconnect();
            }
        }
    }
}