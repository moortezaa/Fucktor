using Business.Attributes;
using Business;
using Core;
using DSTemplate_UI.Interfaces;
using DSTemplate_UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.Extensions.Localization;

namespace Fucktor.Controllers
{
    public class InvoiceController(IServiceProvider serviceProvider, InvoiceManager invoiceManager, IDSTableManager dsTableManager, IStringLocalizer<UserController> localizer) : BaseController(serviceProvider), IDSTableController
    {
        private readonly InvoiceManager _invoiceManager = invoiceManager;
        private readonly IDSTableManager _dsTableManager = dsTableManager;
        private readonly IStringLocalizer<UserController> _localizer = localizer;

        [Permission("GetUserTables", true)]
        public async Task<JsonResult> DSGetTableData(string tableName, string sortPropertyName, bool? sortDesending, string filters, int page = 1, int rowsPerPage = 10, string routeValues = null)
        {
            if (tableName == "index")
            {
                var invoices = await _invoiceManager.GetInvoiceQuery();

                if (!string.IsNullOrEmpty(filters))
                {
                    var filtersObjs = JsonConvert.DeserializeObject<List<DSFilterViewModel>>(filters);
                    if (filtersObjs != null && filtersObjs.Any(f => f.PropertyName == nameof(Invoice.Seller)))
                    {
                        var sellerFilter = filtersObjs.First(f => f.PropertyName == nameof(Invoice.Seller));
                        invoices = invoices.Where(i => i.Seller.Name.Contains(sellerFilter.FilterTerm) && i.Seller.LastName.Contains(sellerFilter.FilterTerm));
                    }
                    if (filtersObjs != null && filtersObjs.Any(f => f.PropertyName == nameof(Invoice.Buyer)))
                    {
                        var sellerFilter = filtersObjs.First(f => f.PropertyName == nameof(Invoice.Buyer));
                        invoices = invoices.Where(i => i.Buyer.Name.Contains(sellerFilter.FilterTerm) && i.Buyer.LastName.Contains(sellerFilter.FilterTerm));
                    }
                }

                invoices = await _dsTableManager.DoSFP(invoices, sortPropertyName, sortDesending, filters, page, rowsPerPage);

                var rows = new List<string>();
                foreach (var invoice in invoices)
                {
                    rows.Add(await _dsTableManager.RenderRow(invoice, ViewData, customRowView: "Invoice/IndexRow"));
                }
                return await _dsTableManager.Json(rows, tableName);
            }
            return Json("invalid table name");
        }

        [Permission("GetUserTables", true)]
        public async Task<JsonResult> DSGetTableDataCount(string tableName, string filters, string routeValues = null)
        {
            if (tableName == "index")
            {

                var invoices = await _invoiceManager.GetInvoiceQuery();

                if (!string.IsNullOrEmpty(filters))
                {
                    var filtersObjs = JsonConvert.DeserializeObject<List<DSFilterViewModel>>(filters);
                    if (filtersObjs != null && filtersObjs.Any(f => f.PropertyName == nameof(Invoice.Seller)))
                    {
                        var sellerFilter = filtersObjs.First(f => f.PropertyName == nameof(Invoice.Seller));
                        invoices = invoices.Where(i => i.Seller.Name.Contains(sellerFilter.FilterTerm) && i.Seller.LastName.Contains(sellerFilter.FilterTerm));
                    }
                    if (filtersObjs != null && filtersObjs.Any(f => f.PropertyName == nameof(Invoice.Buyer)))
                    {
                        var sellerFilter = filtersObjs.First(f => f.PropertyName == nameof(Invoice.Buyer));
                        invoices = invoices.Where(i => i.Buyer.Name.Contains(sellerFilter.FilterTerm) && i.Buyer.LastName.Contains(sellerFilter.FilterTerm));
                    }
                }
                var count = await _dsTableManager.CountData(invoices, filters);
                return await _dsTableManager.Json(count, tableName);
            }
            return await _dsTableManager.Json(0, tableName);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail()
        {
            return View();
        }

        public IActionResult Edit()
        {
            return View();
        }
    }
}
