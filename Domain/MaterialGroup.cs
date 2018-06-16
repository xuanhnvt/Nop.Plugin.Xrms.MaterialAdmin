using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;

namespace Nop.Plugin.Xrms.MaterialAdmin.Domain
{
    /// <summary>
    /// Represents a material group
    /// </summary>
    public partial class MaterialGroup : BaseEntity, ILocalizedEntity
    {

        private ICollection<Material> _materials;

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the parent group identifier
        /// </summary>
        public int ParentGroupId { get; set; }

        /// <summary>
        /// Gets or sets the picture identifier
        /// </summary>
        public int PictureId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance update
        /// </summary>
        public DateTime UpdatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the collection of Material
        /// </summary>
        public virtual ICollection<Material> Materials
        {
            get { return _materials ?? (_materials = new List<Material>()); }
            protected set { _materials = value; }
        }
    }
}
