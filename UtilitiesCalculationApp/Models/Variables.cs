using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UtilitiesCalculationApp.Models;

public class Variables : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string UtilityName { get; set; }  
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Rate { get; set; }        
    
    [Column(TypeName = "decimal(18,5)")]
    public decimal? Norm { get; set; }     
    
    [Required]
    [MaxLength(15)]
    public string Unit { get; set; }    
}