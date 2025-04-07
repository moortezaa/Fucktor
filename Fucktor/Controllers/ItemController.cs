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
using Fucktor.Models;
using System.Threading.Tasks;

namespace Fucktor.Controllers
{
    public class ItemController(IServiceProvider serviceProvider, IDSTableManager dsTableManager, IStringLocalizer<UserController> localizer, IDSSelectManager dsSelectManager, ItemManager itemManager)
        : BaseController(serviceProvider), IDSTableController
    {
        private readonly IDSTableManager _dsTableManager = dsTableManager;
        private readonly IDSSelectManager _dsSelectManager = dsSelectManager;
        private readonly ItemManager _itemManager = itemManager;
        private readonly IStringLocalizer<UserController> _localizer = localizer;

        [Permission("GetInvoiceTable", true)]
        public async Task<JsonResult> DSGetTableData(string tableName, string sortPropertyName, bool? sortDesending, string filters, int page = 1, int rowsPerPage = 10, string routeValues = null)
        {
            if (tableName == "index")
            {
                var items = _itemManager.ItemQuery
                    .Include(i => i.Sellers)
                    .Where(i => i.Sellers.Any(s => s.UserId == CurrentUser.Id));

                if (!string.IsNullOrEmpty(filters))
                {
                    var filtersObjs = JsonConvert.DeserializeObject<List<DSFilterViewModel>>(filters);
                    if (filtersObjs != null)
                    {
                        for (int i = 0; i < filtersObjs.Count; i++)
                        {
                            DSFilterViewModel filterObj = filtersObjs[i];
                            if (filterObj.PropertyName == nameof(ItemViewModel.DefaultUnitPrice))
                            {
                                var filter = decimal.Parse(filterObj.FilterTerm);
                                items = items.Where(i => i.Sellers.Where(s => s.UserId == CurrentUser.Id && s.DefaultUnitPrice == filter).Count() == 1);
                                filtersObjs.Remove(filterObj);
                                i--;
                            }
                            if (filterObj.PropertyName == nameof(ItemViewModel.DisplayName))
                            {
                                items = items.Where(i => i.Sellers.Where(s => s.UserId == CurrentUser.Id && s.DisplayName == filterObj.FilterTerm).Count() == 1);
                                filtersObjs.Remove(filterObj);
                                i--;
                            }
                            if (filterObj.PropertyName == nameof(ItemViewModel.StorageAmount))
                            {
                                var filter = int.Parse(filterObj.FilterTerm);
                                items = items.Where(i => i.Sellers.Where(s => s.UserId == CurrentUser.Id && s.StorageAmount == filter).Count() == 1);
                                filtersObjs.Remove(filterObj);
                                i--;
                            }
                        }
                        filters = JsonConvert.SerializeObject(filtersObjs);
                    }
                }

                items = await _dsTableManager.DoSFP(items, sortPropertyName, sortDesending, filters, page, rowsPerPage);

                var rows = new List<string>();
                foreach (var item in items)
                {
                    UserItem userItem = item.Sellers.Where(s => s.UserId == CurrentUser.Id).Single();
                    var itemViewModel = new ItemViewModel(item)
                    {
                        DefaultUnitPrice = userItem.DefaultUnitPrice,
                        DisplayName = userItem.DisplayName,
                        StorageAmount = userItem.StorageAmount
                    };
                    rows.Add(await _dsTableManager.RenderRow(itemViewModel, ViewData, customRowView: "Item/IndexRow"));
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

                var items = _itemManager.ItemQuery
                    .Where(i => i.Sellers.Any(s => s.UserId == CurrentUser.Id));


                if (!string.IsNullOrEmpty(filters))
                {
                    var filtersObjs = JsonConvert.DeserializeObject<List<DSFilterViewModel>>(filters);
                    if (filtersObjs != null)
                    {
                        for (int i = 0; i < filtersObjs.Count; i++)
                        {
                            DSFilterViewModel filterObj = filtersObjs[i];
                            if (filterObj.PropertyName == nameof(ItemViewModel.DefaultUnitPrice))
                            {
                                var filter = decimal.Parse(filterObj.FilterTerm);
                                items = items.Where(i => i.Sellers.Where(s => s.UserId == CurrentUser.Id && s.DefaultUnitPrice == filter).Count() == 1);
                                filtersObjs.Remove(filterObj);
                                i--;
                            }
                            if (filterObj.PropertyName == nameof(ItemViewModel.DisplayName))
                            {
                                items = items.Where(i => i.Sellers.Where(s => s.UserId == CurrentUser.Id && s.DisplayName == filterObj.FilterTerm).Count() == 1);
                                filtersObjs.Remove(filterObj);
                                i--;
                            }
                            if (filterObj.PropertyName == nameof(ItemViewModel.StorageAmount))
                            {
                                var filter = int.Parse(filterObj.FilterTerm);
                                items = items.Where(i => i.Sellers.Where(s => s.UserId == CurrentUser.Id && s.StorageAmount == filter).Count() == 1);
                                filtersObjs.Remove(filterObj);
                                i--;
                            }
                        }
                        filters = JsonConvert.SerializeObject(filtersObjs);
                    }
                }

                var count = await _dsTableManager.CountData(items, filters);
                return await _dsTableManager.Json(count, tableName);
            }
            return await _dsTableManager.Json(0, tableName);
        }

        [Dashboard("file-invoice", Order = 1)]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Detail(Guid id)
        {
            return View(await _itemManager.GetItemById(id));
        }

        [HttpGet]
        [Dashboard("basket-shopping", Order = 1, ParentAction = nameof(Index))]
        public async Task<IActionResult> Edit(Guid? id)
        {
            var itemViewModel = new ItemViewModel();
            if (id != null)
            {
                var item = await _itemManager.GetItemByIdIncludeSellers(id.Value);
                var userItem = item.Sellers.Where(s => s.UserId == CurrentUser.Id).Single();
                itemViewModel = new ItemViewModel(item)
                {
                    DefaultUnitPrice = userItem.DefaultUnitPrice,
                    DisplayName = userItem.DisplayName,
                    StorageAmount = userItem.StorageAmount
                };
            }
            return View(itemViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ItemViewModel model)
        {
            if (model.Id != null)
            {
                model.Sellers = await _itemManager.GetItemSellers(model.Id);
                var userItem = model.Sellers.Where(s => s.UserId == CurrentUser.Id).Single();
                userItem.DefaultUnitPrice = model.DefaultUnitPrice;
                userItem.DisplayName = model.DisplayName;
                userItem.StorageAmount = model.StorageAmount;
            }
            else
            {
                model.Sellers.Add(new UserItem()
                {
                    DefaultUnitPrice = model.DefaultUnitPrice,
                    DisplayName = model.DisplayName,
                    StorageAmount = model.StorageAmount,
                    UserId = CurrentUser.Id
                });
            }
            ModelState.MarkFieldValid(nameof(ItemViewModel.Sellers));
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _itemManager.CreateOrUpdateItem(model);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }

            return RedirectToAction(nameof(Index), new { model.Id });
        }
    }
}
