using System.ComponentModel.DataAnnotations.Schema;

namespace skylance_backend.Models
{
    public class TicketSale
    {
        public int Id { get; set; }
        [ForeignKey("AircraftId")]
        public virtual required Aircraft Aircraft { get; set; }
        public string AircraftId { get; set; }
        public DateTime SaleDate { get; set; }
        public int Sales { get; set; }
    }
}