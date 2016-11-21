using System;

namespace MSCorp.SQLGenerator
{
    public class Incident
    {
        public int Id { get; set; }
        public string CallNumber { get; set; }
        public string Phone { get; set; }
        public string Title { get; set; }
        public DateTime ReceivedTime { get; set; }
        public string Address { get; set; }
        public string ReportingParty { get; set; }
        public string Description { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public Responder[] Responders { get; set; }
        public int IncidentCategory { get; set; }
        public bool IsHighPriority { get; set; }
    }
}