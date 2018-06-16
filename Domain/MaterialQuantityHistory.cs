using System;
using Nop.Core;

namespace Nop.Plugin.Xrms.MaterialAdmin.Domain
{
    /// <summary>
    /// Represents a stock quantity change entry
    /// </summary>
    public partial class MaterialQuantityHistory : BaseEntity
    {
        /// <summary>
        /// Gets or sets the stock quantity adjustment
        /// </summary>
        public int QuantityAdjustment { get; set; }

        /// <summary>
        /// Gets or sets current stock quantity
        /// </summary>
        public int StockQuantity { get; set; }

        /// <summary>
        /// Gets or sets the message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int MaterialId { get; set; }

        /// <summary>
        /// Gets or sets the product attribute combination identifier
        /// </summary>
        public int? CombinationId { get; set; }

        /// <summary>
        /// Gets or sets the warehouse identifier
        /// </summary>
        public int? WarehouseId { get; set; }

        /// <summary>
        /// Gets the material
        /// </summary>
        public virtual Material Material { get; set; }
    }
}
