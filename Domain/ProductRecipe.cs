using Nop.Core;
using Nop.Core.Domain.Catalog;

namespace Nop.Plugin.Xrms.MaterialAdmin.Domain
{
    /// <summary>
    /// Represents a product recipe.
    /// </summary>
    public partial class ProductRecipe : BaseEntity
    {
        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the material identifier
        /// </summary>
        public int MaterialId { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the unit
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Gets the material
        /// </summary>
        public virtual Material Material { get; set; }

    }
}
