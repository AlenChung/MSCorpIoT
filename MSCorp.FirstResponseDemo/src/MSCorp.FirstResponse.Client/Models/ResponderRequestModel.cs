using MSCorp.FirstResponse.Client.Controls;

namespace MSCorp.FirstResponse.Client.Models
{
    public class ResponderRequestModel
    {
        public ResponderRequestModel(DepartmentType departmentType)
        {
            DepartmentType = departmentType;
        }

        public DepartmentType DepartmentType { get; }
        public ResponderIcon ResponseUnit { get; private set; }
        public bool UnitResponding { get; private set; }
        public bool UnitResponded { get; set; }

        public void RespondToRequest(ResponderIcon unit)
        {
            if (!UnitResponding && unit.Responder.ResponderDepartment == DepartmentType && unit.Responder.Status == ResponseStatus.Available)
            {
                ResponseUnit = unit;
                UnitResponding = true;
            }
        }
    }
}
