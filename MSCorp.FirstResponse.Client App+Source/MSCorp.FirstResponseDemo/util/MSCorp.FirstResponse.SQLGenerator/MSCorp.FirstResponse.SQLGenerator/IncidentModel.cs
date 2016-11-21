using System;
using System.Collections.Generic;

namespace MSCorp.SQLGenerator.MSCorp.FirstResponse.Client.Models
{
    public class IncidentModel
    {
        public int Id { get; set; }

        public int Priority { get; set; }
        public string CallNumber { get; set; }
        public string Phone { get; set; }
        public string Title { get; set; }
        public DateTime ReceivedTime { get; set; }

        public string PropertyName { get; set; }
        public string Address { get; set; }

        public string ReportingParty { get; set; }
        public string Description { get; set; }

        public double Longitude { get; set; }
        public double Latitude { get; set; }

//        public List<ResponderRequestModel> Responders { get; set; } = new List<ResponderRequestModel>();


        public TimeSpan TimeToResolve { get; set; }
        public DateTime? FullyAttendedTime { get; set; }
    }
}