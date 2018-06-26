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
using Nop.Plugin.Xrms.MaterialAdmin.Areas.Admin.Models.MaterialGroups;
using Nop.Plugin.Xrms.MaterialAdmin.Services;
using Nop.Plugin.Xrms.MaterialAdmin.Areas.Admin.Models;
using Nop.Plugin.Xrms.MaterialAdmin.Domain;
using Nop.Services.Shipping;

namespace Nop.Plugin.Xrms.MaterialAdmin.Controllers
{
    public partial class MaterialGroupController : BaseAdminController
    {
        #region Fields

        private readonly IMaterialGroupService _materialGroupService;
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

        public MaterialGroupController(IMaterialGroupService materialGroupService,
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
            this._materialGroupService = materialGroupService;
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

        protected virtual void PrepareAvailableMaterialGroups(MaterialGroupDetailsPageViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.AvailableMaterialGroups.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Xrms.Admin.Catalog.MaterialGroups.Fields.Parent.None"),
                Value = "0"
            });
            var groups = _materialGroupService.GetAllMaterialGroups(showHidden: true);
            var list = groups.Select(c => new SelectListItem
            {
                Text = c.GetFormattedBreadCrumb(_materialGroupService),
                Value = c.Id.ToString()
            });
            foreach (var item in list)
                model.AvailableMaterialGroups.Add(item);
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

        #region List

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageMaterialGroups))
                return AccessDeniedView();

            var model = new MaterialGroupListPageViewModel();

            return View("~/Plugins/Xrms.MaterialAdmin/Areas/Admin/Views/MaterialGroup/List.cshtml", model);
        }

        [HttpPost]
        public virtual IActionResult List(DataSourceRequest command, MaterialGroupListSearchModel model)
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageMaterialGroups))
                return AccessDeniedKendoGridJson();

            var groups = _materialGroupService.GetAllMaterialGroups(model.SearchMaterialGroupName, 
                command.Page - 1, command.PageSize, true);
            var gridModel = new DataSourceResult
            {
                Data = groups.Select(x =>
                {
                    var groupModel = x.ToListItemViewModel();
                    groupModel.Breadcrumb = x.GetFormattedBreadCrumb(_materialGroupService);
                    return groupModel;
                }),
                Total = groups.TotalCount
            };
            return Json(gridModel);
        }

        #endregion
        
        #region Create / Edit / Delete

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageMaterialGroups))
                return AccessDeniedView();

            var model = new MaterialGroupDetailsPageViewModel();
            // set default values
            model.DisplayOrder = 1;

            // prepare parent groups
            PrepareAvailableMaterialGroups(model);

            return View("~/Plugins/Xrms.MaterialAdmin/Areas/Admin/Views/MaterialGroup/Create.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(MaterialGroupModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageMaterialGroups))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var group = model.ToEntity();
                group.CreatedOnUtc = DateTime.UtcNow;
                group.UpdatedOnUtc = DateTime.UtcNow;
                _materialGroupService.InsertMaterialGroup(group);

                //activity log
                _customerActivityService.InsertActivity("AddNewMaterialGroup", _localizationService.GetResource("Xrms.ActivityLog.AddNewMaterialGroup"), group.Name);

                SuccessNotification(_localizationService.GetResource("Xrms.Admin.Catalog.MaterialGroups.Notifications.Created"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = group.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            var viewModel = new MaterialGroupDetailsPageViewModel();
            model.ToDetailsViewModel(viewModel);
            //viewModel.MaterialGroupInfo = model;

            // prepare parent groups
            PrepareAvailableMaterialGroups(viewModel);

            return View("~/Plugins/Xrms.MaterialAdmin/Areas/Admin/Views/MaterialGroup/Create.cshtml", viewModel);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageMaterialGroups))
                return AccessDeniedView();

            var materialGroup = _materialGroupService.GetMaterialGroupById(id);
            if (materialGroup == null || materialGroup.Deleted) 
                //No group found with the specified id
                return RedirectToAction("List");

            var viewModel = materialGroup.ToDetailsViewModel();

            // prepare parent groups
            PrepareAvailableMaterialGroups(viewModel);

            return View("~/Plugins/Xrms.MaterialAdmin/Areas/Admin/Views/MaterialGroup/Edit.cshtml", viewModel);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(int id, MaterialGroupModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageMaterialGroups))
                return AccessDeniedView();

            var materialGroup = _materialGroupService.GetMaterialGroupById(id);
            if (materialGroup == null || materialGroup.Deleted)
                //No group found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var prevPictureId = materialGroup.PictureId;
                materialGroup = model.ToEntity(materialGroup);
                materialGroup.UpdatedOnUtc = DateTime.UtcNow;
                _materialGroupService.UpdateMaterialGroup(materialGroup);

                //delete an old picture (if deleted or updated)
                if (prevPictureId > 0 && prevPictureId != materialGroup.PictureId)
                {
                    var prevPicture = _pictureService.GetPictureById(prevPictureId);
                    if (prevPicture != null)
                        _pictureService.DeletePicture(prevPicture);
                }

                //activity log
                _customerActivityService.InsertActivity("EditMaterialGroup", _localizationService.GetResource("Xrms.ActivityLog.EditMaterialGroup"), materialGroup.Name);

                SuccessNotification(_localizationService.GetResource("Xrms.Admin.Catalog.MaterialGroups.Notifications.Updated"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new {id = materialGroup.Id});
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            var viewModel = new MaterialGroupDetailsPageViewModel();
            model.ToDetailsViewModel(viewModel);
            //viewModel.MaterialGroupInfo = model;

            //If we got this far, something failed, redisplay form
            // prepare parent groups
            PrepareAvailableMaterialGroups(viewModel);

            return View("~/Plugins/Xrms.MaterialAdmin/Areas/Admin/Views/MaterialGroup/Edit.cshtml", viewModel);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageMaterialGroups))
                return AccessDeniedView();

            var materialGroup = _materialGroupService.GetMaterialGroupById(id);
            if (materialGroup == null)
                // No group found with the specified id
                return RedirectToAction("List");

            _materialGroupService.DeleteMaterialGroup(materialGroup);

            //activity log
            _customerActivityService.InsertActivity("DeleteMaterialGroup", _localizationService.GetResource("Xrms.ActivityLog.DeleteMaterialGroup"), materialGroup.Name);

            SuccessNotification(_localizationService.GetResource("Xrms.Admin.Catalog.MaterialGroups.Notifications.Deleted"));
            return RedirectToAction("List");
        }


        #endregion

        #region Export / Import

        public virtual IActionResult ExportXml()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            try
            {
                var xml = _exportManager.ExportCategoriesToXml();
                return File(Encoding.UTF8.GetBytes(xml), "application/xml", "categories.xml");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        public virtual IActionResult ExportXlsx()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            try
            {
                var bytes = _exportManager.ExportCategoriesToXlsx(_categoryService.GetAllCategories(showHidden: true).Where(p => !p.Deleted).ToList());
                return File(bytes, MimeTypes.TextXlsx, "categories.xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual IActionResult ImportFromXlsx(IFormFile importexcelfile)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //a vendor cannot import categories
            if (_workContext.CurrentVendor != null)
                return AccessDeniedView();

            try
            {
                if (importexcelfile != null && importexcelfile.Length > 0)
                {
                    _importManager.ImportCategoriesFromXlsx(importexcelfile.OpenReadStream());
                }
                else
                {
                    ErrorNotification(_localizationService.GetResource("Admin.Common.UploadFile"));
                    return RedirectToAction("List");
                }
                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Categories.Imported"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        #endregion
        
        #region Materials

        [HttpPost]
        public virtual IActionResult MaterialList(DataSourceRequest command, int groupId)
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageMaterialGroups))
                return AccessDeniedKendoGridJson();

            var materials = _materialGroupService.GetMaterialsByGroupId(groupId,
                command.Page - 1, command.PageSize, true);
            var gridModel = new DataSourceResult
            {
                Data = materials.Select(x => new MaterialGroupDetailsPageViewModel.MaterialListItemViewModel
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
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageMaterialGroups))
                return AccessDeniedView();

            _materialGroupService.UngroupMaterial(id);

            return new NullJsonResult();
        }

        public virtual IActionResult AddMaterialsPopup(int materialGroupId)
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageMaterialGroups))
                return AccessDeniedView();
            
            var model = new MaterialGroupDetailsPageViewModel.AddMaterialsPopupViewModel();

            // groups
            model.AvailableMaterialGroups.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.Common.All"),
                Value = "0"
            });
            var groups = _materialGroupService.GetAllMaterialGroups(showHidden: true);

            var list = groups.Select(c => new SelectListItem
            {
                Text = c.GetFormattedBreadCrumb(_materialGroupService),
                Value = c.Id.ToString()
            });
            foreach (var item in list)
                model.AvailableMaterialGroups.Add(item);

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
            return View("~/Plugins/Xrms.MaterialAdmin/Areas/Admin/Views/MaterialGroup/AddMaterialsPopup.cshtml", model);
        }

        [HttpPost]
        public virtual IActionResult AddMaterialsPopupList(DataSourceRequest command, MaterialGroupDetailsPageViewModel.MaterialListSearchModel model)
        {
                if (!_permissionService.Authorize(XrmsPermissionProvider.ManageMaterials))
                    return AccessDeniedKendoGridJson();

                var groupIds = new List<int> { model.SearchMaterialGroupId };
                // include sub groups
                if (model.SearchIncludeSubGroup && model.SearchMaterialGroupId > 0)
                    groupIds.AddRange(GetChildGroupIds(model.SearchMaterialGroupId));

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
                            Group = x.MaterialGroup.GetFormattedBreadCrumb(_materialGroupService)
                        };

                        return materialModel;
                    }),
                    Total = materials.TotalCount
                };

                return Json(gridModel);
            }
        
        [HttpPost]
        //[FormValueRequired("save")]
        public virtual IActionResult AddMaterialsPopup(MaterialGroupDetailsPageViewModel.AddMaterialsPopupModel model)
        {
            if (!_permissionService.Authorize(XrmsPermissionProvider.ManageMaterialGroups))
                return AccessDeniedView();

            if (model.SelectedMaterialIds != null)
            {
                foreach (var id in model.SelectedMaterialIds)
                {
                    _materialGroupService.InsertMaterialIntoGroup(model.MaterialGroupId, id);
                }
            }

            return Ok();
        }

        #endregion
    }
}