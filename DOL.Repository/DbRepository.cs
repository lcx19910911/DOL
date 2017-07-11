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

        /// <summary>
        /// 初始化对象
        /// </summary>
        /// <param name="entry">entry对象</param>
        private void InitObject(DbEntityEntry entry)
        {
            if (entry.Entity is BaseEntity)
            {
                var entity = entry.Entity as BaseEntity;
                switch (entry.State)
                {
                    case EntityState.Added:
                        //初始化这些值，如果这些值为null时，自动赋值
                        if (entity.CreatedTime == new DateTime())
                            entity.CreatedTime = DateTime.Now;
                        if (entity.UpdatedTime == new DateTime())
                            entity.UpdatedTime = DateTime.Now;
                        if (entity.ID.IsNullOrEmpty())
                            entity.ID = Guid.NewGuid().ToString("N");
                        break;
                    case EntityState.Modified:
                        entity.UpdatedTime = DateTime.Now;
                        break;
                }
            }
        }

        public override int SaveChanges()
        {
            try
            {
                var entries = from e in this.ChangeTracker.Entries()
                              where e.State != EntityState.Unchanged
                              select e;   //过滤所有修改了的实体，包括：增加 / 修改 / 删除


                //foreach (var entry in entries)
                //{

                //   InitObject(entry);
                //}
                if (entries.Count() == 0)
                    return 1;
                else
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

        public DbSet<Exam> Exam { get; set; }

        public DbSet<Reference> Reference { get; set; }

        public DbSet<EnteredPoint> EnteredPoint { get; set; }

        public DbSet<DriverShop> DriverShop { get; set; }

        public DbSet<Student> Student { get; set; }


        public DbSet<PayOrder> PayOrder { get; set; }

        public DbSet<Coach> Coach { get; set; }

        public DbSet<ThemeSalary> ThemeSalary { get; set; }

        public DbSet<Log> Log { get; set; }

        public DbSet<OilCard> OilCard { get; set; }

        public DbSet<Recharge> Recharge { get; set; }

        public DbSet<Waste> Waste { get; set; }

        public DbSet<Car> Car { get; set; }

        public DbSet<Expend> Expend { get; set; }
    }

}
