using LabsTRVD.DTOs;
using LabsTRVD.DTOs.ServicesDTOs;
using LabsTRVD.Extensions;
using LabsTRVD.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabsTRVD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BudgetController : ControllerBase
    {
        private readonly IBudgetService _budgetService;
        private Guid CurrentUserId => User.GetUserId();

        public BudgetController(IBudgetService budgetService)
        {
            _budgetService = budgetService;
        }

        // POST: api/Budget
        [HttpPost]
        public async Task<IActionResult> SetBudget([FromBody] BudgetDto dto)
        {
            try
            {
                // Використовуємо CurrentUserId замість dto.UserId
                await _budgetService.SetBudgetAsync(CurrentUserId, dto.Month, dto.Year, dto.MonthlyLimit);
                return Ok(new { message = "Бюджет встановлено", month = dto.Month, year = dto.Year });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/Budget/summary?month=3&year=2026
        [HttpGet("summary")]
        public async Task<IActionResult> GetBudgetSummary([FromQuery] int month, [FromQuery] int year)
        {
            try
            {
                var summary = new BudgetSummaryDto
                {
                    Limit = await _budgetService.GetMonthlyLimitAsync(CurrentUserId, month, year),
                    Used = await _budgetService.GetUsedAmountAsync(CurrentUserId, month, year),
                    Remaining = await _budgetService.GetRemainingBudgetAsync(CurrentUserId, month, year),
                    UsagePercentage = await _budgetService.GetUsagePercentageAsync(CurrentUserId, month, year),
                    Exceeded = await _budgetService.IsBudgetExceeded(CurrentUserId, month, year)
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}