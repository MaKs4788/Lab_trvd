using LabsTRVD.DTOs.ServicesDTOs;

namespace LabsTRVD.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDtoResponse>> GetUserCategoriesAsync(Guid currentUserId);
        Task<CategoryDtoResponse?> GetByIdAsync(int id, Guid currentUserId);
        Task<CategoryDtoResponse> AddCategoryAsync(CategoryDto categoryDto, Guid currentUserId);
        Task<CategoryDtoResponse> UpdateCategoryAsync(int id, CategoryDto categoryDto, Guid currentUserId);
        Task DeleteCategoryAsync(int id, Guid currentUserId);
        Task<bool> IsCategoryUsedAsync(int categoryId, Guid currentUserId); // перевіряємо використання тільки для поточного користувача
    }
}