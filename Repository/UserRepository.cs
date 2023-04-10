using DashboardAppAPI.Data;
using DashboardAppAPI.Model;

namespace DashboardAppAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IDataAccess _db;
        public UserRepository(IDataAccess db)
        {
            _db = db;
        }
        public async Task<bool> AddUser(User user)
        {
            try
            { string query = "insert into dbo.userboard(email,password,fullname) values(@Email,@Password,@FullName)";
                await _db.SaveData(query, new { Email = user.email, Password = user.password, FullName = user.fullName });
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> Login(AuthUser user)
        {
            try
            {
                string query = "select * from dbo.userboard where email = @Email and password = @Password";
                var u = await _db.GetData<User,dynamic>(query, new { Email = user.email, Password = user.password });
                if (u.Count() != 0)
                    return true;
                else return false;
            }
            catch (Exception ex) {
                return false;
            }
        }
    }
}
