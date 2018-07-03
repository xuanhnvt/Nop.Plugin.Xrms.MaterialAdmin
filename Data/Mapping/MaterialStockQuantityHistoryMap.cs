using System;
using Nop.Data.Mapping;
using Nop.Plugin.Xrms.MaterialAdmin.Domain;

namespace Nop.Plugin.Xrms.MaterialAdmin.Data.Mapping
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class MaterialQuantityHistoryMap : NopEntityTypeConfiguration<MaterialQuantityHistory>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public MaterialQuantityHistoryMap()
        {
            this.ToTable("XrmsMaterialQuantityHistory");
            this.HasKey(historyEntry => historyEntry.Id);

            this.HasRequired(historyEntry => historyEntry.Material)
                .WithMany()
                .HasForeignKey(historyEntry => historyEntry.MaterialId)
                .WillCascadeOnDelete(true);
        }
    }
}
