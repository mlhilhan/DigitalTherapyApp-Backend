using DigitalTherapyBackendApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Domain.Interfaces
{
    public interface ISpecialtyRepository
    {
        Task<Specialty> GetByIdAsync(Guid id);
        Task<Specialty> GetByNameAsync(string name);
        Task<List<Specialty>> GetAllAsync();
        Task<Specialty> AddAsync(Specialty specialty);
        Task<Specialty> UpdateAsync(Specialty specialty);
        Task<bool> DeleteAsync(Guid id);
    }
}