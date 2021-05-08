namespace Server.Data
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DBContext : DbContext
    {
        public DBContext()
            : base("name=DBContext")
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<DBContext>());
        }

        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<Position> Position { get; set; }
        public virtual DbSet<Team> Teams { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Process> Processes { get; set; }
        public virtual DbSet<TeamProcess> TeamProcesses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .Property(e => e.user_name)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.full_name)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.logs)
                .WithRequired(e => e.employee)
                .HasForeignKey(e => e.id_employee)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Log>()
                .Property(e => e.name_process)
                .IsUnicode(false);

            modelBuilder.Entity<Position>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<Position>()
                .HasMany(e => e.employees)
                .WithRequired(e => e.position)
                .HasForeignKey(e => e.id_position)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Team>()
                .Property(e => e.team_name)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.user_name)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.full_name)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.position)
                .IsUnicode(false);

            modelBuilder.Entity<Process>()
                .HasMany(e => e.team_processes)
                .WithRequired(e => e.process)
                .HasForeignKey(e => e.id_process)
                .WillCascadeOnDelete(false);
        }
    }
}
