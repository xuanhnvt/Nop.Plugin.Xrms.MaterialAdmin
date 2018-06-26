
using Nop.Data.Mapping;
using Nop.Plugin.Xrms.MaterialAdmin.Domain;

namespace Nop.Plugin.Xrms.MaterialAdmin.Data.Mapping
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class MaterialMap : NopEntityTypeConfiguration<Material>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public MaterialMap()
        {
            this.ToTable("Material");
            this.HasKey(c => c.Id);
            this.Property(c => c.Name).IsRequired().HasMaxLength(400);
            this.HasRequired(c => c.MaterialGroup).WithMany(g => g.Materials).HasForeignKey(c => c.MaterialGroupId);
        }
    }
}
