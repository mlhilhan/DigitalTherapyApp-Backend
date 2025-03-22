using DigitalTherapyBackendApp.Domain.Entities;
using DigitalTherapyBackendApp.Domain.Interfaces;
using DigitalTherapyBackendApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Infrastructure.Repositories
{
    public class InstitutionRepository : IInstitutionRepository
    {
        private readonly AppDbContext _context;

        public InstitutionRepository(AppDbContext context)
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

        public async Task<IEnumerable<InstitutionProfile>> GetVerifiedInstitutionsAsync()
        {
            return await _context.InstitutionProfiles
                .Include(i => i.User)
                .Where(i => i.IsVerified)
                .ToListAsync();
        }

        public async Task<IEnumerable<PsychologistProfile>> GetPsychologistsAsync(Guid institutionId)
        {
            return await _context.PsychologistProfiles
                .Include(p => p.User)
                .Where(p => p.InstitutionId == institutionId)
                .ToListAsync();
        }

        public async Task<InstitutionProfile> AddAsync(InstitutionProfile institution)
        {
            await _context.InstitutionProfiles.AddAsync(institution);
            await _context.SaveChangesAsync();
            return institution;
        }

        public async Task<InstitutionProfile> UpdateAsync(InstitutionProfile institution)
        {
            var existingInstitution = await _context.InstitutionProfiles.FindAsync(institution.Id);
            if (existingInstitution == null)
                return null;

            _context.Entry(existingInstitution).CurrentValues.SetValues(institution);
            await _context.SaveChangesAsync();
            return existingInstitution;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var institution = await _context.InstitutionProfiles.FindAsync(id);
            if (institution == null)
                return false;

            _context.InstitutionProfiles.Remove(institution);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateVerificationStatusAsync(Guid id, bool isVerified)
        {
            var institution = await _context.InstitutionProfiles.FindAsync(id);
            if (institution == null)
                return false;

            institution.IsVerified = isVerified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsByUserIdAsync(Guid userId)
        {
            return await _context.InstitutionProfiles.AnyAsync(i => i.UserId == userId);
        }
    }
}