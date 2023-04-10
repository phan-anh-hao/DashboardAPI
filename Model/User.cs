using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DashboardAppAPI.Model
{
    [Table("User")]
    public class User
    {
        [Key]
        public string email { get; set; }
        public string password { get; set; }
        public string fullName { get; set; }

    }
}
