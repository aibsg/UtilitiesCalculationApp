using UtilitiesCalculationApp.Dtos;
using UtilitiesCalculationApp.Models;

namespace UtilitiesCalculationApp.Services.Interfaces;

public interface ICalculationService
{
    public Task<CalculationResultResponse> CreateResultCalculation(CalculateUtilitiesRequest request);
    public Task<Readings> SaveReadings(CalculateUtilitiesRequest request);
}