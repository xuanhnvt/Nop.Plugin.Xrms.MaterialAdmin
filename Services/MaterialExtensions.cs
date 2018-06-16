using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Plugin.Xrms.MaterialAdmin.Domain;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Xrms.MaterialAdmin.Services
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class MaterialExtensions
    {
        /// <summary>
        /// Sort groups for tree representation
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="parentId">Parent group identifier</param>
        /// <param name="ignoreGroupssWithoutExistingParent">A value indicating whether groups without parent group in provided group list (source) should be ignored</param>
        /// <returns>Sorted groups</returns>
        public static IList<MaterialGroup> SortGroupsForTree(this IList<MaterialGroup> source, int parentId = 0, bool ignoreGroupssWithoutExistingParent = false)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var result = new List<MaterialGroup>();

            foreach (var cat in source.Where(c => c.ParentGroupId == parentId).ToList())
            {
                result.Add(cat);
                result.AddRange(SortGroupsForTree(source, cat.Id, true));
            }
            if (!ignoreGroupssWithoutExistingParent && result.Count != source.Count)
            {
                //find groups without parent in provided group source and insert them into result
                foreach (var cat in source)
                    if (result.FirstOrDefault(x => x.Id == cat.Id) == null)
                        result.Add(cat);
            }
            return result;
        }

        /// <summary>
        /// Get formatted group breadcrumb 
        /// Note: ACL and store mapping is ignored
        /// </summary>
        /// <param name="group">MaterialGroup</param>
        /// <param name="materialGroupService">MaterialGroup service</param>
        /// <param name="separator">Separator</param>
        /// <param name="languageId">Language identifier for localization</param>
        /// <returns>Formatted breadcrumb</returns>
        public static string GetFormattedBreadCrumb(this MaterialGroup group,
            IMaterialGroupService materialGroupService,
            string separator = ">>", int languageId = 0)
        {
            var result = string.Empty;

            var breadcrumb = GetMaterialGroupBreadCrumb(group, materialGroupService, null, null, true);
            for (var i = 0; i <= breadcrumb.Count - 1; i++)
            {
                var groupName = breadcrumb[i].GetLocalized(x => x.Name, languageId);
                result = string.IsNullOrEmpty(result)
                    ? groupName
                    : $"{result} {separator} {groupName}";
            }

            return result;
        }

        /// <summary>
        /// Get group breadcrumb 
        /// </summary>
        /// <param name="group">MaterialGroup</param>
        /// <param name="materialGroupService">MaterialGroup service</param>
        /// <param name="aclService">ACL service</param>
        /// <param name="storeMappingService">Store mapping service</param>
        /// <param name="showHidden">A value indicating whether to load hidden records</param>
        /// <returns>MaterialGroup breadcrumb </returns>
        public static IList<MaterialGroup> GetMaterialGroupBreadCrumb(this MaterialGroup group,
            IMaterialGroupService materialGroupService,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            bool showHidden = false)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));

            var result = new List<MaterialGroup>();

            //used to prevent circular references
            var alreadyProcessedMaterialGroupIds = new List<int>();

            while (group != null && //not null
                !group.Deleted && //not deleted
                //(showHidden || aclService.Authorize(group)) && //ACL
                //(showHidden || storeMappingService.Authorize(group)) && //Store mapping
                !alreadyProcessedMaterialGroupIds.Contains(group.Id)) //prevent circular references
            {
                result.Add(group);

                alreadyProcessedMaterialGroupIds.Add(group.Id);

                group = materialGroupService.GetMaterialGroupById(group.ParentGroupId);
            }
            result.Reverse();
            return result;
        }
    }
}
