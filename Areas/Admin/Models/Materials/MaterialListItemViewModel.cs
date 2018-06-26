using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Plugin.Xrms.MaterialAdmin.Areas.Admin.Models.Materials
{
    public partial class MaterialListItemViewModel : BaseNopEntityModel
    {
        public MaterialListItemViewModel()
        {

        }

        //picture thumbnail
        public string PictureThumbnailUrl { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Code { get; set; }

        public int StockQuantity { get; set; }

        public string Unit { get; set; }

        public decimal Cost { get; set; }

        public int DisplayOrder { get; set; }

        public string Group { get; set; }
    }
}
