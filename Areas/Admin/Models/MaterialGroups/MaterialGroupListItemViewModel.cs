using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Plugin.Xrms.MaterialAdmin.Areas.Admin.Models.MaterialGroups
{
    public partial class MaterialGroupListItemViewModel : BaseNopEntityModel
    {
        public MaterialGroupListItemViewModel()
        {

        }

        public string Name { get; set; }

        public string Description { get; set; }

        public int DisplayOrder { get; set; }

        public string Breadcrumb { get; set; }
    }
}
