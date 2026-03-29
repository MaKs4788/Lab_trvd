using LabsTRVD.DTOs.ServicesDTOs;
using LabsTRVD.Extensions;
using LabsTRVD.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabsTRVD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class IncomeController : ControllerBase
    {
        private readonly IIncomeService _incomeService;
        private Guid CurrentUserId => User.GetUserId();

        public IncomeController(IIncomeService incomeService)
        {
            _incomeService = incomeService;
        }

        // GET: api/Income
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IncomeDtoResponse>>> GetUserIncomes()
        {
            var dtos = await _incomeService.GetUserIncomesAsync(CurrentUserId);
            return Ok(dtos);
        }

        // GET: api/Income/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<IncomeDtoResponse>> GetIncome(int id)
        {
            var income = await _incomeService.GetByIdAsync(id, CurrentUserId);
            if (income == null) return NotFound();
            return Ok(income);
        }

        // POST: api/Income
        [HttpPost]
        public async Task<ActionResult> AddIncome([FromBody] IncomeDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _incomeService.AddIncomeAsync(dto, CurrentUserId);
                return CreatedAtAction(nameof(GetIncome), new { id = result.IncomeId }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message, error = "Validation Error" });
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(new
                {
                    message = "Помилка при збереженні в базі даних",
                    details = ex.InnerException?.Message ?? ex.Message,
                    error = "Database Error"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = ex.Message,
                    error = ex.GetType().Name,
                    stackTrace = ex.StackTrace
                });
            }
        }

        // PUT: api/Income/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateIncome(int id, [FromBody] IncomeDto dto)
        {
            try
            {
                var result = await _incomeService.UpdateIncomeAsync(id, dto, CurrentUserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/Income/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteIncome(int id)
        {
            try
            {
                await _incomeService.DeleteIncomeAsync(id, CurrentUserId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // GET: api/Income/total?from=...&to=...
        [HttpGet("total")]
        public async Task<ActionResult<decimal>> GetTotalForPeriod(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            var total = await _incomeService.GetTotalForPeriodAsync(CurrentUserId, from, to);
            return Ok(total);
        }

        // GET: api/Income/total/current-month
        [HttpGet("total/current-month")]
        public async Task<ActionResult<decimal>> GetTotalCurrentMonth()
        {
            var total = await _incomeService.GetTotalCurrentMonthAsync(CurrentUserId);
            return Ok(total);
        }

        // GET: api/Income/total/all
        [HttpGet("total/all")]
        public async Task<ActionResult<decimal>> GetTotalAll()
        {
            var total = await _incomeService.GetTotalAsync(CurrentUserId);
            return Ok(total);
        }
    }
}