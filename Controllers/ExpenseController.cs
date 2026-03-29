using LabsTRVD.DTOs.ServicesDTOs;
using LabsTRVD.Extensions;
using LabsTRVD.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabsTRVD.Controllers
{
    [ApiController]
    [Route("api/expenses")]
    [Authorize]
    public class ExpensesController : ControllerBase
    {
        private readonly IExpenseService _expenseService;

        public ExpensesController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        private Guid CurrentUserId => User.GetUserId();

        [HttpGet]
        public async Task<IActionResult> GetMyExpenses()
        {
            var expenses = await _expenseService.GetUserExpensesAsync(CurrentUserId);
            return Ok(expenses);
        }

        [HttpGet("period")]
        public async Task<IActionResult> GetByPeriod([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var expenses = await _expenseService.GetByPeriodAsync(CurrentUserId, from, to);
            return Ok(expenses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var expense = await _expenseService.GetByIdAsync(id, CurrentUserId);
            if (expense == null)
                return NotFound(new { message = "Витрата не знайдена" });
            return Ok(expense);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ExpenseDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _expenseService.AddExpenseAsync(dto, CurrentUserId);
                return CreatedAtAction(nameof(GetById), new { id = created.ExpenseId }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ExpenseDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updated = await _expenseService.UpdateExpenseAsync(id, dto, CurrentUserId);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _expenseService.DeleteExpenseAsync(id, CurrentUserId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("total")]
        public async Task<IActionResult> GetTotal()
        {
            var total = await _expenseService.GetTotalAsync(CurrentUserId);
            return Ok(new { total });
        }

        [HttpGet("total/month")]
        public async Task<IActionResult> GetTotalCurrentMonth()
        {
            var total = await _expenseService.GetTotalCurrentMonthAsync(CurrentUserId);
            return Ok(new { total });
        }
    }
}