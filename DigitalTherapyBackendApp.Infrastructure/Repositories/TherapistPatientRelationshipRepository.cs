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
    public class TherapistPatientRelationshipRepository : ITherapistPatientRelationshipRepository
    {
        private readonly AppDbContext _context;

        public TherapistPatientRelationshipRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TherapistPatientRelationship>> GetAllAsync()
        {
            return await _context.TherapistPatientRelationships
                .Include(r => r.Psychologist)
                    .ThenInclude(p => p.User)
                .Include(r => r.Patient)
                    .ThenInclude(p => p.User)
                .ToListAsync();
        }

        public async Task<TherapistPatientRelationship> GetByIdAsync(Guid id)
        {
            return await _context.TherapistPatientRelationships
                .Include(r => r.Psychologist)
                    .ThenInclude(p => p.User)
                .Include(r => r.Patient)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<TherapistPatientRelationship>> GetByPsychologistIdAsync(Guid psychologistId)
        {
            return await _context.TherapistPatientRelationships
                .Include(r => r.Patient)
                    .ThenInclude(p => p.User)
                .Where(r => r.PsychologistId == psychologistId)
                .ToListAsync();
        }

        public async Task<IEnumerable<TherapistPatientRelationship>> GetByPatientIdAsync(Guid patientId)
        {
            return await _context.TherapistPatientRelationships
                .Include(r => r.Psychologist)
                    .ThenInclude(p => p.User)
                .Where(r => r.PatientId == patientId)
                .ToListAsync();
        }

        public async Task<TherapistPatientRelationship> GetActiveBetweenPsychologistAndPatientAsync(Guid psychologistId, Guid patientId)
        {
            return await _context.TherapistPatientRelationships
                .Include(r => r.Psychologist)
                    .ThenInclude(p => p.User)
                .Include(r => r.Patient)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(r =>
                    r.PsychologistId == psychologistId &&
                    r.PatientId == patientId &&
                    r.Status == "Active" &&
                    (r.EndDate == null || r.EndDate > DateTime.UtcNow));
        }

        public async Task<IEnumerable<TherapistPatientRelationship>> GetByStatusAsync(string status)
        {
            return await _context.TherapistPatientRelationships
                .Include(r => r.Psychologist)
                    .ThenInclude(p => p.User)
                .Include(r => r.Patient)
                    .ThenInclude(p => p.User)
                .Where(r => r.Status == status)
                .ToListAsync();
        }

        public async Task<TherapistPatientRelationship> AddAsync(TherapistPatientRelationship relationship)
        {
            relationship.CreatedAt = DateTime.UtcNow;

            await _context.TherapistPatientRelationships.AddAsync(relationship);
            await _context.SaveChangesAsync();
            return relationship;
        }

        public async Task<TherapistPatientRelationship> UpdateAsync(TherapistPatientRelationship relationship)
        {
            var existingRelationship = await _context.TherapistPatientRelationships.FindAsync(relationship.Id);
            if (existingRelationship == null)
                return null;

            relationship.UpdatedAt = DateTime.UtcNow;
            _context.Entry(existingRelationship).CurrentValues.SetValues(relationship);
            await _context.SaveChangesAsync();
            return existingRelationship;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var relationship = await _context.TherapistPatientRelationships.FindAsync(id);
            if (relationship == null)
                return false;

            _context.TherapistPatientRelationships.Remove(relationship);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid psychologistId, Guid patientId, string status)
        {
            return await _context.TherapistPatientRelationships
                .AnyAsync(r =>
                    r.PsychologistId == psychologistId &&
                    r.PatientId == patientId &&
                    r.Status == status);
        }

        public async Task<bool> TerminateRelationshipAsync(Guid id, string notes = null)
        {
            var relationship = await _context.TherapistPatientRelationships.FindAsync(id);
            if (relationship == null)
                return false;

            relationship.Status = "Terminated";
            relationship.EndDate = DateTime.UtcNow;
            relationship.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(notes))
            {
                relationship.Notes = string.IsNullOrEmpty(relationship.Notes)
                    ? notes
                    : $"{relationship.Notes}\n\nTermination Notes: {notes}";
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
