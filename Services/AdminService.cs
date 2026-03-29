using AutoMapper;
using LabsTRVD.DTOs.AdminDTOs;
using LabsTRVD.Entities;
using LabsTRVD.Interfaces.Role;


namespace LabsTRVD.Services
{
    public class AdminService : IAdminService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;

        public AdminService(IRepository<User> userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserAdminDtoResponse>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserAdminDtoResponse>>(users);
        }

        public async Task<UserAdminDtoResponse?> GetUserByIdAsync(Guid userId)
        {
            var users = await _userRepository.FindAsync(u => u.UserId == userId);
            var user = users.FirstOrDefault();

            if (user == null)
                return null;

            return _mapper.Map<UserAdminDtoResponse>(user);
        }

        public async Task<UserAdminDtoResponse> BlockUserAsync(Guid userId)
        {
            var users = await _userRepository.FindAsync(u => u.UserId == userId);
            var user = users.FirstOrDefault()
                ?? throw new Exception("Користувач не знайдений");

            user.IsBlocked = true;
            await _userRepository.UpdateAsync(user);

            return _mapper.Map<UserAdminDtoResponse>(user);
        }

        public async Task<UserAdminDtoResponse> UnblockUserAsync(Guid userId)
        {
            var users = await _userRepository.FindAsync(u => u.UserId == userId);
            var user = users.FirstOrDefault()
                ?? throw new Exception("Користувач не знайдений");

            user.IsBlocked = false;
            await _userRepository.UpdateAsync(user);

            return _mapper.Map<UserAdminDtoResponse>(user);
        }

        public async Task DeleteUserAsync(Guid userId)
        {
            var users = await _userRepository.FindAsync(u => u.UserId == userId);
            var user = users.FirstOrDefault()
                ?? throw new Exception("Користувач не знайдений");

            await _userRepository.DeleteAsync(user);
        }

        public async Task<UserAdminDtoResponse> UpdateUserRoleAsync(Guid userId, string newRole)
        {
            if (string.IsNullOrWhiteSpace(newRole))
                throw new Exception("Роль не може бути порожною");

            var allowedRoles = new[] { "User", "Admin" };
            if (!allowedRoles.Contains(newRole))
                throw new Exception("Невалідна роль. Можливі значення: User, Admin");

            var users = await _userRepository.FindAsync(u => u.UserId == userId);
            var user = users.FirstOrDefault()
                ?? throw new Exception("Користувач не знайдений");

            user.Role = newRole;
            await _userRepository.UpdateAsync(user);

            return _mapper.Map<UserAdminDtoResponse>(user);
        }
    }
}
