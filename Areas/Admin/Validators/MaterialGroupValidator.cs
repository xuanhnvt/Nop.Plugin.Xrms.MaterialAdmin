using System;
using FluentValidation;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Plugin.Xrms.MaterialAdmin.Areas.Admin.Models.MaterialGroups;
using Nop.Plugin.Xrms.MaterialAdmin.Domain;

namespace Nop.Plugin.Xrms.MaterialAdmin.Areas.Admin.Validators
{
    public partial class MaterialGroupValidator : BaseNopValidator<MaterialGroupModel>
    {
        public MaterialGroupValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Xrms.Admin.Catalog.MaterialGroups.Fields.Name.Required"));
            SetDatabaseValidationRules<MaterialGroup>(dbContext);
        }
    }
}
