using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Application.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(Guid userId, string username, string role);
        string GenerateRefreshToken(Guid userId);
    }
}
