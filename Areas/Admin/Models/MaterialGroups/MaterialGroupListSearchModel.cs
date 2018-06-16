using System;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Plugin.Xrms.MaterialAdmin.Areas.Admin.Models.MaterialGroups
{
    public partial class MaterialGroupListSearchModel : BaseNopModel
    {
        [NopResourceDisplayName("Xrms.Admin.Catalog.MaterialGroups.List.Search.MaterialGroupName")]
        public string SearchMaterialGroupName { get; set; }
    }
}
