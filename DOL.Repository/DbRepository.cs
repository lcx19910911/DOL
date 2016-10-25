using DOL.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using DOL.Core;

namespace DOL.Repository
{
    public class DbRepository : DbContext
    {

        public DbRepository()
           : base("name=DbRepository")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<DbRepository>());

            //this.Configuration.LazyLoadingEnabled = false;
            //this.Configuration.ProxyCreationEnabled = false;
            //this.Configuration.AutoDetectChangesEnabled = false;//关闭自动跟踪对象的属性变化
            //this.Configuration.ValidateOnSaveEnabled = false; //关闭保存时的实体验证
            //this.Configuration.UseDatabaseNullSemantics = false;//关闭数据库null比较行为


        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.WriteException(ex);
                //并发冲突数据
                if (ex.GetType() == typeof(DbUpdateConcurrencyException))
                {
                    return -1;
                }
                return 0;
            }

        }


        public DbSet<User> User { get; set; }

        public DbSet<DataDictionary> DataDictionary { get; set; }

        public DbSet<Department> Department { get; set; }

        public DbSet<Menu> Menu { get; set; }
        public DbSet<Role> Role { get; set; }

        public DbSet<Operate> Operate { get; set; }

        public DbSet<SiteMessage> SiteMessage { get; set; }

        public DbSet<Theme> Theme { get; set; }

        public DbSet<Reference> Reference { get; set; }

        public DbSet<EnteredPoint> EnteredPoint { get; set; }

        public DbSet<DriverShop> DriverShop { get; set; }

        public DbSet<Student> Student { get; set; }


        public DbSet<PayOrder> PayOrder { get; set; }
    }

}
