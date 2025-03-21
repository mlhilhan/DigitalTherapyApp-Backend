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
    public class PatientProfileRepository : IPatientProfileRepository
    {
        private readonly AppDbContext _context;

        public PatientProfileRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PatientProfile>> GetAllAsync()
        {
            return await _context.PatientProfiles
                .Include(p => p.User)
                .ToListAsync();
        }

        public async Task<PatientProfile> GetByIdAsync(Guid id)
        {
            return await _context.PatientProfiles
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PatientProfile> GetByUserIdAsync(Guid userId)
        {
            return await _context.PatientProfiles
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<PatientProfile> AddAsync(PatientProfile patientProfile)
        {
            await _context.PatientProfiles.AddAsync(patientProfile);
            await _context.SaveChangesAsync();
            return patientProfile;
        }

        public async Task<PatientProfile> UpdateAsync(PatientProfile patientProfile)
        {
            var existingProfile = await _context.PatientProfiles.FindAsync(patientProfile.Id);
            if (existingProfile == null)
                return null;

            _context.Entry(existingProfile).CurrentValues.SetValues(patientProfile);
            await _context.SaveChangesAsync();
            return existingProfile;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var profile = await _context.PatientProfiles.FindAsync(id);
            if (profile == null)
                return false;

            _context.PatientProfiles.Remove(profile);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsByUserIdAsync(Guid userId)
        {
            return await _context.PatientProfiles.AnyAsync(p => p.UserId == userId);
        }
    }
}
