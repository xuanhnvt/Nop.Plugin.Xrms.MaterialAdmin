using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Nop.Plugin.Xrms.MaterialAdmin.Areas.Admin.Validators;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Plugin.Xrms.MaterialAdmin.Areas.Admin.Models.Materials
{
    [Validator(typeof(MaterialValidator))]
    public partial class MaterialModel : BaseNopModel
    {
        public MaterialModel()
        {

        }

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
        public int SupplierId { get; set; }

        [NopResourceDisplayName("Xrms.Admin.Catalog.Materials.Fields.ManageInventoryMethod")]
        public int ManageInventoryMethodId { get; set; }

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

    }
}
