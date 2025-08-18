using Models.Domain;

namespace Services.Abstractions
{
    public interface IUnitService
    {
        Task<IEnumerable<UnitOfMeasurement>> GetAllAsync();
        Task AddAsync(string name);
        Task DeleteAsync(int id);
    }
}

