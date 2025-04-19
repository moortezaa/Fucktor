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
using Business.DTO;

namespace Fucktor.Controllers
{
    public class InvoiceController(IServiceProvider serviceProvider, InvoiceManager invoiceManager, IDSTableManager dsTableManager, IStringLocalizer<UserController> localizer, IDSSelectManager dsSelectManager, ItemManager itemManager)
        : BaseController(serviceProvider)
    {
        private readonly InvoiceManager _invoiceManager = invoiceManager;
        private readonly ItemManager _itemManager = itemManager;
        private readonly IDSTableManager _dsTableManager = dsTableManager;
        private readonly IDSSelectManager _dsSelectManager = dsSelectManager;
        private readonly IStringLocalizer<UserController> _localizer = localizer;

        [Permission("GetInvoiceTable", true)]
        public async Task<JsonResult> DSGetTableData(string tableName, string sortPropertyName, bool? sortDesending, string filters, int page = 1, int rowsPerPage = 10, Guid? invoiceId = null)
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
                var items = _invoiceManager.InvoiceItemQuery
                    .Include(ii => ii.Item)
                    .Where(i => i.InvoiceId == invoiceId);

                items = await _dsTableManager.DoSFP(items, sortPropertyName, sortDesending, filters, page, rowsPerPage);

                var rows = new List<string>();
                foreach (var item in items)
                {
                    rows.Add(await _dsTableManager.RenderRow(item, ViewData, customRowView: "Invoice/InvoiceItemEditRow"));
                }
                return await _dsTableManager.Json(rows, tableName);
            }
            return Json("invalid table name");
        }

        [Permission("GetInvoiceTable", true)]
        public async Task<JsonResult> DSGetTableDataCount(string tableName, string filters, Guid? invoiceId = null)
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
            else if (tableName == "edit-invoice-items")
            {
                var items = _invoiceManager.InvoiceItemQuery.Where(i => i.InvoiceId == invoiceId);

                var count = await _dsTableManager.CountData(items, filters);
                return await _dsTableManager.Json(count, tableName);
            }
            return await _dsTableManager.Json(0, tableName);
        }

        public async Task<JsonResult> DSGetSelectData(string selectName, string filter, string? selectedKey = null)
        {
            if (selectName == "buyer-select")
            {
                if (selectedKey != null)
                {
                    var key = new Guid(selectedKey);
                    return await _dsSelectManager.Json(selectName, await _appUserManager.AppUserQuery.Where(u => u.Id == key).ToListAsync(), nameof(AppUser.Id), nameof(AppUser.DisplayName));
                }
                var filtered = await _appUserManager.AppUserQuery.Where(u => u.Name.Contains(filter) || u.LastName.Contains(filter) || u.PhoneNumber.Contains(filter)).ToListAsync();
                return await _dsSelectManager.Json(selectName, filtered, nameof(AppUser.Id), nameof(AppUser.DisplayName));
            }
            else if (selectName == "main-invoice-select")
            {
                if (selectedKey != null)
                {
                    var key = new Guid(selectedKey);
                    return await _dsSelectManager.Json(selectName, await _invoiceManager.InvoiceQuery.Where(i => i.Id == key).ToListAsync(), nameof(Invoice.Id), nameof(Invoice.Number));
                }
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
        [Permission("ViewInvoices")]
        public IActionResult Index()
        {
            return View();
        }

        [Permission("ViewInvoiceDetails")]
        public async Task<IActionResult> Detail(Guid id)
        {
            return View(await _invoiceManager.GetInvoiceDetails(id));
        }

        [HttpGet]
        [Dashboard("file-invoice", Order = 1, ParentAction = nameof(Index))]
        [Permission("EditInvoice")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            var lastUserInvoiceNumber = await _invoiceManager.GetLastUserInvoiceNumber(CurrentUser.Id)??0;
            var invoice = new Invoice() { DateTime = DateTime.Now };
            invoice.Number = lastUserInvoiceNumber + 1;
            if (id != null)
            {
                invoice = await _invoiceManager.GetInvoiceById(id.Value);
            }
            return View(invoice);
        }

        [HttpPost]
        [Permission("EditInvoice")]
        public async Task<IActionResult> Edit(Invoice model, bool isAddItem = false, string? selectedItem = null)
        {
            model.SellerId = CurrentUser.Id;
            if (!ModelState.IsValid)
            {
                if (isAddItem)
                {
                    return Json(new { success = false, message = string.Join('\n', ModelState.Values.Select(v => string.Join('\n', v.Errors))) });
                }
                return View(model);
            }

            var result = await _invoiceManager.UpdateInvoice(model);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
                if (isAddItem)
                {
                    return Json(new { success = false, message = string.Join("\r\n", ModelState.Values.Select(v => string.Join('\n', v.Errors))) });
                }
                return View(model);
            }

            if (isAddItem)
            {
                if (selectedItem == null)
                {
                    return Json(new { success = false, message = "item is required." });
                }

                //selectedItem is eather "the name for a new Item" or "the ID for an existing item"
                Item? item = null;
                if (Guid.TryParse(selectedItem, out Guid selectedItemId))
                {
                    item = await _itemManager.GetItemById(selectedItemId);
                }
                else
                {
                    item = new Item()
                    {
                        Name = selectedItem,
                        MeasuringUnit = string.Empty
                    };
                    var itemResult = await _itemManager.CreateOrUpdateItem(item, CurrentUser);
                    if (!itemResult.Succeeded)
                    {
                        return Json(new { success = false, message = $"couldn't add item, reson:{string.Join("\r\n", itemResult.Errors)}" });
                    }
                    item = itemResult.Item;
                }
                var addItemResult = await _invoiceManager.AddItemToInvoice(model.Id, item.Id, CurrentUser.Id);
                if (!addItemResult.Succeeded)
                {
                    return Json(new { success = false, message = $"couldn't add item, reson:{string.Join("\r\n", addItemResult.Errors)}" });
                }
            }

            if (isAddItem)
            {
                return Json(new { success = true, invoiceId = model.Id });
            }
            return RedirectToAction(nameof(Detail), new { model.Id });
        }

        [HttpPost]
        [Permission("EditInvoiceItem")]
        public async Task<JsonResult> EditInvoiceItem(Guid id, string measuringUnit, decimal unitPrice, int amount, float off)
        {
            var invoiceItem = await _invoiceManager.GetInvoiceItemIncludeItemById(id);
            if (invoiceItem == null)
            {
                return Json(new { success = false, message = _localizer["item not found."].Value });
            }
            if (!string.IsNullOrWhiteSpace(measuringUnit))
            {
                invoiceItem.Item.MeasuringUnit = measuringUnit;
            }
            invoiceItem.UnitPrice = unitPrice;
            invoiceItem.Amount = amount;
            invoiceItem.Off = off;

            var userItem = await _itemManager.GetUserItem(CurrentUser.Id, invoiceItem.ItemId);
            if (userItem == null)
            {
                userItem = new UserItem()
                {
                    ItemId = invoiceItem.ItemId,
                    UserId = CurrentUser.Id,
                    DefaultUnitPrice = unitPrice,
                    DisplayName = invoiceItem.Item.Name,
                };
                await _itemManager.CreateUserItem(userItem);
            }

            var result = await _invoiceManager.UpdateInvoiceItem(invoiceItem);
            if (result.Succeeded)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false, message = _localizer["Couldn't Update item."].Value });
        }

        [HttpPost]
        [Permission("EditInvoiceItem")]
        public async Task<JsonResult> DeleteInvoiceItem(Guid invoiceItemId)
        {
            var result = await _invoiceManager.DeleteInvoiceItem(invoiceItemId); 
            if (result.Succeeded)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false, message = _localizer["Couldn't Delete item."].Value });
        }

        [HttpPost]
        [Permission("DeleteInvoice")]
        public async Task<IActionResult> DeleteInvoice(Guid id)
        {
            var result = await _invoiceManager.DeleteInvoice(id);
            if (result.Succeeded)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false, message = _localizer["Couldn't Delete invoice."].Value });
        }
    }
}
