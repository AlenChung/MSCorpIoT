using System;
using MSCorp.FirstResponse.Client.Controls;

namespace MSCorp.FirstResponse.Client.Common
{
    public class IncidentChangedEventArgs : EventArgs
    {
        public IncidentIcon IncidentIcon { get; }

        public IncidentChangedEventArgs(IncidentIcon incident)
        {
            IncidentIcon = incident;
        }
    }
}