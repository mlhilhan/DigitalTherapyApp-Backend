using DigitalTherapyBackendApp.Domain.Entities;
using DigitalTherapyBackendApp.Domain.Interfaces;
using DigitalTherapyBackendApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Infrastructure.Repositories
{
    public class InstitutionProfileRepository : IInstitutionProfileRepository
    {
        private readonly AppDbContext _context;

        public InstitutionProfileRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InstitutionProfile>> GetAllAsync()
        {
            return await _context.InstitutionProfiles
                .Include(i => i.User)
                .ToListAsync();
        }

        public async Task<InstitutionProfile> GetByIdAsync(Guid id)
        {
            return await _context.InstitutionProfiles
                .Include(i => i.User)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<InstitutionProfile> GetByUserIdAsync(Guid userId)
        {
            return await _context.InstitutionProfiles
                .Include(i => i.User)
                .FirstOrDefaultAsync(i => i.UserId == userId);
        }

        public async Task<InstitutionProfile> GetWithPsychologistsAsync(Guid id)
        {
            return await _context.InstitutionProfiles
                .Include(i => i.User)
                .Include(i => i.Psychologists)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<InstitutionProfile>> GetVerifiedInstitutionsAsync()
        {
            return await _context.InstitutionProfiles
                .Include(i => i.User)
                .Where(i => i.IsVerified)
                .ToListAsync();
        }

        public async Task<InstitutionProfile> AddAsync(InstitutionProfile institutionProfile)
        {
            await _context.InstitutionProfiles.AddAsync(institutionProfile);
            await _context.SaveChangesAsync();
            return institutionProfile;
        }

        public async Task<InstitutionProfile> UpdateAsync(InstitutionProfile institutionProfile)
        {
            var existingProfile = await _context.InstitutionProfiles.FindAsync(institutionProfile.Id);
            if (existingProfile == null)
                return null;

            _context.Entry(existingProfile).CurrentValues.SetValues(institutionProfile);
            await _context.SaveChangesAsync();
            return existingProfile;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var profile = await _context.InstitutionProfiles.FindAsync(id);
            if (profile == null)
                return false;

            _context.InstitutionProfiles.Remove(profile);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> VerifyInstitutionAsync(Guid id, bool verified)
        {
            var institution = await _context.InstitutionProfiles.FindAsync(id);
            if (institution == null)
                return false;

            institution.IsVerified = verified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsByUserIdAsync(Guid userId)
        {
            return await _context.InstitutionProfiles.AnyAsync(i => i.UserId == userId);
        }
    }
}
