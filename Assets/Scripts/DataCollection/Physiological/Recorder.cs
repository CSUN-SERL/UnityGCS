﻿using System;
using System.Collections;
using System.Data;
using Assets.Scripts.DataCollection.Physiological.Sensors;
using Missions;
using Participant;
using UnityEngine;
using Participant = Participant.Participant;

namespace Assets.Scripts.DataCollection.Physiological
{
    [RequireComponent(typeof(Uploader))]
    [RequireComponent(typeof(HeartRate))]
    [RequireComponent(typeof(GalvanicSkinResponse))]
    [RequireComponent(typeof(GazeTracking))]
    [RequireComponent(typeof(EmotionClassification))]
    public class Recorder : MonoBehaviour
    {
        private DataTable _dataTable;
        private EmotionClassification _emotionClassification;
        private GalvanicSkinResponse _galvanicSkinResponse;

        private GazeTracking _gazeTracker;
        private HeartRate _heartRate;
        public float Rate;

        private void OnEnable()
        {
            _gazeTracker = gameObject.GetComponent<GazeTracking>();
            _heartRate = gameObject.GetComponent<HeartRate>();
            _galvanicSkinResponse =
                gameObject.GetComponent<GalvanicSkinResponse>();
            _emotionClassification =
                gameObject.GetComponent<EmotionClassification>();

            _dataTable = new DataTable();
            _dataTable.Columns.Add("participant_id");
            _dataTable.Columns.Add("mission_number");
            _dataTable.Columns.Add("mission_time");
            _dataTable.Columns.Add("heart_rate");
            _dataTable.Columns.Add("galvanic_skin_response");
            _dataTable.Columns.Add("emotion");
            _dataTable.Columns.Add("gaze_x");
            _dataTable.Columns.Add("gaze_y");

            Missions.Lifecycle.EventManager.Started += BeginLogging;
            Missions.Lifecycle.EventManager.Completed += EndLogging;
        }

        private void OnDisable()
        {
            Missions.Lifecycle.EventManager.Started -= BeginLogging;
            Missions.Lifecycle.EventManager.Completed -= EndLogging;
        }

        private IEnumerator Log()
        {
            while (true)
            {
                yield return new WaitForSeconds(1F);


                if (ParticipantBehavior.Participant == null) continue;

                var newRow = _dataTable.NewRow();
                newRow["participant_id"] =
                    ParticipantBehavior.Participant.Data.Id;
                newRow["mission_number"] = MissionMetaData.MissionNumber;
                newRow["mission_time"] = Time.timeSinceLevelLoad;
                newRow["heart_rate"] = _heartRate.GetSensorValue();
                newRow["galvanic_skin_response"] =
                    _galvanicSkinResponse.GetSensorValue();
                newRow["emotion"] = _emotionClassification.GetSensorValue();
                newRow["gaze_x"] = _gazeTracker.GetSensorValue().x;
                newRow["gaze_y"] = _gazeTracker.GetSensorValue().y;
                _dataTable.Rows.Add(newRow);
            }
        }

        private void BeginLogging(object sender, EventArgs e)
        {
            Debug.Log("Starting Log");
            EventManager.OnStartLogging();
            StartCoroutine(Log());
        }

        private void EndLogging(object sender, EventArgs e)
        {
            Debug.Log("Stopping Log");
            EventManager.OnStopLogging();
            StopAllCoroutines();
            EventManager.OnUploadData(_dataTable);
            _dataTable.Clear();
        }
    }
}