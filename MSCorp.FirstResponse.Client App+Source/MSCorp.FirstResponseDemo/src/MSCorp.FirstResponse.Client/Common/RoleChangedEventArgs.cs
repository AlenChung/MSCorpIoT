using System;
using MSCorp.FirstResponse.Client.Models;

namespace MSCorp.FirstResponse.Client.Common
{
    public class RoleChangedEventArgs : EventArgs
    {
        public UserRole SelectedRole { get; set; }

        public RoleChangedEventArgs(UserRole selectedRole)
        {
            SelectedRole = selectedRole;
        }
    }
}