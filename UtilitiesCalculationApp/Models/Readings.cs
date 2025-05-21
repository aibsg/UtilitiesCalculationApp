using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UtilitiesCalculationApp.Models;

public class Readings : BaseEntity
{
    [Required]
    [ForeignKey("Address")]
    public Guid AddressId { get; set; }
    
    [Required]
    public DateOnly Date { get; set; }
    
    public decimal? HotWaterSupply { get; set; } 
    
    public decimal? ColdWaterSupply { get; set; }
    
    public decimal? ElectricityNight { get; set; }

    public decimal?  ElectricityDay { get; set; }
}