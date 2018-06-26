using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Helpers;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Plugin.Xrms.MaterialAdmin.Areas.Admin.Models.Suppliers;
using Nop.Plugin.Xrms.MaterialAdmin.Services;
using Nop.Plugin.Xrms.MaterialAdmin.Areas.Admin.Models;
using Nop.Plugin.Xrms.MaterialAdmin.Domain;
using Nop.Services.Shipping;

namespace Nop.Plugin.Xrms.MaterialAdmin.Controllers
{
    public partial class SupplierController : BaseAdminController
    {
        #region Fields

        private readonly ISupplierService _supplierService;
        private readonly IMaterialService _materialService;

        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IExportManager _exportManager;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IWorkContext _workContext;
        private readonly IImportManager _importManager;
        private readonly IShippingService _shippingService;
        private readonly IStaticCacheManager _cacheManager;
        
        #endregion
        
        #region Ctor

        public SupplierController(ISupplierService supplierService,
            IMaterialService materialService,
            ICategoryService categoryService,
            IManufacturerService manufacturerService,
            IPictureService pictureService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IExportManager exportManager,
            ICustomerActivityService customerActivityService,
            IWorkContext workContext,
            IShippingService shippingService,
            IImportManager importManager, 
            IStaticCacheManager cacheManager)
        {
            this._supplierService = supplierService;
            this._materialService = materialService;

            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._pictureService = pictureService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._exportManager = exportManager;
            this._customerActivityService = customerActivityService;
            this._workContext = workContext;
            this._importManager = importManager;
            this._cacheManager = cacheManager;
            this._shippingService = shippingService;
        }
        
        #endregion
        
        #region Utilities

        #endregion

        #region List

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageSuppliers))
                return AccessDeniedView();

            var model = new SupplierListPageViewModel();

            return View("~/Plugins/Xrms.MaterialAdmin/Areas/Admin/Views/Supplier/List.cshtml", model);
        }

        [HttpPost]
        public virtual IActionResult List(DataSourceRequest command, SupplierListSearchModel model)
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageSuppliers))
                return AccessDeniedKendoGridJson();

            var groups = _supplierService.SearchSuppliers(
                command.Page - 1, command.PageSize, model.SearchSupplierName, true);
            var gridModel = new DataSourceResult
            {
                Data = groups.Select(x =>
                {
                    var groupModel = x.ToListItemViewModel();
                    return groupModel;
                }),
                Total = groups.TotalCount
            };

            return Json(gridModel);
        }

        #endregion

        /*
        
        #region Create / Edit / Delete

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageSuppliers))
                return AccessDeniedView();

            var model = new SupplierDetailsPageViewModel();
            // set default values
            model.DisplayOrder = 1;

            return View("~/Plugins/Xrms.MaterialAdmin/Areas/Admin/Views/Supplier/Create.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(SupplierModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageSuppliers))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var group = model.ToEntity();
                group.CreatedOnUtc = DateTime.UtcNow;
                group.UpdatedOnUtc = DateTime.UtcNow;
                _supplierService.InsertSupplier(group);

                //activity log
                _customerActivityService.InsertActivity("AddNewSupplier", _localizationService.GetResource("Xrms.ActivityLog.AddNewSupplier"), group.Name);

                SuccessNotification(_localizationService.GetResource("Xrms.Admin.Catalog.Suppliers.Notifications.Created"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = group.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            var viewModel = new SupplierDetailsPageViewModel();
            model.ToDetailsViewModel(viewModel);
            //viewModel.SupplierInfo = model;

            return View("~/Plugins/Xrms.MaterialAdmin/Areas/Admin/Views/Supplier/Create.cshtml", viewModel);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageSuppliers))
                return AccessDeniedView();

            var supplier = _supplierService.GetSupplierById(id);
            if (supplier == null || supplier.Deleted) 
                //No group found with the specified id
                return RedirectToAction("List");

            var viewModel = supplier.ToDetailsViewModel();

            return View("~/Plugins/Xrms.MaterialAdmin/Areas/Admin/Views/Supplier/Edit.cshtml", viewModel);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(int id, SupplierModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageSuppliers))
                return AccessDeniedView();

            var supplier = _supplierService.GetSupplierById(id);
            if (supplier == null || supplier.Deleted)
                //No group found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var prevPictureId = supplier.PictureId;
                supplier = model.ToEntity(supplier);
                supplier.UpdatedOnUtc = DateTime.UtcNow;
                _supplierService.UpdateSupplier(supplier);

                //delete an old picture (if deleted or updated)
                if (prevPictureId > 0 && prevPictureId != supplier.PictureId)
                {
                    var prevPicture = _pictureService.GetPictureById(prevPictureId);
                    if (prevPicture != null)
                        _pictureService.DeletePicture(prevPicture);
                }

                //activity log
                _customerActivityService.InsertActivity("EditSupplier", _localizationService.GetResource("Xrms.ActivityLog.EditSupplier"), supplier.Name);

                SuccessNotification(_localizationService.GetResource("Xrms.Admin.Catalog.Suppliers.Notifications.Updated"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new {id = supplier.Id});
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            var viewModel = new SupplierDetailsPageViewModel();
            model.ToDetailsViewModel(viewModel);
            //viewModel.SupplierInfo = model;

            //If we got this far, something failed, redisplay form

            return View("~/Plugins/Xrms.MaterialAdmin/Areas/Admin/Views/Supplier/Edit.cshtml", viewModel);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageSuppliers))
                return AccessDeniedView();

            var supplier = _supplierService.GetSupplierById(id);
            if (supplier == null)
                // No group found with the specified id
                return RedirectToAction("List");

            _supplierService.DeleteSupplier(supplier);

            //activity log
            _customerActivityService.InsertActivity("DeleteSupplier", _localizationService.GetResource("Xrms.ActivityLog.DeleteSupplier"), supplier.Name);

            SuccessNotification(_localizationService.GetResource("Xrms.Admin.Catalog.Suppliers.Notifications.Deleted"));
            return RedirectToAction("List");
        }


        #endregion
        
        #region Materials

        [HttpPost]
        public virtual IActionResult MaterialList(DataSourceRequest command, int groupId)
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageSuppliers))
                return AccessDeniedKendoGridJson();

            var materials = _supplierService.GetMaterialsByGroupId(groupId,
                command.Page - 1, command.PageSize, true);
            var gridModel = new DataSourceResult
            {
                Data = materials.Select(x => new SupplierDetailsPageViewModel.MaterialListItemViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    StockQuantity = x.StockQuantity,
                    PictureThumbnailUrl = _pictureService.GetPictureUrl(x.PictureId, 75, true)
                }),
                Total = materials.TotalCount
            };

            return Json(gridModel);
        }

        public virtual IActionResult UnGroupMaterial(int id)
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageSuppliers))
                return AccessDeniedView();

            _supplierService.UngroupMaterial(id);

            return new NullJsonResult();
        }

        public virtual IActionResult AddMaterialsPopup(int supplierId)
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageSuppliers))
                return AccessDeniedView();
            
            var model = new SupplierDetailsPageViewModel.AddMaterialsPopupViewModel();

            // groups
            model.AvailableSuppliers.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.Common.All"),
                Value = "0"
            });
            var groups = _supplierService.GetAllSuppliers(showHidden: true);

            var list = groups.Select(c => new SelectListItem
            {
                Text = c.GetFormattedBreadCrumb(_supplierService),
                Value = c.Id.ToString()
            });
            foreach (var item in list)
                model.AvailableSuppliers.Add(item);

            //manufacturers
            model.AvailableSuppliers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var manufacturers = SelectListHelper.GetManufacturerList(_manufacturerService, _cacheManager, true);
            foreach (var m in manufacturers)
                model.AvailableSuppliers.Add(m);

            //warehouses
            model.AvailableWarehouses.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var wh in _shippingService.GetAllWarehouses())
                model.AvailableWarehouses.Add(new SelectListItem { Text = wh.Name, Value = wh.Id.ToString() });

            //return View(model);
            return View("~/Plugins/Xrms.MaterialAdmin/Areas/Admin/Views/Supplier/AddMaterialsPopup.cshtml", model);
        }

        [HttpPost]
        public virtual IActionResult AddMaterialsPopupList(DataSourceRequest command, SupplierDetailsPageViewModel.MaterialListSearchModel model)
        {
                if (!_permissionService.Authorize(XrmsPermissionProvider.ManageMaterials))
                    return AccessDeniedKendoGridJson();

                var groupIds = new List<int> { model.SearchSupplierId };

                var materials = _materialService.SearchMaterials(
                    groupIds: groupIds,
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
                        var materialModel = new
                        {
                            Id = x.Id,
                            PictureThumbnailUrl = _pictureService.GetPictureUrl(x.PictureId, 75, true),
                            Name = x.Name,
                            Unit = x.Unit,
                            Group = x.Supplier.GetFormattedBreadCrumb(_supplierService)
                        };

                        return materialModel;
                    }),
                    Total = materials.TotalCount
                };

                return Json(gridModel);
            }
        
        [HttpPost]
        //[FormValueRequired("save")]
        public virtual IActionResult AddMaterialsPopup(SupplierDetailsPageViewModel.AddMaterialsPopupModel model)
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageSuppliers))
                return AccessDeniedView();

            if (model.SelectedMaterialIds != null)
            {
                foreach (var id in model.SelectedMaterialIds)
                {
                    _supplierService.InsertMaterialIntoGroup(model.SupplierId, id);
                }
            }

            return Ok();
        }

        #endregion

        */
    }
}