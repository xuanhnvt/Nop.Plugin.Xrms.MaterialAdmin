using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Nop.Core.Data;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Web.Framework.Infrastructure;
using Nop.Plugin.Xrms.MaterialAdmin.Data;
using Nop.Data;
using Nop.Plugin.Xrms.MaterialAdmin.Domain;
using Nop.Plugin.Xrms.MaterialAdmin.Services;

namespace Nop.Plugin.Xrms.MaterialAdmin.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "nop_object_context_xrms_material_manager";
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            //data context
            this.RegisterPluginDataContext<MaterialManagerObjectContext>(builder, CONTEXT_NAME);

            builder.RegisterType<EfRepository<MaterialGroup>>()
            .As<IRepository<MaterialGroup>>()
            .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
            .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Material>>()
            .As<IRepository<Material>>()
            .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
            .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<ProductRecipe>>()
            .As<IRepository<ProductRecipe>>()
            .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
            .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<MaterialQuantityHistory>>()
            .As<IRepository<MaterialQuantityHistory>>()
            .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
            .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Supplier>>()
            .As<IRepository<Supplier>>()
            .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
            .InstancePerLifetimeScope();

            builder.RegisterType<MaterialGroupService>().As<IMaterialGroupService>().InstancePerLifetimeScope();
            builder.RegisterType<MaterialService>().As<IMaterialService>().InstancePerLifetimeScope();
            builder.RegisterType<SupplierService>().As<ISupplierService>().InstancePerLifetimeScope();
        }
        public int Order
        {
            get { return 1; }
        }
    }
}
