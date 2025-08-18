using Models.Domain;

namespace Services.Abstractions
{
    public interface IResourceService
    {
        Task<IEnumerable<Resource>> GetAllAsync();
        Task AddAsync(string name);
        Task DeleteAsync(int id);
    }
}