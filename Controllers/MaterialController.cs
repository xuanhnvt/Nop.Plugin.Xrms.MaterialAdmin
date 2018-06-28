using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Helpers;
using Nop.Web.Areas.Admin.Infrastructure.Cache;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Plugin.Xrms.MaterialAdmin.Services;
using Nop.Plugin.Xrms.MaterialAdmin.Areas.Admin.Models.MaterialGroups;
using Nop.Plugin.Xrms.MaterialAdmin.Areas.Admin.Models.Materials;
using Nop.Plugin.Xrms.MaterialAdmin.Areas.Admin.Models;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class MaterialController : BaseAdminController
    {
        #region Fields

        private readonly IMaterialService _materialService;
        private readonly IMaterialGroupService _materialGroupService;
        private readonly ISupplierService _supplierService;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        //private readonly ICopyMaterialService _copyMaterialService;
        private readonly IPdfService _pdfService;
        private readonly IExportManager _exportManager;
        private readonly IImportManager _importManager;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPermissionService _permissionService;
        private readonly IShippingService _shippingService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IDateTimeHelper _dateTimeHelper;

        #endregion

        #region Ctor

        public MaterialController(IMaterialService materialService,
            IMaterialGroupService materialGroupService,
            ISupplierService supplierService,
            IWorkContext workContext,
            ILocalizationService localizationService,
            IPictureService pictureService,
            //ICopyMaterialService copyMaterialService,
            IPdfService pdfService,
            IExportManager exportManager,
            IImportManager importManager,
            ICustomerActivityService customerActivityService,
            IPermissionService permissionService,
            IShippingService shippingService,
            IStaticCacheManager cacheManager,
            IDateTimeHelper dateTimeHelper)
        {
            this._materialService = materialService;
            this._materialGroupService = materialGroupService;
            this._supplierService = supplierService;
            this._workContext = workContext;
            this._localizationService = localizationService;
            this._pictureService = pictureService;
            //this._copyMaterialService = copyMaterialService;
            this._pdfService = pdfService;
            this._exportManager = exportManager;
            this._importManager = importManager;
            this._customerActivityService = customerActivityService;
            this._permissionService = permissionService;
            this._shippingService = shippingService;
            this._cacheManager = cacheManager;
            this._dateTimeHelper = dateTimeHelper;
        }

        #endregion

        #region Utilities

        protected virtual void PrepareAvailableMaterialGroups(MaterialDetailsPageViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var groups = _materialGroupService.GetAllMaterialGroups(showHidden: true);
            var list = groups.Select(c => new SelectListItem
            {
                Text = c.GetFormattedBreadCrumb(_materialGroupService),
                Value = c.Id.ToString()
            });
            foreach (var item in list)
                model.AvailableMaterialGroups.Add(item);
        }

        protected virtual void PrepareAvailableWarehouses(MaterialDetailsPageViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var warehouses = _shippingService.GetAllWarehouses();
            model.AvailableWarehouses.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Xrms.Admin.Catalog.Materials.Fields.Warehouse.None"),
                Value = "0"
            });

            foreach (var warehouse in warehouses)
            {
                model.AvailableWarehouses.Add(new SelectListItem
                {
                    Text = warehouse.Name,
                    Value = warehouse.Id.ToString()
                });
            }
        }

        protected virtual List<int> GetChildGroupIds(int parentGroupId)
        {
            var groupsIds = new List<int>();
            var groups = _materialGroupService.GetMaterialGroupsByParentGroupId(parentGroupId, true);
            foreach (var group in groups)
            {
                groupsIds.Add(group.Id);
                groupsIds.AddRange(GetChildGroupIds(group.Id));
            }
            return groupsIds;
        }

        #endregion

        #region Methods

        #region Material list / create / edit / delete

        // list materials
        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageMaterials))
                return AccessDeniedView();

            var model = new MaterialListPageViewModel();

            // groups
            model.AvailableMaterialGroups.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.Common.All"),
                Value = "0"
            });
            var categories = _materialGroupService.GetAllMaterialGroups(showHidden: true);
            var list = categories.Select(c => new SelectListItem
            {
                Text = c.GetFormattedBreadCrumb(_materialGroupService),
                Value = c.Id.ToString()
            });
            foreach (var item in list)
                model.AvailableMaterialGroups.Add(item);

            // warehouses
            model.AvailableWarehouses.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var wh in _shippingService.GetAllWarehouses())
                model.AvailableWarehouses.Add(new SelectListItem { Text = wh.Name, Value = wh.Id.ToString() });

            // suppliers
            model.AvailableSuppliers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var sup in _supplierService.SearchSuppliers(showHidden: true))
                model.AvailableSuppliers.Add(new SelectListItem { Text = sup.Name, Value = sup.Id.ToString() });

            return View("~/Plugins/Xrms.MaterialAdmin/Areas/Admin/Views/Material/List.cshtml", model);
        }

        [HttpPost]
        public virtual IActionResult List(DataSourceRequest command, MaterialListSearchModel model)
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageMaterials))
                return AccessDeniedKendoGridJson();

            var categoryIds = new List<int> { model.SearchMaterialGroupId };
            // include sub group
            if (model.SearchIncludeSubGroup && model.SearchMaterialGroupId > 0)
                categoryIds.AddRange(GetChildGroupIds(model.SearchMaterialGroupId));

            var materials = _materialService.SearchMaterials(
                groupIds: categoryIds,
                warehouseId: model.SearchWarehouseId,
                keywords: model.SearchMaterialName,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true
            );
            var gridModel = new DataSourceResult
            {
                Data = materials.Select(x =>
                {
                    var materialModel = x.ToListItemViewModel();

                    // group
                    materialModel.Group = x.MaterialGroup.GetFormattedBreadCrumb(_materialGroupService);

                    // picture
                    materialModel.PictureThumbnailUrl = _pictureService.GetPictureUrl(x.PictureId, 75, true);

                    return materialModel;
                }),
                Total = materials.TotalCount
            };

            return Json(gridModel);
        }

        // create material
        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageMaterials))
                return AccessDeniedView();

            var model = new MaterialDetailsPageViewModel();
            // groups
            PrepareAvailableMaterialGroups(model);
            // warehouses
            PrepareAvailableWarehouses(model);

            return View("~/Plugins/Xrms.MaterialAdmin/Areas/Admin/Views/Material/Create.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(MaterialModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageMaterials))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                //a vendor should have access only to his materials
                /*if (_workContext.CurrentVendor != null)
                {
                    model.VendorId = _workContext.CurrentVendor.Id;
                }*/

                //material
                var material = model.ToEntity();
                material.CreatedOnUtc = DateTime.UtcNow;
                material.UpdatedOnUtc = DateTime.UtcNow;
                _materialService.InsertMaterial(material);

                // quantity change history
                _materialService.AddStockQuantityHistoryEntry(material, material.StockQuantity, material.StockQuantity, material.WarehouseId,
                    _localizationService.GetResource("Xrms.Admin.Catalog.Materials.StockQuantityHistory.Messages.Edit"));

                // activity log
                _customerActivityService.InsertActivity("AddNewMaterial", _localizationService.GetResource("Xrms.ActivityLog.AddNewMaterial"), material.Name);

                SuccessNotification(_localizationService.GetResource("Xrms.Admin.Catalog.Materials.Notifications.Added"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = material.Id });
                }

                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            var viewModel = new MaterialDetailsPageViewModel();
            model.ToDetailsViewModel(viewModel);
            //viewModel.MaterialGroupInfo = model;

            // groups
            PrepareAvailableMaterialGroups(viewModel);
            // warehouses
            PrepareAvailableWarehouses(viewModel);

            return View("~/Plugins/Xrms.MaterialAdmin/Areas/Admin/Views/MaterialGroup/Create.cshtml", viewModel);
        }

        //edit material
        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageMaterials))
                return AccessDeniedView();

            var material = _materialService.GetMaterialById(id);
            if (material == null || material.Deleted)
                //No material found with the specified id
                return RedirectToAction("List");

            var viewModel = material.ToDetailsViewModel();
            // groups
            PrepareAvailableMaterialGroups(viewModel);
            // warehouses
            PrepareAvailableWarehouses(viewModel);

            return View("~/Plugins/Xrms.MaterialAdmin/Areas/Admin/Views/Material/Edit.cshtml", viewModel);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(int id, MaterialModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageMaterials))
                return AccessDeniedView();

            var material = _materialService.GetMaterialById(id);

            if (material == null || material.Deleted)
                //No material found with the specified id
                return RedirectToAction("List");

            //check if the material quantity has been changed while we were editing the material
            //and if it has been changed then we show error notification
            //and redirect on the editing page without data saving
            /*if (material.StockQuantity != model.LastStockQuantity)
            {
                ErrorNotification(_localizationService.GetResource("Admin.Catalog.Materials.Fields.StockQuantity.ChangedWarning"));
                return RedirectToAction("Edit", new { id = material.Id });
            }*/

            if (ModelState.IsValid)
            {
                //some previously used values
                var previousWarehouseId = material.WarehouseId;
                var previousStockQuantity = material.StockQuantity;

                //material
                material = model.ToEntity(material);

                material.UpdatedOnUtc = DateTime.UtcNow;
                _materialService.UpdateMaterial(material);
                
                // quantity change history
                if (previousWarehouseId != material.WarehouseId)
                {
                    //warehouse is changed 
                    //compose a message
                    var oldWarehouseMessage = string.Empty;
                    if (previousWarehouseId > 0)
                    {
                        var oldWarehouse = _shippingService.GetWarehouseById(previousWarehouseId);
                        if (oldWarehouse != null)
                            oldWarehouseMessage = string.Format(_localizationService.GetResource("Xrms.Admin.Catalog.Materials.StockQuantityHistory.Messages.EditWarehouse.Old"), oldWarehouse.Name);
                    }
                    var newWarehouseMessage = string.Empty;
                    if (material.WarehouseId > 0)
                    {
                        var newWarehouse = _shippingService.GetWarehouseById(material.WarehouseId);
                        if (newWarehouse != null)
                            newWarehouseMessage = string.Format(_localizationService.GetResource("Xrms.Admin.Catalog.Materials.StockQuantityHistory.Messages.EditWarehouse.New"), newWarehouse.Name);
                    }
                    var message = string.Format(_localizationService.GetResource("Xrms.Admin.Catalog.Materials.StockQuantityHistory.Messages.EditWarehouse"), oldWarehouseMessage, newWarehouseMessage);

                    //record history
                    _materialService.AddStockQuantityHistoryEntry(material, -previousStockQuantity, 0, previousWarehouseId, message);
                    _materialService.AddStockQuantityHistoryEntry(material, material.StockQuantity, material.StockQuantity, material.WarehouseId, message);

                }
                else
                {
                    _materialService.AddStockQuantityHistoryEntry(material, material.StockQuantity - previousStockQuantity, material.StockQuantity,
                        material.WarehouseId, _localizationService.GetResource("Xrms.Admin.Catalog.Materials.StockQuantityHistory.Messages.Edit"));
                }

                //activity log
                _customerActivityService.InsertActivity("EditMaterial", _localizationService.GetResource("Xrms.ActivityLog.EditMaterial"), material.Name);

                SuccessNotification(_localizationService.GetResource("Xrms.Admin.Catalog.Materials.Notifications.Updated"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = material.Id });
                }
                return RedirectToAction("List");
            }
            
            //If we got this far, something failed, redisplay form
            var viewModel = new MaterialDetailsPageViewModel();
            model.ToDetailsViewModel(viewModel);
            // groups
            PrepareAvailableMaterialGroups(viewModel);
            // warehouses
            PrepareAvailableWarehouses(viewModel);

            return View("~/Plugins/Xrms.MaterialAdmin/Areas/Admin/Views/MaterialGroup/Edit.cshtml", viewModel);
        }

        //delete material
        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageMaterials))
                return AccessDeniedView();

            var material = _materialService.GetMaterialById(id);
            if (material == null)
                //No material found with the specified id
                return RedirectToAction("List");


            _materialService.DeleteMaterial(material);

            //activity log
            _customerActivityService.InsertActivity("DeleteMaterial", _localizationService.GetResource("Xrms.ActivityLog.DeleteMaterial"), material.Name);

            SuccessNotification(_localizationService.GetResource("Xrms.Admin.Catalog.Materials.Notifications.Deleted"));
            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual IActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageMaterials))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                _materialService.DeleteMaterials(_materialService.GetMaterialsByIds(selectedIds.ToArray()));
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        /*public virtual IActionResult CopyMaterial(MaterialModel model)
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageMaterials))
                return AccessDeniedView();

            var copyModel = model.CopyMaterialModel;
            try
            {
                var originalMaterial = _materialService.GetMaterialById(copyModel.Id);

                //a vendor should have access only to his materials
                if (_workContext.CurrentVendor != null && originalMaterial.VendorId != _workContext.CurrentVendor.Id)
                    return RedirectToAction("List");

                var newMaterial = _copyMaterialService.CopyMaterial(originalMaterial,
                    copyModel.Name, copyModel.Published, copyModel.CopyImages);
                SuccessNotification(_localizationService.GetResource("Xrms.Admin.Catalog.Materials.Notifications.Copied"));
                return RedirectToAction("Edit", new { id = newMaterial.Id });
            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = copyModel.Id });
            }
        }
        */
        #endregion
        /*
        #region Export / Import

        [HttpPost, ActionName("List")]
        [FormValueRequired("download-catalog-pdf")]
        public virtual IActionResult DownloadCatalogAsPdf(ProductListModel model)
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                model.SearchVendorId = _workContext.CurrentVendor.Id;
            }

            var categoryIds = new List<int> { model.SearchCategoryId };
            //include subcategories
            //if (model.SearchIncludeSubCategories && model.SearchCategoryId > 0)
                //categoryIds.AddRange(GetChildCategoryIds(model.SearchCategoryId));

            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            bool? overridePublished = null;
            if (model.SearchPublishedId == 1)
                overridePublished = true;
            else if (model.SearchPublishedId == 2)
                overridePublished = false;

            var products = _productService.SearchProducts(
                categoryIds: categoryIds,
                manufacturerId: model.SearchManufacturerId,
                storeId: model.SearchStoreId,
                vendorId: model.SearchVendorId,
                warehouseId: model.SearchWarehouseId,
                productType: model.SearchProductTypeId > 0 ? (ProductType?)model.SearchProductTypeId : null,
                keywords: model.SearchProductName,
                showHidden: true,
                overridePublished: overridePublished
            );

            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    _pdfService.PrintProductsToPdf(stream, products);
                    bytes = stream.ToArray();
                }
                return File(bytes, MimeTypes.ApplicationPdf, "pdfcatalog.pdf");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("exportxml-all")]
        public virtual IActionResult ExportXmlAll(ProductListModel model)
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                model.SearchVendorId = _workContext.CurrentVendor.Id;
            }

            var categoryIds = new List<int> { model.SearchCategoryId };
            //include subcategories
            //if (model.SearchIncludeSubCategories && model.SearchCategoryId > 0)
                //categoryIds.AddRange(GetChildCategoryIds(model.SearchCategoryId));

            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            bool? overridePublished = null;
            if (model.SearchPublishedId == 1)
                overridePublished = true;
            else if (model.SearchPublishedId == 2)
                overridePublished = false;

            var products = _productService.SearchProducts(
                categoryIds: categoryIds,
                manufacturerId: model.SearchManufacturerId,
                storeId: model.SearchStoreId,
                vendorId: model.SearchVendorId,
                warehouseId: model.SearchWarehouseId,
                productType: model.SearchProductTypeId > 0 ? (ProductType?)model.SearchProductTypeId : null,
                keywords: model.SearchProductName,
                showHidden: true,
                overridePublished: overridePublished
            );

            try
            {
                var xml = _exportManager.ExportProductsToXml(products);

                return File(Encoding.UTF8.GetBytes(xml), "application/xml", "products.xml");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual IActionResult ExportXmlSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var products = new List<Product>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                products.AddRange(_productService.GetProductsByIds(ids));
            }
            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                products = products.Where(p => p.VendorId == _workContext.CurrentVendor.Id).ToList();
            }

            var xml = _exportManager.ExportProductsToXml(products);

            return File(Encoding.UTF8.GetBytes(xml), "application/xml", "products.xml");
        }
        
        [HttpPost, ActionName("List")]
        [FormValueRequired("exportexcel-all")]
        public virtual IActionResult ExportExcelAll(ProductListModel model)
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                model.SearchVendorId = _workContext.CurrentVendor.Id;
            }

            var categoryIds = new List<int> { model.SearchCategoryId };
            //include subcategories
            //if (model.SearchIncludeSubCategories && model.SearchCategoryId > 0)
                //categoryIds.AddRange(GetChildCategoryIds(model.SearchCategoryId));

            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            bool? overridePublished = null;
            if (model.SearchPublishedId == 1)
                overridePublished = true;
            else if (model.SearchPublishedId == 2)
                overridePublished = false;

            var products = _productService.SearchProducts(
                categoryIds: categoryIds,
                manufacturerId: model.SearchManufacturerId,
                storeId: model.SearchStoreId,
                vendorId: model.SearchVendorId,
                warehouseId: model.SearchWarehouseId,
                productType: model.SearchProductTypeId > 0 ? (ProductType?)model.SearchProductTypeId : null,
                keywords: model.SearchProductName,
                showHidden: true,
                overridePublished: overridePublished
            );
            try
            {
                var bytes = _exportManager.ExportProductsToXlsx(products);

                return File(bytes, MimeTypes.TextXlsx, "products.xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual IActionResult ExportExcelSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var products = new List<Product>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                products.AddRange(_productService.GetProductsByIds(ids));
            }
            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                products = products.Where(p => p.VendorId == _workContext.CurrentVendor.Id).ToList();
            }

            var bytes = _exportManager.ExportProductsToXlsx(products);

            return File(bytes, MimeTypes.TextXlsx, "products.xlsx");
        }

        [HttpPost]
        public virtual IActionResult ImportExcel(IFormFile importexcelfile)
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageProducts))
                return AccessDeniedView();
            
            //if (_workContext.CurrentVendor != null && !_vendorSettings.AllowVendorsToImportProducts)
                //a vendor can not import products
                //return AccessDeniedView();

            try
            {
                if (importexcelfile != null && importexcelfile.Length > 0)
                {
                    _importManager.ImportProductsFromXlsx(importexcelfile.OpenReadStream());
                }
                else
                {
                    ErrorNotification(_localizationService.GetResource("Admin.Common.UploadFile"));
                    return RedirectToAction("List");
                }

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Products.Imported"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        #endregion
        
        #region Low stock reports

        public virtual IActionResult LowStockReport()
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageProducts))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public virtual IActionResult LowStockReportList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            var vendorId = 0;
            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
                vendorId = _workContext.CurrentVendor.Id;

            IList<Product> products = _productService.GetLowStockProducts(vendorId);
            IList<ProductAttributeCombination> combinations = _productService.GetLowStockProductCombinations(vendorId);

            var models = new List<LowStockProductModel>();
            //products
            foreach (var product in products)
            {
                var lowStockModel = new LowStockProductModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    ManageInventoryMethod = product.ManageInventoryMethod.GetLocalizedEnum(_localizationService, _workContext.WorkingLanguage.Id),
                    StockQuantity = product.GetTotalStockQuantity(),
                    Published = product.Published
                };
                models.Add(lowStockModel);
            }
            //combinations
            foreach (var combination in combinations)
            {
                var product = combination.Product;
                var lowStockModel = new LowStockProductModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    //Attributes = _productAttributeFormatter.FormatAttributes(product, combination.AttributesXml, _workContext.CurrentCustomer, "<br />", true, true, true, false),
                    ManageInventoryMethod = product.ManageInventoryMethod.GetLocalizedEnum(_localizationService, _workContext.WorkingLanguage.Id),
                    StockQuantity = combination.StockQuantity,
                    Published = product.Published
                };
                models.Add(lowStockModel);
            }
            var gridModel = new DataSourceResult
            {
                Data = models.PagedForCommand(command),
                Total = models.Count
            };

            return Json(gridModel);
        }

        #endregion
        */
        #region Stock quantity history

        [HttpPost]
        public virtual IActionResult StockQuantityHistory(DataSourceRequest command, MaterialDetailsPageViewModel.StockQuantityHistorySearchModel searchModel)
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageMaterials))
                return AccessDeniedKendoGridJson();

            var material = _materialService.GetMaterialById(searchModel.MaterialId);
            if (material == null)
                throw new ArgumentException("No material found with the specified id");

            var stockQuantityHistory = _materialService.GetStockQuantityHistory(material, searchModel.WarehouseId, pageIndex: command.Page - 1, pageSize: command.PageSize);

            var gridModel = new DataSourceResult
            {
                Data = stockQuantityHistory.Select(historyEntry =>
                {
                    var warehouseName = _localizationService.GetResource("Admin.Catalog.Materials.Fields.Warehouse.None");
                    if (historyEntry.WarehouseId.HasValue)
                    {
                        var warehouse = _shippingService.GetWarehouseById(historyEntry.WarehouseId.Value);
                        warehouseName = warehouse != null ? warehouse.Name : "Deleted";
                    }

                    /*var attributesXml = string.Empty;
                    if (historyEntry.CombinationId.HasValue)
                    {
                        var combination = _materialAttributeService.GetMaterialAttributeCombinationById(historyEntry.CombinationId.Value);
                        attributesXml = combination == null ? string.Empty :
                            _materialAttributeFormatter.FormatAttributes(historyEntry.Material, combination.AttributesXml, _workContext.CurrentCustomer, renderGiftCardAttributes: false);
                    }*/

                    return new MaterialDetailsPageViewModel.StockQuantityHistoryListItemViewModel
                    {
                        Id = historyEntry.Id,
                        QuantityAdjustment = historyEntry.QuantityAdjustment,
                        StockQuantity = historyEntry.StockQuantity,
                        Message = historyEntry.Message,
                        //AttributeCombination = attributesXml,
                        WarehouseName = warehouseName,
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(historyEntry.CreatedOnUtc, DateTimeKind.Utc)
                    };
                }),
                Total = stockQuantityHistory.TotalCount
            };

            return Json(gridModel);
        }

        #endregion

        #endregion
    }
}