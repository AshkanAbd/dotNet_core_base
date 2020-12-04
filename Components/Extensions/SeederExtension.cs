using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace dotNet_base.Components.Extensions
{
    public abstract class SeederExtension
    {
        protected Random _random;
        protected string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        public bool Production = false;
        public IConfigurationRoot Configuration { get; set; }
        public IServiceCollection Services { get; set; }
        public ServiceProvider ServiceProvider { get; set; }

        public SeederExtension()
        {
            _random = new Random();
        }

        public void InitSerivces()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
            Services = new ServiceCollection();
        }

        public void SetupServices()
        {
            ServiceProvider = Services.BuildServiceProvider();
        }

        public virtual void Start(ModelBuilder modelBuilder)
        {
            var seeders = GetSeeders();
            foreach (var seeder in seeders) {
                var instance = (SeederExtension) Activator.CreateInstance(seeder);
                if (instance == null) {
                    continue;
                }

                var data = instance.Run();

                SetId(instance, data, modelBuilder);
                SetTimeStamps(instance, data);
                InsertData(instance, data, modelBuilder);
            }
        }

        public virtual void StartWithDbContext<T>(T dbContext) where T : DbContext
        {
            var seeders = GetSeeders();
            foreach (var seeder in seeders) {
                var instance = (SeederExtension) Activator.CreateInstance(seeder);

                if (instance == null) continue;

                var dbSetMethod = dbContext.GetType().GetMethod("Set")?
                    .MakeGenericMethod(instance.GetModelType());
                if (dbSetMethod == null) {
                    continue;
                }

                if (Production && !instance.IsProduction()) {
                    continue;
                }

                var dbSet = dbSetMethod.Invoke(dbContext, null);
                if (dbSet == null) continue;

                var countMethod = typeof(Queryable)
                    .GetMethods()
                    .FirstOrDefault(x => x.Name == "Count" && x.GetParameters().Length == 1)?
                    .MakeGenericMethod(instance.GetModelType());
                if (countMethod == null) continue;

                var count = (int) countMethod.Invoke(dbSet, new[] {dbSet});

                if (count == 0) {
                    instance.RunWithDbContext(dbContext);
                    Console.WriteLine(instance.GetModelType().Name + " seeded");
                }
                else {
                    Console.WriteLine(instance.GetModelType().Name + " already has data");
                }
            }
        }

        protected virtual void InsertData(SeederExtension instance, IEnumerable<object> datas,
            ModelBuilder modelBuilder)
        {
            modelBuilder.Entity(instance.GetModelType()).HasData(datas);
        }

        protected virtual void SetId(SeederExtension instance, IEnumerable<object> datas, ModelBuilder modelBuilder)
        {
            var id = 1;
            foreach (var data in datas) {
                var model = Convert.ChangeType(data, instance.GetModelType());

                var idProperty = model.GetType().GetProperty("Id"
                    , BindingFlags.Public | BindingFlags.Instance);
                if (idProperty != null && idProperty.CanWrite) {
                    idProperty.SetValue(model, id++, null);
                }
            }

            IncreaseAutoIncrement(instance, modelBuilder, id);
        }

        protected virtual void IncreaseAutoIncrement(SeederExtension instance, ModelBuilder modelBuilder, int lastId)
        {
            modelBuilder.Entity(instance.GetModelType()).Property("Id").HasIdentityOptions(lastId);
        }

        protected virtual void SetTimeStamps(SeederExtension instance, IEnumerable<object> datas)
        {
            SetCreatedAt(instance, datas);
            SetUpdatedAt(instance, datas);
        }

        protected virtual void SetCreatedAt(SeederExtension instance, IEnumerable<object> datas)
        {
            foreach (var data in datas) {
                var model = Convert.ChangeType(data, instance.GetModelType());

                var createdatProperty = model.GetType().GetProperty("CreatedAt"
                    , BindingFlags.Public | BindingFlags.Instance);
                if (createdatProperty != null && createdatProperty.CanWrite) {
                    createdatProperty.SetValue(model, DateTime.Now, null);
                }
            }
        }

        protected virtual void SetUpdatedAt(SeederExtension instance, IEnumerable<object> datas)
        {
            foreach (var data in datas) {
                var model = Convert.ChangeType(data, instance.GetModelType());

                var updatedatProperty = model.GetType().GetProperty("UpdatedAt"
                    , BindingFlags.Public | BindingFlags.Instance);
                if (updatedatProperty != null && updatedatProperty.CanWrite) {
                    updatedatProperty.SetValue(model, DateTime.Now, null);
                }
            }
        }

        protected virtual string GetTableName(Type type)
        {
            if (type.GetCustomAttributes(
                typeof(TableAttribute), true
            ).FirstOrDefault() is TableAttribute tableAttribute) {
                return tableAttribute.Name;
            }

            return null;
        }

        protected abstract object[] Run();
        protected abstract void RunWithDbContext<T>(T dbContext);

        protected virtual IEnumerable<Type> GetSeeders()
        {
            return GetModelType().Assembly.GetTypes()
                .Where(t => string.Equals(t.Namespace, GetModelType().Namespace, StringComparison.Ordinal))
                .Where(t => t.Name != GetModelType().Name)
                .Where(t => t.IsVisible)
                .ToArray();
        }

        protected abstract Type GetModelType();

        protected string RandomString(int length)
        {
            return new string(Enumerable.Repeat(_chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        protected virtual bool IsProduction()
        {
            return false;
        }
    }
}