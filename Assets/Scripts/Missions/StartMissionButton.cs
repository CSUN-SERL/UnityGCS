using UnityEngine;
using UnityEngine.UI;

namespace Missions
{
    public class StartMissionButton : MonoBehaviour
    {
        private Button _button;

        private void Start()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(MissionLifeCycleController
                .StartMission);
        }

        private void Update()
        {
            _button.interactable =
                MissionLifeCycleController.Instance.Initialized;
        }
    }
}