using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Domain.Entities
{
    public class User: IdentityUser<Guid>
    {
        public Guid RoleId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Role Role { get; set; }
        public virtual UserProfile Profile { get; set; }
    }
}
