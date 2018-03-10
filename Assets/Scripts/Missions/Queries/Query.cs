using System;
using Tobii.Plugins;
using UnityEngine;

namespace Missions.Queries
{
    [Serializable]
    public abstract class Query
    {
        public const string QueryType = "query";

        [SerializeField]
        public int QueryId { get; set; }

        [SerializeField]
        public int UserId { get; set; }

        [SerializeField]
        public int RobotId { get; set; }

        [SerializeField]
        public float ArrivalTime { get; set; }

        [SerializeField]
        public int Response { get; set; }

        public abstract void Display();

        public abstract string GetDisplayName();

        public abstract Query Deserialize(JSONNode json);

        public override string ToString()
        {
            return string.Format(
                "QID:{0}, Robot ID: {1}, Arrival Time: {2}, Response: {3}, Type:{4}",
                QueryId, RobotId, ArrivalTime, Response);
        }
    }
}