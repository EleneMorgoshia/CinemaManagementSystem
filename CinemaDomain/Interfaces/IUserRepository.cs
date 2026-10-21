using CinemaDomain.Enums;
using CinemaDomain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaDomain.Interfaces
{
    public interface IUserRepository
    {
        Task<ICollection<User>> GetUsers(CancellationToken cancellationToken);
        Task<User?> GetUserById(int userId, CancellationToken cancellationToken);

        Task<User?> GetUserByEmail(string email, CancellationToken cancellationToken);
        Task<Role?> GetUserRoleById(int id, CancellationToken cancellationToken);
        Task<Role?> GetUserRoleByEmail(string email, CancellationToken cancellationToken);
        Task AddUser(User user, CancellationToken cancellationToken);
        void DeleteUser(User user);
        void UpdateUser(User user);
       
    }
}
