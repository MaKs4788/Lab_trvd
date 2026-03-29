using AutoMapper;
using LabsTRVD.DTOs.AdminDTOs;
using LabsTRVD.DTOs.ServicesDTOs;
using LabsTRVD.Entities;

namespace LabsTRVD.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Income mappings
            CreateMap<Income, IncomeDtoResponse>().ReverseMap();
            CreateMap<IncomeDto, Income>()
                .ForMember(dest => dest.IncomeId, opt => opt.Ignore()); // ID генерується на сервері

            // Expense mappings
            CreateMap<Expense, ExpenseDtoResponse>().ReverseMap();
            CreateMap<ExpenseDto, Expense>()
                .ForMember(dest => dest.ExpenseId, opt => opt.Ignore()); // ID генерується на сервері

            // Category mappings
            CreateMap<Category, CategoryDtoResponse>().ReverseMap();
            CreateMap<CategoryDto, Category>()
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore()); // ID генерується на сервері

            // Budget mappings
            CreateMap<Budget, BudgetDto>().ReverseMap();

            // Dashboard summary mapping
            CreateMap<DashboardSummaryDto, DashboardSummaryDto>().ReverseMap();

            // User mappings for Admin
            CreateMap<User, UserAdminDtoResponse>();
        }
    }
}