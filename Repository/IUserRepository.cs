using DashboardAppAPI.Model;

namespace DashboardAppAPI.Repository
{
    public interface IUserRepository
    {
        Task<bool> AddUser(User user);

        Task<bool> Login(AuthUser user);
    }
}
