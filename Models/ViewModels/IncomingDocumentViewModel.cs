using Models.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Models.ViewModels
{
    public class IncomingDocumentViewModel
    {
        public IEnumerable<IncomingDocument> Documents { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<int> DocumentNumbers { get; set; } = new List<int>();
        public List<int> ResourceIds { get; set; } = new List<int>();
        public List<int> UnitIds { get; set; } = new List<int>();

        public SelectList AllDocuments { get; set; }
        public SelectList AllResources { get; set; }
        public SelectList AllUnits { get; set; }
    }
}