using System;
using Missions.Broadcasts.Events.EventArgs;
using Missions.Queries;
using UnityEngine;

namespace FeedScreen.Experiment.Missions.Broadcasts.Events
{
    public class EventManager : MonoBehaviour
    {
        public static event EventHandler<QueryEventArgs> RecordQuery;

        public static void OnRecordQuery(Query query)
        {
            if (RecordQuery != null)
                RecordQuery(null, new QueryEventArgs {Query = query});
        }
    }
}