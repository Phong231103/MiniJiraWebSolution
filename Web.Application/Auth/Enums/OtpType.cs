using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Application.Auth.Enums
{
    public enum OtpCodeType
    {
        FirstTimeRegistration = 1,
        NewDeviceVerification = 2,
        ForgotPassword = 3,
        ChangeEmail = 4,
        DeleteAccount = 5
    }
}
