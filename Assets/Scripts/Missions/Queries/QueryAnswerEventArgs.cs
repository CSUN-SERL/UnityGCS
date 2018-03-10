using System;

namespace Missions.Queries
{
    public class QueryAnswerEventArgs : EventArgs
    {
        public int QueryId { get; set; }
        public string Answer { get; set; }
    }
}