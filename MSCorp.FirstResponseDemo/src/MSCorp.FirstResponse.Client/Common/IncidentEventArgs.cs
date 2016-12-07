using System;
using MSCorp.FirstResponse.Client.Models;

namespace MSCorp.FirstResponse.Client.Common
{
    public class IncidentEventArgs : EventArgs
    {
        public IncidentModel Incident { get; }

        public IncidentEventArgs(IncidentModel incident)
        {
            Incident = incident;
        }
    }
}