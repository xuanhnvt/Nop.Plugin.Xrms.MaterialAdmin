using System;
using FluentValidation;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Plugin.Xrms.MaterialAdmin.Areas.Admin.Models.Materials;
using Nop.Plugin.Xrms.MaterialAdmin.Domain;

namespace Nop.Plugin.Xrms.MaterialAdmin.Areas.Admin.Validators
{
    public partial class MaterialValidator : BaseNopValidator<MaterialModel>
    {
        public MaterialValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Xrms.Admin.Catalog.Materials.Fields.Name.Required"));
            SetDatabaseValidationRules<MaterialGroup>(dbContext);
        }
    }
}
