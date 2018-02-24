using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class QueryIndicator : MonoBehaviour {

    // this is the UI.Text or other UI element you want to toggle
    public MaskableGraphic ImageToToggle, TextToToggle;

    public float Interval = 1f;
    public float StartDelay = 0.5f;
    public bool CurrentState = true;
    public bool DefaultState = true;
    bool _isBlinking = false;

    void Start()
    {
        ImageToToggle.enabled = DefaultState;
        TextToToggle.enabled = DefaultState;
        StartBlink();
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
    }

}
