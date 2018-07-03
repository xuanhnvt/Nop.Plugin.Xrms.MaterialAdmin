using Nop.Data.Mapping;
using Nop.Plugin.Xrms.MaterialAdmin.Domain;

namespace Nop.Plugin.Xrms.MaterialAdmin.Data.Mapping
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class SupplierMap : NopEntityTypeConfiguration<Supplier>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public SupplierMap()
        {
            this.ToTable("XrmsSupplier");
            this.HasKey(s => s.Id);
            this.Property(s => s.Name).IsRequired().HasMaxLength(400);
        }
    }
}