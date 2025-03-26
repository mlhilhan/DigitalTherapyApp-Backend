using DigitalTherapyBackendApp.Domain.Entities;
using DigitalTherapyBackendApp.Domain.Interfaces;
using DigitalTherapyBackendApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Infrastructure.Repositories
{
    public class SpecialtyRepository : ISpecialtyRepository
    {
        private readonly AppDbContext _context;

        public SpecialtyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Specialty> GetByIdAsync(Guid id)
        {
            return await _context.Specialties.FindAsync(id);
        }

        public async Task<Specialty> GetByNameAsync(string name)
        {
            return await _context.Specialties
                .FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower());
        }

        public async Task<List<Specialty>> GetAllAsync()
        {
            return await _context.Specialties.ToListAsync();
        }

        public async Task<Specialty> AddAsync(Specialty specialty)
        {
            _context.Specialties.Add(specialty);
            await _context.SaveChangesAsync();
            return specialty;
        }

        public async Task<Specialty> UpdateAsync(Specialty specialty)
        {
            _context.Entry(specialty).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return specialty;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var specialty = await _context.Specialties.FindAsync(id);
            if (specialty == null)
                return false;

            _context.Specialties.Remove(specialty);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}