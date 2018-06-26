using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Plugin.Xrms.MaterialAdmin.Areas.Admin.Models.Materials
{
    public partial class MaterialListPageViewModel : BaseNopModel
    {

        public MaterialListPageViewModel()
        {
            AvailableMaterialGroups = new List<SelectListItem>();
            AvailableSuppliers = new List<SelectListItem>();
            AvailableWarehouses = new List<SelectListItem>();
        }

        public IList<SelectListItem> AvailableMaterialGroups { get; set; }
        public IList<SelectListItem> AvailableSuppliers { get; set; }
        public IList<SelectListItem> AvailableWarehouses { get; set; }

        public MaterialListSearchModel SearchModel { get; set; }
    }
}
