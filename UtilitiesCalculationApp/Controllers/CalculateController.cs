using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using UtilitiesCalculationApp.Dtos;
using UtilitiesCalculationApp.Services.Interfaces;

namespace UtilitiesCalculationApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CalculateController : ControllerBase
{
    private readonly ICalculationService _calculationService;

    public CalculateController(ICalculationService calculationService)
    {
        _calculationService = calculationService;
    }

    [HttpPost]
    public async Task<IActionResult> Calculate([FromBody] CalculateUtilitiesRequest request)
    {
        try
        {
            var result = await _calculationService.CreateResultCalculation(request);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut]
    public async Task<IActionResult> CalculateElectricity([FromBody] CalculateUtilitiesRequest request)
    {
        try
        {
            var result = await _calculationService.SaveReadings(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
        
    }
}