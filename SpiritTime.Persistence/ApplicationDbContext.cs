using System;
using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SpiritTime.Core.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace SpiritTime.Persistence
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TaskTagRule> TagRules { get; set; }
        public DbSet<Workspace> Workspaces { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var configuration = builder.Build();
            var connectionString = configuration["ConnectionStrings:DefaultConnection"];
            optionsBuilder.UseSqlServer(connectionString);
        }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //builder.Entity
            
            builder.Entity<Workspace>()
                .HasOne(x=>x.User)
                .WithMany(x=>x.Workspaces)
                .HasForeignKey(x=>x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<Tag>()
                .HasOne(x=>x.Workspace)
                .WithMany(x=>x.Tags)
                .HasForeignKey(x=>x.WorkspaceId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<TaskTagRule>()
                .HasOne(x=>x.Tag)
                .WithMany()
                .HasForeignKey(x=>x.TagId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<TaskTag>()
                .HasOne(x=>x.Tasks)
                .WithMany(x=>x.TaskTags)
                .HasForeignKey(x=>x.TaskId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<TaskTag>()
                .HasOne(x=>x.Tag)
                .WithMany(x=>x.TaskTags)
                .HasForeignKey(x=>x.TagId)
                .OnDelete(DeleteBehavior.NoAction);
            
            builder.Entity<Tasks>()
                .HasOne(x=>x.Workspace)
                .WithMany(x=>x.Tasks)
                .HasForeignKey(x=>x.WorkspaceId)
                .OnDelete(DeleteBehavior.Cascade);
            
        }
    }
}
