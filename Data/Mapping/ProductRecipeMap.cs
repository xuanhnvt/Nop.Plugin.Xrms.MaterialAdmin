
using Nop.Data.Mapping;
using Nop.Plugin.Xrms.MaterialAdmin.Domain;

namespace Nop.Plugin.Xrms.MaterialAdmin.Data.Mapping
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class ProductRecipeMap : NopEntityTypeConfiguration<ProductRecipe>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public ProductRecipeMap()
        {
            this.ToTable("XrmsProductRecipe");
            this.HasKey(pr => pr.Id);
            this.HasRequired(pr => pr.Material)
                .WithMany()
                .HasForeignKey(pr => pr.MaterialId);

            /*this.HasRequired(pr => pr.Product)
                .WithMany()
                .HasForeignKey(pr => pr.ProductId);*/
        }
    }
}