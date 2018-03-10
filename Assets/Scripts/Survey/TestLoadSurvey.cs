using FeedScreen.Experiment.Missions.Broadcasts.Events;
using UnityEngine;

namespace Survey
{
    public class TestLoadSurvey : MonoBehaviour
    {
        private void Start()
        {
            EventManager.OnLoad(4);
            Debug.Log("nothign happens");
            EventManager.OnLoad(5);
        }

        private void OnEnable()
        {
            EventManager.Load += OnLoad;
        }

        private void OnLoad(object sender, IntEventArgs e)
        {
            Debug.Log(e.intField);
        }

        private void OnDisable()
        {
            EventManager.Load -= OnLoad;
        }
    }
}