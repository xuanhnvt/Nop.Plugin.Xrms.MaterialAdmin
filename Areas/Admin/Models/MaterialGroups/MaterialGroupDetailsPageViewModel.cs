using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Plugin.Xrms.MaterialAdmin.Areas.Admin.Models.MaterialGroups
{
    public partial class MaterialGroupDetailsPageViewModel : BaseNopEntityModel
    {
        public MaterialGroupDetailsPageViewModel()
        {
            AvailableMaterialGroups = new List<SelectListItem>();

            //MaterialGroupInfo = new MaterialGroupModel();
        }

        // parent groups
        public IList<SelectListItem> AvailableMaterialGroups { get; set; }

        [NopResourceDisplayName("Xrms.Admin.Catalog.MaterialGroups.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Xrms.Admin.Catalog.MaterialGroups.Fields.Description")]
        public string Description { get; set; }

        [NopResourceDisplayName("Xrms.Admin.Catalog.MaterialGroups.Fields.Parent")]
        public int ParentGroupId { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName("Xrms.Admin.Catalog.MaterialGroups.Fields.Picture")]
        public int PictureId { get; set; }

        [NopResourceDisplayName("Xrms.Admin.Catalog.MaterialGroups.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        //public MaterialGroupModel MaterialGroupInfo { get; set; }

        #region Nested classes

        public partial class MaterialListItemViewModel : BaseNopEntityModel
        {
            public string Name { get; set; }
            public int StockQuantity { get; set; }
            public string PictureThumbnailUrl { get; set; }
        }

        public partial class AddMaterialsPopupViewModel : BaseNopModel
        {
            public AddMaterialsPopupViewModel()
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

        public partial class AddMaterialsPopupModel : BaseNopModel
        {
            public int MaterialGroupId { get; set; }
            public int[] SelectedMaterialIds { get; set; }
        }

        public partial class MaterialListSearchModel : BaseNopModel
        {
            [NopResourceDisplayName("Xrms.Admin.Catalog.Materials.List.Search.MaterialName")]
            public string SearchMaterialName { get; set; }
            [NopResourceDisplayName("Xrms.Admin.Catalog.Materials.List.Search.MaterialGroup")]
            public int SearchMaterialGroupId { get; set; }
            [NopResourceDisplayName("Xrms.Admin.Catalog.Materials.List.Search.IncludeSubGroup")]
            public bool SearchIncludeSubGroup { get; set; }
            [NopResourceDisplayName("Xrms.Admin.Catalog.Materials.List.Search.Warehouse")]
            public int SearchWarehouseId { get; set; }
        }

        #endregion
    }
}
