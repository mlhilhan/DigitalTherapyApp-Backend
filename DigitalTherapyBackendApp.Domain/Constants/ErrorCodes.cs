using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Domain.Constants
{
    public class ErrorCodes
    {
        public const string UsernameAlreadyExists = "USERNAME_ALREADY_EXISTS";
        public const string EmailAlreadyExists = "EMAIL_ALREADY_EXISTS";
        public const string InvalidRoleId = "INVALID_ROLE_ID";
        public const string UserCreationFailed = "USER_CREATION_FAILED";
        public const string UserCreationError = "USER_CREATION_ERROR";
        public const string UserNotFound = "USER_NOT_FOUND";
        public const string UsernamePasswordIncorrect = "USERNAME_PASSWORD_INCORRECT";
        public const string RoleAssignmentFailed = "ROLE_ASSIGNMENT_FAILED";
    }
}
