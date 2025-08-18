using Microsoft.EntityFrameworkCore;
using Data;
using Models.Domain;
using Services.Abstractions;

namespace Services
{
    public class UnitService : IUnitService
    {
        private readonly ApplicationDbContext _context;

        public UnitService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UnitOfMeasurement>> GetAllAsync() =>
            await _context.UnitsOfMeasurement.AsNoTracking().ToListAsync();

        public async Task AddAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return;
            var exists = await _context.UnitsOfMeasurement.AnyAsync(u => u.Name == name);
            if (exists) return;
            _context.UnitsOfMeasurement.Add(new UnitOfMeasurement { Name = name, State = Models.Enums.EntityState.Active });
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var unit = await _context.UnitsOfMeasurement.FindAsync(id);
            if (unit == null) return;
            var inUse = await _context.IncomingResources.AnyAsync(ir => ir.UnitOfMeasurementId == id);
            if (inUse)
            {
                unit.State = Models.Enums.EntityState.Archived;
                _context.UnitsOfMeasurement.Update(unit);
            }
            else
            {
                _context.UnitsOfMeasurement.Remove(unit);
            }
            await _context.SaveChangesAsync();
        }
    }
}
