namespace UtilitiesCalculationApp.Dtos;


public class CalculationResultResponse
{
    public int Year { get; set; }
    public int Month { get; set; }

    public decimal HotWaterAmount { get; set; }
    public decimal HotWaterElectricityAmount { get; set; }
    public decimal ColdWaterAmount { get; set; }
    public decimal ElectricityAmount { get; set; }
    public decimal TotalAmount => HotWaterAmount + ColdWaterAmount + ElectricityAmount;
}