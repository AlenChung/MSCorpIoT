using System;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace MSCorp.SQLGenerator
{
    public class Program
    {
        static void Main(string[] args)
        {
            var random = new Random(DateTime.Now.Second);
            var jsonData = File.ReadAllText(@"C:\Dev\MSCorp.FirstResponseDemo\src\MSCorp.FirstResponse.Client\Data\IncidentData.json"); // Will need changing to re-generate the insert scripts.
            var incidents = JsonConvert.DeserializeObject<Incident[]>(jsonData);

            var westIncidentJson =
                File.ReadAllText(
                    @"C:\dev\MSCorp.FirstResponseDemo\util\MSCorp.FirstResponse.SQLGenerator\MSCorp.FirstResponse.SQLGenerator\westSideIncidents.json"); // Will need changing to re-generate the insert scripts.
            var westSideIncidents = JsonConvert.DeserializeObject<Incident[]>(westIncidentJson);
            incidents = incidents.Concat(westSideIncidents).ToArray();

            // replace the reporting parties of all incidents with random lca people
            var personData = File.ReadAllText(@"C:\dev\MSCorp.FirstResponseDemo\deployment\AzureSearch\PersonData.json"); // Will need changing to re-generate the insert scripts.
            var persons = JsonConvert.DeserializeObject<Person[]>(personData);
            foreach (var incident in incidents)
            {
                // randomly choose an lca persona for the incident
                var randomPerson = persons[random.Next(0, persons.Length - 1)];
                incident.ReportingParty = $"{randomPerson.FirstName} {randomPerson.LastName}";
            }
            
            BuildSqlInsert(incidents, 1, "PoliceIncidents.sql", random);
            BuildSqlInsert(incidents, 2, "FireIncidents.sql", random);
            BuildSqlInsert(incidents, 3, "AmbulanceIncidents.sql", random);
        }

        private static void BuildSqlInsert(Incident[] incidents, int incidentCategory, string fileName, Random random)
        {
            var filteredIncidents = incidents.Where(i => i.Responders.Any(r => r.DepartmentType == incidentCategory));
            var insertStatment = new StringBuilder();
            foreach (var incident in filteredIncidents)
            {
                BuildSqlStatement(insertStatment, incident, incidentCategory, random);
            }
            insertStatment.AppendLine("GO");

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            File.WriteAllText(fileName, insertStatment.ToString());
        }

        private static void BuildSqlStatement(StringBuilder stringBulider, Incident incident, int serviceDept, Random random)
        {
            var addresses = incident.Address.Split(',');
            string addressLine1 = addresses.Length >= 1 ? addresses[0].Trim() : string.Empty;
            string addressLine2 = addresses.Length >= 2 ? addresses[1].Trim() : string.Empty;
            string addressLine3 = addresses.Length >= 3 ? addresses[2].Trim() : string.Empty;
            string addressLine4 = addresses.Length >= 4 ? addresses[3].Trim() : string.Empty;

            string phoneNumber = incident.Phone
                .Replace("XXXX", random.Next(1000, 9999).ToString())
                .Replace("XXX", random.Next(100, 999).ToString());

            var insert = "INSERT INTO [dbo].[Incidents] " +
                         "([IncidentId]," +
                         "[DepartmentType]," +
                         "[IncidentCategory]," +
                         "[RegionId]," +
                         "[CallNumber]," +
                         "[Phone]," +
                         "[Title]," +
                         "[ReportedBy]," +
                         "[ReportedAt]," +
                         "[Address1]," +
                         "[Address2]," +
                         "[Address3]," +
                         "[Address4]," +
                         "[Description]," +
                         "[Latitude]," +
                         "[Longitude]) " +
                         "VALUES " +
                         $"({incident.Id}, " +
                         $"'{serviceDept}', " +
                         $"'{incident.IncidentCategory}'," +
                         $"'{incident.Address.Substring(incident.Address.Length - 5)}'," +    //Use the zip code
                         $"'{incident.CallNumber}'," +
                         $"'{phoneNumber}'," +
                         $"'{incident.Title}'," +
                         $"'{incident.ReportingParty}'," +
                         $"'{incident.ReceivedTime.ToString("O")}'," +
                         $"'{addressLine1}'," +
                         $"'{addressLine2}'," +
                         $"'{addressLine3}'," +
                         $"'{addressLine4}'," +
                         $"'{incident.Description}'," +
                         $"{incident.Latitude}," +
                         $"{incident.Longitude}" +
                         ")";

            stringBulider.AppendLine(insert);
        }

        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
