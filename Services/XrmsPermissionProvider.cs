using System;
using System.Collections.Generic;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Services.Security;

namespace Nop.Plugin.Xrms.MaterialAdmin.Services
{
    /// <summary>
    /// XRMS permission provider
    /// </summary>
    public partial class XrmsPermissionProvider : IPermissionProvider
    {
        //admin area permissions
        public static readonly PermissionRecord ManageSuppliers = new PermissionRecord { Name = "Admin area. XRMS Manage Suppliers", SystemName = "XrmsManageSuppliers", Category = "Catalog" };
        public static readonly PermissionRecord ManageMaterials = new PermissionRecord { Name = "Admin area. XRMS Manage Materials", SystemName = "XrmsManageMaterials", Category = "Catalog" };
        public static readonly PermissionRecord ManageMaterialGroups = new PermissionRecord { Name = "Admin area. XRMS Manage Material Groups", SystemName = "XrmsManageMaterialGroups", Category = "Catalog" };

        //public store permissions
        /*public static readonly PermissionRecord DisplayPrices = new PermissionRecord { Name = "Public store. Display Prices", SystemName = "DisplayPrices", Category = "PublicStore" };
        public static readonly PermissionRecord EnableShoppingCart = new PermissionRecord { Name = "Public store. Enable shopping cart", SystemName = "EnableShoppingCart", Category = "PublicStore" };
        public static readonly PermissionRecord EnableWishlist = new PermissionRecord { Name = "Public store. Enable wishlist", SystemName = "EnableWishlist", Category = "PublicStore" };
        public static readonly PermissionRecord PublicStoreAllowNavigation = new PermissionRecord { Name = "Public store. Allow navigation", SystemName = "PublicStoreAllowNavigation", Category = "PublicStore" };
        public static readonly PermissionRecord AccessClosedStore = new PermissionRecord { Name = "Public store. Access a closed store", SystemName = "AccessClosedStore", Category = "PublicStore" };*/

        /// <summary>
        /// Get permissions
        /// </summary>
        /// <returns>Permissions</returns>
        public virtual IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[]
            {
                ManageSuppliers,
                ManageMaterials,
                ManageMaterialGroups
            };
        }

        /// <summary>
        /// Get default permissions
        /// </summary>
        /// <returns>Permissions</returns>
        public virtual IEnumerable<DefaultPermissionRecord> GetDefaultPermissions()
        {
            return new[]
            {
                new DefaultPermissionRecord
                {
                    CustomerRoleSystemName = SystemCustomerRoleNames.Administrators,
                    PermissionRecords = new[]
                    {
                        ManageSuppliers,
                        ManageMaterials,
                        ManageMaterialGroups
                    }
                }
            };
        }
    }
}
