using Nop.Web.Framework.Mvc.Models;

namespace Nop.Plugin.Xrms.MaterialAdmin.Areas.Admin.Models.MaterialGroups
{
    public partial class MaterialGroupListPageViewModel : BaseNopModel
    {
        public MaterialGroupListPageViewModel()
        {

        }

        /*[NopResourceDisplayName("Xrms.Admin.Catalog.MaterialGroups.List.Search.MaterialGroupName")]
        public string SearchMaterialGroupName { get; set; }*/

        public MaterialGroupListSearchModel SearchModel { get; set; }
    }
}
