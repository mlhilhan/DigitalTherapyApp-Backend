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
    public class PsychologistProfileRepository : IPsychologistProfileRepository
    {
        private readonly AppDbContext _context;

        public PsychologistProfileRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PsychologistProfile>> GetAllAsync()
        {
            return await _context.PsychologistProfiles
                .Include(p => p.User)
                .Include(p => p.Institution)
                .ToListAsync();
        }

        public async Task<PsychologistProfile> GetByIdAsync(Guid id)
        {
            return await _context.PsychologistProfiles
                .Include(p => p.User)
                .Include(p => p.Institution)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PsychologistProfile> GetByUserIdAsync(Guid userId)
        {
            return await _context.PsychologistProfiles
                .Include(p => p.User)
                .Include(p => p.Institution)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<IEnumerable<PsychologistProfile>> GetByInstitutionIdAsync(Guid institutionId)
        {
            return await _context.PsychologistProfiles
                .Include(p => p.User)
                .Where(p => p.InstitutionId == institutionId)
                .ToListAsync();
        }

        public async Task<IEnumerable<PsychologistProfile>> GetIndependentPsychologistsAsync()
        {
            return await _context.PsychologistProfiles
                .Include(p => p.User)
                .Where(p => p.InstitutionId == null)
                .ToListAsync();
        }

        public async Task<PsychologistProfile> AddAsync(PsychologistProfile psychologistProfile)
        {
            await _context.PsychologistProfiles.AddAsync(psychologistProfile);
            await _context.SaveChangesAsync();
            return psychologistProfile;
        }

        public async Task<PsychologistProfile> UpdateAsync(PsychologistProfile psychologistProfile)
        {
            var existingProfile = await _context.PsychologistProfiles.FindAsync(psychologistProfile.Id);
            if (existingProfile == null)
                return null;

            _context.Entry(existingProfile).CurrentValues.SetValues(psychologistProfile);
            await _context.SaveChangesAsync();
            return existingProfile;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var profile = await _context.PsychologistProfiles.FindAsync(id);
            if (profile == null)
                return false;

            _context.PsychologistProfiles.Remove(profile);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignToInstitutionAsync(Guid psychologistId, Guid institutionId)
        {
            var psychologist = await _context.PsychologistProfiles.FindAsync(psychologistId);
            if (psychologist == null)
                return false;

            var institution = await _context.InstitutionProfiles.FindAsync(institutionId);
            if (institution == null)
                return false;

            psychologist.InstitutionId = institutionId;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFromInstitutionAsync(Guid psychologistId)
        {
            var psychologist = await _context.PsychologistProfiles.FindAsync(psychologistId);
            if (psychologist == null)
                return false;

            psychologist.InstitutionId = null;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsByUserIdAsync(Guid userId)
        {
            return await _context.PsychologistProfiles.AnyAsync(p => p.UserId == userId);
        }
    }
}
