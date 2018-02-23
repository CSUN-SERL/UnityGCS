using System;
using Missions.Lifecycle;
using UnityEngine;
using UnityEngine.UI;

namespace Missions
{
    public class EndMenuScript : MonoBehaviour
    {
        private GameObject _popup;
        public Button ContinueButton;

        // Use this for initialization
        private void Awake()
        {
        }

        private void OnEnable()
        {
            EventManager.Completed += DisplayEndPopup;
        }

        private void OnDisable()
        {
            EventManager.Completed -= DisplayEndPopup;
        }

        private void Update()
        {
            //_popup.SetActive(MissionTimer.Completed);
        }

        private void DisplayEndPopup(object sender, EventArgs e)
        {
            _popup = gameObject.transform.GetChild(0).gameObject;
            _popup.SetActive(true);
            _popup.transform.position = new Vector3(Screen.width / 2.0F,
                Screen.height / 2.0F);
            _popup.transform.localScale = new Vector3(2.0F, 2.0F);


            //ContinueButton = gameObject.transform.Find("ContinueButton").GetComponent<Button>();
            ContinueButton.onClick.AddListener(EventManager.OnClose);
        }
    }
}