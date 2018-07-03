using Nop.Data.Mapping;
using Nop.Plugin.Xrms.MaterialAdmin.Domain;

namespace Nop.Plugin.Xrms.MaterialAdmin.Data.Mapping
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class MaterialGroupMap : NopEntityTypeConfiguration<MaterialGroup>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public MaterialGroupMap()
        {
            this.ToTable("XrmsMaterialGroup");
            this.HasKey(c => c.Id);
            this.Property(c => c.Name).IsRequired().HasMaxLength(400);
        }
    }
}