using System.Reflection;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // البحث عن جميع الخصائص من نوع decimal وتعيين النوع "decimal(18, 2)" لها
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var decimalProperties = entityType.GetProperties()
                                               .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?));

            foreach (var property in decimalProperties)
            {
                property.SetColumnType("decimal(18, 2)");
            }
        }
    }
}
