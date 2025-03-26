using DigitalTherapyBackendApp.Domain.Entities;
using DigitalTherapyBackendApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Infrastructure.Repositories
{
    public class PsychologistProfileRepository : IPsychologistProfileRepository
    {
        private readonly IApplicationDbContext _context;

        public PsychologistProfileRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PsychologistProfile> GetByIdAsync(Guid id)
        {
            return await _context.PsychologistProfiles
                .Include(p => p.User)
                .Include(p => p.Institution)
                .Include(p => p.Specialties)
                .Include(p => p.AvailabilitySlots)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PsychologistProfile> GetByUserIdAsync(Guid userId)
        {
            return await _context.PsychologistProfiles
                .Include(p => p.User)
                .Include(p => p.Institution)
                .Include(p => p.Specialties)
                .Include(p => p.AvailabilitySlots)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<List<PsychologistProfile>> GetAllAsync()
        {
            return await _context.PsychologistProfiles
                .Include(p => p.User)
                .Include(p => p.Institution)
                .Include(p => p.Specialties)
                .Include(p => p.AvailabilitySlots)
                .ToListAsync();
        }

        public async Task<List<PsychologistProfile>> GetByInstitutionIdAsync(Guid institutionId)
        {
            return await _context.PsychologistProfiles
                .Include(p => p.User)
                .Include(p => p.Institution)
                .Include(p => p.Specialties)
                .Include(p => p.AvailabilitySlots)
                .Where(p => p.InstitutionId == institutionId)
                .ToListAsync();
        }

        public async Task<List<PsychologistProfile>> GetBySpecialtyAsync(string specialty)
        {
            return await _context.PsychologistProfiles
                .Include(p => p.User)
                .Include(p => p.Institution)
                .Include(p => p.Specialties)
                .Include(p => p.AvailabilitySlots)
                .Where(p => p.Specialties.Any(s => s.Name.Contains(specialty)))
                .ToListAsync();
        }

        public async Task<List<PsychologistProfile>> GetAvailablePsychologistsAsync()
        {
            return await _context.PsychologistProfiles
                .Include(p => p.User)
                .Include(p => p.Institution)
                .Include(p => p.Specialties)
                .Include(p => p.AvailabilitySlots)
                .Where(p => p.IsAvailable && p.User.IsActive)
                .ToListAsync();
        }

        public async Task<PsychologistProfile> AddAsync(PsychologistProfile profile)
        {
            _context.PsychologistProfiles.Add(profile);
            await _context.SaveChangesAsync();
            return profile;
        }

        public async Task<PsychologistProfile> UpdateAsync(PsychologistProfile profile)
        {
            _context.Entry(profile).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return profile;
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

        public async Task ClearSpecialtiesAsync(Guid psychologistId)
        {
            var profile = await _context.PsychologistProfiles
                .Include(p => p.Specialties)
                .FirstOrDefaultAsync(p => p.Id == psychologistId);

            if (profile != null)
            {
                profile.Specialties.Clear();
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateSpecialtiesAsync(Guid psychologistId, List<Specialty> specialties)
        {
            var profile = await _context.PsychologistProfiles
                .Include(p => p.Specialties)
                .FirstOrDefaultAsync(p => p.Id == psychologistId);

            if (profile != null)
            {
                foreach (var specialty in specialties)
                {
                    profile.Specialties.Add(specialty);
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAvailabilityAsync(Guid psychologistId, bool isAvailable, List<PsychologistAvailabilitySlot> availabilitySlots)
        {
            var profile = await _context.PsychologistProfiles
                .Include(p => p.AvailabilitySlots)
                .FirstOrDefaultAsync(p => p.Id == psychologistId);

            if (profile != null)
            {
                profile.IsAvailable = isAvailable;

                _context.PsychologistAvailabilitySlots.RemoveRange(profile.AvailabilitySlots);

                foreach (var slot in availabilitySlots)
                {
                    slot.PsychologistProfileId = profile.Id;
                    profile.AvailabilitySlots.Add(slot);
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}