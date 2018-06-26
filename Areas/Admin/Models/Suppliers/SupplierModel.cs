using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Nop.Plugin.Xrms.MaterialAdmin.Areas.Admin.Validators;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Plugin.Xrms.MaterialAdmin.Areas.Admin.Models.Suppliers
{
    //[Validator(typeof(SupplierValidator))]
    public partial class SupplierModel : BaseNopModel
    {
        public SupplierModel()
        {

        }

        [NopResourceDisplayName("Xrms.Admin.Catalog.Suppliers.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Xrms.Admin.Catalog.Suppliers.Fields.Description")]
        public string Description { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName("Xrms.Admin.Catalog.Suppliers.Fields.Picture")]
        public int PictureId { get; set; }

        [NopResourceDisplayName("Xrms.Admin.Catalog.Suppliers.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

    }
}
