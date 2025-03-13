using ExpenseTrackerAPI.Attributes;
using ExpenseTrackerAPI.DTOs;
using ExpenseTrackerAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerAPI.Controllers
{
    [JwtAuthorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _expenseService;

        public ExpenseController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateExpense([FromBody] ExpenseRequestDTO expenseDTO)
        {
            ResultDTO<ExpenseResponseDTO> result = await _expenseService.CreateExpense(expenseDTO);
            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }
            return Ok(result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense([FromRoute] int id, [FromBody] ExpenseRequestDTO expenseDTO)
        {
            ResultDTO<ExpenseResponseDTO> result = await _expenseService.UpdateExpense(id, expenseDTO);
            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }
            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetExpense([FromRoute] int id)
        {
            ResultDTO<ExpenseResponseDTO> result = await _expenseService.GetExpense(id);
            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }
            return Ok(result.Data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpenseItem([FromRoute] int id)
        {
            ResultDTO<bool> result = await _expenseService.DeleteExpense(id);
            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }
            return NoContent();
        }

        [HttpGet("ListExpenses")]
        public async Task<IActionResult> GetAllExpense([FromRoute] int page = 1, [FromRoute] int limit = 5)
        {
            ResultDTO<ListExpenseDTO> result = await _expenseService.GetAllExpense(page, limit);
            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }
            return Ok(result.Data);
        }

        [HttpGet("ListExpensesPastWeek")]
        public async Task<IActionResult> GetAllExpensePastWeek([FromQuery] int page = 1, [FromRoute] int limit = 5)
        {
            ResultDTO<ListExpenseDTO> result = await _expenseService.GetExpensesByPastWeek(page, limit);
            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }
            return Ok(result.Data);
        }


        [HttpGet("ListExpensesLast3Months")]
        public async Task<IActionResult> GetAllExpenseLast3Months([FromQuery] int page = 1, [FromQuery] int limit = 5)
        {
            ResultDTO<ListExpenseDTO> result = await _expenseService.GetExpensesByLast3Months(page, limit);
            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }
            return Ok(result.Data);
        }


        [HttpGet("ListExpensesCustom")]
        public async Task<IActionResult> GetAllExpenseCustom([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int page = 1, [FromQuery] int limit = 5)
        {
            ResultDTO<ListExpenseDTO> result = await _expenseService.GetExpensesByCustom(startDate, endDate, page, limit);
            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }
            return Ok(result.Data);
        }
    }
}
