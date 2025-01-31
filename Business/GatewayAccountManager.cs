using Business.DTO;
using Core;
using Microsoft.Extensions.Configuration;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class GatewayAccountManager
    {
        private readonly IGatewayAccountRepository _gatewayAccountRepository;
        public IQueryable<GatewayAccount> GatewayAccountQuery;

        public GatewayAccountManager(IGatewayAccountRepository gatewayAccountRepository)
        {
            _gatewayAccountRepository = gatewayAccountRepository;
            GatewayAccountQuery = _gatewayAccountRepository.GatewayAccountQuery;
        }


        public async Task<GatewayResult> AddGateway(GatewayAccount gatewayAccount)
        {
            _gatewayAccountRepository.Add(gatewayAccount);
            var result = await _gatewayAccountRepository.SaveChangesAsync();
            if (result == 1)
            {
                return new GatewayResult()
                {
                    Succeeded = true,
                    Message = "Gateway account added"
                };
            }
            return new GatewayResult()
            {
                Succeeded = false,
                Message = "Failed to add gateway account"
            };
        }

        public async Task<GatewayResult> UpdateGateway(GatewayAccount gatewayAccount)
        {
            _gatewayAccountRepository.Update(gatewayAccount);
            var result = await _gatewayAccountRepository.SaveChangesAsync();
            if (result == 1)
            {
                return new GatewayResult()
                {
                    Succeeded = true,
                    Message = "Gateway account updated"
                };
            }
            return new GatewayResult()
            {
                Succeeded = false,
                Message = "Failed to update gateway account"
            };
        }

        public async Task<GatewayResult> DeleteGateway(Guid gatewayAccountId)
        {
            var gatewayAccount = await _gatewayAccountRepository.GetByIdAsync(gatewayAccountId);
            if (gatewayAccount == null)
            {
                throw new ArgumentNullException(nameof(gatewayAccountId), "gateway account not found");
            }
            _gatewayAccountRepository.Remove(gatewayAccount);
            var result = await _gatewayAccountRepository.SaveChangesAsync();
            if (result == 1)
            {
                return new GatewayResult()
                {
                    Succeeded = true,
                    Message = "Gateway account removed"
                };
            }
            return new GatewayResult()
            {
                Succeeded = false,
                Message = "Failed to remove gateway account"
            };
        }
    }
}
