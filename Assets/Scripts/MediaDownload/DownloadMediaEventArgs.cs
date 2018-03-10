using System;
using Missions.Queries;

namespace MediaDownload
{
    public class DownloadMediaEventArgs : EventArgs
    {
        public Query query { get; set; }
        public string MediaType { get; set; }
        public string fileName { get; set; }
    }
}