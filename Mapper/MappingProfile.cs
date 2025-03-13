using AutoMapper;
using ExpenseTrackerAPI.DTOs;
using ExpenseTrackerAPI.Models;

namespace ExpenseTrackerAPI.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterDTO, User>();
            CreateMap<ExpenseRequestDTO, Expense>();
            CreateMap<Expense, ExpenseResponseDTO>();
        }
    }
}
