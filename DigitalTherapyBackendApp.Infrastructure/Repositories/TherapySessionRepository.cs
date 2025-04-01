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
                .Include(s => s.Psychologist)
                .Include(s => s.Relationship)
                .Include(s => s.Messages)
                .Include(s => s.EmotionalStates)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<TherapySession>> GetByPatientIdAsync(Guid patientId)
        {
            return await _context.TherapySessions
                .Include(s => s.Psychologist)
                .Include(s => s.Relationship)
                .Where(s => s.PatientId == patientId)
                .OrderByDescending(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<TherapySession>> GetByPsychologistIdAsync(Guid psychologistId)
        {
            return await _context.TherapySessions
                .Include(s => s.Patient)
                .Include(s => s.Relationship)
                .Where(s => s.PsychologistId == psychologistId)
                .OrderByDescending(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<TherapySession>> GetActiveSessionsAsync(Guid userId)
        {
            return await _context.TherapySessions
                .Include(s => s.Patient)
                .Include(s => s.Psychologist)
                .Include(s => s.Relationship)
                .Where(s => (s.PatientId == userId || s.PsychologistId == userId) &&
                           s.Status != SessionStatus.Completed && s.Status != SessionStatus.Cancelled)
                .OrderByDescending(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<TherapySession>> GetAiSessionsByPatientIdAsync(Guid patientId)
        {
            return await _context.TherapySessions
                .Include(s => s.Messages)
                .Include(s => s.EmotionalStates)
                .Where(s => s.PatientId == patientId &&
                           s.IsAiSession &&
                           !s.IsArchived)  // Arşivlenen oturumları hariç tut
                .OrderByDescending(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<TherapySession>> GetArchivedAiSessionsByPatientIdAsync(Guid patientId)
        {
            return await _context.TherapySessions
                .Include(s => s.Messages)
                .Include(s => s.EmotionalStates)
                .Where(s => s.PatientId == patientId &&
                           s.IsAiSession &&
                           s.IsArchived)  // Sadece arşivlenen oturumları getir
                .OrderByDescending(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<TherapySession>> GetHumanSessionsByPatientIdAsync(Guid patientId)
        {
            return await _context.TherapySessions
                .Include(s => s.Psychologist)
                .Include(s => s.Relationship)
                .Include(s => s.Messages)
                .Include(s => s.EmotionalStates)
                .Where(s => s.PatientId == patientId && !s.IsAiSession)
                .OrderByDescending(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<TherapySession>> GetSessionsByRelationshipIdAsync(Guid relationshipId)
        {
            return await _context.TherapySessions
                .Include(s => s.Patient)
                .Include(s => s.Psychologist)
                .Include(s => s.Messages)
                .Where(s => s.RelationshipId == relationshipId)
                .OrderByDescending(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<TherapySession>> GetSessionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.TherapySessions
                .Include(s => s.Patient)
                .Include(s => s.Psychologist)
                .Where(s => s.StartTime >= startDate && s.StartTime <= endDate)
                .OrderByDescending(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<TherapySession>> GetCompletedSessionsAsync(Guid userId, bool isPatient)
        {
            if (isPatient)
            {
                return await _context.TherapySessions
                    .Include(s => s.Psychologist)
                    .Include(s => s.Messages)
                    .Where(s => s.PatientId == userId && s.Status == SessionStatus.Completed)
                    .OrderByDescending(s => s.StartTime)
                    .ToListAsync();
            }
            else
            {
                return await _context.TherapySessions
                    .Include(s => s.Patient)
                    .Include(s => s.Messages)
                    .Where(s => s.PsychologistId == userId && s.Status == SessionStatus.Completed)
                    .OrderByDescending(s => s.StartTime)
                    .ToListAsync();
            }
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
                    .CountAsync(s => s.PsychologistId == userId &&
                              s.StartTime >= startDate &&
                              s.StartTime <= endDate);
            }
        }

        public async Task<TherapySession> GetLastSessionAsync(Guid patientId, bool isAiSession)
        {
            return await _context.TherapySessions
                .Include(s => s.Messages)
                .Include(s => s.EmotionalStates)
                .Where(s => s.PatientId == patientId && s.IsAiSession == isAiSession)
                .OrderByDescending(s => s.StartTime)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateSessionStatusAsync(Guid id, SessionStatus status)
        {
            var session = await _context.TherapySessions.FindAsync(id);
            if (session == null)
                return false;

            session.Status = status;

            if (status == SessionStatus.Completed && session.EndTime == null)
            {
                session.EndTime = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TherapySession> GetActiveAiSessionAsync(Guid patientId)
        {
            return await _context.TherapySessions
                .Include(s => s.Messages)
                .Where(s => s.PatientId == patientId &&
                       s.IsAiSession &&
                       s.IsActive &&
                       s.Status == SessionStatus.InProgress)
                .OrderByDescending(s => s.StartTime)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CloseAiSessionAsync(Guid sessionId)
        {
            var session = await _context.TherapySessions.FindAsync(sessionId);
            if (session == null || !session.IsAiSession)
                return false;

            session.IsActive = false;
            session.Status = SessionStatus.Completed;
            session.EndTime = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TherapySession> CreateAiSessionAsync(Guid patientId)
        {
            var newSession = new TherapySession
            {
                Id = Guid.NewGuid(),
                PatientId = patientId,
                IsAiSession = true,
                StartTime = DateTime.UtcNow,
                Status = SessionStatus.InProgress,
                Type = SessionType.Text,
                IsActive = true,
                MeetingLink = ""
            };

            await _context.TherapySessions.AddAsync(newSession);
            await _context.SaveChangesAsync();

            return newSession;
        }
    }
}