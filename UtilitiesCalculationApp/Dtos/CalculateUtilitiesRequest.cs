using System.ComponentModel.DataAnnotations;

namespace UtilitiesCalculationApp.Dtos;

public class CalculateUtilitiesRequest
{
    [Required(ErrorMessage = "Город обязателен")]
    public string City { get; set; }

    [Required(ErrorMessage = "Улица обязательна")]
    public string Street { get; set; }

    [Required(ErrorMessage = "Номер дома обязателен")]
    public string BuildingNumber { get; set; }

    [Required(ErrorMessage = "Номер квартиры обязателен")]
    public string ApartmentNumber { get; set; }
    
    [Required(ErrorMessage = "Год обязателен")]
    [Range(2000, 2100, ErrorMessage = "Некорректный год")]
    public int Year { get; set; }
    
    [Required(ErrorMessage = "Месяц обязателен")]
    [Range(1, 12, ErrorMessage = "Месяц должен быть от 1 до 12")]
    public int Month { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Показания не могут быть отрицательными")]
    public decimal? HotWaterSupply { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? ColdWaterSupply { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? ElectricityNight { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? ElectricityDay { get; set; }
    
    [Range(1, 20, ErrorMessage = "Количество проживающих должно быть от 1 до 20")]
    public int? ResidentsCount { get; set; }

    public void Validate()
    {
        // Проверка, что есть либо показания, либо количество проживающих
        if (HotWaterSupply == null && 
            ColdWaterSupply == null && 
            ElectricityDay == null && 
            ElectricityNight == null && 
            ResidentsCount == null)
        {
            throw new ValidationException("Должны быть указаны либо показания, либо количество проживающих");
        }

        // Проверка для воды
        if (HotWaterSupply == null && ColdWaterSupply == null && ResidentsCount == null)
        {
            throw new ValidationException("Для расчета воды укажите либо показания, либо количество проживающих");
        }

        // Проверка для электричества
        if (ElectricityDay == null && ElectricityNight == null && ResidentsCount == null)
        {
            throw new ValidationException("Для расчета электричества укажите либо показания, либо количество проживающих");
        }
    }
}