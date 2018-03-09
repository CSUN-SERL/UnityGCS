// (c) 2016-2018 Martin Cvengros. All rights reserved. Redistribution of source code without permission not allowed.

using System.Linq;
using UnityEngine;

namespace AudioStream
{
    [ExecuteInEditMode()]
    public class AudioStreamUNETSourceDemo : MonoBehaviour
    {
        public AudioStreamUNETSource audioStreamUNETSource;

        AudioSource @as;

        /// <summary>
        /// try to make font visible on high DPI resolutions
        /// </summary>
        int dpiMult = 1;

        void Start()
        {
            if (Screen.dpi > 300) // ~~ retina
                this.dpiMult = 2;

            this.@as = this.audioStreamUNETSource.GetComponent<AudioSource>();
        }

        GUIStyle guiStyleLabelSmall = null;
        GUIStyle guiStyleLabelMiddle = null;
        int frameSizeEnumSelection = 3;

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

            GUILayout.Label("==== Encoder");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Encoder thread priority: ", GUILayout.MaxWidth(Screen.width / 4));
            this.audioStreamUNETSource.encoderThreadPriority = (System.Threading.ThreadPriority)GUILayout.SelectionGrid((int)this.audioStreamUNETSource.encoderThreadPriority, System.Enum.GetNames(typeof(System.Threading.ThreadPriority)), 6, GUILayout.MaxWidth(Screen.width / 4 * 3));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Encoder application type: ", GUILayout.MaxWidth(Screen.width / 4));
            GUILayout.Label(this.audioStreamUNETSource.opusApplicationType.ToString());
            GUILayout.EndHorizontal();

            if (this.audioStreamUNETSource.encoderRunning)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Bitrate: ", GUILayout.MaxWidth(Screen.width / 4));
                this.audioStreamUNETSource.bitrate = (int)GUILayout.HorizontalSlider(this.audioStreamUNETSource.bitrate, 6, 510, GUILayout.MaxWidth(Screen.width / 2));
                GUILayout.Label(this.audioStreamUNETSource.bitrate + " KBits/s", GUILayout.MaxWidth(Screen.width / 4));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Complexity: ", GUILayout.MaxWidth(Screen.width / 4));
                this.audioStreamUNETSource.complexity = (int)GUILayout.HorizontalSlider(this.audioStreamUNETSource.complexity, 0, 10, GUILayout.MaxWidth(Screen.width / 2));
                var cdesc = "(Medium)";
                switch (this.audioStreamUNETSource.complexity)
                {
                    case 0:
                    case 1:
                    case 2:
                        cdesc = "(Low)";
                        break;
                    case 3:
                    case 4:
                    case 5:
                        cdesc = "(Medium)";
                        break;
                    case 6:
                    case 7:
                    case 8:
                        cdesc = "(High)";
                        break;
                    case 9:
                    case 10:
                        cdesc = "(Very High)";
                        break;
                }
                GUILayout.Label(this.audioStreamUNETSource.complexity + " " + cdesc, GUILayout.MaxWidth(Screen.width / 4));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Rate Control: ", GUILayout.MaxWidth(Screen.width / 4));
                this.audioStreamUNETSource.rate = (AudioStreamNetworkSource.RATE)GUILayout.SelectionGrid((int)this.audioStreamUNETSource.rate, System.Enum.GetNames(typeof(AudioStreamNetworkSource.RATE)), 3, GUILayout.MaxWidth(Screen.width / 4 * 3));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Frame size: ", GUILayout.MaxWidth(Screen.width / 4));
                this.frameSizeEnumSelection = GUILayout.SelectionGrid(this.frameSizeEnumSelection, System.Enum.GetNames(typeof(AudioStreamNetworkSource.OPUSFRAMESIZE)).Select(s => s.Split('_')[1]).ToArray(), 6, GUILayout.MaxWidth(Screen.width / 4 * 3));
                GUILayout.EndHorizontal();
                switch (this.frameSizeEnumSelection)
                {
                    case 0:
                        this.audioStreamUNETSource.frameSize = AudioStreamNetworkSource.OPUSFRAMESIZE.OPUSFRAMESIZE_120;
                        break;
                    case 1:
                        this.audioStreamUNETSource.frameSize = AudioStreamNetworkSource.OPUSFRAMESIZE.OPUSFRAMESIZE_240;
                        break;
                    case 2:
                        this.audioStreamUNETSource.frameSize = AudioStreamNetworkSource.OPUSFRAMESIZE.OPUSFRAMESIZE_480;
                        break;
                    case 3:
                        this.audioStreamUNETSource.frameSize = AudioStreamNetworkSource.OPUSFRAMESIZE.OPUSFRAMESIZE_960;
                        break;
                    case 4:
                        this.audioStreamUNETSource.frameSize = AudioStreamNetworkSource.OPUSFRAMESIZE.OPUSFRAMESIZE_1920;
                        break;
                    case 5:
                        this.audioStreamUNETSource.frameSize = AudioStreamNetworkSource.OPUSFRAMESIZE.OPUSFRAMESIZE_2880;
                        break;
                }
            }

            GUILayout.Space(10);

            GUILayout.Label("==== Audio source");

            GUILayout.Label(string.Format("Output samplerate: {0} channels: {1}", this.audioStreamUNETSource.serverSampleRate, this.audioStreamUNETSource.serverchannels));

            if (this.@as != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Volume: ", GUILayout.MaxWidth(Screen.width / 4));
                this.@as.volume = GUILayout.HorizontalSlider(this.@as.volume, 0f, 1f, GUILayout.MaxWidth(Screen.width / 2));
                GUILayout.Label(Mathf.RoundToInt(this.@as.volume * 100f) + " %", GUILayout.MaxWidth(Screen.width / 4));
                GUILayout.EndHorizontal();
            }

            this.audioStreamUNETSource.listenHere = GUILayout.Toggle(this.audioStreamUNETSource.listenHere, "Listen here");

            GUILayout.Space(10);

            GUILayout.Label("==== Network");

            if (this.audioStreamUNETSource.listenIP != "0.0.0.0")
                GUILayout.Label(string.Format("Running at {0} : {1}", this.audioStreamUNETSource.listenIP, this.audioStreamUNETSource.listenPort));
            else
                GUILayout.Label("No network seems to be available");

            GUILayout.Label(string.Format("Network buffer size: {0}, fixed rate reactor timeout: {1} ms", this.audioStreamUNETSource.networkQueueSize, this.audioStreamUNETSource.threadAwakeTimeout));

            GUILayout.Label(string.Format("Clients: {0}", this.audioStreamUNETSource.clientsConnectedCount));

            GUILayout.Space(10);

            GUILayout.Label("==== Status");

            GUILayout.Label(string.Format("State = {0} {1}"
                , this.audioStreamUNETSource.encoderRunning ? "Playing" : "Stopped"
                , this.audioStreamUNETSource.lastErrorString
                )
                );

            GUILayout.BeginHorizontal();

            GUILayout.Label(string.Format("Audio buffer size: {0} / available: {1}", this.audioStreamUNETSource.dspBufferSize, this.audioStreamUNETSource.audioSamplesSize), GUILayout.MaxWidth( Screen.width / 3 ));

            var r = Mathf.CeilToInt(((float)this.audioStreamUNETSource.audioSamplesSize / (float)this.audioStreamUNETSource.dspBufferSize) * 10f);
            var c = Mathf.Min(r, 10);

            GUI.color = Color.Lerp(Color.red, Color.green, c / 10f);

            this.gauge.Length = 0;
            for (int i = 0; i < c; ++i) this.gauge.Append("#");
            GUILayout.Label(this.gauge.ToString(), GUILayout.MaxWidth(Screen.width / 2));

            GUILayout.EndHorizontal();
        }
    }
}