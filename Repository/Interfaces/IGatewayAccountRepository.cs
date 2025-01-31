using Core;

namespace Repository
{
    public interface IGatewayAccountRepository : IGeneralRepository<GatewayAccount, Guid>
    {
        IQueryable<GatewayAccount> GatewayAccountQuery { get; }

        Task<List<GatewayAccount>> GetUserGatewayAccount(Guid userId);
    }
}
