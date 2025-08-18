using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Domain
{
    public class IncomingResource
    {
        public int Id { get; set; }

        public int IncomingDocumentId { get; set; }
        [ForeignKey("IncomingDocumentId")]
        public virtual IncomingDocument IncomingDocument { get; set; }

        public int ResourceId { get; set; }
        [ForeignKey("ResourceId")]
        public virtual Resource Resource { get; set; }

        public int UnitOfMeasurementId { get; set; }
        [ForeignKey("UnitOfMeasurementId")]
        public virtual UnitOfMeasurement UnitOfMeasurement { get; set; }

        public decimal Quantity { get; set; }
    }
}