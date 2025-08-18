using System.ComponentModel.DataAnnotations;

namespace Models.Domain
{
    public class IncomingDocument
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Number { get; set; }

        public DateTime Date { get; set; }

        public virtual ICollection<IncomingResource> IncomingResources { get; set; } = new List<IncomingResource>();
    }
}