using Business;
using Core;
using DSTemplate_UI.Interfaces;
using DSTemplate_UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Fucktor.Controllers
{
    public class GatewayAccountController : BaseController, IDSTableController
    {
        private readonly GatewayAccountManager _gatewayAccountManager;
        private readonly IDSTableManager _dsTableManager;
        public GatewayAccountController(IServiceProvider serviceProvider, GatewayAccountManager gatewayAccountManager, IDSTableManager dsTableManager) : base(serviceProvider)
        {
            _gatewayAccountManager = gatewayAccountManager;
            _dsTableManager = dsTableManager;
        }

        public async Task<JsonResult> DSGetTableData(string tableName, string sortPropertyName, bool? sortDesending, string filters, int page = 1, int rowsPerPage = 10, string routeValues = null)
        {
            if (tableName == "gateway-accounts")
            {
                var gatewayAccounts = _gatewayAccountManager.GatewayAccountQuery;
                var routeValuesParsed = JsonConvert.DeserializeObject<List<KeyValuePair<string, object>>>(routeValues);
                var userId = (string?)routeValuesParsed?.Where(x=>x.Key == "userId").FirstOrDefault().Value;
                if (userId == null)
                {
                    return Json("user most not be null!");
                }
                gatewayAccounts = gatewayAccounts.Where(g=>g.UserId == new Guid(userId));

                gatewayAccounts = await _dsTableManager.DoSFP(gatewayAccounts, sortPropertyName, sortDesending, filters, page, rowsPerPage);

                var rows = new List<string>();
                var row = 0;
                foreach (var user in gatewayAccounts)
                {
                    row++;
                    ViewData["Row"] = row;
                    rows.Add(await _dsTableManager.RenderRow(user, ViewData, customRowView: "Gateway/IndexRow"));
                }
                return await _dsTableManager.Json(rows, tableName);
            }
            return Json("invalid table name");
        }

        public async Task<JsonResult> DSGetTableDataCount(string tableName, string filters, string routeValues = null)
        {
            if (tableName == "gateway-accounts")
            {
                var gatewayAccounts = _gatewayAccountManager.GatewayAccountQuery;
                var routeValuesParsed = JsonConvert.DeserializeObject<List<KeyValuePair<string, object>>>(routeValues);
                var userId = (string?)routeValuesParsed?.Where(x => x.Key == "userId").FirstOrDefault().Value;
                if (userId == null)
                {
                    return Json("user most not be null!");
                }
                gatewayAccounts = gatewayAccounts.Where(g => g.UserId == new Guid(userId));
                var count = await _dsTableManager.CountData(gatewayAccounts, filters);
                return await _dsTableManager.Json(count, tableName);
            }
            return await _dsTableManager.Json(0, tableName);
        }

        [HttpPost]
        public async Task<JsonResult> UpdateGatewayAccount(GatewayAccount model)
        {
            var result = await _gatewayAccountManager.UpdateGateway(model);
            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> DeleteGatewayAccount(Guid id)
        {
            var result = await _gatewayAccountManager.DeleteGateway(id);
            return Json(result);
        }
    }
}
