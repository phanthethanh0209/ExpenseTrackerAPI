namespace ExpenseTrackerAPI.DTOs
{
    public class ListExpenseDTO
    {
        public IEnumerable<ExpenseResponseDTO> Expenses { get; set; } = new List<ExpenseResponseDTO>();
        public int? page { get; set; }
        public int? limit { get; set; }
        public int? total { get; set; }

    }
}
