using Models.Domain;
using Models.ViewModels;

namespace Services.Abstractions
{
    public interface IIncomingDocumentService
    {
        Task<IncomingDocumentViewModel> GetFilteredDocumentsAsync(
            DateTime? startDate,
            DateTime? endDate,
            List<int> documentNumbers,
            List<int> resourceIds,
            List<int> unitIds);

        Task<bool> DocumentNumberExistsAsync(string number, int? excludeId = null);

        Task<IncomingDocument?> GetDocumentByIdAsync(int id);

        Task AddDocumentAsync(IncomingDocument document);

        Task UpdateDocumentAsync(IncomingDocument document);

        Task DeleteDocumentAsync(int id);

        Task<IncomingResource?> AddResourceAsync(int documentId, int resourceId, int unitId, decimal quantity);

        Task<bool> RemoveResourceAsync(int incomingResourceId);

        Task<List<Resource>> GetActiveResourcesAsync();

        Task<List<UnitOfMeasurement>> GetActiveUnitsAsync();
    }
}