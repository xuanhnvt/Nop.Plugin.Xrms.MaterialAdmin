using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Nop.Core;
using Nop.Data;
using Nop.Plugin.Xrms.MaterialAdmin.Domain;
using Nop.Plugin.Xrms.MaterialAdmin.Data.Mapping;
using Nop.Data.Mapping.Catalog;

namespace Nop.Plugin.Xrms.MaterialAdmin.Data
{
    public class MaterialManagerObjectContext : DbContext, IDbContext
    {
        public MaterialManagerObjectContext(string nameOrConnectionString) : base(nameOrConnectionString) { }

        #region Implementation of IDbContext

        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new MaterialGroupMap());

            modelBuilder.Configurations.Add(new MaterialMap());

            modelBuilder.Configurations.Add(new MaterialQuantityHistoryMap());

            modelBuilder.Configurations.Add(new SupplierMap());

            base.OnModelCreating(modelBuilder);
        }

        public string CreateDatabaseScript()
        {
            return ((IObjectContextAdapter)this).ObjectContext.CreateDatabaseScript();
        }

        public new IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }

        public void Install()
        {
            //create the table
            var dbScript = CreateDatabaseScript();
            Database.ExecuteSqlCommand(dbScript);
            SaveChanges();
        }

        public void Uninstall()
        {
            var tableName = this.GetTableName<MaterialQuantityHistory>();
            this.DropPluginTable(tableName);

            //drop the table
            tableName = this.GetTableName<Material>();
            this.DropPluginTable(tableName);

            tableName = this.GetTableName<MaterialGroup>();
            this.DropPluginTable(tableName);

            tableName = this.GetTableName<Supplier>();
            this.DropPluginTable(tableName);

        }

        public System.Collections.Generic.IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters) where TEntity : BaseEntity, new()
        {
            throw new System.NotImplementedException();
        }

        public System.Collections.Generic.IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            throw new System.NotImplementedException();
        }

        public int ExecuteSqlCommand(string sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters)
        {
            throw new System.NotImplementedException();
        }

        public void Detach(object entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            ((IObjectContextAdapter)this).ObjectContext.Detach(entity);
        }

        public virtual bool ProxyCreationEnabled
        {
            get
            {
                return Configuration.ProxyCreationEnabled;
            }
            set
            {
                Configuration.ProxyCreationEnabled = value;
            }
        }

        public virtual bool AutoDetectChangesEnabled
        {
            get
            {
                return Configuration.AutoDetectChangesEnabled;
            }
            set
            {
                Configuration.AutoDetectChangesEnabled = value;
            }
        }
    }
}
