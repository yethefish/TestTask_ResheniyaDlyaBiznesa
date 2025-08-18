using Models.Domain;

namespace Models.ViewModels
{
    public class ReferenceViewModel
    {
        public IEnumerable<Resource> Resources { get; set; } = new List<Resource>();
        public IEnumerable<UnitOfMeasurement> Units { get; set; } = new List<UnitOfMeasurement>();
    }
}
