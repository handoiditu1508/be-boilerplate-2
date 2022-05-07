using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq;

namespace Hamburger.Repository.EF.Extensions
{
    public static class DbContextExtension
    {
        /// <summary>
        /// Get table name for a specific type.
        /// </summary>
        /// <typeparam name="T">Entity type to get table name for.</typeparam>
        /// <param name="context">An DbContext instance.</param>
        /// <returns>Name of the table.</returns>
        public static string GetTableName<T>(this DbContext context) where T : class
        {
            // DbContext knows everything about the model.
            var model = context.Model;

            // Get all the entity types information contained in the DbContext class, ...
            var entityTypes = model.GetEntityTypes();

            // ... and get one by entity type information of "FooBars" DbSet property.
            var entityTypeOfT = entityTypes.First(t => t.ClrType == typeof(T));

            // The entity type information has the actual table name as an annotation!
            var tableNameAnnotation = entityTypeOfT.GetAnnotation("Relational:TableName");
            return tableNameAnnotation.Value.ToString();
        }
    }
}
