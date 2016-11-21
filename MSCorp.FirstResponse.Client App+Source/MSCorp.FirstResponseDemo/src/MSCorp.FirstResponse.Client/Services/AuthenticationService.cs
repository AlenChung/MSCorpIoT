using System;
using System.IO;
using MSCorp.FirstResponse.Client.Common;
using MSCorp.FirstResponse.Client.Models;

namespace MSCorp.FirstResponse.Client.Services
{
    public static class AuthenticationService
    {
        public static event EventHandler<RoleChangedEventArgs> RoleChanged;
        private static UserRole _authenticatedUser;
        public static void AuthenticateUser(UserRole value)
        {
            _authenticatedUser = value;
            RoleChanged?.Invoke(null, new RoleChangedEventArgs(value));
        }

        public static UserRole LoadAuthenticatedUser()
        {
            if (_authenticatedUser != null)
            {
                return _authenticatedUser;
            }

            throw new IOException("Authenticated user should be set.");
        }
    }
}
