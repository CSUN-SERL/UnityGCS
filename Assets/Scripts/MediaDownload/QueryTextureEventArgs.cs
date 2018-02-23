using System;
using Missions.Queries;
using UnityEngine;

namespace MediaDownload
{
    public class QueryTextureEventArgs : EventArgs
    {
        public Query query { get; set; }
        public Texture texture { get; set; }
    }
}