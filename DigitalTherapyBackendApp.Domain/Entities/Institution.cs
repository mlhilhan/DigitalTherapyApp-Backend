using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Domain.Entities
{
    public class Institution
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string City { get; set; }
        public string? Phone { get; set; }
        public string Email { get; set; }
        public string? Website { get; set; }
        public bool IsVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<User> Psychologists { get; set; }
    }
}
