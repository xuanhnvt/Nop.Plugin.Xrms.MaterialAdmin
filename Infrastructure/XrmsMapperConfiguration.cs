using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Xrms.MaterialAdmin.Domain;
using Nop.Plugin.Xrms.MaterialAdmin.Areas.Admin.Models.MaterialGroups;
using Nop.Plugin.Xrms.MaterialAdmin.Areas.Admin.Models.Materials;

namespace Nop.Plugin.Xrms.MaterialAdmin.Infrastructure
{
    /// <summary>
    /// AutoMapper configuration for models
    /// </summary>
    public class XrmsMapperConfiguration : Profile, IMapperProfile
    {
        public XrmsMapperConfiguration()
        {
            #region MaterialGroup
            // from entity to view model
            CreateMap<MaterialGroup, MaterialGroupListItemViewModel>();
            CreateMap<MaterialGroup, MaterialGroupDetailsPageViewModel>();

            // from action model to entity
            CreateMap<MaterialGroupModel, MaterialGroup>();

            // from action model to view model
            CreateMap<MaterialGroupModel, MaterialGroupDetailsPageViewModel>();

            #endregion // MaterialGroup

            #region Material

            // from entity to view model
            CreateMap<Material, MaterialListItemViewModel>();
            CreateMap<Material, MaterialDetailsPageViewModel>();

            // from action model to entity
            CreateMap<MaterialModel, Material>();

            // from action model to view model
            CreateMap<MaterialModel, MaterialDetailsPageViewModel>();

            #endregion // Material

        }

        /// <summary>
        /// Order of this mapper implementation
        /// </summary>
        public int Order => 0;
    }
}
