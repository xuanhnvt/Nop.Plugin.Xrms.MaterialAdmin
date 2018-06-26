using Nop.Web.Framework.Mvc.Models;

namespace Nop.Plugin.Xrms.MaterialAdmin.Areas.Admin.Models.Suppliers
{
    public partial class SupplierListPageViewModel : BaseNopModel
    {
        public SupplierListPageViewModel()
        {

        }

        public SupplierListSearchModel SearchModel { get; set; }
    }
}
