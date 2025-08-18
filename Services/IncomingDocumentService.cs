using Data;
using Models.Domain;
using Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Services.Abstractions;

namespace Services
{
    public class IncomingDocumentService : IIncomingDocumentService
    {
        private readonly ApplicationDbContext _context;

        public IncomingDocumentService(ApplicationDbContext context)
        {
            _context = context;
        }

        private static DateTime ToUtc(DateTime dt)
        {
            if (dt.Kind == DateTimeKind.Utc) return dt;
            if (dt.Kind == DateTimeKind.Local) return dt.ToUniversalTime();
            return DateTime.SpecifyKind(dt, DateTimeKind.Local).ToUniversalTime();
        }

        public async Task<IncomingDocumentViewModel> GetFilteredDocumentsAsync(
            DateTime? startDate,
            DateTime? endDate,
            List<int> documentNumbers,
            List<int> resourceIds,
            List<int> unitIds)
        {
            IQueryable<IncomingDocument> query = _context.IncomingDocuments
                .Include(d => d.IncomingResources)
                    .ThenInclude(ir => ir.Resource)
                .Include(d => d.IncomingResources)
                    .ThenInclude(ir => ir.UnitOfMeasurement)
                .OrderByDescending(d => d.Date);

            if (documentNumbers?.Any() == true)
                query = query.Where(d => documentNumbers.Contains(d.Id));

            if (resourceIds?.Any() == true)
                query = query.Where(d => d.IncomingResources.Any(ir => resourceIds.Contains(ir.ResourceId)));

            if (unitIds?.Any() == true)
                query = query.Where(d => d.IncomingResources.Any(ir => unitIds.Contains(ir.UnitOfMeasurementId)));

            if (startDate.HasValue)
                query = query.Where(d => d.Date >= ToUtc(startDate.Value.Date));

            if (endDate.HasValue)
            {
                var endUtc = ToUtc(endDate.Value.Date.AddDays(1)).AddTicks(-1);
                query = query.Where(d => d.Date <= endUtc);
            }

            var viewModel = new IncomingDocumentViewModel
            {
                Documents = await query.ToListAsync(),
                StartDate = startDate,
                EndDate = endDate,
                DocumentNumbers = documentNumbers,
                ResourceIds = resourceIds,
                UnitIds = unitIds,

                AllDocuments = new SelectList(await _context.IncomingDocuments.OrderBy(d => d.Number).ToListAsync(), "Id", "Number"),
                AllResources = new SelectList(await _context.Resources.Where(r => r.State == Models.Enums.EntityState.Active).OrderBy(r => r.Name).ToListAsync(), "Id", "Name"),
                AllUnits = new SelectList(await _context.UnitsOfMeasurement.Where(u => u.State == Models.Enums.EntityState.Active).OrderBy(u => u.Name).ToListAsync(), "Id", "Name")
            };

            return viewModel;
        }

        public async Task<bool> DocumentNumberExistsAsync(string number, int? excludeId = null)
        {
            return await _context.IncomingDocuments
                .AnyAsync(d => d.Number == number && (excludeId == null || d.Id != excludeId));
        }

        public async Task<IncomingDocument?> GetDocumentByIdAsync(int id)
        {
            return await _context.IncomingDocuments
                .Include(d => d.IncomingResources)
                    .ThenInclude(ir => ir.Resource)
                .Include(d => d.IncomingResources)
                    .ThenInclude(ir => ir.UnitOfMeasurement)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AddDocumentAsync(IncomingDocument document)
        {
            document.Date = ToUtc(document.Date);
            _context.Add(document);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDocumentAsync(IncomingDocument document)
        {
            document.Date = ToUtc(document.Date);
            _context.Update(document);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDocumentAsync(int id)
        {
            var document = await _context.IncomingDocuments.Include(d => d.IncomingResources).FirstOrDefaultAsync(d => d.Id == id);
            if (document != null)
            {
                _context.IncomingResources.RemoveRange(document.IncomingResources);
                _context.IncomingDocuments.Remove(document);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IncomingResource?> AddResourceAsync(int documentId, int resourceId, int unitId, decimal quantity)
        {
            if (quantity <= 0) return null;

            var incomingResource = new IncomingResource
            {
                IncomingDocumentId = documentId,
                ResourceId = resourceId,
                UnitOfMeasurementId = unitId,
                Quantity = quantity
            };

            _context.IncomingResources.Add(incomingResource);
            await _context.SaveChangesAsync();

            return await _context.IncomingResources
                .Include(ir => ir.Resource)
                .Include(ir => ir.UnitOfMeasurement)
                .FirstOrDefaultAsync(ir => ir.Id == incomingResource.Id);
        }

        public async Task<bool> RemoveResourceAsync(int incomingResourceId)
        {
            var item = await _context.IncomingResources.FindAsync(incomingResourceId);
            if (item == null) return false;

            _context.IncomingResources.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<List<Resource>> GetActiveResourcesAsync()
        {
            return await _context.Resources
                .Where(r => r.State == Models.Enums.EntityState.Active)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<List<UnitOfMeasurement>> GetActiveUnitsAsync()
        {
            return await _context.UnitsOfMeasurement
                .Where(u => u.State == Models.Enums.EntityState.Active)
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

    }
}
