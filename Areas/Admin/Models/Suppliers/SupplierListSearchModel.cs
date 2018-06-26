using System;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Plugin.Xrms.MaterialAdmin.Areas.Admin.Models.Suppliers
{
    public partial class SupplierListSearchModel : BaseNopModel
    {
        [NopResourceDisplayName("Xrms.Admin.Catalog.Suppliers.List.Search.SupplierName")]
        public string SearchSupplierName { get; set; }
    }
}
