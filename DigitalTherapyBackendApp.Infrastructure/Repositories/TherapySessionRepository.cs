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
    public class TherapySessionRepository : ITherapySessionRepository
    {
        private readonly AppDbContext _context;

        public TherapySessionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TherapySession> GetByIdAsync(Guid id)
        {
            return await _context.TherapySessions
                .Include(s => s.Patient)
                .Include(s => s.Therapist)
                .Include(s => s.Messages)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<TherapySession>> GetByPatientIdAsync(Guid patientId)
        {
            return await _context.TherapySessions
                .Include(s => s.Therapist)
                .Where(s => s.PatientId == patientId)
                .OrderByDescending(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<TherapySession>> GetByTherapistIdAsync(Guid therapistId)
        {
            return await _context.TherapySessions
                .Include(s => s.Patient)
                .Where(s => s.TherapistId == therapistId)
                .OrderByDescending(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<TherapySession>> GetActiveSessionsAsync(Guid userId)
        {
            return await _context.TherapySessions
                .Include(s => s.Patient)
                .Include(s => s.Therapist)
                .Where(s => (s.PatientId == userId || s.TherapistId == userId) &&
                          s.Status != "Completed" && s.Status != "Cancelled")
                .OrderByDescending(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<TherapySession> AddAsync(TherapySession therapySession)
        {
            await _context.TherapySessions.AddAsync(therapySession);
            await _context.SaveChangesAsync();
            return therapySession;
        }

        public async Task UpdateAsync(TherapySession therapySession)
        {
            _context.TherapySessions.Update(therapySession);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var therapySession = await _context.TherapySessions.FindAsync(id);
            if (therapySession != null)
            {
                _context.TherapySessions.Remove(therapySession);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetSessionCountAsync(Guid userId, bool isPatient, DateTime startDate, DateTime endDate)
        {
            if (isPatient)
            {
                return await _context.TherapySessions
                    .CountAsync(s => s.PatientId == userId &&
                              s.StartTime >= startDate &&
                              s.StartTime <= endDate);
            }
            else
            {
                return await _context.TherapySessions
                    .CountAsync(s => s.TherapistId == userId &&
                              s.StartTime >= startDate &&
                              s.StartTime <= endDate);
            }
        }
    }
}