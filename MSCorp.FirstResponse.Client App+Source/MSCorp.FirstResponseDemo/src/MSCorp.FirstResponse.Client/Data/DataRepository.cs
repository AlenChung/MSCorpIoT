using System.Collections.Generic;
using System.IO;
using System.Linq;
using MSCorp.FirstResponse.Client.Models;
using Newtonsoft.Json;

namespace MSCorp.FirstResponse.Client.Data
{
    public static class DataRepository
    {
        private static IList<IncidentModel> _incidentList;
        private static DeviceResponderUnit _userResponder;
        private static IList<RouteModel> _routes;
        private static IList<ResponderModel> _responderList;
        private static IList<TicketModel> _ticketList;
        private static IList<UserRole> _userRoles;
        private static IList<SuspectModel> _suspectList;

        public static IList<IncidentModel> LoadIncidentData()
        {
            return _incidentList ??
                   (_incidentList =
                       LoadData<IList<IncidentModel>>(Configuration.IncidentJsonDataFile)
                           .OrderByDescending(x => x.Priority)
                           .ThenByDescending(x => x.ReceivedTime)
                           .ToList());
        }

        public static DeviceResponderUnit GetUser()
        {
            return _userResponder ?? (_userResponder = new DeviceResponderUnit
            {
                Id = 1,
                ResponderDepartment = DepartmentType.Responder,
                Status = ResponseStatus.Available
            });
        }

        public static IList<RouteModel> LoadRoutes()
        {
            return _routes ?? (_routes = LoadData<IList<RouteModel>>(Configuration.ResponderRoutesJsonDataFile));
        }

        public static IList<ResponderModel> LoadResponderData()
        {
            return _responderList ?? (_responderList = LoadData<IList<ResponderModel>>(Configuration.ResponderJsonDataFile));
        }

        public static IList<TicketModel> LoadTicketData()
        {
            return _ticketList ?? (_ticketList = LoadData<IList<TicketModel>>(Configuration.TicketJsonDataFile));
        }

        public static IList<UserRole> LoadUserRoles()
        {
            return _userRoles ?? (_userRoles = LoadData<IList<UserRole>>(Configuration.UserRolesJsonDataFile));
        }

        public static IList<SuspectModel> LoadtSuspectData()
        {
            return _suspectList ?? (_suspectList = LoadData<List<SuspectModel>>(Configuration.SuspectJsonDataFile));
        }

        /// <summary>
        /// Loads data as type T from a json file
        /// </summary>
        private static T LoadData<T>(string dataFileName)
        {
            using (StreamReader r = File.OpenText(dataFileName))
            {
                return JsonConvert.DeserializeObject<T>(r.ReadToEnd());
            }
        }
    }
}
