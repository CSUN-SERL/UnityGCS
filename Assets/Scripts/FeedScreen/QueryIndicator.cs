using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class QueryIndicator : MonoBehaviour {

    // this is the UI.Text or other UI element you want to toggle
    public MaskableGraphic ImageToToggle, TextToToggle;

    public int numBlinks = 4;
    public float Interval = .1f;
    public float StartDelay = 0.5f;
    public bool CurrentState = true;
    public bool DefaultState = true;
    // _isBlinking prevents us from starting the ToggleState again if its already going.
    private bool _isBlinking = false;
    // use this thingamabob to start and stop the query indicator.
    private static bool _noticeMeSenpai = false;

    // Event to keep track of when we receive notification that
    // a query was generated, from the GCSSocket
    public static event EventHandler<EventArgs> QueryGenerated;


    void Start()
    {
        ImageToToggle.enabled = DefaultState;
        TextToToggle.enabled = DefaultState;
        // StartBlink();
    }

    void Update()
    {
        // want to do StartBlink() once per generated query
        if (_noticeMeSenpai == true)
        {
            StartBlink();
        }

        _noticeMeSenpai = false;
    }

    public void StartBlink()
    {
        // do not invoke the blink twice - needed if you need to start the blink from an external object
        if (_isBlinking)
            return;

        if (ImageToToggle != null && TextToToggle != null)
        {
            _isBlinking = true;
            InvokeRepeating("ToggleState", StartDelay, Interval);
        }
    }

    public void ToggleState()
    {
        ImageToToggle.enabled = !ImageToToggle.enabled;
        TextToToggle.enabled = !TextToToggle.enabled;

        // use numBlinks to ensure fixed number of blinks per generated query.
        numBlinks--;
        if (numBlinks == 0)
        {
            _isBlinking = false;
            numBlinks = 4;
            CancelInvoke("ToggleState");
        }
    }

    // when we get this event toggle bool on to start blink in Update
    // and then update turns it off
    public static void OnQueryGenerated()
    {
        //Debug.Log("A query was generated.");
        _noticeMeSenpai = true;
    }
}
