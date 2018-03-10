using System;
using System.Globalization;
using FeedScreen.Experiment.Missions.Broadcasts.Events;
using Tobii.Plugins;
using UnityEngine;

namespace Missions.Queries.QueryTypes
{
    public class ConfidenceQuery : Query
    {
        [SerializeField]
        public float Confidence { get; set; }

        public override void Display()
        {
            DisplayEventManager.OnDisplayConfidence(
                Confidence.ToString(CultureInfo.CurrentCulture));
        }

        public override string GetDisplayName()
        {
            return "Confidence Query";
        }

        public override Query Deserialize(JSONNode json)
        {
            throw new NotImplementedException();
        }
    }
}