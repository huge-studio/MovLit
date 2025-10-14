using Huge.MovLit.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Oqtane.Modules;
using Oqtane.Repository;
using Oqtane.Repository.Databases.Interfaces;
using System.Collections.Generic;
using System.Text.Json;

namespace Huge.MovLit.Repository
{
    public class Context : DBContextBase, ITransientService, IMultiDatabase
    {
        public virtual DbSet<Models.MyModule> MyModule { get; set; }
        public virtual DbSet<Models.Story> Story { get; set; }
        public virtual DbSet<Models.StoryView> StoryView { get; set; }
        public virtual DbSet<Models.StoryLike> StoryLike { get; set; }
        public virtual DbSet<Models.BlogPost> Blog { get; set; }

        public Context(IDBContextDependencies DBContextDependencies) : base(DBContextDependencies)
        {
            // ContextBase handles multi-tenant database connections
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new StoryConfiguration());

            builder.Entity<Models.MyModule>().ToTable(ActiveDatabase.RewriteName("MyModule"));
            builder.Entity<Models.Story>().ToTable(ActiveDatabase.RewriteName("Story"));
            builder.Entity<Models.StoryView>().ToTable(ActiveDatabase.RewriteName("StoryView"));
            builder.Entity<Models.StoryLike>().ToTable(ActiveDatabase.RewriteName("StoryLike"));
            builder.Entity<Models.BlogPost>().ToTable(ActiveDatabase.RewriteName("Blog"));
        }

        public class StoryConfiguration : IEntityTypeConfiguration<Models.Story>
        {
            public void Configure(EntityTypeBuilder<Models.Story> builder)
            {
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                };

                var listToJson = new ValueConverter<List<string>, string>(
                    v => JsonSerializer.Serialize(v ?? new List<string>(), jsonOptions),
                    v => string.IsNullOrWhiteSpace(v)
                            ? new List<string>()
                            : JsonSerializer.Deserialize<List<string>>(v, jsonOptions) ?? new List<string>());

                builder.Property(x => x.Tags)
                    .HasColumnName("Tags")
                    .HasColumnType("NVARCHAR(MAX)")
                    .HasConversion(listToJson);
            }
        }
    }
}
