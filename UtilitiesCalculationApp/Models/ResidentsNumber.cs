using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UtilitiesCalculationApp.Models;

public class ResidentsNumber : BaseEntity
{
    [Required]
    [ForeignKey("Address")]
    public Guid AddressId { get; set; }
    
    [Required]
    public DateOnly Date { get; set; }
    
    [Required]
    public int Residents  { get; set; }
    
    
}