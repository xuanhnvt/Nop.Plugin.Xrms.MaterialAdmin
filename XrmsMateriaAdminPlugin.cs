using System;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Nop.Core.Plugins;
using Nop.Web.Framework;
using Nop.Web.Framework.Menu;

using Nop.Plugin.Xrms.MaterialAdmin.Data;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Plugin.Xrms.MaterialAdmin.Services;
using Nop.Plugin.Xrms.MaterialAdmin.Domain;

namespace Nop.Plugin.Xrms.MaterialAdmin
{
    public class XrmsMaterialAdminPlugin : BasePlugin, IAdminMenuPlugin
    {
        private readonly MaterialManagerObjectContext _objectContext;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IMaterialGroupService _materialGroupService;
        //private readonly IWorkContext _workContext;
        //private readonly IWebHelper _webHelper;

        public XrmsMaterialAdminPlugin(MaterialManagerObjectContext objectContext,
            ISettingService settingService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IMaterialGroupService materialGroupService)
        {
            _objectContext = objectContext;
            _settingService = settingService;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _materialGroupService = materialGroupService;
        }


        public override void Install()
        {
            _objectContext.Install();

            // add default permission
            _permissionService.InstallPermissions(new XrmsPermissionProvider());

            //locales
            /*this.AddOrUpdatePluginLocaleResource("Plugins.Api", "Api plugin");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Menu.ManageClients", "Manage Api Clients");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Configure", "Configure Web Api");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.GeneralSettings", "General Settings");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.EnableApi", "Enable Api");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.EnableApi.Hint", "By checking this settings you can Enable/Disable the Web Api");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.AllowRequestsFromSwagger", "Allow Requests From Swagger");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.AllowRequestsFromSwagger.Hint", "Swagger is the documentation generation tool used for the API (/Swagger). It has a client that enables it to make GET requests to the API endpoints. By enabling this option you will allow all requests from the swagger client. Do Not Enable on live site, it is only for demo sites or local testing!!!");
            */

            #region Material Groups

            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Notifications.Created", "The new material group has been created successfully.");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Notifications.Updated", "The material group has been updated successfully.");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Notifications.Deleted", "The material group has been deleted successfully.");

            this.AddOrUpdatePluginLocaleResource("Xrms.ActivityLog.AddNewMaterialGroup", "Added a new material group ('{0}')");
            this.AddOrUpdatePluginLocaleResource("Xrms.ActivityLog.EditMaterialGroup", "Edited a material group ('{0}')");
            this.AddOrUpdatePluginLocaleResource("Xrms.ActivityLog.DeleteMaterialGroup", "Deleted a material group ('{0}')");
            
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.List.Title", "Material Groups");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.List.Hints.ImportFromExcelTip", "Imported groups are distinguished by ID. If the ID already exists, then its corresponding group will be updated. You should not specify ID (leave 0) for new groups.");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.List.Search.MaterialGroupName", "Group name");

            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Details.Create.Title", "Create a new material group");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Details.Edit.Title", "Edit material group details");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Details.Buttons.BackToList", "back to material group list");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Details.Tabs.Info", "Material group info");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Details.Tabs.Materials", "Materials");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Details.Tabs.Materials.Columns.Material", "Material");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Details.Tabs.Materials.Buttons.RemoveMaterial", "Remove");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Details.Tabs.Materials.Buttons.AddMaterials", "Add Materials");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Details.Tabs.Materials.Hints.SaveBeforeEdit", "You need to save the group before you can add materials for this group page.");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Details.AddMaterialsPopup.Title", "Add materials into group");

            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Fields.Name", "Name");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Fields.Description", "Description");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Fields.Parent", "Parent group");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Fields.Parent.None", "None");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Fields.Picture", "Picture");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Fields.DisplayOrder", "Display Order");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Fields.AclCustomerRoles", "Limited to customer roles");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Fields.LimitedToStores", "Limited to stores");

            #endregion // Material Groups

            #region Materials

            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.Materials.Notifications.Created", "The new material has been created successfully.");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.Materials.Notifications.Updated", "The material has been updated successfully.");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.Materials.Notifications.Deleted", "The material has been deleted successfully.");

            this.AddOrUpdatePluginLocaleResource("Xrms.ActivityLog.AddNewMaterial", "Added a new material ('{0}')");
            this.AddOrUpdatePluginLocaleResource("Xrms.ActivityLog.EditMaterial", "Edited a material ('{0}')");
            this.AddOrUpdatePluginLocaleResource("Xrms.ActivityLog.DeleteMaterial", "Deleted a material ('{0}')");

            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.Materials.List.Search.MaterialName", "Material name");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.Materials.List.Search.MaterialGroup", "Material group");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.Materials.List.Search.IncludeSubGroup", "Search sub groups");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.Materials.List.Search.Supplier", "Supplier");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.Materials.List.Search.Warehouse", "Warehouse");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.Materials.List.Title", "Materials");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.Materials.List.Hints.ImportFromExcelTip", "Imported materials are distinguished by ID. If the ID already exists, then its corresponding material will be updated. You should not specify ID (leave 0) for new materials.");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.Materials.List.Buttons.DownloadPDF", "Download list as PDF");

            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.Materials.Details.Create.Title", "Create a new material");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.Materials.Details.Edit.Title", "Edit material details");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.Materials.Details.Buttons.BackToList", "back to material list");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.Materials.Details.Tabs.Info", "Material info");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.Materials.Details.Tabs.Info.Sections.CommonInfo", "General information");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.Materials.Details.Tabs.Info.Sections.Inventory", "Inventory");


            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.Materials.Fields.Name", "Name");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.Materials.Fields.Description", "Description");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.Materials.Fields.MaterialGroup", "Material Group");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.Materials.Fields.Picture", "Picture");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.Materials.Fields.DisplayOrder", "Display Order");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.Materials.Fields.Code", "Code");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.Materials.Fields.Supplier", "Manufacturer");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.Materials.Fields.Warehouse", "Warehouse");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.Materials.Fields.StockQuantity", "Stock Quantity");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.Materials.Fields.UsedQuantity", "Used Quantity");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.Materials.Fields.MinStockQuantity", "Minimum Quantity");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.Materials.Fields.Unit", "Unit");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.Materials.Fields.Cost", "Cost");
            this.AddOrUpdatePluginLocaleResource("Xrms.Admin.Catalog.Materials.Fields.AdminComment", "Admin comment");

            #endregion // Materials

            /*this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Page.Settings.Title", "Api Settings");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Page.Clients.Title", "Api Clients");

            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Page.Clients.Create.Title", "Add a new Api client");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Page.Clients.Edit.Title", "Edit Api client");

            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.Name", "Name");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.Name.Hint", "Name Hint");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.ClientId", "Client Id");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.ClientId.Hint", "The id of the client");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.ClientSecret", "Client Secret");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.ClientSecret.Hint", "The client secret is used during the authentication for obtaining the Access Token");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.CallbackUrl", "Callback Url");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.CallbackUrl.Hint", "The url where the Authorization code will be send");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.IsActive", "Is Active");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.IsActive.Hint", "You can use it to enable/disable the access to your store for the client");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.AddNew", "Add New Client");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.Edit", "Edit");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.Created", "Created");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.Deleted", "Deleted");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.Name", "Name is required");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.ClientId", "Client Id is required");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.ClientSecret", "Client Secret is required");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.CallbackUrl", "Callback Url is required");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Settings.GeneralSettingsTitle", "General Settings");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Edit", "Edit");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.BackToList", "Back To List");

            this.AddOrUpdatePluginLocaleResource("Api.Categories.Fields.Id.Invalid", "Id is invalid");
            this.AddOrUpdatePluginLocaleResource("Api.InvalidPropertyType", "Invalid Property Type");
            this.AddOrUpdatePluginLocaleResource("Api.InvalidType", "Invalid {0} type");
            this.AddOrUpdatePluginLocaleResource("Api.InvalidRequest", "Invalid request");
            this.AddOrUpdatePluginLocaleResource("Api.InvalidRootProperty", "Invalid root property");
            this.AddOrUpdatePluginLocaleResource("Api.NoJsonProvided", "No Json provided");
            this.AddOrUpdatePluginLocaleResource("Api.InvalidJsonFormat", "Json format is invalid");
            this.AddOrUpdatePluginLocaleResource("Api.Category.InvalidImageAttachmentFormat", "Invalid image attachment base64 format");
            this.AddOrUpdatePluginLocaleResource("Api.Category.InvalidImageSrc", "Invalid image source");
            this.AddOrUpdatePluginLocaleResource("Api.Category.InvalidImageSrcType", "You have provided an invalid image source/attachment ");

            this.AddOrUpdatePluginLocaleResource("Api.WebHooks.CouldNotRegisterWebhook", "Could not register WebHook due to error: {0}");
            this.AddOrUpdatePluginLocaleResource("Api.WebHooks.CouldNotRegisterDuplicateWebhook", "Could not register WebHook because a webhook with the same URI and Filters is already registered.");
            this.AddOrUpdatePluginLocaleResource("Api.WebHooks.CouldNotUpdateWebhook", "Could not update WebHook due to error: {0}");
            this.AddOrUpdatePluginLocaleResource("Api.WebHooks.CouldNotDeleteWebhook", "Could not delete WebHook due to error: {0}");
            this.AddOrUpdatePluginLocaleResource("Api.WebHooks.CouldNotDeleteWebhooks", "Could not delete WebHooks due to error: {0}");
            this.AddOrUpdatePluginLocaleResource("Api.WebHooks.InvalidFilters", "The following filters are not valid: '{0}'. A list of valid filters can be obtained from the path '{1}'.");

            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.EnableLogging", "Enable Logging");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.EnableLogging.Hint", "By enable logging you will see webhook messages in the Log. These messages are needed ONLY for diagnostic purposes. NOTE: A restart is required when changing this setting in order to take effect");*/

            this._materialGroupService.InsertMaterialGroup(new MaterialGroup()
            {
                Name = "Undefined",
                Description = "Use as defaut group. User could not delete or update info.",
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            });

            base.Install();
        }

        public override void Uninstall()
        {
            _objectContext.Uninstall();

            // remove permissions
            _permissionService.UninstallPermissions(new XrmsPermissionProvider());

            // TODO: Delete all resources
            //locales
            #region Material Groups

            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Notifications.Created");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Notifications.Updated");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Notifications.Deleted");

            this.DeletePluginLocaleResource("Xrms.ActivityLog.AddNewMaterialGroup");
            this.DeletePluginLocaleResource("Xrms.ActivityLog.EditMaterialGroup");
            this.DeletePluginLocaleResource("Xrms.ActivityLog.DeleteMaterialGroup");

            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.List.Title");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.List.Hints.ImportFromExcelTip");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.List.Search.MaterialGroupName");

            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Details.Create.Title");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Details.Buttons.BackToList");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Details.Edit.Title");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Details.Tabs.Info");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Details.Tabs.Materials");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Details.Tabs.Materials.Columns.Material");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Details.Tabs.Materials.Buttons.RemoveMaterial");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Details.Tabs.Materials.Hints.SaveBeforeEdit");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Details.Tabs.Materials.Buttons.AddMaterials");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Details.AddMaterialsPopup.Title");

            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Fields.Name");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Fields.Description");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Fields.Parent");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Fields.Parent.None");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Fields.Picture");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Fields.DisplayOrder");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Fields.AclCustomerRoles");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.MaterialGroups.Fields.LimitedToStores");

            #endregion // Material Groups

            #region Materials

            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.Materials.Notifications.Created");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.Materials.Notifications.Updated");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.Materials.Notifications.Deleted");

            this.DeletePluginLocaleResource("Xrms.ActivityLog.AddNewMaterial");
            this.DeletePluginLocaleResource("Xrms.ActivityLog.EditMaterial");
            this.DeletePluginLocaleResource("Xrms.ActivityLog.DeleteMaterial");

            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.Materials.List.Search.MaterialName");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.Materials.List.Search.MaterialGroup");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.Materials.List.Search.IncludeSubGroup");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.Materials.List.Search.Supplier");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.Materials.List.Search.Warehouse");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.Materials.List.Title");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.Materials.List.Hints.ImportFromExcelTip");

            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.Materials.Details.Create.Title");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.Materials.List.Buttons.DownloadPDF");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.Materials.Details.Buttons.BackToList");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.Materials.Details.Edit.Title");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.Materials.Details.Tabs.Info");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.Materials.Details.Tabs.Info.Sections.CommonInfo");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.Materials.Details.Tabs.Info.Sections.Inventory");

            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.Materials.Fields.Name");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.Materials.Fields.Description");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.Materials.Fields.MaterialGroup");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.Materials.Fields.Picture");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.Materials.Fields.DisplayOrder");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.Materials.Fields.Code");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.Materials.Fields.Supplier");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.Materials.Fields.Warehouse");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.Materials.Fields.StockQuantity");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.Materials.Fields.UsedQuantity");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.Materials.Fields.MinStockQuantity");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.Materials.Fields.Unit");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.Materials.Fields.Cost");
            this.DeletePluginLocaleResource("Xrms.Admin.Catalog.Materials.Fields.AdminComment");

            #endregion // Materials


            /*this.DeletePluginLocaleResource("Plugins.Api");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Menu.ManageClients");

            this.DeletePluginLocaleResource("Plugins.Api.Admin.Menu.Title");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Menu.Settings.Title");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Menu.Clients.Title");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Menu.Docs.Title");

            this.DeletePluginLocaleResource("Plugins.Api.Admin.Configure");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.GeneralSettings");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.EnableApi");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.EnableApi.Hint");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.AllowRequestsFromSwagger");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.AllowRequestsFromSwagger.Hint");

            this.DeletePluginLocaleResource("Plugins.Api.Admin.Client.Name");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Client.ClientId");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Client.ClientSecret");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Client.CallbackUrl");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Client.IsActive");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Client.AddNew");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Client.Edit");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Client.Created");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Client.Deleted");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.Name");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.ClientId");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.ClientSecret");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.CallbackUrl");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Settings.GeneralSettingsTitle");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Edit");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Client.BackToList");

            this.DeletePluginLocaleResource("Api.WebHooks.CouldNotRegisterWebhook");
            this.DeletePluginLocaleResource("Api.WebHooks.CouldNotRegisterDuplicateWebhook");
            this.DeletePluginLocaleResource("Api.WebHooks.CouldNotUpdateWebhook");
            this.DeletePluginLocaleResource("Api.WebHooks.CouldNotDeleteWebhook");
            this.DeletePluginLocaleResource("Api.WebHooks.CouldNotDeleteWebhooks");
            this.DeletePluginLocaleResource("Api.WebHooks.InvalidFilters");*/

            base.Uninstall();
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            string pluginDocumentationUrl = "https://github.com/SevenSpikes/api-plugin-for-nopcommerce";

            //var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Third party plugins");
            var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Catalog");
            if (pluginNode != null)
            {
                //pluginNode.ChildNodes.Add(menuItem);
                // do nothing
            }
            else
            {
                pluginNode = rootNode;
                //rootNode.ChildNodes.Add(menuItem);
            }
            
            pluginNode.ChildNodes.Add(new SiteMapNode()
            {
                SystemName = "Materials",
                Title = "Materials",
                //Url = pluginDocumentationUrl,
                ControllerName = "Material",
                ActionName = "List",
                IconClass = "fa-dot-circle-o",
                Visible = true,
                RouteValues = new RouteValueDictionary() { { "area", AreaNames.Admin } },
            });

            pluginNode.ChildNodes.Add(new SiteMapNode()
            {
                SystemName = "MaterialGroups",
                Title = "Material Groups",
                //Url = pluginDocumentationUrl,
                ControllerName = "MaterialGroup",
                ActionName = "List",
                IconClass = "fa-dot-circle-o",
                Visible = true,
                RouteValues = new RouteValueDictionary() { { "area", AreaNames.Admin } },
            });
        }
    }
}
