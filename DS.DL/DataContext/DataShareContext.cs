using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web;
using DS.Domain;
using DS.Domain.Base;
using Elmah;

namespace DS.DL.DataContext
{
    public class DataShareContext : DbContext
    {
        public DataShareContext()
        {
            Configuration.LazyLoadingEnabled = false;
        }

        //wire up the POCO objects to EF4
        public DbSet<Theme> Themes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SchemaESDFunctionServiceLink> SchemaESDFunctionServiceLink { get; set; }
        public DbSet<DataSetSchema> DataSetSchemas { get; set; }
        public DbSet<DataSetSchemaDefinition> DataSetSchemaDefinition { get; set; }
        public DbSet<DataSetSchemaColumn> DataSetSchemaColumns { get; set; }
        public DbSet<DebugInfo> DebugInfo { get; set; }
        public DbSet<SystemConfigurationObject> SystemConfigurationObjects { get; set; }
        //public DbSet<Group> Groups { get; set; }

        public DbSet<Contact> Contact { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>()
                .Property(p => p.Id)
                .HasColumnName("Id");
            modelBuilder.Entity<DataSetSchema>()
                .Property(p => p.Id)
                .HasColumnName("Id");

            modelBuilder.Entity<DataSetSchemaDefinition>()
                .Property(p => p.Id)
                .HasColumnName("Id");

            //modelBuilder.Entity<Group>()
            //    .HasMany(g => g.Categories)
            //    .WithMany(c => c.Groups)
            //    .Map(m => { m.ToTable("DS_CategoryGroups");
            //                m.MapLeftKey("GroupId");
            //                m.MapRightKey("CategoryId");
            //    });

            //modelBuilder.Entity<Group>()
            //    .HasMany(g => g.Schemas)
            //    .WithMany(s => s.Groups)
            //    .Map(m => { m.ToTable("DS_SchemaGroups");
            //                m.MapLeftKey("GroupId");
            //                m.MapRightKey("DataSetSchemaId");
            //    });

            //modelBuilder.Entity<Group>()
            //    .HasMany(g => g.Users)
            //    .WithRequired();

        }


        public override int SaveChanges()
        {
            //IUpdatable need to update Dates/UpdatedBy users etc
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if (!(entry.Entity is ITrackChanges)) continue;
                var e = ((ITrackChanges)(entry.Entity));
                switch (entry.State)
                {
                    case EntityState.Added:
                        if (e.DateCreated == DateTime.MinValue)
                        {
                            e.DateCreated = DateTime.Now;
                        }
                        e.DateUpdated = e.DateCreated;
                        e.CreatedBy = HttpContext.Current != null ? HttpContext.Current.User.Identity.Name : "WindowsService";
                        break;
                    case EntityState.Modified:
                        //if (e.DateUpdated == DateTime.MinValue)
                        e.DateUpdated = DateTime.Now;

                        e.UpdatedBy = HttpContext.Current != null ? HttpContext.Current.User.Identity.Name : "WindowsService"; ;
                        break;
                }
            }
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationError in dbEx.EntityValidationErrors.SelectMany(validationErrors => validationErrors.ValidationErrors).Where(validationError => HttpContext.Current != null))
                {
                    ErrorSignal.FromCurrentContext().Raise(new Exception(String.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage)));
                }
            }
            return 0;
        }
    }
}
