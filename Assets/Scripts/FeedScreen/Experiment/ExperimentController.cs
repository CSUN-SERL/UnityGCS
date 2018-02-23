using UnityEngine;

namespace FeedScreen.Experiment
{
    public class ExperimentController : MonoBehaviour
    {
        public static ExperimentController Instance;
        public GameObject ParticipantPrefab;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
                Destroy(gameObject);
        }
    }
}