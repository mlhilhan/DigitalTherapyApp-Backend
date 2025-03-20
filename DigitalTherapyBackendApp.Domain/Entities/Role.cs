using Microsoft.AspNetCore.Identity;

namespace DigitalTherapyBackendApp.Domain.Entities
{
    public class Role: IdentityRole<Guid>
    {
        // Bir rol, birden fazla kullanıcıya atanabilir.
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
