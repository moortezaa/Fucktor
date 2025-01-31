using Core;

namespace Repository
{
    public interface IAppUserRepository : IGeneralRepository<AppUser, Guid>
    {
        IQueryable<AppUser> AppUserQuery { get; }

        Task<AppUser?> GetByUserNameAsync(string userName);
        Task<AppUser?> GetUserByPhoneNumberAndNationalCode(string doctorPhone, string doctorNationalCode);
        Task<List<Permission>> GetUserPermissions(Guid id);
        Task<List<AppRole>> GetUserRoles(Guid id);
    }
}
