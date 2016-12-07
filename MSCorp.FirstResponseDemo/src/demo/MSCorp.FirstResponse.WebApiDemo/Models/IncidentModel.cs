using MSCorp.FirstResponse.WebApiDemo.Constants;

namespace MSCorp.FirstResponse.WebApiDemo.Models
{
    /// <summary>
    /// Model of incident data
    /// </summary>
    public class IncidentModel
    {
        /// <summary>
        /// The id of the incident
        /// </summary>
        public int IncidentId { get; set; }
        /// <summary>
        /// The incident type
        /// </summary>
        public int DepartmentType { get; set; }
        /// <summary>
        /// The title of the incident
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// The Description of the incident.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The name of the database which the record has been sourced from.
        /// </summary>
        public string ShardName { get; set; }
        /// <summary>
        /// Region the incident is in.
        /// </summary>
        public int RegionId { get; set; }

    }
}