using System.Collections.Generic;
using Nop.Core;
using Nop.Plugin.Xrms.MaterialAdmin.Domain;

namespace Nop.Plugin.Xrms.MaterialAdmin.Services
{
    /// <summary>
    /// MaterialGroup service interface
    /// </summary>
    public partial interface IMaterialGroupService
    {
        /// <summary>
        /// Delete material group
        /// </summary>
        /// <param name="MaterialGroup">MaterialGroup</param>
        void DeleteMaterialGroup(MaterialGroup materialGroup);

        /// <summary>
        /// Gets all material groups
        /// </summary>
        /// <param name="groupName">Group name</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>MaterialGroup</returns>
        IPagedList<MaterialGroup> GetAllMaterialGroups(string groupName = "",
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Gets all material groups filtered by parent group identifier
        /// </summary>
        /// <param name="parentId">Parent group identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="includeAllLevels">A value indicating whether we should load all child levels</param>
        /// <returns>MaterialGroups</returns>
        IList<MaterialGroup> GetMaterialGroupsByParentGroupId(int parentId,
            bool showHidden = false, bool includeAllLevels = false);

        /// <summary>
        /// Gets a material group
        /// </summary>
        /// <param name="materialGroupId">Group identifier</param>
        /// <returns>MaterialGroup</returns>
        MaterialGroup GetMaterialGroupById(int materialGroupId);

        /// <summary>
        /// Inserts material group
        /// </summary>
        /// <param name="materialGroup">MaterialGroup</param>
        void InsertMaterialGroup(MaterialGroup materialGroup);

        /// <summary>
        /// Updates the material group
        /// </summary>
        /// <param name="materialGroup">MaterialGroup</param>
        void UpdateMaterialGroup(MaterialGroup materialGroup);

        /// <summary>
        /// Returns a list of names of not existing material groups
        /// </summary>
        /// <param name="groupNames">The names of the material groups to check</param>
        /// <returns>List of names not existing material groups</returns>
        string[] GetNotExistingGroups(string[] groupNames);

        /// <summary>
        /// Gets material groups by identifier
        /// </summary>
        /// <param name="groupIds">MaterialGroup identifiers</param>
        /// <returns>List of MaterialGroup</returns>
        List<MaterialGroup> GetMaterialGroupsByIds(int[] groupIds);

        /// <summary>
        /// Gets a material group by material id
        /// </summary>
        /// <param name="materialId">Material id</param>
        /// <returns>MaterialGroup</returns>
        //MaterialGroup GetMaterialGroupByMaterialId(int materialId);

        /// <summary>
        /// Inserts a material into group
        /// </summary>
        /// <param name="groupId">>Id of group that material will be added into</param>
        /// <param name="materialId">>Id of material that will be added into group</param>
        void InsertMaterialIntoGroup(int groupId, int materialId);

        /// <summary>
        /// Removes a material from group
        /// </summary>
        /// <param name="groupId">Id of group that material will be removed from</param>
        /// <param name="materialId">Id of material that will be removed from group</param>
        //void RemoveMaterialFromGroup(int groupId, int materialId);
        // change group of material, will do in


        /// <summary>
        /// Gets material collection by groupd id
        /// </summary>
        /// <param name="groupId">Group identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Material collection</returns>
        IPagedList<Material> GetMaterialsByGroupId(int groupId,
        int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Ungroup a material, set its group to default
        /// </summary>
        /// <param name="materialId">Id of material that will be ungrouped</param>
        void UngroupMaterial(int materialId);
        // change group of material, will do in

    }
}
