using System.Collections.Generic;
using Missions.Broadcasts.Events.EventArgs;
using Missions.Display.Queries;
using Missions.Endpoint;
using Missions.Queries;
using UnityEngine;

namespace Missions
{
    /// <summary>
    ///     Instantiates GUI elements such as Queries to the participant.
    /// </summary>
    public class QueryContainer : MonoBehaviour
    {
        private Queue<Query> _pendingQueries;
        public GameObject QueryPrefab;

        private void OnEnable()
        {
            SocketEventManager.QueryRecieved += OnQueryReceived;
            _pendingQueries = new Queue<Query>();
        }

        private void OnDisable()
        {
            SocketEventManager.QueryRecieved -= OnQueryReceived;
            _pendingQueries = null;
        }

        private void Update()
        {
            while (_pendingQueries.Count > 0)
            {
                var query = _pendingQueries.Dequeue();
                var queryprefab = Instantiate(QueryPrefab);
                queryprefab.GetComponent<QueryButton>().query = query;
                queryprefab.name = string.Format("Query{0}", query.QueryId);
                queryprefab.transform.SetParent(gameObject.transform, false);
            }
        }

        public void AddQuery(Query q)
        {
            _pendingQueries.Enqueue(q);
        }

        private void OnQueryReceived(object sender, QueryEventArgs e)
        {
            AddQuery(e.Query);   
        }
    }
}