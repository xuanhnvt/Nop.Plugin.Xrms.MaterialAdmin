using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Core.Extensions;
using Nop.Data;
using Nop.Plugin.Xrms.MaterialAdmin.Domain;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.ExportImport.Help;
using Nop.Services.Security;
using Nop.Services.Stores;
using OfficeOpenXml;

namespace Nop.Plugin.Xrms.MaterialAdmin.Services
{
    /// <summary>
    /// MaterialGroup service
    /// </summary>
    public partial class MaterialGroupService : IMaterialGroupService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : material group ID
        /// </remarks>
        private const string MATERIAL_GROUPS_BY_ID_KEY = "Nop.materialGroup.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : parent ID
        /// {1} : show hidden records?
        /// {2} : current customer ID
        /// {3} : store ID
        /// {4} : include all levels (child)
        /// </remarks>
        private const string MATERIAL_GROUPS_BY_PARENT_ID_KEY = "Nop.materialGroup.byparent-{0}-{1}-{2}-{3}-{4}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string MATERIAL_GROUPS_PATTERN_KEY = "Nop.materialGroup.";

        #endregion

        #region Fields

        private readonly IRepository<MaterialGroup> _materialGroupRepository;
        private readonly IRepository<Material> _materialRepository;

        private readonly IRepository<AclRecord> _aclRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IDbContext _dbContext;
        private readonly IDataProvider _dataProvider;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IAclService _aclService;
        private readonly CommonSettings _commonSettings;
        private readonly CatalogSettings _catalogSettings;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="materialGroupRepository">MaterialGroup repository</param>
        /// <param name="materialGroupRepository">Material repository</param>
        /// <param name="aclRepository">ACL record repository</param>
        /// <param name="storeMappingRepository">Store mapping repository</param>
        /// <param name="dbContext">DB context</param>
        /// <param name="dataProvider">Data provider</param>
        /// <param name="workContext">Work context</param>
        /// <param name="storeContext">Store context</param>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="storeMappingService">Store mapping service</param>
        /// <param name="aclService">ACL service</param>
        /// <param name="commonSettings">Common settings</param>
        /// <param name="catalogSettings">Catalog settings</param>
        public MaterialGroupService(ICacheManager cacheManager,
            IRepository<MaterialGroup> materialGroupRepository,
            IRepository<Material> materialRepository,
            IRepository<AclRecord> aclRepository,
            IRepository<StoreMapping> storeMappingRepository,
            IDbContext dbContext,
            IDataProvider dataProvider,
            IWorkContext workContext,
            IStoreContext storeContext,
            IEventPublisher eventPublisher,
            IStoreMappingService storeMappingService,
            IAclService aclService,
            CommonSettings commonSettings,
            CatalogSettings catalogSettings)
        {
            this._cacheManager = cacheManager;
            this._materialGroupRepository = materialGroupRepository;
            this._materialRepository = materialRepository;

            this._aclRepository = aclRepository;
            this._storeMappingRepository = storeMappingRepository;
            this._dbContext = dbContext;
            this._dataProvider = dataProvider;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._eventPublisher = eventPublisher;
            this._storeMappingService = storeMappingService;
            this._aclService = aclService;
            this._commonSettings = commonSettings;
            this._catalogSettings = catalogSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get property list by excel cells
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="worksheet">Excel worksheet</param>
        /// <returns>Property list</returns>
        public static IList<PropertyByName<T>> GetPropertiesByExcelCells<T>(ExcelWorksheet worksheet)
        {
            var properties = new List<PropertyByName<T>>();
            var poz = 1;
            while (true)
            {
                try
                {
                    var cell = worksheet.Cells[1, poz];

                    if (string.IsNullOrEmpty(cell?.Value?.ToString()))
                        break;

                    poz += 1;
                    properties.Add(new PropertyByName<T>(cell.Value.ToString()));
                }
                catch
                {
                    break;
                }
            }

            return properties;
        }


        /// <summary>
        /// Delete material group
        /// </summary>
        /// <param name="MaterialGroup">MaterialGroup</param>
        public virtual void DeleteMaterialGroup(MaterialGroup materialGroup)
        {
            if (materialGroup == null)
                throw new ArgumentNullException(nameof(materialGroup));

            if (materialGroup.Id == 1) // default group "Undefined", shouldn't delete it, just return or throw new exception
            {
                return;
                //throw new InvalidOperationException(String.Format("Can not delete default group '{0}'", materialGroup.Name));
            }

            // change group of materials that belong to deleted group.
            var query = from mat in _materialRepository.Table
                        where mat.MaterialGroupId == materialGroup.Id && !mat.Deleted
                        select mat;

            foreach(var mat in query.ToList())
            {
                mat.MaterialGroupId = 1; // set default group
                _materialRepository.Update(mat);
            }

            materialGroup.Deleted = true;
            UpdateMaterialGroup(materialGroup);

            //event notification
            _eventPublisher.EntityDeleted(materialGroup);

            //reset a "parent group" property of all child groups
            var childGroups = GetMaterialGroupsByParentGroupId(materialGroup.Id, true);
            foreach (var group in childGroups)
            {
                group.ParentGroupId = 0;
                UpdateMaterialGroup(group);
            }
        }

        /// <summary>
        /// Gets all material groups
        /// </summary>
        /// <param name="groupName">Group name</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>MaterialGroup</returns>
        public virtual IPagedList<MaterialGroup> GetAllMaterialGroups(string groupName = "",
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query = _materialGroupRepository.Table;
            if (!string.IsNullOrWhiteSpace(groupName))
                query = query.Where(c => c.Name.Contains(groupName));
            query = query.Where(c => !c.Deleted);
            query = query.OrderBy(c => c.ParentGroupId).ThenBy(c => c.DisplayOrder).ThenBy(c => c.Id);

            /*if ((storeId > 0 && !_catalogSettings.IgnoreStoreLimitations) || (!showHidden && !_catalogSettings.IgnoreAcl))
            {
                if (!showHidden && !_catalogSettings.IgnoreAcl)
                {
                    //ACL (access control list)
                    var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();
                    query = from c in query
                        join acl in _aclRepository.Table
                            on new { c1 = c.Id, c2 = "MaterialGroup" } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into c_acl
                        from acl in c_acl.DefaultIfEmpty()
                        where !c.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                        select c;
                }
                if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
                {
                    //Store mapping
                    query = from c in query
                        join sm in _storeMappingRepository.Table
                            on new { c1 = c.Id, c2 = "MaterialGroup" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into c_sm
                        from sm in c_sm.DefaultIfEmpty()
                        where !c.LimitedToStores || storeId == sm.StoreId
                        select c;
                }
                //only distinct material groups (group by ID)
                query = from c in query
                    group c by c.Id
                    into cGroup
                    orderby cGroup.Key
                    select cGroup.FirstOrDefault();
                query = query.OrderBy(c => c.ParentGroupId).ThenBy(c => c.DisplayOrder).ThenBy(c => c.Id);
            }
            */
            var unsortedGroups = query.ToList();

            //sort list
            var sortedGroups = unsortedGroups.SortGroupsForTree();

            //paging
            return new PagedList<MaterialGroup>(sortedGroups, pageIndex, pageSize);
        }

        /// <summary>
        /// Gets all material groups filtered by parent group identifier
        /// </summary>
        /// <param name="parentId">Parent group identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="includeAllLevels">A value indicating whether we should load all child levels</param>
        /// <returns>MaterialGroups</returns>
        public virtual IList<MaterialGroup> GetMaterialGroupsByParentGroupId(int parentId,
            bool showHidden = false, bool includeAllLevels = false)
        {
            var key = string.Format(MATERIAL_GROUPS_BY_PARENT_ID_KEY, parentId, showHidden, _workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id, includeAllLevels);
            return _cacheManager.Get(key, () =>
            {
                var query = _materialGroupRepository.Table;
                query = query.Where(c => c.ParentGroupId == parentId);
                query = query.Where(c => !c.Deleted);
                query = query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id);

                /*if (!showHidden && (!_catalogSettings.IgnoreAcl || !_catalogSettings.IgnoreStoreLimitations))
                {
                    if (!_catalogSettings.IgnoreAcl)
                    {
                        //ACL (access control list)
                        var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();
                        query = from c in query
                                join acl in _aclRepository.Table
                                on new { c1 = c.Id, c2 = "MaterialGroup" } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into c_acl
                                from acl in c_acl.DefaultIfEmpty()
                                where !c.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                                select c;
                    }
                    if (!_catalogSettings.IgnoreStoreLimitations)
                    {
                        //Store mapping
                        var currentStoreId = _storeContext.CurrentStore.Id;
                        query = from c in query
                                join sm in _storeMappingRepository.Table
                                on new { c1 = c.Id, c2 = "MaterialGroup" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into c_sm
                                from sm in c_sm.DefaultIfEmpty()
                                where !c.LimitedToStores || currentStoreId == sm.StoreId
                                select c;
                    }
                    //only distinct material groups (group by ID)
                    query = from c in query
                            group c by c.Id
                            into cGroup
                            orderby cGroup.Key
                            select cGroup.FirstOrDefault();
                    query = query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id);
                }
                */
                var groups = query.ToList();

                if (!includeAllLevels)
                    return groups;

                var childGroups = new List<MaterialGroup>();
                //add child levels
                foreach (var group in groups)
                {
                    childGroups.AddRange(GetMaterialGroupsByParentGroupId(group.Id, showHidden, true));
                }
                groups.AddRange(childGroups);
                return groups;
            });
        }

        /// <summary>
        /// Gets a material group
        /// </summary>
        /// <param name="materialGroupId">Group identifier</param>
        /// <returns>MaterialGroup</returns>
        public virtual MaterialGroup GetMaterialGroupById(int materialGroupId)
        {
            if (materialGroupId == 0)
                return null;
            
            var key = string.Format(MATERIAL_GROUPS_BY_ID_KEY, materialGroupId);
            return _cacheManager.Get(key, () => _materialGroupRepository.GetById(materialGroupId));
        }

        /// <summary>
        /// Inserts material group
        /// </summary>
        /// <param name="materialGroup">MaterialGroup</param>
        public virtual void InsertMaterialGroup(MaterialGroup materialGroup)
        {
            if (materialGroup == null)
                throw new ArgumentNullException(nameof(materialGroup));

            _materialGroupRepository.Insert(materialGroup);

            //cache
            _cacheManager.RemoveByPattern(MATERIAL_GROUPS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(materialGroup);
        }

        /// <summary>
        /// Updates the material group
        /// </summary>
        /// <param name="materialGroup">MaterialGroup</param>
        public virtual void UpdateMaterialGroup(MaterialGroup materialGroup)
        {
            if (materialGroup == null)
                throw new ArgumentNullException(nameof(materialGroup));

            //validate group hierarchy
            var parentGroup = GetMaterialGroupById(materialGroup.ParentGroupId);
            while (parentGroup != null)
            {
                if (materialGroup.Id == parentGroup.Id)
                {
                    materialGroup.ParentGroupId = 0;
                    break;
                }
                parentGroup = GetMaterialGroupById(parentGroup.ParentGroupId);
            }

            _materialGroupRepository.Update(materialGroup);

            //cache
            _cacheManager.RemoveByPattern(MATERIAL_GROUPS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(materialGroup);
        }

        /// <summary>
        /// Returns a list of names of not existing material groups
        /// </summary>
        /// <param name="groupNames">The names of the material groups to check</param>
        /// <returns>List of names not existing material groups</returns>
        public virtual string[] GetNotExistingGroups(string[] groupNames)
        {
            if (groupNames == null)
                throw new ArgumentNullException(nameof(groupNames));

            var query = _materialGroupRepository.Table;
            var queryFilter = groupNames.Distinct().ToArray();
            var filter = query.Select(c => c.Name).Where(c => queryFilter.Contains(c)).ToList();

            return queryFilter.Except(filter).ToArray();
        }

        /// <summary>
        /// Gets material groups by identifier
        /// </summary>
        /// <param name="groupIds">MaterialGroup identifiers</param>
        /// <returns>List of MaterialGroup</returns>
        public virtual List<MaterialGroup> GetMaterialGroupsByIds(int[] groupIds)
        {
            if (groupIds == null || groupIds.Length == 0)
                return new List<MaterialGroup>();

            var query = from p in _materialGroupRepository.Table
                where groupIds.Contains(p.Id) && !p.Deleted
                select p;
            
            return query.ToList();
        }

        /// <summary>
        /// Gets material collection by groupd id
        /// </summary>
        /// <param name="groupId">Group identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Material collection</returns>
        public IPagedList<Material> GetMaterialsByGroupId(int groupId,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            if (groupId == 0)
                return new PagedList<Material>(new List<Material>(), pageIndex, pageSize);

            var query = from mat in _materialRepository.Table
                        where mat.MaterialGroupId == groupId &&
                              !mat.Deleted && showHidden
                        orderby mat.DisplayOrder, mat.Id
                        select mat;

                var materials = new PagedList<Material>(query, pageIndex, pageSize);
                return materials;
        }

        /// <summary>
        /// Inserts a material into group
        /// </summary>
        /// <param name="groupId">>Id of group that material will be added into</param>
        /// <param name="materialId">>Id of material that will be added into group</param>
        public void InsertMaterialIntoGroup(int groupId, int materialId)
        {
            if (materialId == 0)
                throw new ArgumentException(nameof(materialId));
            var mat = _materialRepository.GetById(materialId);
            if (mat != null)
            {
                mat.MaterialGroupId = groupId;
                _materialRepository.Update(mat);
            }
        }

        /// <summary>
        /// Ungroup a material, set its group to default
        /// </summary>
        /// <param name="materialId">Id of material that will be ungrouped</param>
        public void UngroupMaterial(int materialId)
        {
            if (materialId == 0)
                throw new ArgumentException(nameof(materialId));
            var mat = _materialRepository.GetById(materialId);
            if (mat != null)
            {
                mat.MaterialGroupId = 1;
                _materialRepository.Update(mat);
            }
        }

        /// <summary>
        /// Import material groups from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        /*public void ImportManufacturersFromXlsx(Stream stream)
        {
            using (var xlPackage = new ExcelPackage(stream))
            {
                // get the first worksheet in the workbook
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    throw new NopException("No worksheet found");

                //the columns
                var properties = GetPropertiesByExcelCells<Manufacturer>(worksheet);

                var manager = new PropertyManager<Manufacturer>(properties);

                var iRow = 2;
                var setSeName = properties.Any(p => p.PropertyName == "SeName");

                while (true)
                {
                    var allColumnsAreEmpty = manager.GetProperties
                        .Select(property => worksheet.Cells[iRow, property.PropertyOrderPosition])
                        .All(cell => cell == null || cell.Value == null || string.IsNullOrEmpty(cell.Value.ToString()));

                    if (allColumnsAreEmpty)
                        break;

                    manager.ReadFromXlsx(worksheet, iRow);

                    var manufacturer = _manufacturerService.GetManufacturerById(manager.GetProperty("Id").IntValue);

                    var isNew = manufacturer == null;

                    manufacturer = manufacturer ?? new Manufacturer();

                    if (isNew)
                    {
                        manufacturer.CreatedOnUtc = DateTime.UtcNow;

                        //default values
                        manufacturer.PageSize = _catalogSettings.DefaultManufacturerPageSize;
                        manufacturer.PageSizeOptions = _catalogSettings.DefaultManufacturerPageSizeOptions;
                        manufacturer.Published = true;
                        manufacturer.AllowCustomersToSelectPageSize = true;
                    }

                    var seName = string.Empty;

                    foreach (var property in manager.GetProperties)
                    {
                        switch (property.PropertyName)
                        {
                            case "Name":
                                manufacturer.Name = property.StringValue;
                                break;
                            case "Description":
                                manufacturer.Description = property.StringValue;
                                break;
                            case "ManufacturerTemplateId":
                                manufacturer.ManufacturerTemplateId = property.IntValue;
                                break;
                            case "MetaKeywords":
                                manufacturer.MetaKeywords = property.StringValue;
                                break;
                            case "MetaDescription":
                                manufacturer.MetaDescription = property.StringValue;
                                break;
                            case "MetaTitle":
                                manufacturer.MetaTitle = property.StringValue;
                                break;
                            case "Picture":
                                var picture = LoadPicture(manager.GetProperty("Picture").StringValue, manufacturer.Name, isNew ? null : (int?)manufacturer.PictureId);

                                if (picture != null)
                                    manufacturer.PictureId = picture.Id;

                                break;
                            case "PageSize":
                                manufacturer.PageSize = property.IntValue;
                                break;
                            case "AllowCustomersToSelectPageSize":
                                manufacturer.AllowCustomersToSelectPageSize = property.BooleanValue;
                                break;
                            case "PageSizeOptions":
                                manufacturer.PageSizeOptions = property.StringValue;
                                break;
                            case "PriceRanges":
                                manufacturer.PriceRanges = property.StringValue;
                                break;
                            case "Published":
                                manufacturer.Published = property.BooleanValue;
                                break;
                            case "DisplayOrder":
                                manufacturer.DisplayOrder = property.IntValue;
                                break;
                            case "SeName":
                                seName = property.StringValue;
                                break;
                        }
                    }

                    manufacturer.UpdatedOnUtc = DateTime.UtcNow;

                    if (isNew)
                        _manufacturerService.InsertManufacturer(manufacturer);
                    else
                        _manufacturerService.UpdateManufacturer(manufacturer);

                    //search engine name
                    if (setSeName)
                        _urlRecordService.SaveSlug(manufacturer, manufacturer.ValidateSeName(seName, manufacturer.Name, true), 0);

                    iRow++;
                }

                //activity log
                _customerActivityService.InsertActivity("ImportManufacturers", _localizationService.GetResource("ActivityLog.ImportManufacturers"), iRow - 2);
            }
        }*/
        #endregion
    }
}
