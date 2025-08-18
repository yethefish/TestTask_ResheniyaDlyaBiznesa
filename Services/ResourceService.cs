using Microsoft.EntityFrameworkCore;
using Data;
using Models.Domain;
using Services.Abstractions;

namespace Services
{
    public class ResourceService : IResourceService
    {
        private readonly ApplicationDbContext _context;

        public ResourceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Resource>> GetAllAsync() =>
            await _context.Resources.AsNoTracking().ToListAsync();

        public async Task AddAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return;
            var exists = await _context.Resources.AnyAsync(r => r.Name == name);
            if (exists) return;
            _context.Resources.Add(new Resource { Name = name, State = Models.Enums.EntityState.Active });
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var resource = await _context.Resources.FindAsync(id);
            if (resource == null) return;
            var inUse = await _context.IncomingResources.AnyAsync(ir => ir.ResourceId == id);
            if (inUse)
            {
                resource.State = Models.Enums.EntityState.Archived;
                _context.Resources.Update(resource);
            }
            else
            {
                _context.Resources.Remove(resource);
            }
            await _context.SaveChangesAsync();
        }
    }
}
