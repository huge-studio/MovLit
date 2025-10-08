using Microsoft.EntityFrameworkCore;
using Oqtane.Modules;
using Oqtane.Repository;
using Oqtane.Repository.Databases.Interfaces;
using Huge.MovLit.Models;

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

            builder.Entity<Models.MyModule>().ToTable(ActiveDatabase.RewriteName("MyModule"));
            builder.Entity<Models.Story>().ToTable(ActiveDatabase.RewriteName("Story"));
            builder.Entity<Models.StoryView>().ToTable(ActiveDatabase.RewriteName("StoryView"));
            builder.Entity<Models.StoryLike>().ToTable(ActiveDatabase.RewriteName("StoryLike"));
            builder.Entity<Models.BlogPost>().ToTable(ActiveDatabase.RewriteName("Blog"));
        }
    }
}
