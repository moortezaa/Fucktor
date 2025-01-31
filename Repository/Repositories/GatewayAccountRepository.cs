using Core;
using Data;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    internal class GatewayAccountRepository : GeneralRepository<SqlDbContext, GatewayAccount, Guid>, IGatewayAccountRepository
    {
        public GatewayAccountRepository(SqlDbContext sqlDbContext) : base(sqlDbContext)
        {
        }

        public IQueryable<GatewayAccount> GatewayAccountQuery { get => _context.GatewayAccounts; }

        public async Task<List<GatewayAccount>> GetUserGatewayAccount(Guid userId)
        {
            return await _context.GatewayAccounts.Where(g=>g.UserId == userId).ToListAsync();
        }
    }
}