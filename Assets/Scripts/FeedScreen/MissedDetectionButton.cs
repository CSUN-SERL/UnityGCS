using LiveFeedScreen.ROSBridgeLib;
using LiveFeedScreen.ROSBridgeLib.std_msgs;
using System.Collections;
using System.Collections.Generic;
using LiveFeedScreen.ROSBridgeLib.std_msgs.std_msgs;
using UnityEngine;
using UnityEngine.UI;

public class MissedDetectionButton : MonoBehaviour
{

    public Button MissedDetection;

    private ROSBridgeWebSocketConnection _ros;

    private const string Station1 = "ws:ubuntu@192.168.1.161";
    private const string Station4 = "ws:ubuntu@192.168.1.43";
    private const string Topic = "/testTopic";
    private const int Port = 9090;
    private readonly ROSBridgeMsg Msg = new StringMsg("Missed Detection. (Couldn't get it up)");

    private void Start()
    {
        Button button = MissedDetection.GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);

        _ros = new ROSBridgeWebSocketConnection(Station1, Port);
        _ros.Connect();
    }

    void OnButtonClick()
    {
        _ros.Publish(Topic, Msg);
        Debug.Log("Missed Detection.");
        _ros.Render();
    }

    private void OnApplicationQuit()
    {
        if (_ros != null)
        {
            _ros.Disconnect();
        }
    }
}
