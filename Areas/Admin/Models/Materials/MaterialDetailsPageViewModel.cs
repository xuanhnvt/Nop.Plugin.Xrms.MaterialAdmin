using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Plugin.Xrms.MaterialAdmin.Areas.Admin.Models.Materials
{
    public partial class MaterialDetailsPageViewModel : BaseNopEntityModel
    {
        public MaterialDetailsPageViewModel()
        {
            AvailableMaterialGroups = new List<SelectListItem>();
            AvailableWarehouses = new List<SelectListItem>();
        }

        // material groups
        public IList<SelectListItem> AvailableMaterialGroups { get; set; }

        // warehouse
        public IList<SelectListItem> AvailableWarehouses{ get; set; }

        [NopResourceDisplayName("Xrms.Admin.Catalog.Materials.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Xrms.Admin.Catalog.Materials.Fields.Description")]
        public string Description { get; set; }

        [NopResourceDisplayName("Xrms.Admin.Catalog.Materials.Fields.MaterialGroup")]
        public int MaterialGroupId { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName("Xrms.Admin.Catalog.Materials.Fields.Picture")]
        public int PictureId { get; set; }

        [NopResourceDisplayName("Xrms.Admin.Catalog.Materials.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Xrms.Admin.Catalog.Materials.Fields.AdminComment")]
        public string AdminComment { get; set; }

        [NopResourceDisplayName("Xrms.Admin.Catalog.Materials.Fields.Code")]
        public string Code { get; set; }

        [NopResourceDisplayName("Xrms.Admin.Catalog.Materials.Fields.Supplier")]
        public string ManufacturerPartNumber { get; set; }

        [NopResourceDisplayName("Xrms.Admin.Catalog.Materials.Fields.Warehouse")]
        public int WarehouseId { get; set; }

        [NopResourceDisplayName("Xrms.Admin.Catalog.Materials.Fields.StockQuantity")]
        public int StockQuantity { get; set; }

        //[NopResourceDisplayName("Xrms.Admin.Catalog.Materials.Fields.UsedQuantity")]
        //public int UsedQuantity { get; set; }

        [NopResourceDisplayName("Xrms.Admin.Catalog.Materials.Fields.MinStockQuantity")]
        public int MinStockQuantity { get; set; }

        [NopResourceDisplayName("Xrms.Admin.Catalog.Materials.Fields.Unit")]
        public string Unit { get; set; }

        [NopResourceDisplayName("Xrms.Admin.Catalog.Materials.Fields.Cost")]
        public decimal Cost { get; set; }

        #region Nested classes

        /*public partial class AddCategoryProductModel : BaseNopModel
        {
            public AddCategoryProductModel()
            {
                AvailableCategories = new List<SelectListItem>();
                AvailableManufacturers = new List<SelectListItem>();
                AvailableStores = new List<SelectListItem>();
                AvailableVendors = new List<SelectListItem>();
                AvailableProductTypes = new List<SelectListItem>();
            }

            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductName")]
            public string SearchProductName { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchCategory")]
            public int SearchCategoryId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchManufacturer")]
            public int SearchManufacturerId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchStore")]
            public int SearchStoreId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchVendor")]
            public int SearchVendorId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductType")]
            public int SearchProductTypeId { get; set; }

            public IList<SelectListItem> AvailableCategories { get; set; }
            public IList<SelectListItem> AvailableManufacturers { get; set; }
            public IList<SelectListItem> AvailableStores { get; set; }
            public IList<SelectListItem> AvailableVendors { get; set; }
            public IList<SelectListItem> AvailableProductTypes { get; set; }

            public int CategoryId { get; set; }

            public int[] SelectedProductIds { get; set; }
        }*/

        #endregion
    }
}
