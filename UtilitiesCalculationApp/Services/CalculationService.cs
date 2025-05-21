using Microsoft.EntityFrameworkCore;
using UtilitiesCalculationApp.Models;
using UtilitiesCalculationApp.Data;
using UtilitiesCalculationApp.Dtos;
using UtilitiesCalculationApp.Services.Interfaces;

namespace UtilitiesCalculationApp.Services;

public class ConsumptionData
{
    public decimal? ColdWater { get; set; }
    public decimal? HotWater { get; set; }
    public decimal? ElectricityDay { get; set; }
    public decimal? ElectricityNight { get; set; }
}

public class CalculationService : ICalculationService
{
    private readonly AppDbContext _context;
    
    public CalculationService(AppDbContext context)
    {
        _context = context;
    }
    
    private async Task<ConsumptionData> GetReadingDifference(CalculateUtilitiesRequest request)
    {
        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.City == request.City 
                                   && a.Street == request.Street
                                   && a.BuildingNumber == request.BuildingNumber
                                   && a.ApartmentNumber == request.ApartmentNumber);
        
        if (address == null) throw new InvalidOperationException("Адрес не найден");
        
        var previous = await _context.Readings
            .Where(r => r.AddressId == address.Id)
            .OrderByDescending(r => r.Date)
            .FirstOrDefaultAsync();
        
        if (previous == null)
        {
            return new ConsumptionData
            {
                ColdWater = request.ColdWaterSupply,
                HotWater = request.HotWaterSupply,
                ElectricityDay = request.ElectricityDay,
                ElectricityNight = request.ElectricityNight
            };
        }

        return new ConsumptionData
        {
            ColdWater = CalculateDifference(request.ColdWaterSupply, previous.ColdWaterSupply),
            HotWater = CalculateDifference(request.HotWaterSupply, previous.HotWaterSupply),
            ElectricityDay = CalculateDifference(request.ElectricityDay, previous.ElectricityDay),
            ElectricityNight = CalculateDifference(request.ElectricityNight, previous.ElectricityNight)
        };
    }

    private decimal? CalculateDifference(decimal? current, decimal? previous)
    {
        return (current, previous) switch
        {
            (null, null) => null,
            (not null, null) => current,
            (null, not null) => null,
            (not null, not null) => current.Value - previous.Value
        };
    }
    
    private decimal ElectricityAmountCalculation(int? residentsCount, ConsumptionData consumption , Dictionary<string, Variables> variables)
    {
        if (consumption.ElectricityDay == null || consumption.ElectricityNight == null) 
            return residentsCount!.Value * 
                   variables["ЭЭ"].Rate * 
                   variables["ЭЭ"].Norm!.Value;
        return consumption.ElectricityDay!.Value * 
               variables["ЭЭ день"].Rate + 
               consumption.ElectricityNight!.Value *
               variables["ЭЭ ночь"].Rate;   
    }

    private decimal ColdWaterAmountCalculation(int? residentsCount, ConsumptionData consumption , Dictionary<string, Variables> variables)
    {
        if (consumption.ColdWater == null) return residentsCount!.Value *
                                                  variables["ХВС"].Norm!.Value *
                                                  variables["ХВС"].Rate;
        return consumption.ColdWater!.Value *  variables["ХВС"].Rate;
    }

    private decimal HotWaterAmountCalculation(int? residentsCount, ConsumptionData consumption , Dictionary<string, Variables> variables)
    {
        if (consumption.HotWater == null)
            return residentsCount!.Value *
                   variables["ГВС Теплоноситель"].Norm!.Value *
                   variables["ГВС Теплоноситель"].Rate;
        return consumption.HotWater!.Value *
               variables["ГВС Теплоноситель"].Rate;
    }

    private decimal HotWaterElectricityAmountCalculation(int? residentsCount, ConsumptionData consumption , Dictionary<string, Variables> variables)
    {
        if (consumption.HotWater == null) 
            return residentsCount!.Value *
                   variables["ГВС Теплоноситель"].Norm!.Value *
                   variables["ГВС Тепловая энергия"].Norm!.Value *
                   variables["ГВС Тепловая энергия"].Rate;
        return consumption.HotWater!.Value *
               variables["ГВС Тепловая энергия"].Norm!.Value *
               variables["ГВС Тепловая энергия"].Rate;
    }
    
    public async Task<CalculationResultResponse> CreateResultCalculation(CalculateUtilitiesRequest request)
    {
        var result = new CalculationResultResponse();
        
        request.Validate();
        
        var variables = await _context.Variables
            .Where(v => new[] { "ХВС", "ГВС Теплоноситель", "ГВС Тепловая энергия", "ЭЭ день", "ЭЭ ночь", "ЭЭ" }
                .Contains(v.UtilityName))
            .ToDictionaryAsync(v => v.UtilityName);
        
        var residentsCount = request.ResidentsCount;
        var consumption =  await GetReadingDifference(request);
        
        result.HotWaterAmount = HotWaterAmountCalculation(residentsCount, consumption, variables);
        result.ColdWaterAmount = ColdWaterAmountCalculation(residentsCount, consumption, variables);
        result.HotWaterElectricityAmount = HotWaterElectricityAmountCalculation(residentsCount, consumption, variables);
        result.ElectricityAmount = ElectricityAmountCalculation(residentsCount, consumption, variables);
        result.Year = request.Year;
        result.Month = request.Month;
        
        return result;
    }
    
    public async Task<bool> HasReadingsForPeriod(Guid addressId, int year, int month)
    {
        return await _context.Readings
            .AnyAsync(r => 
                r.AddressId == addressId &&
                r.Date.Month == month &&
                r.Date.Year == year );
    }

    public async Task<Readings> SaveReadings(CalculateUtilitiesRequest request)
    {
        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.City == request.City 
                                      && a.Street == request.Street
                                      && a.BuildingNumber == request.BuildingNumber
                                      && a.ApartmentNumber == request.ApartmentNumber);
        
        if (address == null)
            throw new InvalidOperationException("Адрес не найден");
        
        if (await HasReadingsForPeriod(address.Id, request.Year, request.Month)) 
            throw new InvalidOperationException("Запись на выбранный месяц месяц есть");
        
        var reading = new Readings
        {
            AddressId = address.Id,
            Date = new DateOnly(request.Year, request.Month, 1),
            HotWaterSupply = request.HotWaterSupply,
            ColdWaterSupply = request.ColdWaterSupply,
            ElectricityDay = request.ElectricityDay,
            ElectricityNight = request.ElectricityNight
        };
        
        _context.Readings.Add(reading);
        await _context.SaveChangesAsync();
        
        if (request.ResidentsCount.HasValue)
        {
            var residents = new ResidentsNumber
            {
                AddressId = address.Id,
                Date = reading.Date,
                Residents = request.ResidentsCount.Value
            };
            
            _context.ResidentsNumbers.Add(residents);
            await _context.SaveChangesAsync();
        }
        
        return reading;
        
    }
    
}