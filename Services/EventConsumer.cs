using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Payments;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.UI;

namespace Nop.Plugin.Xrms.MaterialAdmin.Services
{
    /// <summary>
    /// Represents event consumer of the Worldpay payment plugin
    /// </summary>
    public class EventConsumer : IConsumer<AdminTabStripCreated>
    {
        #region Fields

        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public EventConsumer(ILocalizationService localizationService)
        {
            this._localizationService = localizationService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle admin tabstrip created event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public void HandleEvent(AdminTabStripCreated eventMessage)
        {
            if (eventMessage?.Helper == null)
                return;

            //we need customer details page
            var tabsElementId = "product-edit";
            if (!eventMessage.TabStripName.Equals(tabsElementId))
                return;

            //compose script to create a new tab
            var productEditTabElementId = "tab-productEdit";
            var productEditTab = new HtmlString($@"
                <script type='text/javascript'>
                    $(document).ready(function() {{
                        $(`
                            <li>
                                <a data-tab-name='{productEditTabElementId}' data-toggle='tab' href='#{productEditTabElementId}'>
                                    {_localizationService.GetResource("Plugins.Payments.Worldpay.WorldpayCustomer")}
                                </a>
                            </li>
                        `).appendTo('#{tabsElementId} .nav-tabs:first');
                        $(`
                            <div class='tab-pane' id='{productEditTabElementId}'>
                                {
                                    //
                                    eventMessage.Helper.Partial("~/Plugins/Xrms.MaterialAdmin/Areas/Admin/Views/Material/_TabEditProduct.Recipe.cshtml").RenderHtmlContent()
                                        .Replace("</script>", "<\\/script>") //we need escape a closing script tag to prevent terminating the script block early
                                }
                            </div>
                        `).appendTo('#{tabsElementId} .tab-content:first');
                    }});
                </script>");

            //add this tab as a block to render on the customer details page
            eventMessage.BlocksToRender.Add(productEditTab);
        }

    #endregion
    }
}