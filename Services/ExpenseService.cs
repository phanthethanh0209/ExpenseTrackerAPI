using AutoMapper;
using ExpenseTrackerAPI.DTOs;
using ExpenseTrackerAPI.Models;
using ExpenseTrackerAPI.Repositories;
using FluentValidation;

namespace ExpenseTrackerAPI.Services
{
    public interface IExpenseService
    {
        Task<ResultDTO<ExpenseResponseDTO>> CreateExpense(ExpenseRequestDTO expenseDTO);
        Task<ResultDTO<ExpenseResponseDTO>> UpdateExpense(int expenseId, ExpenseRequestDTO expenseDTO);
        Task<ResultDTO<bool>> DeleteExpense(int expenseId);
        Task<ResultDTO<ExpenseResponseDTO>> GetExpense(int expenseId);
        Task<ResultDTO<ListExpenseDTO>> GetAllExpense(int page, int limit);
        Task<ResultDTO<ListExpenseDTO>> GetExpensesByPastWeek(int page, int limit);
        Task<ResultDTO<ListExpenseDTO>> GetExpensesByPastMonth(int page, int limit);
        Task<ResultDTO<ListExpenseDTO>> GetExpensesByLast3Months(int page, int limit);
        Task<ResultDTO<ListExpenseDTO>> GetExpensesByCustom(DateTime startDate, DateTime endDate, int page, int limit);
        Task<bool> HasPermission(int expenseId, int userId);

    }
    public class ExpenseService : IExpenseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly IValidator<ExpenseRequestDTO> _validator;

        public ExpenseService(IUnitOfWork unitOfWork, IAuthService authService, IMapper mapper, IValidator<ExpenseRequestDTO> validator)
        {
            _unitOfWork = unitOfWork;
            _authService = authService;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<ResultDTO<ExpenseResponseDTO>> CreateExpense(ExpenseRequestDTO expenseDTO)
        {
            int? userId = _authService.GetUserFromClaims();
            if (userId == null)
            {
                return ResultDTO<ExpenseResponseDTO>.Fail("Create Expense item Fail");
            }

            // validation
            FluentValidation.Results.ValidationResult validationResult = _validator.Validate(expenseDTO);
            if (!validationResult.IsValid)
            {
                return ResultDTO<ExpenseResponseDTO>.Fail(validationResult.Errors.First().ErrorMessage);
            }

            // Check CategoryID exists
            Category? categoryExists = await _unitOfWork.Category.GetAsync(t => t.Id == expenseDTO.CategoryID);
            if (categoryExists == null) return ResultDTO<ExpenseResponseDTO>.Fail("Category Id not exists");

            Expense newExpense = _mapper.Map<Expense>(expenseDTO);
            newExpense.UserID = userId.Value;

            await _unitOfWork.Expense.CreateAsync(newExpense);
            await _unitOfWork.SaveChangeAsync();

            return await GetExpense(newExpense.Id);
        }

        public async Task<ResultDTO<bool>> DeleteExpense(int expenseId)
        {
            Expense? expeseItem = await _unitOfWork.Expense.GetAsync(t => t.Id == expenseId);
            if (expeseItem == null)
            {
                return ResultDTO<bool>.Fail("Delete expense item fail");
            }
            _unitOfWork.Expense.Delete(expeseItem);
            await _unitOfWork.SaveChangeAsync();
            return ResultDTO<bool>.Success(true);
        }


        public async Task<ResultDTO<ListExpenseDTO>> GetAllExpense(int page, int limit)
        {
            int? userId = _authService.GetUserFromClaims();
            if (userId == null)
            {
                return ResultDTO<ListExpenseDTO>.Fail("Get all expense fail");
            }

            IEnumerable<Expense> lstExpense = await _unitOfWork.Expense.GetAllWithPaginationAsync(u => u.UserID == userId, page, limit);
            List<ExpenseResponseDTO> lstResponse = _mapper.Map<List<ExpenseResponseDTO>>(lstExpense);

            ListExpenseDTO lstResponseDTO = new()
            {
                Expenses = lstResponse,
                page = page,
                limit = limit,
                total = lstResponse.Count,
            };

            return ResultDTO<ListExpenseDTO>.Success(lstResponseDTO);
        }

        public async Task<ResultDTO<ExpenseResponseDTO>> GetExpense(int expenseId)
        {
            Expense? expeseItem = await _unitOfWork.Expense.GetAsync(t => t.Id == expenseId);
            if (expeseItem == null)
            {
                return ResultDTO<ExpenseResponseDTO>.Fail("Expense not found");
            }

            ExpenseResponseDTO data = _mapper.Map<ExpenseResponseDTO>(expeseItem);
            return ResultDTO<ExpenseResponseDTO>.Success(data);
        }

        public async Task<bool> HasPermission(int expenseId, int userId)
        {
            Expense? expenseItem = await _unitOfWork.Expense.GetAsync(t => t.Id == expenseId && t.UserID == userId);
            return expenseItem != null ? true : false;
        }

        public async Task<ResultDTO<ExpenseResponseDTO>> UpdateExpense(int expenseId, ExpenseRequestDTO expenseDTO)
        {
            Expense? expenseItem = await _unitOfWork.Expense.GetAsync(t => t.Id == expenseId);
            if (expenseItem == null)
            {
                return ResultDTO<ExpenseResponseDTO>.Fail("Expense not found");
            }

            // validation
            FluentValidation.Results.ValidationResult validationResult = _validator.Validate(expenseDTO);
            if (!validationResult.IsValid)
            {
                return ResultDTO<ExpenseResponseDTO>.Fail(validationResult.Errors.First().ErrorMessage);
            }

            // Check CategoryID exists
            Category? categoryExists = await _unitOfWork.Category.GetAsync(t => t.Id == expenseDTO.CategoryID);
            if (categoryExists == null) return ResultDTO<ExpenseResponseDTO>.Fail("Category Id not exists");

            _mapper.Map(expenseDTO, expenseItem);
            _unitOfWork.Expense.Update(expenseItem);
            await _unitOfWork.SaveChangeAsync();

            return await GetExpense(expenseItem.Id);
        }

        public async Task<ResultDTO<ListExpenseDTO>> GetExpensesByPastWeek(int page, int limit)
        {
            int? userId = _authService.GetUserFromClaims();
            if (userId == null)
            {
                return ResultDTO<ListExpenseDTO>.Fail("Get all expense fail");
            }

            DateTime today = DateTime.Today;
            // số ngày tính từ thứ hiện tai tới t2 tuần này (VD: t6 - t2 = 5 - 1 = 4 ngày)
            int daysSinceMonday = today.DayOfWeek - DayOfWeek.Monday;
            if (daysSinceMonday < 0) daysSinceMonday += 7; // đảm bảo ngày chủ nhật cũng đúng, vì sunday là 0

            // lấy ngày htai trừ cho khoảng thgian của t2 tuần này, trừ thêm 7 ngày => t2 tuần trước
            DateTime lastMonday = today.AddDays(-daysSinceMonday - 7);
            DateTime lastSunday = lastMonday.AddDays(7); // chủ nhật tuần trc

            IEnumerable<Expense> lstExpensePastWeek = await _unitOfWork.Expense.GetAllWithPaginationAsync(t => t.UserID == userId
            && lastMonday <= t.Date && t.Date < lastSunday, page, limit);

            List<ExpenseResponseDTO> expensesDTO = _mapper.Map<List<ExpenseResponseDTO>>(lstExpensePastWeek);
            ListExpenseDTO lstExpenseDTO = new()
            {
                Expenses = expensesDTO,
                page = page,
                limit = limit,
                total = expensesDTO.Count
            };

            return ResultDTO<ListExpenseDTO>.Success(lstExpenseDTO);
        }

        public async Task<ResultDTO<ListExpenseDTO>> GetExpensesByPastMonth(int page, int limit)
        {
            int? userId = _authService.GetUserFromClaims();
            if (userId == null)
            {
                return ResultDTO<ListExpenseDTO>.Fail("Get all expense fail");
            }

            int lastMonth = DateTime.Today.Month - 1;

            IEnumerable<Expense> lstExpensePastMonth = await _unitOfWork.Expense.GetAllWithPaginationAsync(t => t.UserID == userId && t.Date.Month == lastMonth, page, limit);
            List<ExpenseResponseDTO> expensesDTO = _mapper.Map<List<ExpenseResponseDTO>>(lstExpensePastMonth);
            ListExpenseDTO lstExpenseDTO = new()
            {
                Expenses = expensesDTO,
                page = page,
                limit = limit,
                total = expensesDTO.Count
            };

            return ResultDTO<ListExpenseDTO>.Success(lstExpenseDTO);
        }

        public async Task<ResultDTO<ListExpenseDTO>> GetExpensesByLast3Months(int page, int limit)
        {
            int? userId = _authService.GetUserFromClaims();
            if (userId == null)
            {
                return ResultDTO<ListExpenseDTO>.Fail("Get all expense fail");
            }

            DateTime endDate = DateTime.Now;
            DateTime startDate = endDate.AddMonths(-3);
            IEnumerable<Expense> lstExpensePastMonth = await _unitOfWork.Expense.GetAllWithPaginationAsync(t => t.UserID == userId
                                                    && startDate <= t.Date && t.Date <= endDate, page, limit);
            List<ExpenseResponseDTO> expensesDTO = _mapper.Map<List<ExpenseResponseDTO>>(lstExpensePastMonth);
            ListExpenseDTO lstExpenseDTO = new()
            {
                Expenses = expensesDTO,
                page = page,
                limit = limit,
                total = expensesDTO.Count
            };

            return ResultDTO<ListExpenseDTO>.Success(lstExpenseDTO);
        }

        public async Task<ResultDTO<ListExpenseDTO>> GetExpensesByCustom(DateTime startDate, DateTime endDate, int page, int limit)
        {
            int? userId = _authService.GetUserFromClaims();
            if (userId == null)
            {
                return ResultDTO<ListExpenseDTO>.Fail("Get all expense fail");
            }

            IEnumerable<Expense> lstExpensePastMonth = await _unitOfWork.Expense.GetAllWithPaginationAsync(t => t.UserID == userId
                                                    && startDate <= t.Date && t.Date <= endDate, page, limit);
            List<ExpenseResponseDTO> expensesDTO = _mapper.Map<List<ExpenseResponseDTO>>(lstExpensePastMonth);
            ListExpenseDTO lstExpenseDTO = new()
            {
                Expenses = expensesDTO,
                page = page,
                limit = limit,
                total = expensesDTO.Count
            };

            return ResultDTO<ListExpenseDTO>.Success(lstExpenseDTO);
        }
    }
}
