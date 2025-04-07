using Business.Attributes;
using Business;
using Core;
using DSTemplate_UI.Interfaces;
using DSTemplate_UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.Extensions.Localization;
using Fucktor.Attributes;
using Microsoft.EntityFrameworkCore;

namespace Fucktor.Controllers
{
    public class InvoiceController(IServiceProvider serviceProvider, InvoiceManager invoiceManager, IDSTableManager dsTableManager, IStringLocalizer<UserController> localizer, IDSSelectManager dsSelectManager, ItemManager itemManager)
        : BaseController(serviceProvider), IDSTableController, IDSSelectController
    {
        private readonly InvoiceManager _invoiceManager = invoiceManager;
        private readonly ItemManager _itemManager = itemManager;
        private readonly IDSTableManager _dsTableManager = dsTableManager;
        private readonly IDSSelectManager _dsSelectManager = dsSelectManager;
        private readonly IStringLocalizer<UserController> _localizer = localizer;

        [Permission("GetInvoiceTable", true)]
        public async Task<JsonResult> DSGetTableData(string tableName, string sortPropertyName, bool? sortDesending, string filters, int page = 1, int rowsPerPage = 10, string routeValues = null)
        {
            if (tableName == "index")
            {
                var invoices = _invoiceManager.InvoiceQuery;

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
            else if (tableName == "edit-invoice-items")
            {

            }
            return Json("invalid table name");
        }

        [Permission("GetInvoiceTable", true)]
        public async Task<JsonResult> DSGetTableDataCount(string tableName, string filters, string routeValues = null)
        {
            if (tableName == "index")
            {

                var invoices = _invoiceManager.InvoiceQuery;

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

        public async Task<JsonResult> DSGetSelectData(string selectName, string filter, string routeValues = null)
        {
            if (selectName == "buyer-select")
            {
                var filtered = await _appUserManager.AppUserQuery.Where(u => u.Name.Contains(filter) || u.LastName.Contains(filter) || u.PhoneNumber.Contains(filter)).ToListAsync();
                return await _dsSelectManager.Json(selectName, filtered, nameof(AppUser.Id), nameof(AppUser.DisplayName));
            }
            else if (selectName == "main-invoice-select")
            {
                var filtered = await _invoiceManager.InvoiceQuery.Where(i => i.SellerId == CurrentUser.Id && (i.Number.ToString().Contains(filter)
                || i.Buyer.Name.Contains(filter)
                || i.Buyer.LastName.Contains(filter)
                || i.Buyer.PhoneNumber.Contains(filter)
                )).ToListAsync();
                return await _dsSelectManager.Json(selectName, filtered, nameof(Invoice.Id), nameof(Invoice.Number));
            }
            else if (selectName == "item-select")
            {
                var filtered = await _itemManager.ItemQuery.Where(i => i.Sellers.Any(s => s.UserId == CurrentUser.Id) && i.Name.Contains(filter)).ToListAsync();
                return await _dsSelectManager.Json(selectName, filtered, nameof(Item.Id), nameof(Item.Name));
            }
            else
            {
                return Json("Invalid select name.");
            }
        }

        [Dashboard("file-invoice", Order = 1)]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Detail(Guid id)
        {
            return View(await _invoiceManager.GetInvoiceDetails(id));
        }

        [HttpGet]
        [Dashboard("file-invoice", Order = 1, ParentAction = nameof(Index))]
        public IActionResult Edit()
        {
            return View(new Invoice());
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Invoice model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _invoiceManager.UpdateInvoice(model);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }

            return RedirectToAction(nameof(Detail), new { model.Id });
        }
    }
}
