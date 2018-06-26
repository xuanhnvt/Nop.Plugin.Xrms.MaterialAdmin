using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Data;
using Nop.Plugin.Xrms.MaterialAdmin.Domain;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Xrms.MaterialAdmin.Services
{
    /// <summary>
    /// Supplier service
    /// </summary>
    public partial class SupplierService : ISupplierService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : supplier ID
        /// </remarks>
        private const string SUPPLIERS_BY_ID_KEY = "Nop.supplier.id-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string SUPPLIERS_PATTERN_KEY = "Nop.supplier.";

        #endregion

        #region Fields

        private readonly IRepository<Supplier> _supplierRepository;
        private readonly IDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="supplierRepository">Supplier repository</param>
        /// <param name="dbContext">Database Context</param>
        /// <param name="eventPublisher">Event published</param>
        public SupplierService(ICacheManager cacheManager,
            IRepository<Supplier> supplierRepository,
            IDbContext dbContext,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._supplierRepository = supplierRepository;
            this._dbContext = dbContext;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Utilities

        #endregion

        #region Methods

        /// <summary>
        /// Delete a supplier
        /// </summary>
        /// <param name="supplier">Supplier</param>
        public virtual void DeleteSupplier(Supplier supplier)
        {
            if (supplier == null)
                throw new ArgumentNullException(nameof(supplier));

            supplier.Deleted = true;
            //delete supplier
            UpdateSupplier(supplier);

            //event notification
            _eventPublisher.EntityDeleted(supplier);
        }

        /// <summary>
        /// Delete suppliers
        /// </summary>
        /// <param name="suppliers">Suppliers</param>
        public virtual void DeleteSuppliers(IList<Supplier> suppliers)
        {
            if (suppliers == null)
                throw new ArgumentNullException(nameof(suppliers));

            foreach (var supplier in suppliers)
            {
                supplier.Deleted = true;
            }

            //delete supplier
            UpdateSuppliers(suppliers);

            foreach (var supplier in suppliers)
            {
                //event notification
                _eventPublisher.EntityDeleted(supplier);
            }
        }

        /// <summary>
        /// Gets supplier
        /// </summary>
        /// <param name="supplierId">Supplier identifier</param>
        /// <returns>Supplier</returns>
        public virtual Supplier GetSupplierById(int supplierId)
        {
            if (supplierId == 0)
                return null;

            var key = string.Format(SUPPLIERS_BY_ID_KEY, supplierId);
            return _cacheManager.Get(key, () => _supplierRepository.GetById(supplierId));
        }

        /// <summary>
        /// Get suppliers by identifiers
        /// </summary>
        /// <param name="supplierIds">Supplier identifiers</param>
        /// <returns>Suppliers</returns>
        public virtual IList<Supplier> GetSuppliersByIds(int[] supplierIds)
        {
            if (supplierIds == null || supplierIds.Length == 0)
                return new List<Supplier>();

            var query = from p in _supplierRepository.Table
                        where supplierIds.Contains(p.Id) && !p.Deleted
                        select p;
            var suppliers = query.ToList();
            //sort by passed identifiers
            var sortedSuppliers = new List<Supplier>();
            foreach (var id in supplierIds)
            {
                var supplier = suppliers.Find(x => x.Id == id);
                if (supplier != null)
                    sortedSuppliers.Add(supplier);
            }
            return sortedSuppliers;
        }

        /// <summary>
        /// Inserts a supplier
        /// </summary>
        /// <param name="supplier">Supplier</param>
        public virtual void InsertSupplier(Supplier supplier)
        {
            if (supplier == null)
                throw new ArgumentNullException(nameof(supplier));

            //insert
            _supplierRepository.Insert(supplier);

            //clear cache
            _cacheManager.RemoveByPattern(SUPPLIERS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(supplier);
        }

        /// <summary>
        /// Updates the supplier
        /// </summary>
        /// <param name="supplier">Supplier</param>
        public virtual void UpdateSupplier(Supplier supplier)
        {
            if (supplier == null)
                throw new ArgumentNullException(nameof(supplier));

            //update
            _supplierRepository.Update(supplier);

            //cache
            _cacheManager.RemoveByPattern(SUPPLIERS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(supplier);
        }

        /// <summary>
        /// Update suppliers
        /// </summary>
        /// <param name="suppliers">Suppliers</param>
        public virtual void UpdateSuppliers(IList<Supplier> suppliers)
        {
            if (suppliers == null)
                throw new ArgumentNullException(nameof(suppliers));

            //update
            _supplierRepository.Update(suppliers);

            //cache
            _cacheManager.RemoveByPattern(SUPPLIERS_PATTERN_KEY);

            //event notification
            foreach (var supplier in suppliers)
            {
                _eventPublisher.EntityUpdated(supplier);
            }
        }

        /// <summary>
        /// Search suppliers
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="searchDescriptions">A value indicating whether to search by a specified "keyword" in supplier descriptions</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Suppliers</returns>
        public virtual IPagedList<Supplier> SearchSuppliers(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            string keywords = null,
            bool searchDescriptions = false,
            bool showHidden = false)
        {
            //suppliers
            var query = _supplierRepository.Table;
            query = query.Where(p => !p.Deleted);

            //searching by keyword
            if (!string.IsNullOrWhiteSpace(keywords))
            {
                query = from p in query
                        where (p.Name.Contains(keywords)) ||
                              (searchDescriptions && p.Description.Contains(keywords))
                        select p;
            }

            //only distinct suppliers (group by ID)
            //if we use standard Distinct() method, then all fields will be compared (low performance)
            //it'll not work in SQL Server Compact when searching suppliers by a keyword)
            query = from p in query
                    group p by p.Id into pGroup
                    orderby pGroup.Key
                    select pGroup.FirstOrDefault();

            var suppliers = new PagedList<Supplier>(query, pageIndex, pageSize);

            //return suppliers
            return suppliers;
        }

        #endregion
    }
}
