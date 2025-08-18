using System.ComponentModel.DataAnnotations;
using Models.Enums;

namespace Models.Domain
{
    public class Resource
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        public EntityState State { get; set; } = EntityState.Active;

        public virtual ICollection<IncomingResource> IncomingResources { get; set; } = new List<IncomingResource>();
    }
}