using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Plugin.Xrms.MaterialAdmin.Domain;

namespace Nop.Plugin.Xrms.MaterialAdmin.Services
{
    /// <summary>
    /// Supplier service
    /// </summary>
    public partial interface ISupplierService
    {
        #region Suppliers

        /// <summary>
        /// Delete a supplier
        /// </summary>
        /// <param name="supplier">Supplier</param>
        void DeleteSupplier(Supplier supplier);

        /// <summary>
        /// Delete suppliers
        /// </summary>
        /// <param name="suppliers">Suppliers</param>
        void DeleteSuppliers(IList<Supplier> suppliers);

        /// <summary>
        /// Gets supplier
        /// </summary>
        /// <param name="supplierId">Supplier identifier</param>
        /// <returns>Supplier</returns>
        Supplier GetSupplierById(int supplierId);

        /// <summary>
        /// Gets suppliers by identifier
        /// </summary>
        /// <param name="supplierIds">Supplier identifiers</param>
        /// <returns>Suppliers</returns>
        IList<Supplier> GetSuppliersByIds(int[] supplierIds);

        /// <summary>
        /// Inserts a supplier
        /// </summary>
        /// <param name="supplier">Supplier</param>
        void InsertSupplier(Supplier supplier);

        /// <summary>
        /// Updates the supplier
        /// </summary>
        /// <param name="supplier">Supplier</param>
        void UpdateSupplier(Supplier supplier);

        /// <summary>
        /// Updates the suppliers
        /// </summary>
        /// <param name="suppliers">Supplier</param>
        void UpdateSuppliers(IList<Supplier> suppliers);

        /// <summary>
        /// Search suppliers
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="searchDescriptions">A value indicating whether to search by a specified "keyword" in supplier descriptions</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Suppliers</returns>
        IPagedList<Supplier> SearchSuppliers(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            string keywords = null,
            bool searchDescriptions = false,
            bool showHidden = false);

        #endregion
    }
}
