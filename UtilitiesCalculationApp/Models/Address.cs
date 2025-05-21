using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UtilitiesCalculationApp.Models;

public class Address : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string City { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Street { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string BuildingNumber { get; set; } 
    
    [MaxLength(10)]
    public string ApartmentNumber { get; set; } 
}