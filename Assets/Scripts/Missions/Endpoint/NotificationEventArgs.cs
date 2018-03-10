using System;

namespace Missions.Endpoint
{
    public class NotificationEventArgs : EventArgs
    {
        public Notification Notification { get; set; }
    }
}